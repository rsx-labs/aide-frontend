Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class KPITargetDBProvider
    Private _colKPITarget As ObservableCollection(Of KPITargetSet)

    Public Sub New()
        _colKPITarget = New ObservableCollection(Of KPITargetSet)
    End Sub

    Public Function GetAllKPITargets() As ObservableCollection(Of KPITargetSet)
        Return _colKPITarget
    End Function

    Public Sub SetKPITargets(ByRef kpiTargets As KPITargets)
        Dim kpi As New KPITargetSet With {._ID = kpiTargets.KPI_Id,
                                                ._FYStart = kpiTargets.FYStart,
                                                ._FYEnd = kpiTargets.FYEnd,
                                                  ._KPI_RefNo = kpiTargets.KPI_ReferenceNo,
                                                  ._description = kpiTargets.Description,
                                                  ._subject = kpiTargets.Subject,
                                                  ._dateCreated = kpiTargets.DateCreated}

        _colKPITarget.Add(kpi)
    End Sub

End Class

Public Class KPITargetSet
    Property _ID As Integer
    Property _FYStart As Date
    Property _FYEnd As Date
    Property _KPI_RefNo As String
    Property _description As String
    Property _subject As String
    Property _dateCreated As Date
End Class