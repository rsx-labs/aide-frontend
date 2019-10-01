Imports System.ComponentModel

Public Class WeeklyTeamStatusReportModel
    Implements INotifyPropertyChanged

    Private _dateSubmitted As Date

    Public Property WeekRangeID As Integer
    Public Property EmployeeID As Integer
    Public Property EmployeeName As String
    Public Property TotalHours As Double
    Public Property Status As Short
    Public Property StatusDesc As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawWeeklyTeamStatusReport As MyWeeklyTeamStatusReport)
        Me.WeekRangeID = aRawWeeklyTeamStatusReport.WeekRangeID
        Me.EmployeeID = aRawWeeklyTeamStatusReport.EmployeeID
        Me.EmployeeName = aRawWeeklyTeamStatusReport.EmployeeName
        Me.TotalHours = aRawWeeklyTeamStatusReport.TotalHours
        Me.Status = aRawWeeklyTeamStatusReport.Status
        Me.DateSubmitted = aRawWeeklyTeamStatusReport.DateSubmitted
    End Sub

    Public Property DateSubmitted As String
        Get
            If _dateSubmitted = Nothing Then
                Return String.Empty
            Else
                Return _dateSubmitted.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _dateSubmitted = Nothing
            Else
                _dateSubmitted = CDate(value)
            End If
            NotifyPropertyChanged("DateSubmitted")
        End Set
    End Property

    Public Function ToMyWeekRange() As MyWeeklyTeamStatusReport
        ToMyWeekRange = New MyWeeklyTeamStatusReport() With {.WeekRangeID = Me.WeekRangeID,
                                                             .EmployeeID = Me.EmployeeID,
                                                             .EmployeeName = Me.EmployeeName,
                                                             .TotalHours = Me.TotalHours,
                                                             .Status = Me.Status,
                                                             .DateSubmitted = Me.DateSubmitted}
    End Function

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class

