Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class SelectionListViewModel
    Implements INotifyPropertyChanged

    Private _objPositionSet As New ObservableCollection(Of PositionModel)
    Private _objPermissionSet As New ObservableCollection(Of PermissionModel)
    Private _objDepartmentSet As New ObservableCollection(Of DepartmentModel)
    Private _objDivisionSet As New ObservableCollection(Of DivisionModel)
    Private _objMaritalSet As New ObservableCollection(Of MaritalModel)
    Private _objWorkShiftSet As New ObservableCollection(Of WorkShiftModel)
    Private _selectionListDB As New SelectionListDBProvider

    Sub New()
        For Each _myPositionSet As myPositionSet In _selectionListDB._getobjPosition
            _objPositionSet.Add(New PositionModel(_myPositionSet))
        Next

        For Each _myPermissionSet As myPermissionSet In _selectionListDB._getobjPermission
            _objPermissionSet.Add(New PermissionModel(_myPermissionSet))
        Next

        For Each _myDepartmentSet As myDepartmentSet In _selectionListDB._getobjDepartment
            _objDepartmentSet.Add(New DepartmentModel(_myDepartmentSet))
        Next

        For Each _myDivisionSet As myDivisionSet In _selectionListDB._getobjDivision
            _objDivisionSet.Add(New DivisionModel(_myDivisionSet))
        Next

        For Each _myStatusSet As myStatusSet In _selectionListDB._getobjStatus
            _objMaritalSet.Add(New MaritalModel(_myStatusSet))
        Next
        For Each _myStatusSet As myStatusSet In _selectionListDB._getobjStatus
            _objWorkShiftSet.Add(New WorkShiftModel(_myStatusSet))
        Next
    End Sub
    Public Property ObjectPositionSet As ObservableCollection(Of PositionModel)
        Get
            Return _objPositionSet
        End Get
        Set(value As ObservableCollection(Of PositionModel))
            _objPositionSet = value
            NotifyPropertyChanged("ObjectPositionSet")
        End Set
    End Property

    Public Property ObjectPermissionSet As ObservableCollection(Of PermissionModel)
        Get
            Return _objPermissionSet
        End Get
        Set(value As ObservableCollection(Of PermissionModel))
            _objPermissionSet = value
            NotifyPropertyChanged("ObjectPermissionSet")
        End Set
    End Property

    Public Property ObjectDepartmentSet As ObservableCollection(Of DepartmentModel)
        Get
            Return _objDepartmentSet
        End Get
        Set(value As ObservableCollection(Of DepartmentModel))
            _objDepartmentSet = value
            NotifyPropertyChanged("ObjectDepartmentSet")
        End Set
    End Property

    Public Property ObjectDivisionSet As ObservableCollection(Of DivisionModel)
        Get
            Return _objDivisionSet
        End Get
        Set(value As ObservableCollection(Of DivisionModel))
            _objDivisionSet = value
            NotifyPropertyChanged("ObjectDivisionSet")
        End Set
    End Property

    Public Property ObjectMaritalSet As ObservableCollection(Of MaritalModel)
        Get
            Return _objMaritalSet
        End Get
        Set(value As ObservableCollection(Of MaritalModel))
            _objMaritalSet = value
            NotifyPropertyChanged("ObjectMaritalSet")
        End Set
    End Property
    Public Property ObjectWorkShiftSet As ObservableCollection(Of WorkShiftModel)
        Get
            Return _objWorkShiftSet
        End Get
        Set(value As ObservableCollection(Of WorkShiftModel))
            _objWorkShiftSet = value
            NotifyPropertyChanged("ObjectWorkShiftSet")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
