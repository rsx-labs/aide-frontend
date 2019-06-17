Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class WorkplaceAuditModel
    Implements INotifyPropertyChanged

    Private _mon As Boolean
    Private _tue As Boolean
    Private _wed As Boolean
    Private _thurs As Boolean
    Private _fri As Boolean
    Private _auditQuestions As String
    Private _owner As String
    Private _auditQuestionsGroup As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawWorkplaceAudit As MyWorkplaceAudit)
        Me.AUDIT_QUESTIONS = rawWorkplaceAudit.AUDIT_QUESTIONS
        Me.OWNER = rawWorkplaceAudit.OWNER
        Me.AUDIT_QUESTIONS_GROUP = rawWorkplaceAudit.AUDIT_QUESTIONS_GROUP
    End Sub

    Public Property MONDAY As Boolean
        Get
            Return _mon
        End Get
        Set(value As Boolean)
            _mon = value
        End Set
    End Property

    Public Property TUESDAY As Boolean
        Get
            Return _tue
        End Get
        Set(value As Boolean)
            _tue = value
        End Set
    End Property

    Public Property WEDNESDAY As Boolean
        Get
            Return _wed
        End Get
        Set(value As Boolean)
            _wed = value
        End Set
    End Property

    Public Property THURSDAY As Boolean
        Get
            Return _thurs
        End Get
        Set(value As Boolean)
            _thurs = value
        End Set
    End Property

    Public Property FRIDAY As Boolean
        Get
            Return _fri
        End Get
        Set(value As Boolean)
            _fri = value
        End Set
    End Property

    Public Property AUDIT_QUESTIONS As String
        Get
            Return _auditQuestions
        End Get
        Set(value As String)
            _auditQuestions = value
            NotifyPropertyChanged("AUDIT_QUESTIONS")
        End Set
    End Property

    Public Property OWNER As String
        Get
            Return _owner
        End Get
        Set(value As String)
            _owner = value
            NotifyPropertyChanged("OWNER")
        End Set
    End Property

    Public Property AUDIT_QUESTIONS_GROUP As String
        Get
            Return _auditQuestionsGroup
        End Get
        Set(value As String)
            _auditQuestionsGroup = value
            NotifyPropertyChanged("AUDIT_QUESTIONS_GROUP")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class