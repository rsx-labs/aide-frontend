Public Class TasksSpModel

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

    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawTasksSp As MyTasksSp)
        Me.EmployeeName = aRawTasksSp.EmployeeName
        Me.LastWeekOutstanding = aRawTasksSp.LastWeekOutstanding

        Me.MonAt = aRawTasksSp.MonAt
        Me.MonCt = aRawTasksSp.MonCt
        Me.MonOt = aRawTasksSp.MonOt

        Me.TueAt = aRawTasksSp.TueAt
        Me.TueCt = aRawTasksSp.TueCt
        Me.TueOt = aRawTasksSp.TueOt

        Me.WedAt = aRawTasksSp.WedAt
        Me.WedCt = aRawTasksSp.WedCt
        Me.WedOt = aRawTasksSp.WedOt

        Me.ThuAt = aRawTasksSp.ThuAt
        Me.ThuCt = aRawTasksSp.ThuCt
        Me.ThuOt = aRawTasksSp.ThuOt

        Me.FriAt = aRawTasksSp.FriAt
        Me.FriCt = aRawTasksSp.FriCt
        Me.FriOt = aRawTasksSp.FriOt
    End Sub

    Public Function ToMyTasksSp() As MyTasksSp
        ToMyTasksSp = New MyTasksSp() With {.EmployeeName = Me.EmployeeName,
                                            .LastWeekOutstanding = Me.LastWeekOutstanding,
                                            .MonAt = Me.MonAt,
                                            .MonCt = Me.MonCt,
                                            .MonOt = Me.MonOt,
                                            .TueAt = Me.TueAt,
                                            .TueCt = Me.TueCt,
                                            .TueOt = Me.TueOt,
                                            .WedAt = Me.WedAt,
                                            .WedCt = Me.WedCt,
                                            .WedOt = Me.WedOt,
                                            .ThuAt = Me.ThuAt,
                                            .ThuCt = Me.ThuCt,
                                            .ThuOt = Me.ThuOt,
                                            .FriAt = Me.FriAt,
                                            .FriCt = Me.FriCt,
                                            .FriOt = Me.FriOt}
    End Function

End Class
