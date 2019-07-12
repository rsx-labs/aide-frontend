Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class MessageViewModel
    Implements INotifyPropertyChanged


    Private _objMessageList As New ObservableCollection(Of MessageModel)
    Private _objMessage As New MessageModel
    Private myMessageSet As New MessageDBProvider

    Sub New()
        For Each _myMessageSet As myMessageSet In myMessageSet._getobjMessage
            _objMessageList.Add(New MessageModel(_myMessageSet))
        Next
    End Sub

    Public Property ObjectMessageList As ObservableCollection(Of MessageModel)
        Get
            Return Me._objMessageList
        End Get
        Set(value As ObservableCollection(Of MessageModel))
            Me._objMessageList = value
            NotifyPropertyChanged("ObjectMessageList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
