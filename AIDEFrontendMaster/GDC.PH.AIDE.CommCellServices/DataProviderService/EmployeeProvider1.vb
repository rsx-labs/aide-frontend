Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class EmployeeProvider1
    Private _categoryList As New ObservableCollection(Of MyCategoryList)
    Private _billabilityList As New ObservableCollection(Of MyBillabilityList)
    Private _employeeLists As ObservableCollection(Of MyEmployeeList)
    Private _employeeList As ObservableCollection(Of MyEmployee)
    'Private _MyEmployeeList As New MyEmployeeList
    Sub New()
        _employeeList = New ObservableCollection(Of MyEmployee)
        _employeeLists = New ObservableCollection(Of MyEmployeeList)

    End Sub

    Public Function GetEmployees() As ObservableCollection(Of MyEmployee)

        Return _employeeList

    End Function

    Public Function GetEmployeesLists() As ObservableCollection(Of MyEmployeeList)

        Return _employeeLists

    End Function

    Public Sub SetEmployeeList(ByVal _employee As Employee)

        Dim _employeeObj As MyEmployee = New MyEmployee With {.Name = _employee.FirstName + " " + _employee.MiddleInitial + ". " + _employee.LastName, .EmployeeID = _employee.EmployeeID}
        _employeeList.Add(_employeeObj)
    End Sub

    Public Sub SetEmployeeLists(ByVal _employee As Employee)

        Dim _employeeObj As MyEmployeeList = New MyEmployeeList With {.Name = _employee.EmployeeName,
                                                                      .Nickname = _employee.Nickname,
                                                                      .EmpID = _employee.EmployeeID}

        _employeeLists.Add(_employeeObj)
    End Sub



    Public Function AddUser(ByVal aUser As MyEmployee) As Boolean
        _employeeList.Add(aUser)
        AddUser = True
    End Function

    Public Function GetCategoryList() As ObservableCollection(Of MyCategoryList)
        Return _categoryList
    End Function

    Public Function GetBillabilityList() As ObservableCollection(Of MyBillabilityList)
        Return _billabilityList
    End Function




End Class


''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>

Public Class MyCategoryList
    Public Property Key As Integer
    Public Property Value As String
End Class

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class MyBillabilityList
    Public Property Key As Integer
    Public Property Value As String
End Class
