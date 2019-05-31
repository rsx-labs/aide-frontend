Imports UI_AIDE_CommCellServices.ServiceReference1
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

Class MainWindow
    Implements IAideServiceCallback

#Region "Fields"
    Public email As String = "c.lim@ph.fujitsu.com"
    Private departmentID As Integer
    Private empID As Integer
    Private permission As Integer

    Dim profileDBProvider As New ProfileDBProvider
    Dim profileViewModel As New ProfileViewModel
    Dim profile As Profile
    Dim aideClientService As AideServiceClient
    Dim eventStartUpId As String = ConfigurationManager.AppSettings("eventStartUpId")
    Dim eventLogInId As String = ConfigurationManager.AppSettings("eventLogInId")
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
        InitializeComponent()
        InitializeService()
        getTime()
        'CheckOutlook()
        MsgBox("Welcome " & email, MsgBoxStyle.Information, "AIDE")
        SetEmployeeData()
        'attendance()
        LoadSideBar()
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, email, profile))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Public Sub New(_email As String)
        InitializeComponent()
        InitializeService()
        getTime()
        email = _email
        MsgBox("Welcome " & email, MsgBoxStyle.Information, "AIDE")
        SetEmployeeData()
        attendance()
        LoadSideBar()
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, email, profile))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub
#End Region

