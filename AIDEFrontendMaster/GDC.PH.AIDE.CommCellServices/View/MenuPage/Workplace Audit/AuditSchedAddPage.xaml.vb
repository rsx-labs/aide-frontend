Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Class AuditSchedAddPage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback


#Region "Page Declaration"
    Public _frame As Frame
    Private aide As AideServiceClient
    Private auditSched As New AuditSched
    Private AuditSchedModel As New AuditSchedModel
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile
    Private nextYear As Integer
    Private mode As String
    Private auditSchedID As Integer
    Private dsplyByDept = 2
#End Region

#Region "Constructors"
    'Add Constructor
    Public Sub New(_profile As Profile, mainframe As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame)
        Try
            Me._frame = mainframe
            Me._addframe = addframe
            Me._menugrid = menugrid
            Me._submenuframe = submenuframe
            Me.profile = _profile
            InitializeComponent()
            LoadMonth()
            LoadYears()
            LoadEmpNickName()
            clearFields()
            mode = "Add"
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub
    'Update Constructor
    Public Sub New(_profile As Profile, mainframe As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame, _auditSched As AuditSchedModel)
        Try
            Me._frame = mainframe
            Me._addframe = addframe
            Me._menugrid = menugrid
            Me._submenuframe = submenuframe
            Me.profile = _profile
            Me.AuditSchedModel = _auditSched
            Me.auditSchedID = AuditSchedModel.AUDIT_SCHED_ID
            InitializeComponent()
            LoadControls()
            LoadMonth()
            LoadYears()
            LoadEmpNickName()
            clearFields()
            mode = "Update"
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub
#End Region

#Region "Methods/Functions"
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

    Public Sub LoadControls()
        txtHeader.Text = "Update Workplace Auditor"
        txtBlockButton.Text = "Update"
        txtBlockMonth.Text = AuditSchedModel.PERIOD_START.Month
        txtBlockDaily.Text = AuditSchedModel.DAILY
        txtBlockWeekly.Text = AuditSchedModel.WEEKLY
        txtBlockMonthly.Text = AuditSchedModel.MONTHLY
        txtBlockPeriodStart.Text = AuditSchedModel.PERIOD_START
        txtBlockPeriodEnd.Text = AuditSchedModel.PERIOD_END
        txtBlockYear.Text = AuditSchedModel.PERIOD_START.Year

        cbMonth.SelectedValue = txtBlockMonth.Text
        cbPeriodStart.SelectedValue = txtBlockPeriodStart.Text
        txtPeriodEnd.Text = txtBlockPeriodEnd.Text
        cbDaily.SelectedValue = txtBlockDaily.Text
        cbWeekly.SelectedValue = txtBlockWeekly.Text
        cbMonthly.SelectedValue = txtBlockMonthly.Text
        cbYear.SelectedValue = txtBlockYear.Text
    End Sub

    Public Sub LoadMonth()
        cbMonth.DisplayMemberPath = "Text"
        cbMonth.SelectedValuePath = "Value"
        cbMonth.Items.Add(New With {.Text = "January", .Value = 1})
        cbMonth.Items.Add(New With {.Text = "February", .Value = 2})
        cbMonth.Items.Add(New With {.Text = "March", .Value = 3})
        cbMonth.Items.Add(New With {.Text = "April", .Value = 4})
        cbMonth.Items.Add(New With {.Text = "May", .Value = 5})
        cbMonth.Items.Add(New With {.Text = "June", .Value = 6})
        cbMonth.Items.Add(New With {.Text = "July", .Value = 7})
        cbMonth.Items.Add(New With {.Text = "August", .Value = 8})
        cbMonth.Items.Add(New With {.Text = "September", .Value = 9})
        cbMonth.Items.Add(New With {.Text = "October", .Value = 10})
        cbMonth.Items.Add(New With {.Text = "November", .Value = 11})
        cbMonth.Items.Add(New With {.Text = "December", .Value = 12})
    End Sub

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                nextYear = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadEmpNickName()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = aide.ViewNicknameByDeptID(profile.Email_Address, dsplyByDept)
                Dim lstNicknameList As New ObservableCollection(Of NicknameModel)
                Dim successRegisterDBProvider As New SuccessRegisterDBProvider
                Dim nicknameVM As New NicknameViewModel()

                For Each objLessonLearnt As Nickname In lstNickname
                    successRegisterDBProvider.SetMyNickname(objLessonLearnt)
                Next

                For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
                    lstNicknameList.Add(New NicknameModel(rawUser))
                Next

                nicknameVM.NicknameList = lstNicknameList

                cbDaily.DataContext = nicknameVM
                cbMonthly.DataContext = nicknameVM
                cbWeekly.DataContext = nicknameVM
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub GetAllMondayOfWeekPerMonth(ByVal month As Integer, ByVal year As Integer, ByVal dayOfWeek As DayOfWeek)
        Dim dates = New DateTime(year, month, 1)

        If dates.DayOfWeek <> dayOfWeek Then
            Dim daysUntilDayOfWeek As Integer = (CInt(dayOfWeek) - CInt(dates.DayOfWeek) + 7) Mod 7
            dates = dates.AddDays(daysUntilDayOfWeek)
        End If

        Dim days As List(Of DateTime) = New List(Of DateTime)()

        While dates.Month = month
            cbPeriodStart.Items.Add(dates)
            dates = dates.AddDays(7)
        End While
    End Sub

    Public Sub GetAllFridayOfWeek()
        Dim selectedDate As DateTime = DateTime.Parse(cbPeriodStart.SelectedValue)
        Dim num_days As Integer = System.DayOfWeek.Friday - selectedDate.DayOfWeek
        If num_days < 0 Then num_days += 7
        Dim friday As DateTime = selectedDate.AddDays(num_days)
        txtPeriodEnd.Text = friday.ToShortDateString
    End Sub

    Public Sub clearFields()
        cbYear.IsEnabled = True
        cbYear.Text = String.Empty
        cbPeriodStart.Text = String.Empty
        cbPeriodStart.Items.Clear()
        txtPeriodEnd.Text = String.Empty
    End Sub
