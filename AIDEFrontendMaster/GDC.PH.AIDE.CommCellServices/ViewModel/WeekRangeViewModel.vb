Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

Public Class WeekRangeViewModel
    Implements INotifyPropertyChanged

    Private _weeklyReportStatusList As New ObservableCollection(Of WeeklyReportStatusModel)
    Private _weekRangeList As New ObservableCollection(Of WeekRangeModel)

    Public Property WeekRangeList As ObservableCollection(Of WeekRangeModel)
        Get
            Return _weekRangeList
        End Get
        Set(value As ObservableCollection(Of WeekRangeModel))
            _weekRangeList = value
            NotifyPropertyChanged("WeekRangeList")
        End Set
    End Property

    Public Property WeeklyReportStatusList As ObservableCollection(Of WeeklyReportStatusModel)
        Get
            Return _weeklyReportStatusList
        End Get
        Set(value As ObservableCollection(Of WeeklyReportStatusModel))
            _weeklyReportStatusList = value
            NotifyPropertyChanged("WeeklyReportStatusList")
        End Set
    End Property
#Region "PropertyChange"

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

End Class
