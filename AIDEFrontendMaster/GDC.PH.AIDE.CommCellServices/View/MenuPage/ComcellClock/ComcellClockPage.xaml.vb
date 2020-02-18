﻿Imports UI_AIDE_CommCellServices.ServiceReference1
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
    Private profile As Profile
    Private ComcellClockModel As New ComcellClockModel
    Dim isServiceEnabled As Boolean

#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, com_cellFrame As Frame, winx As Window)
        InitializeComponent()
        year = Date.Now.Year
        monthToday = Date.Now.Month
        DataContext = comcellClockVM
        Me.comcellFrame = com_cellFrame
        Me.profile = _profile
        Me.emp_ID = profile.Emp_ID
        Me._window = winx
        year = getSelectedFY(year, monthToday)
        InitializeService()
        GetData()
        setclock()
        getTime()
        'refreshClock()
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
            isServiceEnabled = True
        Catch ex As SystemException
            aide.Abort()
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
    Public Sub getTime()
        Dim actualTime As String
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 0, 1), DispatcherPriority.Normal, Function()

                                                                                                                 Dim dateNow As DateTime = Format(DateTime.Now, "hh:mm:ss tt")
                                                                                                                 actualTime = DateTime.Now.DayOfWeek.ToString().ToUpper() + " " + dateNow.ToString("hh:mm:ss tt")


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

    Private Sub GetData()
        Try
            If InitializeService() Then
                _comcellclock = aide.GetClockTimeByEmployee(Me.emp_ID)
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
        ComcellClockModel = New ComcellClockModel
        Dim ComcellClockDB As New ComcellClockDBProvider
        Dim NewHour As String = String.Empty
        Try
            ComcellClockDB._setlistofitems(_comcellclock)
            ComcellClockModel = New ComcellClockModel(ComcellClockDB._getobjClock)

            comcellClockVM.objectComcellClockSet = ComcellClockModel

            If ComcellClockModel.MIDDAY = "PM" Then
                NewHour = ComcellClockModel.CLOCK_HOUR
                If Not ComcellClockModel.CLOCK_HOUR = 12 Then
                    NewHour = (ComcellClockModel.CLOCK_HOUR + 12).ToString()
                End If
            Else
                NewHour = ComcellClockModel.CLOCK_HOUR
                If ComcellClockModel.CLOCK_HOUR = 12 Then
                    NewHour = "0"
                End If
            End If
            comcellClockVM.objectComcellSetAlarm = GetDayValue(ComcellClockModel.CLOCK_DAY) + NewHour.ToString() + ComcellClockModel.CLOCK_MINUTE.ToString() + "1"
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
        Try
            Dim dayconvert As String = GetDayValue(comcellClockVM.objectComcellClockSet.CLOCK_DAY)
            comcellClockVM.objectComcellDayOnly = dayconvert & " " & _comcellclock.Clock_Hour.ToString("00") & ":" & _comcellclock.Clock_Minute.ToString().PadLeft(2, "0") & ":00" & " " & _comcellclock.MIDDAY
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


    Public Function TimeCheck(timenow As String) As Boolean

        If isServiceEnabled Then
            _comcellclock = aide.GetClockTimeByEmployee(Me.emp_ID)
        Else
            Try
                If InitializeService() Then
                    _comcellclock = aide.GetClockTimeByEmployee(Me.emp_ID)
                    LoadData()
                End If

            Catch ex As Exception
                MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try
        End If


        Dim UpdateComcellTime As String = [Enum].GetName(GetType(DayOfWeek), Convert.ToInt32(_comcellclock.Clock_Day)).ToString.Trim.ToUpper() & " " & _comcellclock.Clock_Hour.ToString("00") & ":" & _comcellclock.Clock_Minute.ToString().PadLeft(2, "0") & ":00" & " " & _comcellclock.MIDDAY


        TimeCheck = False
        If timenow = UpdateComcellTime Then
            TimeCheck = True
        End If
    End Function

    Public Sub refreshClock()
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 5, 0), DispatcherPriority.Normal, Function()
                                                                                                                 comcellFrame.Navigate(New ComcellClockPage(profile, comcellFrame, _window))
                                                                                                             End Function, Me.Dispatcher)
    End Sub

    Public Sub SetData2()
        Try
            If InitializeService() Then
                lstComcell = aide.GetComcellMeeting(emp_ID, year)
                loadComcell()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
            Facilitator.Text = ComcellVM.ComcellItem.FACILITATOR_NAME.ToUpper()
            MinutesTaker.Text = ComcellVM.ComcellItem.MINUTES_TAKER_NAME.ToUpper()
        End If
        'load controls
        ManagerAuth()
    End Sub
#End Region

#Region "Events"
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)

        Dim window As MainWindow = DirectCast(_window, MainWindow)
        Dim addframe As Frame = window.AddFrame
        addframe.Visibility = Visibility.Visible
        addframe.Navigate(New ComcellClockAddPage(profile, comcellFrame, addframe, _window, ComcellClockModel))
        window.MenuGrid.Opacity = 0.3
        window.MenuGrid.IsEnabled = False
        window.SubMenuFrame.Opacity = 0.3
        window.SubMenuFrame.IsEnabled = False
        window.PagesFrame.Opacity = 0.3
        window.PagesFrame.IsEnabled = False
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

End Class
