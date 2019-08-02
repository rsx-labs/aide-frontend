Imports System.ComponentModel

Public Class PositionModel
    Private _posid As Integer
    Private _posdescr As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myPosSet As myPositionSet)
        Me._posid = _myPosSet._posID
        Me._posdescr = _myPosSet._posDescr
    End Sub

    Public Property POSITION_ID As Integer
        Get
            Return _posid
        End Get
        Set(value As Integer)
            _posid = value
            OnPropertyChanged("POSITION_ID")
        End Set
    End Property

    Public Property POSITION_DESCR As String
        Get
            Return _posdescr
        End Get
        Set(value As String)
            _posdescr = value
            OnPropertyChanged("POSITION_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class PermissionModel
    Private _grpid As Integer
    Private _grpdescr As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myGrpSet As myPermissionSet)
        Me._grpid = _myGrpSet._grpID
        Me._grpdescr = _myGrpSet._grpDescr
    End Sub

    Public Property GROUP_ID As Integer
        Get
            Return _grpid
        End Get
        Set(value As Integer)
            _grpid = value
            OnPropertyChanged("GROUP_ID")
        End Set
    End Property

    Public Property GROUP_DESCR As String
        Get
            Return _grpdescr
        End Get
        Set(value As String)
            _grpdescr = value
            OnPropertyChanged("GROUP_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class DepartmentModel

    Private _deptid As Integer
    Private _deptdescr As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myDeptSet As myDepartmentSet)
        Me._deptid = _myDeptSet._deptID
        Me._deptdescr = _myDeptSet._deptDescr
    End Sub

    Public Property DEPARTMENT_ID As Integer
        Get
            Return _deptid
        End Get
        Set(value As Integer)
            _deptid = value
            OnPropertyChanged("DEPARTMENT_ID")
        End Set
    End Property

    Public Property DEPARTMENT_DESCR As String
        Get
            Return _deptdescr
        End Get
        Set(value As String)
            _deptdescr = value
            OnPropertyChanged("DEPARTMENT_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class DivisionModel

    Private _divid As Integer
    Private _divdescr As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myDivSet As myDivisionSet)
        Me._divid = _myDivSet._divID
        Me._divdescr = _myDivSet._divDescr
    End Sub

    Public Property DIVISION_ID As Integer
        Get
            Return _divid
        End Get
        Set(value As Integer)
            _divid = value
            OnPropertyChanged("DIVISION_ID")
        End Set
    End Property

    Public Property DIVISION_DESCR As String
        Get
            Return _divdescr
        End Get
        Set(value As String)
            _divdescr = value
            OnPropertyChanged("DIVISION_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class MaritalModel

    Private _statusid As Integer
    Private _statusdescr As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myStatusSet As myStatusSet)
        Me._statusid = _myStatusSet._statusID
        Me._statusdescr = _myStatusSet._statusDescr
    End Sub

    Public Property STATUS_ID As Integer
        Get
            Return _statusid
        End Get
        Set(value As Integer)
            _statusid = value
            OnPropertyChanged("STATUS_ID")
        End Set
    End Property

    Public Property STATUS_DESCR As String
        Get
            Return _statusdescr
        End Get
        Set(value As String)
            _statusdescr = value
            OnPropertyChanged("STATUS_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class WorkShiftModel

    Private _statusid As Integer
    Private _statusdescr As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myStatusSet As myStatusSet)
        Me._statusid = _myStatusSet._statusID
        Me._statusdescr = _myStatusSet._statusDescr
    End Sub

    Public Property STATUS_ID As Integer
        Get
            Return _statusid
        End Get
        Set(value As Integer)
            _statusid = value
            OnPropertyChanged("STATUS_ID")
        End Set
    End Property

    Public Property STATUS_DESCR As String
        Get
            Return _statusdescr
        End Get
        Set(value As String)
            _statusdescr = value
            OnPropertyChanged("STATUS_DESCR")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class LocationModel

    Private _locationID As Integer
    Private _location As String
    Private _onsiteFLg As Short

    Public Sub New()
    End Sub

    Public Sub New(ByVal _myLocationSet As myLocationSet)
        _locationID = _myLocationSet._locationID
        _location = _myLocationSet._location
        _onsiteFLg = _myLocationSet._onsiteFLg
    End Sub

    Public Property LOCATION_ID As Integer
        Get
            Return _locationID
        End Get
        Set(value As Integer)
            _locationID = value
            OnPropertyChanged("LOCATION_ID")
        End Set
    End Property

    Public Property LOCATION As String
        Get
            Return _location
        End Get
        Set(value As String)
            _location = value
            OnPropertyChanged("LOCATION")
        End Set
    End Property

    Public Property ONSITE_FLG As Short
        Get
            Return _onsiteFLg
        End Get
        Set(value As Short)
            _onsiteFLg = value
            OnPropertyChanged("ONSITE_FLG")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class