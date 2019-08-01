Imports System.ComponentModel
Imports System.Collections.ObjectModel


Public Class AnnouncementListViewModel
    Implements INotifyPropertyChanged

    Public _objAnnouncement As New AnnouncementModel
    Public _objAnnouncementset As New ObservableCollection(Of AnnouncementModel)

    Sub New()
        _objAnnouncementset = New ObservableCollection(Of AnnouncementModel)
        _objAnnouncement = New AnnouncementModel
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

    Public Property ObjectAnnouncement As AnnouncementModel
        Get
            Return _objAnnouncement
        End Get
        Set(value As AnnouncementModel)
            _objAnnouncement = value
            NotifyPropertyChanged("ObjectAnnouncement")
        End Set
    End Property


    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
