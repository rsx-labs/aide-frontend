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

#Region "Paging Declarations"
    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
#End Region

#Region "FIELDS"
    Private frame As Frame
    Private profile As New Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _AideService As ServiceReference1.AideServiceClient
    Dim totalRecords As Integer
    Dim lstAssets As Assets()
    Dim paginatedCollection As PaginatedObservableCollection(Of AssetsModel) = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
#End Region

#Region "CONSTRUCTOR"

    Public Sub New(_frame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
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
                LoadData()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadData()
        Try
            paginatedCollection.Clear()
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
            Dim assetsDBProvider As New AssetsDBProvider

            For Each objAssets As Assets In lstAssets
                assetsDBProvider.SetAssetList(objAssets)
            Next

            For Each assetList As MyAssets In assetsDBProvider.GetAssetList()
                paginatedCollection.Add(New AssetsModel(assetList))
            Next

            lv_assetList.ItemsSource = paginatedCollection
            'LoadDataForPrint()
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstAssets.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String)
        Try
            Dim assetsDBProvider As New AssetsDBProvider

            paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

            Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(input.ToLower) Or i.MANUFACTURER.ToLower.Contains(input.ToLower) _
                      Or i.MODEL_NO.ToLower.Contains(input.ToLower) Or i.SERIAL_NO.ToLower.Contains(input.ToLower) Or i.ASSET_TAG.ToLower.Contains(input.ToLower) _
                      Or i.FULL_NAME.ToLower.Contains(input.ToLower)
            Dim searchAssets = New ObservableCollection(Of Assets)(items)

            For Each assets As Assets In searchAssets
                assetsDBProvider.SetAssetList(assets)
            Next

            For Each assets As MyAssets In assetsDBProvider.GetAssetList()
                paginatedCollection.Add(New AssetsModel(assets))
            Next

            totalRecords = searchAssets.Count
            lv_assetList.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
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
        ' If there has no data found
        If lstAssets.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
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
        Dim totalRecords As Integer = lstAssets.Length

        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
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
