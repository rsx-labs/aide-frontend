Imports System.ComponentModel
Imports System.Collections.ObjectModel


Public Class KPISummaryViewModel
    Implements INotifyPropertyChanged

    Public _kpiSummaryModel As New KPISummaryModel
    Public _collKPISummary As New ObservableCollection(Of KPISummaryModel)

    Sub New()
        _collKPISummary = New ObservableCollection(Of KPISummaryModel)
        _kpiSummaryModel = New KPISummaryModel
    End Sub

    Public Property KPISummaryCollection As ObservableCollection(Of KPISummaryModel)
        Get
            Return _collKPISummary
        End Get
        Set(value As ObservableCollection(Of KPISummaryModel))
            _collKPISummary = value
            NotifyPropertyChanged("KPISummaryCollection")
        End Set
    End Property

    Public Property KPISummaryModelData As KPISummaryModel
        Get
            Return _kpiSummaryModel
        End Get
        Set(value As KPISummaryModel)
            _kpiSummaryModel = value
            NotifyPropertyChanged("KPISummaryModelData")
        End Set
    End Property


    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
