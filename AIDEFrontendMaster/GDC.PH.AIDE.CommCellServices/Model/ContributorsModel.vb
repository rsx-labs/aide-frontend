Imports System.ComponentModel
Public Class ContributorsModel
    Private _fullname As String
    Private _department As String
    Private _division As String
    Private _position As String
    Private _imagepath As String

    Public Sub New(myContri As myContributorsSet)
        Me._fullname = myContri._fullname
        Me._department = myContri._department
        Me._division = myContri._division
        Me._position = myContri._position
        Me._imagepath = myContri._imagepath
    End Sub

    Public Property FULL_NAME As String
        Get
            Return Me._fullname
        End Get
        Set(value As String)
            Me._fullname = value
            OnPropertyChanged("FULL_NAME")
        End Set
    End Property

    Public Property DEPARTMENT As String
        Get
            Return Me._department + " Department"
        End Get
        Set(value As String)
            Me._department = value
            OnPropertyChanged("DEPARTMENT")
        End Set
    End Property

    Public Property DIVISION As String
        Get
            Return Me._division
        End Get
        Set(value As String)
            Me._division = value
            OnPropertyChanged("DIVISION")
        End Set
    End Property

    Public Property POSITION As String
        Get
            Return Me._position
        End Get
        Set(value As String)
            Me._position = value
            OnPropertyChanged("POSITION")
        End Set
    End Property

    Public Property IMAGE_PATH As String
        Get
            Return Me._imagepath
        End Get
        Set(value As String)
            Me._imagepath = value
            OnPropertyChanged("IMAGE_PATH")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

End Class
