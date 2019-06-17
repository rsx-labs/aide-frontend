Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class WorkplaceAuditDBProvider
    Private _WorkplaceAuditList As ObservableCollection(Of MyWorkplaceAudit)
    Private client As AideServiceClient

    Public Sub New()
        _WorkplaceAuditList = New ObservableCollection(Of MyWorkplaceAudit)
    End Sub

    Public Function GetMyWorkplaceAudit() As ObservableCollection(Of MyWorkplaceAudit)
        Return _WorkplaceAuditList
    End Function

    Public Sub SetMyWorkplaceAudit(ByVal _WorkplaceAudit As WorkplaceAudit)
        Dim _WorkplaceAuditObject As MyWorkplaceAudit = New MyWorkplaceAudit With {
                .AUDIT_DAILY_ID = _WorkplaceAudit.AUDIT_DAILY_ID,
                .AUDIT_QUESTIONS_ID = _WorkplaceAudit.AUDIT_QUESTIONS_ID,
                .EMP_ID = _WorkplaceAudit.EMP_ID,
                .FY_WEEK = _WorkplaceAudit.FY_WEEK,
                .STATUS = _WorkplaceAudit.STATUS,
                .DT_CHECKED = _WorkplaceAudit.DT_CHECKED,
                .AUDIT_QUESTIONS = _WorkplaceAudit.AUDIT_QUESTIONS,
                .OWNER = _WorkplaceAudit.OWNER,
                .AUDIT_QUESTIONS_GROUP = _WorkplaceAudit.AUDIT_QUESTIONS_GROUP
                }

        _WorkplaceAuditList.Add(_WorkplaceAuditObject)
    End Sub

End Class

Public Class MyWorkplaceAudit
    Property AUDIT_DAILY_ID As Integer
    Property AUDIT_QUESTIONS_ID As Integer
    Property EMP_ID As Integer
    Property FY_WEEK As Integer
    Property STATUS As Integer
    Property DT_CHECKED As DateTime
    Property AUDIT_QUESTIONS As String
    Property OWNER As String
    Property AUDIT_QUESTIONS_GROUP As String
End Class