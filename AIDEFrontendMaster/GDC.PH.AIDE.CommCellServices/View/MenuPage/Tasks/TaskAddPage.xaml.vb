Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class TaskAddPage
    Implements IAideServiceCallback

#Region "Fields"
    Public frame As Frame
    Public mainWindow As MainWindow
    Public email As String
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private empID As Integer
    Private ProjectName As String
    Private ProjectID As Integer

    Dim incidentTypeID As Integer = 2
    Dim statusID As Integer = 3
    Dim phaseID As Integer = 4
    Dim reworkID As Integer = 5
    Dim severityID As Integer = 13

    Dim tasks As New Tasks
    Dim client As AideServiceClient

    Dim listProjects As New ObservableCollection(Of ProjectModel)
    Dim listReworkStatus As New ObservableCollection(Of ReworkStatusModel)
    Dim listSeverityStatus As New ObservableCollection(Of SeverityStatusModel)
    Dim listCategoryStatus As New ObservableCollection(Of CategoryStatusModel)
    Dim listPhaseStatus As New ObservableCollection(Of PhaseStatusModel)
    Dim listTaskStatus As New ObservableCollection(Of TaskStatusModel)
#End Region

#Region "Provider Declaration"
    Dim taskDBProvider As New TaskDBProvider
    Dim projectDBProvider As New ProjectDBProvider
#End Region

#Region "View Model Declarations"
    Dim tasksViewModel As New TasksViewModel
    Dim projectViewModel As New ProjectViewModel()
#End Region

#Region "Model Declaration"
    Dim tasksModel As New TasksModel
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _empID As Integer)
        InitializeComponent()
        frame = _frame
        email = _email
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.empID = _empID
        mainWindow = _mainWindow
        btnUpdate.Visibility = Windows.Visibility.Collapsed
        dpStartDate.DisplayDate = Date.Now
        LoadData()
    End Sub

    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _taskList As TasksModel, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _empID As Integer)
        InitializeComponent()
        frame = _frame
        mainWindow = _mainWindow
        email = _email
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.empID = _empID
        btnCreate.Visibility = Windows.Visibility.Collapsed
        ProjectID = _taskList.ProjId
        LoadData()
        tasksViewModel.NewTasks = _taskList
        LoadControlsForUpdate()
        txtTitle.Text = "Update Task"
    End Sub
#End Region

