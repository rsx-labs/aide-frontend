Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class DailyViewModel
    Implements INotifyPropertyChanged

   Private _dmvm As ObservableCollection(Of DailyModel)

    Private Sub New()
        _dmvm = New ObservableCollection(Of DailyModel)
    End Sub

    Public Property DMVM As ObservableCollection(Of DailyModel)
        Get
            Return _dmvm
        End Get
        Set(value As ObservableCollection(Of DailyModel))
            _dmvm = value
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
