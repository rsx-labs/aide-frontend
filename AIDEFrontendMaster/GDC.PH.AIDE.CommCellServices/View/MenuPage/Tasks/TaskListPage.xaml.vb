Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class TaskListPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private aideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private mainWindow As MainWindow
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private isEmpty As Boolean
    Public email As String
    Public empID As Integer

    Dim incidentTypeID As Integer = 2
    Dim statusID As Integer = 3
    Dim phaseID As Integer = 4
    Dim reworkID As Integer = 5
    Dim severityID As Integer = 13

    Dim listProjects As New ObservableCollection(Of ProjectModel)
    Dim listReworkStatus As New ObservableCollection(Of ReworkStatusModel)
    Dim listSeverityStatus As New ObservableCollection(Of SeverityStatusModel)
    Dim listCategoryStatus As New ObservableCollection(Of CategoryStatusModel)
    Dim listPhaseStatus As New ObservableCollection(Of PhaseStatusModel)
    Dim listTaskStatus As New ObservableCollection(Of TaskStatusModel)

    Dim lstTask As Tasks()
    Dim lstTasksData As PaginatedObservableCollection(Of TasksModel) = New PaginatedObservableCollection(Of TasksModel)(pagingRecordPerPage)
#End Region

#Region "Provider Declaration"
    Dim tasksDBProvider As New TaskDBProvider
    Dim projectDBProvider As New ProjectDBProvider
#End Region

#Region "Model Declaration"
    Dim tasksModel As New WeeklyReportModel
#End Region

#Region "View Model Declarations"
    Dim tasksViewModel As New TasksViewModel
    Dim projectViewModel As New ProjectViewModel
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _empID As Integer, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        Me.empID = _empID
        Me.mainFrame = _frame
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.mainWindow = _mainWindow
        Me.email = _email
        LoadDescriptionData()
        SetData()
    End Sub
#End Region

#Region "Sub Procedures"
    Private Sub LoadDescriptionData()
        InitializeService()

        ' Load Items For Projects
        Try
            Dim displayStatus As Integer = 0
            Dim lstProjects As Project() = aideService.GetProjectList(empID, displayStatus)

            For Each objProjects As Project In lstProjects
                projectDBProvider.setProjectList(objProjects)
            Next

            For Each myProjects As myProjectList In projectDBProvider.getProjectList()
                listProjects.Add(New ProjectModel(myProjects))
            Next

            projectViewModel.ProjectList = listProjects
        Catch ex As Exception
            aideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Rework 
        Try
            Dim lstStatus As StatusGroup() = aideService.GetStatusList(reworkID)

            For Each objStatus As StatusGroup In lstStatus
                tasksDBProvider.SetMyReworkStatusList(objStatus)
            Next

            For Each myStatus As MyReworkStatusList In tasksDBProvider.GetReworkStatusList()
                listReworkStatus.Add(New ReworkStatusModel(myStatus))
            Next

            tasksViewModel.ReworkStatusList = listReworkStatus
        Catch ex As SystemException
            aideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Severity 
        Try
            Dim lstStatus As StatusGroup() = aideService.GetStatusList(severityID)

            For Each objStatus As StatusGroup In lstStatus
                tasksDBProvider.SetMySeverityStatusList(objStatus)
            Next

            For Each myStatus As MySeverityStatusList In tasksDBProvider.GetSeverityStatusList()
                listSeverityStatus.Add(New SeverityStatusModel(myStatus))
            Next

            tasksViewModel.SeverityStatusList = listSeverityStatus
        Catch ex As SystemException
            aideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Incident Type 
        Try
            Dim lstStatus As StatusGroup() = aideService.GetStatusList(incidentTypeID)

            For Each objStatus As StatusGroup In lstStatus
                tasksDBProvider.SetMyCategoryStatusList(objStatus)
            Next

            For Each myStatus As MyCategoryStatusList In tasksDBProvider.GetCategoryStatusList()
                listCategoryStatus.Add(New CategoryStatusModel(myStatus))
            Next

            tasksViewModel.CategoryStatusList = listCategoryStatus
        Catch ex As SystemException
            aideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Phase Status 
        Try
            Dim lstStatus As StatusGroup() = aideService.GetStatusList(phaseID)

            For Each objStatus As StatusGroup In lstStatus
                tasksDBProvider.SetMyPhaseStatusList(objStatus)
            Next

            For Each myStatus As MyPhaseStatusList In tasksDBProvider.GetPhaseStatusList()
                listPhaseStatus.Add(New PhaseStatusModel(myStatus))
            Next

            tasksViewModel.PhaseStatusList = listPhaseStatus
        Catch ex As SystemException
            aideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        ' Load Items For Task Status
        Try
            Dim lstStatus As StatusGroup() = aideService.GetStatusList(statusID)

            For Each objStatus As StatusGroup In lstStatus
                tasksDBProvider.SetMyTaskStatusList(objStatus)
            Next

            For Each myStatus As MyTaskStatusList In tasksDBProvider.GetTaskStatusList()
                listTaskStatus.Add(New TaskStatusModel(myStatus))
            Next

            tasksViewModel.TaskStatusList = listTaskStatus
        Catch ex As SystemException
            aideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

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

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstTask = aideService.GetTasksByEmpID(empID)
                If lstTask.Count <> 0 Then
                    LoadData()
                    totalRecords = lstTask.Length
                    DisplayPagingInfo()
                Else
                    lv_taskList.Visibility = Windows.Visibility.Collapsed
                    lbl_noOT.Visibility = Windows.Visibility.Visible
                    taskborder.Visibility = Windows.Visibility.Hidden

                    btnNext.Visibility = Windows.Visibility.Collapsed
                    btnPrev.Visibility = Windows.Visibility.Collapsed
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstTaskList As New ObservableCollection(Of TasksModel)
            Dim tasksListDBProvider As New TaskDBProvider
            Dim taskListVM As New TasksViewModel()

            For Each objTasks As Tasks In lstTask
                tasksListDBProvider.SetTaskList(objTasks)
            Next

            For Each tasks As MyTasks In tasksListDBProvider.GetTaskList()
                lstTasksData.Add(New TasksModel With {.TaskId = tasks.TaskId,
                                                      .ProjId = tasks.ProjId,
                                                      .ProjectCode = tasks.ProjectCode,
                                                      .Rework = tasks.Rework,
                                                      .ReworkDesc = getReworkValue(tasks.Rework),
                                                      .ReferenceID = tasks.ReferenceID,
                                                      .IncDescr = tasks.IncDescr,
                                                      .Severity = tasks.Severity,
                                                      .SeverityDesc = getSeverityValue(tasks.Severity),
                                                      .IncidentType = tasks.IncidentType,
                                                      .IncidentDesc = getIncidentValue(tasks.IncidentType),
                                                      .EmpId = tasks.EmpId,
                                                      .Phase = tasks.Phase,
                                                      .PhaseDesc = getPhaseValue(tasks.Phase),
                                                      .Status = tasks.Status,
                                                      .StatusDesc = getStatusValue(tasks.Status),
                                                      .DateStarted = tasks.DateStarted,
                                                      .TargetDate = tasks.TargetDate,
                                                      .CompltdDate = tasks.CompltdDate,
                                                      .DateCreated = tasks.DateCreated,
                                                      .EffortEst = tasks.EffortEst,
                                                      .ActEffortWk = tasks.ActEffortWk,
                                                      .ActEffort = tasks.ActEffort,
                                                      .Comments = tasks.Comments
                                                    })
            Next

            currentPage = lstTasksData.CurrentPage + 1
            lastPage = Math.Ceiling(lstTask.Length / pagingRecordPerPage)
            lv_taskList.ItemsSource = lstTasksData

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
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
#End Region

