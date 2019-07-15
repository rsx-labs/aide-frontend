Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class TasksModel
    Implements INotifyPropertyChanged

    Private _dateStarted As Date
    Private _targetDate As Date
    Private _compltdDate As Date

    Private _effortEst As Double
    Private _actEffort As Double
    Private _actEffortWk As Double

    Private _taskID As Integer
    Private _refID As String
    Private _projID As Integer
    Private _projCode As Integer

    Private _incDescr As String
    Private _comments As String

    Private _incType As Short
    Private _status As Short
    Private _severity As Short
    Private _rework As Short
    Private _phase As Short

    Private _severityDesc As String
    Private _incDesc As String
    Private _statusDesc As String
    Private _reworkDesc As String
    Private _phaseDesc As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawTasks As MyTasks)
        Me.TaskId = aRawTasks.TaskId
        Me.ProjId = aRawTasks.ProjId
        Me.ProjectCode = aRawTasks.ProjectCode
        Me.Rework = aRawTasks.Rework
        Me.ReferenceID = aRawTasks.ReferenceID
        Me.IncDescr = aRawTasks.IncDescr
        Me.Severity = aRawTasks.Severity
        Me.IncidentType = aRawTasks.IncidentType
        Me.EmpId = aRawTasks.EmpId
        Me.Phase = aRawTasks.Phase
        Me.Status = aRawTasks.Status
        Me.DateStarted = aRawTasks.DateStarted
        Me.TargetDate = aRawTasks.TargetDate
        Me.CompltdDate = aRawTasks.CompltdDate
        Me.DateCreated = aRawTasks.DateCreated
        Me.ActEffort = aRawTasks.ActEffort
        Me.ActEffortWk = aRawTasks.ActEffortWk
        Me.EffortEst = aRawTasks.EffortEst
        Me.Comments = aRawTasks.Comments
        Me.Others1 = aRawTasks.Others1
        Me.Others2 = aRawTasks.Others2
        Me.Others3 = aRawTasks.Others3
    End Sub

    Public Function ToMyTasks() As MyTasks
        ToMyTasks = New MyTasks() With {.TaskId = Me.TaskId,
                                        .ProjId = Me.ProjId,
                                        .ProjectCode = Me.ProjectCode,
                                        .Rework = Me.Rework,
                                        .ReferenceID = Me.ReferenceID,
                                        .IncDescr = Me.IncDescr,
                                        .Severity = Me.Severity,
                                        .IncidentType = Me.IncidentType,
                                        .EmpId = Me.EmpId,
                                        .Phase = Me.Phase,
                                        .Status = Me.Status,
                                        .DateStarted = Me.DateStarted,
                                        .TargetDate = Me.TargetDate,
                                        .CompltdDate = Me.CompltdDate,
                                        .DateCreated = Date.Now,
                                        .EffortEst = Me.EffortEst,
                                        .ActEffort = Me.ActEffort,
                                        .ActEffortWk = Me.ActEffortWk,
                                        .Comments = Me.Comments,
                                        .Others1 = Me.Others1,
                                        .Others2 = Me.Others2,
                                        .Others3 = Me.Others3
                                        }
    End Function

    Public Property EmpId As Integer

    Public Property TaskId As Integer
        Get
            Return _taskID
        End Get
        Set(value As Integer)
            _taskID = value
            NotifyPropertyChanged("TaskId")
        End Set
    End Property

    Public Property ProjId As Integer
        Get
            Return _projID
        End Get
        Set(value As Integer)
            _projID = value
            NotifyPropertyChanged("ProjId")
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

    Public Property ReferenceID As String
        Get
            Return _refID
        End Get
        Set(value As String)
            _refID = value
            NotifyPropertyChanged("ReferenceID")
        End Set
    End Property

    Public Property IncDescr As String
        Get
            Return _incDescr
        End Get
        Set(value As String)
            _incDescr = value
            NotifyPropertyChanged("IncDescr")
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

    Public Property TargetDate As String
        Get
            If _targetDate = Nothing Then
                Return String.Empty
            Else
                Return _targetDate.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _targetDate = Nothing
            Else
                _targetDate = CDate(value)
            End If
            NotifyPropertyChanged("TargetDate")
        End Set
    End Property

    Public Property CompltdDate As String
        Get
            If _compltdDate = Nothing Then
                Return String.Empty
            Else
                Return _compltdDate.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _compltdDate = Nothing
            Else
                _compltdDate = CDate(value)
            End If
            NotifyPropertyChanged("CompltdDate")
        End Set
    End Property

    Public Property DateCreated As Date

    Public Property EffortEst As String
        Get
            If _effortEst = Nothing Then
                Return String.Empty
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

    Public Property ActEffort As String
        Get
            If _actEffort = Nothing Then
                Return String.Empty
            Else
                Return _actEffort.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _actEffort = Nothing
            Else
                _actEffort = CDbl(value)
            End If
            NotifyPropertyChanged("ActEffort")
        End Set
    End Property

    Public Property ActEffortWk As String
        Get
            If _actEffortWk = Nothing Then
                Return String.Empty
            Else
                Return _actEffortWk.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _actEffortWk = Nothing
            Else
                _actEffortWk = CDbl(value)
            End If
            NotifyPropertyChanged("ActEffortWk")
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

    Public Property Others1 As String
    Public Property Others2 As String
    Public Property Others3 As String

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class

Public Class TaskStatusModel
    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Short
    Public Property Value As String

    Public Sub New(ByVal rawStatus As MyTaskStatusList)
        Me.Key = rawStatus.Key
        Me.Value = rawStatus.Value
    End Sub

    Public Function ToMyStatus() As MyTaskStatusList
        ToMyStatus = New MyTaskStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function
End Class

Public Class CategoryStatusModel

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Short
    Public Property Value As String

    Public Sub New(ByVal aRawstatus As MyCategoryStatusList)
        Me.Key = aRawstatus.Key
        Me.Value = aRawstatus.Value
    End Sub

    Public Function ToMyStatus() As MyCategoryStatusList
        ToMyStatus = New MyCategoryStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function

End Class

Public Class PhaseStatusModel

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Short
    Public Property Value As String

    Public Sub New(ByVal aRawstatus As MyPhaseStatusList)
        Me.Key = aRawstatus.Key
        Me.Value = aRawstatus.Value
    End Sub

    Public Function ToMyStatus() As MyPhaseStatusList
        ToMyStatus = New MyPhaseStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function

End Class

Public Class ReworkStatusModel
    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Short
    Public Property Value As String

    Public Sub New(ByVal aRawstatus As MyReworkStatusList)
        Me.Key = aRawstatus.Key
        Me.Value = aRawstatus.Value
    End Sub

    Public Function ToMyStatus() As MyReworkStatusList
        ToMyStatus = New MyReworkStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function
End Class

Public Class SeverityStatusModel
    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Short
    Public Property Value As String

    Public Sub New(ByVal aRawstatus As MySeverityStatusList)
        Me.Key = aRawstatus.Key
        Me.Value = aRawstatus.Value
    End Sub

    Public Function ToMyStatus() As MySeverityStatusList
        ToMyStatus = New MySeverityStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function
End Class