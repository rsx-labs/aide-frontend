Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class TaskDBProvider

    Private _taskStatusList As New ObservableCollection(Of MyTaskStatusList)
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
        Dim rawStatusDesc As String = SetStatusDesc(_task.Status)
        Dim rawPhaseDesc As String = SetPhaseDesc(_task.Phase)

        Dim _objTask As MyTasks = New MyTasks With {.ActEffortEst = _task.ActualEffortEst, _
                                                     .ActEffortEstWk = _task.EffortEstWk, _
                                                     .CompltdDate = _task.CompletedDate, _
                                                     .DateCreated = _task.DateCreated, _
                                                     .DateStarted = _task.DateStarted, _
                                                     .EffortEst = _task.EffortEst, _
                                                     .EmpId = _task.EmpID, _
                                                     .HoursWorked_Date = _task.HoursWorked_Date, _
                                                     .IncDescr = _task.IncidentDescr, _
                                                     .IncId = _task.IncidentID, _
                                                     .Others1 = _task.Others1, _
                                                     .Others2 = _task.Others2, _
                                                     .Others3 = _task.Others3, _
                                                     .Phase = rawPhaseDesc, _
                                                     .ProjectCode = _task.ProjectCode, _
                                                     .ProjId = _task.ProjectID, _
                                                     .Remarks = _task.Remarks, _
                                                     .Rework = _task.Rework, _
                                                     .Status = rawStatusDesc, _
                                                     .TargetDate = _task.TargetDate, _
                                                     .TaskDescr = _task.TaskDescr, _
                                                     .TaskId = _task.TaskID, _
                                                     .TaskType = _task.TaskType}
        _taskList.Add(_objTask)
    End Sub

    Public Function GetTaskStatusList() As ObservableCollection(Of MyTaskStatusList)
        Return _taskStatusList
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

    Public Function SetStatusDesc(_status As Object) As String

        Select Case _status
            Case "1"
                Return "Not Yet Started"
            Case "2"
                Return "In Progress"
            Case "3"
                Return "Completed"
            Case "4"
                Return "Returned to Triage"
            Case "5"
                Return "Waiting for Info"
            Case "6"
                Return "On-hold"
            Case "Not Yet Started"
                Return 1
            Case "In Progress"
                Return 2
            Case "Completed"
                Return 3
            Case "Returned to Triage"
                Return 4
            Case "Waiting for Info"
                Return 5
            Case "On-hold"
                Return 6
        End Select
    End Function

    Public Function SetPhaseDesc(_phase As Object) As String

        Select Case _phase
            Case "1"
                Return "Coding"
            Case "2"
                Return "Setup/Install"
            Case "3"
                Return "Training"
            Case "4"
                Return "Investigation"
            Case "Coding"
                Return 1
            Case "Setup/Install"
                Return 2
            Case "Training"
                Return 3
            Case "Investigation"
                Return 4
        End Select
    End Function
End Class

Public Class MyTasks
    Public Property TaskId As Integer
    Public Property EmpId As Integer
    Public Property IncId As String
    Public Property TaskType As Short
    Public Property ProjId As Integer
    Public Property IncDescr As String
    Public Property TaskDescr As String
    Public Property DateStarted As Date
    Public Property TargetDate As Date
    Public Property CompltdDate As Date
    Public Property DateCreated As Date
    Public Property Status As String
    Public Property Remarks As String
    Public Property EffortEst As Double
    Public Property ActEffortEst As Double
    Public Property ActEffortEstWk As Double
    Public Property ProjectCode As Integer
    Public Property Rework As Short
    Public Property Phase As String
    Public Property HoursWorked_Date As String
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


