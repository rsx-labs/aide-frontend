Imports System.ComponentModel
Public Class KPISummaryModel
    Private _Id As Integer
    Private _fystart As Date
    Private _fyend As Date
    Private _KPI_refNo As String
    Private _month As Short
    Private _target As Double
    Private _actual As Double
    Private _overall As Double
    Private _description As String
    Private _subject As String
    Private _datePosted As Date

    Public Sub New()
    End Sub

    Public Sub New(ByVal _kpiSummary As KPISummaryData)
        Me.ID = _kpiSummary._ID
        Me.FYStart = _kpiSummary._FYStart
        Me.FYEnd = _kpiSummary._FYEnd
        Me.Month = _kpiSummary._Month
        Me.Target = _kpiSummary.KPI_Target
        Me.Actual = _kpiSummary.KPI_Actual
        Me.Overall = _kpiSummary.KPI_Overall
        Me.KPIReferenceNo = _kpiSummary._KPI_RefNo
        Me.Description = _kpiSummary._description
        Me.Subject = _kpiSummary._subject
        Me.DatePosted = _kpiSummary._datePosted
    End Sub

    Public Property ID As Integer
        Get
            Return _Id
        End Get
        Set(value As Integer)
            _Id = value
            OnPropertyChanged("ID")
        End Set
    End Property

    Public Property FYStart As Date
        Get
            Return _fystart
        End Get
        Set(value As Date)
            _fystart = value
            OnPropertyChanged("FYStart")
        End Set
    End Property

    Public Property FYEnd As Date
        Get
            Return _fyend
        End Get
        Set(value As Date)
            _fyend = value
            OnPropertyChanged("FYEnd")
        End Set
    End Property

    Public Property Month As Short
        Get
            Return _month
        End Get
        Set(value As Short)
            _month = value
            OnPropertyChanged("Month")
        End Set
    End Property


    Public Property Target As Double
        Get
            Return _target
        End Get
        Set(value As Double)
            _target = value
            OnPropertyChanged("Target")
        End Set
    End Property

    Public Property Actual As Double
        Get
            Return _actual
        End Get
        Set(value As Double)
            _actual = value
            OnPropertyChanged("Actual")
        End Set
    End Property

    Public Property Overall As Double
        Get
            Return _overall
        End Get
        Set(value As Double)
            _overall = value
            OnPropertyChanged("Overall")
        End Set
    End Property
    Public Property KPIReferenceNo As String
        Get
            Return _KPI_refNo
        End Get
        Set(value As String)
            _KPI_refNo = value
            OnPropertyChanged("KPIReferenceNo")
        End Set
    End Property

    Public Property Description As String
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
            OnPropertyChanged("Description")
        End Set
    End Property

    Public Property Subject As String
        Get
            Return _subject
        End Get
        Set(value As String)
            _subject = value
            OnPropertyChanged("Subject")
        End Set
    End Property

    Public Property DatePosted As Date
        Get
            Return _datePosted
        End Get
        Set(value As Date)
            _datePosted = value
            OnPropertyChanged("DatePosted")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
