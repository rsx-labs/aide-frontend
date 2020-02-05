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
    Private _auditSchedMonth As String
    Private _auditweekDays As String
    Private _auditnickname As String
    Private _auditweekDate As String
    Private _auditDt_Checked As String
    Private _auditDt_CheckfLG As Integer
    Private _auditqesution_Number As Integer

    Private _auditquestion_id As Integer
    Private _audit_daily_id As Integer
    Private _auditemp_id As Integer
    Private _auditFyWeek As Integer
    Private _week_date_sched As String
    Private _date_checked As String
    Public Sub New()
    End Sub

    Public Sub New(ByVal rawWorkplaceAudit As MyWorkplaceAudit)
        Me.AUDIT_QUESTIONS = rawWorkplaceAudit.AUDIT_QUESTIONS
        Me.OWNER = rawWorkplaceAudit.OWNER
        Me.AUDIT_QUESTIONS_GROUP = rawWorkplaceAudit.AUDIT_QUESTIONS_GROUP
        Me._auditSchedMonth = rawWorkplaceAudit.AUDITSCHED_MONTH
        Me._auditweekDays = rawWorkplaceAudit.WEEKDAYS
        Me._auditnickname = rawWorkplaceAudit.NICKNAME
        Me._auditweekDate = rawWorkplaceAudit.WEEKDATE
        Me._auditDt_Checked = rawWorkplaceAudit.DT_CHECKED
        Me._auditDt_CheckfLG = rawWorkplaceAudit.DT_CHECK_FLG
        _auditweekDate = rawWorkplaceAudit.WEEKDATE
        _auditquestion_id = rawWorkplaceAudit.AUDIT_QUESTIONS_ID
        _audit_daily_id = rawWorkplaceAudit.AUDIT_DAILY_ID
        _auditemp_id = rawWorkplaceAudit.EMP_ID
        _auditFyWeek = rawWorkplaceAudit.FY_WEEK
        _week_date_sched = rawWorkplaceAudit.weekdatesched
        _date_checked = rawWorkplaceAudit.date_checked
    End Sub
    Public Sub New(ByVal _myFiscalYearSet As myAuditSchedMonthSet)
        _auditSchedMonth = _myFiscalYearSet._auditSchedMonth
        _auditFyWeek = _myFiscalYearSet._fy_week
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
    Public Property AUDITSCHED_MONTH As String
        Get
            Return _auditSchedMonth
        End Get
        Set(value As String)
            _auditSchedMonth = value
            NotifyPropertyChanged("AUDITSCHED_MONTH")
        End Set
    End Property
    Public Property WEEKDAYS As String
        Get
            Return _auditweekDays
        End Get
        Set(value As String)
            _auditweekDays = value
            NotifyPropertyChanged("WEEKDAYS")
        End Set
    End Property
    Public Property NICKNAME As String
        Get
            Return _auditnickname
        End Get
        Set(value As String)
            _auditnickname = value
            NotifyPropertyChanged("NICKNAME ")
        End Set
    End Property
    Public Property WEEKDATE As String
        Get
            Return _auditweekDate
        End Get
        Set(value As String)
            _auditweekDate = value
            NotifyPropertyChanged("WEEKDATE")
        End Set
    End Property
    Public Property DT_CHECKED As String
        Get
            Return _auditDt_Checked
        End Get
        Set(value As String)
            _auditDt_Checked = value
            NotifyPropertyChanged("DT_CHECKED")
        End Set
    End Property
    Public Property DT_CHECK_FLG As Integer
        Get
            Return _auditDt_CheckfLG
        End Get
        Set(value As Integer)
            _auditDt_CheckfLG = value
            NotifyPropertyChanged("DT_CHECK_FLG")
        End Set
    End Property
    Public Property QUESTION_NUMBER As Integer
        Get
            Return _auditqesution_Number
        End Get
        Set(value As Integer)
            _auditqesution_Number = value
            NotifyPropertyChanged("QUESTION_NUMBER")
        End Set
    End Property


    Public Property AUDIT_QUESTIONS_ID As String
        Get
            Return _auditquestion_id
        End Get
        Set(value As String)
            _auditquestion_id = value
            NotifyPropertyChanged("AUDIT_QUESTIONS_ID")
        End Set
    End Property
    Public Property AUDIT_DAILY_ID As Integer
        Get
            Return _audit_daily_id
        End Get
        Set(value As Integer)
            _audit_daily_id = value
            NotifyPropertyChanged("AUDIT_DAILY_ID")
        End Set
    End Property
    Public Property EMP_ID As String
        Get
            Return _auditemp_id
        End Get
        Set(value As String)
            _auditemp_id = value
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property FY_WEEK As Integer
        Get
            Return _auditFyWeek
        End Get
        Set(value As Integer)
            _auditFyWeek = value
            NotifyPropertyChanged("FY_WEEK")
        End Set
    End Property
    Public Property WEEKDATESCHED As String
        Get
            Return _week_date_sched
        End Get
        Set(value As String)
            _week_date_sched = value
            NotifyPropertyChanged("WEEKDATESCHED")
        End Set
    End Property
    Public Property DATE_CHECKED As String
        Get
            Return _date_checked
        End Get
        Set(value As String)
            _date_checked = value
            NotifyPropertyChanged("DATE_CHECKED")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class