Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AssetsBorrowingAddPage
    Implements ServiceReference1.IAideServiceCallback
#Region "Fields"

    Private mainFrame As Frame
    Private fromPage As String
    Private approvalStatus As Integer
    Private client As AideServiceClient
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
#End Region

#Region "Constructor"
    Public Sub New(_assetsModel As AssetsModel, mainFrame As Frame, _profile As Profile, _fromPage As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()

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
        tbSuccessForm.Text = "Assets Borrowing"
        Me.pageDefinition = "Borrow"
        LoadData()
        LoadStatus()
        AssignEvents()
        PopulateComboBoxAssetID()
        'ListOfCustodians()
        ListOfAssetType()
        ListOfAssetManufacturer()

        If fromPage = "Borrow" And profile.Permission_ID = 1 Then
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

        If fromPage = "Borrow" Then
            txtBtnUpdate.Text = "SEND BORROW REQUEST"
        ElseIf fromPage = "Request" Then
            txtBtnUpdate.Text = "APPROVE"
        ElseIf fromPage = "Return" Then
            txtBtnUpdate.Text = "RETURN"
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
                If Me.fromPage = "Borrow" Then
                    Integer.TryParse(txtEmpID.Text, empId)
                    assets.EMP_ID = empId
                    assets.ASSET_ID = Integer.Parse(txtID.Text)
                    assets.ASSET_BORROWING_ID = Integer.Parse(txtAssetBorrowingID.Text)
                    assets.DATE_ASSIGNED = Date.Parse(Now)
                    assets.COMMENTS = txtComments.Text
                    assets.TRANS_FG = 10
                    assets.ASSET_DESC = txtAssetType.Text
                    assets.MANUFACTURER = txtAssetManufacturer.Text
                    assets.MODEL_NO = txtModel.Text
                    assets.SERIAL_NO = txtSerial.Text
                    assets.ASSET_TAG = txtAssetTag.Text
                    assets.STATUS = 10
                    'assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL
                    'assets.PREVIOUS_ID = assetsModel.EMP_ID
                    assets.TRANSFER_ID = cbNickname.SelectedValue
                    assets.APPROVAL = 0
                    'If profile.Permission_ID = 1 Then
                    '    assets.APPROVAL = 1
                    'Else
                    '    assets.APPROVAL = 0
                    'End If
                    'ElseIf Me.fromPage = "Request" Then
                    '    assets.EMP_ID = empId
                    '    assets.ASSET_ID = Integer.Parse(txtID.Text)
                    '    assets.ASSET_BORROWING_ID = Integer.Parse(txtAssetBorrowingID.Text)
                    '    assets.DATE_ASSIGNED = Date.Parse(Now)
                    '    assets.COMMENTS = txtComments.Text
                    '    assets.TRANS_FG = 11
                    '    assets.ASSET_DESC = txtAssetType.Text
                    '    assets.MANUFACTURER = txtAssetManufacturer.Text
                    '    assets.MODEL_NO = txtModel.Text
                    '    assets.SERIAL_NO = txtSerial.Text
                    '    assets.ASSET_TAG = txtAssetTag.Text
                    '    assets.STATUS = 11
                    '    'assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL
                    '    'assets.PREVIOUS_ID = assetsModel.EMP_ID
                    '    assets.TRANSFER_ID = cbNickname.SelectedValue
                    '    assets.APPROVAL = 11
                    '    'If profile.Permission_ID = 1 Then
                    '    '    assets.APPROVAL = 1
                    '    'Else
                    '    '    assets.APPROVAL = 0
                    '    'End If

                ElseIf Me.fromPage = "Return" Then
                    assets.EMP_ID = empId
                    assets.ASSET_ID = Integer.Parse(txtID.Text)
                    assets.ASSET_BORROWING_ID = Integer.Parse(txtAssetBorrowingID.Text)
                    assets.DATE_ASSIGNED = Date.Parse(Now)
                    assets.COMMENTS = txtComments.Text
                    assets.TRANS_FG = 12
                    assets.ASSET_DESC = txtAssetType.Text
                    assets.MANUFACTURER = txtAssetManufacturer.Text
                    assets.MODEL_NO = txtModel.Text
                    assets.SERIAL_NO = txtSerial.Text
                    assets.ASSET_TAG = txtAssetTag.Text
                    assets.STATUS = 12
                    'assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL
                    'assets.PREVIOUS_ID = assetsModel.EMP_ID
                    assets.TRANSFER_ID = cbNickname.SelectedValue
                    assets.APPROVAL = 1
                    'If profile.Permission_ID = 1 Then
                    '    assets.APPROVAL = 1
                    'Else
                    '    assets.APPROVAL = 0
                    'End If
                End If

                If InitializeService() Then
                    client.InsertAssetsBorrowing(assets)
                    MsgBox("Asset has been updated.", MsgBoxStyle.Information, "AIDE")
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
                End If
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
        mainFrame.Navigate(New AssetBorrowingPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
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

    Private Sub btnApprove_Click(sender As Object, e As RoutedEventArgs) Handles btnApprove.Click
        Try
            e.Handled = True
            Approval(11, 1)

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
            assets.APPROVAL = 1
            assets.ASSIGNED_TO = 999 'USED JUST TO BE NOT NULL


            If InitializeService() Then
                    client.UpdateAssetsInventoryCancel(assets)
                    MsgBox("Asset has been updated.", MsgBoxStyle.Information, "AIDE")
                    ClearFields()
                    mainFrame.Navigate(New AssetBorrowingPage(mainFrame, profile, _addframe, _menugrid, _submenuframe, fromPage))
                    mainFrame.IsEnabled = True
                    mainFrame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                _addframe.Visibility = Visibility.Hidden
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

    Public Sub Approval(ByVal stat As Integer, ByVal approval As Integer)
        Dim assets As New Assets
        assets.EMP_ID = empId
        assets.ASSET_ID = Integer.Parse(txtID.Text)
        assets.ASSET_BORROWING_ID = Integer.Parse(txtAssetBorrowingID.Text)
        assets.DATE_ASSIGNED = Date.Parse(Now)
        assets.COMMENTS = txtComments.Text
        assets.TRANS_FG = 11
        assets.ASSET_DESC = txtAssetType.Text
        assets.MANUFACTURER = txtAssetManufacturer.Text
        assets.MODEL_NO = txtModel.Text
        assets.SERIAL_NO = txtSerial.Text
        assets.ASSET_TAG = txtAssetTag.Text
        assets.STATUS = stat
        assets.APPROVAL = approval

        Dim result As Integer = MsgBox("Are you sure you want to disapprove this asset assignment?", MessageBoxButton.YesNo, "AIDE")
        If result = 6 Then
            If InitializeService() Then
                client.InsertAssetsBorrowing(assets)
                MsgBox("Asset assignment has been disapproved.", MsgBoxStyle.Information, "AIDE")
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
            End If
        Else
            Exit Sub
        End If


     
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

        If fromPage = "Borrow" Or fromPage = "Return" Then
            btnUpdate.Visibility = Windows.Visibility.Visible
            btnApprove.Visibility = Windows.Visibility.Collapsed
            btnDisapprove.Visibility = Windows.Visibility.Collapsed
            'btnCancel.Visibility = Windows.Visibility.Collapsed

        ElseIf fromPage = "Request" Then
            btnUpdate.Visibility = Windows.Visibility.Collapsed
            btnApprove.Visibility = Windows.Visibility.Visible
            btnDisapprove.Visibility = Windows.Visibility.Visible

        ElseIf fromPage = "Return" Then
            btnUpdate.Visibility = Windows.Visibility.Visible
            btnApprove.Visibility = Windows.Visibility.Collapsed
            btnDisapprove.Visibility = Windows.Visibility.Collapsed
            'Else
            '    If profile.Permission_ID = 1 OrElse profile.Permission_ID = 4 Then
            '        btnUpdate.Visibility = Windows.Visibility.Visible
            '        'btnCancel.Visibility = Windows.Visibility.Collapsed
            '        btnDisapprove.Visibility = Windows.Visibility.Collapsed
            '        btnApprove.Visibility = Windows.Visibility.Collapsed
            '    Else
            '        btnDisapprove.Visibility = Windows.Visibility.Collapsed
            '        btnApprove.Visibility = Windows.Visibility.Collapsed
            '        btnUpdate.Visibility = Windows.Visibility.Visible

            '        'btnCancel.Visibility = Windows.Visibility.Collapsed
            '    End If
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
        txtAssetBorrowingID.Text = assetsModel.ASSET_BORROWING_ID

    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            client.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Public Function CheckMissingField() As Boolean
        If txtEmpID.Text = String.Empty Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub PopulateComboBoxAssignedTo()
        Try
            If InitializeService() Then
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
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ListOfCustodians()
        Dim lstMangers As Assets() = client.GetAllAssetsCustodian(profile.Emp_ID)
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
        lstNickname = client.ViewNicknameByDeptID(profile.Email_Address, dsplyByDiv)
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
            If InitializeService() Then
                ' For Asset ID Combobox
                lstAssets = client.GetAllAssetsUnAssigned(profile.Emp_ID)
                Dim lstAssetsList As New ObservableCollection(Of AssetsModel)

                For Each objAsset As Assets In lstAssets
                    assetDBProvider.SetAssetList(objAsset)
                Next

                For Each rawUser As MyAssets In assetDBProvider.GetAssetList()
                    lstAssetsList.Add(New AssetsModel(rawUser))
                Next

                assetVM.AssetList = lstAssetsList

                cbAssetID.ItemsSource = assetVM.AssetList

            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ListOfAssetType()
        Try
            If InitializeService() Then
                Dim lstAssets As Assets() = client.GetAssetDescription()
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
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ListOfAssetManufacturer()
        Try
            If InitializeService() Then
                Dim lstAssets As Assets() = client.GetAssetManufacturer()
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
