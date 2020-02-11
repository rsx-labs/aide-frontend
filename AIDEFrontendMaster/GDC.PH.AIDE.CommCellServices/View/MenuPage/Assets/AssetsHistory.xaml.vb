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
Public Class AssetsHistory
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

    Private _AideService As ServiceReference1.AideServiceClient
    Dim lstAssets As Assets()
    Dim totalRecords As Integer
    Dim searchAssets = New ObservableCollection(Of Assets)
    Dim paginatedCollection As PaginatedObservableCollection(Of AssetsModel) = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
#End Region

#Region "CONSTRUCTOR"

    Public Sub New(_frame As Frame, _profile As Profile)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        frame = _frame
        profile = _profile
        SetData()
    End Sub
#End Region

#Region "EVENTS"
    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        SetDataForSearch(txtSearch.Text)
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

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = lstAssets.Count

        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
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

#Region "METHODS"

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstAssets = _AideService.GetAllAssetsHistory(profile.Emp_ID)
                btnPrint.Visibility = Windows.Visibility.Hidden
                LoadData()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
            Dim assetsDBProvider As New AssetsDBProvider

            For Each objAssets As Assets In lstAssets
                assetsDBProvider.SetAssetHistoryList(objAssets)
            Next

            For Each assetList As MyAssets In assetsDBProvider.GetAssetHistoryList()
                paginatedCollection.Add(New AssetsModel(assetList))
            Next

            lv_assetList.ItemsSource = paginatedCollection
            'LoadDataForPrint()
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstAssets.Length / pagingRecordPerPage)
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String)
        Try
            'Dim items
            Dim assetsDBProvider As New AssetsDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

            If input.ToLower.Equals("approved") Then
                Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(input.ToLower) Or i.MANUFACTURER.ToLower.Contains(input.ToLower) _
                      Or i.MODEL_NO.ToLower.Contains(input.ToLower) Or i.SERIAL_NO.ToLower.Contains(input.ToLower) Or i.ASSET_TAG.ToLower.Contains(input.ToLower) _
                      Or i.FULL_NAME.ToLower.Contains(input.ToLower) Or i.STATUS_DESCR.ToLower.Equals("approved")
                searchAssets = New ObservableCollection(Of Assets)(items)
            Else
                Dim items2 = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(input.ToLower) Or i.MANUFACTURER.ToLower.Contains(input.ToLower) _
                      Or i.MODEL_NO.ToLower.Contains(input.ToLower) Or i.SERIAL_NO.ToLower.Contains(input.ToLower) Or i.ASSET_TAG.ToLower.Contains(input.ToLower) _
                      Or i.FULL_NAME.ToLower.Contains(input.ToLower) Or i.STATUS_DESCR.ToLower.Contains(input.ToLower)
                searchAssets = New ObservableCollection(Of Assets)(items2)
            End If


            For Each assets As Assets In searchAssets
                assetsDBProvider.SetAssetHistoryList(assets)
            Next

            For Each assets As MyAssets In assetsDBProvider.GetAssetHistoryList()
                paginatedCollection.Add(New AssetsModel(assets))
            Next

            totalRecords = searchAssets.Count
            lv_assetList.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_assetList.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
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
