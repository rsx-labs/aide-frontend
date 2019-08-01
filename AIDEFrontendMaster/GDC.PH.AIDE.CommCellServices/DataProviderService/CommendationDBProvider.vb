Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class CommendationDBProvider
    Private _commendList As ObservableCollection(Of MyCommendations)
    Private client As AideServiceClient

    Public Sub New()
        _commendList = New ObservableCollection(Of MyCommendations)
    End Sub

    Public Function GetMyCommendations() As ObservableCollection(Of MyCommendations)
        Return _commendList
    End Function

    Public Sub SetMyCommendations(ByVal _commend As Commendations)
        Dim _commendObject As MyCommendations = New MyCommendations With {
                .COMMEND_ID = _commend.COMMEND_ID,
                .EMP_ID = _commend.EMP_ID,
                .PROJECT = _commend.PROJECT,
                .EMPLOYEE = _commend.EMPLOYEE,
                .DATE_SENT = _commend.DATE_SENT,
                .REASON = _commend.REASON,
                .SENT_BY = _commend.SENT_BY
                }
        _commendList.Add(_commendObject)
    End Sub

End Class

Public Class MyCommendations
    Property COMMEND_ID As Integer
    Property EMP_ID As Integer
    Property PROJECT As String
    Property EMPLOYEE As String
    Property DATE_SENT As Date
    Property REASON As String
    Property SENT_BY As String
End Class