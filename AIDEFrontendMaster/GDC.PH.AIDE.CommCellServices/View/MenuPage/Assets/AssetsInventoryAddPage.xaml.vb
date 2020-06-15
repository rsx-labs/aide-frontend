Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AssetsInventoryAddPage
    Implements ServiceReference1.IAideServiceCallback
#Region "Fields"

    Private mainFrame As Frame
    Private fromPage As String
    Private approvalStatus As Integer
    'Private client As AideServiceClient
    Private assetsModel As New AssetsModel
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

    Private mailConfig As New MailConfig
    Private mailConfigVM As New MailConfigViewModel
    'Private _OptionsViewModel As OptionViewModel
    Private _option As OptionModel
    Private lstMissingAttendance As Employee()
    Private isAINotifAllow As Boolean

    Private approvalDescr As String
#End Region

#Region "Constructor"
    Public Sub New(_assetsModel As AssetsModel, mainFrame As Frame, _profile As Profile,
                   _fromPage As String, _addframe As Frame, _menugrid As Grid,
                   _submenuframe As Frame)

        InitializeComponent()

        'client = aideService
        mailConfigVM = New MailConfigViewModel()
        Me.mainFrame = mainFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.profile = _profile
        Me.assetsModel = _assetsModel
        Me.fromPage = _fromPage
        If Me.fromPage = "Approval" Then
            cbStatus.IsEnabled = True
        End If
        tbSuccessForm.Text = "Update Assigned Assets"
        Me.pageDefinition = "Update"
        GetMailConfig()
        LoadData()
        LoadStatus()
        AssignEvents()
        PopulateComboBoxAssetID()
        'ListOfCustodians()
        ListOfAssetType()
        ListOfAssetManufacturer()

        If fromPage = "Update" And profile.Permission_ID = 1 Then
            txtEmpID.IsEnabled = True
            txtEmpID.Text = String.Empty
        Else
            txtEmpID.Text = _assetsModel.EMP_ID
            Integer.TryParse(txtEmpID.Text, empId)
        End If

        If assetsModel.STATUS = 4 And assetsModel.PREVIOUS_ID <> 0 Then
            status = 1
        ElseIf assetsModel.STATUS = 3 And assetsModel.PREVIOUS_ID <> 0 Then
            status = 1
        ElseIf assetsModel.STATUS = 4 Then
            status = 2
        ElseIf assetsModel.STATUS = 3 And assetsModel.PREVIOUS_ID = 0 Then
            status = 1
        End If
        If assetsModel.APPROVAL = 6 Then
            btnVerify.Visibility = Visibility.Visible
        End If
    End Sub

#End Region

