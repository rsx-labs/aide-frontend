Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports System.Configuration

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class InsertContactList
    Implements ServiceReference1.IAideServiceCallback


#Region "Declarations"
    Private mainFrame As Frame
    Private client As ServiceReference1.AideServiceClient
    Private contacts As New ContactListModel
    Private email As String
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private attendanceFrame As Frame
    Private profile As Profile
    Private user_empid As Integer
    Private contactVM As New ContactListViewModel

    Dim locationEco As String = ConfigurationManager.AppSettings("locationEco")
    Dim locationNet As String = ConfigurationManager.AppSettings("locationNet")
    Dim locationDurham As String = ConfigurationManager.AppSettings("locationDurham")
    Dim locationWfh As String = ConfigurationManager.AppSettings("locationWfh")
    Dim shiftsemi As String = ConfigurationManager.AppSettings("shiftsemi")
    Dim shiftflex As String = ConfigurationManager.AppSettings("shiftflex")
    Dim addFg As Integer = ConfigurationManager.AppSettings("addFg")

    'Marital Status
    Dim maritalSingleID As String = ConfigurationManager.AppSettings("maritalSingleID")
    Dim maritalSingleDesc As String = ConfigurationManager.AppSettings("maritalSingleDesc")
    Dim maritalMarriedID As String = ConfigurationManager.AppSettings("maritalMarriedID")
    Dim maritalMarriedDesc As String = ConfigurationManager.AppSettings("maritalMarriedDesc")

    'Job Position
    Dim posManagerID As Integer = ConfigurationManager.AppSettings("posManagerID")
    Dim posJDeveloperID As Integer = ConfigurationManager.AppSettings("posJDeveloperID")
    Dim posSDeveloperID As Integer = ConfigurationManager.AppSettings("posSDeveloperID")
    Dim posMDeveloperID As Integer = ConfigurationManager.AppSettings("posMDeveloperID")
    Dim posInternID As Integer = ConfigurationManager.AppSettings("posInternID")
    Dim posManagerDesc As String = ConfigurationManager.AppSettings("posManagerDesc")
    Dim posJDeveloperDesc As String = ConfigurationManager.AppSettings("posJDeveloperDesc")
    Dim posSDeveloperDesc As String = ConfigurationManager.AppSettings("posSDeveloperDesc")
    Dim posMDeveloperDesc As String = ConfigurationManager.AppSettings("posMDeveloperDesc")
    Dim posInternDesc As String = ConfigurationManager.AppSettings("posInternDesc")

    'Permission Group
    Dim permManagerID As Integer = ConfigurationManager.AppSettings("permManagerID")
    Dim permUserLevelID As Integer = ConfigurationManager.AppSettings("permUserLevelID")
    Dim permManagerDesc As String = ConfigurationManager.AppSettings("permManagerDesc")
    Dim permUserLevelDesc As String = ConfigurationManager.AppSettings("permUserLevelDesc")

