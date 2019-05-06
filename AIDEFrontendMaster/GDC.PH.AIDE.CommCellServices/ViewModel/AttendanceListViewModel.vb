Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Public Class AttendanceListViewModel
    Implements INotifyPropertyChanged

    Private _employeelistattendance As New ObservableCollection(Of AttendanceModel)

    Public Property EmployeeListAttendance As ObservableCollection(Of AttendanceModel)
        Get
            Return _employeelistattendance
        End Get
        Set(value As ObservableCollection(Of AttendanceModel))
            _employeelistattendance = value
            NotifyPropertyChanged("EmployeeListAttendance")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
