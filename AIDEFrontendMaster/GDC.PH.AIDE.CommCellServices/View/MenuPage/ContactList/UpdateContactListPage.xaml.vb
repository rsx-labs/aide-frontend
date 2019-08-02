Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports System.Configuration

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class UpdateContactListPage
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
    Private old_empid As Integer
    Private contactVM As New ContactListViewModel

    Dim locationEco As String = ConfigurationManager.AppSettings("locationEco")
    Dim locationNet As String = ConfigurationManager.AppSettings("locationNet")
    Dim locationDurham As String = ConfigurationManager.AppSettings("locationDurham")
    Dim locationWfh As String = ConfigurationManager.AppSettings("locationWfh")
    
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
        ProcessUIAccess()
        AssignEvents()
        textLimits()
        LoadAllCB()
        loadUI(contactModel)
    End Sub
#End Region

#Region "Main Methods"
    Private Function GetManagerAuth() As Boolean
        If Me.profile.Position = "Manager" Then
            Return True
        End If

        Return False
    End Function

    Public Sub LoadAllCB()
        LoadLocation()
        LoadJobPosition()
        LoadPermission()
        LoadDepartment()
        LoadDivision()
        LoadMaritalStatus()
        LoadWorkShift()
    End Sub

    Public Sub loadUI(contactmod As ContactListModel)
        cbContactLocation.Text = contactmod.LOCATION

        cbContactPosition.SelectedValue = contactmod.POSITION_ID
        cbContactPosition.Text = contactmod.POSITION

        cbContactGroup.SelectedValue = contactmod.PERMISSION_GROUP_ID
        cbContactGroup.Text = contactmod.PERMISSION_GROUP

        cbContactDepartment.SelectedValue = contactmod.DEPARTMENT_ID
        cbContactDepartment.Text = contactmod.DEPARTMENT

        cbContactDivision.SelectedValue = contactmod.DIVISION_ID
        cbContactDivision.Text = contactmod.DIVISION

        cbContactMaritalStatus.SelectedValue = contactmod.MARITAL_STATUS_ID
        cbContactMaritalStatus.Text = contactmod.MARITAL_STATUS

        cbContactShiftStatus.Text = contactmod.SHIFT
    End Sub

    Private Sub ProcessUIAccess()
        If Not GetManagerAuth() Then
            ManagerAuthScreen.Visibility = Windows.Visibility.Visible
            btnCDelete.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Public Sub textLimits()
        txtContactCellNo.MaxLength = 11
        txtContactHomePhone.MaxLength = 15
        txtContactLocalNumber.MaxLength = 4
        txtContactOtherPhone.MaxLength = 15
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

    Public Sub LoadJobPosition()
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
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadPermission()
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
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadDepartment()
        Try
            If InitializeService() Then
                Dim lstDepartment As DepartmentList() = client.GetAllDepartment()
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
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadDivision()
        Try
            If InitializeService() Then
                Dim lstDivision As DivisionList() = client.GetAllDivision()
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
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Public Sub LoadMaritalStatus()
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
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadWorkShift()
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
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^0-9/]+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub

    Private Sub AssignEvents()
        AddHandler btnCCancel.Click, AddressOf btnCCancel_Click
        AddHandler btnCUpdate.Click, AddressOf btnCUpdate_Click
        AddHandler btnCDelete.Click, AddressOf btnCDelete_Click
    End Sub 'Assign events to buttons

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
            If Me.profile.Emp_ID = 0 OrElse _
                contactVM.ContactProfile.LAST_NAME = String.Empty OrElse _
                contactVM.ContactProfile.FIRST_NAME = String.Empty OrElse _
                contactVM.ContactProfile.NICK_NAME = String.Empty OrElse _
                IsNothing(contactVM.ContactProfile.BDATE) OrElse _
                IsNothing(contactVM.ContactProfile.DT_HIRED) OrElse _
                cbContactGroup.SelectedValue = Nothing OrElse _
                cbContactMaritalStatus.SelectedValue = Nothing OrElse _
                cbContactPosition.SelectedValue = Nothing OrElse _
                cbContactShiftStatus.SelectedValue = Nothing OrElse _
                cbContactDepartment.SelectedValue = Nothing OrElse _
                cbContactDivision.SelectedValue = Nothing OrElse _
                contactVM.ContactProfile.EMAIL_ADDRESS = String.Empty OrElse _
                contactVM.ContactProfile.CEL_NO = String.Empty OrElse _
                contactVM.ContactProfile.EMAIL_ADDRESS2 = String.Empty OrElse _
                cbContactLocation.SelectedValue = Nothing Then
                MsgBox("Please fill up all required fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                If MsgBox("Are you sure you want to continue?", vbYesNo, "AIDE") = vbYes Then
                    If InitializeService() Then
                        contactList.EmpID = contactVM.ContactProfile.EMP_ID
                        contactList.LAST_NAME = contactVM.ContactProfile.LAST_NAME.ToUpper()
                        contactList.FIRST_NAME = contactVM.ContactProfile.FIRST_NAME.ToUpper()
                        contactList.MIDDLE_NAME = contactVM.ContactProfile.MIDDLE_NAME.ToUpper()
                        contactList.Nick_Name = contactVM.ContactProfile.NICK_NAME.ToUpper()
                        contactList.ACTIVE = contactVM.ContactProfile.ACTIVE
                        contactList.BIRTHDATE = contactVM.ContactProfile.BDATE
                        contactList.DT_HIRED = contactVM.ContactProfile.DT_HIRED
                        contactList.IMAGE_PATH = contactVM.ContactProfile.IMAGE_PATH
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
                        client.UpdateContactListByEmpID(contactList, 0)
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

    Private Sub btnCDelete_Click(sender As Object, e As RoutedEventArgs) Handles btnCDelete.Click
        Try
            e.Handled = True
            contactVM.ContactProfile = DataContext
            Dim contactList As New ContactList
            If contactVM.ContactProfile.EMP_ID = 0 Then

            Else
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
                contactList.IMAGE_PATH = contactVM.ContactProfile.IMAGE_PATH
                contactList.PERMISSION_GROUP = contactVM.ContactProfile.PERMISSION_GROUP
                contactList.DEPARTMENT = contactVM.ContactProfile.DEPARTMENT
                contactList.DIVISION = contactVM.ContactProfile.DIVISION
                contactList.SHIFT = contactVM.ContactProfile.SHIFT
                contactList.EMADDRESS = contactVM.ContactProfile.EMAIL_ADDRESS
                contactList.EMADDRESS2 = contactVM.ContactProfile.EMAIL_ADDRESS2
                contactList.LOC = contactVM.ContactProfile.LOCATION
                contactList.CELL_NO = contactVM.ContactProfile.CEL_NO
                contactList.lOCAL = contactVM.ContactProfile.LOCAL
                contactList.HOUSEPHONE = contactVM.ContactProfile.HOMEPHONE
                contactList.OTHERPHONE = contactVM.ContactProfile.OTHER_PHONE
                contactList.DateReviewed = DateTime.Now.Date
                contactList.MARITAL_STATUS_ID = contactVM.ContactProfile.MARITAL_STATUS_ID
                contactList.POSITION_ID = contactVM.ContactProfile.POSITION_ID
                contactList.PERMISSION_GROUP_ID = contactVM.ContactProfile.PERMISSION_GROUP_ID
                contactList.DEPARTMENT_ID = 0
                contactList.DIVISION_ID = 0
                contactList.OLD_EMP_ID = old_empid

                If MsgBox("Are you sure you want to continue? Employee will be removed from the lists.", vbYesNo, "AIDE") = vbYes Then
                    If InitializeService() Then
                        client.UpdateContactListByEmpID(contactList, 0)
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
