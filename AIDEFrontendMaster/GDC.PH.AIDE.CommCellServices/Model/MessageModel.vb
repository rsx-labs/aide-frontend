Imports System.ComponentModel
Public Class MessageModel
    Private _messagedescr As String
    Private _title As String

    Public Sub New()

    End Sub
    Public Sub New(ByVal myMsg As myMessageSet)
        Me._messagedescr = myMsg._messagedescr
        Me._title = myMsg._title
    End Sub

    Public Property MESSAGE_DESCR As String
        Get
            Return Me._messagedescr
        End Get
        Set(value As String)
            Me._messagedescr = value
        End Set
    End Property

    Public Property TITLE As String
        Get
            Return Me._title
        End Get
        Set(value As String)
            Me._title = value
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
