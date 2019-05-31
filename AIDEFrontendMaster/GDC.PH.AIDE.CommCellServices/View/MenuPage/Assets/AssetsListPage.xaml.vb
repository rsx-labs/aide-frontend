Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
Imports System.Drawing.Printing

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AssetsListPage
    Implements ServiceReference1.IAideServiceCallback

#Region "FIELDS"
    Private frame As Frame
    Private profile As New Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _AideService As ServiceReference1.AideServiceClient
    Dim lstAssets As Assets()

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
#End Region

#Region "CONSTRUCTOR"

    Public Sub New(_frame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        frame = _frame
        profile = _profile
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        SetData()
    End Sub
#End Region

#Region "EVENTS"
    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged

    End Sub

    Private Sub txtSearch_TextChanged_1(sender As Object, e As TextChangedEventArgs)
        SetDataForSearch(txtSearch.Text)
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        _addframe.Navigate(New AssetsAddPage(frame, profile, _addframe, _menugrid, _submenuframe))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 80, 150, 80)
        _addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub lv_assetList_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles lv_assetList.LoadingRow
        Dim RowDataContaxt As AssetsModel = TryCast(e.Row.DataContext, AssetsModel)

        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.STATUS_DESCR = "Unassigned" Then
                e.Row.Background = New BrushConverter().ConvertFrom("#FFC0FBD8")
            End If
        End If
    End Sub


    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()
        If lv_assetList.HasItems Then
            If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
                dialog.PrintTicket.PageOrientation = PageOrientation.Landscape

                Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
                lv_assetList.Measure(pageSize)
                lv_assetList.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
                dialog.PrintVisual(lv_assetList, "Print Success Register")
            End If
        Else
            MsgBox("No Records Found!", MsgBoxStyle.Exclamation, "AIDE")
        End If
    End Sub

    Private Sub lv_assetList_MouseDoubleClick_1(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If lv_assetList.SelectedIndex <> -1 Then

            If lv_assetList.SelectedItem IsNot Nothing Then
                Dim assetsModel As New AssetsModel

                assetsModel.ASSET_ID = CType(lv_assetList.SelectedItem, AssetsModel).ASSET_ID
                assetsModel.EMP_ID = CType(lv_assetList.SelectedItem, AssetsModel).EMP_ID
                assetsModel.ASSET_DESC = CType(lv_assetList.SelectedItem, AssetsModel).ASSET_DESC
                assetsModel.MANUFACTURER = CType(lv_assetList.SelectedItem, AssetsModel).MANUFACTURER
                assetsModel.MODEL_NO = CType(lv_assetList.SelectedItem, AssetsModel).MODEL_NO
                assetsModel.SERIAL_NO = CType(lv_assetList.SelectedItem, AssetsModel).SERIAL_NO
                assetsModel.ASSET_TAG = CType(lv_assetList.SelectedItem, AssetsModel).ASSET_TAG
                assetsModel.DATE_PURCHASED = CType(lv_assetList.SelectedItem, AssetsModel).DATE_PURCHASED
                assetsModel.STATUS = CType(lv_assetList.SelectedItem, AssetsModel).STATUS
                assetsModel.OTHER_INFO = CType(lv_assetList.SelectedItem, AssetsModel).OTHER_INFO
                _addframe.Navigate(New AssetsAddPage(assetsModel, frame, profile, _addframe, _menugrid, _submenuframe))
                frame.IsEnabled = False
                frame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(150, 80, 150, 80)
                _addframe.Visibility = Visibility.Visible
            End If
        End If
    End Sub
#End Region

#Region "METHODS"

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstAssets = _AideService.GetAllAssetsByEmpID(profile.Emp_ID)
                btnPrint.Visibility = Windows.Visibility.Hidden

                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
            Dim assetsDBProvider As New AssetsDBProvider
            Dim assetsVM As New AssetsViewModel()

            Dim objAssets As New Assets()

            ' Set the MyLessonLearntList 
            For i As Integer = startRowIndex To lastRowIndex
                objAssets = lstAssets(i)
                assetsDBProvider.SetAssetList(objAssets)
            Next

            For Each rawUser As MyAssets In assetsDBProvider.GetAssetList()
                lstAssetsList.Add(New AssetsModel(rawUser))
            Next

            assetsVM.AssetList = lstAssetsList
            Me.DataContext = assetsVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String)
        Try
            If InitializeService() Then
                lstAssets = _AideService.GetAllAssetsBySearch(profile.Emp_ID, input)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _AideService.Abort()
        End Try
        Return bInitialize
    End Function

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstAssets.Length

            Select Case mode
                Case CInt(PagingMode._Next)
                    ' Set the rows to be displayed if the total records is more than the (Record per Page * Page Index)
                    If totalRecords > (pagingPageIndex * pagingRecordPerPage) Then

                        ' Set the last row to be displayed if the total records is more than the (Record per Page * Page Index) + Record per Page
                        If totalRecords >= ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) Then
                            lastRowIndex = ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) - 1
                        Else
                            lastRowIndex = totalRecords - 1
                        End If

                        startRowIndex = pagingPageIndex * pagingRecordPerPage
                        pagingPageIndex += 1
                    Else
                        startRowIndex = (pagingPageIndex - 1) * pagingRecordPerPage
                        lastRowIndex = totalRecords - 1
                    End If
                    ' Bind data to the Data Grid
                    LoadData()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadData()
                    End If
                    Exit Select
                Case CInt(PagingMode._First)
                    If totalRecords > pagingRecordPerPage Then
                        pagingPageIndex = 2
                        SetPaging(CInt(PagingMode._Previous))
                    Else
                        pagingPageIndex = 1
                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)

                        If Not totalRecords = 0 Then
                            lastRowIndex = totalRecords - 1
                            LoadData()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstAssets.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()
        Dim pagingInfo As String

        ' If there has no data found
        If lstAssets.Length = 0 Then
            pagingInfo = "No Results Found "
            GUISettingsOff()
        Else
            pagingInfo = "Displaying " & startRowIndex + 1 & " to " & lastRowIndex + 1
            GUISettingsOn()
        End If

        'lblPagingInfo2.Content = pagingInfo

    End Sub

    Private Sub GUISettingsOff()
        lv_assetList.Visibility = Windows.Visibility.Hidden

        btnPrev2.IsEnabled = False
        btnNext2.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_assetList.Visibility = Windows.Visibility.Visible

        btnPrev2.IsEnabled = True
        btnNext2.IsEnabled = True
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Next))
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Previous))
    End Sub

    Private Sub btnFirst_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._First))
    End Sub

    Private Sub btnLast_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Last))
    End Sub
#End Region

#Region "ICallBack Function"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
