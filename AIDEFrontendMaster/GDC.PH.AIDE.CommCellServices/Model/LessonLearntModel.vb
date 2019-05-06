Imports System.ComponentModel

''' <summary>
''' By John Harvey Sanchez
''' </summary>
Public Class LessonLearntModel
    Implements INotifyPropertyChanged

    Private _refNo As String
    Private _empID As Integer
    Private _nickname As String
    Private _problem As String
    Private _resolution As String
    Private _actionNo As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal rawLessonLearntList As MyLessonLearntList)
        Me.ReferenceNo = rawLessonLearntList.ReferenceNo
        Me.EmployeeID = rawLessonLearntList.EmpID
        Me.Nickname = rawLessonLearntList.Nickname
        Me.Problem = rawLessonLearntList.Problem
        Me.Resolution = rawLessonLearntList.Resolution
        Me.ActionNo = rawLessonLearntList.ActionNo
    End Sub

    Public Property ReferenceNo As String
        Get
            Return _refNo
        End Get
        Set(value As String)
            _refNo = value
            NotifyPropertyChanged("ReferenceNo")
        End Set
    End Property

    Public Property EmployeeID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            NotifyPropertyChanged("EmployeeID")
        End Set
    End Property

    Public Property Nickname As String
        Get
            Return _nickname
        End Get
        Set(value As String)
            _nickname = value
            NotifyPropertyChanged("Nickname")
        End Set
    End Property

    Public Property Problem As String
        Get
            Return _problem
        End Get
        Set(value As String)
            _problem = value
            NotifyPropertyChanged("Problem")
        End Set
    End Property

    Public Property Resolution As String
        Get
            Return _resolution
        End Get
        Set(value As String)
            _resolution = value
            NotifyPropertyChanged("Resolution")
        End Set
    End Property

    Public Property ActionNo As String
        Get
            Return _actionNo
        End Get
        Set(value As String)
            _actionNo = value
            NotifyPropertyChanged("ActionNo")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