#Region "Common Methods"

    Public Function CheckOutlook() As Boolean
        Try

            Dim app As Outlook.Application
            app = System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application")
            email = app.Session.CurrentUser.AddressEntry.GetExchangeUser.PrimarySmtpAddress
            'email = Application.ActiveExplorer.Session.CurrentUser.Address
            'email = app.Session.CurrentUser.Address
            Return True
        Catch ex As Exception
            '    CheckOutlook()
            MsgBox("Please login to Outlook", MsgBoxStyle.Critical, "AIDE")
            Environment.Exit(0)
            Return Nothing
        End Try
    End Function

    Public Sub LoadSideBar()
        AttendanceFrame.Navigate(New AttendanceDashBoard(PagesFrame, profile))
        CommendationFrame.Navigate(New CommendationDashBoard(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, profile.Email_Address, profile))
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aideClientService = New AideServiceClient(Context)
            aideClientService.Open()
            bInitialize = True
        Catch ex As SystemException
            aideClientService.Abort()
        End Try
        Return bInitialize
    End Function

    Private Function SignOn() As Boolean
        Try
            Dim szReturn As Boolean
            If InitializeService() Then
                profile = aideClientService.SignOn(email)
                szReturn = True
            Else
                szReturn = False
            End If
            Return szReturn
        Catch ex As SystemException
            aideClientService.Abort()
            SetEmployeeData()
            Return False
        End Try
    End Function

    Public Sub SaveProfile(ByVal _profile As Profile)
        Try
            If Not IsNothing(_profile) Then
                EmployeeID = _profile.Emp_ID

                If (_profile.Permission = "Manager") Then
                    IsManagerSignedOn = True
                Else
                    IsManagerSignedOn = False
                End If
                profileDBProvider.SetMyProfile(_profile)
                profileViewModel.SelectedUser = New ProfileModel(profileDBProvider.GetMyProfile())
            End If
        Catch ex As SystemException
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetEmployeeData()
        Try
            If email <> String.Empty Then
                If Me.SignOn Then
                    IsSignedOn = True
                    SaveProfile(profile)
                End If
            Else
                SetEmployeeData()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub sendEmail()
        Dim OutlookMessage As outlook.MailItem
        Dim AppOutlook As New outlook.Application
        Try
            OutlookMessage = AppOutlook.CreateItem(outlook.OlItemType.olMailItem)
            Dim Recipents As outlook.Recipients = OutlookMessage.Recipients
            Recipents.Add("jhunellbarcenas@gmail.com")
            OutlookMessage.Subject = "Sending through Outlook"
            OutlookMessage.Body = "Testing outlook Mail"
            OutlookMessage.BodyFormat = outlook.OlBodyFormat.olFormatHTML
            OutlookMessage.Send()
        Catch ex As Exception
            MessageBox.Show("Mail could not be sent") 'if you dont want this message, simply delete this line 
        Finally
            OutlookMessage = Nothing
            AppOutlook = Nothing
        End Try
    End Sub

    Public Sub attendance()
        Try
            'Get Login Time
            Dim dateToday As Date = DateTime.Now.ToString("MM/dd/yyyy")
            Dim logName As EventLog = New EventLog()
            logName.Log = "System"
            Dim entries = logName.Entries.Cast(Of EventLogEntry)().Where(Function(x) x.InstanceId = CLng(eventStartUpId) And x.TimeWritten.Date = dateToday).[Select](Function(x) New With {x.MachineName, x.Site, x.Source, x.TimeWritten, x.InstanceId}).ToList()

            If entries.Count = 0 Then
                logName.Source = "Microsoft Windows security auditing."
                logName.Log = "Security"
                entries = logName.Entries.Cast(Of EventLogEntry)().Where(Function(x) x.InstanceId = CLng(eventLogInId) And x.TimeWritten.Date = dateToday).[Select](Function(x) New With {x.MachineName, x.Site, x.Source, x.TimeWritten, x.InstanceId}).ToList()
            End If

            Dim timeIn As String = entries.First().TimeWritten.ToString
            Dim attendanceSummarry As New AttendanceSummary
            attendanceSummarry.EmployeeID = EmployeeID
            attendanceSummarry.TimeIn = timeIn

            'Dim timeIn As String = DateTime.Now.ToString
            'Dim attendanceSummarry As New AttendanceSummary
            'attendanceSummarry.EmployeeID = EmployeeID
            'attendanceSummarry.TimeIn = timeIn

            If attendanceSummarry.EmployeeID = 0 Then 'Service time-out needs to be handled on the service or else always restart it when it time's out
                MsgBox("Service Time-Out! Attendance will not be Recorded!" + Environment.NewLine + "Application will Automatically Close.", MsgBoxStyle.Critical, "AIDE")
                Environment.Exit(0)
            Else
                aideClientService.InsertAttendance(attendanceSummarry)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Public Sub getTime()
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 0, 1), DispatcherPriority.Normal, Function()
                                                                                                                 Me.TimeTxt.Text = DateTime.Now.ToShortTimeString.ToString
                                                                                                             End Function, Me.Dispatcher)
        DateTxt.Text = Date.Now.ToLongDateString
    End Sub
    Private Sub dispatcherTimer_Tick()
        TimeTxt.Text = Date.Now.ToShortTimeString
        DateTxt.Text = Date.Now.ToLongDateString
    End Sub

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

    Private Sub ImprovementBtn_Click(sender As Object, e As RoutedEventArgs) Handles ImprovementBtn.Click
        LoadSideBar()
        PagesFrame.Navigate(New ThreeC_Page(email, PagesFrame, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New ImproveSubMenuPage(PagesFrame, email, profile, AddFrame, MenuGrid, SubMenuFrame))
    End Sub

    Private Sub HomeBtn_Click(sender As Object, e As RoutedEventArgs) Handles HomeBtn.Click
        LoadSideBar()
        PagesFrame.Navigate(New HomePage(PagesFrame, profile.Position, profile.Emp_ID, AddFrame, MenuGrid, SubMenuFrame, email, profile))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub ExitBtn_Click(sender As Object, e As RoutedEventArgs)
        If MsgBox("Are you sure to quit?", vbInformation + MsgBoxStyle.YesNo, "AIDE") = vbYes Then
            Environment.Exit(0)
        End If
    End Sub

    Private Sub EmployeesBtn_Click(sender As Object, e As RoutedEventArgs)
        LoadSideBar()
        PagesFrame.Navigate(New ContactListPage(PagesFrame, email, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub SkillsBtn_Click(sender As Object, e As RoutedEventArgs)
        empID = profile.Emp_ID

        LoadSideBar()

        If IsManagerSignedOn Then
            PagesFrame.Navigate(New SkillsMatrixManagerPage(empID))
        Else
            PagesFrame.Navigate(New SkillsMatrixPage(empID))
        End If
        'PagesFrame.Navigate(New ContactListPage(PagesFrame, email))
        SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub ProjectBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New CreateProjectPage(PagesFrame, profile.Emp_ID))
        SubMenuFrame.Navigate(New ProjectSubMenuPage(PagesFrame, profile.Emp_ID, email, AddFrame, MenuGrid, SubMenuFrame))
        LoadSideBar()
        'PagesFrame.Navigate(New ViewProjectUI(PagesFrame))
        'SubMenuFrame.Navigate(New BlankSubMenu())
    End Sub

    Private Sub TaskBtn_Click(sender As Object, e As RoutedEventArgs)
        'email = profile.Email_Address

        PagesFrame.Navigate(New TaskAdminPage(PagesFrame, Me, profile.Emp_ID, email, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New BlankSubMenu())
        LoadSideBar()
        'End If
    End Sub

    Private Sub AttendanceBtn_Click(sender As Object, e As RoutedEventArgs)
        email = profile.Email_Address
        empID = profile.Emp_ID

        PagesFrame.Navigate(New ResourcePlannerPage(profile, PagesFrame, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New BlankSubMenu())
        LoadSideBar()
    End Sub

    Private Sub BirthdayBtn_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub AssetsBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New AssetsListPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
        SubMenuFrame.Navigate(New AssetsSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
        LoadSideBar()
    End Sub

    Private Sub BillabilityBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New BillablesPage(profile, PagesFrame))
        SubMenuFrame.Navigate(New BillabilitySubMenu(PagesFrame, profile))
        LoadSideBar()
    End Sub
    Private Sub MinimizeBtn_Click(sender As Object, e As RoutedEventArgs)
        Me.WindowState = Windows.WindowState.Minimized
    End Sub

    Private Sub OtherBtn_Click(sender As Object, e As RoutedEventArgs)
        PagesFrame.Navigate(New BirthdayPage(PagesFrame, email))
        SubMenuFrame.Navigate(New OtherSubMenuPage(PagesFrame, profile, AddFrame, MenuGrid, SubMenuFrame))
        LoadSideBar()
    End Sub
#End Region

End Class
