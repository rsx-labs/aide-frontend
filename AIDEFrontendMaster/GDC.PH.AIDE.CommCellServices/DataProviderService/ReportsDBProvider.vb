Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class ReportsDBProvider

    Private _objReports As ObservableCollection(Of myReportsSet)

    Public Sub New()
        _objReports = New ObservableCollection(Of myReportsSet)
    End Sub

    Public Function _getobjReports() As ObservableCollection(Of myReportsSet)
        Return _objReports
    End Function

    Public Function _setlistofitems(ByRef reports As Reports)
        Dim _myReportsSet As New myReportsSet With {._reportid = reports.REPORT_ID,
                                                  ._optid = reports.OPT_ID,
                                                  ._moduleid = reports.MODULE_ID,
                                                  ._description = reports.DESCRIPTION,
                                                  ._filepath = reports.FILE_PATH}

        _objReports.Add(_myReportsSet)
        Return _myReportsSet
    End Function
End Class

Public Class myReportsSet
    Property _reportid As Integer
    Property _optid As Integer
    Property _moduleid As Integer
    Property _description As String
    Property _filepath As String
End Class
