Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports NLog
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class DailyAuditPage
    Implements ServiceReference1.IAideServiceCallback
    Private dailyVMM As New dayVM
    Private email As String
    Private pageframe As Frame
    Private profile As Profile
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    'Private _AideService As ServiceReference1.AideServiceClient
    Dim lstAuditSchedMonth As WorkplaceAudit()
    Dim AuditSchedMonthVM As New SelectionListViewModel
    Dim workPlaceAuditVM As New WorkplaceAuditViewModel
    Dim lstEmployee As WorkplaceAudit()
    Dim lstAuditQuestions As WorkplaceAudit()
    Private currDailyAuditAssigned As Integer
    Private defaultDisplay As String
    Private defaultFy_Week As Integer
    Private LstAuditDailySchedByWeek As New ObservableCollection(Of WorkplaceAuditModel)
    Dim month As Integer
    Dim year As Integer
    Dim lstFiscalYear As FiscalYear()

    Dim fiscalyearVM As New SelectionListViewModel
    Dim isthereAnyRecord As Boolean = True
    Dim isInitialize As Boolean
    Dim isMessageAlreadyPopupInCbyear As Boolean = False

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Public Sub New(_pageframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid,
                   _submenuframe As Frame)

        _logger.Debug("Start : Constructor")

        Me.pageframe = _pageframe
        Me.profile = _profile
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe

        '_AideService = aideService

        ' This call is required by the designer.
        InitializeComponent()
        'InitializeService()
        ' Add any initialization after the InitializeComponent() call.
        isInitialize = True

        LoadYear()
        SetFiscalYear()
        LoadSChed()
        'isSetDefault = True
        LoadMonthLst()

        isInitialize = False

        _logger.Debug("End : Constructor")

    End Sub

    Private Sub GenerateQuestions()

        _logger.Debug("Start : GenerateQuestions")

        Dim questModel As New QuestionsDayModel
        Dim imgdtcheck As String

        Try
            'If InitializeService() Then
            lstAuditQuestions = AideClient.GetClient().GetAuditQuestions(profile.Emp_ID, "1")
            'End If

            Dim FYDBProvider As New WorkplaceAuditDBProvider
            LstAuditDailySchedByWeek.Clear()
            FYDBProvider.GetMyWorkplaceAudit.Clear()
            For Each objFiscal As WorkplaceAudit In lstAuditQuestions
                FYDBProvider.SetMyWorkplaceAudit(objFiscal)
            Next

            For Each rawUser As MyWorkplaceAudit In FYDBProvider.GetMyWorkplaceAudit()
                LstAuditDailySchedByWeek.Add(New WorkplaceAuditModel(rawUser))
            Next
            workPlaceAuditVM.WorkPlaceAuditLst = LstAuditDailySchedByWeek

            For Each quest As WorkplaceAuditModel In LstAuditDailySchedByWeek.ToList
                '0 - Not yet Check the Question
                '1 - Checked Already
                '2 - checked but not completed/success
                If defaultDisplay.ToString.Trim() = quest.WEEKDATE.ToString.Trim() AndAlso defaultFy_Week = quest.FY_WEEK Then
                    If quest.DT_CHECK_FLG = 0 Then
                        imgdtcheck = "..\..\..\Assets\Button\audittocheck.png"
                    ElseIf quest.DT_CHECK_FLG = 1 Then
                        imgdtcheck = "..\..\..\Assets\Button\Checked.png"
                    Else
                        imgdtcheck = "..\..\..\Assets\Button\wrong.png"
                    End If
                    dailyVMM.QuestionDayList.Add(New QuestionsDayModel(quest.AUDIT_QUESTIONS, quest.OWNER, quest.DT_CHECK_FLG, imgdtcheck, quest.WEEKDATE))
                End If
            Next

            QuarterLVQuestions.ItemsSource = dailyVMM.QuestionDayList
            DataContext = dailyVMM.QuestionDayList
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : GenerateQuestions")

    End Sub

    Public Shared Function GetWeekNumber(ByVal datum As Date) As Integer
        Return (datum.Day - 1) \ 7 + 1
    End Function

    Public Sub LoadMonthLst()

        _logger.Debug("Start : LoadMonthList")

        Try
            'If InitializeService() Then
            lstAuditSchedMonth = AideClient.GetClient().GetAuditSChed_Month(2, Date.Now.Year, Date.Now.Month)
            GetMonthLst()
            'End If
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : LoadMonthList")

    End Sub

    Public Sub GetMonthLst()

        _logger.Debug("Start : GetMonthList")

        Try
            Dim lstAuditSchedMonthList As New ObservableCollection(Of WorkplaceAuditModel)
            Dim FYDBProvider As New SelectionListDBProvider

            For Each objFiscal As WorkplaceAudit In lstAuditSchedMonth
                FYDBProvider._setListOfAuditSchedMonth(objFiscal)
            Next

            For Each rawUser As myAuditSchedMonthSet In FYDBProvider._getAuditSchedMonth()
                lstAuditSchedMonthList.Add(New WorkplaceAuditModel(rawUser))
                If defaultFy_Week = 0 Then
                    defaultFy_Week = rawUser._fy_week
                End If
            Next
            AuditSchedMonthVM.ObjectAuditSchedMonthSet = lstAuditSchedMonthList
            cbMonth.ItemsSource = AuditSchedMonthVM.ObjectAuditSchedMonthSet

        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : GetMonthList")

    End Sub

    Private Sub InitEmpAuditDailybyWeekData()

        _logger.Debug("End : InitEmpAuditData")

        Try
            Dim statusAudit As String = ""
            Dim LstAuditDailySchedByWeek As New ObservableCollection(Of WorkplaceAuditModel)
            Dim FYDBProvider As New WorkplaceAuditDBProvider
            Dim dateNow As String = ""

            LstAuditDailySchedByWeek.Clear()
            FYDBProvider.GetMyWorkplaceAudit.Clear()
            If Not cbWeek.SelectedItem.AUDITSCHED_MONTH Is Nothing Then
                Dim stringMonth As String = cbWeek.SelectedItem.AUDITSCHED_MONTH
                Dim DateString As String = stringMonth.Substring(0, 13)

                defaultDisplay = CDate(DateString).Date.Year & "-" & CDate(DateString).Date.Month.ToString("00") & "-" & CDate(DateString).Date.Day.ToString("00")
                cbMonth.SelectedValue = CDate(defaultDisplay).Month
                dateNow = Date.Now.Year & "-" & Date.Now.Month.ToString("00") & "-" & Date.Now.Day.ToString("00")
            End If
            If Date.Parse(defaultDisplay).Year = Date.Parse(dateNow).Year Then
                If Date.Parse(defaultDisplay).Month = Date.Parse(dateNow).Month Then
                    If GetWeekNumber(Date.Parse(defaultDisplay)) = GetWeekNumber(Date.Parse(dateNow)) Then
                        defaultDisplay = CDate(DateString).Date.Year & "-" & CDate(DateString).Date.Month.ToString("00") & "-" & CDate(DateString).Date.Day.ToString("00")
                    End If
                End If
            End If

            Dim dateParam As Date = CDate(defaultDisplay)

            'If InitializeService() Then
            lstEmployee = AideClient.GetClient().GetDailyAuditorByWeek(profile.Emp_ID, cbWeek.SelectedValue, dateParam)
            'End If


            For Each objFiscal As WorkplaceAudit In lstEmployee
                FYDBProvider.SetMyWorkplaceAudit(objFiscal)
            Next

            For Each rawUser As MyWorkplaceAudit In FYDBProvider.GetMyWorkplaceAudit()
                LstAuditDailySchedByWeek.Add(New WorkplaceAuditModel(rawUser))
                currDailyAuditAssigned = rawUser.EMP_ID
                defaultFy_Week = rawUser.FY_WEEK

            Next
            workPlaceAuditVM.WorkPlaceAuditLst = LstAuditDailySchedByWeek
            For Each quest As WorkplaceAuditModel In LstAuditDailySchedByWeek.ToList
                Select Case quest.DT_CHECK_FLG
                    Case 0
                        statusAudit = "Not Completed"
                    Case 1
                        statusAudit = "Completed"
                    Case 2
                        statusAudit = "Completed"
                End Select
                dailyVMM.Days.Add(New DayMod(quest.WEEKDAYS, quest.WEEKDATE, quest.NICKNAME, statusAudit, quest.WEEKDATE, quest.DATE_CHECKED))
            Next

            QuarterLV.ItemsSource = dailyVMM.Days
            DataContext = dailyVMM.Days

        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : InitEmpAuditData")

    End Sub

    'Public Function InitializeService() As Boolean
    '    _logger.Debug("Start : InitializeService")

    '    Dim bInitialize As Boolean = False
    '    Try

    '        If _AideService.State = CommunicationState.Faulted Then

    '            _logger.Debug("Service is faulted, reinitializing ...")

    '            Dim Context As InstanceContext = New InstanceContext(Me)
    '            _AideService = New AideServiceClient(Context)
    '            _AideService.Open()
    '        End If

    '        bInitialize = True
    '    Catch ex As SystemException
    '        _logger.Error(ex.ToString())

    '        _AideService.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try

    '    _logger.Debug("End : InitializeService")

    '    Return bInitialize
    'End Function

    Public Sub LoadSChed()
        Try
            'If InitializeService() Then
            lstAuditSchedMonth = AideClient.GetClient().GetAuditSChed_Month(1, year, month)
            If lstAuditSchedMonth.Count <> 0 Then
                LoadPerWeekSchedule()
            Else
                If isthereAnyRecord = True And isInitialize = False Then
                    MsgBox("There is no records in selected date.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                End If

                isthereAnyRecord = False
                dailyVMM.QuestionDayList.Clear()
                dailyVMM.Days.Clear()
                cbWeek.ItemsSource = Nothing
                Return
            End If
            'End If
        Catch ex As Exception
            _logger.Error($"Error at LoadSched. {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadPerWeekSchedule()
        Try
            Dim lstAuditSchedMonthList As New ObservableCollection(Of WorkplaceAuditModel)
            Dim FYDBProvider As New SelectionListDBProvider
            Dim stringMonth As String = ""
            Dim DateString As String
            Dim DateNow As String
            Dim notequalToDateNow As String = ""

            For Each objFiscal As WorkplaceAudit In lstAuditSchedMonth
                FYDBProvider._setListOfAuditSchedMonth(objFiscal)
            Next

            For Each rawUser As myAuditSchedMonthSet In FYDBProvider._getAuditSchedMonth()
                lstAuditSchedMonthList.Add(New WorkplaceAuditModel(rawUser))
                stringMonth = rawUser._auditSchedMonth
                DateString = stringMonth.Substring(0, 13)
                stringMonth = CDate(DateString).Date.Year & "-" & CDate(DateString).Date.Month.ToString("00") & "-" & CDate(DateString).Date.Day.ToString("00")
                DateNow = Date.Now.Year & "-" & Date.Now.Month.ToString("00") & "-" & Date.Now.Day.ToString("00")
                defaultFy_Week = 0
                If notequalToDateNow = String.Empty Then
                    notequalToDateNow = rawUser._fy_week
                End If

                If CDate(stringMonth).Year = CDate(DateNow).Year Then
                    If GetWeekNumber(CDate(stringMonth)) = GetWeekNumber(CDate(DateNow)) Then
                        defaultFy_Week = rawUser._fy_week
                    End If
                End If
            Next
            If defaultFy_Week = 0 Then
                defaultFy_Week = CInt(notequalToDateNow)
            End If
            AuditSchedMonthVM.ObjectAuditSchedMonthSet = lstAuditSchedMonthList
            cbWeek.ItemsSource = AuditSchedMonthVM.ObjectAuditSchedMonthSet

        Catch ex As Exception
            _logger.Error($"Error at LoadPerWeekSchedule. {ex.ToString()}")
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ListViewItem_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Dim item = (TryCast(sender, FrameworkElement)).DataContext

        Dim FYDBProvider As New WorkplaceAuditDBProvider

        For Each objFiscal As WorkplaceAudit In lstAuditQuestions
            If objFiscal.WEEKDATE.ToString.Trim() = item.WEEKDATE.ToString.Trim() Then
                FYDBProvider.SetMyWorkplaceAudit(objFiscal)
            End If

        Next

        LstAuditDailySchedByWeek.Clear()
        For Each rawUser As MyWorkplaceAudit In FYDBProvider.GetMyWorkplaceAudit()
            LstAuditDailySchedByWeek.Add(New WorkplaceAuditModel(rawUser))
        Next

        If profile.Emp_ID = currDailyAuditAssigned OrElse profile.Permission_ID = 1 Then
            addframe.Navigate(New DailyAuditCheck(pageframe, profile, addframe, menugrid, submenuframe, LstAuditDailySchedByWeek, 1))
            pageframe.IsEnabled = False
            pageframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Visibility = Visibility.Visible
            addframe.Margin = New Thickness(120, 60, 120, 60)
        End If
    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate
        Throw New NotImplementedException()
    End Sub

    Private Sub SetSelectedDay(sender As Object, e As SelectionChangedEventArgs)
        Dim item As Object
        Dim questModel As New QuestionsDayModel
        Dim imgdtcheck As String
        If Not QuarterLV.SelectedItem Is Nothing Then
            item = QuarterLV.SelectedItem.WEEKDATE
        Else

            Return
        End If

        dailyVMM.QuestionDayList.Clear()

        Try
            For Each quest As WorkplaceAuditModel In LstAuditDailySchedByWeek.ToList
                '0 - Not yet Check the Question
                '1 - Checked Already
                '2 - checked but not completed/success

                If item.ToString.Trim() = (Convert.ToDateTime(quest.WEEKDATE).Month.ToString.Trim() & "/" & Convert.ToDateTime(quest.WEEKDATE).Day.ToString.Trim()) AndAlso year = CDate(quest.WEEKDATE).Year Then
                    If quest.DT_CHECK_FLG = 0 Then
                        imgdtcheck = "..\..\..\Assets\Button\audittocheck.png"
                    ElseIf quest.DT_CHECK_FLG = 1 Then
                        imgdtcheck = "..\..\..\Assets\Button\Checked.png"
                    Else
                        imgdtcheck = "..\..\..\Assets\Button\wrong.png"
                    End If

                    dailyVMM.QuestionDayList.Add(New QuestionsDayModel(quest.AUDIT_QUESTIONS, quest.OWNER, quest.DT_CHECK_FLG, imgdtcheck, quest.WEEKDATE))
                End If

            Next

            QuarterLVQuestions.ItemsSource = dailyVMM.QuestionDayList
            DataContext = dailyVMM.QuestionDayList
        Catch ex As Exception
            _logger.Error($"Error at SetSelectedDay. {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub cbWeek_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbWeek.SelectionChanged
        Dim selectedFy_Week As Integer
        selectedFy_Week = defaultFy_Week
        If cbWeek.SelectedValue Is Nothing Then
            cbWeek.SelectedValue = selectedFy_Week
        End If

        dailyVMM.QuestionDayList.Clear()
        dailyVMM.Days.Clear()
        If lstAuditSchedMonth.Count <> 0 Then
            InitEmpAuditDailybyWeekData()
            If dailyVMM.Days.Count <> 0 Then
                GenerateQuestions()
            End If
        End If
    End Sub

    Public Sub LoadYear()
        Try
            'If InitializeService() Then
            lstFiscalYear = AideClient.GetClient().GetAllFiscalYear()
            LoadFiscalYear()
            'End If
        Catch ex As Exception
            _logger.Error($"Error at LoadYear. {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
            _logger.Error($"Error at LoadFiscalYear. {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetFiscalYear()
        Try
            Month = Date.Now.Month

            If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
                cbYear.SelectedValue = (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
            Else
                cbYear.SelectedValue = (Date.Now.Year).ToString() + "-" + (Date.Now.Year + 1).ToString()
            End If

            Year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
            If month = 1 Then
                year += 1
            ElseIf month = 2 Then
                year += 1
            ElseIf month = 3 Then
                year += 1
            End If

        Catch ex As Exception
            _logger.Error($"Error at SetFiscalYear. {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged

        If cbMonth.SelectedValue Is Nothing Then
            cbMonth.SelectedValue = Date.Now.Month
        End If
        year = cbYear.SelectedItem.FISCAL_year.ToString.Substring(0, 4)
        If month = 1 Then
            year += 1
        ElseIf month = 2 Then
            year += 1
        ElseIf month = 3 Then
            year += 1
        End If

        isthereAnyRecord = True
        isMessageAlreadyPopupInCbyear = True
        LoadSChed()
    End Sub

    Private Sub cbMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbMonth.SelectionChanged
        If cbMonth.SelectedValue Is Nothing Then
            cbMonth.SelectedValue = Date.Now.Month
        End If
        month = cbMonth.SelectedValue
        year = cbYear.SelectedItem.FISCAL_year.ToString.Substring(0, 4)
        If month = 1 Then
            year += 1
        ElseIf month = 2 Then
            year += 1
        ElseIf month = 3 Then
            year += 1
        End If
        If isMessageAlreadyPopupInCbyear = True Then
            isMessageAlreadyPopupInCbyear = False
        Else
            isthereAnyRecord = True
        End If
        If e.RemovedItems.Count <> 0 Then
            isthereAnyRecord = True
        End If

        LoadSChed()

        If cbWeek.SelectedValue Is Nothing Then
            cbWeek.SelectedValue = defaultFy_Week
        End If
    End Sub
End Class

Public Class dayVM
    Private _dailyobjlst As ObservableCollection(Of DayMod)
    Private QtnLst As ObservableCollection(Of QuestionsDayModel)

    Public Sub New()
        _dailyobjlst = New ObservableCollection(Of DayMod)
        QtnLst = New ObservableCollection(Of QuestionsDayModel)
    End Sub

    Public Property Days As ObservableCollection(Of DayMod)
        Get
            Return _dailyobjlst
        End Get
        Set(value As ObservableCollection(Of DayMod))
            _dailyobjlst = value
        End Set
    End Property
    Public Property QuestionDayList As ObservableCollection(Of QuestionsDayModel)
        Get
            Return QtnLst
        End Get
        Set(value As ObservableCollection(Of QuestionsDayModel))
            QtnLst = value
        End Set
    End Property
End Class

Public Class DayMod
    Private _days As String
    Private _dates As String
    Private _empName As String
    Private _notes As String
    Private _weekdate As String
    Private _date_checked As String
    Private _weekdateSched As String
    Private _fyweek As Integer

    Public Sub New()

    End Sub

    Public Sub New(dayss As String, datess As String, emp As String, note As String)
        _days = dayss
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If
    End Sub

    Public Sub New(dayss As String, datess As String, emp As String, note As String, weekdate As String, date_checked As String)
        _days = dayss
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If
        _weekdate = weekdate
        If date_checked.ToString.Trim.ToLower = "NULL".ToString.Trim.ToLower() Then
            _date_checked = "Not yet check"
        Else
            _date_checked = date_checked
        End If
    End Sub

    Public Sub New(dayss As String, datess As String, emp As String, note As String, weekdate As String, date_checked As String, weekdateSched As String)
        _days = dayss
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If
        _weekdate = weekdate
        If date_checked.ToString.Trim.ToLower = "NULL".ToString.Trim.ToLower() Then
            _date_checked = "Not yet check"
        Else
            _date_checked = date_checked
        End If
        _weekdateSched = weekdateSched
    End Sub

    Public Sub New(fyweek As Integer, datess As String, emp As String, note As String, weekdate As String, date_checked As String, weekdateSched As String)
        _fyweek = fyweek
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If
        _weekdate = weekdate
        If date_checked.ToString.Trim.ToLower = "NULL".ToString.Trim.ToLower() Then
            _date_checked = "Not yet check"
        Else
            _date_checked = date_checked
        End If
        _weekdateSched = weekdateSched
    End Sub

    Public Property Days As String
        Get
            Return _days
        End Get
        Set(value As String)
            _days = value
        End Set
    End Property

    Public Property FY_WEEK As Integer
        Get
            Return _fyweek
        End Get
        Set(value As Integer)
            _fyweek = value
        End Set
    End Property

    Public Property Dates As String
        Get
            Return _dates
        End Get
        Set(value As String)
            _dates = value
        End Set
    End Property

    Public Property EmpName As String
        Get
            Return _empName
        End Get
        Set(value As String)
            _empName = value
        End Set
    End Property

    Public Property Notes As String
        Get
            Return _notes
        End Get
        Set(value As String)
            _notes = value
        End Set
    End Property

    Public Property WEEKDATE As String
        Get
            Return _weekdate
        End Get
        Set(value As String)
            _weekdate = value
        End Set
    End Property

    Public Property DATE_CHECKED As String
        Get
            Return _date_checked
        End Get
        Set(value As String)
            _date_checked = value
        End Set
    End Property

    Public Property WEEK_DATE_SCHED As String
        Get
            Return _weekdateSched
        End Get
        Set(value As String)
            _weekdateSched = value
        End Set
    End Property
End Class

Public Class QuestionsDayModel
    Private Qtn As String
    Private Ppl As String
    Private dtchck As String
    Private _questionNumber As Integer
    Private dtchckflg As Integer
    Private _weekdate As String

    Public Sub New()

    End Sub
    Public Sub New(st As String, pl As String, dtchck As String, questionNum As Integer, dt_chck_flg As Integer)
        Qtn = st
        Ppl = pl
        Me.dtchck = dtchck
        _questionNumber = questionNum
        Me.dtchckflg = dt_chck_flg
    End Sub

    Public Sub New(st As String, pl As String, questionNum As Integer, dt_chck_flg As Integer)
        Qtn = st
        Ppl = pl
        _questionNumber = questionNum
        Me.dtchckflg = dt_chck_flg
    End Sub

    Public Sub New(st As String, pl As String, dt_chck_flg As Integer, dtchck As String)
        Qtn = st
        Ppl = pl
        Me.dtchck = dtchck
        Me.dtchckflg = dt_chck_flg
    End Sub

    Public Sub New(st As String, pl As String, dt_chck_flg As Integer, dtchck As String, WEEKDATE As String)
        Qtn = st
        Ppl = pl
        Me.dtchck = dtchck
        Me.dtchckflg = dt_chck_flg
        Me._weekdate = WEEKDATE
    End Sub

    Public Property Questions As String
        Get
            Return Qtn
        End Get
        Set(value As String)
            Qtn = value
        End Set
    End Property

    Public Property People As String
        Get
            Return Ppl
        End Get
        Set(value As String)
            Ppl = value
        End Set
    End Property

    Public Property DT_CHECK As String
        Get
            Return dtchck
        End Get
        Set(value As String)
            dtchck = value
        End Set
    End Property

    Public Property DT_CHECK_FLG As Integer
        Get
            Return dtchckflg
        End Get
        Set(value As Integer)
            dtchckflg = value
        End Set
    End Property

    Public Property QUESTION_NUMBER As Integer
        Get
            Return _questionNumber
        End Get
        Set(value As Integer)
            _questionNumber = value
        End Set
    End Property

    Public Property WEEKDATE As String
        Get
            Return _weekdate
        End Get
        Set(value As String)
            _weekdate = value
        End Set
    End Property
End Class

