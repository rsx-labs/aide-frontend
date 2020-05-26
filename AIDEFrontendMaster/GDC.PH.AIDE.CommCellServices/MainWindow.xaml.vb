﻿Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports System.Windows.Threading
Imports System.Runtime.InteropServices
Imports Outlook = Microsoft.Office.Interop.Outlook
Imports System.Reflection
Imports System.Diagnostics.Eventing.Reader
Imports System.Security
Imports System.Configuration
Imports NLog
Imports Microsoft.VisualBasic.ApplicationServices
Imports GDC.PH.AIDE.ServiceFactory

Class MainWindow
    Implements IAideServiceCallback

#Region "Fields"
    Public email As String
    Private departmentID As Integer
    Private empID As Integer
    Private permission As Integer
    Private objMutex As System.Threading.Mutex
    Private _OptionsViewModel As OptionViewModel
    Private _instance As Integer
    Dim profileDBProvider As New ProfileDBProvider
    Dim profileViewModel As New ProfileViewModel
    Dim profile As Profile

    Dim machineOS As String = My.Computer.Info.OSFullName
    Dim guestPermission As Integer = 5

    'Private _aideClientService As AideServiceClient
    'Private AppState.GetInstance() As AppState

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

#Region "Property declarations"
    Private _EmailAddress As String
    Public Property EmailAddress() As String
        Get
            Return _EmailAddress
        End Get
        Set(ByVal value As String)
            _EmailAddress = value
        End Set
    End Property

    Private _EmployeeID As Integer
    Public Property EmployeeID() As Integer
        Get
            Return _EmployeeID
        End Get
        Set(ByVal value As Integer)
            _EmployeeID = value
        End Set
    End Property

    Private _DeptID As Integer
    Public Property DeptID() As Integer
        Get
            Return _DeptID
        End Get
        Set(ByVal value As Integer)
            _DeptID = value
        End Set
    End Property

    Private _PosID As String
    Public Property PosID() As String
        Get
            Return _PosID
        End Get
        Set(ByVal value As String)
            _PosID = value
        End Set
    End Property

    Private _EmployeeName As String
    Public Property EmployeeName() As String
        Get
            Return _EmployeeName
        End Get
        Set(ByVal value As String)
            _EmployeeName = value
        End Set
    End Property

    Private _IsManagerSignedOn As Boolean
    Public Property IsManagerSignedOn() As Boolean
        Get
            Return _IsManagerSignedOn
        End Get
        Set(ByVal value As Boolean)
            _IsManagerSignedOn = value
        End Set
    End Property

    Private _IsSignedOn As Boolean
    Public Property IsSignedOn() As Boolean
        Get
            Return _IsSignedOn
        End Get
        Set(ByVal value As Boolean)
            _IsSignedOn = value
        End Set
    End Property

#End Region

#Region "Constructors"
    Public Sub New()
        Dim optIDDefUser As Integer = 15
        Dim modIdDefUser As Integer = 4
        Dim funcIdDefUser As Integer = 8

        With Assembly.GetExecutingAssembly().GetName().Version
            _logger.Info("****** Starting AIDE " & .Major & "." & .Minor & "." & .Build & " ******")
        End With

        _logger.Debug("Start : Constructor")

        LoadOnce()
        InitializeComponent()
        InitializeService()
        GetTime()

        GetOptionData()
        'Dim useOutlook As Boolean = False
        'Boolean.TryParse(enableOutlook, useOutlook)

        If AppState.GetInstance().UseOutlook Then
            CheckOutlook()
        Else
            email = AppState.GetInstance().OptionValueDictionary(Constants.OPT_DEFAULT_EMAIL)
        End If

        InitializeData()

        _logger.Debug("End : Constructor")
    End Sub

    Public Sub New(_email As String)

        _logger.Debug($"Start : Constructor with {_email}")

        InitializeComponent()
        'InitializeService()
        GetTime()
        email = _email

        InitializeData()

        _logger.Debug("End : Constructor")
    End Sub
