Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Text.RegularExpressions


<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class WeeklyReportAddPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    Private AideServiceClient As ServiceReference1.AideServiceClient
    Private _ProjectViewModel As New ProjectViewModel
    Private _EmployeeListViewModel As New EmployeeListViewModel
    Private _employeeList As New ObservableCollection(Of EmployeeModel)
    Public email As String
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private empID As Integer

    Dim selectedValue As Integer
    Dim incidentTypeID As Integer = 2
    Dim statusID As Integer = 3
    Dim phaseID As Integer = 4
    Dim reworkID As Integer = 5
    Dim severityID As Integer = 13

    Dim lstWeekRange As WeekRange()
    Dim lstWeeklyReportsData As ObservableCollection(Of WeeklyReportModel) = New ObservableCollection(Of WeeklyReportModel)

    Dim dateToday As Date = Date.Today
    Dim dayMonDiff As Integer = Today.DayOfWeek - DayOfWeek.Monday
    Dim monday As Date = Today.AddDays(-dayMonDiff)
    Dim lastWeekMonday As Date = monday.AddDays(-7)
#End Region

#Region "Provider Declaration"
    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
    Dim projectDBProvider As New ProjectDBProvider
#End Region

#Region "Model Declaration"
    Dim weeklyReportModel As New WeeklyReportModel
#End Region

#Region "View Model Declarations"
    Dim weeklyReportViewModel As New WeeklyReportViewModel
    Dim weekRangeViewModel As New WeekRangeViewModel
    Dim projectViewModel As New ProjectViewModel
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _empID As Integer, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        email = _email
        Me.frame = _frame
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.empID = _empID

        dgWeeklyReport.ItemsSource = lstWeeklyReportsData
        GenerateWeekRange()
        LoadData()
        GetPreviousData() ' Get last week data for the tasks that are not completed yet
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
        End Try
        Return bInitialize
    End Function
#End Region

