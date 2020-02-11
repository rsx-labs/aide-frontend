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

#End Region

#Region "Constructor"
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

        AssignEvents()
        textLimits()
        LoadAllCB()
    End Sub
#End Region

#Region "Service methods"

#End Region

#Region "Main methods"
    Private Sub LoadAllCB()
        LoadLocation()
        LoadJobPosition()
        LoadPermission()
        LoadMaritalStatus()
        LoadWorkShift()
    End Sub

    Private Sub AssignEvents()
        AddHandler btnCCancel.Click, AddressOf btnCCancel_Click
        AddHandler btnCreate.Click, AddressOf btnCreate_Click
    End Sub

    Private Sub textLimits()
        txtContactCellNo.MaxLength = 11
        txtContactHomePhone.MaxLength = 15
        txtContactLocalNumber.MaxLength = 4
        txtContactOtherPhone.MaxLength = 15
        txtContactEmpID.MaxLength = 9
        txtContactMName.MaxLength = 1
    End Sub

    Private Sub LoadLocation()
        If InitializeService() Then
            Dim lstLocation As LocationList() = client.GetAllLocation()
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
        End If
    End Sub

    Private Sub LoadJobPosition()
        Try
            If InitializeService() Then
                Dim lstPosition As PositionList() = client.GetAllPosition()
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
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadPermission()
        Try
            If InitializeService() Then
                Dim lstPermission As PermissionList() = client.GetAllPermission()
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
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadMaritalStatus()
        Try
            If InitializeService() Then
                Dim lstMarital As StatusList() = client.GetAllStatus("EMPLOYEE")
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
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadWorkShift()
        Try
            If InitializeService() Then
                Dim lstWorkShift As StatusList() = client.GetAllStatus("WORK_SHIFT")
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
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ClearTextVal()
        txtContactEmpID.Text = String.Empty

    End Sub

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^0-9/]+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub
    Private Sub AlphaValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^A-Za-z-.']+")
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
            If contactVM.ContactProfile.EMP_ID = 0 OrElse _
                txtContactEmpID.Text = String.Empty OrElse _
                Me.profile.Emp_ID = 0 OrElse _
                contactVM.ContactProfile.LAST_NAME = String.Empty OrElse _
                contactVM.ContactProfile.FIRST_NAME = String.Empty OrElse _
                contactVM.ContactProfile.NICK_NAME = String.Empty OrElse _
                IsNothing(contactVM.ContactProfile.BDATE) OrElse _
                IsNothing(contactVM.ContactProfile.DT_HIRED) OrElse _
                cbContactGroup.SelectedValue = Nothing OrElse _
                cbContactMaritalStatus.SelectedValue = Nothing OrElse _
                cbContactPosition.SelectedValue = Nothing OrElse _
                cbContactShiftStatus.SelectedValue = Nothing OrElse _
                contactVM.ContactProfile.EMAIL_ADDRESS = String.Empty OrElse _
                contactVM.ContactProfile.CEL_NO = String.Empty OrElse _
                cbContactLocation.SelectedValue = Nothing Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                contactList.EmpID = contactVM.ContactProfile.EMP_ID
                contactList.LAST_NAME = contactVM.ContactProfile.LAST_NAME.ToUpper()
                contactList.FIRST_NAME = contactVM.ContactProfile.FIRST_NAME.ToUpper()
                contactList.MIDDLE_NAME = contactVM.ContactProfile.MIDDLE_NAME.ToUpper()
                contactList.Nick_Name = contactVM.ContactProfile.NICK_NAME.ToUpper()
                contactList.ACTIVE = 1
                contactList.BIRTHDATE = contactVM.ContactProfile.BDATE
                contactList.DT_HIRED = contactVM.ContactProfile.DT_HIRED
                contactList.IMAGE_PATH = contactVM.ContactProfile.IMAGE_PATH
                contactList.DEPARTMENT = contactVM.ContactProfile.DEPARTMENT
                contactList.DIVISION = contactVM.ContactProfile.DIVISION
                contactList.DEPARTMENT_ID = contactVM.ContactProfile.DEPARTMENT_ID
                contactList.DIVISION_ID = contactVM.ContactProfile.DIVISION_ID
                contactList.EMADDRESS = contactVM.ContactProfile.EMAIL_ADDRESS
                contactList.EMADDRESS2 = contactVM.ContactProfile.EMAIL_ADDRESS2
                contactList.CELL_NO = contactVM.ContactProfile.CEL_NO
                If contactVM.ContactProfile.LOCAL Is Nothing Then
                    contactList.lOCAL = 0
                Else
                    contactList.lOCAL = contactVM.ContactProfile.LOCAL
                End If

                contactList.HOUSEPHONE = contactVM.ContactProfile.HOMEPHONE
                contactList.OTHERPHONE = contactVM.ContactProfile.OTHER_PHONE
                contactList.DateReviewed = DateTime.Now.Date

                contactList.OLD_EMP_ID = user_empid

                contactList.LOC = cbContactLocation.SelectedValue.ToString 

                contactList.POSITION_ID = CInt(cbContactPosition.SelectedValue)
                contactList.POSITION = cbContactPosition.Text

                contactList.PERMISSION_GROUP_ID = CInt(cbContactGroup.SelectedValue)
                contactList.PERMISSION_GROUP = cbContactGroup.Text

                contactList.SHIFT = cbContactShiftStatus.Text

                contactList.MARITAL_STATUS_ID = CInt(cbContactMaritalStatus.SelectedValue)
                contactList.MARITAL_STATUS = cbContactMaritalStatus.Text



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
            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