#End Region


    Public Sub New(_mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.addframe = _addframe
        Me.mainFrame = _mainFrame
        Me.attendanceFrame = _attendanceFrame
        Me.email = _profile.Email_Address
        Me.profile = _profile
        DataContext = contactVM.ContactProfile
        ClearTextVal()
        user_empid = Me.profile.Emp_ID

        ' Add any initialization after the InitializeComponent() call.
        AssignEvents()
        textLimits()
        LoadAllCB()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

#Region "Service methods"

#End Region

#Region "Main methods"
    Public Sub LoadAllCB()
        SetLocationCB()
        SetWorkShiftCB()
        SetMaritalStatusCB()
        SetJobPositionCB()
        SetPermissionGroupCB()
    End Sub
    Private Sub AssignEvents()
        AddHandler btnCCancel.Click, AddressOf btnCCancel_Click
        AddHandler btnCreate.Click, AddressOf btnCreate_Click
    End Sub
    Public Sub textLimits()
        txtContactCellNo.MaxLength = 15
        txtContactHomePhone.MaxLength = 15
        txtContactLocalNumber.MaxLength = 4
        txtContactOtherPhone.MaxLength = 15
    End Sub
    Public Sub SetLocationCB()
        cbContactLocation.DisplayMemberPath = "Text"
        cbContactLocation.SelectedValuePath = "Value"
        cbContactLocation.Items.Add(New With {.Text = locationEco, .Value = locationEco})
        cbContactLocation.Items.Add(New With {.Text = locationNet, .Value = locationNet})
        cbContactLocation.Items.Add(New With {.Text = locationDurham, .Value = locationDurham})
        cbContactLocation.Items.Add(New With {.Text = locationWfh, .Value = locationWfh})
    End Sub

    Public Sub SetWorkShiftCB()
        cbContactShiftStatus.DisplayMemberPath = "Text"
        cbContactShiftStatus.SelectedValuePath = "Value"
        cbContactShiftStatus.Items.Add(New With {.Text = shiftsemi, .Value = shiftsemi})
        cbContactShiftStatus.Items.Add(New With {.Text = shiftflex, .Value = shiftflex})
    End Sub

    Public Sub SetMaritalStatusCB()
        cbContactMaritalStatus.DisplayMemberPath = "Text"
        cbContactMaritalStatus.SelectedValuePath = "Value"
        cbContactMaritalStatus.Items.Add(New With {.Text = maritalSingleDesc, .Value = maritalSingleID})
        cbContactMaritalStatus.Items.Add(New With {.Text = maritalMarriedDesc, .Value = maritalMarriedID})
    End Sub

    Public Sub SetJobPositionCB()
        cbContactPosition.DisplayMemberPath = "Text"
        cbContactPosition.SelectedValuePath = "Value"
        cbContactPosition.Items.Add(New With {.Text = posManagerDesc, .Value = posManagerID})
        cbContactPosition.Items.Add(New With {.Text = posJDeveloperDesc, .Value = posJDeveloperID})
        cbContactPosition.Items.Add(New With {.Text = posSDeveloperDesc, .Value = posSDeveloperID})
        cbContactPosition.Items.Add(New With {.Text = posMDeveloperDesc, .Value = posMDeveloperID})
        cbContactPosition.Items.Add(New With {.Text = posInternDesc, .Value = posInternID})
    End Sub

    Public Sub ClearTextVal()
        txtContactEmpID.Text = String.Empty

    End Sub


    Public Sub SetPermissionGroupCB()
        cbContactGroup.DisplayMemberPath = "Text"
        cbContactGroup.SelectedValuePath = "Value"
        cbContactGroup.Items.Add(New With {.Text = permManagerDesc, .Value = permManagerID})
        cbContactGroup.Items.Add(New With {.Text = permUserLevelDesc, .Value = permUserLevelID})
    End Sub

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^0-9/]+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub
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

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        Try
            e.Handled = True
            contactVM.ContactProfile = DataContext
            Dim contactList As New ContactList
            If contactVM.ContactProfile.EMP_ID = 0 OrElse Me.profile.Emp_ID = 0 OrElse contactVM.ContactProfile.LAST_NAME = String.Empty OrElse contactVM.ContactProfile.FIRST_NAME = String.Empty OrElse _
             contactVM.ContactProfile.NICK_NAME = String.Empty OrElse IsNothing(contactVM.ContactProfile.BDATE) OrElse IsNothing(contactVM.ContactProfile.DT_HIRED) _
             OrElse contactVM.ContactProfile.EMAIL_ADDRESS = String.Empty OrElse contactVM.ContactProfile.CEL_NO = String.Empty AndAlso contactVM.ContactProfile.LOCATION = String.Empty Then
                MsgBox("Please Fill Up all the Fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                contactList.EmpID = contactVM.ContactProfile.EMP_ID
                contactList.LAST_NAME = contactVM.ContactProfile.LAST_NAME.ToUpper()
                contactList.FIRST_NAME = contactVM.ContactProfile.FIRST_NAME.ToUpper()
                contactList.MIDDLE_NAME = contactVM.ContactProfile.MIDDLE_NAME.ToUpper()
                contactList.Nick_Name = contactVM.ContactProfile.NICK_NAME.ToUpper()
                contactList.ACTIVE = addFg
                contactList.BIRTHDATE = contactVM.ContactProfile.BDATE
                contactList.POSITION = contactVM.ContactProfile.POSITION
                contactList.DT_HIRED = contactVM.ContactProfile.DT_HIRED
                contactList.MARITAL_STATUS = contactVM.ContactProfile.MARITAL_STATUS
                contactList.IMAGE_PATH = contactVM.ContactProfile.IMAGE_PATH
                contactList.PERMISSION_GROUP = contactVM.ContactProfile.PERMISSION_GROUP
                contactList.DEPARTMENT = contactVM.ContactProfile.DEPARTMENT
                contactList.DIVISION = contactVM.ContactProfile.DIVISION
                contactList.SHIFT = contactVM.ContactProfile.SHIFT
                contactList.EMADDRESS = contactVM.ContactProfile.EMAIL_ADDRESS
                contactList.EMADDRESS2 = contactVM.ContactProfile.EMAIL_ADDRESS2
                contactList.LOC = contactVM.ContactProfile.LOCATION
                contactList.CELL_NO = contactVM.ContactProfile.CEL_NO
                If contactVM.ContactProfile.LOCAL Is Nothing Then
                    contactList.lOCAL = 0
                Else
                    contactList.lOCAL = contactVM.ContactProfile.LOCAL
                End If

                contactList.HOUSEPHONE = contactVM.ContactProfile.HOMEPHONE
                contactList.OTHERPHONE = contactVM.ContactProfile.OTHER_PHONE
                contactList.DateReviewed = DateTime.Now.Date
                contactList.MARITAL_STATUS_ID = contactVM.ContactProfile.MARITAL_STATUS_ID
                contactList.POSITION_ID = contactVM.ContactProfile.POSITION_ID
                contactList.PERMISSION_GROUP_ID = contactVM.ContactProfile.PERMISSION_GROUP_ID
                contactList.DEPARTMENT_ID = contactVM.ContactProfile.DEPARTMENT_ID
                contactList.DIVISION_ID = contactVM.ContactProfile.DIVISION_ID
                contactList.OLD_EMP_ID = user_empid

                Dim result As Integer = MsgBox("Are you sure you want to continue?", MsgBoxStyle.OkCancel, "AIDE")
                If result = 1 Then
                    If InitializeService() Then
                        client.CreateNewContactByEmpID(contactList)
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
                    End If
                Else
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub
#End Region


#Region "Service methods"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

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
