Imports System.ComponentModel
Public Class SendCodeModel

    Private _workEmail As String
    Private _fName As String
    Private _lName As String
    Private _personalEmail As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal _iSendCodeList As mySendCodeSet)
        Me._workEmail = _iSendCodeList._sendcodeWorkEmail
        Me._fName = _iSendCodeList._sendcodeFName
        Me._lName = _iSendCodeList._sendcodeLName
        Me._personalEmail = _iSendCodeList._sendcodePersonalEmail
    End Sub


    Public Property WORK_EMAIL As String
        Get
            Return _workEmail
        End Get
        Set(value As String)
            _workEmail = value
            OnPropertyChanged("WORK_EMAIL")
        End Set
    End Property

    Public Property FNAME As String
        Get
            Return _fName
        End Get
        Set(value As String)
            _fName = value
            OnPropertyChanged("FNAME")
        End Set
    End Property

    Public Property LNAME As String
        Get
            Return _lName
        End Get
        Set(value As String)
            _lName = value
            OnPropertyChanged("LNAME")
        End Set
    End Property

    Public Property PERSONAL_EMAIL As String
        Get
            Return _personalEmail
        End Get
        Set(value As String)
            _personalEmail = value
            OnPropertyChanged("PERSONAL_EMAIL")
        End Set
    End Property


    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
