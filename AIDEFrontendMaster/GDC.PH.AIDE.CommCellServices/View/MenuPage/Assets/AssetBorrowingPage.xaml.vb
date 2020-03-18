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
Public Class AssetBorrowingPage
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

#Region "Fields"
    Private frame As Frame
    Private profile As New Profile
    Private page As String
    Private _addframe As New Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private client As AideServiceClient
    Private assetVM As New AssetsViewModel()

    Private _AideService As ServiceReference1.AideServiceClient
    Dim show As Boolean = True
    Dim guestAccount = 5
    Dim totalRecords As Integer
    Dim lstAssets As Assets()
    Dim paginatedCollection As PaginatedObservableCollection(Of AssetsModel) = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _profile As Profile, addframe As Frame, menugrid As Grid, submenuframe As Frame, page As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        frame = _frame
        Me.profile = _profile
        Me._addframe = addframe
        Me._menugrid = menugrid
        Me._submenuframe = submenuframe
        SetUnApprovedtTabVisible()
        PopulateComboBoxAssetID()
        If page = "Personal" Then
            SR.SelectedIndex = 2
        ElseIf page = "Update" Then
            SR.SelectedIndex = 1
        ElseIf page = "Borrow" Then
            SR.SelectedIndex = 2
        ElseIf page = "Request" Then
            SR.SelectedIndex = 3
        ElseIf page = "Return" Then
            SR.SelectedIndex = 4
        ElseIf page = "BorrowersLog" Then
            SR.SelectedIndex = 5
        ElseIf page = "Approval" Then
            SR.SelectedIndex = 6
        End If

    End Sub
#End Region

