Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class AssignedProjectDBProvider
    Private _assignedProjectList As ObservableCollection(Of MyAssignedProjectLists)
    Sub New()
        _assignedProjectList = New ObservableCollection(Of MyAssignedProjectLists)
    End Sub
    Public Function GetAssignedProjectList() As ObservableCollection(Of MyAssignedProjectLists)
        Return _assignedProjectList
    End Function
    Public Sub SetAssignedProjectList(ByVal _project As ViewProject)
        Dim _iAssignedProjectList As MyAssignedProjectLists = New MyAssignedProjectLists With {.Name = _project.Status, .Position = _project.Name, .EmailAddress = _project.Month1, .AlternateEmail = _project.Month2, .WorkMobile = _project.Month3}
        _assignedProjectList.Add(_iAssignedProjectList)
    End Sub
End Class
Public Class MyAssignedProjectLists
    Public Property Name As String
    Public Property Position As String
    Public Property EmailAddress As String
    Public Property AlternateEmail As String
    Public Property WorkMobile As String
End Class
