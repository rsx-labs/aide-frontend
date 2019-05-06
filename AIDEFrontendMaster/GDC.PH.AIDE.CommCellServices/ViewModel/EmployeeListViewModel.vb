Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input


''' <summary>
'''MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>

Public Class EmployeeListViewModel
    Implements INotifyPropertyChanged

    Private _db As New EmployeeListProvider
    Private _employeeList As New ObservableCollection(Of EmployeeListModel)
    Private _assignEmployee As New ObservableCollection(Of EmployeeListModel)

    Sub New()
        Try
            For Each rawUser As MyEmployeeList In _db.GetEmployeeList()
                _employeeList.Add(New EmployeeListModel(rawUser))
            Next
        Catch ex As Exception
            Console.Write(ex.Message)
        End Try

    End Sub

#Region "Properties"
    Public Property EmployeeList As ObservableCollection(Of EmployeeListModel)
        Get
            Return _employeeList
        End Get
        Set(value As ObservableCollection(Of EmployeeListModel))
            _employeeList = value
            NotifyPropertyChanged("EmployeeList")
        End Set
    End Property


    Public Property AssignedEmployeeList As ObservableCollection(Of EmployeeListModel)
        Get
            Return _assignEmployee
        End Get
        Set(value As ObservableCollection(Of EmployeeListModel))
            _assignEmployee = value
            NotifyPropertyChanged("AssignedEmployeeList")
        End Set
    End Property

#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
