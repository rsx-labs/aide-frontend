Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           START               '
'''''''''''''''''''''''''''''''''
Public Class BillabilityDBProvider
    Private _billabilityMonthList As ObservableCollection(Of MyBillability)
    Private _billabilityWeekList As ObservableCollection(Of MyBillability)
    Private client As AideServiceClient

    Public Sub New()
        _billabilityMonthList = New ObservableCollection(Of MyBillability)
    End Sub

    Public Function GetBillabilityWeekList() As ObservableCollection(Of MyBillability)
        Return _billabilityWeekList
    End Function

    Public Sub SetBillabilityWeekList(ByVal _status As NonBillableSummary)
        Dim _birthdayObject As MyBillability = New MyBillability With {
                .EMPID = _status.EmployeeID,
                .NICK_NAME = _status.Name,
                .VL = _status.VacationLeave,
                .SL = _status.SickLeave,
                .HOLIDAY = _status.Holiday,
                .HALFDAY = _status.HalfDay,
                .HALFVL = _status.HalfdayVL,
                .HALFSL = _status.HalfdaySL,
                .TOTAL = _status.Total
                }
        _billabilityWeekList.Add(_birthdayObject)
    End Sub

    Public Function GetBillabilityMonthList() As ObservableCollection(Of MyBillability)
        Return _billabilityMonthList
    End Function

    Public Sub SetBillabilityMonthList(ByVal _status As NonBillableSummary)
        Dim _birthdayObjectMonth As MyBillability = New MyBillability With {
                .EMPID = _status.EmployeeID,
                .NICK_NAME = _status.Name,
                .VL = _status.VacationLeave,
                .SL = _status.SickLeave,
                .HOLIDAY = _status.Holiday,
                .HALFDAY = _status.HalfDay,
                .HALFVL = _status.HalfdayVL,
                .HALFSL = _status.HalfdaySL,
                .TOTAL = _status.Total
                }
        _billabilityMonthList.Add(_birthdayObjectMonth)
    End Sub
End Class

Public Class MyBillability
    Property EMPID As Integer
    Property NICK_NAME As String
    Property VL As Double
    Property SL As Double
    Property HOLIDAY As Double
    Property HALFDAY As Double
    Property HALFVL As Double
    Property HALFSL As Double
    Property TOTAL As Double
End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           END                 '
'''''''''''''''''''''''''''''''''

