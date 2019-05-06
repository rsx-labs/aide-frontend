Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class ResourcePlannerDBProvider

    Private _resourceList As ObservableCollection(Of myResourceList)
    Private _categoryList As ObservableCollection(Of myResourceList)
    Private _AllcategoryList As ObservableCollection(Of myResourceList)
    Private _emprpList As ObservableCollection(Of myResourceList)
    Private _AllemprpList As ObservableCollection(Of myResourceList)
    Public _splist As ObservableCollection(Of myResourceList)

    Sub New()
        _resourceList = New ObservableCollection(Of myResourceList)
        _emprpList = New ObservableCollection(Of myResourceList)
        _AllemprpList = New ObservableCollection(Of myResourceList)
        _categoryList = New ObservableCollection(Of myResourceList)
        _AllcategoryList = New ObservableCollection(Of myResourceList)
        _splist = New ObservableCollection(Of myResourceList)
    End Sub

    Public Sub SetResourceList(ByVal resourceLst As ResourcePlanner)
        Dim _Resourceobj As myResourceList = New myResourceList With {.Emp_ID = resourceLst.EmpID,
                                                             .Emp_Name = resourceLst.NAME,
                                                             .Emp_Image = resourceLst.Image_Path}
        _resourceList.Add(_Resourceobj)

    End Sub

    Public Function GetResourceList()
        Return _resourceList
    End Function

    Public Sub SetCategoryList(ByVal resourceLst As ResourcePlanner)
        Dim _Resourceobj As myResourceList = New myResourceList With {.Status = resourceLst.Status,
                                                             .Desc = resourceLst.DESCR}
        _categoryList.Add(_Resourceobj)

    End Sub

    Public Function GetCategoryList()
        Return _categoryList
    End Function

    Public Sub SetAllCategoryList(ByVal resourceLst As ResourcePlanner)
        Dim _Resourceobj As myResourceList = New myResourceList With {.Status = resourceLst.Status,
                                                             .Desc = resourceLst.DESCR}
        _AllcategoryList.Add(_Resourceobj)

    End Sub

    Public Function GetAllCategoryList()
        Return _AllcategoryList
    End Function

    Public Sub SetEmpRPList(ByVal resourceLst As ResourcePlanner)
        Dim _Resourceobj As myResourceList = New myResourceList With {.Date_Entry = resourceLst.DateEntry,
                                                             .Status = resourceLst.Status}
        _splist.Add(_Resourceobj)

    End Sub

    Public Function GetEmpRPList()
        Return _splist
    End Function

    Public Sub SetAllEmpRPList(ByVal resourceLst As ResourcePlanner)
        Dim _Resourceobj As myResourceList = New myResourceList With {.Emp_ID = resourceLst.EmpID,
                                                             .Emp_Name = resourceLst.NAME,
                                                             .Date_Entry = resourceLst.DateEntry,
                                                             .Status = resourceLst.Status}
        _splist.Add(_Resourceobj)

    End Sub

    Public Function GetAllEmpRPList()
        Return _splist
    End Function

End Class


Public Class myResourceList
    Public Emp_ID As Integer
    Public Emp_Name As String
    Public Status As Double
    Public Desc As String
    Public Emp_Image As String
    Public Date_Entry As Date
End Class
