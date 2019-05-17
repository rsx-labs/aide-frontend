Imports System.ComponentModel
Public Class SabaLearningModel
    Private _sabaid As Integer
    Private _empid As Integer
    Private _title As String
    Private _enddate As Date
    Private _datecompleted As String
    Private _imagepath As String

    Public Sub New()

    End Sub
    Public Sub New(ByVal _iSabaLearningSet As mySabaLearningSet)
        Me._sabaid = _iSabaLearningSet._sabaid
        Me._empid = _iSabaLearningSet._empid
        Me._title = _iSabaLearningSet._title
        Me._enddate = _iSabaLearningSet._enddate
        Me._datecompleted = _iSabaLearningSet._datecompleted
        Me._imagepath = _iSabaLearningSet._imagepath
    End Sub

    Public Property SABA_ID As Integer
        Get
            Return Me._sabaid
        End Get
        Set(value As Integer)
            Me._sabaid = value
            OnPropertyChanged("SABA_ID")
        End Set
    End Property

    Public Property EMP_ID As Integer
        Get
            Return Me._empid
        End Get
        Set(value As Integer)
            Me._empid = value
            OnPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property TITLE As String
        Get
            Return Me._title
        End Get
        Set(value As String)
            Me._title = value
            OnPropertyChanged("TITLE")
        End Set
    End Property

    Public Property END_DATE As Date
        Get
            Return Me._enddate
        End Get
        Set(value As Date)
            Me._enddate = value
            OnPropertyChanged("END_DATE")
        End Set
    End Property

    Public Property DATE_COMPLETED As String
        Get
            Return Me._datecompleted
        End Get
        Set(value As String)
            Me._datecompleted = value
            OnPropertyChanged("DATE_COMPLETED")
        End Set
    End Property

    Public Property IMAGE_PATH As String
        Get
            Return Me._imagepath
        End Get
        Set(value As String)
            Me._imagepath = value
            OnPropertyChanged("IMAGE_PATH")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
