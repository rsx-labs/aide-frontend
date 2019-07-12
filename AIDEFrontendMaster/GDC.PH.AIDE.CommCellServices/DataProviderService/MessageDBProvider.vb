Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class MessageDBProvider
    Private _objMessage As ObservableCollection(Of myMessageSet)

    Public Sub New()
        _objMessage = New ObservableCollection(Of myMessageSet)
    End Sub

    Public Function _getobjMessage() As ObservableCollection(Of myMessageSet)
        Return _objMessage
    End Function

    Public Function _setlistofitems(ByRef msg As MessageDetail)
        Dim _myMessageSet As New myMessageSet With {._messagedescr = msg.MESSAGE_DESCR, _
                                                ._title = msg.TITLE}

        _objMessage.Add(_myMessageSet)
        Return _myMessageSet
    End Function
End Class

Public Class myMessageSet
    Property _messagedescr As String
    Property _title As String
End Class
