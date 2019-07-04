Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports System.Configuration

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class UpdateContactListPage
    Implements ServiceReference1.IAideServiceCallback

    Dim contactVM As New ContactListViewModel

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

    Dim locationEco As String = ConfigurationManager.AppSettings("locationEco")
    Dim locationNet As String = ConfigurationManager.AppSettings("locationNet")
    Dim locationDurham As String = ConfigurationManager.AppSettings("locationDurham")
    Dim locationWfh As String = ConfigurationManager.AppSettings("locationWfh")
    Dim shiftsemi As String = ConfigurationManager.AppSettings("shiftsemi")
    Dim shiftflex As String = ConfigurationManager.AppSettings("shiftflex")

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
        DataContext = contactVM.ContactProfile
        ' Add any initialization after the InitializeComponent() call.
        ProcessUIAccess()
        AssignEvents()
        textLimits()
        SetLocationCB()
        SetWorkShiftCB()
    End Sub
#End Region

#Region "Main Methods"
    Private Function GetManagerAuth() As Boolean
        If Me.profile.Position = "Manager" Then
            Return True
        End If

        Return False
    End Function

    Private Sub ProcessUIAccess()
        If Not GetManagerAuth() Then
            ManagerAuthScreen.Visibility = Windows.Visibility.Visible
        End If
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
        cbContactShiftStatus.DisplayMemberPath = "Value"
        cbContactShiftStatus.Items.Add(New With {.Text = shiftsemi, .Value = shiftsemi})
        cbContactShiftStatus.Items.Add(New With {.Text = shiftflex, .Value = shiftflex})
    End Sub

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^0-9]+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub

    Private Sub AssignEvents()
        AddHandler btnCCancel.Click, AddressOf btnCCancel_Click
        AddHandler btnCUpdate.Click, AddressOf btnCUpdate_Click
    End Sub 'Assign events to buttons

    'Private Sub ClearFields()
    '    txtCEmpID.Clear()
    '    txtCCelNo.Clear()
    '    txtCEmail.Clear()
    '    txtCEmail2.Clear()
    '    txtCHome.Clear()
    '    txtCLocal.Clear()
    '    cbLocation.Text = String.Empty
    'End Sub

    'Private Sub LoadData()
    '    txtCCelNo.Text = contacts.CEL_NO
    '    txtCEmail.Text = contacts.EMAIL_ADDRESS
    '    txtCEmail2.Text = contacts.EMAIL_ADDRESS2
    '    txtCHome.Text = contacts.HOMEPHONE
    '    txtCOther.Text = contacts.OTHER_PHONE
    '    txtCEmpID.Text = contacts.EMP_ID
    '    txtCLocal.Text = contacts.LOCAL
    '    cbLocation.Text = contacts.LOCATION
    '    txtCNickName.Text = contacts.NICK_NAME
    'End Sub

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
            If contactVM.ContactProfile.CEL_NO = String.Empty AndAlso contactVM.ContactProfile.LOCATION = String.Empty Then
                MsgBox("Please Fill Up all the Fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                contactList.EmpID = contactVM.ContactProfile.EMP_ID
                contactList.LAST_NAME = contactVM.ContactProfile.LAST_NAME
                contactList.FIRST_NAME = contactVM.ContactProfile.FIRST_NAME
                contactList.MIDDLE_NAME = contactVM.ContactProfile.MIDDLE_NAME
                contactList.Nick_Name = contactVM.ContactProfile.NICK_NAME
                contactList.ACTIVE = contactVM.ContactProfile.ACTIVE
                contactList.BIRTHDATE = contactVM.ContactProfile.BDATE
                contactList.POSITION = contactVM.ContactProfile.POSITION
                contactList.DT_HIRED = contactVM.ContactProfile.DT_HIRED
                contactList.MARITAL_STATUS = contactVM.ContactProfile.MARITAL_STATUS
                contactList.IMAGE_PATH = contactVM.ContactProfile.IMAGE_PATH
                contactList.PERMISSION_GROUP = contactVM.ContactProfile.PERMISSION_GROUP
                contactList.DEPARTMENT = contactVM.ContactProfile.DEPARTMENT
                contactList.DIVISION = contactVM.ContactProfile.DIVISION
                contactList.DEPARTMENT = contactVM.ContactProfile.DEPARTMENT
                contactList.SHIFT = contactVM.ContactProfile.SHIFT
                contactList.EMADDRESS = contactVM.ContactProfile.EMAIL_ADDRESS
                contactList.EMADDRESS2 = contactVM.ContactProfile.EMAIL_ADDRESS2
                contactList.LOC = contactVM.ContactProfile.LOCATION
                contactList.CELL_NO = contactVM.ContactProfile.CEL_NO
                contactList.lOCAL = contactVM.ContactProfile.LOCAL
                contactList.HOUSEPHONE = contactVM.ContactProfile.HOMEPHONE
                contactList.OTHERPHONE = contactVM.ContactProfile.OTHER_PHONE
                contactList.DateReviewed = DateTime.Now.Date

                Dim result As Integer = MsgBox("Are you sure you want to continue?", MsgBoxStyle.OkCancel, "AIDE")
                If result = 1 Then
                    If InitializeService() Then
                        client.UpdateContactListByEmpID(contactList)
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
End Class
