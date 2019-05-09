Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

Public Class TasksSpViewModel
    Implements INotifyPropertyChanged

    Private _selectedTasksSp As New TasksSpModel()
    Private _tasksSpList As New ObservableCollection(Of TasksSpModel)

    Public Property TasksSpList As ObservableCollection(Of TasksSpModel)
        Get
            Return _tasksSpList
        End Get
        Set(value As ObservableCollection(Of TasksSpModel))
            _tasksSpList = value
            NotifyPropertyChanged("TasksSpList")
        End Set
    End Property

    Public Property TasksSpListForPrint As ObservableCollection(Of TasksSpModel)
        Get
            Return _tasksSpList
        End Get
        Set(value As ObservableCollection(Of TasksSpModel))
            _tasksSpList = value
            NotifyPropertyChanged("TasksSpListForPrint")
        End Set
    End Property

#Region "PropertyChange"

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

End Class
