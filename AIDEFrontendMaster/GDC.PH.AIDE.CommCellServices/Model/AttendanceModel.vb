Imports System.ComponentModel
Public Class AttendanceModel
    Implements INotifyPropertyChanged

    Private _empID As Integer
    Private _EmpName As String
    Private _status As Double
    Private _desc As String
    Private _EmpImage As String
    Private _dateEntry As DateTime
    Private _displayStatus As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal rawAttendanceList As myAttendanceList)
        Me._empID = rawAttendanceList.Emp_ID
        Me._EmpName = rawAttendanceList.Emp_Name
        Me._desc = rawAttendanceList.Desc
        Me._status = rawAttendanceList.Status
        Me._EmpImage = rawAttendanceList.Emp_Image
        Me._dateEntry = rawAttendanceList.Date_Entry
        Me._displayStatus = rawAttendanceList.Display_Status
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

    Public Property Status As Double

        Get
            Return _status
        End Get
        Set(value As Double)
            _status = value
            SetCategory()
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
            If _EmpImage = String.Empty Then
                Return "\..\Assets\EmployeePhotos\shadowImage.png"
            End If
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


    Public Property DisplayStatus As String
        Get
            Return _displayStatus
        End Get
        Set(value As String)
            _displayStatus = value
            NotifyPropertyChanged("DisplayStatus")
        End Set
    End Property

    Public Sub SetCategory()
        If _status = 1 Then
            _displayStatus = "Absent"
        ElseIf _status = 2 Then
            _displayStatus = "Present"
        ElseIf _status = 3 Then
            _displayStatus = "Sick Leave"
        ElseIf _status = 4 Then
            _displayStatus = "Vacation Leave"
        End If
    End Sub

    Private Sub NotifyPropertyChanged(ByVal propertyname As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyname))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
