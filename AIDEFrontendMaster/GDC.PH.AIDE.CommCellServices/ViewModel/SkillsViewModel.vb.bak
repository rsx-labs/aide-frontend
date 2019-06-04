Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Public Class SkillsViewModel
    Implements INotifyPropertyChanged

    Private _skill As New ObservableCollection(Of SkillsModel)
    Private _empDetails As New ObservableCollection(Of SkillsModel)

    Public Property SkillList As ObservableCollection(Of SkillsModel)
        Get
            Return _skill
        End Get
        Set(value As ObservableCollection(Of SkillsModel))
            _skill = value
            NotifyPropertyChanged("SkillList")
        End Set
    End Property

    Public Property EmpDetails As ObservableCollection(Of SkillsModel)
        Get
            Return _empDetails
        End Get
        Set(value As ObservableCollection(Of SkillsModel))
            _empDetails = value
            NotifyPropertyChanged("EmpDetails")
        End Set
    End Property
    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