#Region "Events"

    Private Sub lv_taskList_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lv_taskList.MouseDoubleClick
        e.Handled = True

        Dim tasksListDBProvider As New TaskDBProvider

        If lv_taskList.SelectedIndex <> -1 Then
            If lv_taskList.SelectedItem IsNot Nothing Then

                Dim taskList As New TasksModel

                taskList.TaskId = CType(lv_taskList.SelectedItem, TasksModel).TaskId
                taskList.ProjId = CType(lv_taskList.SelectedItem, TasksModel).ProjId
                taskList.ProjectCode = CType(lv_taskList.SelectedItem, TasksModel).ProjectCode
                taskList.Rework = CType(lv_taskList.SelectedItem, TasksModel).Rework
                taskList.ReferenceID = CType(lv_taskList.SelectedItem, TasksModel).ReferenceID
                taskList.IncDescr = CType(lv_taskList.SelectedItem, TasksModel).IncDescr
                taskList.Severity = CType(lv_taskList.SelectedItem, TasksModel).Severity
                taskList.IncidentType = CType(lv_taskList.SelectedItem, TasksModel).IncidentType
                taskList.EmpId = CType(lv_taskList.SelectedItem, TasksModel).EmpId
                taskList.Phase = CType(lv_taskList.SelectedItem, TasksModel).Phase
                taskList.Status = CType(lv_taskList.SelectedItem, TasksModel).Status
                taskList.DateStarted = CType(lv_taskList.SelectedItem, TasksModel).DateStarted
                taskList.TargetDate = CType(lv_taskList.SelectedItem, TasksModel).TargetDate
                taskList.CompltdDate = CType(lv_taskList.SelectedItem, TasksModel).CompltdDate
                taskList.DateCreated = CType(lv_taskList.SelectedItem, TasksModel).DateCreated
                taskList.EffortEst = CType(lv_taskList.SelectedItem, TasksModel).EffortEst
                taskList.ActEffort = CType(lv_taskList.SelectedItem, TasksModel).ActEffort
                taskList.ActEffortWk = CType(lv_taskList.SelectedItem, TasksModel).ActEffortWk
                taskList.Comments = CType(lv_taskList.SelectedItem, TasksModel).Comments

                addframe.Navigate(New TaskAddPage(mainFrame, mainWindow, taskList, email, addframe, menugrid, submenuframe, empID))
                addframe.Margin = New Thickness(100, 50, 100, 50)
                addframe.IsEnabled = True
                addframe.Visibility = Visibility.Visible
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
            End If
        End If
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        mainFrame.Navigate(New TaskAdminPage(mainFrame, mainWindow, empID, email, addframe, menugrid, submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        Dim totalRecords As Integer = lstTask.Length

        If totalRecords >= ((lstTasksData.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            lstTasksData.CurrentPage = lstTasksData.CurrentPage + 1
            currentPage = lstTasksData.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If

        DisplayPagingInfo()
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstTask.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
        lstTasksData.CurrentPage = lstTasksData.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub GUISettingsOff()
        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub
#End Region

#Region "ICallBack Function"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
