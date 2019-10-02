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

Public Class LatePage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _LateDBProvider As New LateDBProvider
    Private _LateViewModel As New LateViewModel
    Private mainFrame As Frame
    Private empID As Integer
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame

    Dim month As Integer = Date.Now.Month
    Dim status As Integer
    Dim img As String
    Dim displayDataMonthly As Integer = 1
    Dim displayDataFiscalYear As Integer = 2
    Dim checkStatus As Integer
    Dim year As Integer
    Dim day As Integer
    Dim startYear As Integer = 2019 'Default Start Year
    Dim monthHeader As String = "Late for the Month of"
    Dim yearHeader As String = "Late for the Fiscal Year"
#End Region

    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        Me.empID = _profile.Emp_ID
        Me.mainFrame = _mainframe
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
        lblMonthLate.Content = monthHeader + " " + MonthName(month)
        SetTitle()
        LoadMonth()
        LoadYears()
        LoadStackLateFY()
        LoadStackLate()

        cbMonth.SelectedValue = month
        cbYear.SelectedValue = year
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

    Private Sub SetTitle()

        Dim nextYear As Integer = year + 1
        Dim prevYear As Integer = year - 1

        If Date.Now.Month >= 4 Then
            lblYear.Content = yearHeader + " " + year.ToString + "-" + nextYear.ToString
        Else
            lblYear.Content = yearHeader + " " + year.ToString + "-" + nextYear.ToString
        End If
    End Sub

    Private Sub LoadStackLateFY()
        Try
            InitializeService()
            _LateDBProvider._lateList.Clear()
            Dim lstlate = client.GetLate(empID, month, year, displayDataFiscalYear)
            Dim latelist As New ObservableCollection(Of LateModel)
            Dim latelistVM As New LateViewModel()
            Dim LatesFY As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection

            For Each objResource As Late In lstlate
                _LateDBProvider.SetMyLate(objResource)
            Next

            Dim employeeFY(lstlate.Length) As String
            Dim numberOfLates(lstlate.Length) As String
            Dim i As Integer = 0

            For Each iResource As MyLate In _LateDBProvider.GetMyLate()
                LatesFY.Add(iResource.NO_OF_LATE)
                employeeFY(i) = iResource.MONTH
                i += 1
            Next

            SeriesCollection = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = LatesFY,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Late",
                    .Fill = Brushes.Orange
                }
            }

            chartLateFY.Series = SeriesCollection
            chartLateFY.AxisX.First().Labels = employeeFY
            chartLateFY.AxisY.First().LabelFormatter = Function(value) value
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadStackLate()
        Try
            InitializeService()
            _LateDBProvider._lateList.Clear()
            Dim lstlate = client.GetLate(empID, month, year, displayDataMonthly)
            Dim latelist As New ObservableCollection(Of LateModel)
            Dim latelistVM As New LateViewModel()
            Dim Lates As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection

            For Each objResource As Late In lstlate
                _LateDBProvider.SetMyLate(objResource)
            Next

            Dim employee(lstlate.Length) As String
            Dim numberOfLates(lstlate.Length) As String
            Dim i As Integer = 0

            For Each iResource As MyLate In _LateDBProvider.GetMyLate()
                Lates.Add(iResource.STATUS)
                employee(i) = iResource.FIRST_NAME
                i += 1
            Next

            SeriesCollection = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = Lates,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Late",
                    .Fill = Brushes.Red
                }
            }

            chartLate.Series = SeriesCollection
            chartLate.AxisX.First().Labels = employee
            chartLate.AxisY.First().LabelFormatter = Function(value) value
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

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = startYear To DateTime.Today.Year
                Dim nextYear As Integer = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
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

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        year = cbYear.SelectedValue
        LoadStackLateFY()
        lblYear.Content = yearHeader + " " + cbYear.SelectedItem.Text
    End Sub

    Private Sub cbMonth_DropDownClosed(sender As Object, e As EventArgs) Handles cbMonth.DropDownClosed
        month = cbMonth.SelectedValue
        LoadStackLate()
        lblMonthLate.Content = monthHeader + " " + cbMonth.SelectedItem.Text
    End Sub
End Class
