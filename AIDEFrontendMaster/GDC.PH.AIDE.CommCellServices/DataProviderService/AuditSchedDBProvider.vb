Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class AuditSchedDBProvider
    Private _auditSchedList As ObservableCollection(Of MyAuditSched)
    Private client As AideServiceClient

    Public Sub New()
        _auditSchedList = New ObservableCollection(Of MyAuditSched)
    End Sub

    Public Function GetMyAuditSched() As ObservableCollection(Of MyAuditSched)
        Return _auditSchedList
    End Function

    Public Sub SetMyAuditSched(ByVal _auditSched As AuditSched)
        Dim _auditSchedObject As MyAuditSched = New MyAuditSched With {
                .AUDIT_SCHED_ID = _auditSched.AUDIT_SCHED_ID,
                .EMP_ID = _auditSched.EMP_ID,
                .FY_WEEK = _auditSched.FY_WEEK,
                .PERIOD_START = _auditSched.PERIOD_START,
                .PERIOD_END = _auditSched.PERIOD_END,
                .DAILY = _auditSched.DAILY,
                .WEEKLY = _auditSched.WEEKLY,
                .MONTHLY = _auditSched.MONTHLY,
                .FY_START = _auditSched.FY_START,
                .FY_END = _auditSched.FY_END
                }

        _auditSchedList.Add(_auditSchedObject)
    End Sub

End Class

Public Class MyAuditSched
    Property AUDIT_SCHED_ID As Integer
    Property EMP_ID As Integer
    Property FY_WEEK As Integer
    Property PERIOD_START As DateTime
    Property PERIOD_END As DateTime
    Property DAILY As String
    Property WEEKLY As String
    Property MONTHLY As String
    Property FY_START As DateTime
    Property FY_END As DateTime
End Class