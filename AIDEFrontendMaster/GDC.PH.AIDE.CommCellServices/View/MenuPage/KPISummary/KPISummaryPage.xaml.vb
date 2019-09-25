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
Imports System.Data

Public Class KPISummaryPage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _KPISummaryDBProvider As New KPISummaryDBProvider
    Private _KPISummaryVM As New KPISummaryViewModel
    Private _mainFrame As Frame
    Private _profile As Profile
    Private _menugrid As Grid
    Private _addFrame As Frame
    Private _submenuFrame As Frame

    Dim _month As Integer = Date.Now.Month
    Dim displayData As Integer
    Dim _year As Integer
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame)
        Me._profile = _profile
        Me._mainFrame = mFrame
        Me._menugrid = menugrid
        Me._addFrame = addframe
        Me._submenuFrame = submenuframe
        Me.InitializeComponent()

        _month = Date.Now.Month
        _year = Date.Now.Year

        LoadMonth()
        LoadYears()
        LoadData()

        cbMonth.SelectedValue = _month
        cbYear.SelectedValue = _year

        dgKPISummary.SelectionMode = DataGridSelectionMode.Single
        dgKPISummary.SelectionUnit = DataGridSelectionUnit.Cell

        If _profile.Permission_ID = 1 Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If
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

    Public Sub LoadYears()
        Try
            For i As Integer = 2018 To DateTime.Today.Year
                cbYear.Items.Add(i)
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
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

    Private Sub LoadData()
        LoadKPISummary()
        LoadKPISummaryTable()
        lblKPISummary.Content = "Monthly KPI Summary for FY " + _year.ToString
        lblKPISummaryChart.Content = "Monthly KPI Summary for FY " + _year.ToString
    End Sub

    Private Sub LoadKPISummary()
        Try
            InitializeService()

            displayData = 1 'Display data per Week

            Dim FYStart As Date
            Dim FYEnd As Date
            If Date.Now.Month >= 4 Then
                FYStart = Convert.ToDateTime(Date.Now.Year.ToString() + "-" + "04-01")
                FYEnd = Convert.ToDateTime((Date.Now.Year + 1).ToString() + "-" + "03-31")
            Else
                FYStart = Convert.ToDateTime((Date.Now.Year - 1).ToString() + "-" + "04-01")
                FYEnd = Convert.ToDateTime(Date.Now.Year.ToString() + "-" + "03-31")
            End If
            Dim lstKPISummary = client.GetKPISummaryList(Me._profile.Emp_ID, FYStart, FYEnd)
            Dim lstKPISummaryModel As New ObservableCollection(Of KPISummaryModel)
            Dim kpiSummaryVM As New KPISummaryViewModel()
            Dim kpi1 As New ChartValues(Of Double)()
            Dim kpi2 As New ChartValues(Of Double)()
            Dim kpi3 As New ChartValues(Of Double)()
            Dim overallKPI As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection
            Dim dfmi As System.Globalization.DateTimeFormatInfo = New Globalization.DateTimeFormatInfo()
            Dim kpi1Name As String = ""
            Dim kpi2Name As String = ""
            Dim kpi3Name As String = ""
            Dim overallValue As Double = 0
            For Each objSummary As KPISummary In lstKPISummary
                _KPISummaryDBProvider.SetKPISummary(objSummary)
            Next

            Dim monthName(lstKPISummary.Length) As String
            Dim x As Integer = 0
            Dim y As Integer = 0

            For Each iSummary As KPISummaryData In _KPISummaryDBProvider.GetAllKPISummary()
                x = x + 1
                If x = 1 Then
                    kpi1.Add(iSummary.KPI_Actual * 100)
                    kpi1Name = iSummary._subject
                    monthName(y) = dfmi.GetMonthName(iSummary._Month)
                    y = y + 1
                    overallValue = iSummary.KPI_Overall * 100
                ElseIf x = 2 Then
                    kpi2.Add(iSummary.KPI_Actual * 100)
                    kpi2Name = iSummary._subject
                    overallValue = overallValue + (iSummary.KPI_Overall * 100)
                ElseIf x = 3 Then
                    kpi3.Add(iSummary.KPI_Actual * 100)
                    kpi3Name = iSummary._subject
                    x = 0
                    overallValue = (overallValue + (iSummary.KPI_Overall * 100)) / 3
                    overallKPI.Add(Math.Round(overallValue, 2))
                End If
            Next

            SeriesCollection = New SeriesCollection From {
                New ColumnSeries With {
                    .Values = kpi1,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = kpi1Name,
                    .Fill = Brushes.ForestGreen,
                    .Foreground = Brushes.White
                },
                New ColumnSeries With {
                    .Values = kpi2,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = kpi2Name,
                    .Fill = Brushes.DodgerBlue,
                    .Foreground = Brushes.White
                },
                New ColumnSeries With {
                     .Values = kpi3,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = kpi3Name,
                    .Fill = Brushes.Red,
                    .Foreground = Brushes.White
                },
                New LineSeries With {
                     .Values = overallKPI,
                    .DataLabels = True,
                    .Title = "Overall",
                    .Fill = Brushes.Transparent,
                    .Foreground = Brushes.DarkGray
                }
            }

            chartMonthSummary.Series = SeriesCollection
            chartMonthSummary.AxisX.First().Labels = monthName
            chartMonthSummary.AxisY.First().LabelFormatter = Function(value) value
            chartMonthSummary.AxisX.First().LabelsRotation = 135

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadKPISummaryTable()
        Dim lstDict As New List(Of Dictionary(Of String, String))()
        Dim dict As New Dictionary(Of String, String)()
        Dim dict2 As New Dictionary(Of String, String)()
        Dim dict3 As New Dictionary(Of String, String)()
        Dim dict4 As New Dictionary(Of String, String)()
        Dim monthName(12) As String
        Dim x As Integer = 0
        Dim y As Integer = 0
        Dim kpiName As String = ""
        Dim kpi1Name As String = ""
        Dim kpi2Name As String = ""
        Dim kpi3Name As String = ""
        Dim overallValue As Double = 0
        Dim curYear As Integer = 0
        Dim dfmi As System.Globalization.DateTimeFormatInfo = New Globalization.DateTimeFormatInfo()

        dict4.Add("KPI", "Overall")
        For Each iSummary As KPISummaryData In _KPISummaryDBProvider.GetAllKPISummary()
            If iSummary._Month >= 4 Then
                curYear = Date.Now.Year
            Else
                curYear = Date.Now.Year - 1
            End If


            If kpiName <> iSummary._subject Then
                x = x + 1

                If x = 1 Then
                    overallValue = iSummary.KPI_Overall * 100
                    If Not dict.ContainsKey("KPI") Then
                        dict = New Dictionary(Of String, String)()
                        dict.Add("EMP_ID", iSummary._EmployeeID)
                        dict.Add("KPI_REF", iSummary._KPI_RefNo)
                        dict.Add("FY_START", iSummary._FYStart)
                        dict.Add("FY_END", iSummary._FYEnd)
                        dict.Add("KPI", iSummary._subject)
                    End If

                    dict.Add(dfmi.GetMonthName(iSummary._Month) + " " + curYear.ToString(), (iSummary.KPI_Target * 100).ToString() + " | " + (iSummary.KPI_Actual * 100).ToString())
                ElseIf x = 2 Then
                    overallValue = overallValue + (iSummary.KPI_Overall * 100)
                    If Not dict2.ContainsKey("KPI") Then
                        dict2 = New Dictionary(Of String, String)()
                        dict2.Add("EMP_ID", iSummary._EmployeeID)
                        dict2.Add("KPI_REF", iSummary._KPI_RefNo)
                        dict2.Add("FY_START", iSummary._FYStart)
                        dict2.Add("FY_END", iSummary._FYEnd)
                        dict2.Add("KPI", iSummary._subject)
                    End If
                    dict2.Add(dfmi.GetMonthName(iSummary._Month) + " " + curYear.ToString(), (iSummary.KPI_Target * 100).ToString() + " | " + (iSummary.KPI_Actual * 100).ToString())
                ElseIf x = 3 Then
                    overallValue = (overallValue + (iSummary.KPI_Overall * 100)) / 3
                    If Not dict3.ContainsKey("KPI") Then
                        dict3 = New Dictionary(Of String, String)()
                        dict3.Add("EMP_ID", iSummary._EmployeeID)
                        dict3.Add("KPI_REF", iSummary._KPI_RefNo)
                        dict3.Add("FY_START", iSummary._FYStart)
                        dict3.Add("FY_END", iSummary._FYEnd)
                        dict3.Add("KPI", iSummary._subject)
                    End If
                    dict3.Add(dfmi.GetMonthName(iSummary._Month) + " " + curYear.ToString(), (iSummary.KPI_Target * 100).ToString() + " | " + (iSummary.KPI_Actual * 100).ToString())
                    dict4.Add(dfmi.GetMonthName(iSummary._Month) + " " + curYear.ToString(), Math.Round(overallValue, 2).ToString())
                    x = 0
                End If
            End If

            kpiName = iSummary._subject
        Next
        lstDict.Add(dict)
        lstDict.Add(dict2)
        lstDict.Add(dict3)
        lstDict.Add(dict4)
        Dim table As DataTable = New DataTable
        table = ToDataTable(lstDict)
        dgKPISummary.RowBackground = New SolidColorBrush(Colors.White)

        dgKPISummary.ItemsSource = table.AsDataView
    End Sub

    Private Function ToDataTable(list As List(Of Dictionary(Of String, String))) As DataTable
        Dim result As New DataTable()

        If list.Count = 0 Then
            Return result
        End If

        Dim columnNames = list.SelectMany(Function(dict) dict.Keys).Distinct()
        result.Columns.AddRange(columnNames.[Select](Function(c) New DataColumn(c)).ToArray())
        For Each item As Dictionary(Of String, String) In list
            Dim row = result.NewRow()
            For Each key In item.Keys
                row(key) = item(key)
            Next
            result.Rows.Add(row)
        Next
        Return result
    End Function


