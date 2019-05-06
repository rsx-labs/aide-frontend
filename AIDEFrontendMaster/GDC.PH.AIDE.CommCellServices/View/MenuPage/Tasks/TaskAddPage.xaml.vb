Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class TaskAddPage
    Implements IAideServiceCallback

    Public frame As Frame
    Public mainWindow As MainWindow
    Public email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    'Public taskList As New TasksModel

#Region "Provider Declaration"
    Dim taskDBProvider As New TaskDBProvider
    Dim projectDBProvider As New ProjectDBProvider
#End Region

#Region "View Model Declarations"
    Dim tasksViewModel As New TasksViewModel
    Dim projectViewModel As New ProjectViewModel
#End Region

#Region "Model Declaration"
    Dim tasksModel As New TasksModel
#End Region

    Dim tasks As New Tasks
    Dim client As AideServiceClient

    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        frame = _frame
        email = _email
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        mainWindow = _mainWindow
        btnUpdate.Visibility = Windows.Visibility.Collapsed
        dpStartDate.DisplayDate = Date.Now
        LoadData()
    End Sub

    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _taskList As TasksModel, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        frame = _frame
        mainWindow = _mainWindow
        email = _email
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        btnCreate.Visibility = Windows.Visibility.Collapsed
        LoadData()
        tasksViewModel.NewTasks = _taskList
        Me.DataContext = tasksViewModel
        GetDataContext(Me.DataContext)
        LoadControlsForUpdate()
    End Sub

#Region "Common Methods"
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

