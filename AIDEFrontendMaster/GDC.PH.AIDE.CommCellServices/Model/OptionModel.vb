Imports System.ComponentModel
Public Class OptionModel
    Private _optionID As Integer
    Private _moduleID As Integer
    Private _functionID As Integer
    Private _description As String
    Private _value As String
    Private _moduleDescr As String
    Private _functionDescr As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal _myOptionSet As myOptionSet)
        Me._optionID = _myOptionSet._optionID
        Me._moduleID = _myOptionSet._moduleID
        Me._functionID = _myOptionSet._functionID
        Me._description = _myOptionSet._description
        Me._value = _myOptionSet._value
        Me._moduleDescr = _myOptionSet._moduleDescr
        Me._functionDescr = _myOptionSet._functionDescr
    End Sub
    Public Property OPTION_ID As Integer
        Get
            Return _optionID
        End Get
        Set(value As Integer)
            _optionID = value
            OnPropertyChanged("OPTION_ID")
        End Set
    End Property
    Public Property MODULE_ID As Integer
        Get
            Return _moduleID
        End Get
        Set(value As Integer)
            _moduleID = value
            OnPropertyChanged("MODULE_ID")
        End Set
    End Property
    Public Property FUNCTION_ID As Integer
        Get
            Return _functionID
        End Get
        Set(value As Integer)
            _functionID = value
            OnPropertyChanged("FUNCTION_ID")
        End Set
    End Property
    Public Property DESCRIPTION As String
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
            OnPropertyChanged("DESCRIPTION")
        End Set
    End Property
    Public Property VALUE As String
        Get
            Return _value
        End Get
        Set(value As String)
            _value = value
            OnPropertyChanged("VALUE")
        End Set
    End Property
    Public Property MODULE_DESCR As String
        Get
            Return _moduleDescr
        End Get
        Set(value As String)
            _moduleDescr = value
            OnPropertyChanged("MODULE_DESCR")
        End Set
    End Property
    Public Property FUNCTION_DESCR As String
        Get
            Return _functionDescr
        End Get
        Set(value As String)
            _functionDescr = value
            OnPropertyChanged("FUNCTION_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
