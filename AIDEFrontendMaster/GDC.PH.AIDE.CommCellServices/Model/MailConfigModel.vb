Imports System.ComponentModel
Public Class MailConfigModel
    Private _scsenderemail As String
    Private _scsubject As String
    Private _scbody As String
    Private _scport As Integer
    Private _schost As String
    Private _scenablessl As Integer
    Private _sctimeout As Integer
    Private _scusedfltcredentials As Integer
    Private _scsenderpassword As String
    Private _scpasswordexpiry As Integer

    Public Sub New()

    End Sub

    Public Sub New(ByVal mConfig As myMailConfigSet)
        Me._scsenderemail = mConfig.scsenderemail
        Me._scsubject = mConfig.scsubject
        Me._scbody = mConfig.scbody
        Me._scport = mConfig.scport
        Me._schost = mConfig.schost
        Me._scenablessl = mConfig.scenablessl
        Me._sctimeout = mConfig.sctimeout
        Me._scusedfltcredentials = mConfig.scusedfltcredentials
        Me._scsenderpassword = mConfig.scsenderpassword
        Me._scpasswordexpiry = mConfig.scpasswordexpiry
    End Sub

    Public Property SENDER_EMAIL As String
        Get
            Return _scsenderemail
        End Get
        Set(value As String)
            _scsenderemail = value
            OnPropertyChanged("SENDER_EMAIL")
        End Set
    End Property

    Public Property SUBJECT As String
        Get
            Return _scsubject
        End Get
        Set(value As String)
            _scsubject = value
            OnPropertyChanged("SUBJECT")
        End Set
    End Property

    Public Property BODY As String
        Get
            Return _scbody
        End Get
        Set(value As String)
            _scbody = value
            OnPropertyChanged("BODY")
        End Set
    End Property

    Public Property PORT As Integer
        Get
            Return _scport
        End Get
        Set(value As Integer)
            _scport = value
            OnPropertyChanged("PORT")
        End Set
    End Property

    Public Property HOST As String
        Get
            Return _schost
        End Get
        Set(value As String)
            _schost = value
            OnPropertyChanged("HOST")
        End Set
    End Property

    Public Property ENABLE_SSL As Integer
        Get
            Return _scenablessl
        End Get
        Set(value As Integer)
            _scenablessl = value
            OnPropertyChanged("ENABLE_SSL")
        End Set
    End Property

    Public Property TIMEOUT As Integer
        Get
            Return _sctimeout
        End Get
        Set(value As Integer)
            _sctimeout = value
            OnPropertyChanged("TIMEOUT")
        End Set
    End Property

    Public Property USE_DFLT_CREDENTIALS As Integer
        Get
            Return _scusedfltcredentials
        End Get
        Set(value As Integer)
            _scusedfltcredentials = value
            OnPropertyChanged("USE_DFLT_CREDENTIALS")
        End Set
    End Property

    Public Property SENDER_PASSWORD As String
        Get
            Return _scsenderpassword
        End Get
        Set(value As String)
            _scsenderpassword = value
            OnPropertyChanged("SENDER_PASSWORD")
        End Set
    End Property

    Public Property PASSWORD_EXPIRY As Integer
        Get
            Return _scpasswordexpiry
        End Get
        Set(value As Integer)
            _scpasswordexpiry = value
            OnPropertyChanged("PASSWORD_EXPIRY")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
