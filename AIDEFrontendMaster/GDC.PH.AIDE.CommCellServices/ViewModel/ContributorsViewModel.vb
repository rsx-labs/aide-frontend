Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class ContributorsViewModel
    Implements INotifyPropertyChanged

    Private _objContributorsSet As New ObservableCollection(Of ContributorsModel)
    Private myContributorsSet As New ContributorsDBProvider

    Sub New()
        For Each _myContriSet As myContributorsSet In myContributorsSet._getobjContributors
            _objContributorsSet.Add(New ContributorsModel(_myContriSet))
        Next
    End Sub

    Public Property ObjectContributors As ObservableCollection(Of ContributorsModel)
        Get
            Return _objContributorsSet
        End Get
        Set(value As ObservableCollection(Of ContributorsModel))
            _objContributorsSet = value
            NotifyPropertyChanged("ObjectContributors")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