#End Region

#Region "Events"
    Private Sub AddBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()
            If mode = "Add" Then
                If cbMonth.Text = Nothing Or cbDaily.Text = Nothing Or cbMonthly.Text = Nothing Or cbWeekly.Text = Nothing Or cbPeriodStart.Text = Nothing Or cbYear.Text = Nothing Then
                    MsgBox("Please Fill Up All Fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                ElseIf cbDaily.Text = cbWeekly.Text Or cbDaily.Text = cbMonthly.Text Or cbWeekly.Text = cbMonthly.Text Then
                    MsgBox("Selected Auditors are the same", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                Else
                    MsgBox("Successfully Added!", vbOKOnly + MsgBoxStyle.Information, "AIDE")

                    auditSched.EMP_ID = profile.Emp_ID
                    auditSched.PERIOD_START = cbPeriodStart.Text
                    auditSched.PERIOD_END = txtPeriodEnd.Text
                    auditSched.DAILY = cbDaily.Text
                    auditSched.WEEKLY = cbWeekly.Text
                    auditSched.MONTHLY = cbMonthly.Text
                    auditSched.YEAR = cbYear.SelectedValue

                    aide.InsertAuditSched(auditSched)


                    _frame.Navigate(New AuditSchedMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
                    _frame.IsEnabled = True
                    _frame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                End If
            Else
                If cbMonth.Text = Nothing Or cbDaily.SelectedValue = Nothing Or cbMonthly.SelectedValue = Nothing Or cbWeekly.SelectedValue = Nothing Or cbPeriodStart.SelectedValue = Nothing Or txtBlockYear.Text = Nothing Then
                    MsgBox("Please Fill Up All Fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                ElseIf txtBlockDaily.Text = txtBlockWeekly.Text Or txtBlockDaily.Text = txtBlockMonthly.Text Or txtBlockWeekly.Text = txtBlockMonthly.Text Then
                    MsgBox("Selected Auditors are the same", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                Else
                    MsgBox("Successfully Updated!", vbOKOnly + MsgBoxStyle.Information, "AIDE")

                    auditSched.AUDIT_SCHED_ID = auditSchedID
                    auditSched.PERIOD_START = cbPeriodStart.SelectedValue
                    auditSched.PERIOD_END = txtBlockPeriodEnd.Text
                    auditSched.DAILY = cbDaily.SelectedValue
                    auditSched.WEEKLY = cbWeekly.SelectedValue
                    auditSched.MONTHLY = cbMonthly.SelectedValue
                    auditSched.YEAR = txtBlockYear.Text

                    aide.UpdateAuditSched(auditSched)


                    _frame.Navigate(New AuditSchedMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
                    _frame.IsEnabled = True
                    _frame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                End If
            End If

        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "AIDE") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        _frame.Navigate(New AuditSchedMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
        _frame.IsEnabled = True
        _frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub cbPeriodStart_DropDownOpened(sender As Object, e As EventArgs) Handles cbPeriodStart.DropDownOpened
        GetAllMondayOfWeekPerMonth(cbMonth.SelectedValue, cbYear.SelectedValue, DayOfWeek.Monday)
    End Sub

    Private Sub cbPeriodStart_DropDownClosed(sender As Object, e As EventArgs) Handles cbPeriodStart.DropDownClosed
        GetAllFridayOfWeek()
    End Sub

    Private Sub cbMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbMonth.SelectionChanged
        clearFields()
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        cbPeriodStart.IsEnabled = True
    End Sub
#End Region

#Region "INotify Methods"
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
