Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Windows.Threading
Imports System.Windows.Media.Animation
Imports System.Media
Imports System.Collections.ObjectModel
Imports System.Globalization

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellClockPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declaration"
    Private aide As ServiceReference1.AideServiceClient
    Private emp_ID As Integer
    Private _comcellclock As New ComcellClock
    Private comcellClockVM As New ComcellClockViewModel
    Private comcellFrame As Frame
    Private _window As Window
    Private alarmActive As Boolean
    Private pos As String
    Private lstComcell() As Comcell
    Private year As Integer
    Private monthToday As Integer
    Private ComcellVM As New ComcellViewModel
#End Region

#Region "Constructor"
    Public Sub New(empID As Integer, com_cellFrame As Frame, winx As Window, _pos As String)
        InitializeComponent()
        year = Date.Now.Year
        monthToday = Date.Now.Month
        DataContext = comcellClockVM
        Me.comcellFrame = com_cellFrame
        Me.emp_ID = empID
        Me._window = winx
        Me.pos = _pos
        InitializeService()
        ManagerAuth(pos)
        GetData()
        setclock()
        getTime()
        refreshClock()
        SetData2()
    End Sub
#End Region

#Region "Service Methods"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aide = New AideServiceClient(Context)
            aide.Open()
            bInitialize = True
        Catch ex As SystemException
            aide.Abort()
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

#Region "Main Functions"
    Private Sub GetData()
        Try
            If InitializeService() Then
                _comcellclock = aide.GetClockTimeByEmployee(Me.emp_ID)
                LoadData()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub LoadData()
        'Dim AlarmDate As String
        Dim ComcellClockModel As New ComcellClockModel
        Dim ComcellClockDB As New ComcellClockDBProvider

        Try
            ComcellClockDB._setlistofitems(_comcellclock)
            ComcellClockModel = New ComcellClockModel(ComcellClockDB._getobjClock)

            comcellClockVM.objectComcellClockSet = ComcellClockModel
            comcellClockVM.objectComcellSetAlarm = GetDayValue(ComcellClockModel.CLOCK_DAY) + ComcellClockModel.CLOCK_HOUR.ToString() + ComcellClockModel.CLOCK_MINUTE.ToString() + "1"
        Catch ex As Exception

        End Try
    End Sub

    Public Sub ManagerAuth(pos As String)
        If pos = "Manager" Then
            btnCreate.Visibility = Windows.Visibility.Visible
        Else
            btnCreate.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Public Sub setclock()
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
        Dim dayconvert As String = GetDayValue(comcellClockVM.objectComcellClockSet.CLOCK_DAY)
        comcellClockVM.objectComcellDayOnly = dayconvert
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
#End Region


    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        comcellFrame.Navigate(New ComcellClockAddPage(Me.emp_ID, Me.comcellFrame, _window, Me.pos))
    End Sub

    Public Sub getTime()
        Dim actualTime As String
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 0, 1), DispatcherPriority.Normal, Function()
                                                                                                                 actualTime = DateTime.Now.DayOfWeek.ToString().ToUpper() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()
                                                                                                                 If TimeCheck(actualTime) Then
                                                                                                                     'alarmActive = True
                                                                                                                     Dim winwin As New ComcellClockWindow
                                                                                                                     winwin.ShowDialog()
                                                                                                                     If _window.WindowState = WindowState.Minimized Then
                                                                                                                         _window.Show()

                                                                                                                     End If
                                                                                                                 End If
                                                                                                             End Function, Me.Dispatcher)
    End Sub

    Public Function TimeCheck(timenow As String) As Boolean
        TimeCheck = False
        If timenow = comcellClockVM.objectComcellSetAlarm Then
            TimeCheck = True
        End If
    End Function

    Public Sub refreshClock()
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 5, 0), DispatcherPriority.Normal, Function()
                                                                                                                 comcellFrame.Navigate(New ComcellClockPage(emp_ID, comcellFrame, _window, pos))
                                                                                                             End Function, Me.Dispatcher)
    End Sub

    Public Sub SetData2()
        Try
            If InitializeService() Then
                lstComcell = aide.GetComcellMeeting(emp_ID, year)
                loadComcell()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub loadComcell()
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
            ComcellVM.ComcellItem = ComcellToday
            Facilitator.Text = ComcellVM.ComcellItem.FACILITATOR.ToUpper()
            MinutesTaker.Text = ComcellVM.ComcellItem.MINUTES_TAKER.ToUpper()
        End If


    End Sub



    'Private Sub StopBtn_Click(sender As Object, e As RoutedEventArgs)
    '    GridAlarm.Tag = String.Empty
    '    StopBtn.Tag = String.Empty
    '    GridAlarm.Background = New BrushConverter().ConvertFrom("#FFD8D8D8")
    '    alarmActive = False
    '    StopBtn.Visibility = False
    'End Sub
End Class
