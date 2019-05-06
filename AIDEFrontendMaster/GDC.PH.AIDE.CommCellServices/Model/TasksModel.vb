Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class TasksModel
    Implements INotifyPropertyChanged

    Private _dateStarted As Date
    Private _targetDate As Date
    Private _compltdDate As Date

    Private _effortEst As Double
    Private _actEffortEst As Double
    Private _actEffortEstWk As Double

    Private _taskID As Integer
    Private _incID As String
    Private _projID As Integer
    Private _projCode As Integer

    Private _incDescr As String
    Private _taskDescr As String
    Private _remarks As String

    Private _taskType As Integer
    Private _status As String
    Private _rework As Integer
    Private _phase As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawTasks As MyTasks)
        Me.TaskId = aRawTasks.TaskId
        Me.EmpId = aRawTasks.EmpId
        Me.IncId = aRawTasks.IncId
        Me.TaskType = aRawTasks.TaskType
        Me.ProjId = aRawTasks.ProjId 'set equivalent to projID because there is no field for projectcode
        Me.IncDescr = aRawTasks.IncDescr
        Me.TaskDescr = aRawTasks.TaskDescr
        Me.DateStarted = aRawTasks.DateStarted
        Me.TargetDate = aRawTasks.TargetDate
        Me.CompltdDate = aRawTasks.CompltdDate
        Me.DateCreated = aRawTasks.DateCreated
        Me.Status = aRawTasks.Status
        Me.Remarks = aRawTasks.Remarks
        Me.EffortEst = aRawTasks.EffortEst
        Me.ActEffortEst = aRawTasks.ActEffortEst
        Me.ActEffortEstWk = aRawTasks.ActEffortEstWk
        Me.ProjectCode = aRawTasks.ProjId 'set equivalent to projID because there is no field for projectcode
        Me.Rework = aRawTasks.Rework
        Me.Phase = aRawTasks.Phase
        Me.Others1 = aRawTasks.Others1
        Me.Others2 = aRawTasks.Others2
        Me.Others3 = aRawTasks.Others3
        Me.HrsWrkedDate = aRawTasks.HoursWorked_Date
    End Sub

    Public Function ToMyTasks() As MyTasks
        ToMyTasks = New MyTasks() With {.TaskId = Me.TaskId,
                                       .EmpId = Me.EmpId,
                                       .IncId = Me.IncId,
                                       .TaskType = Me.TaskType,
                                       .ProjId = Me.ProjId,
                                       .IncDescr = Me.IncDescr,
                                       .TaskDescr = Me.TaskDescr,
                                       .DateStarted = Me.DateStarted,
                                       .TargetDate = Me.TargetDate,
                                       .CompltdDate = Me.CompltdDate,
                                       .DateCreated = Date.Now,
                                       .Status = Me.Status,
                                       .Remarks = Me.Remarks,
                                       .EffortEst = Me.EffortEst,
                                       .ActEffortEst = Me.ActEffortEst,
                                       .ActEffortEstWk = Me.ActEffortEstWk,
                                       .ProjectCode = Me.ProjId,
                                       .Rework = Me.Rework,
                                       .Phase = Me.Phase,
                                       .Others1 = Me.Others1,
                                       .Others2 = Me.Others2,
                                       .Others3 = Me.Others3,
                                       .HoursWorked_Date = Me.HrsWrkedDate}
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

    Public Property IncId As String
        Get
            Return _incID
        End Get
        Set(value As String)
            _incID = value
            NotifyPropertyChanged("IncId")
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

    Public Property IncDescr As String
        Get
            Return _incDescr
        End Get
        Set(value As String)
            _incDescr = value
            NotifyPropertyChanged("IncDescr")
        End Set
    End Property

    Public Property TaskDescr As String
        Get
            Return _taskDescr
        End Get
        Set(value As String)
            _taskDescr = value
            NotifyPropertyChanged("TaskDescr")
        End Set
    End Property

    Public Property Remarks As String
        Get
            Return _remarks
        End Get
        Set(value As String)
            _remarks = value
            NotifyPropertyChanged("Remarks")
        End Set
    End Property

    Public Property DateCreated As Date

    Public Property TaskType As Integer
        Get
            Return _taskType
        End Get
        Set(value As Integer)
            _taskType = value
            NotifyPropertyChanged("TaskType")
        End Set
    End Property

    Public Property Status As String
        Get
            Return _status
        End Get
        Set(value As String)
            _status = value
            NotifyPropertyChanged("Status")
        End Set
    End Property

    Public Property Rework As Integer
        Get
            Return _rework
        End Get
        Set(value As Integer)
            _rework = value
            NotifyPropertyChanged("Rework")
        End Set
    End Property

    Public Property Phase As String
        Get
            Return _phase
        End Get
        Set(value As String)
            _phase = value
            NotifyPropertyChanged("Phase")
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

    Public Property Others1 As String
    Public Property Others2 As String
    Public Property Others3 As String
    Public Property HrsWrkedDate As String

    Public Property DateStarted As String
        Get
            If _dateStarted = Nothing Then
                Return String.Empty
            Else
                Return _dateStarted.ToString("MM-dd-yyyy")
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
                Return _targetDate.ToString("MM-dd-yyyy")
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
                Return _compltdDate.ToString("MM-dd-yyyy")
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

    Public Property ActEffortEst As String
        Get
            If _actEffortEst = Nothing Then
                Return String.Empty
            Else
                Return _actEffortEst.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _actEffortEst = Nothing
            Else
                _actEffortEst = CDbl(value)
            End If
            NotifyPropertyChanged("ActEffortEst")
        End Set
    End Property

    Public Property ActEffortEstWk As String
        Get
            If _actEffortEstWk = Nothing Then
                Return String.Empty
            Else
                Return _actEffortEstWk.ToString
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _actEffortEstWk = Nothing
            Else
                _actEffortEstWk = CDbl(value)
            End If
            NotifyPropertyChanged("ActEffortEstWk")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class

Public Class TaskStatusModel
    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Integer
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

    Public Property Key As Integer
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

    Public Property Key As Integer
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

    Public Property Key As Integer
    Public Property Value As String

    Public Sub New(ByVal aRawstatus As MyReworkStatusList)
        Me.Key = aRawstatus.Key
        Me.Value = aRawstatus.Value
    End Sub

    Public Function ToMyStatus() As MyReworkStatusList
        ToMyStatus = New MyReworkStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function
End Class