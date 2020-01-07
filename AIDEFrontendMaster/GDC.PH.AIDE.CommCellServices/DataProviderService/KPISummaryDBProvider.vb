Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class KPISummaryDBProvider
    Implements INotifyPropertyChanged

    Private _colKPISummary As ObservableCollection(Of KPISummaryData)
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New()
        _colKPISummary = New ObservableCollection(Of KPISummaryData)
    End Sub

    Public Function GetAllKPISummary() As ObservableCollection(Of KPISummaryData)
        Return _colKPISummary
    End Function

    Public Sub SetKPISummary(ByRef kpiSummary As KPISummary)
        Dim kpi As New KPISummaryData With {._ID = kpiSummary.KPI_Id,
                                                ._EmployeeID = kpiSummary.EmployeeId,
                                                ._FYStart = kpiSummary.FYStart,
                                                ._FYEnd = kpiSummary.FYEnd,
                                                ._Month = kpiSummary.KPI_Month,
                                                .KPI_Target = kpiSummary.KPITarget,
                                                .KPI_Actual = kpiSummary.KPIActual,
                                                .KPI_Overall = kpiSummary.KPIOverall,
                                                  ._KPI_RefNo = kpiSummary.KPI_Reference,
                                                  ._description = kpiSummary.Description,
                                                  ._subject = kpiSummary.Subject,
                                                  ._datePosted = kpiSummary.DatePosted}

        _colKPISummary.Add(kpi)
    End Sub

    Public Property KPISummaryDataList As ObservableCollection(Of KPISummaryData)
        Get
            Return _colKPISummary
        End Get
        Set(value As ObservableCollection(Of KPISummaryData))
            _colKPISummary = value
            NotifyPropertyChanged("KPISummaryDataList")
        End Set
    End Property
    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

Public Class KPISummaryData
    Property _ID As Integer
    Property _EmployeeID As Integer
    Property _FYStart As Date
    Property _FYEnd As Date
    Property _Month As Short
    Property _KPI_RefNo As String
    Property KPI_Actual As Double
    Property KPI_Target As Double
    Property KPI_Overall As Double
    Property _description As String
    Property _subject As String
    Property _datePosted As Date
End Class