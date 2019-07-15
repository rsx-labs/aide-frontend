Imports System.ComponentModel

Public Class WeekRangeModel
    Implements INotifyPropertyChanged

    Private _dateSubmitted As Date

    Public Property WeekRangeID As Integer
    Public Property StartWeek As Date
    Public Property EndWeek As Date
    Public Property EmpID As Integer
    Public Property Status As Short
    Public Property StatusDesc As String
    Public Property DateRange As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawWeekRange As MyWeekRange)
        Me.WeekRangeID = aRawWeekRange.WeekRangeID
        Me.StartWeek = aRawWeekRange.StartWeek
        Me.EndWeek = aRawWeekRange.EndWeek
        Me.EmpID = aRawWeekRange.EmpID
        Me.Status = aRawWeekRange.Status
        Me.DateSubmitted = aRawWeekRange.DateSubmitted
        Me.DateRange = aRawWeekRange.DateRange
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

    Public Function ToMyWeekRange() As MyWeekRange
        ToMyWeekRange = New MyWeekRange() With {.WeekRangeID = Me.WeekRangeID,
                                                .StartWeek = Me.StartWeek,
                                                .EndWeek = Me.EndWeek,
                                                .EmpID = Me.EmpID,
                                                .Status = Me.Status,
                                                .DateSubmitted = Me.DateSubmitted,
                                                .DateRange = Me.DateRange}
    End Function

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class

Public Class WeeklyReportStatusModel
    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Short
    Public Property Value As String

    Public Sub New(ByVal rawStatus As MyWeeklyReportStatusList)
        Me.Key = rawStatus.Key
        Me.Value = rawStatus.Value
    End Sub

    Public Function ToMyStatus() As MyWeeklyReportStatusList
        ToMyStatus = New MyWeeklyReportStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function
End Class
