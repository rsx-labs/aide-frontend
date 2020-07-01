Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class BillabilityFilter
    Implements ServiceReference1.IAideServiceCallback
#Region "Fields"

    Private mainFrame As Frame
    Private fromPage As String
    Private approvalStatus As Integer
    'Private client As AideServiceClient
    Private reportsModel As New ReportsModel
    Private profile As New Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private dsplyByDiv As Integer = 1
    Private pageDefinition As String

    Dim lstAssets As Assets()
    Dim assetDBProvider As New AssetsDBProvider
    Dim assetVM As New AssetsViewModel()

    Dim lstNickname As Nickname()
    Dim nicknameVM As New NicknameViewModel()
    Dim empId As Integer = 0
    Dim status As Integer

    Dim fiscalyearVM As New SelectionListViewModel
    Dim _year As Integer

    Dim lstFiscalYear As FiscalYear()
#End Region

#Region "Constructor"
    Public Sub New(_reportsmodel As ReportsModel, mainFrame As Frame, _profile As Profile, _fromPage As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()

        Me.mainFrame = mainFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.profile = _profile
        Me.reportsModel = _reportsmodel
        Me.fromPage = _fromPage
        If Me.fromPage = "Approval" Then
            'cbStatus.IsEnabled = True
        End If

        If Me.fromPage = "Project" Then
            tbSuccessForm.Text = "Project Billability Filter"
        ElseIf Me.fromPage = "Employee" Then
            tbSuccessForm.Text = "Employee Billability Filter"
        End If


        Me.pageDefinition = "Borrow"
        LoadData()
        'LoadStatus()
        'cbNickname.IsEnabled = True
        AssignEvents()
        PopulateComboBoxAssetID()
        'ListOfCustodians()
        ListOfAssetType()
        PopulateComboBoxAssignedTo()
        ListOfAssetManufacturer()
        LoadYear()

        'txtEmpID.Text = _assetsModel.EMP_ID
        'Integer.TryParse(txtEmpID.Text, empId)

        'If assetsModel.STATUS = 4 And assetsModel.PREVIOUS_ID <> 0 Then
        '    status = 1
        'ElseIf assetsModel.STATUS = 3 And assetsModel.PREVIOUS_ID <> 0 Then
        '    status = 1
        'ElseIf assetsModel.STATUS = 4 Then
        '    status = 2
        'ElseIf assetsModel.STATUS = 3 And assetsModel.PREVIOUS_ID = 0 Then
        '    status = 1
        'End If
        txtBtnUpdate.Text = "GENERATE REPORT"
        'If fromPage = "Borrow" Then
        '    txtBtnUpdate.Text = "SEND BORROW REQUEST"
        'ElseIf fromPage = "Request" Then
        '    txtBtnUpdate.Text = "APPROVE"
        'ElseIf fromPage = "Return" Then
        '    txtBtnUpdate.Text = "RETURN"
        'End If

        'dateStart1.IsEnabled = True
        'dateStart2.IsEnabled = True
    End Sub

#End Region

#Region "Events"
    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        Try
            Dim result As Integer
            Dim fiscalYear As String = "FY" + cbYear.SelectedValue.Substring(0, 4)
            If fromPage = "Employee" Then



                Dim process1 As System.Diagnostics.Process = Nothing
                Dim processStartInfo As System.Diagnostics.ProcessStartInfo
                processStartInfo = New System.Diagnostics.ProcessStartInfo()
                processStartInfo.FileName = reportsModel.FILE_PATH 'Use the the full Pathname of the program
                processStartInfo.Verb = "runas"
                processStartInfo.Arguments = fiscalYear
                processStartInfo.UseShellExecute = False
                processStartInfo.RedirectStandardOutput = True
                processStartInfo.CreateNoWindow = True 'Dont show the cmd window when the program is running
                processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                process1 = System.Diagnostics.Process.Start(processStartInfo)
                process1.Start()

                'Read the text from the cmd window
                Dim output As String = process1.StandardOutput.ReadToEnd()

                'Check to see if the text contains whatever the program shows in the cmd window to show it was successful
                If output.Contains("Report generated") Then 'Replace text with the text that shows it was successful in the command prompt
                    result = MsgBox("Report has been downloaded. Open the Report Now?", MessageBoxButton.YesNo, "AIDE")
                    If result = 6 Then
                        Process.Start("C:\GeneratedReports\Retail Services Employee Billability.xlsx")
                    End If
                Else
                    'blocker_failed.Show()

                End If

                process1.Dispose()

            ElseIf fromPage = "Project" Then

                Dim process1 As System.Diagnostics.Process = Nothing
                Dim processStartInfo As System.Diagnostics.ProcessStartInfo
                processStartInfo = New System.Diagnostics.ProcessStartInfo()
                processStartInfo.FileName = reportsModel.FILE_PATH 'Use the the full Pathname of the program
                processStartInfo.Verb = "runas"
                processStartInfo.Arguments = fiscalYear
                processStartInfo.UseShellExecute = False
                processStartInfo.RedirectStandardOutput = True
                processStartInfo.CreateNoWindow = True 'Dont show the cmd window when the program is running
                processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                process1 = System.Diagnostics.Process.Start(processStartInfo)
                process1.Start()

                'Read the text from the cmd window
                Dim output As String = process1.StandardOutput.ReadToEnd()

                'Check to see if the text contains whatever the program shows in the cmd window to show it was successful
                If output.Contains("Report generated") Then 'Replace text with the text that shows it was successful in the command prompt
                    result = MsgBox("Report has been downloaded. Open the Report Now?", MessageBoxButton.YesNo, "AIDE")
                    If result = 6 Then
                        Process.Start("C:\GeneratedReports\Retail Services Project Billability.xlsx")
                    End If
                Else
                    'blocker_failed.Show()

                End If

                process1.Dispose()
            End If

        


            'mainFrame.Navigate(New ReportsMainPage(mainFrame, profile, _addframe, _menugrid, _submenuframe))
            'mainFrame.IsEnabled = True
            'mainFrame.Opacity = 1
            '_menugrid.IsEnabled = True
            '_menugrid.Opacity = 1
            '_submenuframe.IsEnabled = True
            '_submenuframe.Opacity = 1

            '_addframe.Visibility = Visibility.Hidden
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
    '    Try
    '        e.Handled = True
    '        Dim assets As New Assets
    '        If CheckMissingField() Then
    '            MsgBox("Please Fill up the Fields!", MsgBoxStyle.Exclamation, "AIDE")
    '        Else
    '            assets.EMP_ID = Integer.Parse(txtEmpID.Text)
    '            assets.ASSET_ID = Integer.Parse(txtID.Text)
    '            assets.DATE_ASSIGNED = Date.Parse(dateInput.SelectedDate)
    '            assets.STATUS = cbStatus.SelectedIndex + 1
    '            assets.COMMENTS = txtComments.Text
    '            assets.ASSET_DESC = txtAssetDescr.Text
    '            assets.MANUFACTURER = txtManufacturer.Text
    '            assets.MODEL_NO = txtModel.Text
    '            assets.SERIAL_NO = txtSerial.Text
    '            assets.ASSET_TAG = txtAssetTag.Text
    '            Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
    '            If result = 1 Then
    '                If InitializeService() Then
    '                    client.InsertAssetsInventory(assets)
    '                    ClearFields()
    '                    mainFrame.Navigate(New AssetsInventoryListPage(mainFrame, profile))
    '                End If
    '            Else
    '                Exit Sub
    '            End If
    '        End If
    '    Catch ex As Exception
    '         MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    'End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        mainFrame.Navigate(New ReportsMainPage(mainFrame, profile, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden

    End Sub

    Private Sub btnDisapprove_Click(sender As Object, e As RoutedEventArgs) Handles btnDisapprove.Click
        Try
            e.Handled = True

            Approval(10, 2)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Private Sub btnApprove_Click(sender As Object, e As RoutedEventArgs) Handles btnApprove.Click
    '    Try
    '        e.Handled = True
    '        Approval(11, 1)

    '    Catch ex As Exception
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    'End Sub

    'Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
    '    Try

    '        e.Handled = True
    '        Dim assets As New Assets
    '        assets.EMP_ID = profile.Emp_ID
    '        'assets.ASSET_ID = Integer.Parse(txtID.Text)
    '        assets.DATE_ASSIGNED = Date.Now
    '        'assets.COMMENTS = txtComments.Text
    '        assets.ASSET_DESC = cbAssetType.Text
    '        assets.MANUFACTURER = cbAssetManufacturer.Text
    '        assets.MODEL_NO = txtModel.Text
    '        assets.SERIAL_NO = txtSerial.Text
    '        'assets.ASSET_TAG = txtAssetTag.Text
    '        assets.STATUS = 1
    '        assets.APPROVAL = 1
    '        assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL


    '        If InitializeService() Then
    '            client.UpdateAssetsInventoryCancel(assets)
    '            MsgBox("Asset has been updated.", MsgBoxStyle.Information, "AIDE")
    '            ClearFields()
    '            mainFrame.Navigate(New AssetBorrowingPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
    '            mainFrame.IsEnabled = True
    '            mainFrame.Opacity = 1
    '            _menugrid.IsEnabled = True
    '            _menugrid.Opacity = 1
    '            _submenuframe.IsEnabled = True
    '            _submenuframe.Opacity = 1

    '            _addframe.Visibility = Visibility.Hidden
    '        End If
    '    Catch ex As Exception
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    'End Sub

    'Private Sub dateStart1_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateStart1.SelectedDateChanged
    '    e.Handled = True
    '    If dateStart1.SelectedDate > dateStart2.SelectedDate Then
    '        MsgBox("Please enter a valid date", MsgBoxStyle.Exclamation, "AIDE")
    '        'dateStart1.SelectedDate = Date.Now
    '    Else
    '        Exit Sub
    '    End If
    'End Sub

    'Private Sub dateStart2_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateStart2.SelectedDateChanged
    '    e.Handled = True
    '    If dateStart1.SelectedDate < dateStart2.SelectedDate Then
    '        MsgBox("Please enter a valid date", MsgBoxStyle.Exclamation, "AIDE")
    '        'dateStart1.SelectedDate = Date.Now
    '    Else
    '        Exit Sub
    '    End If
    'End Sub

    'Private Sub cbAssetID_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbAssetID.SelectionChanged
    '    'Dim q = assetVM.AssetList.Where(Function(X) X.ASSET_ID = cbAssetID.SelectedValue).FirstOrDefault()

    '    'If q IsNot Nothing Then
    '    '    txtID.Text = q.ASSET_ID
    '    '    cbAssetType.Text = q.ASSET_DESC
    '    '    cbAssetManufacturer.Text = q.MANUFACTURER
    '    '    txtModel.Text = q.MODEL_NO
    '    '    txtSerial.Text = q.SERIAL_NO
    '    '    txtAssetTag.Text = q.ASSET_TAG
    '    'End If
    'End Sub

    'Private Sub cbNickname_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbNickname.SelectionChanged
    '    'txtEmpID.Text = cbNickname.SelectedValue
    '    'Integer.TryParse(txtEmpID.Text, empId)
    'End Sub

    'Private Sub cbStatus_DropDownOpened(sender As Object, e As EventArgs) Handles cbStatus.DropDownOpened
    '    txtEmpName.Text = "Select employee *"
    '    txtEmpID.Clear()
    'End Sub

    'Private Sub cbStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbStatus.SelectionChanged
    '    PopulateComboBoxAssignedTo()
    'End Sub


#End Region

#Region "Functions"

    Public Sub LoadYear()
        Try
            'If InitializeService() Then
            lstFiscalYear = CommonUtility.Instance().FiscalYears 'AideClient.GetClient().GetAllFiscalYear()
            LoadFiscalYear()
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadFiscalYear()
        Try

            Dim lstFiscalYearList As New ObservableCollection(Of FiscalYearModel)
            Dim FYDBProvider As New SelectionListDBProvider

            For Each objFiscalYear As FiscalYear In lstFiscalYear
                FYDBProvider._setlistofFiscal(objFiscalYear)
            Next

            For Each rawFiscalYear As myFiscalYearSet In FYDBProvider._getobjFiscal()
                lstFiscalYearList.Add(New FiscalYearModel(rawFiscalYear))
            Next

            fiscalyearVM.ObjectFiscalYearSet = lstFiscalYearList
            cbYear.ItemsSource = fiscalyearVM.ObjectFiscalYearSet


        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub Approval(ByVal stat As Integer, ByVal approval As Integer)
        'Dim assets As New Assets
        'assets.EMP_ID = empId
        ''assets.ASSET_ID = Integer.Parse(txtID.Text)
        '' assets.COMMENTS = txtComments.Text
        'assets.TRANS_FG = 11
        'assets.ASSET_DESC = txtAssetType.Text
        'assets.MANUFACTURER = txtAssetManufacturer.Text
        'assets.MODEL_NO = txtModel.Text
        'assets.SERIAL_NO = txtSerial.Text
        ''assets.ASSET_TAG = txtAssetTag.Text
        'assets.ASSET_BORROWING_ID = Integer.Parse(txtAssetBorrowingID.Text)
        'assets.DATE_ASSIGNED = Date.Parse(Now)
        'assets.STATUS = stat
        'assets.APPROVAL = approval

        Dim result As Integer
        If approval = 1 Then
            result = MsgBox("Are you sure you want to Approve this asset borrowing request?", MessageBoxButton.YesNo, "AIDE")
        ElseIf approval = 2 Then
            result = MsgBox("Are you sure you want to Reject this asset borrowing request?", MessageBoxButton.YesNo, "AIDE")
        End If

        If result = 6 Then
            'If InitializeService() Then
            'client.InsertAssetsBorrowing(assets)

            If approval = 1 Then
                MsgBox("Asset borrowing request has been Approved.", MsgBoxStyle.Information, "AIDE")
            ElseIf approval = 2 Then
                MsgBox("Asset borrowing request has been Rejected.", MsgBoxStyle.Information, "AIDE")
            End If
            ClearFields()
            'mainFrame.Navigate(New AssetsInventoryListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
            mainFrame.Navigate(New AssetBorrowingPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
            mainFrame.IsEnabled = True
            mainFrame.Opacity = 1
            _menugrid.IsEnabled = True
            _menugrid.Opacity = 1
            _submenuframe.IsEnabled = True
            _submenuframe.Opacity = 1

            _addframe.Visibility = Visibility.Hidden
            'End If
        Else
            Exit Sub
        End If



    End Sub

    Private Sub AssignEvents()
        If fromPage = "Approval" Then
            'cbStatus.IsEnabled = True
            'cbNickname.IsEnabled = False
        End If
        'AddHandler btnApprove.Click, AddressOf btnApprove_Click
        'AddHandler btnCancel.Click, AddressOf btnCancel_Click
        AddHandler btnBack.Click, AddressOf btnBack_Click
        'AddHandler btnUpdate.Click, AddressOf btnUpdate_Click
        AddHandler btnDisapprove.Click, AddressOf btnDisapprove_Click
    End Sub 'Assign events to buttons

    Private Sub ClearFields()
        'txtAssetTag.Clear()
        'txtModel.Clear()
        'txtComments.Clear()
        'txtSerial.Clear()
        'd.Text = String.Empty
    End Sub

    Public Sub LoadStatus()
        'cbStatus.IsEnabled = True
        'cbStatus.DisplayMemberPath = "Text"
        'cbStatus.SelectedValuePath = "Value"
        'If profile.Permission_ID = 1 Then
        '    cbStatus.Items.Add(New With {.Text = "Unassigned", .Value = 1})
        '    cbStatus.Items.Add(New With {.Text = "Assigned", .Value = 2})
        'Else
        '    cbStatus.Items.Add(New With {.Text = "Unassigned", .Value = 4})
        '    cbStatus.Items.Add(New With {.Text = "Assigned", .Value = 3})
        'End If

        'If assetsModel.STATUS = 3 Then
        '    txtStatus.Text = "Partially Assigned"
        'ElseIf assetsModel.STATUS = 4 Then
        '    txtStatus.Text = "Partially Unassigned"
        'End If
    End Sub

    Private Sub LoadData()

        'If fromPage = "Borrow" Or fromPage = "Return" Then
        '    btnUpdate.Visibility = Windows.Visibility.Visible
        '    btnApprove.Visibility = Windows.Visibility.Collapsed
        '    btnDisapprove.Visibility = Windows.Visibility.Collapsed
        '    'btnCancel.Visibility = Windows.Visibility.Collapsed

        'ElseIf fromPage = "Request" Then
        '    btnUpdate.Visibility = Windows.Visibility.Collapsed
        '    btnApprove.Visibility = Windows.Visibility.Visible
        '    btnDisapprove.Visibility = Windows.Visibility.Visible

        'ElseIf fromPage = "Return" Then
        '    btnUpdate.Visibility = Windows.Visibility.Visible
        '    btnApprove.Visibility = Windows.Visibility.Collapsed
        '    btnDisapprove.Visibility = Windows.Visibility.Collapsed
        '    'Else
        '    '    If profile.Permission_ID = 1 OrElse profile.Permission_ID = 4 Then
        '    '        btnUpdate.Visibility = Windows.Visibility.Visible
        '    '        'btnCancel.Visibility = Windows.Visibility.Collapsed
        '    '        btnDisapprove.Visibility = Windows.Visibility.Collapsed
        '    '        btnApprove.Visibility = Windows.Visibility.Collapsed
        '    '    Else
        '    '        btnDisapprove.Visibility = Windows.Visibility.Collapsed
        '    '        btnApprove.Visibility = Windows.Visibility.Collapsed
        '    '        btnUpdate.Visibility = Windows.Visibility.Visible

        '    '        'btnCancel.Visibility = Windows.Visibility.Collapsed
        '    '    End If
        'End If

        'cbAssetID.IsEnabled = False
        'cbAssetType.Visibility = Windows.Visibility.Collapsed
        'cbAssetManufacturer.Visibility = Windows.Visibility.Collapsed
        'txtAssetType.Visibility = Windows.Visibility.Visible
        'txtAssetManufacturer.Visibility = Windows.Visibility.Visible
        'txtID.Text = assetsModel.ASSET_ID
        'txtEmpID.Text = assetsModel.EMP_ID
        'txtAssetType.Text = assetsModel.ASSET_DESC
        'txtAssetManufacturer.Text = assetsModel.MANUFACTURER
        'txtModel.Text = assetsModel.MODEL_NO
        'txtSerial.Text = assetsModel.SERIAL_NO
        'txtAssetTag.Text = assetsModel.ASSET_TAG
        'cbStatus.Tag = assetsModel.STATUS
        'cbStatus.SelectedIndex = assetsModel.STATUS - 1
        'cbAssetID.SelectedValue = assetsModel.ASSET_ID
        'txtEmpName.Text = assetsModel.FULL_NAME
        'txtComments.Text = assetsModel.COMMENTS
        'dateInput.Text = assetsModel.DATE_PURCHASED
        'txtAssetBorrowingID.Text = assetsModel.ASSET_BORROWING_ID

    End Sub

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        'DisplayText("Opening client service...")
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        client = New AideServiceClient(Context)
    '        client.Open()
    '        bInitialize = True
    '        'DisplayText("Service opened successfully...")
    '        'Return True
    '    Catch ex As SystemException
    '        client.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    Public Function CheckMissingField() As Boolean
        'If txtEmpID.Text = String.Empty Then
        '    Return True
        'Else
        '    Return False
        'End If
    End Function

    Public Sub PopulateComboBoxAssignedTo()
        Try
            'If InitializeService() Then
            '    If profile.Permission_ID = 2 Then ' User
            '        If cbStatus.SelectedIndex = 1 Then ' Assigned
            '            txtEmpID.Text = profile.Emp_ID
            '            cbNickname.IsEnabled = False
            '        Else ' Unassigned
            '            cbNickname.IsEnabled = True
            '            ListOfCustodians()
            '        End If
            '    Else ' Manager or Custodian
            '        If cbStatus.SelectedIndex = 1 Then ' Assigned
            '            ListOfAllUser()
            '        Else ' Unassigned
            '            cbNickname.IsEnabled = True
            '            ListOfCustodians()
            '        End If
            '    End If
            'End If
            ListOfAllUser()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ListOfCustodians()
        Dim lstMangers As Assets() = AideClient.GetClient().GetAllAssetsCustodian(profile.Emp_ID)
        Dim lstMangersList As New ObservableCollection(Of AssetsModel)
        Dim assetsDBProvider As New AssetsDBProvider
        Dim assetsVM As New AssetsViewModel()

        For Each objAssets As Assets In lstMangers
            assetsDBProvider.SetManagerList(objAssets)
        Next

        For Each rawUser As MyAssets In assetsDBProvider.GetManagerList()
            lstMangersList.Add(New AssetsModel(rawUser))
        Next

        assetsVM.AssetManagerList = lstMangersList

        'cbNickname.DataContext = assetsVM
        'cbNickname.ItemsSource = assetsVM.AssetManagerList
    End Sub

    Public Sub ListOfAllUser()
        lstNickname = AideClient.GetClient().ViewNicknameByDeptID(profile.Email_Address, dsplyByDiv)
        Dim lstNicknameList As New ObservableCollection(Of NicknameModel)
        Dim successRegisterDBProvider As New SuccessRegisterDBProvider

        For Each objLessonLearnt As Nickname In lstNickname
            successRegisterDBProvider.SetMyNickname(objLessonLearnt)
        Next

        For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
            lstNicknameList.Add(New NicknameModel(rawUser))
        Next

        nicknameVM.NicknameList = Nothing
        'cbNickname.ItemsSource = Nothing
        'nicknameVM.NicknameList = lstNicknameList
        'cbNickname.ItemsSource = nicknameVM.NicknameList
    End Sub

    Public Sub PopulateComboBoxAssetID()
        Try
            'If InitializeService() Then
            '    ' For Asset ID Combobox
            lstAssets = AideClient.GetClient().GetAllAssetsUnAssigned(profile.Emp_ID)
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)

            For Each objAsset As Assets In lstAssets
                assetDBProvider.SetAssetList(objAsset)
            Next

            For Each rawUser As MyAssets In assetDBProvider.GetAssetList()
                lstAssetsList.Add(New AssetsModel(rawUser))
            Next

            assetVM.AssetList = lstAssetsList

            'cbAssetID.ItemsSource = assetVM.AssetList

            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ListOfAssetType()
        Try
            'If InitializeService() Then
            Dim lstAssets As Assets() = AideClient.GetClient().GetAssetDescription()
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
            Dim assetsDBProvider As New AssetsDBProvider
            Dim assetsVM As New AssetsViewModel()

            ' Set the MyLessonLearntList 
            For Each objAssets As Assets In lstAssets
                assetsDBProvider.SetAssetTypeList(objAssets)
            Next

            For Each rawUser As MyAssets In assetsDBProvider.GetAssetTypeList()
                lstAssetsList.Add(New AssetsModel(rawUser))
            Next

            'assetsVM.AssetTypeList = Nothing
            'cbAssetType.ItemsSource = Nothing
            'assetsVM.AssetTypeList = lstAssetsList
            'cbAssetType.ItemsSource = assetsVM.AssetTypeList
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ListOfAssetManufacturer()
        Try
            'If InitializeService() Then
            Dim lstAssets As Assets() = AideClient.GetClient().GetAssetManufacturer()
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
            Dim assetsDBProvider As New AssetsDBProvider
            Dim assetsVM As New AssetsViewModel()

            ' Set the MyLessonLearntList 
            For Each objAssets As Assets In lstAssets
                assetsDBProvider.SetAssetManufacturerList(objAssets)
            Next

            For Each rawUser As MyAssets In assetsDBProvider.GetAssetManufacturerList()
                lstAssetsList.Add(New AssetsModel(rawUser))
            Next

            assetsVM.AssetManufacturerList = Nothing
            'cbAssetManufacturer.ItemsSource = Nothing
            'assetsVM.AssetManufacturerList = lstAssetsList
            'cbAssetManufacturer.ItemsSource = assetsVM.AssetManufacturerList
            'End If
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
