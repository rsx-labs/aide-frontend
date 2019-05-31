Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class LateModel
    Implements INotifyPropertyChanged

    Private _fName As String
    Private _status As Integer
    Private _dateEntry As Date
    Private _month As String
    Private _noOfLate As Integer

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawLate As MyLate)
        Me.FIRST_NAME = rawLate.FIRST_NAME
        Me.DATE_ENTRY = rawLate.DATE_ENTRY
        Me.STATUS = rawLate.STATUS
        Me.MONTH = rawLate.MONTH
        Me.NUMBER_OF_LATE = rawLate.NO_OF_LATE
    End Sub

    Public Property FIRST_NAME As String
        Get
            Return _fName
        End Get
        Set(value As String)
            _fName = value
            NotifyPropertyChanged("FIRST_NAME")
        End Set
    End Property

    Public Property DATE_ENTRY As DateTime
        Get
            Return _dateEntry
        End Get
        Set(value As DateTime)
            _dateEntry = value
            NotifyPropertyChanged("DATE_ENTRY")
        End Set
    End Property

    Public Property STATUS As Integer
        Get
            Return _status
        End Get
        Set(value As Integer)
            _status = value
            NotifyPropertyChanged("STATUS")
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

    Public Property NUMBER_OF_LATE As Integer
        Get
            Return _noOfLate
        End Get
        Set(value As Integer)
            _noOfLate = value
            NotifyPropertyChanged("NUMBER_OF_LATE")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
