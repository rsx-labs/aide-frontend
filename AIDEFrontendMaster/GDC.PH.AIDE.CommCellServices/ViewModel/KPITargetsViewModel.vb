Imports System.ComponentModel
Imports System.Collections.ObjectModel


Public Class KPITargetsViewModel
    Implements INotifyPropertyChanged

    Public _kpiTargetsModel As New KPITargetsModel
    Public _collKPITargets As New ObservableCollection(Of KPITargetsModel)

    Sub New()
        _collKPITargets = New ObservableCollection(Of KPITargetsModel)
        _kpiTargetsModel = New KPITargetsModel
    End Sub

    Public Property KPITargetsCollection As ObservableCollection(Of KPITargetsModel)
        Get
            Return _collKPITargets
        End Get
        Set(value As ObservableCollection(Of KPITargetsModel))
            _collKPITargets = value
            NotifyPropertyChanged("KPITargetsCollection")
        End Set
    End Property

    Public Property KPITargetsSet As KPITargetsModel
        Get
            Return _kpiTargetsModel
        End Get
        Set(value As KPITargetsModel)
            _kpiTargetsModel = value
            NotifyPropertyChanged("KPITargetsSet")
        End Set
    End Property


    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
