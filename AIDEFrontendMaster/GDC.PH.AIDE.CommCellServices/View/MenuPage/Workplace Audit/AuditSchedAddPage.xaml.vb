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
    Dim lstFiscalYear As FiscalYear()
    Dim commendationVM As New CommendationViewModel()
    Dim fiscalyearVM As New SelectionListViewModel
    Dim month As Integer
    Dim year As Integer
    Dim lstAuditSchedMonth As WorkplaceAudit()
    Private lstNicknameList As New ObservableCollection(Of NicknameModel)
    Dim lstauditSched As AuditSched()
    Dim auditSchedVM As New AuditSchedViewModel()
#End Region

#Region "Constructors"
    'Add Constructor
    Public Sub New(_profile As Profile, mainframe As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame, _lstauditSched As AuditSched())
        Try
            Me._frame = mainframe
            Me._addframe = addframe
            Me._menugrid = menugrid
            Me._submenuframe = submenuframe
            Me.profile = _profile
            Me.lstauditSched = _lstauditSched
            InitializeComponent()
            LoadSChed()
            LoadMonth()
            LoadEmpNickName()
            clearFields()
            LoadauditSched()
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

            LoadSChed()
            LoadMonth()
            LoadControls()
            LoadEmpNickName()
            'clearFields()
            cbYear.IsEnabled = False
            cbMonth.IsEnabled = False
            cbPeriodStart.IsEnabled = False

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
    Public Sub LoadauditSched()
        Try
            Dim lstauditSchedList As New ObservableCollection(Of AuditSchedModel)
            Dim auditSchedDBProvider As New AuditSchedDBProvider
            'Dim objauditSched As New AuditSched


            For Each obj In lstauditSched.ToList

                auditSchedDBProvider.SetMyAuditSched(obj)
            Next

            For Each rawUser As MyAuditSched In auditSchedDBProvider.GetMyAuditSched()
                lstauditSchedList.Add(New AuditSchedModel(rawUser))
            Next

            auditSchedVM.AuditSchedList = lstauditSchedList

            Me.DataContext = auditSchedVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub


    Public Sub LoadControls()
        Dim getmonth As String = ""
        getmonth = txtBlockMonth.Text
        Try


            txtHeader.Text = "Update Workplace Auditor"
            txtBlockButton.Text = "Update"
            txtBlockMonth.Text = GetSelectedMonth(AuditSchedModel.PERIOD_START.Month)

            txtBlockPeriodStart.Text = AuditSchedModel.PERIOD_START
            txtBlockPeriodEnd.Text = AuditSchedModel.PERIOD_END
            txtBlockYear.Text = AuditSchedModel.PERIOD_START.Year


            txtBlockDaily.Text = AuditSchedModel.DAILY
            txtBlockWeekly.Text = AuditSchedModel.WEEKLY
            txtBlockMonthly.Text = AuditSchedModel.MONTHLY

            'cbMonth.SelectedValue = AuditSchedModel.PERIOD_START.Month
            'cbPeriodStart.SelectedValue = txtBlockPeriodStart.Text
            'txtPeriodEnd.Text = txtBlockPeriodEnd.Text
            'cbDaily.SelectedValue = txtBlockDaily.Text
            'cbWeekly.SelectedValue = txtBlockWeekly.Text
            ''cbMonthly.SelectedValue = txtBlockMonth.Text
            'cbYear.SelectedValue = txtBlockYear.Text
        Catch ex As Exception
            ex.Message.ToString()
        End Try
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
    Public Function GetSelectedMonth(month As String)
        Select Case month
            Case 1
                month = "January"
            Case 2
                month = "February"
            Case 3
                month = "March"
            Case 4
                month = "April"
            Case 5
                month = "May"
            Case 6
                month = "June"
            Case 7
                month = "July"
            Case 8
                month = "August"
            Case 9
                month = "September"
            Case 10
                month = "October"
            Case 11
                month = "November"
            Case 12
                month = "December"
        End Select


        Return month
    End Function

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

    Public Sub LoadYear()
        Try
            If InitializeService() Then
                lstFiscalYear = aide.GetAllFiscalYear()
                LoadFiscalYear()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Public Sub LoadFiscalYear()
        Try
            Dim lstFiscalYearList As New ObservableCollection(Of FiscalYearModel)
            Dim FYDBProvider As New SelectionListDBProvider

            For Each objFiscal As FiscalYear In lstFiscalYear
                FYDBProvider._setlistofFiscal(objFiscal)
            Next

            For Each rawUser As myFiscalYearSet In FYDBProvider._getobjFiscal()
                lstFiscalYearList.Add(New FiscalYearModel(rawUser))
            Next

            fiscalyearVM.ObjectFiscalYearSet = lstFiscalYearList
            cbYear.ItemsSource = fiscalyearVM.ObjectFiscalYearSet
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub
    Public Sub LoadSChed()
        Try
            If InitializeService() Then
                lstAuditSchedMonth = aide.GetAuditSChed_Month(2, Date.Now.Year, Date.Now.Month)
                LoadYear()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadEmpNickName()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = aide.ViewNicknameByDeptID(profile.Email_Address, dsplyByDept)

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
        cbPeriodStart.ITEMS.Clear()
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
        If cbPeriodStart.SelectedValue Is Nothing Then
            Return
        End If
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
                If cbMonth.SelectedValue = Nothing Or cbDaily.SelectedValue = Nothing Or cbMonthly.SelectedValue = Nothing Or cbWeekly.SelectedValue = Nothing Or cbPeriodStart.SelectedValue = Nothing Or cbYear.SelectedValue.ToString.Substring(0, 4) = Nothing Then
                    MsgBox("Please Fill Up All Fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                ElseIf cbDaily.SelectedValue = cbWeekly.SelectedValue Or cbDaily.SelectedValue = cbMonthly.SelectedValue Or cbWeekly.SelectedValue = cbMonthly.SelectedValue Then
                    MsgBox("Selected Auditors are the same", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                Else


                    auditSched.EMP_ID = profile.Emp_ID
                    auditSched.PERIOD_START = cbPeriodStart.Text
                    auditSched.PERIOD_END = txtPeriodEnd.Text
                    auditSched.DAILY = cbDaily.SelectedValue
                    auditSched.WEEKLY = cbWeekly.SelectedValue
                    auditSched.MONTHLY = cbMonthly.SelectedValue
                    auditSched.YEAR = cbYear.SelectedValue.ToString.Substring(0, 4)

                    Dim isMessageSuccessfuly As Boolean = aide.InsertAuditSched(auditSched)
                    If isMessageSuccessfuly Then
                        MsgBox("Auditor has been added.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                    Else
                        MsgBox("An application error was encountered. Please contact your AIDE Administrator. ", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                    End If

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
                auditSched.AUDIT_SCHED_ID = auditSchedID
                For Each obj In lstNicknameList.ToList
                    If obj.First_Name = txtBlockDaily.Text.ToString.Trim() Then
                        If cbDaily.SelectedValue Is Nothing Then
                            cbDaily.SelectedValue = obj.EMP_ID
                            auditSched.DAILY = cbDaily.SelectedValue
                        Else
                            auditSched.DAILY = cbDaily.SelectedValue
                        End If
                    End If
                    If obj.First_Name = txtBlockWeekly.Text.ToString.Trim() Then
                        If cbWeekly.SelectedValue Is Nothing Then
                            cbWeekly.SelectedValue = obj.EMP_ID
                            auditSched.WEEKLY = cbWeekly.SelectedValue
                        Else

                            auditSched.WEEKLY = cbWeekly.SelectedValue
                        End If
                    End If
                    If obj.First_Name = txtBlockMonthly.Text.ToString.Trim() Then
                        If cbMonthly.SelectedValue Is Nothing Then
                            cbMonthly.SelectedValue = obj.EMP_ID
                            auditSched.MONTHLY = cbMonthly.SelectedValue
                        Else
                            auditSched.MONTHLY = cbMonthly.SelectedValue
                        End If
                    End If
                Next




                'auditSched.YEAR = cbYear.SelectedValue.ToString.Substring(0, 4)


                Dim isMessageSuccessfuly As Boolean = aide.UpdateAuditSched(auditSched)
                If isMessageSuccessfuly Then
                    MsgBox("Schedule has been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                Else
                    MsgBox("An application error was encountered. Please contact your AIDE Administrator. ", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                End If

                _frame.Navigate(New AuditSchedMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
                    _frame.IsEnabled = True
                    _frame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                End If
            'End If

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

    End Sub

    Private Sub cbPeriodStart_DropDownClosed(sender As Object, e As EventArgs) Handles cbPeriodStart.DropDownClosed



    End Sub

    Private Sub cbMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbMonth.SelectionChanged
        Dim getYr As Integer
        getYr = cbYear.SelectedValue.ToString.Substring(0, 4)
        cbPeriodStart.IsEnabled = True

        If cbMonth.SelectedValue = 1 Then
            getYr = getYr + 1
        ElseIf cbMonth.SelectedValue = 2 Then
            getYr = getYr + 1
        ElseIf cbMonth.SelectedValue = 3 Then
            getYr = getYr + 1
        End If
        GetAllMondayOfWeekPerMonth(cbMonth.SelectedValue, getYr, DayOfWeek.Monday)
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

    Private Sub cbDaily_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbDaily.SelectionChanged
        Dim xe As String
        xe = cbDaily.SelectedValue
    End Sub

    Private Sub cbMonthly_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbMonthly.SelectionChanged
        Dim xe As String
        xe = cbMonthly.SelectedValue
    End Sub

    Private Sub cbWeekly_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbWeekly.SelectionChanged
        Dim xe As String
        xe = cbWeekly.SelectedValue
    End Sub

    Private Sub cbPeriodStart_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbPeriodStart.SelectionChanged
        Dim ifDuplicate As Boolean
        If cbPeriodStart.SelectedValue Is Nothing Then
            Return
        Else
            If Not cbPeriodStart.SelectedValue Is Nothing Then
                For Each obj In lstauditSched.ToList
                    If obj.PERIOD_START = cbPeriodStart.SelectedValue Then
                        MsgBox(" Schedule already assigned. Please select different schedule.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                        ifDuplicate = True
                        cbPeriodStart.SelectedItem = Nothing
                        Return
                    End If
                Next

                For Each obj In lstauditSched.ToList
                    If Date.Parse(obj.PERIOD_START).Year = Date.Parse(cbPeriodStart.SelectedValue).Year Then
                        If Date.Parse(obj.PERIOD_START).Month = Date.Parse(cbPeriodStart.SelectedValue).Month Then
                            For Each nickname In lstNicknameList.ToList
                                If nickname.First_Name = obj.MONTHLY Then
                                    cbMonthly.SelectedValue = nickname.EMP_ID
                                    cbMonthly.IsEnabled = False
                                End If
                            Next

                        End If
                    End If

                Next
            End If
            If ifDuplicate = False Then
                GetAllFridayOfWeek()
            Else

            End If
        End If
    End Sub
#End Region


End Class
