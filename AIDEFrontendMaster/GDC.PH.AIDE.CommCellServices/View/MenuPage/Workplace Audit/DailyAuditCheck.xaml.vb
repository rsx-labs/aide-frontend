Imports System.Data
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class DailyAuditCheck
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback


#Region "Page Declaration"
    Public _frame As Frame
    Private aide As AideServiceClient
    Private auditSched As New AuditSched
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile
    Private auditSchedID As Integer

    Private dailyVMM As New dayVM
    Private _AideService As ServiceReference1.AideServiceClient
    Private _lstAuditQuestionSelected As New ObservableCollection(Of WorkplaceAuditModel)
    Private auditDisplay As Integer
#End Region

#Region "Constructors"
    'Add Constructor
    Public Sub New(_pageframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _lstAuditQuestions As ObservableCollection(Of WorkplaceAuditModel), AuditType As Integer)
        Try
            Me._frame = _pageframe
            Me._addframe = _addframe
            Me._menugrid = _menugrid
            Me._submenuframe = _submenuframe
            Me.profile = _profile
            Me.auditDisplay = AuditType
            Me._lstAuditQuestionSelected = _lstAuditQuestions
            InitializeComponent()
            DisplayPageTitle()

            LoadData()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub


#End Region

#Region "Methods/Functions"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()

            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

#End Region

#Region "Events"

    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        ReturnToLastPage()
    End Sub

    Private Sub ReturnToLastPage()
        If auditDisplay = 1 Then
            _frame.Navigate(New DailyAuditPage(_frame, profile, _addframe, _menugrid, _submenuframe))
            _frame.IsEnabled = True
            _frame.Opacity = 1
            _menugrid.IsEnabled = True
            _menugrid.Opacity = 1
            _submenuframe.IsEnabled = True
            _submenuframe.Opacity = 1
            _addframe.Visibility = Visibility.Hidden
        ElseIf auditDisplay = 2 Then
            _frame.Navigate(New WeeklyAuditPage(_frame, profile, _addframe, _menugrid, _submenuframe))
            _frame.IsEnabled = True
            _frame.Opacity = 1
            _menugrid.IsEnabled = True
            _menugrid.Opacity = 1
            _submenuframe.IsEnabled = True
            _submenuframe.Opacity = 1
            _addframe.Visibility = Visibility.Hidden
        ElseIf auditDisplay = 3 Then
            _frame.Navigate(New MonthlyAuditPage(_frame, profile, _addframe, _menugrid, _submenuframe))
            _frame.IsEnabled = True
            _frame.Opacity = 1
            _menugrid.IsEnabled = True
            _menugrid.Opacity = 1
            _submenuframe.IsEnabled = True
            _submenuframe.Opacity = 1
            _addframe.Visibility = Visibility.Hidden
        ElseIf auditDisplay = 4 Then
            _frame.Navigate(New QuarterlyAuditPage(_frame, profile, _addframe, _menugrid, _submenuframe))
            _frame.IsEnabled = True
            _frame.Opacity = 1
            _menugrid.IsEnabled = True
            _menugrid.Opacity = 1
            _submenuframe.IsEnabled = True
            _submenuframe.Opacity = 1
            _addframe.Visibility = Visibility.Hidden
        End If
    End Sub
    Private Function DisplayPageTitle()
        Dim strDisplay As String = ""
        Select Case auditDisplay
            Case 1
                strDisplay = "Daily Audit"
            Case 2
                strDisplay = "Weekly Audit"
            Case 3
                strDisplay = "Monthly Audit"
            Case 4
                strDisplay = "Quarterly Audit"
        End Select

        txtReminder.Text = "Please Check " & strDisplay
        txtHeader.Text = "Check " & strDisplay
        Return strDisplay
    End Function
    Private Sub LoadData()

        Dim iquestioNumber As Integer = 1
        For Each quest As WorkplaceAuditModel In _lstAuditQuestionSelected.ToList
            dailyVMM.QuestionDayList.Add(New QuestionsDayModel(quest.AUDIT_QUESTIONS, quest.OWNER, iquestioNumber, quest.DT_CHECK_FLG))
            iquestioNumber += 1
        Next

        QuarterLVQuestions.ItemsSource = dailyVMM.QuestionDayList
        DataContext = dailyVMM.QuestionDayList
    End Sub

    Private Sub Validate_Click(sender As Object, e As RoutedEventArgs)
        Dim btnName As String = sender.Name().ToString()
        Dim dt_check_flg As Integer

        Select Case btnName
            Case "btnYes"
                dt_check_flg = 1
            Case "btnNo"
                dt_check_flg = 2
        End Select

        Dim item = (TryCast(sender, FrameworkElement)).DataContext
        Dim iquestioNumber As Integer = 1

        dailyVMM.QuestionDayList.Clear()
        For Each quest As WorkplaceAuditModel In _lstAuditQuestionSelected.ToList
            If quest.AUDIT_QUESTIONS.ToString.Trim() = item.Questions.ToString.Trim() Then
                quest.DT_CHECKED = Date.Now.ToString
                quest.DT_CHECK_FLG = dt_check_flg
            End If
            dailyVMM.QuestionDayList.Add(New QuestionsDayModel(quest.AUDIT_QUESTIONS, quest.OWNER, quest.DT_CHECKED, iquestioNumber, quest.DT_CHECK_FLG))
            iquestioNumber += 1
        Next

        QuarterLVQuestions.ItemsSource = dailyVMM.QuestionDayList
        DataContext = dailyVMM.QuestionDayList

    End Sub

    Private Sub Save_Btn(sender As Object, e As RoutedEventArgs)

        Try
            If InitializeService() Then
                Dim InsertUpdatedDataInAudit As New WorkplaceAudit
                For Each quest As WorkplaceAuditModel In _lstAuditQuestionSelected.ToList
                    InsertUpdatedDataInAudit.DT_CHECKED = quest.DT_CHECKED
                    InsertUpdatedDataInAudit.DT_CHECK_FLG = quest.DT_CHECK_FLG
                    InsertUpdatedDataInAudit.AUDIT_QUESTIONS_ID = quest.AUDIT_QUESTIONS_ID
                    InsertUpdatedDataInAudit.FY_WEEK = quest.FY_WEEK
                    InsertUpdatedDataInAudit.AUDIT_DAILY_ID = quest.AUDIT_DAILY_ID
                    InsertUpdatedDataInAudit.WEEKDATE = quest.WEEKDATE
                    InsertUpdatedDataInAudit.AUDIT_QUESTIONS_GROUP = quest.AUDIT_QUESTIONS_GROUP
                    _AideService.UpdateCheckAuditQuestionStatus(InsertUpdatedDataInAudit)
                Next

                MsgBox(DisplayPageTitle() & " have been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")


                ReturnToLastPage()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
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
