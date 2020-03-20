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
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "FIELDS"
    Private frame As Frame
    Private profile As New Profile
    Private _OptionsViewModel As OptionViewModel
    Private _AideService As ServiceReference1.AideServiceClient
    Dim lstAssets As Assets()
    Dim assetsDBProvider As New AssetsDBProvider
    Dim paginatedCollection As PaginatedObservableCollection(Of AssetsModel) = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
#End Region

#Region "CONSTRUCTOR"
    Public Sub New(_frame As Frame, _profile As Profile)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        frame = _frame
        profile = _profile
        pagingRecordPerPage = GetOptionData(29, 13, 12)
        paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
        
        LoadAssets()
        btnPrint.Visibility = Windows.Visibility.Hidden
    End Sub
#End Region

#Region "METHODS"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Public Sub LoadAssets()
        Try
            If InitializeService() Then
                lstAssets = _AideService.GetAllAssetsHistory(profile.Emp_ID)
                SetLists(lstAssets)
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetLists(listAssets As Assets())
        Try
            assetsDBProvider = New AssetsDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

            For Each objAssets As Assets In listAssets
                assetsDBProvider.SetAssetHistoryList(objAssets)
            Next

            For Each assetList As MyAssets In assetsDBProvider.GetAssetHistoryList()
                paginatedCollection.Add(New AssetsModel(assetList))
            Next

            lv_assetList.ItemsSource = paginatedCollection
            'LoadDataForPrint()
            totalRecords = listAssets.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SearchAssets(search As String)
        Try
            Dim searchAssets = New ObservableCollection(Of Assets)()

            If search.ToLower.Equals("approved") Then
                Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(search.ToLower) _
                            Or i.MANUFACTURER.ToLower.Contains(search.ToLower) _
                            Or i.MODEL_NO.ToLower.Contains(search.ToLower) _
                            Or i.SERIAL_NO.ToLower.Contains(search.ToLower) _
                            Or i.ASSET_TAG.ToLower.Contains(search.ToLower) _
                            Or i.FULL_NAME.ToLower.Contains(search.ToLower) _
                            Or i.STATUS_DESCR.ToLower.Equals("approved")
                searchAssets = New ObservableCollection(Of Assets)(items)
            Else
                Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(search.ToLower) _
                            Or i.MANUFACTURER.ToLower.Contains(search.ToLower) _
                            Or i.MODEL_NO.ToLower.Contains(search.ToLower) _
                            Or i.SERIAL_NO.ToLower.Contains(search.ToLower) _
                            Or i.ASSET_TAG.ToLower.Contains(search.ToLower) _
                            Or i.FULL_NAME.ToLower.Contains(search.ToLower) _
                            Or i.STATUS_DESCR.ToLower.Contains(search.ToLower)
                searchAssets = New ObservableCollection(Of Assets)(items)
            End If

            SetLists(searchAssets.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    
	Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
        Dim strData As String = String.Empty
        Try
            _OptionsViewModel = New OptionViewModel
            If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        strData = opt.VALUE
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return strData
    End Function

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If totalRecords = 0 Then
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

#Region "Events"
    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        SearchAssets(txtSearch.Text.Trim)
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
