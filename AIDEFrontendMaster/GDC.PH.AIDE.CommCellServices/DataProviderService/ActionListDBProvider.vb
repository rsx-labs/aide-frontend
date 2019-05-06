Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1


''' <summary>
''' By Jester Sanchez/ Lemuela Abulencia
''' </summary>
''' <remarks></remarks>
Public Class ActionListDBProvider
    Private _obAction As ObservableCollection(Of myActionSet)


    Public Sub New()
        _obAction = New ObservableCollection(Of myActionSet)
    End Sub

    Public Function _getobAction() As ObservableCollection(Of myActionSet)
        Return _obAction
    End Function

    Public Function _setlistofitems(ByRef act As Action)
        Dim _myActionSet As New myActionSet With {._actRef = act.Act_ID, _
                                                  ._actMessage = act.Act_Message, _
                                                  ._actAssignee = act.Act_Assignee, _
                                                  ._actNickName = act.Act_NickName, _
                                                  ._actDueDate = act.Act_DueDate, _
                                                  ._actDateClosed = act.Act_DateClosed}

        _obAction.Add(_myActionSet)
        Return _myActionSet
    End Function

End Class

Public Class myActionSet
    Property _actRef As String
    Property _actMessage As String
    Property _actAssignee As Integer
    Property _actNickName As String
    Property _actDueDate As Date
    Property _actDateClosed As String
End Class
