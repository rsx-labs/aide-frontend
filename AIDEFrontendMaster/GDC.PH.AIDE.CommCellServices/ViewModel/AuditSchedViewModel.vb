Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class AuditSchedViewModel
    Implements INotifyPropertyChanged

    Public _auditSchedList As New ObservableCollection(Of AuditSchedModel)
    Private client As AideServiceClient

    Sub New()
    End Sub

    Public Property AuditSchedList As ObservableCollection(Of AuditSchedModel)
        Get
            Return _auditSchedList
        End Get
        Set(value As ObservableCollection(Of AuditSchedModel))
            _auditSchedList = value
            NotifyPropertyChanged("AuditSchedList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
