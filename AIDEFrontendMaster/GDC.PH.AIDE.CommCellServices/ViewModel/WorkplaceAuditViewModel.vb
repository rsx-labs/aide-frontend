Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class WorkplaceAuditViewModel
    Implements INotifyPropertyChanged

    Private _dmvm As ObservableCollection(Of WorkplaceAuditModel)

    Public Sub New()
        _dmvm = New ObservableCollection(Of WorkplaceAuditModel)
    End Sub

    Public Property DMVM As ObservableCollection(Of WorkplaceAuditModel)
        Get
            Return _dmvm
        End Get
        Set(value As ObservableCollection(Of WorkplaceAuditModel))
            _dmvm = value
            NotifyPropertyChanged("DMVM")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
