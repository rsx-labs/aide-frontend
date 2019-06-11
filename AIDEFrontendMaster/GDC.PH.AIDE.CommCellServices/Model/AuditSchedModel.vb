Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class AuditSchedModel
    Implements INotifyPropertyChanged

    Private _auditSchedID As Integer
    Private _empID As Integer
    Private _fyWeek As Integer
    Private _periodStart As DateTime
    Private _periodEnd As DateTime
    Private _daily As String
    Private _weekly As String
    Private _monthly As String
    Private _fyStart As DateTime
    Private _fyEnd As DateTime

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawComcell As MyAuditSched)
        Me.AUDIT_SCHED_ID = rawComcell.AUDIT_SCHED_ID
        Me.EMP_ID = rawComcell.EMP_ID
        Me.FY_WEEK = rawComcell.FY_WEEK
        Me.PERIOD_START = rawComcell.PERIOD_START
        Me.PERIOD_END = rawComcell.PERIOD_END
        Me.DAILY = rawComcell.DAILY
        Me.WEEKLY = rawComcell.WEEKLY
        Me.MONTHLY = rawComcell.MONTHLY
        Me.FY_START = rawComcell.FY_START
        Me.FY_END = rawComcell.FY_END
    End Sub

    Public Property AUDIT_SCHED_ID As Integer
        Get
            Return _auditSchedID
        End Get
        Set(value As Integer)
            _auditSchedID = value
            NotifyPropertyChanged("AUDIT_SCHED_ID")
        End Set
    End Property

    Public Property EMP_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property FY_WEEK As Integer
        Get
            Return _fyWeek
        End Get
        Set(value As Integer)
            _fyWeek = value
            NotifyPropertyChanged("FY_WEEK")
        End Set
    End Property

    Public Property PERIOD_START As DateTime
        Get
            Return _periodStart
        End Get
        Set(value As DateTime)
            _periodStart = value
            NotifyPropertyChanged("PERIOD_START")
        End Set
    End Property

    Public Property PERIOD_END As DateTime
        Get
            Return _periodEnd
        End Get
        Set(value As DateTime)
            _periodEnd = value
            NotifyPropertyChanged("PERIOD_END")
        End Set
    End Property

    Public Property DAILY As String
        Get
            Return _daily
        End Get
        Set(value As String)
            _daily = value
            NotifyPropertyChanged("DAILY")
        End Set
    End Property

    Public Property WEEKLY As String
        Get
            Return _weekly
        End Get
        Set(value As String)
            _weekly = value
            NotifyPropertyChanged("WEEKLY")
        End Set
    End Property

    Public Property MONTHLY As String
        Get
            Return _monthly
        End Get
        Set(value As String)
            _monthly = value
            NotifyPropertyChanged("MONTHLY")
        End Set
    End Property

    Public Property FY_START As DateTime
        Get
            Return _fyStart
        End Get
        Set(value As DateTime)
            _fyStart = value
            NotifyPropertyChanged("FY_START")
        End Set
    End Property

    Public Property FY_END As DateTime
        Get
            Return _fyEnd
        End Get
        Set(value As DateTime)
            _fyEnd = value
            NotifyPropertyChanged("FY_END")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
