Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Text.RegularExpressions

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellClockAddPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    Private empID As Integer
    Private comcellFrame As Frame
    Private addframe As Frame
    Private aideService As ServiceReference1.AideServiceClient
    Private comcellclock As New ComcellClock
    Private comcellClockVM As New ComcellClockViewModel
    Private window As Window
    Private pos As String
    Private profile As Profile
    Private comcellClockModel As New ComcellClockModel
    Private comcellClockPage As ComcellClockPage
#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, _comcellframe As Frame, _addframe As Frame, _window As Window, _comcellClockModel As ComcellClockModel, _comcellClockPage As ComcellClockPage)
        ' This call is required by the designer.
        InitializeComponent()
        profile = _profile
        empID = profile.Emp_ID
        comcellFrame = _comcellframe
        comcellClockPage = _comcellClockPage
        addframe = _addframe
        window = _window
        comcellClockModel = _comcellClockModel
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
            aideService = New AideServiceClient(Context)
            aideService.Open()
            bInitialize = True
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
                        comcellclock.Clock_Day = clockVM.CLOCK_DAY
                        comcellclock.Clock_Hour = clockVM.CLOCK_HOUR
                        comcellclock.Clock_Minute = clockVM.CLOCK_MINUTE
                        comcellclock.Emp_ID = clockVM.EMP_ID
                        comcellclock.MIDDAY = clockVM.MIDDAY
                        aideService.UpdateComcellClock(comcellclock)
                        MsgBox("The Comm. Cell time has been set.", MsgBoxStyle.OkOnly, "AIDE")
                        comcellClockPage.timer.IsEnabled = False 'Stop the previous timer
                        ExitPage()
                    End If
                Else
                    MsgBox("Please enter a valid time.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            Else
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ExitPage()
        Dim mainWindow As MainWindow = DirectCast(window, MainWindow)
        comcellFrame.Navigate(New ComcellClockPage(profile, comcellFrame, window))
        mainWindow.MenuGrid.IsEnabled = True
        mainWindow.MenuGrid.Opacity = 1
        mainWindow.PagesFrame.Opacity = 1
        mainWindow.PagesFrame.IsEnabled = True
        comcellFrame.IsEnabled = True
        comcellFrame.Opacity = 1
        mainWindow.SubMenuFrame.Opacity = 1
        mainWindow.SubMenuFrame.IsEnabled = True
        addframe.Visibility = Visibility.Hidden
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
        Dim day As Integer

        Select Case days
            Case "Monday"
                day = 1
            Case "Tuesday"
                day = 2
            Case "Wednesday"
                day = 3
            Case "Thursday"
                day = 4
            Case "Friday"
                day = 5
            Case "Saturday"
                day = 6
            Case "Sunday"
                day = 7
        End Select

        Return day
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
    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        ExitPage()
    End Sub

    Private Sub UpdateBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            SetData(comcellClockVM.objectComcellClockSet)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
