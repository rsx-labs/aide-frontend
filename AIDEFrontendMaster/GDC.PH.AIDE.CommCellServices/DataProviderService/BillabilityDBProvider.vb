Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

Public Class BillabilityDBProvider
    Private _billabilityList As ObservableCollection(Of MyBillability)

    Public Sub New()
        _billabilityList = New ObservableCollection(Of MyBillability)
    End Sub

    Public Function GetBillabilityList() As ObservableCollection(Of MyBillability)
        Return _billabilityList
    End Function

    Public Sub SetBillabilityList(ByVal _billability As BillableHours)
        Dim billability As MyBillability = New MyBillability With {
                .Name = _billability.Name,
                .Status = _billability.Status,
                .Hours = _billability.Hours
                }
        _billabilityList.Add(billability)
    End Sub

End Class

Public Class MyBillability
    Property Name As String
    Property Status As Short
    Property Hours As Decimal
End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           END                 '
'''''''''''''''''''''''''''''''''

