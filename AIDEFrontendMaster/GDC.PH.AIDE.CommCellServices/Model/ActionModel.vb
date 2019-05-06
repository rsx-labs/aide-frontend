Imports System.ComponentModel

''' <summary>
''' By Jester Sanchez
''' </summary>
''' <remarks></remarks>
Public Class ActionModel
    Private _refno As String
    Private _actionMessage As String
    Private _empID As Integer
    Private _nickName As String
    Private _dueDate As Date
    Private _dateClosed As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal _iActionList As myActionSet)
        Me._refno = _iActionList._actRef
        Me._actionMessage = _iActionList._actMessage
        Me._empID = _iActionList._actAssignee
        Me._nickName = _iActionList._actNickName
        Me._dueDate = _iActionList._actDueDate
        Me._dateClosed = _iActionList._actDateClosed
    End Sub

    Public Property REF_NO As String
        Get
            Return _refno
        End Get
        Set(value As String)
            _refno = value
            OnPropertyChanged("REF_NO")
        End Set
    End Property

    Public Property ACTION_MESSAGE As String
        Get
            Return _actionMessage
        End Get
        Set(value As String)
            _actionMessage = value
            OnPropertyChanged("ACTION_MESSAGE")
        End Set
    End Property

    Public Property EMP_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            OnPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property NICK_NAME As String
        Get
            Return _nickName
        End Get
        Set(value As String)
            _nickName = value
            OnPropertyChanged("NICK_NAME")
        End Set
    End Property

    Public Property DUE_DATE As Date
        Get
            Return _dueDate
        End Get
        Set(value As Date)
            _dueDate = value
            OnPropertyChanged("DUE_DATE")
        End Set
    End Property

    Public Property DATE_CLOSED As String
        Get
            Return _dateClosed
        End Get
        Set(value As String)
            _dateClosed = value
            OnPropertyChanged("DATE_CLOSED")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

End Class
