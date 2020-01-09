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

Public Class BillabilityPage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame
    Private profile As Profile

    Dim month As Integer
    Dim year As Integer
    Dim displayData As Integer

    Dim lstFiscalYear As FiscalYear()
    Dim commendationVM As New CommendationViewModel()
    Dim fiscalyearVM As New SelectionListViewModel
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me.InitializeComponent()

        LoadMonth()
        LoadYear()

        SetFiscalYear()
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
        End Try
        Return bInitialize
    End Function

    Public Sub LoadYear()
        Try
            If InitializeService() Then
                lstFiscalYear = client.GetAllFiscalYear()
                LoadFiscalYear()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
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
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadMonth()
        cbMonth.DisplayMemberPath = "Text"
        cbMonth.SelectedValuePath = "Value"
        cbMonth.Items.Add(New With {.Text = "January", .Value = 1})
        cbMonth.Items.Add(New With {.Text = "February", .Value = 2})
        cbMonth.Items.Add(New With {.Text = "March", .Value = 3})
        cbMonth.Items.Add(New With {.Text = "April", .Value = 4})
        cbMonth.Items.Add(New With {.Text = "May", .Value = 5})
        cbMonth.Items.Add(New With {.Text = "June", .Value = 6})
        cbMonth.Items.Add(New With {.Text = "July", .Value = 7})
        cbMonth.Items.Add(New With {.Text = "August", .Value = 8})
        cbMonth.Items.Add(New With {.Text = "September", .Value = 9})
        cbMonth.Items.Add(New With {.Text = "October", .Value = 10})
        cbMonth.Items.Add(New With {.Text = "November", .Value = 11})
        cbMonth.Items.Add(New With {.Text = "December", .Value = 12})
    End Sub

    Private Sub SetFiscalYear()
        Try
            month = Date.Now.Month

            cbMonth.SelectedValue = month

            If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
                cbYear.SelectedValue = (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
            Else
                cbYear.SelectedValue = (Date.Now.Year).ToString() + "-" + (Date.Now.Year + 1).ToString()
            End If

            year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadData()
        LoadNonBillMonthSummary()
        LoadNonBillMonth()

        lblNonBill.Content = "Non-Billable Hours for " + MonthName(month) + " " + GetSelectedMonth(year, month).ToString
        lblSummary.Content = "Non-Billable Hours Summary for " + MonthName(month) + " " + GetSelectedMonth(year, month).ToString
    End Sub

    Private Sub LoadNonBillMonthSummary()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()

            displayData = 1 'Display data per Week

            Dim lstResource = client.GetNonBillableHours(profile.Email_Address, displayData, month, year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()
            Dim holidayHours As New ChartValues(Of Double)()
            Dim vlHours As New ChartValues(Of Double)()
            Dim slHours As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection

            For Each objResource As ResourcePlanner In lstResource
                _ResourceDBProvider.SetNonBillableList(objResource)
            Next

            Dim employee(lstResource.Length) As String
            Dim i As Integer = 0

            For Each iResource As myResourceList In _ResourceDBProvider.GetNonBillableList()
                holidayHours.Add(iResource.holidayHours)
                vlHours.Add(iResource.vlHours)
                slHours.Add(iResource.slHours)
                employee(i) = iResource.Emp_Name
                i += 1
            Next

            SeriesCollection = New SeriesCollection From {
                New ColumnSeries With {
                    .Values = holidayHours,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Holiday",
                    .Fill = Brushes.ForestGreen,
                    .Foreground = Brushes.White
                },
                New ColumnSeries With {
                    .Values = vlHours,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Vacation Leave",
                    .Fill = Brushes.DodgerBlue,
                    .Foreground = Brushes.White
                },
                New ColumnSeries With {
                     .Values = slHours,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Sick Leave",
                    .Fill = Brushes.Red,
                    .Foreground = Brushes.White
                }
            }

            chartMonthSummary.Series = SeriesCollection
            chartMonthSummary.AxisX.First().Labels = employee
            chartMonthSummary.AxisY.First().LabelFormatter = Function(value) value
            chartMonthSummary.AxisX.First().LabelsRotation = 135

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadNonBillMonth()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()
            displayData = 2 'Display data per Month

            Dim lstResource = client.GetNonBillableHours(profile.Email_Address, displayData, month, year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()
            Dim holidayHours As New ChartValues(Of Double)()
            Dim vlHours As New ChartValues(Of Double)()
            Dim slHours As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection

            For Each objResource As ResourcePlanner In lstResource
                _ResourceDBProvider.SetNonBillableList(objResource)
            Next

            Dim employee(lstResource.Length) As String
            Dim i As Integer = 0

            For Each iResource As myResourceList In _ResourceDBProvider.GetNonBillableList()
                holidayHours.Add(iResource.holidayHours)
                vlHours.Add(iResource.vlHours)
                slHours.Add(iResource.slHours)
                employee(i) = iResource.Emp_Name
                i += 1
            Next

            SeriesCollection = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = holidayHours,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Holiday",
                    .Fill = Brushes.ForestGreen
                },
                New StackedColumnSeries With {
                    .Values = vlHours,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Vacation Leave",
                    .Fill = Brushes.DodgerBlue
                },
                New StackedColumnSeries With {
                    .Values = slHours,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Sick Leave",
                    .Fill = Brushes.Red
                }
            }

            chartMonth.Series = SeriesCollection
            chartMonth.AxisX.First().Labels = employee
            chartMonth.AxisY.First().LabelFormatter = Function(value) value
            chartMonth.AxisX.First().LabelsRotation = 135

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Function GetSelectedMonth(year As Integer, month As Integer)
        If month < 4 Then
            year = year + 1
        Else
            year = year
        End If

        Return year
    End Function
#End Region

#Region "Events"

    Private Sub cbMonth_DropDownClosed(sender As Object, e As EventArgs) Handles cbMonth.DropDownClosed
        month = cbMonth.SelectedValue
        LoadData()
    End Sub

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
        LoadData()
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
