Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class ContributorsDBProvider
    Private _objContributors As ObservableCollection(Of myContributorsSet)

    Public Sub New()
        _objContributors = New ObservableCollection(Of myContributorsSet)
    End Sub

    Public Function _getobjContributors() As ObservableCollection(Of myContributorsSet)
        Return _objContributors
    End Function

    Public Function _setlistofitems(ByRef contri As Contributors)
        Dim _myContributorsSet As New myContributorsSet With {._fullname = contri.FULL_NAME, _
                                                ._department = contri.DEPARTMENT, _
                                                  ._division = contri.DIVISION, _
                                                  ._position = contri.POSITION, _
                                                    ._imagepath = contri.IMAGE_PATH}

        _objContributors.Add(_myContributorsSet)
        Return _myContributorsSet
    End Function
End Class

Public Class myContributorsSet
    Property _fullname As String
    Property _department As String
    Property _division As String
    Property _position As String
    Property _imagepath As String
End Class
