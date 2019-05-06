Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1


''' <summary>
'''HYACINTH AMARLES 
''' </summary>
''' <remarks></remarks>
Public Class ProjectDBProvider
    Public _myprojectlist As New ObservableCollection(Of myProjectList)
    Public _project As New myProjectList

    Public Sub New()
        _myprojectlist = New ObservableCollection(Of myProjectList)
    End Sub

    Public Sub setProjectList(ByRef projectlist As Project)
        Dim projObj As myProjectList = New myProjectList With {.Project_ID = projectlist.ProjectID,
                                                              .Project_Name = projectlist.ProjectName,
                                                               .category = projectlist.Category,
                                                              .billability = projectlist.Billability}
        _myprojectlist.Add(projObj)
    End Sub

    Public Sub setProject(ByRef project As Project)
        _project.Project_ID = project.ProjectID
        _project.Project_Name = project.ProjectName
        _project.category = project.Category
        _project.billability = project.Billability
    End Sub

    Public Function getProjectList()
        Return _myprojectlist
    End Function

    Public Function getProject()
        Return _project
    End Function
End Class
''' <summary>
'''HYACINTH AMARLES 
''' </summary>
''' <remarks></remarks>
Public Class myProjectList
    Public Project_ID As Integer
    Public Project_Name As String
    Public category As String
    Public billability As String

End Class