Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

Public Class WeeklyReportViewModel
    Implements INotifyPropertyChanged

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
