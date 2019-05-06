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

Public Class BillablesPage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame
    Private profile As Profile

    Dim month As Integer = Date.Now.Month
    Dim setStatus As Integer
    Dim displayStatus As String = String.Empty
    Dim status As Integer
    Dim img As String
    Dim displayMonth As String
    Dim checkStatus As Integer
    Dim count As Integer
    Dim year As Integer
    Dim day As Integer
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
        LoadDataMonthly()
        LoadDataWeekly()
        'LoadAllCategory()
    End Sub


#Region "Private Methods"

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
            Dim lstresource As ResourcePlanner() = client.GetBillableHoursByWeek(profile.Emp_ID)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim billableListVM As New BillabilityViewModel()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
                totalWeekly = totalWeekly + iResource.Status
                If iResource.Emp_Name = "Breaktime" Or iResource.Emp_Name = "Sick Leave" Or iResource.Emp_Name = "Vacation Leave" Or iResource.Emp_Name = "Holiday" Or iResource.Emp_Name = "FAI-Admin" Or iResource.Emp_Name = "WS-Training" Or iResource.Emp_Name = "IBP" Then
                    nonBillable = nonBillable + iResource.Status
                Else
                    billable = billable + iResource.Status
                End If
            Next
            billableListVM.BillabilityWeek = Nothing
            billableListVM.BillabilityWeek = resourcelist
            BWChartSeries.ItemsSource = resourcelist
            
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
            Dim lstresource As ResourcePlanner() = client.GetBillableHoursByMonth(profile.Emp_ID)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim billableListVM As New BillabilityViewModel()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
                totalMonthly = totalMonthly + iResource.Status
                If iResource.Emp_Name = "Breaktime" Or iResource.Emp_Name = "Sick Leave" Or iResource.Emp_Name = "Vacation Leave" Or iResource.Emp_Name = "Holiday" Or iResource.Emp_Name = "FAI-Admin" Or iResource.Emp_Name = "WS-Training" Or iResource.Emp_Name = "IBP" Then
                    nonBillable = nonBillable + iResource.Status
                Else
                    billable = billable + iResource.Status
                End If
            Next
            billableListVM.BillabilityMonth = Nothing
            billableListVM.BillabilityMonth = resourcelist
            BMChartSeries.ItemsSource = resourcelist

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
