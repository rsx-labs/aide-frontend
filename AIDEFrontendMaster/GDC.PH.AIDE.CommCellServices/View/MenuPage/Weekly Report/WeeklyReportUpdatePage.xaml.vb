Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Text.RegularExpressions


<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class WeeklyReportUpdatePage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    Private AideServiceClient As ServiceReference1.AideServiceClient
    Private _ProjectViewModel As New ProjectViewModel
    Public email As String
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private empID As Integer
    Private profile As Profile

    Dim weekRangeID As Integer
    Dim incidentTypeID As Integer = 2
    Dim statusID As Integer = 3
    Dim phaseID As Integer = 4
    Dim reworkID As Integer = 5
    Dim severityID As Integer = 13

    Dim submitStatus As Integer = 3
    Dim workingStatus As Integer = 1
    Dim entryType As Integer = 0

    Dim stsNotStarted As Integer = 1
    Dim stsInPrgress As Integer = 2
    Dim stsCmpltd As Integer = 3
    Dim stsRtrnToTrge As Integer = 4
    Dim stsWaitngForInfo As Integer = 5
    Dim stsOnhold As Integer = 6

    Dim listProjects As New ObservableCollection(Of ProjectModel)
    Dim listReworkStatus As New ObservableCollection(Of ReworkStatusModel)
    Dim listSeverityStatus As New ObservableCollection(Of SeverityStatusModel)
    Dim listCategoryStatus As New ObservableCollection(Of CategoryStatusModel)
    Dim listPhaseStatus As New ObservableCollection(Of PhaseStatusModel)
    Dim listTaskStatus As New ObservableCollection(Of TaskStatusModel)

    Dim lstWeekRange As WeekRange()
    Dim lstWeeklyReportsData As ObservableCollection(Of WeeklyReportModel) = New ObservableCollection(Of WeeklyReportModel)
    Dim lstRemoveWeeklyReportData As ObservableCollection(Of WeeklyReportModel) = New ObservableCollection(Of WeeklyReportModel)

    Dim dateToday As Date = Date.Today
    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)

    Dim totalWeeklyHours As Decimal
    Dim month As Integer
    Dim year As Integer

#End Region

#Region "Provider Declaration"
    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
    Dim taskDBProvider As New TaskDBProvider
    Dim projectDBProvider As New ProjectDBProvider
#End Region

#Region "Model Declaration"
    Dim weeklyReportModel As New WeeklyReportModel
    Dim tasksModel As New TasksModel
#End Region

#Region "View Model Declarations"
    Dim weeklyReportViewModel As New WeeklyReportViewModel
    Dim tasksViewModel As New TasksViewModel
    Dim weekRangeViewModel As New WeekRangeViewModel
    Dim projectViewModel As New ProjectViewModel
#End Region

