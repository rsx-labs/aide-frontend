Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class CommendationModel
    Implements INotifyPropertyChanged

    Private _commendID As Integer
    Private _deptID As Integer
    Private _employee As String
    Private _project As String
    Private _dateSent As Date
    Private _sentBy As String
    Private _reason As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawSuccessRegister As MyCommendations)
        Me.CommendID = rawSuccessRegister.COMMEND_ID
        Me.Dept_ID = rawSuccessRegister.DEPT_ID
        Me.Employees = rawSuccessRegister.EMPLOYEE
        Me.DateSent = rawSuccessRegister.DATE_SENT
        Me.Project = rawSuccessRegister.PROJECT
        Me.SentBy = rawSuccessRegister.SENT_BY
        Me.Reason = rawSuccessRegister.REASON
    End Sub

    Public Property CommendID As Integer
        Get
            Return _commendID
        End Get
        Set(value As Integer)
            _commendID = value
            NotifyPropertyChanged("CommendID")
        End Set
    End Property

    Public Property Dept_ID As Integer
        Get
            Return _deptID
        End Get
        Set(value As Integer)
            _deptID = value
            NotifyPropertyChanged("Dept_ID")
        End Set
    End Property

    Public Property Employees As String
        Get
            Return _employee
        End Get
        Set(value As String)
            _employee = value
            NotifyPropertyChanged("Employees")
        End Set
    End Property

    Public Property DateSent As Date
        Get
            _dateSent = _dateSent.ToString("d")
            Return _dateSent
        End Get
        Set(value As Date)
            value = value.ToString("d")
            _dateSent = value
            NotifyPropertyChanged("DateSent")
        End Set
    End Property

    Public Property Project As String
        Get
            Return _project
        End Get
        Set(value As String)
            _project = value
            NotifyPropertyChanged("Project")
        End Set
    End Property

    Public Property SentBy As String
        Get
            Return _sentBy
        End Get
        Set(value As String)
            _sentBy = value
            NotifyPropertyChanged("SentBy")
        End Set
    End Property

    Public Property Reason As String
        Get
            Return _reason
        End Get
        Set(value As String)
            _reason = value
            NotifyPropertyChanged("Reason")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
