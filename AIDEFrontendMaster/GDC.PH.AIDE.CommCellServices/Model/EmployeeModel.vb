''' <summary>
'''MODIFIED GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>

Public Class EmployeeModel
    Public Property Name() As String
    Public Property EmployeeID() As String
    Public Property DateStarted() As Date
    Public Property DateFinished() As Date

    Public Sub New(ByVal rawEmployee As MyEmployee)
        Me.Name = rawEmployee.Name
        Me.EmployeeID = rawEmployee.EmployeeID
        Me.DateStarted = rawEmployee.DateStarted
        Me.DateFinished = rawEmployee.DateFinished

    End Sub

    'Public Sub New(ByVal employee As MyEmployee)

    'End Sub

    Public Function ToMyEmployee() As MyEmployee
        Return New MyEmployee() With {.Name = Me.Name, .EmployeeID = Me.EmployeeID, .DateStarted = Me.DateStarted, .DateFinished = Me.DateFinished}
    End Function


End Class
