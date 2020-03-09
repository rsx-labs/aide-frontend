Imports System.ComponentModel
Imports LiveCharts

Public Class ResourcePlannerModel
    Implements INotifyPropertyChanged

    Private _empID As Integer
    Private _EmpName As String
    Private _status As String
    Private _usedVL As ChartValues(Of Double)
    Private _desc As String
    Private _EmpImage As String
    Private _dateEntry As DateTime
    Private _startDate As DateTime
    Private _endDate As DateTime
    Private _durations As String
    Private _leaveStatus As String
    Private _statusCD As Short
    Private _comment As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal rawResourceList As myResourceList)
        Me._empID = rawResourceList.Emp_ID
        Me._EmpName = rawResourceList.Emp_Name
        Me._desc = rawResourceList.Desc
        Me._status = rawResourceList.Status
        Me._EmpImage = rawResourceList.Emp_Image
        Me._dateEntry = rawResourceList.Date_Entry
        Me._comment = rawResourceList.comments
    End Sub

    Public Sub New(ByVal rawResourceList As myResourceList, ByVal StatusLeave As String)
        Me._empID = rawResourceList.Emp_ID
        Me._EmpName = rawResourceList.Emp_Name
        Me._desc = rawResourceList.Desc
        Me._status = rawResourceList.Status
        Me._EmpImage = rawResourceList.Emp_Image
        Me._dateEntry = rawResourceList.Date_Entry
        Me._startDate = rawResourceList.startDate
        Me._endDate = rawResourceList.endDate
        Me._durations = rawResourceList.duration.ToString()
        Me._leaveStatus = StatusLeave
        Me._status = rawResourceList.Status
        Me._desc = rawResourceList.Desc
    End Sub

    Public Property EMP_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = EMP_ID
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property EmpName As String
        Get
            Return _EmpName
        End Get
        Set(value As String)
            _EmpName = value
            NotifyPropertyChanged("EmpName")
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

    Public Property Descr As String
        Get
            Return _desc
        End Get
        Set(value As String)
            _desc = value
            NotifyPropertyChanged("Descr")
        End Set
    End Property

    Public Property EmpImage As String
        Get
            Return _EmpImage
        End Get
        Set(value As String)
            _EmpImage = value
            NotifyPropertyChanged("EmpImage")
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

    Public Property UsedVL As ChartValues(Of Double)
        Get
            Return _usedVL
        End Get
        Set(value As ChartValues(Of Double))
            _usedVL = value
            NotifyPropertyChanged("UsedVL")
        End Set
    End Property

    Public Property START_DATE As DateTime
        Get
            Return _startDate
        End Get
        Set(value As DateTime)
            _startDate = value
            NotifyPropertyChanged("START_DATE")
        End Set
    End Property

    Public Property END_DATE As DateTime
        Get
            Return _endDate
        End Get
        Set(value As DateTime)
            _endDate = value
            NotifyPropertyChanged("END_DATE")
        End Set
    End Property

    Public Property DURATIONS As String
        Get
            If Convert.ToDouble(_durations) > 1 Then
                Return _durations + " days"
            Else
                Return _durations + " day"
            End If

        End Get
        Set(value As String)
            _durations = value
            NotifyPropertyChanged("DURATIONS")
        End Set
    End Property

    Public Property LEAVE_STATUS As String
        Get
            Return _leaveStatus
        End Get
        Set(value As String)
            _leaveStatus = value
            NotifyPropertyChanged("LEAVE_STATUS")
        End Set
    End Property

    Public Property STATUS_CD As Short
        Get
            Return _statusCD
        End Get
        Set(value As Short)
            _statusCD = value
            NotifyPropertyChanged("STATUS_CD")
        End Set
    End Property

    Public Property COMMENT As String
        Get
            Return _comment
        End Get
        Set(value As String)
            _comment = value
            NotifyPropertyChanged("COMMENT")
        End Set
    End Property
    Private Sub NotifyPropertyChanged(ByVal propertyname As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyname))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