#Region "Constructor"
    Public Sub New(_weekRangeID As Integer, _frame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        InitializeService()
        ' Add any initialization after the InitializeComponent() call.
        email = _profile.Email_Address
        Me.frame = _frame
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.empID = _profile.Emp_ID
        Me.weekRangeID = _weekRangeID
        Me.profile = _profile

        dgWeeklyReport.ItemsSource = lstWeeklyReportsData
        LoadData()
        LoadWeeklyReportData()
    End Sub

    Public Sub New(_weekRangeID As Integer, _month As Integer, _year As Integer, _frame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _empID As Integer)
        ' This call is required by the designer.
        InitializeComponent()
        InitializeService()
        entryType = 1
        ' Add any initialization after the InitializeComponent() call.
        email = _profile.Email_Address
        month = _month
        year = _year

        Me.frame = _frame
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.empID = _empID
        Me.weekRangeID = _weekRangeID
        Me.profile = _profile

        dgWeeklyReport.ItemsSource = lstWeeklyReportsData
        LoadData()
        LoadWeeklyReportData()
    End Sub

    Private Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim context As InstanceContext = New InstanceContext(Me)
            AideServiceClient = New AideServiceClient(context)
            AideServiceClient.Open()
            bInitialize = True
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function
#End Region

#Region "Sub Procedures"
    Private Sub LoadData()

        ' Load Items For Projects
        Try
            Dim displayStatus As Integer = 1
            Dim lstProjects As Project() = AideServiceClient.GetProjectList(empID, displayStatus)

            For Each objProjects As Project In lstProjects
                projectDBProvider.setProjectList(objProjects)
            Next

            For Each myProjects As myProjectList In projectDBProvider.getProjectList()
                listProjects.Add(New ProjectModel(myProjects))
            Next

            projectViewModel.ProjectList = listProjects
            cbProject.DataContext = projectViewModel
        Catch ex As Exception
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Rework Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(reworkID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyReworkStatusList(objStatus)
            Next

            For Each myStatus As MyReworkStatusList In taskDBProvider.GetReworkStatusList()
                listReworkStatus.Add(New ReworkStatusModel(myStatus))
            Next

            tasksViewModel.ReworkStatusList = listReworkStatus
            cbRework.DataContext = tasksViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Severity Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(severityID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMySeverityStatusList(objStatus)
            Next

            For Each myStatus As MySeverityStatusList In taskDBProvider.GetSeverityStatusList()
                listSeverityStatus.Add(New SeverityStatusModel(myStatus))
            Next

            tasksViewModel.SeverityStatusList = listSeverityStatus
            cbSeverity.DataContext = tasksViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Incident Type Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(incidentTypeID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyCategoryStatusList(objStatus)
            Next

            For Each myStatus As MyCategoryStatusList In taskDBProvider.GetCategoryStatusList()
                listCategoryStatus.Add(New CategoryStatusModel(myStatus))
            Next

            tasksViewModel.CategoryStatusList = listCategoryStatus
            cbIncidentType.DataContext = tasksViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Phase Status Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(phaseID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyPhaseStatusList(objStatus)
            Next

            For Each myStatus As MyPhaseStatusList In taskDBProvider.GetPhaseStatusList()
                listPhaseStatus.Add(New PhaseStatusModel(myStatus))
            Next

            tasksViewModel.PhaseStatusList = listPhaseStatus
            cbPhase.DataContext = tasksViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Task Status Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(statusID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyTaskStatusList(objStatus)
            Next

            For Each myStatus As MyTaskStatusList In taskDBProvider.GetTaskStatusList()
                listTaskStatus.Add(New TaskStatusModel(myStatus))
            Next

            tasksViewModel.TaskStatusList = listTaskStatus
            cbStatus.DataContext = tasksViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items for Week Range Combobox
        Try
            lstWeekRange = AideServiceClient.GetWeekRange(Date.Now, weekRangeID, 0)
            Dim listWeekRange As New ObservableCollection(Of WeekRangeModel)

            For Each objWeekRange As WeekRange In lstWeekRange
                weeklyReportDBProvider.SetWeekRangeList(objWeekRange)
            Next

            For Each weekRange As MyWeekRange In weeklyReportDBProvider.GetWeekRangeList()
                listWeekRange.Add(New WeekRangeModel(weekRange))
            Next

            weekRangeViewModel.WeekRangeList = listWeekRange
            cbDateRange.DataContext = weekRangeViewModel
            cbDateRange.SelectedValue = weekRangeID
        Catch ex As SystemException
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        'If dgWeeklyReport.DataContext Is Nothing Then
        '    dgWeeklyReport.Visibility = Windows.Visibility.Hidden
        'End If
    End Sub

    Private Sub LoadWeeklyReportData()
        Try
            Dim lstWeeklyReport As WeeklyReport() = AideServiceClient.GetWeeklyReportsByWeekRangeID(weekRangeID, dateToday, empID)
            For Each objWeeklyReport As WeeklyReport In lstWeeklyReport
                weeklyReportDBProvider.SetWeeklyReportList(objWeeklyReport)
            Next

            For Each weeklyReport As MyWeeklyReport In weeklyReportDBProvider.GetWeeklyReportList()
                lstWeeklyReportsData.Add(New WeeklyReportModel With {
                                            .WeekID = weeklyReport.WeekID,
                                            .WeekRangeID = weeklyReport.WeekRangeID,
                                            .ProjectID = weeklyReport.ProjectID,
                                            .ProjectDesc = listProjects.Where(Function(x) x.ProjectID = weeklyReport.ProjectID).First().ProjectName,
                                            .Rework = weeklyReport.Rework,
                                            .ReworkDesc = getReworkValue(weeklyReport.Rework),
                                            .RefID = weeklyReport.RefID,
                                            .Subject = weeklyReport.Subject,
                                            .Severity = weeklyReport.Severity,
                                            .SeverityDesc = getSeverityValue(weeklyReport.Severity),
                                            .IncidentType = weeklyReport.IncType,
                                            .IncidentDesc = getIncidentValue(weeklyReport.IncType),
                                            .EmpID = weeklyReport.EmpID,
                                            .Phase = weeklyReport.Phase,
                                            .PhaseDesc = getPhaseValue(weeklyReport.Phase),
                                            .Status = weeklyReport.Status,
                                            .StatusDesc = getStatusValue(weeklyReport.Status),
                                            .DateStarted = weeklyReport.DateStarted,
                                            .DateTarget = weeklyReport.DateTarget,
                                            .DateFinished = weeklyReport.DateFinished,
                                            .EffortEst = weeklyReport.EffortEst,
                                            .ActualEffort = weeklyReport.ActEffort,
                                            .ActualEffortWk = weeklyReport.ActEffortWk,
                                            .Comments = weeklyReport.Comment,
                                            .InboundContacts = weeklyReport.InboundContacts,
                                            .ProjectCode = weeklyReport.ProjectCode,
                                            .TaskID = weeklyReport.TaskID
                                         })
            Next

            If lstWeeklyReportsData.Count > 0 Then
                btnSave.IsEnabled = True
                GetTotalHours()
            End If

        Catch ex As Exception
            AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub DatePicker_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim datePicker As DatePicker = CType(sender, DatePicker)
        If (Not (datePicker) Is Nothing) Then
            Dim datePickerTextBox As System.Windows.Controls.Primitives.DatePickerTextBox = FindVisualChild(Of System.Windows.Controls.Primitives.DatePickerTextBox)(datePicker)
            If (Not (datePickerTextBox) Is Nothing) Then
                Dim watermark As ContentControl = CType(datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox), ContentControl)
                If (Not (watermark) Is Nothing) Then
                    watermark.Content = String.Empty
                    'or set it some value here...
                End If

            End If
        End If
    End Sub

    Private Sub GetTotalHours()
        totalWeeklyHours = 0

        For Each reports In lstWeeklyReportsData
            totalWeeklyHours = totalWeeklyHours + reports.ActualEffortWk
        Next

        If (totalWeeklyHours >= 40) Then
            btnSubmit.IsEnabled = True
        Else
            btnSubmit.IsEnabled = False
        End If

        tbTotalHours.Text = totalWeeklyHours.ToString
    End Sub
#End Region

#Region "Events"
    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        Select Case entryType
            Case 0
                ExitPage()
            Case 1
                ExitPage2()
        End Select
    End Sub

    Private Sub dgWeeklyReport_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgWeeklyReport.SelectionChanged
        Dim selectedItem = dgWeeklyReport.SelectedItem

        If selectedItem IsNot Nothing Then
            cbProject.SelectedValue = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ProjectID
            cbRework.SelectedValue = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Rework
            txtRefID.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).RefID
            txtSubject.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Subject
            cbSeverity.SelectedValue = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Severity
            cbIncidentType.SelectedValue = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).IncidentType
            dpStartDate.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateStarted
            dpTargetDate.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateTarget
            dpCompletedDate.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateFinished
            txtComments.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Comments
            cbPhase.SelectedValue = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Phase
            cbStatus.SelectedValue = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Status
            txtEffortEst.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).EffortEst
            txtActualEffort.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ActualEffort
            txtActualEffortWk.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ActualEffortWk
            txtInboundContacts.Text = lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).InboundContacts

            btnUpdate.IsEnabled = True
            btnRemove.IsEnabled = True
        Else
            btnUpdate.IsEnabled = False
            btnRemove.IsEnabled = False
        End If
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim isValidate As Boolean

        isValidate = ValidateFields(isValidate)

        If isValidate Then
            lstWeeklyReportsData.Add(New WeeklyReportModel With {
                         .WeekRangeID = cbDateRange.SelectedValue,
                         .EmpID = empID,
                         .ProjectID = cbProject.SelectedValue,
                         .ProjectDesc = cbProject.Text,
                         .Rework = cbRework.SelectedValue,
                         .ReworkDesc = cbRework.Text,
                         .RefID = txtRefID.Text,
                         .Subject = txtSubject.Text,
                         .Severity = cbSeverity.SelectedValue,
                         .SeverityDesc = cbSeverity.Text,
                         .IncidentType = cbIncidentType.SelectedValue,
                         .IncidentDesc = cbIncidentType.Text,
                         .Phase = cbPhase.SelectedValue,
                         .PhaseDesc = cbPhase.Text,
                         .Status = cbStatus.SelectedValue,
                         .StatusDesc = cbStatus.Text,
                         .DateStarted = dpStartDate.Text,
                         .DateTarget = dpTargetDate.Text,
                         .DateFinished = dpCompletedDate.Text,
                         .EffortEst = txtEffortEst.Text,
                         .ActualEffort = txtActualEffort.Text,
                         .ActualEffortWk = txtActualEffortWk.Text,
                         .Comments = txtComments.Text,
                         .InboundContacts = txtInboundContacts.Text})

            GetTotalHours()
            ClearFields()
            dgWeeklyReport.SelectedIndex = -1

            If lstWeeklyReportsData.Count > 0 Then
                btnSave.IsEnabled = True
            End If
        End If
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As RoutedEventArgs) Handles btnRemove.Click
        Dim selectedItem = dgWeeklyReport.SelectedItem
        lstRemoveWeeklyReportData.Add(selectedItem)

        If selectedItem IsNot Nothing Then
            lstWeeklyReportsData.RemoveAt(dgWeeklyReport.SelectedIndex)
            ClearFields()
            GetTotalHours()
            dgWeeklyReport.SelectedIndex = -1

            If lstWeeklyReportsData.Count = 0 Then
                btnSubmit.IsEnabled = False
                btnSave.IsEnabled = False
            End If
        End If
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        Dim isValidate As Boolean

        isValidate = ValidateFields(isValidate)

        If isValidate Then
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ProjectID = cbProject.SelectedValue
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ProjectDesc = cbProject.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Rework = cbRework.SelectedValue
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ReworkDesc = cbRework.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).RefID = txtRefID.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Subject = txtSubject.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Severity = cbSeverity.SelectedValue
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).SeverityDesc = cbSeverity.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).IncidentType = cbIncidentType.SelectedValue
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).IncidentDesc = cbIncidentType.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Phase = cbPhase.SelectedValue
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).PhaseDesc = cbPhase.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Status = cbStatus.SelectedValue
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).StatusDesc = cbStatus.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateStarted = dpStartDate.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateTarget = dpTargetDate.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateFinished = dpCompletedDate.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).EffortEst = txtEffortEst.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ActualEffort = txtActualEffort.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ActualEffortWk = txtActualEffortWk.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Comments = txtComments.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).InboundContacts = txtInboundContacts.Text

            GetTotalHours()
            ClearFields()
            dgWeeklyReport.SelectedIndex = -1
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As RoutedEventArgs) Handles btnSubmit.Click
        If lstWeeklyReportsData.Count > 0 Then

            Dim deletedWeeklyReport As New List(Of WeeklyReport)
            Dim weeklyReport As New List(Of WeeklyReport)
            Dim weeklyReportXref As New WeekRange

            Try
                weeklyReportXref.WeekRangeID = cbDateRange.SelectedValue
                weeklyReportXref.EmployeeID = empID
                weeklyReportXref.Status = submitStatus
                weeklyReportXref.DateSubmitted = Date.Now

                For Each reports In lstWeeklyReportsData

                    Dim objReports As New WeeklyReport
                    objReports.WeekID = reports.WeekID
                    objReports.WeekRangeID = cbDateRange.SelectedValue
                    objReports.EmpID = reports.EmpID
                    objReports.ProjectID = reports.ProjectID
                    objReports.Rework = reports.Rework
                    objReports.ReferenceID = reports.RefID
                    objReports.Subject = reports.Subject
                    objReports.Severity = reports.Severity
                    objReports.IncidentType = reports.IncidentType
                    objReports.Phase = reports.Phase
                    objReports.Status = reports.Status
                    objReports.EffortEst = reports.EffortEst
                    objReports.ActualEffort = reports.ActualEffort
                    objReports.ActualEffortWk = reports.ActualEffortWk
                    objReports.Comments = reports.Comments

                    If reports.DateStarted IsNot String.Empty Then
                        objReports.DateStarted = reports.DateStarted
                    End If

                    If reports.DateTarget IsNot String.Empty Then
                        objReports.DateTarget = reports.DateTarget
                    End If

                    If reports.DateFinished IsNot String.Empty Then
                        objReports.DateFinished = reports.DateFinished
                    End If

                    If reports.InboundContacts IsNot String.Empty Then
                        objReports.InboundContacts = reports.InboundContacts
                    End If

                    objReports.ProjCode = reports.ProjectCode
                    objReports.TaskID = reports.TaskID

                    weeklyReport.Add(objReports)
                Next

                ' Get remove weekly report data
                For Each reports In lstRemoveWeeklyReportData
                    Dim objReports As New WeeklyReport
                    objReports.WeekID = reports.WeekID
                    objReports.WeekRangeID = reports.WeekRangeID
                    objReports.EmpID = reports.EmpID
                    objReports.TaskID = reports.TaskID

                    deletedWeeklyReport.Add(objReports)
                Next

                If totalWeeklyHours < 40 Then
                    MsgBox("Actual effort hours [Total:" + totalWeeklyHours.ToString + "] is insufficient.", MsgBoxStyle.Critical, "AIDE")
                Else
                    Dim result As Integer = MsgBox("Update weekly report on week " + cbDateRange.Text + "?", MsgBoxStyle.YesNo, "AIDE")

                    If result = vbYes Then
                        If InitializeService() Then
                            AideServiceClient.UpdateWeeklyReport(weeklyReport.ToArray, deletedWeeklyReport.ToArray, weeklyReportXref)
                            MsgBox("Weekly report has been updated.", MsgBoxStyle.Information, "AIDE")
                            ExitPage()
                        End If
                    End If
                End If

            Catch ex As Exception
                 MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        If cbDateRange.SelectedIndex = -1 Then
            If cbDateRange.SelectedIndex = -1 Then
                MsgBox("Please select a date range", MsgBoxStyle.Critical, "AIDE")
                cbDateRange.Focus()
            End If
        Else
            If lstWeeklyReportsData.Count > 0 Then
                Dim deletedWeeklyReport As New List(Of WeeklyReport)
                Dim weeklyReport As New List(Of WeeklyReport)
                Dim weeklyReportXref As New WeekRange

                Try
                    weeklyReportXref.WeekRangeID = cbDateRange.SelectedValue
                    weeklyReportXref.EmployeeID = empID
                    weeklyReportXref.Status = workingStatus
                    weeklyReportXref.DateSubmitted = Date.Now

                    For Each reports In lstWeeklyReportsData
                        Dim objReports As New WeeklyReport
                        objReports.WeekID = reports.WeekID
                        objReports.WeekRangeID = cbDateRange.SelectedValue
                        objReports.EmpID = reports.EmpID
                        objReports.ProjectID = reports.ProjectID
                        objReports.Rework = reports.Rework
                        objReports.ReferenceID = reports.RefID
                        objReports.Subject = reports.Subject
                        objReports.Severity = reports.Severity
                        objReports.IncidentType = reports.IncidentType
                        objReports.Phase = reports.Phase
                        objReports.Status = reports.Status
                        objReports.EffortEst = reports.EffortEst
                        objReports.ActualEffort = reports.ActualEffort
                        objReports.ActualEffortWk = reports.ActualEffortWk
                        objReports.Comments = reports.Comments

                        If reports.DateStarted IsNot String.Empty Then
                            objReports.DateStarted = reports.DateStarted
                        End If

                        If reports.DateTarget IsNot String.Empty Then
                            objReports.DateTarget = reports.DateTarget
                        End If

                        If reports.DateFinished IsNot String.Empty Then
                            objReports.DateFinished = reports.DateFinished
                        End If

                        If reports.InboundContacts IsNot String.Empty Then
                            objReports.InboundContacts = reports.InboundContacts
                        End If

                        objReports.ProjCode = reports.ProjectCode
                        objReports.TaskID = reports.TaskID

                        weeklyReport.Add(objReports)
                    Next

                    ' Get remove weekly report data
                    For Each reports In lstRemoveWeeklyReportData
                        Dim objReports As New WeeklyReport
                        objReports.WeekID = reports.WeekID
                        objReports.WeekRangeID = reports.WeekRangeID
                        objReports.EmpID = reports.EmpID
                        objReports.TaskID = reports.TaskID

                        deletedWeeklyReport.Add(objReports)
                    Next

                    Dim result As Integer = MsgBox("Save weekly report for the week " + cbDateRange.Text + "?", MsgBoxStyle.YesNo, "AIDE")

                    If result = vbYes Then
                        If InitializeService() Then
                            AideServiceClient.UpdateWeeklyReport(weeklyReport.ToArray, deletedWeeklyReport.ToArray, weeklyReportXref)
                            MsgBox("Weekly report have been updated", MsgBoxStyle.Information)
                            Select Case entryType
                                Case 0
                                    ExitPage()
                                Case 1
                                    ExitPage2()
                            End Select
                        End If
                    End If

                Catch ex As Exception
                     MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
                End Try
            End If
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As RoutedEventArgs) Handles btnClear.Click
        ClearFields()
    End Sub

    Private Sub txtRefID_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtRefID.TextChanged
        If txtRefID.Text IsNot String.Empty Then
            If Not txtIncidentType.Text.Contains("*") Then
                txtIncidentType.Text = "Select a category *"
            End If

            'If Not txtTargetDate.Text.Contains("*") Then
            '    txtTargetDate.Text = "Target date *"
            'End If

            If Not txtEffort.Text.Contains("*") Then
                txtEffort.Text = "Estimate effort(hrs) *"
            End If
        Else
            txtIncidentType.Text = "Select a category"
            'txtTargetDate.Text = "Target date"
            txtEffort.Text = "Estimate effort(hrs)"
        End If
    End Sub

    Private Sub dpCompletedDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dpCompletedDate.SelectedDateChanged
        If dpCompletedDate.Text IsNot String.Empty Then
            Dim stsValue As Integer = stsCmpltd

            If cbStatus.SelectedValue = stsRtrnToTrge Then
                stsValue = stsRtrnToTrge
            End If

            cbStatus.SelectedValue = stsValue

            If Not txtStartDate.Text.Contains("*") Then
                txtStartDate.Text = "Select start date *"
            End If
        Else
            If cbStatus.SelectedValue = stsNotStarted Then
                txtStartDate.Text = "Select start date"
            End If
        End If
    End Sub

    Private Sub cbIncidentType_DropDownClosed(sender As Object, e As EventArgs) Handles cbIncidentType.DropDownClosed
        If cbIncidentType.Text IsNot String.Empty Then
            If Not txtPhase.Text.Contains("*") Then
                txtPhase.Text = "Select phase *"
            End If
        Else
            txtPhase.Text = "Select phase"
        End If
    End Sub

    Private Sub cbIncidentType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbIncidentType.SelectionChanged
        If cbIncidentType.SelectedIndex = -1 Then
            txtPhase.Text = "Select phase"
        End If
    End Sub

    Private Sub cbStatus_DropDownClosed(sender As Object, e As EventArgs) Handles cbStatus.DropDownClosed
        If Not cbStatus.SelectedValue = stsNotStarted Then
            If Not txtStartDate.Text.Contains("*") Then
                txtStartDate.Text = "Select start date *"
            End If

            If cbStatus.SelectedValue = stsCmpltd Or cbStatus.SelectedValue = stsRtrnToTrge Then
                dpCompletedDate.SelectedDate = Date.Now
                txtDateCompleted.Text = "Select completed date *"
            Else
                dpCompletedDate.Text = String.Empty
                txtDateCompleted.Text = "Select completed date"
            End If
        Else
            txtStartDate.Text = "Select start date"
            dpCompletedDate.Text = String.Empty
        End If
    End Sub

    Private Sub cbStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbStatus.SelectionChanged
        If cbStatus.SelectedIndex = -1 Then
            txtStartDate.Text = "Select start date"
        End If
    End Sub

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex = New Regex("^[0-9]*(?:\.[0-9]*)?$")

        If regex.IsMatch(e.Text) AndAlso Not (e.Text = "." AndAlso (CType(sender, TextBox)).Text.Contains(e.Text)) Then
            e.Handled = False
        Else
            e.Handled = True
        End If
    End Sub