#End Region

#Region "Private Functions"

    Private Sub cbMonth_DropDownClosed(sender As Object, e As EventArgs) Handles cbMonth.DropDownClosed
        _month = cbMonth.SelectedValue
        'LoadData()
    End Sub

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        _year = cbYear.SelectedValue
        'LoadData()
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

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        _addFrame.Navigate(New KPISummaryAddPage(Me._profile, Me._mainFrame, Me._addFrame, Me._menugrid, Me._submenuFrame))
        _mainFrame.IsEnabled = False
        _mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuFrame.IsEnabled = False
        _submenuFrame.Opacity = 0.3
        _addFrame.Visibility = Visibility.Visible
        _addFrame.Margin = New Thickness(150, 60, 150, 60)
    End Sub


    Private Sub dgKPISummary_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgKPISummary.MouseDoubleClick

        If _profile.Permission_ID = 1 Then
            If (dgKPISummary.Items.Count > 0) Then
                'Dim collKPISummary As ObservableCollection(Of KPISummaryData) = _KPISummaryDBProvider.GetAllKPISummary()
                Dim columnIndex As Integer = dgKPISummary.CurrentColumn.DisplayIndex
                Dim selectedItem As String = dgKPISummary.CurrentCell.Item(columnIndex)
                Dim kpi As New KPISummary
                Dim strCell As String() = selectedItem.Split("|")
                If strCell.Count > 0 Then
                    kpi.KPITarget = strCell.GetValue(0)
                    kpi.KPIActual = strCell.GetValue(1)
                    kpi.EmployeeID = dgKPISummary.CurrentCell.Item(0)
                    kpi.KPI_Reference = dgKPISummary.CurrentCell.Item(1)
                    kpi.FYStart = dgKPISummary.CurrentCell.Item(2)
                    kpi.FYEnd = dgKPISummary.CurrentCell.Item(3)
                    kpi.Subject = dgKPISummary.CurrentCell.Item(4)
                    kpi.KPI_Month = Convert.ToDateTime(dgKPISummary.CurrentColumn.Header).Month

                    _addFrame.Navigate(New KPISummaryAddPage(Me._profile, Me._mainFrame, Me._addFrame, Me._menugrid, Me._submenuFrame, kpi))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menugrid.IsEnabled = False
                    _menugrid.Opacity = 0.3
                    _submenuFrame.IsEnabled = False
                    _submenuFrame.Opacity = 0.3
                    _addFrame.Visibility = Visibility.Visible
                    _addFrame.Margin = New Thickness(150, 60, 150, 60)

                End If

            End If
        End If

    End Sub

    Private Sub dgKPISummary_Loaded(sender As Object, e As RoutedEventArgs) Handles dgKPISummary.Loaded
        If dgKPISummary.Items.Count > 0 Then
            dgKPISummary.Columns(0).Visibility = Visibility.Collapsed
            dgKPISummary.Columns(1).Visibility = Visibility.Collapsed
            dgKPISummary.Columns(2).Visibility = Visibility.Collapsed
            dgKPISummary.Columns(3).Visibility = Visibility.Collapsed
        End If
    End Sub


#End Region

End Class