#Region "Events"
    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        Try
            e.Handled = True
            Dim assets As New Assets
            If CheckMissingField() Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Integer.TryParse(txtEmpID.Text, empId)
                assets.EMP_ID = empId
                assets.ASSET_ID = Integer.Parse(txtID.Text)
                assets.DATE_ASSIGNED = Date.Parse(dateInput.SelectedDate)
                assets.COMMENTS = txtComments.Text
                assets.ASSET_DESC = txtAssetType.Text
                assets.MANUFACTURER = txtAssetManufacturer.Text
                assets.MODEL_NO = txtModel.Text
                assets.SERIAL_NO = txtSerial.Text
                assets.ASSET_TAG = txtAssetTag.Text
                assets.STATUS = cbStatus.SelectedValue
                assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL
                assets.PREVIOUS_ID = assetsModel.PREVIOUS_ID
                assets.TRANSFER_ID = cbNickname.SelectedValue
                If profile.Permission_ID = 1 Then 'Manage
                    If assetsModel.APPROVAL <> 0 Then
                        assets.APPROVAL = 6
                    Else
                        assets.APPROVAL = assetsModel.APPROVAL
                    End If
                ElseIf profile.Permission_ID = 4 Then 'Asset Custodian
                    assets.APPROVAL = 6
                Else
                    assets.APPROVAL = assetsModel.APPROVAL
                End If
                'If InitializeService() Then
                AideClient.GetClient().UpdateAssetsInventory(assets)

                If isAINotifAllow Then
                    If profile.Permission_ID = 4 And cbStatus.SelectedValue = 3 Then
                        GetAssetMovementData(16, 0, 0)
                        SetAssetAssignEmailNotification(assets, cbNickname.SelectedValue)
                    End If
                    If cbStatus.SelectedValue = 4 Then
                        GetAssetMovementData(19, 0, 0)
                        SetAssetUnAssignEmailNotification(assets, cbNickname.SelectedValue)
                    End If
                End If
                MsgBox("Asset inventory have been updated.", MsgBoxStyle.Information, "AIDE")
                ClearFields()
                mainFrame.Navigate(New AssetsInventoryListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
                mainFrame.IsEnabled = True
                mainFrame.Opacity = 1
                _menugrid.IsEnabled = True
                _menugrid.Opacity = 1
                _submenuframe.IsEnabled = True
                _submenuframe.Opacity = 1

                _addframe.Visibility = Visibility.Hidden
                'End If
            End If
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
        mainFrame.Navigate(New AssetsInventoryListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden

    End Sub

    Private Sub btnVerify_Click(sender As Object, e As RoutedEventArgs) Handles btnVerify.Click
        Try
            If isAINotifAllow Then
                GetAssetMovementData(17, 0, 0)
                SetAssetVerifyEmailNotification(assetsModel, assetsModel.EMP_ID)
            End If
            MsgBox("Asset has been verified.", MsgBoxStyle.Information, "AIDE")
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnDisapprove_Click(sender As Object, e As RoutedEventArgs) Handles btnDisapprove.Click
        Try
            e.Handled = True
            approvalDescr = "disapproved"
            If status = 2 Or status = 3 Then
                approvalStatus = 5 'Approved
            ElseIf status = 1 Or status = 4 Then
                approvalStatus = 6
            End If
            empId = assetsModel.PREVIOUS_ID
            Approval()
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnApprove_Click(sender As Object, e As RoutedEventArgs) Handles btnApprove.Click
        Try
            e.Handled = True
            approvalStatus = 5
            approvalDescr = "approved"
            Integer.TryParse(txtEmpID.Text, empId) 'Integer.Parse(profile.Emp_ID)
            If assetsModel.STATUS = 4 Then
                status = 1
            ElseIf assetsModel.STATUS = 3 Then
                status = 2
            End If
            Approval()
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        Try

            e.Handled = True
            Dim assets As New Assets
            assets.EMP_ID = profile.Emp_ID
            assets.ASSET_ID = Integer.Parse(txtID.Text)
            assets.DATE_ASSIGNED = Date.Now
            assets.COMMENTS = txtComments.Text
            assets.ASSET_DESC = cbAssetType.Text
            assets.MANUFACTURER = cbAssetManufacturer.Text
            assets.MODEL_NO = txtModel.Text
            assets.SERIAL_NO = txtSerial.Text
            assets.ASSET_TAG = txtAssetTag.Text
            assets.STATUS = 1
            assets.APPROVAL = 6
            assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL

            Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
            If result = 1 Then
                'If InitializeService() Then
                AideClient.GetClient().UpdateAssetsInventoryCancel(assets)
                ClearFields()
                mainFrame.Navigate(New AssetsInventoryListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
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
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub dateInput_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateInput.SelectedDateChanged
        e.Handled = True
        If dateInput.SelectedDate > Date.Now Then
            MsgBox("Please enter a date on or before the current date.", MsgBoxStyle.Exclamation, "AIDE")
            dateInput.SelectedDate = Date.Now
        Else
            Exit Sub
        End If
    End Sub

    Private Sub cbAssetID_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbAssetID.SelectionChanged
        Dim q = assetVM.AssetList.Where(Function(X) X.ASSET_ID = cbAssetID.SelectedValue).FirstOrDefault()

        If q IsNot Nothing Then
            txtID.Text = q.ASSET_ID
            cbAssetType.Text = q.ASSET_DESC
            cbAssetManufacturer.Text = q.MANUFACTURER
            txtModel.Text = q.MODEL_NO
            txtSerial.Text = q.SERIAL_NO
            txtAssetTag.Text = q.ASSET_TAG
        End If
    End Sub

    Private Sub cbNickname_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbNickname.SelectionChanged
        txtEmpID.Text = cbNickname.SelectedValue
        Integer.TryParse(txtEmpID.Text, empId)
    End Sub

    Private Sub cbStatus_DropDownOpened(sender As Object, e As EventArgs) Handles cbStatus.DropDownOpened
        txtEmpName.Text = "Select employee *"
        txtEmpID.Clear()
    End Sub

    Private Sub cbStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbStatus.SelectionChanged
        PopulateComboBoxAssignedTo()
    End Sub


#End Region

#Region "Functions"

    Public Sub Approval()
        Dim assets As New Assets
        assets.EMP_ID = empId
        assets.ASSET_ID = Integer.Parse(txtID.Text)
        assets.DATE_ASSIGNED = Date.Parse(dateInput.SelectedDate)
        assets.COMMENTS = txtComments.Text
        assets.ASSET_DESC = cbAssetType.Text
        assets.MANUFACTURER = cbAssetManufacturer.Text
        assets.MODEL_NO = txtModel.Text
        assets.SERIAL_NO = txtSerial.Text
        assets.ASSET_TAG = txtAssetTag.Text
        assets.APPROVAL = approvalStatus
        assets.STATUS = status
        assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL

        'If InitializeService() Then
        AideClient.GetClient().UpdateAssetsInventoryApproval(assets)
        If isAINotifAllow Then
            GetAssetMovementData(18, 0, 0)
            SetAssetApprovalEmailNotification(assetsModel, cbNickname.SelectedValue)
        End If
        MsgBox("Asset inventory have been updated.", MsgBoxStyle.Information, "AIDE")
        ClearFields()
        mainFrame.Navigate(New AssetsInventoryListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden

        'End If
    End Sub

    Private Sub AssignEvents()
        If fromPage = "Approval" Then
            cbStatus.IsEnabled = True
            cbNickname.IsEnabled = False
        End If
        AddHandler btnApprove.Click, AddressOf btnApprove_Click
        AddHandler btnCancel.Click, AddressOf btnCancel_Click
        AddHandler btnBack.Click, AddressOf btnBack_Click
        AddHandler btnUpdate.Click, AddressOf btnUpdate_Click
        AddHandler btnDisapprove.Click, AddressOf btnDisapprove_Click
        cbNickname.SelectedValue = assetsModel.EMP_ID
    End Sub 'Assign events to buttons

    Private Sub ClearFields()
        txtAssetTag.Clear()
        txtModel.Clear()
        txtComments.Clear()
        txtSerial.Clear()
        dateInput.Text = String.Empty
    End Sub

    Public Sub LoadStatus()
        cbStatus.IsEnabled = True
        cbStatus.DisplayMemberPath = "Text"
        cbStatus.SelectedValuePath = "Value"
        If profile.Permission_ID = 1 Then
            cbStatus.Items.Add(New With {.Text = "Unassigned", .Value = 1})
            cbStatus.Items.Add(New With {.Text = "Assigned", .Value = 2})
        Else
            cbStatus.Items.Add(New With {.Text = "Unassigned", .Value = 4})
            cbStatus.Items.Add(New With {.Text = "Assigned", .Value = 3})
        End If

        If assetsModel.STATUS = 3 Then
            txtStatus.Text = "Partially Assigned"
        ElseIf assetsModel.STATUS = 4 Then
            txtStatus.Text = "Partially Unassigned"
        End If
    End Sub

    Private Sub LoadData()

        If fromPage = "Update" Then
            btnUpdate.Visibility = Windows.Visibility.Visible
            btnApprove.Visibility = Windows.Visibility.Collapsed
            btnDisapprove.Visibility = Windows.Visibility.Collapsed
            'btnCancel.Visibility = Windows.Visibility.Collapsed

        ElseIf fromPage = "Approval" Then
            btnUpdate.Visibility = Windows.Visibility.Collapsed
            btnApprove.Visibility = Windows.Visibility.Visible
            btnDisapprove.Visibility = Windows.Visibility.Visible
        Else
            If profile.Permission_ID = 1 Then
                btnUpdate.Visibility = Windows.Visibility.Visible
                'btnCancel.Visibility = Windows.Visibility.Collapsed
                btnDisapprove.Visibility = Windows.Visibility.Collapsed
                btnApprove.Visibility = Windows.Visibility.Collapsed
            Else
                btnDisapprove.Visibility = Windows.Visibility.Collapsed
                btnApprove.Visibility = Windows.Visibility.Collapsed
                btnUpdate.Visibility = Windows.Visibility.Visible

                'btnCancel.Visibility = Windows.Visibility.Collapsed
            End If
        End If

        cbAssetID.IsEnabled = False
        cbAssetType.Visibility = Windows.Visibility.Collapsed
        cbAssetManufacturer.Visibility = Windows.Visibility.Collapsed
        txtAssetType.Visibility = Windows.Visibility.Visible
        txtAssetManufacturer.Visibility = Windows.Visibility.Visible
        txtID.Text = assetsModel.ASSET_ID
        txtEmpID.Text = assetsModel.EMP_ID
        txtAssetType.Text = assetsModel.ASSET_DESC
        txtAssetManufacturer.Text = assetsModel.MANUFACTURER
        txtModel.Text = assetsModel.MODEL_NO
        txtSerial.Text = assetsModel.SERIAL_NO
        txtAssetTag.Text = assetsModel.ASSET_TAG
        cbStatus.Tag = assetsModel.STATUS
        cbStatus.SelectedIndex = assetsModel.STATUS - 1
        cbAssetID.SelectedValue = assetsModel.ASSET_ID
        txtEmpName.Text = assetsModel.FULL_NAME
        txtComments.Text = assetsModel.COMMENTS
        dateInput.Text = assetsModel.DATE_PURCHASED

    End Sub

    'Public Function InitializeService() As Boolean
    '    'Dim bInitialize As Boolean = False
    '    'Try
    '    '    'DisplayText("Opening client service...")
    '    '    Dim Context As InstanceContext = New InstanceContext(Me)
    '    '    client = New AideServiceClient(Context)
    '    '    client.Open()
    '    '    bInitialize = True
    '    '    'DisplayText("Service opened successfully...")
    '    '    'Return True
    '    'Catch ex As SystemException
    '    '    client.Abort()
    '    '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    'End Try
    '    'Return bInitialize
    '    Return True
    'End Function

    Public Function CheckMissingField() As Boolean
        If txtEmpID.Text = String.Empty Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub PopulateComboBoxAssignedTo()
        Try
            'If InitializeService() Then
            If profile.Permission_ID = 2 Then ' User
                If cbStatus.SelectedIndex = 1 Then ' Assigned
                    txtEmpID.Text = profile.Emp_ID
                    cbNickname.IsEnabled = False
                Else ' Unassigned
                    cbNickname.IsEnabled = True
                    ListOfCustodians()
                End If
            Else ' Manager or Custodian
                If cbStatus.SelectedIndex = 1 Then ' Assigned
                    ListOfAllUser()
                Else ' Unassigned
                    cbNickname.IsEnabled = True
                    ListOfCustodians()
                End If
            End If
            'End If
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

        cbNickname.DataContext = assetsVM
        cbNickname.ItemsSource = assetsVM.AssetManagerList
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
        cbNickname.ItemsSource = Nothing
        nicknameVM.NicknameList = lstNicknameList
        cbNickname.ItemsSource = nicknameVM.NicknameList
    End Sub

    Public Sub PopulateComboBoxAssetID()
        Try
            'If InitializeService() Then
            ' For Asset ID Combobox
            lstAssets = AideClient.GetClient().GetAllAssetsUnAssigned(profile.Emp_ID)
            Dim lstAssetsList As New ObservableCollection(Of AssetsModel)

            For Each objAsset As Assets In lstAssets
                assetDBProvider.SetAssetList(objAsset)
            Next

            For Each rawUser As MyAssets In assetDBProvider.GetAssetList()
                lstAssetsList.Add(New AssetsModel(rawUser))
            Next

            assetVM.AssetList = lstAssetsList
            cbAssetID.ItemsSource = assetVM.AssetList

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

            assetsVM.AssetTypeList = Nothing
            cbAssetType.ItemsSource = Nothing
            assetsVM.AssetTypeList = lstAssetsList
            cbAssetType.ItemsSource = assetsVM.AssetTypeList
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
            cbAssetManufacturer.ItemsSource = Nothing
            assetsVM.AssetManufacturerList = lstAssetsList
            cbAssetManufacturer.ItemsSource = assetsVM.AssetManufacturerList
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetMailConfig()
        Try
            'If InitializeService() Then
            mailConfig = AideClient.GetClient().GetMailConfig()
            LoadMailConfig()
            isAINotifAllow = mailConfigVM.isSendEmail(10, 0, 0)
            'End If

        Catch ex As Exception

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub LoadMailConfig()

        Dim MConfigModel As New MailConfigModel
        Dim MConfigProvider As New MailConfigDBProvider

        Try
            MConfigProvider._setlistofitems(MailConfig)
            MConfigModel = New MailConfigModel(MConfigProvider._getobjmailconfig)

            mailConfigVM.objectMailConfigSet = MConfigModel

        Catch ex As Exception

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetAssetMovementData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer)
        Try
            '_OptionsViewModel = New OptionViewModel
            ''_OptionsViewModel.Service = client
            '_option = New OptionModel
            'If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
            '    For Each opt As OptionModel In _OptionsViewModel.OptionList
            '        If Not opt Is Nothing Then
            '            _option = opt
            '        End If
            '    Next
            'End If

            _option = AppState.GetInstance().OptionDictionary(optID)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetAssetAssignEmailNotification(ByVal asset As Assets, ByVal emp_id As Integer)
        Try
            Dim assetinfo As String = GetAssetInfo(asset)
            lstMissingAttendance = AideClient.GetClient().GetEmployeeEmailForAssetMovement(emp_id)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.EmailAddress, objEmployee.ManagerEmail, 2, assetinfo)
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetAssetUnAssignEmailNotification(ByVal asset As Assets, ByVal emp_id As Integer)
        Try
            Dim assetinfo As String = GetAssetInfo(asset)
            lstMissingAttendance = AideClient.GetClient().GetEmployeeEmailForAssetMovement(emp_id)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.EmployeeName, objEmployee.ManagerEmail, 3, assetinfo, assetsModel.FULL_NAME)
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetAssetVerifyEmailNotification(ByVal asset As AssetsModel, ByVal emp_id As Integer)
        Try
            Dim assetinfo As String = GetAssetInfo2(asset)
            lstMissingAttendance = AideClient.GetClient().GetEmployeeEmailForAssetMovement(emp_id)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.ManagerEmail, objEmployee.EmployeeName, 3, assetinfo, asset.FULL_NAME)
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetAssetApprovalEmailNotification(ByVal asset As AssetsModel, ByVal emp_id As Integer)
        Try
            Dim assetinfo As String = GetAssetInfo2(asset)
            Dim opt2 As String = asset.FULL_NAME + "," + approvalDescr + "," + Me.profile.FirstName + " " + Me.profile.LastName
            lstMissingAttendance = AideClient.GetClient().GetEmployeeEmailForAssetMovement(emp_id)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.EmployeeName, objEmployee.EmailAddress, 4, assetinfo, opt2)
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Function GetAssetInfo(ByVal asset As Assets) As String
        Dim assetinfo As String = String.Empty

        assetinfo = "Type: " + asset.ASSET_DESC + "," +
                    "Manufacturer: " + asset.MANUFACTURER + "," +
                    "Model #: " + asset.MODEL_NO + "," +
                    "Serial #:" + asset.SERIAL_NO + "," +
                    "Asset Tag: " + asset.ASSET_TAG

        Return assetinfo
    End Function
    Private Function GetAssetInfo2(ByVal asset As AssetsModel) As String
        Dim assetinfo As String = String.Empty

        assetinfo = "Type: " + asset.ASSET_DESC + "," +
                    "Manufacturer: " + asset.MANUFACTURER + "," +
                    "Model #: " + asset.MODEL_NO + "," +
                    "Serial #:" + asset.SERIAL_NO + "," +
                    "Asset Tag: " + asset.ASSET_TAG

        Return assetinfo
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
