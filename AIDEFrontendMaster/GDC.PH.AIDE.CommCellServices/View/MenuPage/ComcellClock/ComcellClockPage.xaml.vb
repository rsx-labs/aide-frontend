Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Windows.Threading
Imports System.Windows.Media.Animation
Imports System.Media
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Net.Mail
Imports System.Configuration
Imports NLog

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellClockPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declaration"
    'Private aideService As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private comcellclock As New ComcellClock
    Private comcellClockVM As New ComcellClockViewModel
    Private comcellFrame As Frame
    Private window As Window
    Private alarmActive As Boolean
    Private pos As String
    Private lstComcell() As Comcell
    Private year As Integer
    Private monthToday As Integer
    Private comcellVM As New ComcellViewModel
    Private profile As Profile
    Private comcellClockModel As New ComcellClockModel
    'Private _OptionsViewModel As OptionViewModel
    Private _option As OptionModel
    Private isServiceEnabled As Boolean

    Private configMissingWeeklyStatus As New List(Of String)
    Private configMissingAttendanceSemiFlex As New List(Of String)
    Private configMissingAttendanceFlex As New List(Of String)
    Private configMissingAttendanceDispatch As New List(Of String)
    Private configUpdateContacts As New List(Of String)
    Private configUpdateSkills As New List(Of String)
    Private configUpdateWorkPlaceAudit As New List(Of String)
    Private configUpdateWorkPlaceAuditDaily As New List(Of String)
    Private configUpdateWorkPlaceAuditWeekly As New List(Of String)
    Private configUpdateWorkPlaceAuditMonthly As New List(Of String)
    Private isRPNotifAllow As Boolean
    Private isWRNotifAllow As Boolean
    Private isCNotifAllow As Boolean
    Private isSMNotifAllow As Boolean
    Private isWANotifAllow As Boolean
    Private allowRPDays As String
    Private allowRPDaysDispatch As String

    Private mailConfig As New MailConfig
    Private mailConfigVM As New MailConfigViewModel
    Private lstMissingReports As ContactList()
    Private lstMissingAttendance As Employee()
    Private objAuditor As Employee

    Private contactListVM As ContactListViewModel

    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)
    Dim lastWeekSaturday As Date = saturday.AddDays(-14) 'For Missing reports label

    Dim dayFriDiff As Integer = Today.DayOfWeek - DayOfWeek.Friday
    Dim friday As Date = Today.AddDays(-dayFriDiff)
    Dim lastWeekFriday As Date = friday.AddDays(-7) ' For Missing reports label

    Public timer As DispatcherTimer = New DispatcherTimer()

    Dim enableNotification As Boolean = CBool(ConfigurationManager.AppSettings("enableNotification"))

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, _comcellFrame As Frame, _window As Window)

        _logger.Debug("Start : Constructor")

        InitializeComponent()

        'Me.aideService = aideService
        mailConfigVM = New MailConfigViewModel()

        year = Date.Now.Year
        monthToday = Date.Now.Month

        DataContext = comcellClockVM
        comcellFrame = _comcellFrame
        profile = _profile
        empID = profile.Emp_ID
        window = _window
        year = getSelectedFY(year, monthToday)
        GetMailConfig()
        LoadAllEmailNotifConfig()
        GetAlarmClockData()
        SetAlarmClock()
        SetTime()
        'refreshClock()
        SetComcellMinutes()

        'load controls
        ManagerAuth()

        _logger.Debug("End : Constructor")

    End Sub
#End Region

#Region "Service Methods"
    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False

    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        aideService = New AideServiceClient(Context)
    '        aideService.Open()
    '        bInitialize = True
    '        isServiceEnabled = True
    '    Catch ex As SystemException
    '        aideService.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try

    '    Return bInitialize
    'End Function

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