#Region "Sub Procedures"

    Private Sub GenerateWeekRange()
        Try
            InitializeService()
            Dim weekRange As New WeekRange
            weekRange.StartWeek = Date.Now
            AideServiceClient.CreateWeekRange(weekRange)
        Catch ex As Exception
            AideServiceClient.Abort()
        End Try
    End Sub

    Private Sub GetPreviousData()
        InitializeService()
        Try
            'Dim lstWeeklyReport As WeeklyReport() = AideServiceClient.GetWeeklyReportsNotCompleted(lastWeekMonday, empID)
            'For Each objWeeklyReport As WeeklyReport In lstWeeklyReport
            '    weeklyReportDBProvider.SetWeeklyReportList(objWeeklyReport)
            'Next

            'For Each weeklyReport As MyWeeklyReport In weeklyReportDBProvider.GetWeeklyReportList()
            '    lstWeeklyReportsData.Add(New WeeklyReportModel With {
            '                                .WeekID = weeklyReport.WeekID,
            '                                .WeekRangeID = weeklyReport.WeekRangeID,
            '                                .ProjectID = weeklyReport.ProjectID,
            '                                .ProjectDesc = listProjects.Where(Function(x) x.ProjectID = weeklyReport.ProjectID).First().ProjectName,
            '                                .Rework = weeklyReport.Rework,
            '                                .ReworkDesc = getReworkValue(weeklyReport.Rework),
            '                                .RefID = weeklyReport.RefID,
            '                                .Subject = weeklyReport.Subject,
            '                                .Severity = weeklyReport.Severity,
            '                                .SeverityDesc = getSeverityValue(weeklyReport.Severity),
            '                                .IncidentType = weeklyReport.IncType,
            '                                .IncidentDesc = getIncidentValue(weeklyReport.IncType),
            '                                .EmpID = weeklyReport.EmpID,
            '                                .Phase = weeklyReport.Phase,
            '                                .PhaseDesc = getPhaseValue(weeklyReport.Phase),
            '                                .Status = weeklyReport.Status,
            '                                .StatusDesc = getStatusValue(weeklyReport.Status),
            '                                .DateStarted = weeklyReport.DateStarted,
            '                                .DateTarget = weeklyReport.DateTarget,
            '                                .DateFinished = weeklyReport.DateFinished,
            '                                .DateCreated = weeklyReport.DateCreated,
            '                                .EffortEst = weeklyReport.EffortEst,
            '                                .ActualEffort = weeklyReport.ActEffort,
            '                                .ActualEffortWk = weeklyReport.ActEffortWk,
            '                                .Comments = weeklyReport.Comment,
            '                                .InboundContacts = weeklyReport.InboundContacts
            '                             })
            'Next
        Catch ex As Exception
            AideServiceClient.Abort()
        End Try
    End Sub

    Private Sub LoadData()
        InitializeService()

        ' Load Items For Projects
        Try
            Dim lstProjects As Project() = AideServiceClient.GetProjectList(empID)
            Dim listProjects As New ObservableCollection(Of ProjectModel)

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
        End Try

        ' Load Items For Rework Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(reworkID)
            Dim listReworkStatus As New ObservableCollection(Of WReworkStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMyReworkStatusList(objStatus)
            Next

            For Each myStatus As MyWReworkStatusList In weeklyReportDBProvider.GetReworkStatusList()
                listReworkStatus.Add(New WReworkStatusModel(myStatus))
            Next

            weeklyReportViewModel.ReworkStatusList = listReworkStatus
            cbRework.DataContext = weeklyReportViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try

        ' Load Items For Severity Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(severityID)
            Dim listSeverityStatus As New ObservableCollection(Of SeverityStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMySeverityStatusList(objStatus)
            Next

            For Each myStatus As MySeverityStatusList In weeklyReportDBProvider.GetSeverityStatusList()
                listSeverityStatus.Add(New SeverityStatusModel(myStatus))
            Next

            weeklyReportViewModel.SeverityStatusList = listSeverityStatus
            cbSeverity.DataContext = weeklyReportViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try

        ' Load Items For Incident Type Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(incidentTypeID)
            Dim listCategoryStatus As New ObservableCollection(Of WCategoryStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMyCategoryStatusList(objStatus)
            Next

            For Each myStatus As MyWCategoryStatusList In weeklyReportDBProvider.GetCategoryStatusList()
                listCategoryStatus.Add(New WCategoryStatusModel(myStatus))
            Next

            weeklyReportViewModel.CategoryStatusList = listCategoryStatus
            cbIncidentType.DataContext = weeklyReportViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try

        ' Load Items For Phase Status Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(phaseID)
            Dim listPhaseStatus As New ObservableCollection(Of WPhaseStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMyPhaseStatusList(objStatus)
            Next

            For Each myStatus As MyWPhaseStatusList In weeklyReportDBProvider.GetPhaseStatusList()
                listPhaseStatus.Add(New WPhaseStatusModel(myStatus))
            Next

            weeklyReportViewModel.PhaseStatusList = listPhaseStatus
            cbPhase.DataContext = weeklyReportViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try

        ' Load Items For Task Status Combobox
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(statusID)
            Dim listTaskStatus As New ObservableCollection(Of WTaskStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMyTaskStatusList(objStatus)
            Next

            For Each myStatus As MyWTaskStatusList In weeklyReportDBProvider.GetTaskStatusList()
                listTaskStatus.Add(New WTaskStatusModel(myStatus))
            Next

            weeklyReportViewModel.TaskStatusList = listTaskStatus
            cbStatus.DataContext = weeklyReportViewModel
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try

        ' Load Items for Week Range Combobox
        Try
            lstWeekRange = AideServiceClient.GetWeekRange(Date.Now, empID)
            Dim listWeekRange As New ObservableCollection(Of WeekRangeModel)

            For Each objWeekRange As WeekRange In lstWeekRange
                weeklyReportDBProvider.SetWeekRangeList(objWeekRange)
            Next

            For Each weekRange As MyWeekRange In weeklyReportDBProvider.GetWeekRangeList()
                listWeekRange.Add(New WeekRangeModel(weekRange))

                If monday = weekRange.StartWeek Then
                    selectedValue = weekRange.WeekRangeID
                End If
            Next

            weekRangeViewModel.WeekRangeList = listWeekRange
            cbDateRange.DataContext = weekRangeViewModel
            cbDateRange.SelectedValue = selectedValue
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try

        'If dgWeeklyReport.DataContext Is Nothing Then
        '    dgWeeklyReport.Visibility = Windows.Visibility.Hidden
        'End If
    End Sub
#End Region

#Region "Events"
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

    Private Sub cbDateRange_DropDownClosed(sender As Object, e As EventArgs) Handles cbDateRange.DropDownClosed
        If Not selectedValue = cbDateRange.SelectedValue Then
            If lstWeeklyReportsData.Count > 0 Then
                Dim result As Integer = MsgBox("Changing Period Date will delete input data?", MsgBoxStyle.YesNo, "AIDE")

                If result = vbYes Then
                    selectedValue = cbDateRange.SelectedValue
                    lstWeeklyReportsData.Clear()
                    btnSubmit.IsEnabled = False
                Else
                    cbDateRange.SelectedValue = selectedValue
                End If
            Else
                selectedValue = cbDateRange.SelectedValue
            End If
        End If
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Try
            Dim isValidate As Boolean

            isValidate = ValidateFields(isValidate)

            If isValidate Then
                lstWeeklyReportsData.Add(New WeeklyReportModel With {
                             .WeekRangeID = 1,
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
                             .DateCreated = Date.Now,
                             .EffortEst = txtEffortEst.Text,
                             .ActualEffort = txtActualEffort.Text,
                             .ActualEffortWk = txtActualEffortWk.Text,
                             .Comments = txtComments.Text,
                             .InboundContacts = txtInboundContacts.Text})

                ClearFields()
                dgWeeklyReport.SelectedIndex = -1
                btnSubmit.IsEnabled = True
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "AIDE")
        End Try
        
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As RoutedEventArgs) Handles btnRemove.Click
        Dim selectedItem = dgWeeklyReport.SelectedItem

        If selectedItem IsNot Nothing Then
            lstWeeklyReportsData.RemoveAt(dgWeeklyReport.SelectedIndex)
            ClearFields()
            dgWeeklyReport.SelectedIndex = -1

            If lstWeeklyReportsData.Count = 0 Then
                btnSubmit.IsEnabled = False
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
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).DateCreated = Date.Now
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).EffortEst = txtEffortEst.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ActualEffort = txtActualEffort.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).ActualEffortWk = txtActualEffortWk.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).Comments = txtComments.Text
            lstWeeklyReportsData.Item(dgWeeklyReport.SelectedIndex).InboundContacts = txtInboundContacts.Text

            ClearFields()
            dgWeeklyReport.SelectedIndex = -1
        End If
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        ExitPage()
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As RoutedEventArgs) Handles btnSubmit.Click
        If lstWeeklyReportsData.Count > 0 Then

            Dim weeklyReport As New List(Of WeeklyReport)
            Dim totalActualEffortWeek As Decimal

            Try
                For Each reports In lstWeeklyReportsData

                    Dim objReports As New WeeklyReport
                    objReports.WeekRangeID = selectedValue
                    objReports.EmpID = reports.EmpID
                    objReports.ProjectID = reports.ProjectID
                    objReports.Rework = reports.Rework
                    objReports.ReferenceID = reports.RefID
                    objReports.Subject = reports.Subject
                    objReports.Severity = reports.Severity
                    objReports.IncidentType = reports.IncidentType
                    objReports.Phase = reports.Phase
                    objReports.Status = reports.Status
                    objReports.DateCreated = reports.DateCreated
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

                    totalActualEffortWeek = totalActualEffortWeek + objReports.ActualEffortWk
                    weeklyReport.Add(objReports)
                Next

                If totalActualEffortWeek < 40 Then
                    MsgBox("Insufficient Actual Effort Hours [Total:" + totalActualEffortWeek.ToString + "]", MsgBoxStyle.Critical, "AIDE")
                Else
                    Dim result As Integer = MsgBox("Submit Weekly Report for the week " + cbDateRange.Text + "?", MsgBoxStyle.YesNo, "AIDE")

                    If result = vbYes Then
                        AideServiceClient.CreateWeeklyReport(weeklyReport.ToArray)
                        MsgBox("Weekly Report Successfully Created!", MsgBoxStyle.Information)
                        ExitPage()
                    End If
                End If

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If
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

    Private Sub txtRefID_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtRefID.TextChanged
        If txtRefID.Text IsNot String.Empty Then
            If Not txtIncidentType.Text.Contains("*") Then
                txtIncidentType.Text = "Incident Type *"
            End If

            If Not txtTargetDate.Text.Contains("*") Then
                txtTargetDate.Text = "Target Date *"
            End If

            If Not txtEffort.Text.Contains("*") Then
                txtEffort.Text = "Estimate Effort(hrs) *"
            End If
        Else
            txtIncidentType.Text = "Incident Type"
            txtTargetDate.Text = "Target Date"
            txtEffort.Text = "Estimate Effort(hrs)"
        End If
    End Sub

    Private Sub dpCompletedDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dpCompletedDate.SelectedDateChanged
        If dpCompletedDate.Text IsNot String.Empty Then
            If Not txtStartDate.Text.Contains("*") Then
                txtStartDate.Text = "Select Start Date *"
            End If
        Else
            txtStartDate.Text = "Select Start Date"
        End If
    End Sub

    Private Sub cbIncidentType_DropDownClosed(sender As Object, e As EventArgs) Handles cbIncidentType.DropDownClosed
        If cbIncidentType.Text IsNot String.Empty Then
            If Not txtPhase.Text.Contains("*") Then
                txtPhase.Text = "Phase *"
            End If
        Else
            txtPhase.Text = "Phase"
        End If
    End Sub

    Private Sub cbStatus_DropDownClosed(sender As Object, e As EventArgs) Handles cbStatus.DropDownClosed
        If cbStatus.Text = "Completed" Then
            If Not txtDateCompleted.Text.Contains("*") Then
                txtDateCompleted.Text = "Select Completed Date *"
            End If
        Else
            txtDateCompleted.Text = "Select Completed Date"
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

    Private Function ValidateFields(isValidate As Boolean)
        If cbProject.SelectedIndex = -1 Then
            MsgBox("Please select a Project", MsgBoxStyle.Critical, "AIDE")
            cbProject.Focus()
            Return False
        End If

        If txtSubject.Text Is String.Empty Then
            MsgBox("Please enter a Description", MsgBoxStyle.Critical, "AIDE")
            txtSubject.Focus()
            Return False
        End If

        If txtRefID.Text IsNot String.Empty And cbIncidentType.SelectedIndex = -1 Then
            MsgBox("Please select Incident Type", MsgBoxStyle.Critical, "AIDE")
            cbIncidentType.Focus()
            Return False
        End If

        If Not cbIncidentType.SelectedIndex = -1 And cbPhase.SelectedIndex = -1 Then
            MsgBox("Please select a Phase", MsgBoxStyle.Critical, "AIDE")
            cbPhase.Focus()
            Return False
        End If

        If cbStatus.SelectedIndex = -1 Then
            MsgBox("Please select a Status", MsgBoxStyle.Critical, "AIDE")
            cbStatus.Focus()
            Return False
        End If

        If dpCompletedDate.Text IsNot String.Empty And dpStartDate.Text = String.Empty Then
            MsgBox("Please enter Date Started", MsgBoxStyle.Critical, "AIDE")
            dpStartDate.Focus()
            Return False
        End If

        If dpCompletedDate.Text IsNot String.Empty And txtRefID.Text IsNot String.Empty And dpTargetDate.Text = String.Empty Then
            MsgBox("Please enter Target Date", MsgBoxStyle.Critical, "AIDE")
            dpTargetDate.Focus()
            Return False
        End If

        If cbStatus.Text = "Completed" And dpCompletedDate.Text Is String.Empty Then
            MsgBox("Please enter Date Finished", MsgBoxStyle.Critical, "AIDE")
            dpCompletedDate.Focus()
            Return False
        End If

        If txtEffortEst.Text Is String.Empty And txtRefID.Text IsNot String.Empty Then
            MsgBox("Please enter Effort Estimate", MsgBoxStyle.Critical, "AIDE")
            txtEffortEst.Focus()
            Return False
        End If

        If txtActualEffort.Text Is String.Empty Then
            MsgBox("Please enter Actual Effort", MsgBoxStyle.Critical, "AIDE")
            txtActualEffort.Focus()
            Return False
        End If

        If txtActualEffortWk.Text Is String.Empty Then
            MsgBox("Please enter Actual Effort for the Week", MsgBoxStyle.Critical, "AIDE")
            txtActualEffortWk.Focus()
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
    End Sub

    Private Sub ExitPage()
        frame.Navigate(New WeeklyReportPage(frame, empID, email, addframe, menugrid, submenuframe))
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub

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
