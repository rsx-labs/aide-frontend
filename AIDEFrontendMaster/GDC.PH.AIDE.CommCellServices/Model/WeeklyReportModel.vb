Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class WeeklyReportModel
    Implements INotifyPropertyChanged

    Private _dateStarted As Date
    Private _dateTarget As Date
    Private _dateFinished As Date

    Private _effortEst As Double
    Private _actualEffort As Double
    Private _actualEffortWk As Double

    Private _taskID As Integer
    Private _wkRangeID As Integer
    Private _refID As String
    Private _projID As Integer
    Private _projCode As Integer
    Private _empID As Integer

    Private _subject As String
    Private _comments As String
    Private _inboundContacts As Short

    Private _severity As Short
    Private _incType As Short
    Private _status As Short
    Private _rework As Short
    Private _phase As Short

    Private _projDesc As String
    Private _severityDesc As String
    Private _incDesc As String
    Private _statusDesc As String
    Private _reworkDesc As String
    Private _phaseDesc As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawTasks As MyWeeklyReport)
        Me.WeekID = aRawTasks.WeekID
        Me.WeekRangeID = aRawTasks.WeekRangeID
        Me.ProjectID = aRawTasks.ProjectID  'set equivalent to projID because there is no field for projectcode
        Me.Rework = aRawTasks.Rework
        Me.RefID = aRawTasks.RefID
        Me.Subject = aRawTasks.Subject
        Me.Severity = aRawTasks.Severity
        Me.IncidentType = aRawTasks.IncType
        Me.EmpID = aRawTasks.EmpID
        Me.Phase = aRawTasks.Phase
        Me.Status = aRawTasks.Status
        Me.DateStarted = aRawTasks.DateStarted
        Me.DateTarget = aRawTasks.DateTarget
        Me.DateFinished = aRawTasks.DateFinished
        Me.EffortEst = aRawTasks.EffortEst
        Me.ActualEffort = aRawTasks.ActEffort
        Me.ActualEffortWk = aRawTasks.ActEffortWk
        Me.Comments = aRawTasks.Comment
        Me.InboundContacts = aRawTasks.InboundContacts
        Me.ProjectCode = aRawTasks.ProjectCode
        Me.TaskID = aRawTasks.TaskID
    End Sub

    Public Function ToMyTasks() As MyWeeklyReport
        ToMyTasks = New MyWeeklyReport() With {
                                       .WeekID = Me.WeekID,
                                       .WeekRangeID = Me.WeekRangeID,
                                       .ProjectID = Me.ProjectID,
                                       .Rework = Me.Rework,
                                       .RefID = Me.RefID,
                                       .Subject = Me.Subject,
                                       .Severity = Me.Severity,
                                       .IncType = Me.IncidentType,
                                       .EmpID = Me.EmpID,
                                       .Phase = Me.Phase,
                                       .Status = Me.Status,
                                       .DateStarted = Me.DateStarted,
                                       .DateTarget = Me.DateTarget,
                                       .DateFinished = Me.DateFinished,
                                       .DateCreated = Date.Now,
                                       .EffortEst = Me.EffortEst,
                                       .ActEffort = Me.ActualEffort,
                                       .ActEffortWk = Me.ActualEffortWk,
                                       .Comment = Me.Comments,
                                       .InboundContacts = Me.InboundContacts,
                                       .ProjectCode = Me.ProjectCode,
                                       .TaskID = Me.TaskID}
    End Function

    Public Property WeekID As Integer

    Public Property WeekRangeID As Integer
        Get
            Return _wkRangeID
        End Get
        Set(value As Integer)
            _wkRangeID = value
            NotifyPropertyChanged("WeekRangeID")
        End Set
    End Property

    Public Property ProjectID As Integer
        Get
            Return _projID
        End Get
        Set(value As Integer)
            _projID = value
            NotifyPropertyChanged("ProjectID")
        End Set
    End Property

    Public Property ProjectDesc As String
        Get
            Return _projDesc
        End Get
        Set(value As String)
            _projDesc = value
            NotifyPropertyChanged("ProjectDesc")
        End Set
    End Property

    Public Property Rework As Short
        Get
            Return _rework
        End Get
        Set(value As Short)
            _rework = value
            NotifyPropertyChanged("Rework")
        End Set
    End Property

    Public Property ReworkDesc As String
        Get
            Return _reworkDesc
        End Get
        Set(value As String)
            _reworkDesc = value
            NotifyPropertyChanged("ReworkDesc")
        End Set
    End Property

    Public Property RefID As String
        Get
            Return _refID
        End Get
        Set(value As String)
            _refID = value
            NotifyPropertyChanged("RefID")
        End Set
    End Property

    Public Property Subject As String
        Get
            Return _subject
        End Get
        Set(value As String)
            _subject = value
            NotifyPropertyChanged("Subject")
        End Set
    End Property

    Public Property Severity As Short
        Get
            Return _severity
        End Get
        Set(value As Short)
            _severity = value
            NotifyPropertyChanged("Severity")
        End Set
    End Property

    Public Property SeverityDesc As String
        Get
            Return _severityDesc
        End Get
        Set(value As String)
            _severityDesc = value
            NotifyPropertyChanged("SeverityDesc")
        End Set
    End Property

    Public Property IncidentType As Short
        Get
            Return _incType
        End Get
        Set(value As Short)
            _incType = value
            NotifyPropertyChanged("IncidentType")
        End Set
    End Property

    Public Property IncidentDesc As String
        Get
            Return _incDesc
        End Get
        Set(value As String)
            _incDesc = value
            NotifyPropertyChanged("IncidentDesc")
        End Set
    End Property

    Public Property EmpID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            NotifyPropertyChanged("EmpID")
        End Set
    End Property
   
    Public Property Phase As Short
        Get
            Return _phase
        End Get
        Set(value As Short)
            _phase = value
            NotifyPropertyChanged("Phase")
        End Set
    End Property

    Public Property PhaseDesc As String
        Get
            Return _phaseDesc
        End Get
        Set(value As String)
            _phaseDesc = value
            NotifyPropertyChanged("PhaseDesc")
        End Set
    End Property

    Public Property Status As Short
        Get
            Return _status
        End Get
        Set(value As Short)
            _status = value
            NotifyPropertyChanged("Status")
        End Set
    End Property

    Public Property StatusDesc As String
        Get
            Return _statusDesc
        End Get
        Set(value As String)
            _statusDesc = value
            NotifyPropertyChanged("StatusDesc")
        End Set
    End Property

    Public Property DateStarted As String
        Get
            If _dateStarted = Nothing Then
                Return String.Empty
            Else
                Return _dateStarted.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _dateStarted = Nothing
            Else
                _dateStarted = CDate(value)
            End If
            NotifyPropertyChanged("DateStarted")
        End Set
    End Property

    Public Property DateTarget As String
        Get
            If _dateTarget  = Nothing Then
                Return String.Empty
            Else
                Return _dateTarget.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _dateTarget = Nothing
            Else
                _dateTarget = CDate(value)
            End If
            NotifyPropertyChanged("DateTarget")
        End Set
    End Property

    Public Property DateFinished As String
        Get
            If _dateFinished = Nothing Then
                Return String.Empty
            Else
                Return _dateFinished.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _dateFinished = Nothing
            Else
                _dateFinished = CDate(value)
            End If
            NotifyPropertyChanged("DateFinished")
        End Set
    End Property

    Public Property EffortEst As String
        Get
            If _effortEst = Nothing Then
                Return "0"
            Else
                Return _effortEst.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _effortEst = Nothing
            Else
                _effortEst = CDbl(value)
            End If
            NotifyPropertyChanged("EffortEst")
        End Set
    End Property

    Public Property ActualEffort As String
        Get
            If _actualEffort = Nothing Then
                Return "0"
            Else
                Return _actualEffort.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _actualEffort = Nothing
            Else
                _actualEffort = CDbl(value)
            End If
            NotifyPropertyChanged("ActualEffort")
        End Set
    End Property

    Public Property ActualEffortWk As String
        Get
            If _actualEffortWk = Nothing Then
                Return "0"
            Else
                Return _actualEffortWk.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _actualEffortWk = Nothing
            Else
                _actualEffortWk = CDbl(value)
            End If
            NotifyPropertyChanged("ActualEffortWk")
        End Set
    End Property

    Public Property Comments As String
        Get
            Return _comments
        End Get
        Set(value As String)
            _comments = value
            NotifyPropertyChanged("Comments")
        End Set
    End Property
    Public ReadOnly Property IncidentImagePath As String
        Get
            If _incType = 1 Then
                Return "..\..\..\Assets\Icon\EnhLogo.png"
            ElseIf _incType = 2 Then
                Return "..\..\..\Assets\Icon\BugLogo.png"
            Else
                Return "..\..\..\Assets\Icon\TaskLogo.png"
            End If
        End Get
    End Property

    Public Property InboundContacts As String
        Get
            If _inboundContacts = Nothing Then
                Return String.Empty
            Else
                Return _inboundContacts.ToString
            End If
            Return _inboundContacts
        End Get
        Set(value As String)
            If value = Nothing Then
                _inboundContacts = Nothing
            Else
                _inboundContacts = CInt(value)
            End If
            NotifyPropertyChanged("InboundContacts")
        End Set
    End Property

    Public Property ProjectCode As Integer
        Get
            Return _projCode
        End Get
        Set(value As Integer)
            _projCode = value
            NotifyPropertyChanged("ProjectCode")
        End Set
    End Property

    Public Property TaskID As Integer
        Get
            Return _taskID
        End Get
        Set(value As Integer)
            _taskID = value
            NotifyPropertyChanged("TaskID")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class

