Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.IO.Directory

Public Class BillabilityModel
    Implements INotifyPropertyChanged

#Region "Data Members"

    Private _key As Integer
    Private _value As String

#End Region

    Public Sub New()
    End Sub

    Public Sub New(ByVal raw As MyBillability)
        Me.EMPID = raw.EMPID
        Me.NICK_NAME = raw.NICK_NAME
        Me.VL = raw.VL
        Me.SL = raw.SL
        Me.HOLIDAY = raw.HOLIDAY
        Me.HALFDAY = raw.HALFDAY
        Me.HALFSL = raw.HALFSL
        Me.HALFVL = raw.HALFVL
        Me.TOTAL = raw.TOTAL
    End Sub

#Region "Properties"

    Private _empID As Integer
    Public Property EMPID() As Integer
        Get
            Return _empID
        End Get
        Set(ByVal value As Integer)
            _empID = value
            NotifyPropertyChanged("EMPID")
        End Set
    End Property

    Private _name As String
    Public Property NICK_NAME() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
            NotifyPropertyChanged("NICK_NAME")
        End Set
    End Property

    Private _vl As Double
    Public Property VL() As Double
        Get
            Return _vl
        End Get
        Set(ByVal value As Double)
            _vl = value
            NotifyPropertyChanged("VL")
        End Set
    End Property

    Private _sl As Double
    Public Property SL() As Double
        Get
            Return _sl
        End Get
        Set(ByVal value As Double)
            _sl = value
            NotifyPropertyChanged("SL")
        End Set
    End Property

    Private _holiday As Double
    Public Property HOLIDAY() As Double
        Get
            Return _holiday
        End Get
        Set(ByVal value As Double)
            _holiday = value
            NotifyPropertyChanged("HOLIDAY")
        End Set
    End Property

    Private _halfday As Double
    Public Property HALFDAY() As Double
        Get
            Return _halfday
        End Get
        Set(ByVal value As Double)
            _halfday = value
            NotifyPropertyChanged("HALFDAY")
        End Set
    End Property

    Private _halfdayVL As Double
    Public Property HALFVL() As Double
        Get
            Return _halfdayVL
        End Get
        Set(ByVal value As Double)
            _halfdayVL = value
            NotifyPropertyChanged("HALFVL")
        End Set
    End Property

    Private _halfdaySL As Double
    Public Property HALFSL() As Double
        Get
            Return _halfdaySL
        End Get
        Set(ByVal value As Double)
            _halfdaySL = value
            NotifyPropertyChanged("HALFSL")
        End Set
    End Property

    Private _total As Double
    Public Property TOTAL() As Double
        Get
            Return _total
        End Get
        Set(ByVal value As Double)
            _total = value
            NotifyPropertyChanged("TOTAL")
        End Set
    End Property


#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
