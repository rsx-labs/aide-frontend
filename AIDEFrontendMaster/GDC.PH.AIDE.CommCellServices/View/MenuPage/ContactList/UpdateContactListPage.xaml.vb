Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports System.Configuration
Imports System.Windows.Forms

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class UpdateContactListPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    Private mainFrame As Frame
    'Private client As ServiceReference1.AideServiceClient
    Private contacts As New ContactListModel
    Private email As String
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private attendanceFrame As Frame
    Private profile As Profile
    Private old_empid As Integer
    Private contactVM As New ContactListViewModel
    'Private _OptionsViewModel As OptionViewModel
    Dim empPhoto As String
    Dim photoPath As String

#End Region

#Region "Constructors"
    Public Sub New(contactModel As ContactListModel, _mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)

        ' This call is required by the designer.
        InitializeComponent()
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.addframe = _addframe
        Me.mainFrame = _mainFrame
        Me.attendanceFrame = _attendanceFrame
        Me.email = _profile.Email_Address
        Me.profile = _profile
        contactVM.ContactProfile = contactModel
        old_empid = profile.Emp_ID
        DataContext = contactVM.ContactProfile
        ' Add any initialization after the InitializeComponent() call.
        photoPath = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PHOTO_PATH)
        txtPhotoNote.Text = "Note: Copy your picture to this path (" + photoPath + ")"
        ProcessUIAccess()
        AssignEvents()
        textLimits()
        LoadAllCB(contactModel)
        loadUI(contactModel)
        Try
            imgPhoto.Source = New BitmapImage(New Uri(contactVM.ContactProfile.IMAGE_PATH.ToString))
        Catch ex As Exception
            MsgBox("File Path for employee photo not found.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub
#End Region

#Region "Main Methods"
    Private Function GetManagerAuth() As Boolean
        If Me.profile.Permission_ID = 1 Then
            Return True
        End If

        Return False
    End Function

    Public Sub LoadAllCB(contactmod As ContactListModel)
        LoadLocation()
        LoadJobPosition()
        LoadPermission()
        LoadDepartment()
        LoadDivision(contactmod.DEPARTMENT_ID)
        LoadMaritalStatus()
        LoadWorkShift()
    End Sub

    Public Sub loadUI(contactmod As ContactListModel)
        cbContactLocation.Text = contactmod.LOCATION

        cbContactPosition.SelectedValue = contactmod.POSITION_ID
        'cbContactPosition.Text = contactmod.POSITION

        cbContactGroup.SelectedValue = contactmod.PERMISSION_GROUP_ID
        'cbContactGroup.Text = contactmod.PERMISSION_GROUP

        cbContactDepartment.SelectedValue = contactmod.DEPARTMENT_ID
        'cbContactDepartment.Text = contactmod.DEPARTMENT

        cbContactDivision.SelectedValue = contactmod.DIVISION_ID
        'cbContactDivision.Text = contactmod.DIVISION

        cbContactMaritalStatus.SelectedValue = contactmod.MARITAL_STATUS_ID
        'cbContactMaritalStatus.Text = contactmod.MARITAL_STATUS

        cbContactShiftStatus.Text = contactmod.SHIFT
    End Sub

    Private Sub ProcessUIAccess()
        If Not GetManagerAuth() Then
            ManagerAuthScreen.Visibility = Windows.Visibility.Visible
            cbContactDivision.IsEnabled = False
            cbContactDepartment.IsEnabled = False
            btnCDelete.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Public Sub textLimits()
        txtContactCellNo.MaxLength = 11
        txtContactHomePhone.MaxLength = 15
        txtContactLocalNumber.MaxLength = 4
        txtContactOtherPhone.MaxLength = 15
        txtContactEmpID.MaxLength = 9
        txtContactMName.MaxLength = 1
    End Sub

    Private Sub LoadLocation()
        'If InitializeService() Then
        Dim lstLocation As LocationList() = AideClient.GetClient().GetAllLocation()
        Dim lstLocationList As New ObservableCollection(Of LocationModel)
        Dim selectionDBProvider As New SelectionListDBProvider
        Dim selectionListVM As New SelectionListViewModel()

        For Each objLocation As LocationList In lstLocation
            selectionDBProvider._setlistofLocation(objLocation)
        Next

        For Each rawUser As myLocationSet In selectionDBProvider._getobjLocation()
            lstLocationList.Add(New LocationModel(rawUser))
        Next

        selectionListVM.ObjectLocationSet = lstLocationList

        cbContactLocation.DataContext = selectionListVM
        cbContactLocation.ItemsSource = selectionListVM.ObjectLocationSet
        'End If
    End Sub

    Public Sub LoadJobPosition()
        Try
            'If InitializeService() Then
            Dim lstPosition As PositionList() = AideClient.GetClient().GetAllPosition()
            Dim lstPositionList As New ObservableCollection(Of PositionModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objposition As PositionList In lstPosition
                selectionDBProvider._setlistofPosition(objposition)
            Next

            For Each rawUser As myPositionSet In selectionDBProvider._getobjPosition()
                lstPositionList.Add(New PositionModel(rawUser))
            Next

            selectionListVM.ObjectPositionSet = lstPositionList

            cbContactPosition.DataContext = selectionListVM
            cbContactPosition.ItemsSource = selectionListVM.ObjectPositionSet

            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadPermission()
        Try
            'If InitializeService() Then
            Dim lstPermission As PermissionList() = AideClient.GetClient().GetAllPermission()
            Dim lstPermissionList As New ObservableCollection(Of PermissionModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objpermission As PermissionList In lstPermission
                selectionDBProvider._setlistofPermission(objpermission)
            Next

            For Each rawUser As myPermissionSet In selectionDBProvider._getobjPermission()
                lstPermissionList.Add(New PermissionModel(rawUser))
            Next

            selectionListVM.ObjectPermissionSet = lstPermissionList

            cbContactGroup.DataContext = selectionListVM
            cbContactGroup.ItemsSource = selectionListVM.ObjectPermissionSet

            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadDepartment()
        Try
            'If InitializeService() Then
            Dim lstDepartment As DepartmentList() = AideClient.GetClient().GetAllDepartment()
            Dim lstDepartmentList As New ObservableCollection(Of DepartmentModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objdepartment As DepartmentList In lstDepartment
                selectionDBProvider._setlistofDepartment(objdepartment)
            Next

            For Each rawUser As myDepartmentSet In selectionDBProvider._getobjDepartment()
                lstDepartmentList.Add(New DepartmentModel(rawUser))
            Next

            selectionListVM.ObjectDepartmentSet = lstDepartmentList

            cbContactDepartment.DataContext = selectionListVM
            cbContactDepartment.ItemsSource = selectionListVM.ObjectDepartmentSet
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadDivision(ByVal DeptID As Integer)
        Try
            'If InitializeService() Then
            Dim lstDivision As DivisionList() = AideClient.GetClient().GetAllDivision(DeptID)
            Dim lstDivisionList As New ObservableCollection(Of DivisionModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objdivision As DivisionList In lstDivision
                selectionDBProvider._setlistofDivision(objdivision)
            Next

            For Each rawUser As myDivisionSet In selectionDBProvider._getobjDivision()
                lstDivisionList.Add(New DivisionModel(rawUser))
            Next

            selectionListVM.ObjectDivisionSet = lstDivisionList

            cbContactDivision.DataContext = selectionListVM
            cbContactDivision.ItemsSource = selectionListVM.ObjectDivisionSet

            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadMaritalStatus()
        Try
            'If InitializeService() Then
            Dim lstMarital As StatusList() = AideClient.GetClient().GetAllStatus("EMPLOYEE")
            Dim lstStatusList As New ObservableCollection(Of MaritalModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objMarital As StatusList In lstMarital
                selectionDBProvider._setlistofStatus(objMarital)
            Next

            For Each rawUser As myStatusSet In selectionDBProvider._getobjStatus()
                lstStatusList.Add(New MaritalModel(rawUser))
            Next

            selectionListVM.ObjectMaritalSet = lstStatusList

            cbContactMaritalStatus.DataContext = selectionListVM
            cbContactMaritalStatus.ItemsSource = selectionListVM.ObjectMaritalSet
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadWorkShift()
        Try
            'If InitializeService() Then
            Dim lstWorkShift As StatusList() = AideClient.GetClient().GetAllStatus("WORK_SHIFT")
            Dim lstWorkList As New ObservableCollection(Of WorkShiftModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objWork As StatusList In lstWorkShift
                selectionDBProvider._setlistofStatus(objWork)
            Next

            For Each rawUser As myStatusSet In selectionDBProvider._getobjStatus()
                lstWorkList.Add(New WorkShiftModel(rawUser))
            Next

            selectionListVM.ObjectWorkShiftSet = lstWorkList

            cbContactShiftStatus.DataContext = selectionListVM
            cbContactShiftStatus.ItemsSource = selectionListVM.ObjectWorkShiftSet
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '    Dim strData As String = String.Empty
    '    Try
    '        _OptionsViewModel = New OptionViewModel
    '        '_OptionsViewModel.Service = client
    '        If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
    '            For Each opt As OptionModel In _OptionsViewModel.OptionList
    '                If Not opt Is Nothing Then
    '                    strData = opt.VALUE
    '                    Exit For
    '                End If
    '            Next
    '        End If
    '    Catch ex As Exception
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return strData
    'End Function

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^0-9/]+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub
    Private Sub AlphaValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^A-Za-z-.']+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub

    Private Sub AssignEvents()
        AddHandler btnCCancel.Click, AddressOf btnCCancel_Click
        AddHandler btnCUpdate.Click, AddressOf btnCUpdate_Click
        AddHandler btnCDelete.Click, AddressOf btnCDelete_Click
    End Sub 'Assign events to buttons

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        client = New AideServiceClient(Context)
    '        client.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        client.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

#End Region

#Region "Events"
    Private Sub btnCCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCCancel.Click
        mainFrame.Navigate(New ContactListPage(mainFrame, profile, addframe, menugrid, submenuframe, attendanceFrame))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub btnCUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnCUpdate.Click
        Try
            e.Handled = True
            contactVM.ContactProfile = DataContext
            Dim contactList As New ContactList
            If Me.profile.Emp_ID = 0 OrElse
                contactVM.ContactProfile.LAST_NAME = String.Empty OrElse
                contactVM.ContactProfile.FIRST_NAME = String.Empty OrElse
                contactVM.ContactProfile.NICK_NAME = String.Empty OrElse
                IsNothing(contactVM.ContactProfile.BDATE) OrElse
                IsNothing(contactVM.ContactProfile.DT_HIRED) OrElse
                cbContactGroup.SelectedValue = Nothing OrElse
                cbContactMaritalStatus.SelectedValue = Nothing OrElse
                cbContactPosition.SelectedValue = Nothing OrElse
                cbContactShiftStatus.SelectedValue = Nothing OrElse
                cbContactDepartment.SelectedValue = Nothing OrElse
                cbContactDivision.SelectedValue = Nothing OrElse
                contactVM.ContactProfile.EMAIL_ADDRESS = String.Empty OrElse
                contactVM.ContactProfile.CEL_NO = String.Empty OrElse
                cbContactLocation.SelectedValue = Nothing Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else

                'If InitializeService() Then
                contactList.EmpID = contactVM.ContactProfile.EMP_ID
                contactList.LAST_NAME = contactVM.ContactProfile.LAST_NAME.ToUpper()
                contactList.FIRST_NAME = contactVM.ContactProfile.FIRST_NAME.ToUpper()
                contactList.MIDDLE_NAME = contactVM.ContactProfile.MIDDLE_NAME.ToUpper()
                contactList.Nick_Name = contactVM.ContactProfile.NICK_NAME.ToUpper()
                contactList.ACTIVE = contactVM.ContactProfile.ACTIVE
                contactList.BIRTHDATE = contactVM.ContactProfile.BDATE
                contactList.DT_HIRED = contactVM.ContactProfile.DT_HIRED
                contactList.EMADDRESS = contactVM.ContactProfile.EMAIL_ADDRESS
                contactList.EMADDRESS2 = contactVM.ContactProfile.EMAIL_ADDRESS2
                contactList.CELL_NO = contactVM.ContactProfile.CEL_NO
                contactList.lOCAL = contactVM.ContactProfile.LOCAL
                contactList.HOUSEPHONE = contactVM.ContactProfile.HOMEPHONE
                contactList.OTHERPHONE = contactVM.ContactProfile.OTHER_PHONE
                contactList.DateReviewed = DateTime.Now.Date

                contactList.LOC = cbContactLocation.SelectedValue.ToString
                contactList.POSITION = cbContactPosition.Text
                contactList.PERMISSION_GROUP = cbContactGroup.Text
                contactList.DEPARTMENT = cbContactDepartment.Text
                contactList.DIVISION = cbContactDivision.Text
                contactList.MARITAL_STATUS = cbContactMaritalStatus.Text

                contactList.MARITAL_STATUS_ID = cbContactMaritalStatus.SelectedValue
                contactList.POSITION_ID = cbContactPosition.SelectedValue
                contactList.PERMISSION_GROUP_ID = cbContactGroup.SelectedValue
                contactList.DEPARTMENT_ID = cbContactDepartment.SelectedValue
                contactList.DIVISION_ID = cbContactDivision.SelectedValue
                contactList.SHIFT = cbContactShiftStatus.Text

                contactList.OLD_EMP_ID = old_empid
                contactList.IMAGE_PATH = photoPath + empPhoto
                If empPhoto = Nothing Then
                    contactList.IMAGE_PATH = contactVM.ContactProfile.IMAGE_PATH
                End If

                AideClient.GetClient().UpdateContactListByEmpID(contactList, 0)
                MsgBox("Contacts have been updated.", MsgBoxStyle.Information, "AIDE")
                'ClearFields()
                attendanceFrame.Navigate(New AttendanceDashBoard(mainFrame, profile))
                mainFrame.Navigate(New ContactListPage(mainFrame, profile, addframe, menugrid, submenuframe, attendanceFrame))
                mainFrame.IsEnabled = True
                mainFrame.Opacity = 1
                menugrid.IsEnabled = True
                menugrid.Opacity = 1
                submenuframe.IsEnabled = True
                submenuframe.Opacity = 1
                addframe.Visibility = Visibility.Hidden
                'End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnCDelete_Click(sender As Object, e As RoutedEventArgs) Handles btnCDelete.Click
        Try
            e.Handled = True
            contactVM.ContactProfile = DataContext
            Dim contactList As New ContactList
            If contactVM.ContactProfile.EMP_ID <> 0 Then
                contactList.EmpID = contactVM.ContactProfile.EMP_ID
                contactList.LAST_NAME = contactVM.ContactProfile.LAST_NAME
                contactList.FIRST_NAME = contactVM.ContactProfile.FIRST_NAME
                contactList.MIDDLE_NAME = contactVM.ContactProfile.MIDDLE_NAME
                contactList.Nick_Name = contactVM.ContactProfile.NICK_NAME
                contactList.ACTIVE = 2
                contactList.BIRTHDATE = contactVM.ContactProfile.BDATE
                contactList.POSITION = contactVM.ContactProfile.POSITION
                contactList.DT_HIRED = contactVM.ContactProfile.DT_HIRED
                contactList.MARITAL_STATUS = contactVM.ContactProfile.MARITAL_STATUS
                contactList.IMAGE_PATH = photoPath + empPhoto
                contactList.PERMISSION_GROUP = contactVM.ContactProfile.PERMISSION_GROUP
                contactList.DEPARTMENT = contactVM.ContactProfile.DEPARTMENT
                contactList.DIVISION = contactVM.ContactProfile.DIVISION
                contactList.SHIFT = contactVM.ContactProfile.SHIFT
                contactList.EMADDRESS = contactVM.ContactProfile.EMAIL_ADDRESS
                contactList.EMADDRESS2 = contactVM.ContactProfile.EMAIL_ADDRESS2
                contactList.LOC = cbContactLocation.SelectedValue.ToString
                contactList.CELL_NO = contactVM.ContactProfile.CEL_NO
                contactList.lOCAL = contactVM.ContactProfile.LOCAL
                contactList.HOUSEPHONE = contactVM.ContactProfile.HOMEPHONE
                contactList.OTHERPHONE = contactVM.ContactProfile.OTHER_PHONE
                contactList.DateReviewed = DateTime.Now.Date
                contactList.MARITAL_STATUS_ID = contactVM.ContactProfile.MARITAL_STATUS_ID
                contactList.POSITION_ID = contactVM.ContactProfile.POSITION_ID
                contactList.PERMISSION_GROUP_ID = contactVM.ContactProfile.PERMISSION_GROUP_ID
                contactList.DEPARTMENT_ID = contactVM.ContactProfile.DEPARTMENT_ID
                contactList.DIVISION_ID = contactVM.ContactProfile.DIVISION_ID
                contactList.OLD_EMP_ID = old_empid

                If MsgBox("Are you sure you want to delete the Employee?", vbYesNo, "AIDE") = vbYes Then
                    'If InitializeService() Then
                    AideClient.GetClient().UpdateContactListByEmpID(contactList, 0)
                    'ClearFields()
                    MsgBox("Employee has been deleted.", MsgBoxStyle.OkOnly, "AIDE")
                    attendanceFrame.Navigate(New AttendanceDashBoard(mainFrame, profile))
                    mainFrame.Navigate(New ContactListPage(mainFrame, profile, addframe, menugrid, submenuframe, attendanceFrame))
                    mainFrame.IsEnabled = True
                    mainFrame.Opacity = 1
                    menugrid.IsEnabled = True
                    menugrid.Opacity = 1
                    submenuframe.IsEnabled = True
                    submenuframe.Opacity = 1
                    addframe.Visibility = Visibility.Hidden

                    'End If
                Else
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub cbContactDivision_DropDownOpened(sender As Object, e As EventArgs) Handles cbContactDivision.DropDownOpened
        Dim SelectedDivision As Integer
        SelectedDivision = cbContactDivision.SelectedValue
        If cbContactDepartment.SelectedValue = Nothing Then
            MsgBox("Please select a department.", vbInformation, "AIDE")
        Else
            LoadDivision(cbContactDepartment.SelectedValue)
            cbContactDivision.SelectedValue = SelectedDivision
        End If
    End Sub

    Private Sub btnPhoto_Click(sender As Object, e As RoutedEventArgs) Handles btnPhoto.Click
        Dim op As OpenFileDialog = New OpenFileDialog()
        op.Title = "Select a picture"
        op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" & "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" & "Portable Network Graphic (*.png)|*.png"

        Dim result As DialogResult = op.ShowDialog()

        If result = DialogResult.OK Then
            imgPhoto.Source = New BitmapImage(New Uri(op.FileName))
            empPhoto = op.SafeFileName
        End If
    End Sub
#End Region

#Region "Service Methods"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError

    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
