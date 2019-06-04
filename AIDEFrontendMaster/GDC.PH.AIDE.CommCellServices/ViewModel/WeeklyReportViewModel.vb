Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

Public Class WeeklyReportViewModel
    Implements INotifyPropertyChanged

    Private _taskStatusList As New ObservableCollection(Of WTaskStatusModel)
    Private _categoryStatusList As New ObservableCollection(Of WCategoryStatusModel)
    Private _phaseStatusList As New ObservableCollection(Of WPhaseStatusModel)
    Private _reworkStatusList As New ObservableCollection(Of WReworkStatusModel)
    Private _severityStatusList As New ObservableCollection(Of SeverityStatusModel)
    Private _weeklyReportList As New ObservableCollection(Of WeeklyReportModel)
    Private _weekRangeList As New ObservableCollection(Of WeekRangeModel)
    Private _newWeeklyReport As New WeeklyReportModel

    Public Property NewWeeklyReport As WeeklyReportModel
        Get
            Return _newWeeklyReport
        End Get
        Set(value As WeeklyReportModel)
            _newWeeklyReport = value
            NotifyPropertyChanged("NewWeeklyReport")
        End Set
    End Property

    Public Property TaskStatusList As ObservableCollection(Of WTaskStatusModel)
        Get
            Return _taskStatusList
        End Get
        Set(value As ObservableCollection(Of WTaskStatusModel))
            _taskStatusList = value
            NotifyPropertyChanged("TaskStatusList")
        End Set
    End Property

    Public Property CategoryStatusList As ObservableCollection(Of WCategoryStatusModel)
        Get
            Return _categoryStatusList
        End Get
        Set(value As ObservableCollection(Of WCategoryStatusModel))
            _categoryStatusList = value
            NotifyPropertyChanged("CategoryStatusList")
        End Set
    End Property

    Public Property PhaseStatusList As ObservableCollection(Of WPhaseStatusModel)
        Get
            Return _phaseStatusList
        End Get
        Set(value As ObservableCollection(Of WPhaseStatusModel))
            _phaseStatusList = value
            NotifyPropertyChanged("PhaseStatusList")
        End Set
    End Property

    Public Property ReworkStatusList As ObservableCollection(Of WReworkStatusModel)
        Get
            Return _reworkStatusList
        End Get
        Set(value As ObservableCollection(Of WReworkStatusModel))
            _reworkStatusList = value
            NotifyPropertyChanged("ReworkStatusList")
        End Set
    End Property

    Public Property SeverityStatusList As ObservableCollection(Of SeverityStatusModel)
        Get
            Return _severityStatusList
        End Get
        Set(value As ObservableCollection(Of SeverityStatusModel))
            _severityStatusList = value
            NotifyPropertyChanged("SeverityStatusList")
        End Set
    End Property

    Public Property WeeklyReportList As ObservableCollection(Of WeeklyReportModel)
        Get
            Return _weeklyReportList
        End Get
        Set(value As ObservableCollection(Of WeeklyReportModel))
            _weeklyReportList = value
            NotifyPropertyChanged("WeeklyReportList")
        End Set
    End Property

#Region "PropertyChange"

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

End Class
