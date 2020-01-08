Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class WeeklyReportDBProvider

    Private _weeklyReportList As New ObservableCollection(Of MyWeeklyReport)
    Private _weeklyReportStatusList As New ObservableCollection(Of MyWeeklyReportStatusList)
    Private _weekRangeList As New ObservableCollection(Of MyWeekRange)
    Private _weeklyTeamStatusReportList As New ObservableCollection(Of MyWeeklyTeamStatusReport)

    Public Sub SetWeekRangeList(ByVal weekRange As WeekRange)
        Dim objWeekRange As MyWeekRange = New MyWeekRange With {.WeekRangeID = weekRange.WeekRangeID,
                                                                .StartWeek = weekRange.StartWeek,
                                                                .EndWeek = weekRange.EndWeek,
                                                                .EmpID = weekRange.EmployeeID,
                                                                .Status = weekRange.Status,
                                                                .DateSubmitted = weekRange.DateSubmitted,
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
                                                     .EffortEst = _weeklyReport.EffortEst,
                                                     .ActEffort = _weeklyReport.ActualEffort,
                                                     .ActEffortWk = _weeklyReport.ActualEffortWk,
                                                     .Comment = _weeklyReport.Comments,
                                                     .InboundContacts = _weeklyReport.InboundContacts,
                                                     .ProjectCode = _weeklyReport.ProjCode,
                                                     .TaskID = _weeklyReport.TaskID
                                                     }
        _weeklyReportList.Add(_objWeeklyReport)
    End Sub

    Public Sub SetWeeklyTeamStatusReportList(ByVal weeklyTeamStatusReport As WeeklyTeamStatusReport)
        Dim objWeeklyTeamStatusReport As MyWeeklyTeamStatusReport = New MyWeeklyTeamStatusReport With {.WeekRangeID = weeklyTeamStatusReport.WeekRangeID,
                                                                                                       .EmployeeID = weeklyTeamStatusReport.EmployeeID,
                                                                                                       .EmployeeName = weeklyTeamStatusReport.EmployeeName,
                                                                                                       .TotalHours = weeklyTeamStatusReport.TotalHours,
                                                                                                       .Status = weeklyTeamStatusReport.Status,
                                                                                                       .DateSubmitted = weeklyTeamStatusReport.DateSubmitted}
        _weeklyTeamStatusReportList.Add(objWeeklyTeamStatusReport)
    End Sub

    Public Sub SetMyWeeklyReportStatusList(ByVal _status As StatusGroup)
        Dim status As New MyWeeklyReportStatusList
        status.Key = _status.Status
        status.Value = _status.Description
        _weeklyReportStatusList.Add(status)
    End Sub

    Public Function GetWeekRangeList() As ObservableCollection(Of MyWeekRange)
        Return _weekRangeList
    End Function

    Public Function GetWeeklyReportList() As ObservableCollection(Of MyWeeklyReport)
        Return _weeklyReportList
    End Function

    Public Function GetWeeklyTeamStatusReportList() As ObservableCollection(Of MyWeeklyTeamStatusReport)
        Return _weeklyTeamStatusReportList
    End Function

    Public Function GetWeeklyReportStatusList() As ObservableCollection(Of MyWeeklyReportStatusList)
        Return _weeklyReportStatusList
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
    Public Property ProjectCode As Integer
    Public Property TaskID As Integer
End Class

Public Class MyWeeklyTeamStatusReport
    Public Property WeekRangeID As Integer
    Public Property EmployeeID As Integer
    Public Property EmployeeName As String
    Public Property TotalHours As Double
    Public Property Status As Short
    Public Property DateSubmitted As Date
    Public Property DateRange As String
End Class

Public Class MyWeekRange
    Public Property WeekRangeID As Integer
    Public Property StartWeek As Date
    Public Property EndWeek As Date
    Public Property EmpID As Integer
    Public Property Status As Short
    Public Property DateSubmitted As Date
    Public Property DateRange As String
End Class

Public Class MyWeeklyReportStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class


