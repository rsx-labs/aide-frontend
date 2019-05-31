Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class LateViewModel
    Implements INotifyPropertyChanged

    Public _lateList As New ObservableCollection(Of LateModel)
    Private client As AideServiceClient

    Sub New()
    End Sub

    Public Property LateList As ObservableCollection(Of LateModel)
        Get
            Return _lateList
        End Get
        Set(value As ObservableCollection(Of LateModel))
            _lateList = value
            NotifyPropertyChanged("LateList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
