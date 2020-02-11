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

Public Class BillabilitySickLeavePage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame
    Private profile As Profile

    Dim lstFiscalYear As FiscalYear()
    Dim commendationVM As New CommendationViewModel()
    Dim fiscalyearVM As New SelectionListViewModel

    Dim month As Integer = Date.Now.Month
    Dim displayFiscalYear As Integer = 3
    Dim slStatus As Integer = 3
    Dim year As Integer
    Dim day As Integer
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me.InitializeComponent()

        LoadYear()
        SetFiscalYear()
        GenerateLeaveCredits()
        LoadData()
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Private Sub SetTitle()
        lblYear.Content = "Sick Leave For Fiscal Year " + cbYear.SelectedValue
    End Sub

    Private Sub GenerateLeaveCredits()
        If InitializeService() Then
            Try
                If Date.Today.Month = 4 And Date.Today.Day <= 7 Then
                    client.InsertLeaveCredits(profile.Emp_ID, year)
                End If
            Catch ex As Exception
               MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try
        End If
    End Sub

    Private Sub SetFiscalYear()
        Try
            month = Date.Now.Month

            If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
                cbYear.SelectedValue = (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
            Else
                cbYear.SelectedValue = (Date.Now.Year).ToString() + "-" + (Date.Now.Year + 1).ToString()
            End If

            year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadYear()
        Try
            If InitializeService() Then
                lstFiscalYear = client.GetAllFiscalYear()
                LoadFiscalYear()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadFiscalYear()
        Try
            Dim lstFiscalYearList As New ObservableCollection(Of FiscalYearModel)
            Dim FYDBProvider As New SelectionListDBProvider

            For Each objFiscal As FiscalYear In lstFiscalYear
                FYDBProvider._setlistofFiscal(objFiscal)
            Next

            For Each rawUser As myFiscalYearSet In FYDBProvider._getobjFiscal()
                lstFiscalYearList.Add(New FiscalYearModel(rawUser))
            Next

            fiscalyearVM.ObjectFiscalYearSet = lstFiscalYearList
            cbYear.ItemsSource = fiscalyearVM.ObjectFiscalYearSet
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadData()
        LoadDataSLYearly()
        SetTitle()
    End Sub

    Private Sub LoadDataSLYearly()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(profile.Email_Address, slStatus, displayFiscalYear, year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()
            Dim UsedSL As New ChartValues(Of Double)()
            Dim TotalBalance As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim employee(lstresource.Length) As String
            Dim usedLeaves(lstresource.Length) As String
            Dim i As Integer = 0

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                UsedSL.Add(iResource.UsedVL)
                TotalBalance.Add(iResource.TotalBalance)
                employee(i) = iResource.Emp_Name
                i += 1
            Next

            SeriesCollection = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = UsedSL,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Used SL",
                    .Fill = Brushes.Red
                },
                New StackedColumnSeries With {
                    .Values = TotalBalance,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Balance",
                    .Fill = Brushes.Gray
                }
            }

            chartSL.Series = SeriesCollection
            chartSL.AxisX.First().Labels = employee
            chartSL.AxisY.First().LabelFormatter = Function(value) value
            chartSL.AxisX.First().LabelsRotation = 135

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#End Region

#Region "Private Functions"

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        If Not cbYear.SelectedIndex = -1 Then
            year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
            LoadData()
        End If
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