#End Region

#Region "Common Methods"

    Private Sub InitializeData()

        _logger.Debug("Start : InitializeData")

        SetEmployeeData()
        Attendance()
        LoadVersionNo()
        LoadSideBar()

        _logger.Info("Show greeting box and navigate to home screen")

        MsgBox("Welcome " & email & ".", MsgBoxStyle.Information, "AIDE")
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, email, profile))
        SubMenuFrame.Navigate(New BlankSubMenu())

        _logger.Debug("End : InitializeData")
    End Sub

    Public Function CheckOutlook() As Boolean
        Dim bCheckOutlook As Boolean

        _logger.Debug("Start : CheckOutlook")

        Try
            Dim app As Outlook.Application
            app = System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application")
            email = app.Session.CurrentUser.AddressEntry.GetExchangeUser.PrimarySmtpAddress
            'email = Application.ActiveExplorer.Session.CurrentUser.Address
            'email = app.Session.CurrentUser.Address
            bCheckOutlook = True
        Catch ex As Exception
            _logger.Error(ex.ToString())

            If MsgBox("Outlook is not running. Do you want to proceed with AIDE without Outlook?", MsgBoxStyle.Critical + vbYesNo, "AIDE") = vbYes Then
                Dim addwindow As New AddEmailWindow()
                addwindow.ShowDialog()
                email = addwindow.GetEmail
            Else
                Environment.Exit(0)
                Return Nothing
            End If
        End Try

        _logger.Debug("End : CheckOutlook")

        Return bCheckOutlook
    End Function

    Public Sub LoadSideBar()
        _logger.Debug("Start : LoadSideBar")

        AttendanceFrame.Navigate(New AttendanceDashBoard(PagesFrame, profile))
        genericFrame.Navigate(New _3CDashboard(email, PagesFrame, AddFrame, MenuGrid, SubMenuFrame, profile))
        'CommendationFrame.Navigate(New CommendationDashBoard(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, profile.Email_Address, profile, CommendationFrame))
        BirthdayFrame.Navigate(New BirthdayDashboard(profile.Email_Address))
        ComcellClockFrame.Navigate(New ComcellClockPage(profile, ComcellClockFrame, Me))

        _logger.Debug("End : LoadSideBar")
    End Sub

    Public Function InitializeService() As Boolean

        _logger.Debug("Start : InitializeService")

        Dim bInitialize As Boolean = False
        Try

            Dim Context As InstanceContext = New InstanceContext(Me)
            AideClient.CreateInstance(Context)

            '_aideClientService = New AideServiceClient(Context)

            _logger.Info("Opening AIDE service ...")

            '_aideClientService.Open()
            bInitialize = True
        Catch ex As SystemException

            _logger.Error($"Failed to initialize service. {ex.ToString()}")

            '_aideClientService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : InitializeService")
        Return bInitialize
    End Function

    Private Function SignOn() As Boolean
        _logger.Debug("Start : SignOn")

        Dim szReturn As Boolean

        Try
            'If InitializeService() Then
            profile = AideClient.GetClient().SignOn(email)
            If profile Is Nothing Then
                szReturn = False
            Else
                szReturn = True
            End If
        Catch ex As SystemException
            _logger.Error(ex.ToString())

            '_aideClientService.Abort()
            szReturn = False
        End Try

        _logger.Debug($"End : SignOn result = {szReturn}")
        Return szReturn
    End Function

    Public Sub SaveProfile(ByVal _profile As Profile)
        _logger.Debug("Start : SaveProfile")

        Try
            If Not IsNothing(_profile) Then
                EmployeeID = _profile.Emp_ID

                If (_profile.Permission_ID = 1) Then
                    IsManagerSignedOn = True
                Else
                    IsManagerSignedOn = False
                End If

                profileDBProvider.SetMyProfile(_profile)
                profileViewModel.SelectedUser = New ProfileModel(profileDBProvider.GetMyProfile())
            End If
        Catch ex As SystemException
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("Start : SaveProfile")
    End Sub

    Private Sub SetEmployeeData()

        _logger.Debug("Start : SetEmployeeData")

        Try
            If email <> String.Empty Then
                If SignOn() Then
                    IsSignedOn = True
                    SaveProfile(profile)
                End If
            Else
                MsgBox("Please login to Outlook.", MsgBoxStyle.Critical, "AIDE")
                Environment.Exit(0)
            End If
        Catch ex As Exception
            _logger.Error(ex.ToString())
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : SetEmployeeData")

    End Sub

    Public Sub Attendance()

        _logger.Debug("Start : Attendance")

        Try
            'Get Login Time
            Dim eventStartUpId As String = AppState.GetInstance().OptionValueDictionary(Constants.OPT_STARTUP_ID)
            If machineOS.Contains("Windows 7") Then
                eventStartUpId = "12"
            End If

            Dim dateToday As Date = DateTime.Now.ToString("MM/dd/yyyy")
            Dim logName As EventLog = New EventLog()
            logName.Log = "System"

            _logger.Debug("Getting system event time.")

            Dim entries = logName.Entries.Cast(Of EventLogEntry) _
                            ().Where(Function(x) x.InstanceId = CLng(eventStartUpId) _
                                        And x.TimeWritten.Date = dateToday) _
                           .[Select](Function(x) New With {x.MachineName,
                                        x.Site, x.Source, x.TimeWritten, x.InstanceId}) _
                           .ToList()

            Dim timeIn As String

            If entries.Count = 0 Then
                timeIn = Date.Now
            Else
                timeIn = entries.First().TimeWritten.ToString
            End If

            Dim attendanceSummarry As New AttendanceSummary

            attendanceSummarry.EmployeeID = EmployeeID
            attendanceSummarry.TimeIn = timeIn

            If profile Is Nothing Then 'Service time-out needs to be handled on the service or else always restart it when it time's out

                _logger.Error("Service times out")

                MsgBox("Service timed out. Application will close automatically." _
                        + Environment.NewLine + "Please note that no attendance will be recorded.",
                       MsgBoxStyle.Critical, "AIDE")

                Environment.Exit(0)
            Else
                If Not profileDBProvider.GetMyProfile.Permission_ID = guestPermission Then

                    _logger.Info("Recording attendance ...")

                    AideClient.GetClient().InsertAttendanceByEmpID(attendanceSummarry)
                End If
            End If
        Catch ex As Exception
            _logger.Error(ex.ToString())
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : Attendance")

    End Sub

    Public Sub GetTime()

        Dim timer As DispatcherTimer = New DispatcherTimer(
            New TimeSpan(0, 0, 1),
            DispatcherPriority.Normal,
            Sub()
                Me.TimeTxt.Text = DateTime.Now.ToShortTimeString.ToString
            End Sub,
            Me.Dispatcher
        )

        DateTxt.Text = Date.Now.ToLongDateString
    End Sub

    Private Sub dispatcherTimer_Tick()
        TimeTxt.Text = Date.Now.ToShortTimeString
        DateTxt.Text = Date.Now.ToLongDateString
    End Sub

    Private Sub LoadOnce()
        _logger.Debug("Start : LoadOnce")
        _logger.Debug($"Checking for existing instance. Count = {_instance}")

        _instance += 1
        'Check to prevent running twice
        objMutex = New System.Threading.Mutex(False, "AIDE")
        If objMutex.WaitOne(0, False) = False Then
            objMutex.Close()
            objMutex = Nothing
            Me.Focus()
            Me.Topmost = True

            'display the error only once
            If _instance = 2 Then
                MessageBox.Show("Another instance of AIDE is already running.")
            End If

            _logger.Error("Another instance of AIDE is already running.")
            End
        End If

        _logger.Debug("End : LoadOnce")

    End Sub

    Private Sub LoadVersionNo()
        With Assembly.GetExecutingAssembly().GetName().Version
            txtTitle.Text = "Adaptive Intelligent Dashboard for Employees " & .Major & "." & .Minor & "." & .Build
        End With
    End Sub

    Private Function GetOptionData() As Boolean

        _logger.Debug("Start : GetOptionData")

        Dim strData As String = String.Empty
        Try
            _OptionsViewModel = New OptionViewModel
            _OptionsViewModel.Service = AideClient.GetClient()
            If _OptionsViewModel.GetOptions(0, 0, 0) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        AppState.GetInstance().OptionValueDictionary.Add(
                                opt.OPTION_ID,
                                opt.VALUE
                            )
                        AppState.GetInstance().OptionDictionary.Add(
                            opt.OPTION_ID,
                            opt
                        )
                    End If
                Next
            End If

            Return True
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            _logger.Error($"Error : {ex.ToString()}")

            Return False
        End Try

        _logger.Debug("End : GetOptionData")

    End Function

