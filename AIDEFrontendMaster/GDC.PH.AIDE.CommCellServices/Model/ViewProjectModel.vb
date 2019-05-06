Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input


''' <summary>
'''GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class ViewProjectModel
    Implements INotifyPropertyChanged


    Private _iname As String
    Public Property Name() As String
        Get
            Return _iname
        End Get
        Set(ByVal value As String)
            _iname = value
            NotifyPropertyChanged("Name")
        End Set
    End Property

    Private _iposition As String
    Public Property Position() As String
        Get
            Return _iposition
        End Get
        Set(ByVal value As String)
            _iposition = value
            NotifyPropertyChanged("Position")
        End Set
    End Property

    Private _iemail As String
    Public Property EmailAddress() As String
        Get
            Return _iemail
        End Get
        Set(ByVal value As String)
            _iemail = value
            NotifyPropertyChanged("EmailAddress")
        End Set
    End Property

    Private _ialternateemail As String
    Public Property AlternateEmail() As String
        Get
            Return _ialternateemail
        End Get
        Set(ByVal value As String)
            _ialternateemail = value
            NotifyPropertyChanged("AlternateEmail")
        End Set
    End Property

    Private _iworkmobile As String
    Public Property WorkMobile() As String
        Get
            Return _iworkmobile
        End Get
        Set(ByVal value As String)
            _iworkmobile = value
            NotifyPropertyChanged("WorkMobile")
        End Set
    End Property

    Private _ilocalnumber As String
    Public Property LocalNumber() As String
        Get
            Return _ilocalnumber
        End Get
        Set(ByVal value As String)
            _ilocalnumber = value
            NotifyPropertyChanged("LocalNumber")
        End Set
    End Property


    Public Sub New(ByVal rawEmployeeList As MyProjectLists)
        Me.Name = rawEmployeeList.Name
        Me.Position = rawEmployeeList.Position
        Me.EmailAddress = rawEmployeeList.EmailAddress
        Me.AlternateEmail = rawEmployeeList.AlternateEmail
        Me.WorkMobile = rawEmployeeList.WorkMobile

    End Sub

    Public Function ToMyEmployee() As MyProjectLists
        Return New MyProjectLists() With {.Name = Me.Name, .Position = Me.Position, .EmailAddress = Me.EmailAddress, .AlternateEmail = Me.AlternateEmail, .WorkMobile = Me.WorkMobile}
    End Function



    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
