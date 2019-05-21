Imports System.ComponentModel
Imports System.Collections.ObjectModel


Public Class AnnouncementListViewModel
    Implements INotifyPropertyChanged

    Private _objAnnouncementset As New ObservableCollection(Of AnnouncementModel)
    Private myAnnouncementSet As New AnnouncementDBProvider

    Sub New()
        For Each _myannounceSet As myAnnouncementSet In myAnnouncementSet._getobjAnnouncement
            _objAnnouncementset.Add(New AnnouncementModel(_myannounceSet))
        Next
    End Sub

    Public Property ObjectAnnouncementSet As ObservableCollection(Of AnnouncementModel)
        Get
            Return _objAnnouncementset
        End Get
        Set(value As ObservableCollection(Of AnnouncementModel))
            _objAnnouncementset = value
            NotifyPropertyChanged("ObjectAnnouncementSet")
        End Set
    End Property


    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