#End Region

#Region "Notify Functions"
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

#Region "Button/Events"
    Private Sub AboutBtn_Click(sender As Object, e As RoutedEventArgs)
        Dim AboutPage As New AboutPage()
        AboutPage.ShowDialog()
    End Sub

    Private Sub ImprovementBtn_Click(sender As Object, e As RoutedEventArgs) Handles ImprovementBtn.Click

        _logger.Debug("Start : ImprovementBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If
        PagesFrame.Navigate(New ThreeC_Page(profile, PagesFrame, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New ImproveSubMenuPage(PagesFrame, email, profile, AddFrame, MenuGrid, SubMenuFrame))


        _logger.Debug("End : ImprovementBtn_Click")
    End Sub

    Private Sub HomeBtn2_Click(sender As Object, e As RoutedEventArgs) Handles HomeBtn2.Click

        _logger.Debug("Start : HomeBtn2_Click")

        LoadSideBar()
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, email, profile))
        SubMenuFrame.Navigate(New BlankSubMenu())

        _logger.Debug("End : HomeBtn2_Click")

        'If MenuGrid.Visibility = Windows.Visibility.Visible Then
        '    MenuGrid.Visibility = Windows.Visibility.Collapsed
        '    AttendanceGrid.Visibility = Windows.Visibility.Collapsed
        '    SideBarGrid.Visibility = Windows.Visibility.Collapsed
        '    MainGrid.SetValue(Grid.RowProperty, 0)
        '    MainGrid.SetValue(Grid.RowSpanProperty, 3)
        '    MainGridParent.SetValue(Grid.ColumnProperty, 0)
        '    MainGridParent.SetValue(Grid.ColumnSpanProperty, 3)
        'Else
        '    AttendanceGrid.Visibility = Windows.Visibility.Visible
        '    SideBarGrid.Visibility = Windows.Visibility.Visible
        '    MenuGrid.Visibility = Windows.Visibility.Visible
        '    MainGrid.SetValue(Grid.RowSpanProperty, 1)
        '    MainGridParent.SetValue(Grid.ColumnSpanProperty, 1)
        '    MainGrid.SetValue(Grid.RowProperty, 1)
        '    MainGridParent.SetValue(Grid.ColumnProperty, 1)
        'End If

        'LoadSideBar()
        'PagesFrame.Navigate(New AuditSchedMainPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
        'SubMenuFrame.Navigate(New AuditSchedSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
    End Sub

    Private Sub ExitBtn_Click(sender As Object, e As RoutedEventArgs)
        _logger.Debug("Start : ExitBtn_Click. Logging off...")

        If profile.Permission_ID = guestPermission Then
            Dim result = MsgBox("Do you want to close the application?", vbQuestion + MsgBoxStyle.YesNo, "AIDE")

            If result = MsgBoxResult.Yes Then
                _logger.Info("****** Closing AIDE ******")
                Environment.Exit(0)
            End If
        Else
            Dim result = MsgBox("Do you want to logoff?" & vbCrLf & vbCrLf &
                            "Click YES to logoff." & vbCrLf &
                            "Click NO to close the application only.", vbQuestion + MsgBoxStyle.YesNoCancel, "AIDE")

            Dim logoffTime As Date = DateTime.Now

            If result = MsgBoxResult.Yes Then
                'If InitializeService() Then
                _logger.Info("Inserting logoff time ...")
                    _logger.Info("****** Closing AIDE ******")

                AideClient.GetClient().InsertLogoffTime(profile.Emp_ID, logoffTime)
                Environment.Exit(0)
                'End If
            ElseIf result = MsgBoxResult.No Then
                _logger.Info("****** Closing AIDE ******")
                Environment.Exit(0)
            End If
        End If

        _logger.Debug("End : ExitBtn_Click.")
    End Sub

    Private Sub EmployeesBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : EmployeesBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If
        PagesFrame.Navigate(New ContactListPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame, AttendanceFrame))
        SubMenuFrame.Navigate(New BlankSubMenu())

        _logger.Debug("Start : EmployeesBtn_Click")

    End Sub

    Private Sub SkillsBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : SkillsBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If
        PagesFrame.Navigate(New SkillsMatrixManagerPage(profile, IsManagerSignedOn))
        SubMenuFrame.Navigate(New BlankSubMenu())

        _logger.Debug("End : SkillsBtn_Click")

    End Sub

    Private Sub ProjectBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : ProjectBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If
        PagesFrame.Navigate(New CreateProjectPage(PagesFrame, profile))
        SubMenuFrame.Navigate(New ProjectSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))


        _logger.Debug("End : ProjectBtn_Click")

        'PagesFrame.Navigate(New ViewProjectUI(PagesFrame))
        'SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub TaskBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : TaskBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If

        PagesFrame.Navigate(New TaskAdminPage(PagesFrame, Me, profile, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New TaskSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame, Me))


        _logger.Debug("End : TaskBtn_Click")

    End Sub

    Private Sub AttendanceBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : AttendanceBtn_Click")

        email = profile.Email_Address
        empID = profile.Emp_ID

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If

        PagesFrame.Navigate(New ResourcePlannerPage(profile, PagesFrame, AddFrame, MenuGrid, SubMenuFrame, AttendanceFrame))
        SubMenuFrame.Navigate(New AttendanceSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame, AttendanceFrame))

        _logger.Debug("End : AttendanceBtn_Click")

    End Sub

    Private Sub AssetsBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : AssetsBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If

        PagesFrame.Navigate(New AssetsListPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New AssetsSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))

        _logger.Debug("End : AssetsBtn_Click")

    End Sub

    Private Sub BillabilityBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : BillabilityBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If

        PagesFrame.Navigate(New BillablesPage(profile, PagesFrame))
        SubMenuFrame.Navigate(New BillabilitySubMenu(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))

        _logger.Debug("End : BillabilityBtn_Click")

    End Sub
    Private Sub MinimizeBtn_Click(sender As Object, e As RoutedEventArgs)
        Me.WindowState = Windows.WindowState.Minimized
    End Sub

    Private Sub OtherBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : OtherBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If

        PagesFrame.Navigate(New BirthdayPage(PagesFrame, email))
        SubMenuFrame.Navigate(New OtherSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))


        _logger.Debug("End : OtherBtn_Click")

    End Sub

    Private Sub DashboardBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : DashboardBtn_Click")

        LoadSideBar()
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, email, profile))
        SubMenuFrame.Navigate(New BlankSubMenu())

        _logger.Debug("End : DashboardBtn_Click")

    End Sub

    Private Sub WorkPlaceAuditBtn_Click(sender As Object, e As RoutedEventArgs)

        _logger.Debug("Start : WorkPlaceAuditBtn_Click")

        If AppState.GetInstance().ReloadSideBar Then
            LoadSideBar()
        End If
        PagesFrame.Navigate(New AuditSchedMainPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New AuditSchedSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))

        _logger.Debug("End : WorkPlaceAuditBtn_Click")

    End Sub
#End Region

End Class
