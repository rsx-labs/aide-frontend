Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1


''' <summary>
'''GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class ViewProjectProvider


    Private _projectList As ObservableCollection(Of MyProjectLists)
    Private _MyProjectList As New MyProjectLists
    Sub New()
        _projectList = New ObservableCollection(Of MyProjectLists)
    End Sub

    Public Function GetProjectList() As ObservableCollection(Of MyProjectLists)

        Return _projectList
    End Function


    Public Sub SetProjectList(ByVal _project As ViewProject)

        Dim _employeeObj As MyProjectLists = New MyProjectLists With {.Name = _project.Status, .Position = _project.Name, .EmailAddress = _project.Month1, .AlternateEmail = _project.Month2, .WorkMobile = _project.Month3}
        _projectList.Add(_employeeObj)
    End Sub
End Class


''' <summary>
'''GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class MyProjectLists

    Public Property Name As String
    Public Property Position As String
    Public Property EmailAddress As String
    Public Property AlternateEmail As String
    Public Property WorkMobile As String

End Class