#Region "Sub Methods"

    Private Sub LoadControlsForUpdate()
        txtIncID.IsReadOnly = True
        dpStartDate.SelectedDate = tasks.DateStarted
        btnCreate.Visibility = Windows.Visibility.Collapsed
        Project.Visibility = Windows.Visibility.Hidden
        cboProject.Visibility = Windows.Visibility.Hidden
        Rework.Visibility = Windows.Visibility.Hidden
        cbRework.Visibility = Windows.Visibility.Hidden
        Remarks.Visibility = Windows.Visibility.Hidden
        txtRemarks.Visibility = Windows.Visibility.Hidden
        Category.Visibility = Windows.Visibility.Hidden
        cbCategory.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub LoadData()
        Me.InitializeService()
        CreateTaskID()
        SetEmployeeID()

        ' Load Items For Category Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(2)
            Dim listCategoryStatus As New ObservableCollection(Of CategoryStatusModel)

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

        ' Load Items For Task Status Combobox
        Try
            Dim lstStatus As StatusGroup() = client.GetStatusList(3)
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
            Dim lstStatus As StatusGroup() = client.GetStatusList(4)
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
            Dim lstStatus As StatusGroup() = client.GetStatusList(5)
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
            Dim lstProjects As Project() = client.GetProjectList()
            Dim listProjects As New ObservableCollection(Of ProjectModel)

            For Each objProjects As Project In lstProjects
                projectDBProvider.SetProjectList(objProjects)
            Next

            For Each myProjects As MyProjectList In projectDBProvider.getProjectList()
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
                tasks.EmpID = obj.NewTasks.EmpId
                tasks.IncidentID = obj.NewTasks.IncId

                'tasks.DateCreated = Date.Now.ToShortDateString
                If Not obj.NewTasks.DateCreated.Equals(Nothing) Then
                    tasks.DateCreated = obj.NewTasks.DateCreated
                Else
                    tasks.DateCreated = Date.Now.ToShortDateString
                End If

                If Not obj.NewTasks.TargetDate.Equals(String.Empty) Then
                    tasks.TargetDate = obj.NewTasks.TargetDate
                End If

                If Not obj.NewTasks.CompltdDate.Equals(String.Empty) Then
                    tasks.CompletedDate = obj.NewTasks.CompltdDate
                End If

                'tasks.DateCreated = Date.Now.ToShortDateString
                If Not obj.NewTasks.DateCreated.Equals(String.Empty) Then
                    tasks.DateStarted = obj.NewTasks.DateCreated
                Else
                    tasks.DateCreated = Date.Now.ToShortDateString
                End If
                tasks.Status = obj.NewTasks.Status

                If Not obj.NewTasks.EffortEst.Equals(String.Empty) Then
                    tasks.EffortEst = obj.NewTasks.EffortEst
                End If

                If Not obj.NewTasks.ActEffortEstWk.Equals(String.Empty) Then
                    tasks.EffortEstWk = obj.NewTasks.ActEffortEstWk
                End If

                If Not obj.NewTasks.ActEffortEst.Equals(String.Empty) Then
                    tasks.ActualEffortEst = obj.NewTasks.ActEffortEst
                End If

                tasks.TaskType = obj.NewTasks.TaskType
                tasks.ProjectID = cboProject.SelectedValue
                tasks.ProjectCode = cboProject.SelectedValue
                tasks.Rework = obj.NewTasks.Rework
                tasks.Phase = obj.NewTasks.Phase
                tasks.TaskDescr = obj.NewTasks.TaskDescr
                tasks.IncidentDescr = obj.NewTasks.IncDescr
                tasks.Remarks = obj.NewTasks.Remarks
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
        tasksModel.IncId = Nothing

        tasksModel.EffortEst = Nothing
        tasksModel.ActEffortEst = Nothing
        tasksModel.ActEffortEstWk = Nothing

        tasksModel.DateStarted = Nothing
        tasksModel.TargetDate = Nothing
        tasksModel.CompltdDate = Nothing

        tasksModel.IncDescr = Nothing
        tasksModel.Remarks = Nothing
        tasksModel.TaskDescr = Nothing

        tasksModel.Status = Nothing
        tasksModel.TaskType = Nothing
        tasksModel.Phase = Nothing

        cboProject.SelectedIndex = -1
    End Sub

    Private Function FindMissingFields(obj As TasksViewModel) As Boolean
        If txtIncID.Text = String.Empty And
           txtIncDescr.Text = String.Empty And
           cbPhase.Text = String.Empty And
           cbCategory.Text = String.Empty And
            cboProject.Text = String.Empty And
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
        frame.Navigate(New TaskAdminPage(frame, mainWindow, txtEmpID.Text, email, _addframe, _menugrid, _submenuframe))
        frame.IsEnabled = True
        frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        If Not FindMissingFields(Me.DataContext) Then
            Try
                Dim result As Integer = MsgBox("Are you sure you want to add task?", MsgBoxStyle.YesNo, "AIDE")

                If result = vbYes Then
                    If Me.InitializeService Then
                        If GetDataContext(Me.DataContext) Then
                            client.CreateTask(tasks)
                            MsgBox("Successfully Created Task", MsgBoxStyle.Information, "AIDE")
                            ClearValues()
                            frame.Navigate(New TaskAdminPage(frame, mainWindow, txtEmpID.Text, email, _addframe, _menugrid, _submenuframe))
                            frame.IsEnabled = True
                            frame.Opacity = 1
                            _menugrid.IsEnabled = True
                            _menugrid.Opacity = 1
                            _submenuframe.IsEnabled = True
                            _submenuframe.Opacity = 1
                            _addframe.Visibility = Visibility.Hidden
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
            If cbStatus.Text = "Completed" And txtEffortEst.Text = String.Empty And
                txtActualEffortEst.Text = String.Empty And txtActualEfforEstWk.Text = String.Empty Then
                MsgBox("Please Fill Effort Estimate, Actual Effort, and Weekly Effort Fields", MsgBoxStyle.Exclamation, "FAILED")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to Update?", MsgBoxStyle.YesNo, "AIDE")
                If result = vbYes Then
                    If Me.InitializeService Then
                        If GetDataContext(Me.DataContext) Then
                            client.UpdateTask(tasks)
                            MsgBox("Successfully Updated", MsgBoxStyle.Information, "AIDE")
                            ClearValues()
                            frame.Navigate(New TaskAdminPage(frame, mainWindow, txtEmpID.Text, email, _addframe, _menugrid, _submenuframe))
                            frame.IsEnabled = True
                            frame.Opacity = 1
                            _menugrid.IsEnabled = True
                            _menugrid.Opacity = 1
                            _submenuframe.IsEnabled = True
                            _submenuframe.Opacity = 1
                            _addframe.Visibility = Visibility.Hidden
                        End If
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

    Private Sub dpCompltdDate_CalendarOpened(sender As Object, e As RoutedEventArgs) Handles dpCompltdDate.CalendarOpened
        dpCompltdDate.DisplayDateStart = dpTargetDate.SelectedDate
    End Sub

#End Region

End Class