#Region "Sub Methods"

    Private Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize

    End Function

    Private Sub LoadControlsForUpdate()
        btnCreate.Visibility = Windows.Visibility.Collapsed
        cboProject.Text = ProjectName
        cbCategory.SelectedValue = tasksViewModel.NewTasks.IncidentType
        cbPhase.SelectedValue = tasksViewModel.NewTasks.Phase
        cbRework.SelectedValue = tasksViewModel.NewTasks.Rework
        cbSeverity.SelectedValue = tasksViewModel.NewTasks.Severity
        cbStatus.SelectedValue = tasksViewModel.NewTasks.Status
        ''Project.Visibility = Windows.Visibility.Hidden
        'cboProject.IsEnabled = True
        ''Rework.Visibility = Windows.Visibility.Hidden
        'cbRework.IsEnabled = T
        ''Remarks.Visibility = Windows.Visibility.Hidden
        ''txtRemarks.IsEnabled = False

        ''Category.Visibility = Windows.Visibility.Hidden
        'cbCategory.IsEnabled = False
    End Sub

    Private Sub LoadData()
        Me.InitializeService()
        CreateTaskID()
        SetEmployeeID()

        ' Load Items For Category Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(incidentTypeID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyCategoryStatusList(objStatus)
            Next

            For Each myStatus As MyCategoryStatusList In taskDBProvider.GetCategoryStatusList()
                listCategoryStatus.Add(New CategoryStatusModel(myStatus))
            Next

            tasksViewModel.CategoryStatusList = listCategoryStatus
        Catch ex As SystemException
            client.Abort()
        End Try

        ' Load Items For Severity Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(severityID)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMySeverityStatusList(objStatus)
            Next

            For Each myStatus As MySeverityStatusList In taskDBProvider.GetSeverityStatusList()
                listSeverityStatus.Add(New SeverityStatusModel(myStatus))
            Next

            tasksViewModel.SeverityStatusList = listSeverityStatus
        Catch ex As SystemException
            client.Abort()
        End Try

        ' Load Items For Task Status Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(statusID)
            Dim listTaskStatus As New ObservableCollection(Of TaskStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyTaskStatusList(objStatus)
            Next

            For Each myStatus As MyTaskStatusList In taskDBProvider.GetTaskStatusList()
                listTaskStatus.Add(New TaskStatusModel(myStatus))
            Next

            tasksViewModel.TaskStatusList = listTaskStatus
        Catch ex As SystemException
            client.Abort()
        End Try

        ' Load Items For Phase Status Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(phaseID)
            Dim listPhaseStatus As New ObservableCollection(Of PhaseStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyPhaseStatusList(objStatus)
            Next

            For Each myStatus As MyPhaseStatusList In taskDBProvider.GetPhaseStatusList()
                listPhaseStatus.Add(New PhaseStatusModel(myStatus))
            Next

            tasksViewModel.PhaseStatusList = listPhaseStatus
        Catch ex As SystemException
            client.Abort()
        End Try

        ' Load Items For Rework Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(reworkID)
            Dim listReworkStatus As New ObservableCollection(Of ReworkStatusModel)

            For Each objStatus As StatusGroup In lstStatus
                taskDBProvider.SetMyReworkStatusList(objStatus)
            Next

            For Each myStatus As MyReworkStatusList In taskDBProvider.GetReworkStatusList()
                listReworkStatus.Add(New ReworkStatusModel(myStatus))
            Next

            tasksViewModel.ReworkStatusList = listReworkStatus
        Catch ex As SystemException
            client.Abort()
        End Try

        tasksViewModel.NewTasks = tasksModel
        Me.DataContext = tasksViewModel

        ' Load Items For Projects
        Try
            Dim lstProjects As Project() = client.GetProjectList(empID)

            For Each objProjects As Project In lstProjects
                projectDBProvider.setProjectList(objProjects)
            Next

            For Each myProjects As myProjectList In projectDBProvider.getProjectList()
                If ProjectID = myProjects.Project_ID Then
                    ProjectName = myProjects.Project_Name
                End If
                listProjects.Add(New ProjectModel(myProjects))
            Next

            projectViewModel.ProjectList = listProjects
            cboProject.DataContext = projectViewModel
        Catch ex As Exception
            client.Abort()
        End Try

    End Sub

    Private Sub CreateTaskID()
        Try
            If Me.InitializeService() Then
                Dim lstTasks As Tasks() = client.GetAllTasks()
                Dim totalCount As Integer

                totalCount = lstTasks.Length + 1
                tasksModel.TaskId = totalCount
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetEmployeeID()
        tasksModel.EmpId = mainWindow.EmployeeID
    End Sub

    Private Function GetDataContext(obj As TasksViewModel) As Boolean
        Try
            tasks.TaskID = obj.NewTasks.TaskId

            If IsNothing(cboProject.SelectedValue) Then
                tasks.ProjectID = obj.NewTasks.ProjId
                tasks.ProjectCode = obj.NewTasks.ProjId
            Else
                tasks.ProjectID = cboProject.SelectedValue
                tasks.ProjectCode = cboProject.SelectedValue
            End If

            tasks.Rework = obj.NewTasks.Rework
            tasks.ReferenceID = obj.NewTasks.ReferenceID
            tasks.IncidentDescr = obj.NewTasks.IncDescr
            tasks.Severity = obj.NewTasks.Severity
            tasks.IncidentType = obj.NewTasks.IncidentType
            tasks.EmpID = obj.NewTasks.EmpId
            tasks.Phase = obj.NewTasks.Phase
            tasks.Status = obj.NewTasks.Status

            If Not obj.NewTasks.DateStarted.Equals(String.Empty) Then
                tasks.DateStarted = obj.NewTasks.DateStarted
            End If

            If Not obj.NewTasks.TargetDate.Equals(String.Empty) Then
                tasks.TargetDate = obj.NewTasks.TargetDate
            End If

            If Not obj.NewTasks.CompltdDate.Equals(String.Empty) Then
                tasks.CompletedDate = obj.NewTasks.CompltdDate
            End If

            'tasks.DateCreated = Date.Now.ToShortDateString
            If Not obj.NewTasks.DateCreated.Equals(Nothing) Then
                tasks.DateCreated = obj.NewTasks.DateCreated
            Else
                tasks.DateCreated = Date.Now.ToShortDateString
            End If

            If Not obj.NewTasks.EffortEst.Equals(String.Empty) Then
                tasks.EffortEst = obj.NewTasks.EffortEst
            End If

            If Not obj.NewTasks.ActEffortWk.Equals(String.Empty) Then
                tasks.ActualEffortWk = obj.NewTasks.ActEffortWk
            End If

            If Not obj.NewTasks.ActEffort.Equals(String.Empty) Then
                tasks.ActualEffort = obj.NewTasks.ActEffort
            End If

            tasks.Comments = obj.NewTasks.Comments
            tasks.Others1 = obj.NewTasks.Others1
            tasks.Others2 = obj.NewTasks.Others2
            tasks.Others3 = obj.NewTasks.Others3
            Return True
        Catch ex As Exception
            MsgBox("Invalid Input!", MsgBoxStyle.Critical, "AIDE")
            Return False
        End Try
    End Function

    Private Sub ClearValues()
        tasksModel.ReferenceID = Nothing

        tasksModel.EffortEst = Nothing
        tasksModel.ActEffort = Nothing
        tasksModel.ActEffortWk = Nothing

        tasksModel.DateStarted = Nothing
        tasksModel.TargetDate = Nothing
        tasksModel.CompltdDate = Nothing

        tasksModel.IncDescr = Nothing
        tasksModel.Comments = Nothing

        tasksModel.Status = Nothing
        tasksModel.IncidentType = Nothing
        tasksModel.Phase = Nothing
        tasksModel.Severity = Nothing
        tasksModel.Rework = Nothing

        cboProject.SelectedIndex = -1
    End Sub

    Private Function FindMissingFields(obj As TasksViewModel) As Boolean
        If txtRefID.Text = String.Empty Or
           txtIncDescr.Text = String.Empty Or
           cbPhase.Text = String.Empty Or
           cbCategory.Text = String.Empty Or
            cboProject.Text = String.Empty Or
            cbStatus.Text = String.Empty Then
            MsgBox("Please Fill All Required Fields", MsgBoxStyle.Exclamation, "FAILED")
            Return True
        End If
        Return False
    End Function

    'Private Sub dpStartDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dpStartDate.SelectedDateChanged
    '    If dpStartDate.SelectedDate <> String.Empty Then
    '        dpTargetDate.DisplayDateStart = tasksViewModel.NewTasks.DateStarted
    '        dpCompltdDate.DisplayDateStart = tasksViewModel.NewTasks.DateStarted
    '    End If
    'End Sub

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
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "Button/Events"

    Private Sub cbStatus_DropDownClosed(sender As Object, e As EventArgs) Handles cbStatus.DropDownClosed
        If cbStatus.Text = "Completed" Or cbStatus.Text = "Returned to Triage" Then
            dpTargetDate.SelectedDate = Date.Now
            dpCompltdDate.SelectedDate = Date.Now
        Else
            dpCompltdDate.Text = String.Empty
            dpTargetDate.Text = String.Empty
        End If
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        If txtTitle.Text = "Update Task" Then
            frame.Navigate(New TaskListPage(frame, mainWindow, empID, email, addframe, menugrid, submenuframe))
            ExitPage()
        Else
            frame.Navigate(New TaskAdminPage(frame, mainWindow, empID, email, addframe, menugrid, submenuframe))
            ExitPage()
        End If
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        If Not FindMissingFields(Me.DataContext) Then
            Try
                Dim result As Integer = MsgBox("Are you sure you want to add task?", MsgBoxStyle.YesNo, "AIDE")

                If result = vbYes Then
                    If InitializeService() Then
                        If GetDataContext(Me.DataContext) Then
                            client.CreateTask(tasks)
                            MsgBox("Successfully Created Task", MsgBoxStyle.Information, "AIDE")
                            ClearValues()
                            frame.Navigate(New TaskAdminPage(frame, mainWindow, empID, email, addframe, menugrid, submenuframe))
                            ExitPage()
                        End If
                    End If
                End If
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
            End Try
        End If
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        Try
            Dim result As Integer = MsgBox("Are you sure you want to Update?", MsgBoxStyle.YesNo, "AIDE")
            If result = vbYes Then
                If InitializeService() Then
                    If GetDataContext(Me.DataContext) Then
                        client.UpdateTask(tasks)
                        MsgBox("Successfully Updated", MsgBoxStyle.Information, "AIDE")
                        ClearValues()
                        frame.Navigate(New TaskListPage(frame, mainWindow, empID, email, addframe, menugrid, submenuframe))
                        ExitPage()
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub dpTargetDate_CalendarOpened(sender As Object, e As RoutedEventArgs) Handles dpTargetDate.CalendarOpened
        dpTargetDate.DisplayDateStart = dpStartDate.SelectedDate
    End Sub

#End Region

#Region "ICallBack Function"
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
