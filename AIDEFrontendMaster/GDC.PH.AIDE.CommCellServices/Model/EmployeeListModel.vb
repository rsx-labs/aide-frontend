Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input



''' <summary>
'''MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class EmployeeListModel
    Implements INotifyPropertyChanged

    Public Property DateStarted() As Date
    Public Property DateFinished() As Date

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

    Private _inickname As String
    Public Property Nickname() As String
        Get
            Return _inickname
        End Get
        Set(ByVal value As String)
            _inickname = value
            NotifyPropertyChanged("Nickname")
        End Set
    End Property

    Private _iempID As Integer
    Public Property EmpID() As String
        Get
            Return _iempID
        End Get
        Set(ByVal value As String)
            _iempID = value
            NotifyPropertyChanged("EmpID")
        End Set
    End Property

    Public Sub New(ByVal rawEmployeeList As MyEmployeeList)
        Me.Name = rawEmployeeList.Name
        Me.Nickname = rawEmployeeList.Nickname
        Me.EmpID = rawEmployeeList.EmpID
        Me.DateStarted = rawEmployeeList.DateStarted
        Me.DateFinished = rawEmployeeList.DateFinished
    End Sub

    Public Function ToMyEmployee() As MyEmployeeList
        Return New MyEmployeeList() With {.Name = Me.Name, .Nickname = Me.Nickname, .EmpID = Me.EmpID}
    End Function



    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