#Region "Methods/Functions"
    Private Sub GetAlarmClockData()
        Try
            'If InitializeService() Then
            comcellclock = AideClient.GetClient().GetClockTimeByEmployee(empID)
            LoadData()
            'End If

        Catch ex As Exception
            _logger.Error($"Error at GetAlarmClockData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Function getSelectedFY(Year As Integer, month As Integer)
        Select Case month
            Case 1
                Year = Year - 1
            Case 2
                Year = Year - 1
            Case 3
                Year = Year - 1
            Case Else
                Year = Year
        End Select

        Return Year
    End Function



    Private Sub LoadData()
        'Dim AlarmDate As String
        comcellClockModel = New ComcellClockModel
        Dim comcellClockDB As New ComcellClockDBProvider
        Dim newHour As String = String.Empty

        Try
            comcellClockDB._setlistofitems(comcellclock)
            comcellClockModel = New ComcellClockModel(comcellClockDB._getobjClock)

            comcellClockVM.objectComcellClockSet = comcellClockModel

            If comcellClockModel.MIDDAY = "PM" Then
                newHour = comcellClockModel.CLOCK_HOUR
                If Not comcellClockModel.CLOCK_HOUR = 12 Then
                    newHour = (comcellClockModel.CLOCK_HOUR + 12).ToString()
                End If
            Else
                newHour = comcellClockModel.CLOCK_HOUR
                If comcellClockModel.CLOCK_HOUR = 12 Then
                    newHour = "0"
                End If
            End If

            comcellClockVM.objectComcellSetAlarm = GetDayValue(comcellClockModel.CLOCK_DAY) + newHour.ToString() + comcellClockModel.CLOCK_MINUTE.ToString() + "1"
        Catch ex As Exception
            _logger.Error($"Error at LoadData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub ManagerAuth()
        If profile.Permission_ID = 1 Or profile.FirstName.ToUpper = Facilitator.Text Or profile.FirstName.ToUpper = MinutesTaker.Text Then
            btnCreate.Visibility = Windows.Visibility.Visible
        Else
            btnCreate.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Public Sub SetAlarmClock()
        CalculateHour()
        CalculateMinute()
        GetComcellDay()
    End Sub

    Private Sub CalculateMinute()
        Dim AngleMinute As Double
        AngleMinute = comcellClockVM.objectComcellClockSet.CLOCK_MINUTE * 6
        comcellClockVM.objectComcellClockSet.CLOCK_MINUTE = AngleMinute
    End Sub

    Private Sub CalculateHour()
        Dim AngleHour As Double
        AngleHour = comcellClockVM.objectComcellClockSet.CLOCK_HOUR * 30
        comcellClockVM.objectComcellClockSet.CLOCK_HOUR = AngleHour
    End Sub

    Private Sub GetComcellDay()
        Try
            Dim dayconvert As String = GetDayValue(comcellClockVM.objectComcellClockSet.CLOCK_DAY)
            comcellClockVM.objectComcellDayOnly = dayconvert & " " & comcellclock.Clock_Hour.ToString("00") & ":" & comcellclock.Clock_Minute.ToString().PadLeft(2, "0") & ":00" & " " & comcellclock.MIDDAY
        Catch ex As Exception
            _logger.Error($"Error at GetCommCellDay = {ex.ToString()}")

            MsgBox("Please set Comm. Cell time.", MsgBoxStyle.Critical, "AIDE")
        End Try
    End Sub

    Private Function GetDayValue(ByVal daycount As Integer) As String
        Select Case daycount
            Case 1
                Return "MONDAY"
            Case 2
                Return "TUESDAY"
            Case 3
                Return "WEDNESDAY"
            Case 4
                Return "THURSDAY"
            Case 5
                Return "FRIDAY"
            Case Else
                Return String.Empty
        End Select
    End Function

    Public Sub SetTime()
        AddHandler timer.Tick, New EventHandler(AddressOf Timer_Click)
        timer.Interval = New TimeSpan(0, 0, 1)
        timer.Start()
    End Sub

    Private Sub Timer_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dateNow As DateTime = Format(DateTime.Now, "hh:mm:ss tt")
        Dim actualTime As String
        Dim actualTimeMonthly As String

        actualTime = DateTime.Now.DayOfWeek.ToString().ToUpper() + " " + dateNow.ToString("hh:mm:ss tt")

        Dim comcellTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(comcellclock.Clock_Day)).ToString.Trim.ToUpper() & " " & comcellclock.Clock_Hour.ToString("00") & ":" & comcellclock.Clock_Minute.ToString().PadLeft(2, "0") & ":00" & " " & comcellclock.MIDDAY
        If actualTime = comcellTime Then
            Dim winwin As New ComcellClockWindow
            winwin.ShowDialog()
            If window.WindowState = WindowState.Minimized Then
                window.Show()
            End If
        End If

        If isRPNotifAllow And enableNotification Then
            Dim AttendanceTimeSemiFlex As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(Today.DayOfWeek)).ToString.Trim.ToUpper() & " " & configMissingAttendanceSemiFlex(0)
            If actualTime = AttendanceTimeSemiFlex And allowRPDays.Contains(Convert.ToInt32(Today.DayOfWeek).ToString()) Then
                GetAttendanceEmailData()
                SetMissingAttendance(1)
            End If

            Dim AttendanceTimeFlex As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(Today.DayOfWeek)).ToString.Trim.ToUpper() & " " & configMissingAttendanceFlex(0)
            If actualTime = AttendanceTimeSemiFlex And allowRPDays.Contains(Convert.ToInt32(Today.DayOfWeek).ToString()) Then
                GetAttendanceEmailData()
                SetMissingAttendance(2)
            End If

            Dim AttendanceTimeDispatch As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(Today.DayOfWeek)).ToString.Trim.ToUpper() & " " & configMissingAttendanceDispatch(0)
            If actualTime = AttendanceTimeDispatch And allowRPDaysDispatch.Contains(Convert.ToInt32(Today.DayOfWeek).ToString()) Then
                GetAttendanceEmailData()
                SetMissingAttendance(3)
            End If
        End If

        If isWRNotifAllow And enableNotification Then
            Dim weeklyReportTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(configMissingWeeklyStatus(0))).ToString.Trim.ToUpper() & " " & configMissingWeeklyStatus(1)
            If actualTime = weeklyReportTime Then
                GetWeeklyReportEmailData()
                SetMissingReports()
            End If
        End If

        If isCNotifAllow And enableNotification Then
            actualTime = Now.ToString("MM/dd/yyyy") & " " & dateNow.ToString("hh:mm:ss tt")
            Dim ContactsTime As String = Now.Month.ToString("00") & "/" & CInt(configUpdateContacts(0)).ToString("00") & "/" & Now.Year.ToString("0000") & " " & configUpdateContacts(1)
            If actualTime = ContactsTime Then
                GetContactsEmailData()
                SetUpdateContacts()
            End If
        End If

        If isSMNotifAllow And enableNotification Then
            actualTime = Now.ToString("MM/dd/yyyy") & " " & dateNow.ToString("hh:mm:ss tt")
            If configUpdateSkills.Count > 0 Then
                Dim SkillsTime As String = Now.Month.ToString("00") & "/" & CInt(configUpdateSkills(0)).ToString("00") & "/" & Now.Year.ToString("0000") & " " & configUpdateSkills(1)
                If actualTime = SkillsTime Then
                    GetSkillsEmailData()
                    SetUpdateSkills()
                End If
            End If
        End If

        If isWANotifAllow And enableNotification Then
            actualTime = DateTime.Now.DayOfWeek.ToString().ToUpper() + " " + dateNow.ToString("hh:mm:ss tt")
            actualTimeMonthly = Now.ToString("MM/dd/yyyy") & " " & dateNow.ToString("hh:mm:ss tt")
            Dim WAWeeklyTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(configUpdateWorkPlaceAuditWeekly(0))).ToString.Trim.ToUpper() & " " & configUpdateWorkPlaceAudit(0)
            Dim WADailyTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(Today.DayOfWeek)).ToString.Trim.ToUpper() & " " & configUpdateWorkPlaceAudit(0)
            Dim WAMonthlyTime As String = Now.Month.ToString("00") & "/" & CInt(configUpdateWorkPlaceAuditMonthly(0)).ToString("00") & "/" & Now.Year.ToString("0000") & " " & configUpdateWorkPlaceAudit(0)
            If actualTime = WADailyTime And configUpdateWorkPlaceAuditDaily.Contains(Convert.ToInt32(Today.DayOfWeek).ToString()) Then
                GetWorkPlaceAuditEmailData()
                SetUpdateWADaily()
            End If
            If actualTime = WAWeeklyTime Then
                GetWorkPlaceAuditEmailData()
                SetUpdateWAWeekly()
            End If
            If actualTimeMonthly = WAMonthlyTime Then
                GetWorkPlaceAuditEmailData()
                SetUpdateWAMonthly()
            End If

        End If
    End Sub

    Public Sub refreshClock()
        Dim timer As DispatcherTimer = New DispatcherTimer(
            New TimeSpan(0, 5, 0),
            DispatcherPriority.Normal,
            Function()
                comcellFrame.Navigate(New ComcellClockPage(profile, comcellFrame, window))
            End Function, Me.Dispatcher)
    End Sub

    Public Sub SetComcellMinutes()
        Try
            'If InitializeService() Then
            lstComcell = AideClient.GetClient().GetComcellMeeting(empID, year)
            LoadComcellMinutes()
            'End If
        Catch ex As Exception
            _logger.Error($"Error at SetComCellMinutes = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadComcellMinutes()
        Dim ComcellToday As New ComcellModel
        Dim ComcellDBProvider As New ComcellDBProvider

        For Each comcellItem As Comcell In lstComcell
            If comcellItem.WEEK = AppState.GetInstance().CurrentWeek Then
                ComcellDBProvider.SetMyComcellItem(comcellItem)
                Exit For
            End If
        Next

        If Not ComcellDBProvider.GetMyComcellItem.COMCELL_ID = 0 Then
            ComcellToday = New ComcellModel(ComcellDBProvider.GetMyComcellItem)
            comcellVM.ComcellItem = ComcellToday
            Facilitator.Text = comcellVM.ComcellItem.FACILITATOR_NAME.ToUpper()
            MinutesTaker.Text = comcellVM.ComcellItem.MINUTES_TAKER_NAME.ToUpper()
        End If
    End Sub

    Public Sub LoadAllEmailNotifConfig()
        GetWeeklyReportConfig()
        GetAttendanceConfig()
        GetContactsConfig()
        GetSkillsConfig()
        GetWorkPlaceAuditConfig()
        GetWADailyConfig()
        GetWAWeeklyConfig()
        GetWAMonthlyConfig()

        isRPNotifAllow = mailConfigVM.isSendEmail(8, 0, 0)
        isWRNotifAllow = mailConfigVM.isSendEmail(9, 0, 0)
        isCNotifAllow = mailConfigVM.isSendEmail(11, 0, 0)
        isSMNotifAllow = mailConfigVM.isSendEmail(12, 0, 0)
        isWANotifAllow = mailConfigVM.isSendEmail(37, 0, 0)
        allowRPDays = AppState.GetInstance().OptionValueDictionary(Constants.OPT_RPDAYS)
        allowRPDaysDispatch = AppState.GetInstance().OptionValueDictionary(Constants.OPT_RPDAYSDISPATCH)
    End Sub



#End Region

#Region "Events"
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        Dim mainWindow As MainWindow = DirectCast(window, MainWindow)
        Dim addframe As Frame = mainWindow.AddFrame
        addframe.Visibility = Visibility.Visible
        addframe.Navigate(New ComcellClockAddPage(profile, comcellFrame, addframe, window, comcellClockModel, Me))
        mainWindow.MenuGrid.Opacity = 0.3
        mainWindow.MenuGrid.IsEnabled = False
        mainWindow.SubMenuFrame.Opacity = 0.3
        mainWindow.SubMenuFrame.IsEnabled = False
        mainWindow.PagesFrame.Opacity = 0.3
        mainWindow.PagesFrame.IsEnabled = False
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(200, 160, 200, 160)
    End Sub

    'Private Sub StopBtn_Click(sender As Object, e As RoutedEventArgs)
    '    GridAlarm.Tag = String.Empty
    '    StopBtn.Tag = String.Empty
    '    GridAlarm.Background = New BrushConverter().ConvertFrom("#FFD8D8D8")
    '    alarmActive = False
    '    StopBtn.Visibility = False
    'End Sub
#End Region

#Region "Email Notification - Missing Weekly Report"
    Private Sub GetWeeklyReportConfig()
        Try

            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_WEEKLY_CONFIG) Is Nothing Then
                configMissingWeeklyStatus = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_WEEKLY_CONFIG).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWeeklyReportConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetWeeklyReportEmailData()
        Try
            _option = New OptionModel
            If Not AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_MISSING_WEEKLY_REPORT) Is Nothing Then
                _option =
                    AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_MISSING_WEEKLY_REPORT)

            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWeeklyReportData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '    Dim strData As String = String.Empty
    '    Try
    '        _OptionsViewModel = New OptionViewModel
    '        '_OptionsViewModel.Service = aideService
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

    Public Sub SetMissingReports()
        Try
            Dim weekrange As String = lastWeekSaturday.ToShortDateString + " - " + lastWeekFriday.ToShortDateString
            lstMissingReports = AideClient.GetClient().GetMissingReportsByEmpID(empID, lastWeekSaturday)
            If lstMissingReports.Count > 0 Then
                For Each objContacts As ContactList In lstMissingReports
                    mailConfigVM.SendEmail(mailConfigVM, _option, objContacts.EMADDRESS, "", 1, weekrange)
                Next
            End If
        Catch ex As Exception
            _logger.Error($"Error at SetMissingReports = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetMailConfig()
        Try
            'If InitializeService() Then
            mailConfig = AideClient.GetClient().GetMailConfig()
            LoadMailConfig()
            'End If

        Catch ex As Exception

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub LoadMailConfig()

        Dim MConfigModel As New MailConfigModel
        Dim MConfigProvider As New MailConfigDBProvider

        Try
            MConfigProvider._setlistofitems(mailConfig)
            MConfigModel = New MailConfigModel(MConfigProvider._getobjmailconfig)

            mailConfigVM.objectMailConfigSet = MConfigModel

        Catch ex As Exception
            _logger.Error($"Error at LoadMailConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#End Region
#Region "Email Notification - Missing Attendance"
    Private Sub GetAttendanceConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_CHECK_ATTENDANCE_SEMI_FLEX) Is Nothing Then
                configMissingAttendanceSemiFlex = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_CHECK_ATTENDANCE_SEMI_FLEX).Split(","c)
                 )
            End If

            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_CHECK_ATTENDANCE_FLEXI) Is Nothing Then
                configMissingAttendanceFlex = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_CHECK_ATTENDANCE_FLEXI).Split(","c)
                 )
            End If

            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_CHECK_ATTENDANCE_DISPATCH) Is Nothing Then
                configMissingAttendanceDispatch = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_CHECK_ATTENDANCE_DISPATCH).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetAttendanceConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetAttendanceEmailData()
        Try
            _option = New OptionModel
            If Not AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_MISSING_ATTENDANCE) Is Nothing Then
                _option =
                    AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_MISSING_ATTENDANCE)

            End If
        Catch ex As Exception
            _logger.Error($"Error at GetAttendanceEmailData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetMissingAttendance(choice As Integer)
        Try
            lstMissingAttendance = AideClient.GetClient().GetMissingAttendanceForToday(empID, choice)
            Dim employeeCCLst As New List(Of String)
            Dim managerToLst As New List(Of String)

            If lstMissingAttendance.Count > 0 Then
                Dim objLst As List(Of String) = Nothing
                For Each objEmployee As Employee In lstMissingAttendance
                    objLst = New List(Of String)(objEmployee.EmailAddress.Split(","c))
                    For Each obj As String In objLst
                        If Not employeeCCLst.Contains(obj) Then
                            employeeCCLst.Add(obj)
                        End If
                    Next
                    objLst = New List(Of String)(objEmployee.ManagerEmail.Split(","c))
                    For Each obj As String In objLst
                        If Not managerToLst.Contains(obj) Then
                            managerToLst.Add(obj)
                        End If
                    Next
                Next
                SendEmail(mailConfigVM, _option, managerToLst, employeeCCLst, 1, lstMissingAttendance)
            End If
        Catch ex As Exception
            _logger.Error($"Error atSetMissingAttendace = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SendEmail(ByVal mcVM As MailConfigViewModel, ByVal optmodel As OptionModel, LstemailTo As List(Of String), LstemailCC As List(Of String), ByVal bodyType As Integer, ByVal EmployeeList As Employee())
        Try

            Dim sentFrom As String = mcVM.objectMailConfigSet.SENDER_EMAIL
            Dim subject As String = mcVM.objectMailConfigSet.SUBJECT

            Dim body As String = ComposeBody(optmodel, bodyType, EmployeeList)
            Dim client As SmtpClient = New SmtpClient()

            client.Port = mcVM.objectMailConfigSet.PORT
            client.Host = mcVM.objectMailConfigSet.HOST
            client.EnableSsl = CBool(mcVM.objectMailConfigSet.ENABLE_SSL)
            client.Timeout = mcVM.objectMailConfigSet.TIMEOUT
            client.DeliveryMethod = SmtpDeliveryMethod.Network
            client.UseDefaultCredentials = CBool(mcVM.objectMailConfigSet.USE_DFLT_CREDENTIALS)
            client.Credentials = New System.Net.NetworkCredential(sentFrom, mcVM.objectMailConfigSet.SENDER_PASSWORD)

            Dim mail As MailMessage = New MailMessage()
            mail.From = New MailAddress(sentFrom)

            If Not LstemailTo Is Nothing Then
                For Each objSentTo As String In LstemailTo
                    mail.To.Add(objSentTo)
                Next
            End If

            If Not LstemailCC Is Nothing Then
                For Each objSentCC As String In LstemailCC
                    mail.CC.Add(objSentCC)
                Next
            End If

            mail.Subject = subject
            mail.IsBodyHtml = True
            mail.Body = body

            client.Send(mail)
        Catch ex As Exception
            _logger.Error($"Error at SendEmail = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Function ComposeBody(ByVal optmodel As OptionModel, ByVal choice As Integer, ByVal EmpList As Employee()) As String
        Dim body As String
        Dim bodyList As New List(Of String)(optmodel.VALUE.Split(","c))
        Dim strOptionLst As List(Of String) = Nothing
        Dim optList As String = String.Empty
        Dim objDate As String = String.Empty
        For Each objEmp As Employee In EmpList
            If objEmp.WeekDate = Today.ToString("yyyy-MM-dd") Then
                objDate = "TODAY"
            Else
                objDate = CDate(objEmp.WeekDate).ToString("MM-dd-yyyy")
            End If
            optList = optList + "<tr><td><center><b><font size=""5"">" + objDate + "</font></b></center></td></tr>"
            strOptionLst = New List(Of String)(objEmp.EmployeeName.Split(","c))
            For Each obj As String In strOptionLst
                optList = optList + "<tr><td><center>" + obj + "</center></td></tr>"
            Next
            optList = optList + "<br>"
        Next

        Select Case choice
            Case 1
                body = "<html><body><div style=""margin:30px 0px""><center><div style=""background-color:steelblue""><font size=""6"" color=""white"">" + optmodel.MODULE_DESCR + " - " + optmodel.FUNCTION_DESCR + "</font></div><div style=""background-color:#fcfff9""><font size=""4"">" + bodyList(0) + " " + bodyList(1) + "</font><table style=""width:100%"">" + optList + "</table></div></center></div></body></html>"
        End Select

        Return body
    End Function
#End Region
#Region "Update Contact List"
    Private Sub GetContactsConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_UPDATE_CONTACT) Is Nothing Then
                configUpdateContacts = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_UPDATE_CONTACT).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetContactsConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetContactsEmailData()
        Try
            _option = New OptionModel
            If Not AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_UPDATE_CONTACT_DATA) Is Nothing Then
                _option =
                    AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_UPDATE_CONTACT_DATA)

            End If
        Catch ex As Exception
            _logger.Error($"Error at GetContactsEmailData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetUpdateContacts()
        Try
            lstMissingAttendance = AideClient.GetClient().GetSkillAndContactsNotUpdated(empID, 1)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.EmailAddress, "", 1, MonthName(Now.Month(), False))
                Next
            End If
        Catch ex As Exception
            _logger.Error($"Error at SetUpdateConfigs = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region
#Region "Update Skill Matrix"
    Private Sub GetSkillsConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_UPDATE_SKILLS) Is Nothing Then
                configUpdateContacts = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_UPDATE_SKILLS).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetSkillsConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetSkillsEmailData()
        Try
            _option = New OptionModel
            If Not AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_UPDATE_SKILLS_DATA) Is Nothing Then
                _option =
                    AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_UPDATE_SKILLS_DATA)

            End If
        Catch ex As Exception
            _logger.Error($"Error at GetSkillsEmailData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetUpdateSkills()
        Try
            lstMissingAttendance = AideClient.GetClient().GetSkillAndContactsNotUpdated(empID, 2)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.EmailAddress, "", 1, MonthName(Now.Month(), False))
                Next
            End If
        Catch ex As Exception
            _logger.Error($"Error at SetUpdateSkills = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

#Region "Workplace Audit - Daily"
    Private Sub GetWorkPlaceAuditConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_TIME) Is Nothing Then
                configUpdateWorkPlaceAudit = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_TIME).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWorkplaceAuditConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub GetWADailyConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_DAY) Is Nothing Then
                configUpdateWorkPlaceAuditDaily = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_DAY).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWADailyConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub GetWorkPlaceAuditEmailData()
        Try
            _option = New OptionModel
            If Not AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_DAILY_WPA_DATA) Is Nothing Then
                _option =
                    AppState.GetInstance().OptionDictionary(Constants.OPT_REPORT_DAILY_WPA_DATA)

            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWAAuditEmailData = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetUpdateWADaily()
        Try
            objAuditor = AideClient.GetClient().GetWorkPlaceAuditor(empID, 1)
            If Not objAuditor Is Nothing Then
                mailConfigVM.SendEmail(mailConfigVM, _option, objAuditor.EmailAddress, "", 1, "Daily Auditor")
            End If
        Catch ex As Exception
            _logger.Error($"Error at SetUpdateWADaily = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region
#Region "Workplace Audit - Weekly"
    Private Sub GetWAWeeklyConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_WEEKLY) Is Nothing Then
                configUpdateWorkPlaceAuditWeekly = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_WEEKLY).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWAWeeklyConfig = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetUpdateWAWeekly()
        Try
            objAuditor = AideClient.GetClient().GetWorkPlaceAuditor(empID, 2)
            If Not objAuditor Is Nothing Then
                mailConfigVM.SendEmail(mailConfigVM, _option, objAuditor.EmailAddress, "", 1, "Weekly Auditor")
            End If
        Catch ex As Exception
            _logger.Error($"Error at SetUpdateWAWeekly = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

#Region "Workplace Audit - Weekly"
    Private Sub GetWAMonthlyConfig()
        Try
            If Not AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_MONTHLY) Is Nothing Then
                configUpdateWorkPlaceAuditMonthly = New List(Of String)(
                    AppState.GetInstance().OptionValueDictionary(Constants.OPT_REPORT_DAILY_WPA_MONTHLY).Split(","c)
                 )
            End If
        Catch ex As Exception
            _logger.Error($"Error at GetWAMonthlyConfig= {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetUpdateWAMonthly()
        Try
            objAuditor = AideClient.GetClient().GetWorkPlaceAuditor(empID, 3)
            If Not objAuditor Is Nothing Then
                mailConfigVM.SendEmail(mailConfigVM, _option, objAuditor.EmailAddress, "", 1, "Monthly Auditor")
            End If
        Catch ex As Exception
            _logger.Error($"Error at SetUpdateWAMonthly = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

End Class
