Imports System.ComponentModel
Public Class AnnouncementModel
    Private _announcementid As Integer
    Private _empid As Integer
    Private _title As String
    Private _message As String
    Private _dateposted As Date
    Private _deletedfg As Integer

    Public Sub New()
    End Sub

    Public Sub New(ByVal _iAnnouncementSet As myAnnouncementSet)
        Me._announcementid = _iAnnouncementSet._announcementID
        Me._empid = _iAnnouncementSet._empid
        Me.MESSAGE = _iAnnouncementSet._message
        Me.TITLE = _iAnnouncementSet._title
        Me.DATE_POSTED = _iAnnouncementSet._enddate
    End Sub

    Public Property ANNOUNCEMENT_ID As Integer
        Get
            Return _announcementid
        End Get
        Set(value As Integer)
            _announcementid = value
            OnPropertyChanged("ANNOUNCEMENT_ID")
        End Set
    End Property

    Public Property EMP_ID As Integer
        Get
            Return _empid
        End Get
        Set(value As Integer)
            _empid = value
            OnPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property TITLE As String
        Get
            Return _title
        End Get
        Set(value As String)
            _title = value
            OnPropertyChanged("TITLE")
        End Set
    End Property

    Public Property MESSAGE As String
        Get
            Return _message
        End Get
        Set(value As String)
            _message = value
            OnPropertyChanged("MESSAGE")
        End Set
    End Property

    Public Property DATE_POSTED As Date
        Get
            Return _dateposted
        End Get
        Set(value As Date)
            _dateposted = value
            OnPropertyChanged("DATE_POSTED")
        End Set
    End Property

    Public Property DELETED_FG As Date
        Get
            Return _dateposted
        End Get
        Set(value As Date)
            _dateposted = value
            OnPropertyChanged("DATE_POSTED")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
