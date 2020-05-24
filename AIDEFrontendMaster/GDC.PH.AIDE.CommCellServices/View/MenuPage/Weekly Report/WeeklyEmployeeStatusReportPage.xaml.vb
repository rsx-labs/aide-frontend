Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Text.RegularExpressions


<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class WeeklyEmployeeStatusReportPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    'Private AideServiceClient As ServiceReference1.AideServiceClient
    Private _ProjectViewModel As New ProjectViewModel
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private empID As Integer
    Private weekRangeID As Integer
    Private profile As Profile

    Dim selectedValue As Integer
    Dim incidentTypeID As Integer = 2
    Dim statusID As Integer = 3
    Dim phaseID As Integer = 4
    Dim reworkID As Integer = 5
    Dim severityID As Integer = 13

    Dim submitStatus As Integer = 3
    Dim workingStatus As Integer = 1

    Dim listProjects As New ObservableCollection(Of ProjectModel)
    Dim listReworkStatus As New ObservableCollection(Of ReworkStatusModel)
    Dim listSeverityStatus As New ObservableCollection(Of SeverityStatusModel)
    Dim listCategoryStatus As New ObservableCollection(Of CategoryStatusModel)
    Dim listPhaseStatus As New ObservableCollection(Of PhaseStatusModel)
    Dim listTaskStatus As New ObservableCollection(Of TaskStatusModel)

    Dim lstWeekRange As WeekRange()
    Dim lstWeeklyReportsData As ObservableCollection(Of WeeklyReportModel) = New ObservableCollection(Of WeeklyReportModel)

    Dim dateToday As Date = Date.Today
    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)
    Dim lastWeekSaturday As Date = saturday.AddDays(-7)

    Dim totalWeeklyHours As Decimal
#End Region

#Region "Provider Declaration"
    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
    Dim taskDBProvider As New TaskDBProvider
    Dim projectDBProvider As New ProjectDBProvider
#End Region

#Region "Model Declaration"
    Dim weeklyReportModel As New WeeklyReportModel
    Dim taskModel As New TasksModel
#End Region

#Region "View Model Declarations"
    Dim weeklyReportViewModel As New WeeklyReportViewModel
    Dim taskViewModel As New TasksViewModel
    Dim weekRangeViewModel As New WeekRangeViewModel
    Dim projectViewModel As New ProjectViewModel
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _profile As Profile, _empID As Integer, _weekRangeID As Integer, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        'InitializeService()
        ' Add any initialization after the InitializeComponent() call.
        Me.frame = _frame
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.empID = _empID
        Me.weekRangeID = _weekRangeID
        Me.profile = _profile

        LoadData()
        dgWeeklyReport.ItemsSource = lstWeeklyReportsData
        PopulateWeeklyReportData() ' Get data for the tasks that are not completed yet
    End Sub

    'Private Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim context As InstanceContext = New InstanceContext(Me)
    '        AideServiceClient = New AideServiceClient(context)
    '        AideServiceClient.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        AideServiceClient.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function
#End Region

#Region "Sub Procedures"

    Private Sub LoadData()
        ' Load Items For Projects
        Try
            Dim displayStatus As Integer = 1
            Dim lstProjects As Project() = AideClient.GetClient().GetProjectList(empID, displayStatus)

            For Each objProjects As Project In lstProjects
                projectDBProvider.setProjectList(objProjects)
            Next

            For Each myProjects As myProjectList In projectDBProvider.getProjectList()
                listProjects.Add(New ProjectModel(myProjects))
            Next

            projectViewModel.ProjectList = listProjects
        Catch ex As Exception
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Rework Combobox
        Try
            Dim lstStatus As StatusGroup() = AideClient.GetClient().GetStatusList(reworkID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyReworkStatusList(objStatus)
            Next

            For Each myStatus As MyReworkStatusList In taskDBProvider.GetReworkStatusList()
                listReworkStatus.Add(New ReworkStatusModel(myStatus))
            Next

            taskViewModel.ReworkStatusList = listReworkStatus
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Severity Combobox
        Try
            Dim lstStatus As StatusGroup() = AideClient.GetClient().GetStatusList(severityID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMySeverityStatusList(objStatus)
            Next

            For Each myStatus As MySeverityStatusList In taskDBProvider.GetSeverityStatusList()
                listSeverityStatus.Add(New SeverityStatusModel(myStatus))
            Next

            taskViewModel.SeverityStatusList = listSeverityStatus
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Incident Type Combobox
        Try
            Dim lstStatus As StatusGroup() = AideClient.GetClient().GetStatusList(incidentTypeID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyCategoryStatusList(objStatus)
            Next

            For Each myStatus As MyCategoryStatusList In taskDBProvider.GetCategoryStatusList()
                listCategoryStatus.Add(New CategoryStatusModel(myStatus))
            Next

            taskViewModel.CategoryStatusList = listCategoryStatus
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Phase Status Combobox
        Try
            Dim lstStatus As StatusGroup() = AideClient.GetClient().GetStatusList(phaseID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyPhaseStatusList(objStatus)
            Next

            For Each myStatus As MyPhaseStatusList In taskDBProvider.GetPhaseStatusList()
                listPhaseStatus.Add(New PhaseStatusModel(myStatus))
            Next

            taskViewModel.PhaseStatusList = listPhaseStatus
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Task Status Combobox
        Try
            Dim lstStatus As StatusGroup() = AideClient.GetClient().GetStatusList(statusID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyTaskStatusList(objStatus)
            Next

            For Each myStatus As MyTaskStatusList In taskDBProvider.GetTaskStatusList()
                listTaskStatus.Add(New TaskStatusModel(myStatus))
            Next

            taskViewModel.TaskStatusList = listTaskStatus
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub PopulateWeeklyReportData()
        Try
            'InitializeService()

            Dim lstWeeklyReport As WeeklyReport() = AideClient.GetClient().GetTasksDataByEmpID(weekRangeID, empID)

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
                                            .InboundContacts = weeklyReport.InboundContacts
                                         })
            Next

            If lstWeeklyReportsData.Count > 0 Then
                GetTotalHours()
            End If

        Catch ex As Exception
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub GetTotalHours()
        totalWeeklyHours = 0

        For Each reports In lstWeeklyReportsData
            totalWeeklyHours = totalWeeklyHours + reports.ActualEffortWk
        Next
    End Sub
#End Region

#Region "Events"
    Private Sub dgWeeklyReport_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgWeeklyReport.SelectionChanged
       
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        ExitPage()
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
        frame.Navigate(New WeeklyTeamStatusReportPage(frame, profile, addframe, menugrid, submenuframe, weekRangeID, 1, 1))
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
