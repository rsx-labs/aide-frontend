Imports System.ComponentModel
Public Class ComcellClockModel
    Private _clockDay As Integer
    Private _clockHour As Integer
    Private _clockMinute As Integer
    Private _empID As Integer

    Public Sub New()

    End Sub

    Public Sub New(ByVal _iclockSet As myComcellClockSet)
        Me._clockDay = _iclockSet._clockDay
        Me._clockHour = _iclockSet._clockHour
        Me._clockMinute = _iclockSet._clockMinute
        Me._empID = _iclockSet._empID
    End Sub

    Public Property CLOCK_DAY As Integer
        Get
            Return _clockDay
        End Get
        Set(value As Integer)
            _clockDay = value
            OnPropertyChanged("CLOCK_DAY")
        End Set
    End Property

    Public Property CLOCK_HOUR As Double
        Get
            Return _clockHour
        End Get
        Set(value As Double)
            _clockHour = value
            OnPropertyChanged("CLOCK_HOUR")
        End Set
    End Property

    Public Property CLOCK_MINUTE As Double
        Get
            Return _clockMinute
        End Get
        Set(value As Double)
            _clockMinute = value
            OnPropertyChanged("CLOCK_MINUTE")
        End Set
    End Property

    Public Property EMP_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            OnPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
Public Class ComcellDay
    Private _clockIntoDay As String
    Private _clockSetAlarm As String

    Public Sub New()

    End Sub

    Public Sub New(_day As String)
        Me._clockIntoDay = _day
    End Sub


    Public Property CLOCK_INTO_DAY As String
        Get
            Return _clockIntoDay
        End Get
        Set(value As String)
            _clockIntoDay = value
            OnPropertyChanged("CLOCK_INTO_DAY")
        End Set
    End Property

    Public Property CLOCK_SET_ALARM As String
        Get
            Return _clockSetAlarm
        End Get
        Set(value As String)
            _clockSetAlarm = value
            OnPropertyChanged("CLOCK_SET_ALARM")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
