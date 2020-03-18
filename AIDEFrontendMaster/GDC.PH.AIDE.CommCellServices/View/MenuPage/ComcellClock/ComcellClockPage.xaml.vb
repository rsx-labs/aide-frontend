Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Windows.Threading
Imports System.Windows.Media.Animation
Imports System.Media
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Net.Mail
Imports System.Configuration

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellClockPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declaration"
    Private aideService As ServiceReference1.AideServiceClient
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
    Private _OptionsViewModel As OptionViewModel
    Private _option As OptionModel
    Private isServiceEnabled As Boolean

    Private configMissingWeeklyStatus As New List(Of String)
    Private configMissingAttendance As New List(Of String)
    Private isRPNotifAllow As Boolean
    Private isWRNotifAllow As Boolean
    Private allowRPDays As String

    Private mailConfig As New MailConfig
    Private mailConfigVM As New MailConfigViewModel
    Private lstMissingReports As ContactList()
    Private lstMissingAttendance As Employee()

    Private contactListVM As ContactListViewModel

    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)
    Dim lastWeekSaturday As Date = saturday.AddDays(-14) 'For Missing reports label

    Dim dayFriDiff As Integer = Today.DayOfWeek - DayOfWeek.Friday
    Dim friday As Date = Today.AddDays(-dayFriDiff)
    Dim lastWeekFriday As Date = friday.AddDays(-7) ' For Missing reports label

    Public timer As DispatcherTimer = New DispatcherTimer()

    Dim enableNotification As Boolean = CBool(ConfigurationManager.AppSettings("enableNotification"))
#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, _comcellFrame As Frame, _window As Window)
        InitializeComponent()

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
    End Sub
#End Region

#Region "Service Methods"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False

        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aideService = New AideServiceClient(Context)
            aideService.Open()
            bInitialize = True
            isServiceEnabled = True
        Catch ex As SystemException
            aideService.Abort()
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

#Region "Methods/Functions"
    Private Sub GetAlarmClockData()
        Try
            If InitializeService() Then
                comcellclock = aideService.GetClockTimeByEmployee(empID)
                LoadData()
            End If

        Catch ex As Exception
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

            comcellClockVM.objectComcellClockSet = ComcellClockModel

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
            Dim AttendanceTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(Today.DayOfWeek)).ToString.Trim.ToUpper() & " " & configMissingAttendance(0)
            If actualTime = AttendanceTime And allowRPDays.Contains(Convert.ToInt32(Today.DayOfWeek).ToString()) Then
                GetAttendanceEmailData()
                SetMissingAttendance()
            End If
        End If

        If isWRNotifAllow And enableNotification Then
            Dim weeklyReportTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(configMissingWeeklyStatus(0))).ToString.Trim.ToUpper() & " " & configMissingWeeklyStatus(1)
            If actualTime = weeklyReportTime Then
                GetWeeklyReportEmailData()
                SetMissingReports()
            End If
        End If

    End Sub

    Public Sub refreshClock()
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 5, 0), DispatcherPriority.Normal, Function()
                                                                                                                 comcellFrame.Navigate(New ComcellClockPage(profile, comcellFrame, window))
                                                                                                             End Function, Me.Dispatcher)
    End Sub

    Public Sub SetComcellMinutes()
        Try
            If InitializeService() Then
                lstComcell = aideService.GetComcellMeeting(empID, year)
                LoadComcellMinutes()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadComcellMinutes()
        Dim ComcellToday As New ComcellModel
        Dim ComcellDBProvider As New ComcellDBProvider

        For Each comcellItem As Comcell In lstComcell
            If DateTime.ParseExact(comcellItem.MONTH, "MMMM", CultureInfo.CurrentCulture).Month = monthToday Then
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

        isRPNotifAllow = mailConfigVM.isSendEmail(8, 0, 0)
        isWRNotifAllow = mailConfigVM.isSendEmail(9, 0, 0)
        allowRPDays = GetOptionData(13, 0, 0)
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
            _OptionsViewModel = New OptionViewModel
            If _OptionsViewModel.GetOptions(4, 0, 0) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        configMissingWeeklyStatus = New List(Of String)(opt.VALUE.Split(","c))
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetWeeklyReportEmailData()
        Try
            _OptionsViewModel = New OptionViewModel
            _option = New OptionModel
            If _OptionsViewModel.GetOptions(5, 0, 0) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        _option = opt
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
        Dim strData As String = String.Empty
        Try
            _OptionsViewModel = New OptionViewModel
            If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        strData = opt.VALUE
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return strData
    End Function

    Public Sub SetMissingReports()
        Try
            Dim weekrange As String = lastWeekSaturday.ToShortDateString + " - " + lastWeekFriday.ToShortDateString
            lstMissingReports = aideService.GetMissingReportsByEmpID(empID, lastWeekSaturday)
            If lstMissingReports.Count > 0 Then
                For Each objContacts As ContactList In lstMissingReports
                    mailConfigVM.SendEmail(mailConfigVM, _option, objContacts.EMADDRESS, "", 1, weekrange)
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetMailConfig()
        Try
            If InitializeService() Then
                mailConfig = aideService.GetMailConfig()
                LoadMailConfig()
            End If

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

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#End Region
#Region "Email Notification - Missing Attendance"
    Private Sub GetAttendanceConfig()
        Try
            _OptionsViewModel = New OptionViewModel
            If _OptionsViewModel.GetOptions(2, 0, 0) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        configMissingAttendance = New List(Of String)(opt.VALUE.Split(","c))
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub GetAttendanceEmailData()
        Try
            _OptionsViewModel = New OptionViewModel
            _option = New OptionModel
            If _OptionsViewModel.GetOptions(3, 0, 0) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        _option = opt
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub SetMissingAttendance()
        Try
            lstMissingAttendance = aideService.GetMissingAttendanceForToday(empID)
            If lstMissingAttendance.Count > 0 Then
                For Each objEmployee As Employee In lstMissingAttendance
                    mailConfigVM.SendEmail(mailConfigVM, _option, objEmployee.ManagerEmail, objEmployee.EmailAddress, 2, objEmployee.EmployeeName)
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region
End Class
