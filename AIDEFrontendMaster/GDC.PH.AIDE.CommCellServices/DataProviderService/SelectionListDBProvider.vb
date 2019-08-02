Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class SelectionListDBProvider

    Private _objLocation As ObservableCollection(Of myLocationSet)
    Private _objPosition As ObservableCollection(Of myPositionSet)
    Private _objPermission As ObservableCollection(Of myPermissionSet)
    Private _objDepartment As ObservableCollection(Of myDepartmentSet)
    Private _objDivision As ObservableCollection(Of myDivisionSet)
    Private _objStatus As ObservableCollection(Of myStatusSet)

    Public Sub New()
        _objLocation = New ObservableCollection(Of myLocationSet)
        _objPosition = New ObservableCollection(Of myPositionSet)
        _objPermission = New ObservableCollection(Of myPermissionSet)
        _objDepartment = New ObservableCollection(Of myDepartmentSet)
        _objDivision = New ObservableCollection(Of myDivisionSet)
        _objStatus = New ObservableCollection(Of myStatusSet)
    End Sub

    Public Function _getobjLocation() As ObservableCollection(Of myLocationSet)
        Return _objLocation
    End Function

    Public Function _getobjPosition() As ObservableCollection(Of myPositionSet)
        Return _objPosition
    End Function

    Public Function _getobjPermission() As ObservableCollection(Of myPermissionSet)
        Return _objPermission
    End Function

    Public Function _getobjDepartment() As ObservableCollection(Of myDepartmentSet)
        Return _objDepartment
    End Function

    Public Function _getobjDivision() As ObservableCollection(Of myDivisionSet)
        Return _objDivision
    End Function

    Public Function _getobjStatus() As ObservableCollection(Of myStatusSet)
        Return _objStatus
    End Function

    Public Function _setlistofLocation(ByRef obj As LocationList)
        Dim _myobjSet As New myLocationSet With {._locationID = obj.LOCATION_ID, ._location = obj.LOCATION, ._onsiteFLg = obj.ONSITE_FLG}

        _objLocation.Add(_myobjSet)
        Return _myobjSet
    End Function

    Public Function _setlistofPosition(ByRef obj As PositionList)
        Dim _myobjSet As New myPositionSet With {._posID = obj.POS_ID, ._posDescr = obj.POS_DESCR}

        _objPosition.Add(_myobjSet)
        Return _myobjSet
    End Function

    Public Function _setlistofPermission(ByRef obj As PermissionList)
        Dim _myobjSet As New myPermissionSet With {._grpID = obj.GRP_ID, ._grpDescr = obj.GRP_DESCR}

        _objPermission.Add(_myobjSet)
        Return _myobjSet
    End Function

    Public Function _setlistofDepartment(ByRef obj As DepartmentList)
        Dim _myobjSet As New myDepartmentSet With {._deptID = obj.DEPT_ID, ._deptDescr = obj.DEPT_DESCR}

        _objDepartment.Add(_myobjSet)
        Return _myobjSet
    End Function

    Public Function _setlistofDivision(ByRef obj As DivisionList)
        Dim _myobjSet As New myDivisionSet With {._divID = obj.DIV_ID, ._divDescr = obj.DIV_DESCR}

        _objDivision.Add(_myobjSet)
        Return _myobjSet
    End Function

    Public Function _setlistofStatus(ByRef obj As StatusList)
        Dim _myobjSet As New myStatusSet With {._statusID = obj.STATUS_ID, ._statusDescr = obj.STATUS_DESCR}

        _objStatus.Add(_myobjSet)
        Return _myobjSet
    End Function
End Class

Public Class myPositionSet
    Property _posID As Integer
    Property _posDescr As String
End Class

Public Class myPermissionSet
    Property _grpID As Integer
    Property _grpDescr As String
End Class

Public Class myDepartmentSet
    Property _deptID As Integer
    Property _deptDescr As String
End Class

Public Class myDivisionSet
    Property _divID As Integer
    Property _divDescr As String
End Class

Public Class myStatusSet
    Property _statusID As Integer
    Property _statusDescr As String
End Class

Public Class myLocationSet
    Property _locationID As Integer
    Property _location As String
    Property _onsiteFLg As Short
End Class
