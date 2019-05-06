Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

Public Class TasksViewModel
    Implements INotifyPropertyChanged

    Private _taskStatusList As New ObservableCollection(Of TaskStatusModel)
    Private _categoryStatusList As New ObservableCollection(Of CategoryStatusModel)
    Private _phaseStatusList As New ObservableCollection(Of PhaseStatusModel)
    Private _reworkStatusList As New ObservableCollection(Of ReworkStatusModel)
    Private _taskList As New ObservableCollection(Of TasksModel)
    Private _newTasks As New TasksModel

    Public Property NewTasks As TasksModel
        Get
            Return _newTasks
        End Get
        Set(value As TasksModel)
            _newTasks = value
            NotifyPropertyChanged("NewTasks")
        End Set
    End Property

    Public Property TaskStatusList As ObservableCollection(Of TaskStatusModel)
        Get
            Return _taskstatusList
        End Get
        Set(value As ObservableCollection(Of TaskStatusModel))
            _taskstatusList = value
            NotifyPropertyChanged("TaskStatusList")
        End Set
    End Property

    Public Property CategoryStatusList As ObservableCollection(Of CategoryStatusModel)
        Get
            Return _categoryStatusList
        End Get
        Set(value As ObservableCollection(Of CategoryStatusModel))
            _categoryStatusList = value
            NotifyPropertyChanged("CategoryStatusList")
        End Set
    End Property

    Public Property PhaseStatusList As ObservableCollection(Of PhaseStatusModel)
        Get
            Return _phaseStatusList
        End Get
        Set(value As ObservableCollection(Of PhaseStatusModel))
            _phaseStatusList = value
            NotifyPropertyChanged("PhaseStatusList")
        End Set
    End Property

    Public Property ReworkStatusList As ObservableCollection(Of ReworkStatusModel)
        Get
            Return _reworkStatusList
        End Get
        Set(value As ObservableCollection(Of ReworkStatusModel))
            _reworkStatusList = value
            NotifyPropertyChanged("ReworkStatusList")
        End Set
    End Property

    Public Property TaskList As ObservableCollection(Of TasksModel)
        Get
            Return _taskList
        End Get
        Set(value As ObservableCollection(Of TasksModel))
            _taskList = value
            NotifyPropertyChanged("TaskList")
        End Set
    End Property

#Region "PropertyChange"

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

End Class
