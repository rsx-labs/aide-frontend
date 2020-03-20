Imports System.ComponentModel
Public Class ReportsModel
    Private _reportid As Integer
    Private _optid As Integer
    Private _moduleid As Integer
    Private _filepath As String
    Private _decription As String
    Public Sub New()

    End Sub
    Public Sub New(ByVal _iReportsSet As myReportsSet)
        Me._reportid = _iReportsSet._reportid
        Me._optid = _iReportsSet._optid
        Me._moduleid = _iReportsSet._moduleid
        Me._filepath = _iReportsSet._filepath
        Me._decription = _iReportsSet._description
    End Sub

    Public Property REPORT_ID As Integer
        Get
            Return Me._reportid
        End Get
        Set(value As Integer)
            Me._reportid = value
            OnPropertyChanged("REPORT_ID")
        End Set
    End Property

    Public Property OPT_ID As Integer
        Get
            Return Me._optid
        End Get
        Set(value As Integer)
            Me._optid = value
            OnPropertyChanged("OPT_ID")
        End Set
    End Property

    Public Property MODULE_ID As Integer
        Get
            Return Me._moduleid
        End Get
        Set(value As Integer)
            Me._moduleid = value
            OnPropertyChanged("MODULE_ID")
        End Set
    End Property

    Public Property DESCRIPTION As String
        Get
            Return Me._decription
        End Get
        Set(value As String)
            Me._decription = value
            OnPropertyChanged("DESCRIPTION")
        End Set
    End Property

    Public Property FILE_PATH As String
        Get
            Return Me._filepath
        End Get
        Set(value As String)
            Me._filepath = value
            OnPropertyChanged("FILE_PATH")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
