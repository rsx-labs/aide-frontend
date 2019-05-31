Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class ComcellViewModel
    Implements INotifyPropertyChanged

    Public _comcellList As New ObservableCollection(Of ComcellModel)
    Private client As AideServiceClient

    Sub New()
    End Sub

    Public Property ComcellList As ObservableCollection(Of ComcellModel)
        Get
            Return _comcellList
        End Get
        Set(value As ObservableCollection(Of ComcellModel))
            _comcellList = value
            NotifyPropertyChanged("ComcellList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
