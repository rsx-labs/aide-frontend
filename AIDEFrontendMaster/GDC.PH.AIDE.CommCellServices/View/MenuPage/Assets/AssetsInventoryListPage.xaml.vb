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
Imports System.Data

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AssetsInventoryListPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private frame As Frame
    Private profile As New Profile
    Private page As String
    Private _addframe As New Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _OptionsViewModel As OptionViewModel
    Private _AideService As ServiceReference1.AideServiceClient
    Dim show As Boolean = True
    Dim lstAssets As Assets()
    Dim assetsDBProvider As New AssetsDBProvider
    Dim paginatedCollection As PaginatedObservableCollection(Of AssetsModel) = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _profile As Profile, addframe As Frame, menugrid As Grid, submenuframe As Frame, page As String)
        ' This call is required by the designer.
        pagingRecordPerPage = GetOptionData(28, 5, 12)
        paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

        InitializeComponent()

        frame = _frame
        Me.profile = _profile
        Me._addframe = addframe
        Me._menugrid = menugrid
        Me._submenuframe = submenuframe
        SetUnApprovedtTabVisible()

        If page = "Personal" Then
            SR.SelectedIndex = 0
        ElseIf page = "Update" Then
            SR.SelectedIndex = 1
        ElseIf page = "Approval" Then
            SR.SelectedIndex = 2
        End If

        PermissionSettings()
    End Sub
#End Region