#Region "Sub Procedures"

    Public Sub SetUnApprovedtTabVisible()
        'If profile.Permission_ID = 1 Then
        '    Unapproved.Visibility = Windows.Visibility.Visible
        'End If
        If profile.Permission_ID <> 1 And profile.Permission_ID <> 4 Then
            AssetBorrowRequestList.Visibility = Windows.Visibility.Collapsed
            AssetReturnList.Visibility = Windows.Visibility.Collapsed
        End If
        btnPrint.Visibility = Windows.Visibility.Hidden

    End Sub

    Public Sub SetData()
        Try
            If InitializeService() Then
                If SR.SelectedIndex = 0 Then
                    lstAssets = _AideService.GetMyAssets(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                ElseIf SR.SelectedIndex = 1 Then
                    lstAssets = _AideService.GetAllAssetsInventoryByEmpID(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                ElseIf SR.SelectedIndex = 2 Then
                    lstAssets = _AideService.GetAllAssetsBorrowingByEmpID(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                ElseIf SR.SelectedIndex = 3 Then
                    lstAssets = _AideService.GetAllAssetsBorrowingRequestByEmpID(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                ElseIf SR.SelectedIndex = 4 Then
                    lstAssets = _AideService.GetAllAssetsReturnsByEmpID(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                ElseIf SR.SelectedIndex = 5 Then
                    lstAssets = _AideService.GetAssetBorrowersLog(profile.Emp_ID, 0)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                Else
                    lstAssets = _AideService.GetAllAssetsInventoryUnApproved(profile.Emp_ID)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                End If

                LoadData()
                totalRecords = lstAssets.Length
                DisplayPagingInfo()
                ' SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
            Dim assetsDBProvider As New AssetsDBProvider

            paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

            For Each objAssets As Assets In lstAssets
                assetsDBProvider.SetAssetInventoryList(objAssets)
            Next

            For Each assets As MyAssets In assetsDBProvider.GetAssetInventoryList()
                paginatedCollection.Add(New AssetsModel(assets))
            Next

            If SR.SelectedIndex = 0 Then
                lv_assetInventoryListOwn.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 1 Then
                lv_assetInventoryList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 2 Then
                lv_assetBorrowingList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 3 Then
                lv_assetBorrowingRequestList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 4 Then
                lv_assetReturnList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 5 Then
                lv_assetAssetBorrowersLog.ItemsSource = paginatedCollection
            Else
                lv_assetInventoryListUnapproved.ItemsSource = paginatedCollection
            End If
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstAssets.Length / pagingRecordPerPage)
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Public Sub LoadData()
    '    Try
    '        Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
    '        Dim assetsDBProvider As New AssetsDBProvider
    '        Dim assetsVM As New AssetsViewModel()

    '        Dim objAssets As New Assets()

    '        ' Set the MyLessonLearntList 
    '        For i As Integer = startRowIndex To lastRowIndex
    '            objAssets = lstAssets(i)
    '            assetsDBProvider.SetAssetInventoryList(objAssets)
    '        Next

    '        For Each rawUser As MyAssets In assetsDBProvider.GetAssetInventoryList()
    '            lstAssetsList.Add(New AssetsModel(rawUser))
    '        Next

    '        assetsVM.AssetInventoryList = lstAssetsList
    '        Me.DataContext = assetsVM
    '    Catch ex As Exception
    '       MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    'End Sub

    Public Sub SetDataForSearch(input As String)
        Try
            Dim assetsDBProvider As New AssetsDBProvider

            paginatedCollection = New PaginatedObservableCollection(Of AssetsModel)(pagingRecordPerPage)

            Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(input.ToLower) Or i.MANUFACTURER.ToLower.Contains(input.ToLower) _
                      Or i.MODEL_NO.ToLower.Contains(input.ToLower) Or i.SERIAL_NO.ToLower.Contains(input.ToLower) Or i.ASSET_TAG.ToLower.Contains(input.ToLower) _
                      Or i.FULL_NAME.ToLower.Contains(input.ToLower) 'Or i.OTHER_INFO.ToLower.Contains(input.ToLower) 'Or i.STATUS_DESCR.ToLower.Contains(input.ToLower) 

            'Dim items = From i In lstAssets Where i.ASSET_DESC.ToLower.Contains(input.ToLower) Or i.MANUFACTURER.ToLower.Contains(input.ToLower) _
            '          Or i.MODEL_NO.ToLower.Contains(input.ToLower) Or i.SERIAL_NO.ToLower.Contains(input.ToLower) Or i.ASSET_TAG.ToLower.Contains(input.ToLower) _
            '          Or i.FULL_NAME.ToLower.Contains(input.ToLower) 'Or i.OTHER_INFO.ToLower.Contains(input.ToLower) 'Or i.STATUS_DESCR.ToLower.Contains(input.ToLower) 
            Dim searchAssets = New ObservableCollection(Of Assets)(items)

            For Each assets As Assets In searchAssets
                assetsDBProvider.SetAssetInventoryList(assets)
            Next

            For Each assets As MyAssets In assetsDBProvider.GetAssetInventoryList()
                paginatedCollection.Add(New AssetsModel(assets))
            Next

            totalRecords = searchAssets.Count
            If SR.SelectedIndex = 0 Then
                lv_assetInventoryListOwn.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 1 Then
                lv_assetInventoryList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 2 Then
                lv_assetBorrowingList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 3 Then
                lv_assetBorrowingRequestList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 4 Then
                lv_assetReturnList.ItemsSource = paginatedCollection
            ElseIf SR.SelectedIndex = 5 Then
                lv_assetAssetBorrowersLog.ItemsSource = paginatedCollection
            Else
                lv_assetInventoryListUnapproved.ItemsSource = paginatedCollection
            End If
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
            txtAllPageNo.Text = "No Results Found "
            txtBorrowPageNo.Text = "No Results Found "
            txtBorrowRequestPageNo.Text = "No Results Found "
            txtReturnPageNo.Text = "No Results Found "
            txtBorrowersLogPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            txtAllPageNo.Text = "page " & currentPage & " of " & lastPage
            txtBorrowPageNo.Text = "page " & currentPage & " of " & lastPage
            txtBorrowRequestPageNo.Text = "page " & currentPage & " of " & lastPage
            txtReturnPageNo.Text = "page " & currentPage & " of " & lastPage
            txtBorrowersLogPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        lv_assetInventoryList.Visibility = Windows.Visibility.Hidden

        btnPrev2.IsEnabled = False
        btnNext2.IsEnabled = False

        btnPrev3.IsEnabled = False
        btnNext3.IsEnabled = False

        btnPrev4.IsEnabled = False
        btnNext4.IsEnabled = False

        btnPrev5.IsEnabled = False
        btnNext5.IsEnabled = False

        btnPrev6.IsEnabled = False
        btnNext6.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_assetInventoryList.Visibility = Windows.Visibility.Visible

        btnPrev2.IsEnabled = True
        btnNext2.IsEnabled = True

        btnPrev3.IsEnabled = True
        btnNext3.IsEnabled = True

        btnPrev4.IsEnabled = True
        btnNext4.IsEnabled = True

        btnPrev5.IsEnabled = True
        btnNext5.IsEnabled = True

        btnPrev6.IsEnabled = True
        btnNext6.IsEnabled = True
    End Sub
#End Region

#Region "Events"
    Private Sub txtSearch_TextChanged_1(sender As Object, e As TextChangedEventArgs)
        paginatedCollection.Clear()

        If txtSearch.Text = String.Empty Then
            SetData()
        Else
            SetDataForSearch(txtSearch.Text)
        End If
        e.Handled = True
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
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

    Private Sub btnFirst_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._First))
    End Sub

    Private Sub btnLast_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Last))
    End Sub

    Private Sub SR_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SR.SelectionChanged
        e.Handled = True
        paginatedCollection.Clear()
        txtSearch.Clear()

        If SR.SelectedIndex = 0 Then
            page = "Personal"
        ElseIf SR.SelectedIndex = 1 Then
            page = "All"
            btnPrint.Visibility = Windows.Visibility.Hidden
        ElseIf SR.SelectedIndex = 2 Then
            page = "Borrow"
            'btnPrint.Visibility = Windows.Visibility.Visible
        ElseIf SR.SelectedIndex = 3 Then
            page = "Request"
            'btnPrint.Visibility = Windows.Visibility.Visible
        ElseIf SR.SelectedIndex = 4 Then
            page = "Return"
            'btnPrint.Visibility = Windows.Visibility.Visible
        ElseIf SR.SelectedIndex = 5 Then
            page = "BorrowersLog"
            'btnPrint.Visibility = Windows.Visibility.Visible
        Else
            page = "Approval"
        End If

        SetData()
    End Sub

    Private Sub lv_assetInventoryListOwn_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles lv_assetInventoryListOwn.LoadingRow
        Dim RowDataContaxt As AssetsModel = TryCast(e.Row.DataContext, AssetsModel)

        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.EMP_ID = profile.Emp_ID And RowDataContaxt.STATUS = 2 And RowDataContaxt.APPROVAL = 1 And profile.Permission_ID = 1 Then
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

    Private Sub lv_assetBorrowingList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetBorrowingList.SelectionChanged
        e.Handled = True
        If show = True Then
            If lv_assetBorrowingList.SelectedIndex <> -1 And Not profile.Permission_ID = guestAccount Then 'allow only custodian to assign assets
                If lv_assetBorrowingList.SelectedItem IsNot Nothing Then
                    'If CType(lv_assetBorrowingList.SelectedItem, AssetsModel).STATUS <> 1 And CType(lv_assetBorrowingList.SelectedItem, AssetsModel).EMP_ID <> profile.Emp_ID Then
                    '    Exit Sub
                    'Else
                    Dim assetsModel As New AssetsModel

                    assetsModel.ASSET_ID = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).ASSET_ID
                    assetsModel.ASSET_BORROWING_ID = 0
                    assetsModel.EMP_ID = profile.Emp_ID
                    assetsModel.ASSET_DESC = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).ASSET_DESC
                    assetsModel.MANUFACTURER = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).MANUFACTURER
                    assetsModel.MODEL_NO = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).MODEL_NO
                    assetsModel.SERIAL_NO = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).SERIAL_NO
                    assetsModel.ASSET_TAG = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).ASSET_TAG
                    assetsModel.DATE_ASSIGNED = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).DATE_ASSIGNED
                    assetsModel.STATUS = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).STATUS
                    assetsModel.OTHER_INFO = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).OTHER_INFO
                    assetsModel.FULL_NAME = CType(lv_assetBorrowingList.SelectedItem, AssetsModel).FULL_NAME
                    _addframe.Navigate(New AssetsBorrowingAddPage(assetsModel, frame, profile, "Borrow", _addframe, _menugrid, _submenuframe))
                    frame.IsEnabled = False
                    frame.Opacity = 0.3
                    _menugrid.IsEnabled = False
                    _menugrid.Opacity = 0.3
                    _submenuframe.IsEnabled = False
                    _submenuframe.Opacity = 0.3
                    _addframe.Margin = New Thickness(150, 60, 150, 60)
                    _addframe.Visibility = Visibility.Visible
                    'End If
                End If
            End If
        End If
    End Sub

    Private Sub lv_assetBorrowingRequestList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetBorrowingRequestList.SelectionChanged
        e.Handled = True
        If show = True Then
            If lv_assetBorrowingRequestList.SelectedIndex <> -1 And (profile.Permission_ID = 4 Or profile.Permission_ID = 1) Then 'allow only custodian to assign assets
                If lv_assetBorrowingRequestList.SelectedItem IsNot Nothing Then
                    'If CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).STATUS <> 1 And CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).EMP_ID <> profile.Emp_ID Then
                    '    Exit Sub
                    'Else
                    Dim assetsModel As New AssetsModel

                    assetsModel.ASSET_ID = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).ASSET_ID
                    assetsModel.ASSET_BORROWING_ID = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).ASSET_BORROWING_ID
                    assetsModel.EMP_ID = profile.Emp_ID
                    assetsModel.ASSET_DESC = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).ASSET_DESC
                    assetsModel.MANUFACTURER = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).MANUFACTURER
                    assetsModel.MODEL_NO = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).MODEL_NO
                    assetsModel.SERIAL_NO = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).SERIAL_NO
                    assetsModel.ASSET_TAG = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).ASSET_TAG
                    assetsModel.DATE_ASSIGNED = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).DATE_ASSIGNED
                    assetsModel.DATE_BORROWED = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).DATE_BORROWED
                    assetsModel.DATE_RETURNED = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).DATE_RETURNED
                    assetsModel.STATUS = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).STATUS
                    assetsModel.OTHER_INFO = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).OTHER_INFO
                    assetsModel.FULL_NAME = CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).FULL_NAME
                    _addframe.Navigate(New AssetsBorrowingAddPage(assetsModel, frame, profile, "Request", _addframe, _menugrid, _submenuframe))
                    frame.IsEnabled = False
                    frame.Opacity = 0.3
                    _menugrid.IsEnabled = False
                    _menugrid.Opacity = 0.3
                    _submenuframe.IsEnabled = False
                    _submenuframe.Opacity = 0.3
                    _addframe.Margin = New Thickness(150, 60, 150, 60)
                    _addframe.Visibility = Visibility.Visible
                    'End If
                End If
            End If
        End If
    End Sub

    Private Sub lv_assetReturnList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lv_assetReturnList.SelectionChanged
        e.Handled = True
        If show = True Then
            If lv_assetReturnList.SelectedIndex <> -1 And (profile.Permission_ID = 4 Or profile.Permission_ID = 1) Then 'allow only custodian to assign assets
                If lv_assetReturnList.SelectedItem IsNot Nothing Then
                    'If CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).STATUS <> 1 And CType(lv_assetBorrowingRequestList.SelectedItem, AssetsModel).EMP_ID <> profile.Emp_ID Then
                    '    Exit Sub
                    'Else
                    Dim assetsModel As New AssetsModel

                    assetsModel.ASSET_ID = CType(lv_assetReturnList.SelectedItem, AssetsModel).ASSET_ID
                    assetsModel.ASSET_BORROWING_ID = CType(lv_assetReturnList.SelectedItem, AssetsModel).ASSET_BORROWING_ID
                    assetsModel.EMP_ID = profile.Emp_ID
                    assetsModel.ASSET_DESC = CType(lv_assetReturnList.SelectedItem, AssetsModel).ASSET_DESC
                    assetsModel.MANUFACTURER = CType(lv_assetReturnList.SelectedItem, AssetsModel).MANUFACTURER
                    assetsModel.MODEL_NO = CType(lv_assetReturnList.SelectedItem, AssetsModel).MODEL_NO
                    assetsModel.SERIAL_NO = CType(lv_assetReturnList.SelectedItem, AssetsModel).SERIAL_NO
                    assetsModel.ASSET_TAG = CType(lv_assetReturnList.SelectedItem, AssetsModel).ASSET_TAG
                    assetsModel.DATE_ASSIGNED = CType(lv_assetReturnList.SelectedItem, AssetsModel).DATE_ASSIGNED
                    assetsModel.DATE_BORROWED = CType(lv_assetReturnList.SelectedItem, AssetsModel).DATE_BORROWED
                    assetsModel.DATE_RETURNED = CType(lv_assetReturnList.SelectedItem, AssetsModel).DATE_RETURNED
                    assetsModel.STATUS = CType(lv_assetReturnList.SelectedItem, AssetsModel).STATUS
                    assetsModel.OTHER_INFO = CType(lv_assetReturnList.SelectedItem, AssetsModel).OTHER_INFO
                    assetsModel.FULL_NAME = CType(lv_assetReturnList.SelectedItem, AssetsModel).FULL_NAME
                    _addframe.Navigate(New AssetsBorrowingAddPage(assetsModel, frame, profile, "Return", _addframe, _menugrid, _submenuframe))
                    frame.IsEnabled = False
                    frame.Opacity = 0.3
                    _menugrid.IsEnabled = False
                    _menugrid.Opacity = 0.3
                    _submenuframe.IsEnabled = False
                    _submenuframe.Opacity = 0.3
                    _addframe.Margin = New Thickness(150, 60, 150, 60)
                    _addframe.Visibility = Visibility.Visible
                    'End If
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

    Public Sub PopulateComboBoxAssetID()
        Try
            Dim assetDBProvider As New AssetsDBProvider
            If InitializeService() Then
                ' For Asset ID Combobox
                lstAssets = _AideService.GetAllAssetsUnAssigned(profile.Emp_ID)
                Dim lstAssetsList As New ObservableCollection(Of AssetsModel)

                For Each objAsset As Assets In lstAssets
                    assetDBProvider.SetAssetList(objAsset)
                Next

                For Each rawUser As MyAssets In assetDBProvider.GetAssetList()
                    lstAssetsList.Add(New AssetsModel(rawUser))
                Next

                assetVM.AssetList = lstAssetsList
                'cbAssetID.ItemsSource = assetVM.AssetList

            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
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
