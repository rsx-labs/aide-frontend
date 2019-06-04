Public Class WeekRangeModel

    Public Property WeekRangeID As Integer
    Public Property StartWeek As Date
    Public Property EndWeek As Date
    Public Property DateCreated As Date
    Public Property DateRange As String
    
    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawWeekRange As MyWeekRange)
        Me.WeekRangeID = aRawWeekRange.WeekRangeID
        Me.StartWeek = aRawWeekRange.StartWeek
        Me.EndWeek = aRawWeekRange.EndWeek
        Me.DateCreated = aRawWeekRange.DateCreated
        Me.DateRange = aRawWeekRange.DateRange
    End Sub

    Public Function ToMyWeekRange() As MyWeekRange
        ToMyWeekRange = New MyWeekRange() With {.WeekRangeID = Me.WeekRangeID,
                                                .StartWeek = Me.StartWeek,
                                                .EndWeek = Me.EndWeek,
                                                .DateCreated = Me.DateCreated,
                                                .DateRange = Me.DateRange}
    End Function

End Class
