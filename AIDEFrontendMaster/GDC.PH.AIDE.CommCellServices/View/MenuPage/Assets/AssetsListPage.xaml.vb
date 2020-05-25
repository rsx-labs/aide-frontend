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
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "FIELDS"
    Private frame As Frame
    Private profile As New Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _AideService As ServiceReference1.AideServiceClient
    'Private _OptionsViewModel As OptionViewModel
    Dim lstAssets As Assets()
    Dim assetsDBProvider As New AssetsDBProvider
    Dim paginatedCollection As PaginatedObservableCollection(Of AssetsModel) = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
#End Region

#Region "CONSTRUCTOR"

    Public Sub New(_frame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        pagingRecordPerPage = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PAGING_ALIST)
        paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

        InitializeComponent()
        frame = _frame
        profile = _profile
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe

        Assets.SelectedIndex = 0

        PermissionSettings()
    End Sub
#End Region

#Region "METHODS"

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

    Public Sub LoadAssets()
        Try
            'If InitializeService() Then
            If Assets.SelectedIndex = 0 Then
                lstAssets = AideClient.GetClient().GetAllAssetsByEmpID(profile.Emp_ID)
                btnPrint.Visibility = Windows.Visibility.Hidden
                Else
                lstAssets = AideClient.GetClient().GetAllDeletedAssetsByEmpID(profile.Emp_ID)
                btnPrint.Visibility = Windows.Visibility.Hidden
                End If

                SetLists(lstAssets)
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetLists(listAssets As Assets())
        Try
            assetsDBProvider = New AssetsDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

            For Each objAssets As Assets In listAssets
                assetsDBProvider.SetAssetList(objAssets)
            Next

            For Each assetList As MyAssets In assetsDBProvider.GetAssetList()
                paginatedCollection.Add(New AssetsModel(assetList))
            Next

            If Assets.SelectedIndex = 0 Then
                lv_assetList.ItemsSource = paginatedCollection
            Else
                lv_assetDeletedList.ItemsSource = paginatedCollection
            End If

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
            Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(search.ToLower) _
                        Or i.MANUFACTURER.ToLower.Contains(search.ToLower) _
                        Or i.MODEL_NO.ToLower.Contains(search.ToLower) _
                        Or i.SERIAL_NO.ToLower.Contains(search.ToLower) _
                        Or i.ASSET_TAG.ToLower.Contains(search.ToLower) _
                        Or i.FULL_NAME.ToLower.Contains(search.ToLower) _
                        Or i.OTHER_INFO.ToLower.Contains(search.ToLower)  'Or i.STATUS_DESCR.ToLower.Contains(input.ToLower) 

            Dim searchAssets = New ObservableCollection(Of Assets)(items)

            SetLists(searchAssets.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

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

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '       Dim strData As String = String.Empty
    '       Try
    '           _OptionsViewModel = New OptionViewModel
    '           _OptionsViewModel.Service = _AideService
    '           If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
    '               For Each opt As OptionModel In _OptionsViewModel.OptionList
    '                   If Not opt Is Nothing Then
    '                       strData = opt.VALUE
    '                       Exit For
    '                   End If
    '               Next
    '           End If
    '       Catch ex As Exception
    '           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '       End Try
    '       Return strData
    '   End Function

    Private Sub PermissionSettings()
        If profile.Permission_ID = 4 Then 'Allow custodian only to add assets
            btnAdd.Visibility = Windows.Visibility.Visible
        End If
    End Sub
#End Region

#Region "Events"

    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        _addframe.Navigate(New AssetsAddPage(frame, profile, _addframe, _menugrid, _submenuframe))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 60, 150, 60)
        _addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        SearchAssets(txtSearch.Text.Trim)
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

    Private Sub lv_assetList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetList.SelectionChanged
        e.Handled = True
        If lv_assetList.SelectedIndex <> -1 And (profile.Permission_ID = 4 Or profile.Permission_ID = 1) Then
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
                assetsModel.FULL_NAME = CType(lv_assetList.SelectedItem, AssetsModel).FULL_NAME

                _addframe.Navigate(New AssetsUpdatePage(assetsModel, frame, profile, _addframe, _menugrid, _submenuframe))
                frame.IsEnabled = False
                frame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(150, 130, 150, 130)
                _addframe.Visibility = Visibility.Visible
            End If
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

    Private Sub Assets_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Assets.SelectionChanged
        LoadAssets()
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
