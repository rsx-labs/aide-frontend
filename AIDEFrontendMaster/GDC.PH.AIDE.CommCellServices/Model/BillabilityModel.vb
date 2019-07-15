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
        Me.NAME = raw.Name
        Me.HOURS = raw.Hours
        Me.STATUS = raw.Status
    End Sub

#Region "Properties"

    Private _name As String
    Public Property NAME As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
            NotifyPropertyChanged("NAME")
        End Set
    End Property

    Private _hours As Double
    Public Property HOURS As Double
        Get
            Return _hours
        End Get
        Set(ByVal value As Double)
            _hours = value
            NotifyPropertyChanged("HOURS")
        End Set
    End Property

    Private _status As Double
    Public Property STATUS As Double
        Get
            Return _status
        End Get
        Set(ByVal value As Double)
            _status = value
            NotifyPropertyChanged("STATUS")
        End Set
    End Property

#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
