Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class ComcellModel
    Implements INotifyPropertyChanged

    Private _comcellID As Integer
    Private _empID As Integer
    Private _month As String
    Private _facilitator As String
    Private _minsTaker As String
    Private _fyStart As DateTime
    Private _fyEnd As DateTime
    Private _facilitatorName As String
    Private _minsTakerName As String
    Private _week As Integer
    Private _weekStart As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawComcell As MyComcell)
        Me.COMCELL_ID = rawComcell.COMCELL_ID
        Me.EMP_ID = rawComcell.EMP_ID
        Me.MONTH = rawComcell.MONTH
        Me.FACILITATOR = rawComcell.FACILITATOR
        Me.MINUTES_TAKER = rawComcell.MINUTES_TAKER
        Me.FY_START = rawComcell.FY_START
        Me.FY_END = rawComcell.FY_END
        Me.FACILITATOR_NAME = rawComcell.FACILITATOR_NAME
        Me.MINUTES_TAKER_NAME = rawComcell.MINUTES_TAKER_NAME
        Me.WEEK = rawComcell.WEEK
        ' lets just assume that for valid date , year should be greater that 2000
        Me.WEEK_START = If(rawComcell.WEEK_START.Year > 2000, rawComcell.WEEK_START.ToString("d"), String.Empty)
    End Sub

    Public Property COMCELL_ID As Integer
        Get
            Return _comcellID
        End Get
        Set(value As Integer)
            _comcellID = value
            NotifyPropertyChanged("COMCELL_ID")
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

    Public Property MONTH As String
        Get
            Return _month
        End Get
        Set(value As String)
            _month = value
            NotifyPropertyChanged("MONTH")
        End Set
    End Property

    Public Property FACILITATOR As String
        Get
            Return _facilitator
        End Get
        Set(value As String)
            _facilitator = value
            NotifyPropertyChanged("FACILITATOR")
        End Set
    End Property

    Public Property MINUTES_TAKER As String
        Get
            Return _minsTaker
        End Get
        Set(value As String)
            _minsTaker = value
            NotifyPropertyChanged("MINUTES_TAKER")
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

    Public Property FACILITATOR_NAME As String
        Get
            Return _facilitatorName
        End Get
        Set(value As String)
            _facilitatorName = value
            NotifyPropertyChanged("FACILITATOR_NAME")
        End Set
    End Property

    Public Property MINUTES_TAKER_NAME As String
        Get
            Return _minsTakerName
        End Get
        Set(value As String)
            _minsTakerName = value
            NotifyPropertyChanged("MINUTES_TAKER_NAME")
        End Set
    End Property

    Public Property WEEK As Integer
        Get
            Return _week
        End Get
        Set(value As Integer)
            _week = value
            NotifyPropertyChanged("WEEK")
        End Set
    End Property

    Public Property WEEK_START As String
        Get
            Return _weekStart
        End Get
        Set(value As String)
            _weekStart = value
            NotifyPropertyChanged("WEEK_START")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