#Region "Sub Procedures"

    Public Sub SetUnApprovedtTabVisible()
        If profile.Permission_ID = 1 Then
            Unapproved.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Public Sub LoadAssets()
        Try
            If InitializeService() Then
                If SR.SelectedIndex = 0 Then
                    lstAssets = _AideService.GetMyAssets(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                ElseIf SR.SelectedIndex = 1 Then
                    lstAssets = _AideService.GetAllAssetsInventoryByEmpID(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                Else
                    lstAssets = _AideService.GetAllAssetsInventoryUnApproved(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                End If

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
                assetsDBProvider.SetAssetInventoryList(objAssets)
            Next

            For Each assets As MyAssets In assetsDBProvider.GetAssetInventoryList()
                paginatedCollection.Add(New AssetsModel(assets))
            Next

            If SR.SelectedIndex = 0 Then
                lv_assetInventoryListOwn.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 1 Then
                lv_assetInventoryList.ItemsSource = paginatedCollection
            Else
                lv_assetInventoryListUnapproved.ItemsSource = paginatedCollection
            End If

            totalRecords = listAssets.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SearchAsset(search As String)
        Try
            Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(search.ToLower) _
                        Or i.MANUFACTURER.ToLower.Contains(search.ToLower) _
                        Or i.MODEL_NO.ToLower.Contains(search.ToLower) _
                        Or i.SERIAL_NO.ToLower.Contains(search.ToLower) _
                        Or i.ASSET_TAG.ToLower.Contains(search.ToLower) _
                        Or i.FULL_NAME.ToLower.Contains(search.ToLower) 'Or i.OTHER_INFO.ToLower.Contains(input.ToLower) 'Or i.STATUS_DESCR.ToLower.Contains(input.ToLower) 

            Dim searchAssets = New ObservableCollection(Of Assets)(items)

            SetLists(searchAssets.ToArray)

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

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
            txtAllPageNo.Text = "No Results Found "
            txtUnApprovePageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            txtAllPageNo.Text = "page " & currentPage & " of " & lastPage
            txtUnApprovePageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        lv_assetInventoryList.Visibility = Windows.Visibility.Hidden

        btnPrev2.IsEnabled = False
        btnNext2.IsEnabled = False
        btnPrev3.IsEnabled = False
        btnNext3.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_assetInventoryList.Visibility = Windows.Visibility.Visible

        btnPrev2.IsEnabled = True
        btnNext2.IsEnabled = True
        btnPrev3.IsEnabled = True
        btnNext3.IsEnabled = True
    End Sub

    Private Sub PermissionSettings()
        Dim guestAccount As Integer = 5

        If profile.Permission_ID = guestAccount Then
            Own.Visibility = Windows.Visibility.Collapsed
            SR.SelectedIndex = 1
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub txtSearch_TextChanged_1(sender As Object, e As TextChangedEventArgs)
        SearchAsset(txtSearch.Text.Trim)
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = lastPage
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

    'Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
    '    SetPaging(CInt(PagingMode._Next))
    'End Sub

    'Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
    '    SetPaging(CInt(PagingMode._Previous))
    'End Sub

    Private Sub SR_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SR.SelectionChanged
        e.Handled = True
        txtSearch.Clear()

        If SR.SelectedIndex = 0 Then
            page = "Personal"
        ElseIf SR.SelectedIndex = 1 Then
            page = "All"
            btnPrint.Visibility = Windows.Visibility.Visible
        Else
            page = "Approval"
        End If

        LoadAssets()
    End Sub

    Private Sub lv_assetInventoryListOwn_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles lv_assetInventoryListOwn.LoadingRow
        Dim RowDataContaxt As AssetsModel = TryCast(e.Row.DataContext, AssetsModel)

        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.EMP_ID = profile.Emp_ID And RowDataContaxt.STATUS = 2 And RowDataContaxt.APPROVAL = 5 And (profile.Permission_ID = 1 Or profile.Permission_ID = 4) Then
                e.Row.Background = New BrushConverter().ConvertFrom("#CCFFD8D8")
                e.Row.Foreground = New SolidColorBrush(Colors.Black)
                boxPersonal.Visibility = Windows.Visibility.Visible
                lblPersonal.Visibility = Windows.Visibility.Visible
                boxUnassigned.Visibility = Windows.Visibility.Visible
                lblUnassigned.Visibility = Windows.Visibility.Visible
            End If
        End If
    End Sub

    Private Sub lv_assetInventoryListOwn_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetInventoryListOwn.SelectionChanged
        e.Handled = True
        If lv_assetInventoryListOwn.SelectedIndex <> -1 Then

            If lv_assetInventoryListOwn.SelectedItem IsNot Nothing Then
                Dim assetsModel As New AssetsModel

                assetsModel.ASSET_ID = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).ASSET_ID
                assetsModel.EMP_ID = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).EMP_ID
                assetsModel.ASSET_DESC = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).ASSET_DESC
                assetsModel.MANUFACTURER = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).MANUFACTURER
                assetsModel.MODEL_NO = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).MODEL_NO
                assetsModel.SERIAL_NO = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).SERIAL_NO
                assetsModel.ASSET_TAG = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).ASSET_TAG
                assetsModel.DATE_ASSIGNED = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).DATE_ASSIGNED
                assetsModel.STATUS = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).STATUS
                assetsModel.COMMENTS = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).COMMENTS
                assetsModel.FULL_NAME = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).FULL_NAME
                assetsModel.ISAPPROVED = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).ISAPPROVED
                assetsModel.APPROVAL = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).APPROVAL
                assetsModel.PREVIOUS_ID = CType(lv_assetInventoryListOwn.SelectedItem, AssetsModel).PREVIOUS_ID
                _addframe.Navigate(New AssetsInventoryAddPage(assetsModel, frame, profile, "Personal", _addframe, _menugrid, _submenuframe))
                frame.IsEnabled = False
                frame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(150, 60, 150, 60)
                _addframe.Visibility = Visibility.Visible
            End If
        End If

    End Sub

    Private Sub lv_assetInventoryList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetInventoryList.SelectionChanged
        e.Handled = True
        If show = True Then
            If lv_assetInventoryList.SelectedIndex <> -1 And profile.Permission_ID = 4 Then 'allow only custodian to assign assets
                If lv_assetInventoryList.SelectedItem IsNot Nothing Then
                    If CType(lv_assetInventoryList.SelectedItem, AssetsModel).STATUS <> 1 And CType(lv_assetInventoryList.SelectedItem, AssetsModel).EMP_ID <> profile.Emp_ID Then
                        Exit Sub
                    Else
                        Dim assetsModel As New AssetsModel

                        assetsModel.ASSET_ID = CType(lv_assetInventoryList.SelectedItem, AssetsModel).ASSET_ID
                        assetsModel.EMP_ID = CType(lv_assetInventoryList.SelectedItem, AssetsModel).EMP_ID
                        assetsModel.ASSET_DESC = CType(lv_assetInventoryList.SelectedItem, AssetsModel).ASSET_DESC
                        assetsModel.MANUFACTURER = CType(lv_assetInventoryList.SelectedItem, AssetsModel).MANUFACTURER
                        assetsModel.MODEL_NO = CType(lv_assetInventoryList.SelectedItem, AssetsModel).MODEL_NO
                        assetsModel.SERIAL_NO = CType(lv_assetInventoryList.SelectedItem, AssetsModel).SERIAL_NO
                        assetsModel.ASSET_TAG = CType(lv_assetInventoryList.SelectedItem, AssetsModel).ASSET_TAG
                        assetsModel.DATE_ASSIGNED = CType(lv_assetInventoryList.SelectedItem, AssetsModel).DATE_ASSIGNED
                        assetsModel.STATUS = CType(lv_assetInventoryList.SelectedItem, AssetsModel).STATUS
                        assetsModel.OTHER_INFO = CType(lv_assetInventoryList.SelectedItem, AssetsModel).OTHER_INFO
                        assetsModel.FULL_NAME = CType(lv_assetInventoryList.SelectedItem, AssetsModel).FULL_NAME
                        _addframe.Navigate(New AssetsInventoryAddPage(assetsModel, frame, profile, "Update", _addframe, _menugrid, _submenuframe))
                        frame.IsEnabled = False
                        frame.Opacity = 0.3
                        _menugrid.IsEnabled = False
                        _menugrid.Opacity = 0.3
                        _submenuframe.IsEnabled = False
                        _submenuframe.Opacity = 0.3
                        _addframe.Margin = New Thickness(150, 60, 150, 60)
                        _addframe.Visibility = Visibility.Visible
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub lv_assetInventoryListUnapproved_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetInventoryListUnapproved.SelectionChanged
        e.Handled = True
        If lv_assetInventoryListUnapproved.SelectedIndex <> -1 Then

            If lv_assetInventoryListUnapproved.SelectedItem IsNot Nothing Then
                Dim assetsModel As New AssetsModel

                assetsModel.ASSET_ID = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).ASSET_ID
                assetsModel.EMP_ID = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).EMP_ID
                assetsModel.ASSET_DESC = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).ASSET_DESC
                assetsModel.MANUFACTURER = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).MANUFACTURER
                assetsModel.MODEL_NO = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).MODEL_NO
                assetsModel.SERIAL_NO = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).SERIAL_NO
                assetsModel.ASSET_TAG = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).ASSET_TAG
                assetsModel.DATE_ASSIGNED = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).DATE_ASSIGNED
                assetsModel.STATUS = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).STATUS
                assetsModel.OTHER_INFO = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).OTHER_INFO
                assetsModel.FULL_NAME = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).FULL_NAME
                assetsModel.PREVIOUS_ID = CType(lv_assetInventoryListUnapproved.SelectedItem, AssetsModel).PREVIOUS_ID
                _addframe.Navigate(New AssetsInventoryAddPage(assetsModel, frame, profile, "Approval", _addframe, _menugrid, _submenuframe))
                frame.IsEnabled = False
                frame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(150, 50, 150, 50)
                _addframe.Visibility = Visibility.Visible
                'frame.Navigate(New AssetsInventoryAddPage(assetsModel, frame, profile, "Approval"))
            End If
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()
        If lv_assetInventoryList.HasItems Then
            If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
                dialog.PrintTicket.PageOrientation = PageOrientation.Landscape

                Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
                lv_assetInventoryList.Measure(pageSize)
                lv_assetInventoryList.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
                dialog.PrintVisual(lv_assetInventoryList, "Print Asset Inventory")
            End If
        Else
            MsgBox("No Records Found!", MsgBoxStyle.Exclamation, "AIDE")
        End If
    End Sub

    Private Sub btnExport_Click(sender As Object, e As RoutedEventArgs) Handles btnExport.Click
        'Dim exportExcel As ExportToExcel = New ExportToExcel
        'Dim sheetName As String = "FAI HW Inventory"
        'Dim path As String = "C:\Program Files (x86)\GDC PH\AIDE CommCell\ExcelFiles"
        'Dim fileName As String = "\Hardware Inventory.xlsx"
        'Dim dt As DataTable = ConvertToDataTable()

        'exportExcel.WriteDataTableToExcel(dt, sheetName, path, fileName)
        'Try
        '    Dim path As String = "C:\Users\Admin\Documents\Excel Files"
        '    Dim exists As Boolean = Directory.Exists(path)

        '    If Not exists Then Directory.CreateDirectory(path)

        '    show = False
        '    lv_assetInventoryList.SelectAllCells()
        '    lv_assetInventoryList.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader

        '    ApplicationCommands.Copy.Execute(Nothing, lv_assetInventoryList)
        '    Dim resultat As String = CStr(Clipboard.GetData(DataFormats.CommaSeparatedValue))
        '    Dim result As String = CStr(Clipboard.GetData(DataFormats.Text))
        '    lv_assetInventoryList.UnselectAllCells()
        '    Dim file1 As StreamWriter = New System.IO.StreamWriter("C:\Program Files (x86)\GDC PH\AIDE CommCell\Excel Files\Assets Inventory.xls")
        '    file1.WriteLine(result.Replace(","c, " "c))
        '    file1.Close()
        '    MessageBox.Show("Exporting DataGrid data to Excel file created.xls")
        '    show = True
        '    'frame.Navigate(New AssetsInventoryListPage(frame, profile, _addframe, _menugrid, _submenuframe))
        'Catch ex As Exception
        '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        'End Try
    End Sub

    Private Function ConvertToDataTable()
        Dim dataTable As New DataTable()

        dataTable.Columns.Add("Name of Employee")
        dataTable.Columns.Add("Department")
        dataTable.Columns.Add("Manufacturer")
        dataTable.Columns.Add("Asset Type")
        dataTable.Columns.Add("Model No")
        dataTable.Columns.Add("Asset Tag")
        dataTable.Columns.Add("Serial Number")
        dataTable.Columns.Add("Date Assigned")
        dataTable.Columns.Add("Date Purchased")
        dataTable.Columns.Add("Comments and other additional information")

        For Each asset In paginatedCollection.Collections
            Dim newRow = dataTable.NewRow()

            newRow("Name of Employee") = asset.FULL_NAME
            newRow("Department") = asset.DEPARTMENT
            newRow("Manufacturer") = asset.MANUFACTURER
            newRow("Asset Type") = asset.ASSET_DESC
            newRow("Model No") = asset.MODEL_NO
            newRow("Asset Tag") = asset.ASSET_TAG
            newRow("Serial Number") = asset.SERIAL_NO
            newRow("Date Assigned") = asset.DATE_ASSIGNED
            newRow("Date Purchased") = asset.DATE_PURCHASED
            newRow("Comments and other additional information") = asset.COMMENTS

            dataTable.Rows.Add(newRow)
        Next

        Return dataTable
    End Function

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
