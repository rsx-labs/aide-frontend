Imports System.ComponentModel
Public Class KPITargetsModel
    Private _Id As Integer
    Private _fystart As Date
    Private _fyend As Date
    Private _KPI_refNo As String
    Private _description As String
    Private _subject As String
    Private _dateCreated As Date

    Public Sub New()
    End Sub

    Public Sub New(ByVal kpiTarget As KPITargetSet)
        Me.ID = kpiTarget._ID
        Me.FYStart = kpiTarget._FYStart
        Me.FYEnd = kpiTarget._FYEnd
        Me.KPIReferenceNo = kpiTarget._KPI_RefNo
        Me.Description = kpiTarget._description
        Me.Subject = kpiTarget._subject
        Me.DateCreated = kpiTarget._dateCreated
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

    Public Property DateCreated As Date
        Get
            Return _dateCreated
        End Get
        Set(value As Date)
            _dateCreated = value
            OnPropertyChanged("DateCreated")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
