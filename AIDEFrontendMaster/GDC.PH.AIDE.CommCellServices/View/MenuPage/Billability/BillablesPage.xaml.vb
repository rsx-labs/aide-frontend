Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
Imports System.Drawing.Printing
Imports LiveCharts
Imports LiveCharts.Wpf

Public Class BillablesPage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame
    Private profile As Profile

    Dim month As Integer = Date.Now.Month
    Dim year As Integer
    Dim displayOption As Integer = 1 'Weekly is the Default Display Options

    Dim totalMonthly As Integer
    Dim totalWeekly As Integer
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
        LoadDataWeekly()
        LoadDataMonthly()
    End Sub

#Region "Sub Procedures"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

    Public Sub LoadDataWeekly()
        Try
            Dim nonBillable As Double
            Dim billable As Double
            Dim percentNonBillable As Double
            Dim percentBillable As Double

            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource As ResourcePlanner() = client.GetBillableHoursByWeek(profile.Emp_ID, Date.Now)

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim labelPoint As Func(Of ChartPoint, String) = Function(chartPoint) String.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
            Dim cht_y_values As ChartValues(Of Double) = New ChartValues(Of Double)()
            Dim series As SeriesCollection = New SeriesCollection()

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                totalWeekly = totalWeekly + iResource.Status

                If iResource.Emp_Name = "Breaktime" Or iResource.Emp_Name = "Sick Leave" Or iResource.Emp_Name = "Vacation Leave" Or iResource.Emp_Name = "Holiday" Or iResource.Emp_Name = "FAI-Admin" Or iResource.Emp_Name = "WS-Training" Then
                    nonBillable = nonBillable + iResource.Status
                Else
                    billable = billable + iResource.Status
                End If

                Dim ps As PieSeries = New PieSeries With {
                    .Title = iResource.Emp_Name,
                    .Values = New ChartValues(Of Double) From {
                        Double.Parse(iResource.Status)
                    },
                    .DataLabels = True,
                    .LabelPoint = labelPoint
                }
                series.Add(ps)
            Next

            pieChartWeekly.Series = series

            percentBillable = (billable / totalWeekly) * 100
            percentNonBillable = (nonBillable / totalWeekly) * 100

            WNonBillableHours.Content = percentNonBillable.ToString("N2") + "%"
            WBillableHours.Content = percentBillable.ToString("N2") + "%"

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadDataMonthly()
        Try
            Dim nonBillable As Double
            Dim billable As Double
            Dim percentNonBillable As Double
            Dim percentBillable As Double

            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource As ResourcePlanner() = client.GetBillableHoursByMonth(profile.Emp_ID, month, year)
           
            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim labelPoint As Func(Of ChartPoint, String) = Function(chartPoint) String.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
            Dim cht_y_values As ChartValues(Of Double) = New ChartValues(Of Double)()
            Dim series As SeriesCollection = New SeriesCollection()

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                totalMonthly = totalMonthly + iResource.Status

                If iResource.Emp_Name = "Breaktime" Or iResource.Emp_Name = "Sick Leave" Or iResource.Emp_Name = "Vacation Leave" Or iResource.Emp_Name = "Holiday" Or iResource.Emp_Name = "FAI-Admin" Or iResource.Emp_Name = "WS-Training" Then
                    nonBillable = nonBillable + iResource.Status
                Else
                    billable = billable + iResource.Status
                End If

                Dim ps As PieSeries = New PieSeries With {
                    .Title = iResource.Emp_Name,
                    .Values = New ChartValues(Of Double) From {
                        Double.Parse(iResource.Status)
                    },
                    .DataLabels = True,
                    .LabelPoint = labelPoint
                }
                series.Add(ps)
            Next

            pieChartMonth.Series = series

            percentBillable = (billable / totalMonthly) * 100
            percentNonBillable = (nonBillable / totalMonthly) * 100

            MNonBillableHours.Content = percentNonBillable.ToString("N2") + "%"
            MBillableHours.Content = percentBillable.ToString("N2") + "%"

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub
#End Region

#Region "ICallback Functions"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError

    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
