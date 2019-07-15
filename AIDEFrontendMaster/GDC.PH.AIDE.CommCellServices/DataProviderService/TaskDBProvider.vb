Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class TaskDBProvider

    Private _taskStatusList As New ObservableCollection(Of MyTaskStatusList)
    Private _severityStatusList As New ObservableCollection(Of MySeverityStatusList)
    Private _categoryStatusList As New ObservableCollection(Of MyCategoryStatusList)
    Private _phaseStatusList As New ObservableCollection(Of MyPhaseStatusList)
    Private _reworkStatusList As New ObservableCollection(Of MyReworkStatusList)
    Private _taskList As New ObservableCollection(Of MyTasks)

    Private _tasksSpList As New ObservableCollection(Of MyTasksSp)

    Public Sub SetMyTaskStatusList(ByVal _status As StatusGroup)
        Dim status As New MyTaskStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _taskStatusList.Add(status)
    End Sub

    Public Sub SetMySeverityStatusList(ByVal _status As StatusGroup)
        Dim status As New MySeverityStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _severityStatusList.Add(status)
    End Sub

    Public Sub SetMyCategoryStatusList(ByVal _status As StatusGroup)
        Dim status As New MyCategoryStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _categoryStatusList.Add(status)
    End Sub

    Public Sub SetMyPhaseStatusList(ByVal _status As StatusGroup)
        Dim status As New MyPhaseStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _phaseStatusList.Add(status)
    End Sub

    Public Sub SetMyReworkStatusList(ByVal _status As StatusGroup)
        Dim status As New MyReworkStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _reworkStatusList.Add(status)
    End Sub

    Public Sub SetTasksSpList(ByVal _tasks As TaskSummary)
        Dim _objTask As MyTasksSp = New MyTasksSp With {.EmployeeName = _tasks.Name, _
                                                        .FriAt = _tasks.FridayAT, _
                                                        .FriCt = _tasks.FridayCT, _
                                                        .FriOt = _tasks.FridayOT, _
                                                        .LastWeekOutstanding = _tasks.OutstandingTaskLastWeek, _
                                                        .MonAt = _tasks.MondayAT, _
                                                        .MonCt = _tasks.MondayCT, _
                                                        .MonOt = _tasks.MondayOT, _
                                                        .ThuAt = _tasks.ThursdayAT, _
                                                        .ThuCt = _tasks.ThursdayCT, _
                                                        .ThuOt = _tasks.ThursdayOT, _
                                                        .TueAt = _tasks.TuesdayAT, _
                                                        .TueCt = _tasks.TuesdayCT, _
                                                        .TueOt = _tasks.TuesdayOT, _
                                                        .WedAt = _tasks.WednesdayAT, _
                                                        .WedCt = _tasks.WednesdayCT, _
                                                        .WedOt = _tasks.WednesdayOT}
        _tasksSpList.Add(_objTask)
    End Sub

    Public Sub SetTaskList(ByVal _task As Tasks)
        Dim _objTask As MyTasks = New MyTasks With {.TaskId = _task.TaskID,
                                                    .ProjId = _task.ProjectID,
                                                    .ProjectCode = _task.ProjectCode,
                                                    .Rework = _task.Rework,
                                                    .ReferenceID = _task.ReferenceID,
                                                    .IncDescr = _task.IncidentDescr,
                                                    .Severity = _task.Severity,
                                                    .IncidentType = _task.IncidentType,
                                                    .EmpId = _task.EmpID,
                                                    .Phase = _task.Phase,
                                                    .Status = _task.Status,
                                                    .DateStarted = _task.DateStarted,
                                                    .TargetDate = _task.TargetDate,
                                                    .CompltdDate = _task.CompletedDate,
                                                    .DateCreated = _task.DateCreated,
                                                    .EffortEst = _task.EffortEst,
                                                    .ActEffort = _task.ActualEffort,
                                                    .ActEffortWk = _task.ActualEffortWk,
                                                    .Comments = _task.Comments,
                                                    .Others1 = _task.Others1,
                                                    .Others2 = _task.Others2,
                                                    .Others3 = _task.Others3
                                                    }
        _taskList.Add(_objTask)
    End Sub

    Public Function GetTaskStatusList() As ObservableCollection(Of MyTaskStatusList)
        Return _taskStatusList
    End Function

    Public Function GetSeverityStatusList() As ObservableCollection(Of MySeverityStatusList)
        Return _severityStatusList
    End Function

    Public Function GetCategoryStatusList() As ObservableCollection(Of MyCategoryStatusList)
        Return _categoryStatusList
    End Function

    Public Function GetPhaseStatusList() As ObservableCollection(Of MyPhaseStatusList)
        Return _phaseStatusList
    End Function

    Public Function GetReworkStatusList() As ObservableCollection(Of MyReworkStatusList)
        Return _reworkStatusList
    End Function

    Public Function GetTasksSp() As ObservableCollection(Of MyTasksSp)
        Return _tasksSpList
    End Function

    Public Function GetTaskList() As ObservableCollection(Of MyTasks)
        Return _taskList
    End Function

End Class

Public Class MyTasks
    Public Property TaskId As Integer
    Public Property ProjId As Integer
    Public Property ProjectCode As Integer
    Public Property Rework As Short
    Public Property ReferenceID As String
    Public Property IncDescr As String
    Public Property Severity As Short
    Public Property IncidentType As Short
    Public Property EmpId As Integer
    Public Property Phase As Short
    Public Property Status As Short
    Public Property DateStarted As Date
    Public Property TargetDate As Date
    Public Property CompltdDate As Date
    Public Property DateCreated As Date
    Public Property EffortEst As Double
    Public Property ActEffort As Double
    Public Property ActEffortWk As Double
    Public Property Comments As String
    Public Property Others1 As String
    Public Property Others2 As String
    Public Property Others3 As String
End Class

Public Class MyTasksSp

    Public Property EmpID As Integer
    Public Property EmployeeName As String
    Public Property LastWeekOutstanding As Integer

    Public Property MonAt As Integer
    Public Property MonCt As Integer
    Public Property MonOt As Integer

    Public Property TueAt As Integer
    Public Property TueCt As Integer
    Public Property TueOt As Integer

    Public Property WedAt As Integer
    Public Property WedCt As Integer
    Public Property WedOt As Integer

    Public Property ThuAt As Integer
    Public Property ThuCt As Integer
    Public Property ThuOt As Integer

    Public Property FriAt As Integer
    Public Property FriCt As Integer
    Public Property FriOt As Integer

End Class

Public Class MyTaskStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MyCategoryStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MyPhaseStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MyReworkStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MySeverityStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