#End Region

#Region "Functions"
    Private Function getReworkValue(key As Integer) As String
        Dim value = listReworkStatus.Where(Function(x) x.Key = key).FirstOrDefault()

        If value Is Nothing Then
            Return ""
        Else
            Return listReworkStatus.Where(Function(x) x.Key = key).First().Value
        End If
    End Function

    Private Function getSeverityValue(key As Integer) As String
        Dim value = listSeverityStatus.Where(Function(x) x.Key = key).FirstOrDefault()

        If value Is Nothing Then
            Return ""
        Else
            Return listSeverityStatus.Where(Function(x) x.Key = key).First().Value
        End If
    End Function

    Private Function getIncidentValue(key As Integer) As String
        Dim value = listCategoryStatus.Where(Function(x) x.Key = key).FirstOrDefault()

        If value Is Nothing Then
            Return ""
        Else
            Return listCategoryStatus.Where(Function(x) x.Key = key).First().Value
        End If
    End Function

    Private Function getPhaseValue(key As Integer) As String
        Dim value = listPhaseStatus.Where(Function(x) x.Key = key).FirstOrDefault()

        If value Is Nothing Then
            Return ""
        Else
            Return listPhaseStatus.Where(Function(x) x.Key = key).First().Value
        End If
    End Function

    Private Function getStatusValue(key As Integer) As String
        Dim value = listTaskStatus.Where(Function(x) x.Key = key).FirstOrDefault()

        If value Is Nothing Then
            Return ""
        Else
            Return listTaskStatus.Where(Function(x) x.Key = key).First().Value
        End If
    End Function

    Private Function ValidateFields(isValidate As Boolean)
        If cbProject.SelectedIndex = -1 Then
            MsgBox("Please select a project", MsgBoxStyle.Critical, "AIDE")
            cbProject.Focus()
            Return False
        End If

        If txtSubject.Text Is String.Empty Then
            MsgBox("Please enter a description", MsgBoxStyle.Critical, "AIDE")
            txtSubject.Focus()
            Return False
        End If

        If txtRefID.Text IsNot String.Empty And cbIncidentType.SelectedIndex = -1 Then
            MsgBox("Please select a category", MsgBoxStyle.Critical, "AIDE")
            cbIncidentType.Focus()
            Return False
        End If

        If Not cbIncidentType.SelectedIndex = -1 And cbPhase.SelectedIndex = -1 Then
            MsgBox("Please select phase", MsgBoxStyle.Critical, "AIDE")
            cbPhase.Focus()
            Return False
        End If

        If cbStatus.SelectedIndex = -1 Then
            MsgBox("Please select a status.", MsgBoxStyle.Critical, "AIDE")
            cbStatus.Focus()
            Return False
        End If

        If dpCompletedDate.Text IsNot String.Empty And dpStartDate.Text = String.Empty Then
            MsgBox("Please enter a start date.", MsgBoxStyle.Critical, "AIDE")
            dpStartDate.Focus()
            Return False
        End If

        'If dpCompletedDate.Text IsNot String.Empty And txtRefID.Text IsNot String.Empty Then
        '    MsgBox("Please enter target date", MsgBoxStyle.Critical, "AIDE")
        '    dpTargetDate.Focus()
        '    Return False
        'End If

        If (cbStatus.SelectedValue = stsInPrgress OrElse cbStatus.SelectedValue = stsWaitngForInfo OrElse cbStatus.SelectedValue = stsOnhold) And dpStartDate.Text Is String.Empty Then
            MsgBox("Please enter a start date.", MsgBoxStyle.Critical, "AIDE")
            dpStartDate.Focus()
            Return False
        End If

        If cbStatus.SelectedValue = stsCmpltd And dpCompletedDate.Text Is String.Empty Then
            MsgBox("Please enter a completed date.", MsgBoxStyle.Critical, "AIDE")
            dpCompletedDate.Focus()
            Return False
        End If

        If txtEffortEst.Text Is String.Empty And txtRefID.Text IsNot String.Empty Then
            MsgBox("Please enter an estimate effort.", MsgBoxStyle.Critical, "AIDE")
            txtEffortEst.Focus()
            Return False
        End If

        If txtActualEffortWk.Text Is String.Empty Then
            MsgBox("Please enter an actual effort for the week.", MsgBoxStyle.Critical, "AIDE")
            txtActualEffortWk.Focus()
            Return False
        End If

        If txtActualEffort.Text Is String.Empty Then
            MsgBox("Please enter an actual effort.", MsgBoxStyle.Critical, "AIDE")
            txtActualEffort.Focus()
            Return False
        End If


        Return True
    End Function

    Private Sub ClearFields()
        cbProject.Text = String.Empty
        cbRework.Text = String.Empty
        txtRefID.Text = String.Empty
        txtSubject.Text = String.Empty
        cbSeverity.SelectedIndex = -1
        cbIncidentType.SelectedIndex = -1
        dpStartDate.Text = String.Empty
        dpTargetDate.Text = String.Empty
        dpCompletedDate.Text = String.Empty
        txtComments.Text = String.Empty
        cbPhase.SelectedIndex = -1
        cbStatus.SelectedIndex = -1
        txtEffortEst.Text = String.Empty
        txtActualEffort.Text = String.Empty
        txtActualEffortWk.Text = String.Empty
        txtInboundContacts.Text = String.Empty

        dgWeeklyReport.SelectedIndex = -1
    End Sub

    Private Function FindVisualChild(Of T)(ByVal depencencyObject As DependencyObject) As T
        If (Not (depencencyObject) Is Nothing) Then
            Dim i As Integer = 0
            Do While (i < VisualTreeHelper.GetChildrenCount(depencencyObject))
                Dim child As DependencyObject = VisualTreeHelper.GetChild(depencencyObject, i)
                Dim result As T
                FindVisualChild(Of T)(child)
                If (Not (result) Is Nothing) Then
                    Return result
                End If

                i = (i + 1)
            Loop
        End If

        Return Nothing
    End Function

    Private Sub ExitPage()
        frame.Navigate(New WeeklyReportPage(frame, Profile, addframe, menugrid, submenuframe))
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub ExitPage2()
        frame.Navigate(New WeeklyTeamStatusReportPage(frame, profile, addframe, menugrid, submenuframe, weekRangeID, month, year))
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "Callback Functions"
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
