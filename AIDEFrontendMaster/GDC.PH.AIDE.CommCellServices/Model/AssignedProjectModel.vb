Imports System.ComponentModel
Public Class AssignedProjectModel
    Implements INotifyPropertyChanged

    Private _iname As String
    Private _iposition As String
    Private _iemail As String
    Private _ialternateemail As String
    Private _iworkmobile As String
    Private _ilocalnumber As String
    Public Sub New(ByVal rawAssignedProjectList As MyAssignedProjectLists)
        Me.Name = rawAssignedProjectList.Name
        Me.Position = rawAssignedProjectList.Position
        Me.EmailAddress = rawAssignedProjectList.EmailAddress
        Me.AlternateEmail = rawAssignedProjectList.AlternateEmail
        Me.WorkMobile = rawAssignedProjectList.WorkMobile
    End Sub
    Public Property Name() As String
        Get
            Return _iname
        End Get
        Set(ByVal value As String)
            _iname = value
            NotifyPropertyChanged("Name")
        End Set
    End Property
    Public Property Position() As String
        Get
            Return _iposition
        End Get
        Set(ByVal value As String)
            _iposition = value
            NotifyPropertyChanged("Position")
        End Set
    End Property
    Public Property EmailAddress() As String
        Get
            Return _iemail
        End Get
        Set(ByVal value As String)
            _iemail = value
            NotifyPropertyChanged("EmailAddress")
        End Set
    End Property
    Public Property AlternateEmail() As String
        Get
            Return _ialternateemail
        End Get
        Set(ByVal value As String)
            _ialternateemail = value
            NotifyPropertyChanged("AlternateEmail")
        End Set
    End Property
    Public Property WorkMobile() As String
        Get
            Return _iworkmobile
        End Get
        Set(ByVal value As String)
            _iworkmobile = value
            NotifyPropertyChanged("WorkMobile")
        End Set
    End Property
    Public Property LocalNumber() As String
        Get
            Return _ilocalnumber
        End Get
        Set(ByVal value As String)
            _ilocalnumber = value
            NotifyPropertyChanged("LocalNumber")
        End Set
    End Property
    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class
