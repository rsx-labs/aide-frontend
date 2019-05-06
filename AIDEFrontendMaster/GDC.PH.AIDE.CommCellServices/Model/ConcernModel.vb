Imports System.ComponentModel


''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class ConcernModel

    Implements INotifyPropertyChanged

    Private _refID As String
    Private _concern As String
    Private _cause As String
    Private _counterMeasure As String
    Private _empID As String
    Private _status As String
    Private _actReference As String
    Private _dateClosed As Date
    Private _dueDate As Date
    Private _gENERATEDREF_ID As String
    Private displaygenerated As String
    Private countMes As Integer
    Private _actREF As String
    Private _aCTMESSAGE As String
    Private _aCTION_REFERENCES As String
    Private _dATE1 As Date
    Private _dATE2 As Date

#Region "Property"



    Public Property REF_ID As String
        Get
            Return _refID
        End Get
        Set(value As String)
            _refID = value
            NotifyPropertyChanged("REF_ID")
        End Set
    End Property

    Public Property CONCERN As String
        Get
            Return _concern
        End Get
        Set(value As String)
            _concern = value
            NotifyPropertyChanged("CONCERN")
        End Set
    End Property

    Public Property CAUSE As String
        Get
            Return _cause
        End Get
        Set(value As String)
            _cause = value
            NotifyPropertyChanged("CAUSE")
        End Set
    End Property

    Public Property COUNTERMEASURE As String
        Get
            Return _counterMeasure
        End Get
        Set(value As String)
            _counterMeasure = value
            NotifyPropertyChanged("COUNTERMEASURE")
        End Set
    End Property

    Public Property ACT_REFERENCE As String
        Get
            Return _actReference
        End Get
        Set(value As String)
            _actReference = value
            NotifyPropertyChanged("ACT_REFERENCE")
        End Set
    End Property

    Public Property STATUS As String
        Get
            Return _status
        End Get
        Set(value As String)
            _status = value
            NotifyPropertyChanged("STATUS")
        End Set
    End Property

    Public Property EMP_ID As String
        Get
            Return _empID
        End Get
        Set(value As String)
            _empID = value
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property DUE_DATE As Date
        Get
            Return _dueDate
        End Get
        Set(value As Date)
            _dueDate = value
            NotifyPropertyChanged("DUE_DATE")
        End Set
    End Property



    Public Property GENERATEDREF_ID As String
        Get
            Return _gENERATEDREF_ID
        End Get
        Set(value As String)
            _gENERATEDREF_ID = value
            NotifyPropertyChanged("GENERATEDREF_ID")
        End Set
    End Property



    Public Property ACTREF As String
        Get
            Return _actREF
        End Get
        Set(value As String)
            _actREF = value
            NotifyPropertyChanged("ACTREF")
        End Set
    End Property

    Public Property ACT_MESSAGE As String
        Get
            Return _aCTMESSAGE
        End Get
        Set(value As String)
            _aCTMESSAGE = value
            NotifyPropertyChanged("ACT_MESSAGE")
        End Set
    End Property



    Public Property ACTION_REFERENCES As String
        Get
            Return _aCTION_REFERENCES
        End Get
        Set(value As String)
            _aCTION_REFERENCES = value
            NotifyPropertyChanged("ACTION_REFERENCES")
        End Set
    End Property


    Public Property DATE1 As Date
        Get
            Return _dATE1
        End Get
        Set(value As Date)
            _dATE1 = value
            NotifyPropertyChanged("DATE1")
        End Set
    End Property



    Public Property DATE2 As Date
        Get
            Return _dATE2
        End Get
        Set(value As Date)
            _dATE2 = value
            NotifyPropertyChanged("DATE2")
        End Set
    End Property





#End Region


    Public Sub New(ByVal rawConcernList As MyConcern)

        Me.REF_ID = rawConcernList.RefID
        Me.CONCERN = rawConcernList.Concern
        Me.CAUSE = rawConcernList.Cause
        Me.COUNTERMEASURE = rawConcernList.CounterMeasure
        Me.ACT_REFERENCE = rawConcernList.Act_Reference
        Me.EMP_ID = rawConcernList.EmpID
        Me.DUE_DATE = rawConcernList.Due_Date
        Me.STATUS = rawConcernList.Status
        Me.ACT_MESSAGE = rawConcernList.ActMessage
        Me.ACTREF = rawConcernList.ActRef


    End Sub



    Public Sub New(ByVal rawConcernList As MyGeneratedRefNo)
        Me.GENERATEDREF_ID = rawConcernList.GENERATEDREF_ID
    End Sub


    Public Sub New(ByVal rawConcernList As MyActionReference)

        Me.ACTION_REFERENCES = rawConcernList.Action_References
        Me.ACTREF = rawConcernList.ACTREF

    End Sub


    Public Sub New()

    End Sub

    Public Function ToMyConcern() As MyConcern
        Return New MyConcern() With {.RefID = Me.REF_ID, .Concern = Me.CONCERN, .Cause = Me.CAUSE, .CounterMeasure = Me.COUNTERMEASURE, .Act_Reference = Me.ACT_REFERENCE, .Status = Me.STATUS, .EmpID = Me.EMP_ID, .Due_Date = Me.DUE_DATE}
    End Function


    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
