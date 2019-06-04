Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class WeeklyReportDBProvider

    Private _taskStatusList As New ObservableCollection(Of MyWTaskStatusList)
    Private _categoryStatusList As New ObservableCollection(Of MyWCategoryStatusList)
    Private _phaseStatusList As New ObservableCollection(Of MyWPhaseStatusList)
    Private _reworkStatusList As New ObservableCollection(Of MyWReworkStatusList)
    Private _severityStatusList As New ObservableCollection(Of MySeverityStatusList)
    Private _weeklyReportList As New ObservableCollection(Of MyWeeklyReport)

    Private _weekRangeList As New ObservableCollection(Of MyWeekRange)

    Public Sub SetMyTaskStatusList(ByVal _status As StatusGroup)
        Dim status As New MyWTaskStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _taskStatusList.Add(status)
    End Sub

    Public Sub SetMyCategoryStatusList(ByVal _status As StatusGroup)
        Dim status As New MyWCategoryStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _categoryStatusList.Add(status)
    End Sub

    Public Sub SetMyPhaseStatusList(ByVal _status As StatusGroup)
        Dim status As New MyWPhaseStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _phaseStatusList.Add(status)
    End Sub

    Public Sub SetMySeverityStatusList(ByVal _status As StatusGroup)
        Dim status As New MySeverityStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _severityStatusList.Add(status)
    End Sub

    Public Sub SetMyReworkStatusList(ByVal _status As StatusGroup)
        Dim status As New MyWReworkStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _reworkStatusList.Add(status)
    End Sub

    Public Sub SetWeekRangeList(ByVal weekRange As WeekRange)
        Dim objWeekRange As MyWeekRange = New MyWeekRange With {.WeekRangeID = weekRange.WeekRangeID,
                                                                .StartWeek = weekRange.StartWeek,
                                                                .EndWeek = weekRange.EndWeek,
                                                                .DateCreated = weekRange.DateCreated, 
                                                                .DateRange = weekRange.DateRange}
        _weekRangeList.Add(objWeekRange)
    End Sub

    Public Sub SetWeeklyReportList(ByVal _weeklyReport As WeeklyReport)
        Dim _objWeeklyReport As MyWeeklyReport = New MyWeeklyReport With {
                                                     .WeekID = _weeklyReport.WeekID,
                                                     .WeekRangeID = _weeklyReport.WeekRangeID,
                                                     .ProjectID = _weeklyReport.ProjectID,
                                                     .Rework = _weeklyReport.Rework,
                                                     .RefID = _weeklyReport.ReferenceID,
                                                     .Subject = _weeklyReport.Subject,
                                                     .Severity = _weeklyReport.Severity,
                                                     .IncType = _weeklyReport.IncidentType,
                                                     .EmpID = _weeklyReport.EmpID,
                                                     .Phase = _weeklyReport.Phase,
                                                     .Status = _weeklyReport.Status,
                                                     .DateStarted = _weeklyReport.DateStarted,
                                                     .DateTarget = _weeklyReport.DateTarget,
                                                     .DateFinished = _weeklyReport.DateFinished,
                                                     .DateCreated = _weeklyReport.DateCreated,
                                                     .EffortEst = _weeklyReport.EffortEst,
                                                     .ActEffort = _weeklyReport.ActualEffort,
                                                     .ActEffortWk = _weeklyReport.ActualEffortWk,
                                                     .Comment = _weeklyReport.Comments,
                                                     .InboundContacts = _weeklyReport.InboundContacts
                                                     }
        _weeklyReportList.Add(_objWeeklyReport)
    End Sub

    Public Function GetTaskStatusList() As ObservableCollection(Of MyWTaskStatusList)
        Return _taskStatusList
    End Function

    Public Function GetCategoryStatusList() As ObservableCollection(Of MyWCategoryStatusList)
        Return _categoryStatusList
    End Function

    Public Function GetPhaseStatusList() As ObservableCollection(Of MyWPhaseStatusList)
        Return _phaseStatusList
    End Function

    Public Function GetReworkStatusList() As ObservableCollection(Of MyWReworkStatusList)
        Return _reworkStatusList
    End Function

    Public Function GetSeverityStatusList() As ObservableCollection(Of MySeverityStatusList)
        Return _severityStatusList
    End Function

    Public Function GetWeekRangeList() As ObservableCollection(Of MyWeekRange)
        Return _weekRangeList
    End Function

    Public Function GetWeeklyReportList() As ObservableCollection(Of MyWeeklyReport)
        Return _weeklyReportList
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

Public Class MyWeeklyReport
    Public Property WeekID As Integer
    Public Property WeekRangeID As Integer
    Public Property ProjectID As Short
    Public Property Rework As Short
    Public Property RefID As String
    Public Property Subject As String
    Public Property Severity As Short
    Public Property IncType As Short
    Public Property EmpID As Integer
    Public Property Phase As Short
    Public Property Status As Short
    Public Property DateStarted As Date
    Public Property DateTarget As Date
    Public Property DateFinished As Date
    Public Property DateCreated As Date
    Public Property EffortEst As Double
    Public Property ActEffort As Double
    Public Property ActEffortWk As Double
    Public Property Comment As String
    Public Property InboundContacts As Short
End Class

Public Class MyWeekRange

    Public Property WeekRangeID As Integer
    Public Property StartWeek As Date
    Public Property EndWeek As Date
    Public Property DateCreated As Date
    Public Property DateRange As String

End Class

Public Class MyWTaskStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MyWCategoryStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MyWPhaseStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MyWReworkStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class

Public Class MySeverityStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class


