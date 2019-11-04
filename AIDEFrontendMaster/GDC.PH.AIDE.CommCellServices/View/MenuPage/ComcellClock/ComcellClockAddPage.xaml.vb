Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Text.RegularExpressions

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellClockAddPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    Private empID As Integer
    Private comcellFrame As Frame
    Private _addframe As Frame
    Private aide As ServiceReference1.AideServiceClient
    Private _comcellclock As New ComcellClock
    Private comcellClockVM As New ComcellClockViewModel
    Private _window As Window
    Private pos As String
    Private profile As Profile
    Private comcellClockModel As New ComcellClockModel
#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, com_cellframe As Frame, addframe As Frame, winx As Window, _ComcellClockModel As ComcellClockModel)
        ' This call is required by the designer.
        InitializeComponent()
        Me.profile = _profile
        Me.empID = profile.Emp_ID
        Me.comcellFrame = com_cellframe
        Me._addframe = addframe
        Me._window = winx
        Me.comcellClockModel = _ComcellClockModel
        populateDayCB()
        DataContext = comcellClockVM
        LoadData()

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

#Region "Methods/Functions"

    Public Sub LoadData()
        ComcellDayCB.SelectedIndex = comcellClockModel.CLOCK_DAY - 1
        ComcellHourCB.Text = (comcellClockModel.CLOCK_HOUR / 30).ToString("00")
        ComcellMinuteCB.Text = (comcellClockModel.CLOCK_MINUTE / 6).ToString("00")
        ComcellTimeExtensionCB.Text = comcellClockModel.MIDDAY
    End Sub
    Private Sub SetDataDay()
        If Not ComcellDayCB.Text = String.Empty Then
            comcellClockVM.objectComcellClockSet.CLOCK_DAY = ConvertDays(ComcellDayCB.Text)
        End If
        If Not ComcellHourCB.Text = String.Empty Then
            comcellClockVM.objectComcellClockSet.CLOCK_HOUR = ComcellHourCB.Text
        End If
        If Not ComcellMinuteCB.Text = String.Empty Then
            comcellClockVM.objectComcellClockSet.CLOCK_MINUTE = ComcellMinuteCB.Text
        End If
        If Not Me.empID = 0 Then
            comcellClockVM.objectComcellClockSet.EMP_ID = Me.empID
        End If
        If Not Me.ComcellTimeExtensionCB.Text = String.Empty Then
            comcellClockVM.objectComcellClockSet.MIDDAY = Me.ComcellTimeExtensionCB.Text
        End If
    End Sub

    Public Sub SetData(clockVM As ComcellClockModel)
        Try
            SetDataDay()
            If Not clockVM.CLOCK_DAY = 0 AndAlso Not clockVM.CLOCK_HOUR = 0 AndAlso Not clockVM.MIDDAY = String.Empty Then
                If checkLimit() Then
                    If InitializeService() Then
                        _comcellclock.Clock_Day = clockVM.CLOCK_DAY
                        _comcellclock.Clock_Hour = clockVM.CLOCK_HOUR
                        _comcellclock.Clock_Minute = clockVM.CLOCK_MINUTE
                        _comcellclock.Emp_ID = clockVM.EMP_ID
                        _comcellclock.MIDDAY = clockVM.MIDDAY
                        aide.UpdateComcellClock(_comcellclock)
                        MsgBox("Successfully set new Comm. Cell clock", MsgBoxStyle.OkOnly, "AIDE")
                        comcellFrame.Navigate(New ComcellClockPage(profile, Me.comcellFrame, Me._window))
                    End If
                Else
                    MsgBox("Please check your time entry. Input hours should not exceed to 12 and Input Minutes should not exceed to 59.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            Else
                MsgBox("Please fill up all required fields!", MsgBoxStyle.Exclamation, "AIDE")
            End If
        Catch ex As Exception

        End Try
        ExitPage()
    End Sub

    Private Sub ExitPage()
        Dim window As MainWindow = DirectCast(_window, MainWindow)
        comcellFrame.Navigate(New ComcellClockPage(profile, Me.comcellFrame, _window))
        window.MenuGrid.IsEnabled = True
        window.MenuGrid.Opacity = 1
        window.PagesFrame.Opacity = 1
        window.PagesFrame.IsEnabled = True
        comcellFrame.IsEnabled = True
        comcellFrame.Opacity = 1
        window.SubMenuFrame.Opacity = 1
        window.SubMenuFrame.IsEnabled = True
        _addframe.Visibility = Visibility.Hidden
    End Sub

    Public Sub populateDayCB()
        For count As Integer = 1 To 5
            ComcellDayCB.Items.Add(getDays(count))
        Next
    End Sub

    Private Function getDays(count As Integer) As String
        Select Case count
            Case 1
                Return "Monday"
            Case 2
                Return "Tuesday"
            Case 3
                Return "Wednesday"
            Case 4
                Return "Thursday"
            Case 5
                Return "Friday"
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function ConvertDays(days As String) As Integer
        Select Case days
            Case "Monday"
                Return 1
            Case "Tuesday"
                Return 2
            Case "Wednesday"
                Return 3
            Case "Thursday"
                Return 4
            Case "Friday"
                Return 5
            Case "Saturday"
                Return 6
            Case "Sunday"
                Return 7
        End Select
    End Function

    Private Function checkLimit() As Boolean
        checkLimit = False
        If comcellClockVM.objectComcellClockSet.CLOCK_HOUR > 0 And comcellClockVM.objectComcellClockSet.CLOCK_HOUR <= 12 Then
            If comcellClockVM.objectComcellClockSet.CLOCK_MINUTE >= 0 AndAlso comcellClockVM.objectComcellClockSet.CLOCK_MINUTE < 60 Then
                checkLimit = True
            End If
        End If
        Return checkLimit
    End Function
#End Region

#Region "Events"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        Dim window As MainWindow = DirectCast(_window, MainWindow)
        comcellFrame.Navigate(New ComcellClockPage(profile, Me.comcellFrame, _window))
        window.MenuGrid.IsEnabled = True
        window.MenuGrid.Opacity = 1
        window.PagesFrame.Opacity = 1
        window.PagesFrame.IsEnabled = True
        comcellFrame.IsEnabled = True
        comcellFrame.Opacity = 1
        window.SubMenuFrame.Opacity = 1
        window.SubMenuFrame.IsEnabled = True
        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub UpdateBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            SetData(comcellClockVM.objectComcellClockSet)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ComcellMinuteCB_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub

    Private Sub ComcellHourCB_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub
#End Region
End Class
