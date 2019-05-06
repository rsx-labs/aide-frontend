Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class EmployeeListProvider
    Private _employeeLists As ObservableCollection(Of MyEmployee)


    Private _employeeList As ObservableCollection(Of MyEmployeeList)
    Private _MyEmployeeList As New MyEmployeeList
    Sub New()
        _employeeList = New ObservableCollection(Of MyEmployeeList)
        _employeeLists = New ObservableCollection(Of MyEmployee)
    End Sub

    Public Function GetEmployeeList() As ObservableCollection(Of MyEmployeeList)

        Return _employeeList

    End Function
    Public Function GetEmployees() As ObservableCollection(Of MyEmployee)

        Return _employeeLists

    End Function


    Public Sub SetEmployeeList(ByVal _employee As Employee)

        Dim _employeeObj As MyEmployeeList = New MyEmployeeList With {.Name = _employee.EmployeeName,
                                                                      .Nickname = _employee.Nickname,
                                                                      .EmpID = _employee.EmployeeID}

        _employeeList.Add(_employeeObj)
    End Sub
End Class


''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class MyEmployeeList

    Public Property Name As String
    Public Property Nickname As String
    Public Property EmpID As Integer
    Public Property DateStarted As Date = Date.Now
    Public Property DateFinished As Date = Date.Now
End Class

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class MyEmployee

    Public Property Name() As String
    Public Property EmployeeID() As String
    Public Property DateStarted() As Date = Date.Now
    Public Property DateFinished() As Date = Date.Now
End Class