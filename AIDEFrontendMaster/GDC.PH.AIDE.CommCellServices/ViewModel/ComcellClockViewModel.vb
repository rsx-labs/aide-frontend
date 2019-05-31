Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class ComcellClockViewModel
    Implements INotifyPropertyChanged
    Private _objClockDayOnly As New ComcellDay
    Private _objectClockSet As New ComcellClockModel
    Private clockprov As New ComcellClockDBProvider

    Sub New()
        _objectClockSet = New ComcellClockModel(clockprov._getobjClock)
    End Sub

    Public Property objectComcellClockSet As ComcellClockModel
        Get
            Return _objectClockSet
        End Get
        Set(value As ComcellClockModel)
            _objectClockSet = value
            NotifyPropertyChanged("objectComcellClockSet")
        End Set
    End Property
    Public Property objectComcellDayOnly As String
        Get
            Return _objClockDayOnly.CLOCK_INTO_DAY
        End Get
        Set(value As String)
            _objClockDayOnly.CLOCK_INTO_DAY = value
            NotifyPropertyChanged("objectComcellDayOnly")
        End Set
    End Property

    Public Property objectComcellSetAlarm As String
        Get
            Return _objClockDayOnly.CLOCK_SET_ALARM
        End Get
        Set(value As String)
            _objClockDayOnly.CLOCK_SET_ALARM = value
            NotifyPropertyChanged("objectComcellSetAlarm")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class



