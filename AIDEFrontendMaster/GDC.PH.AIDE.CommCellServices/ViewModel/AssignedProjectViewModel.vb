Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class AssignedProjectViewModel
    Implements INotifyPropertyChanged

    Private _assignedProjModel As New ObservableCollection(Of AssignedProjectModel)
    Sub New()

    End Sub
    Public Property AssignedProjectList As ObservableCollection(Of AssignedProjectModel)
        Get
            Return _assignedProjModel
        End Get
        Set(value As ObservableCollection(Of AssignedProjectModel))
            _assignedProjModel = value
            NotifyPropertyChanged("AssignedProjectList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class
