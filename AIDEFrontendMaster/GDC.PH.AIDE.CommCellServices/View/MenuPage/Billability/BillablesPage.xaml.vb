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
    Private billabilityDBProvider As New BillabilityDBProvider
    Private mainFrame As Frame
    Private profile As Profile

    Dim weekID As Integer
    Dim month As Integer = Date.Now.Month
    Dim year As Integer
    Dim displayOption As Integer = 1 'Weekly is the Default Display Options

    Dim selectedValue As Integer
    Dim totalMonthly As Double
    Dim totalWeekly As Double

    Dim dateToday As Date = Date.Today
    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)
    Dim lastWeekSaturday As Date = saturday.AddDays(-14)

    Dim lstWeekRange As WeekRange()
    Dim colorList As New List(Of System.Windows.Media.Brush)
#End Region

#Region "Provider Declaration"
    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
#End Region

#Region "Model Declaration"
    Dim weeklyReportModel As New WeeklyReportModel
#End Region

#Region "View Model Declarations"
    Dim weekRangeViewModel As New WeekRangeViewModel
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me.InitializeComponent()
        InitializeService()

        month = Date.Now.Month
        year = Date.Now.Year

        GenerateColors()
        LoadMonth()
        LoadYears()
        LoadWeeks()

        LoadDataWeekly()
        LoadDataMonthly()

        cbMonth.SelectedValue = month
        cbYear.SelectedValue = year
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

            ' Reset Data
            billabilityDBProvider.GetBillabilityList.Clear()
            totalWeekly = 0

            Dim lstresource As BillableHours() = client.GetBillableHoursByWeek(profile.Emp_ID, cbDateRange.SelectedValue)

            For Each objBillables As BillableHours In lstresource
                billabilityDBProvider.SetBillabilityList(objBillables)
            Next

            Dim labelPoint As Func(Of ChartPoint, String) = Function(chartPoint) String.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
            Dim cht_y_values As ChartValues(Of Double) = New ChartValues(Of Double)()
            Dim series As SeriesCollection = New SeriesCollection()
            Dim i As Integer = 0
            Dim projectListName As New List(Of String)

            For Each iBillability As MyBillability In billabilityDBProvider.GetBillabilityList()
                totalWeekly = totalWeekly + iBillability.Hours

                If iBillability.Status = 0 Then
                    nonBillable = nonBillable + iBillability.Hours
                Else
                    billable = billable + iBillability.Hours
                End If

                Dim ps As PieSeries = New PieSeries With {
                    .Title = iBillability.Name,
                    .Values = New ChartValues(Of Double) From {
                        Double.Parse(iBillability.Hours)
                    },
                    .DataLabels = False,
                    .Fill = colorList(i)
                }
                projectListName.Add(iBillability.Name)
                i = i + 1
                series.Add(ps)
            Next

            CreateEllipse(projectListName)

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

            ' Reset Data
            billabilityDBProvider.GetBillabilityList.Clear()
            totalMonthly = 0

            Dim lstresource As BillableHours() = client.GetBillableHoursByMonth(profile.Emp_ID, month, year)

            For Each objBillables As BillableHours In lstresource
                billabilityDBProvider.SetBillabilityList(objBillables)
            Next

            Dim labelPoint As Func(Of ChartPoint, String) = Function(chartPoint) String.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
            Dim cht_y_values As ChartValues(Of Double) = New ChartValues(Of Double)()
            Dim series As SeriesCollection = New SeriesCollection()
            Dim i As Integer = 0

            Dim projectListName As New List(Of String)

            For Each iBillability As MyBillability In billabilityDBProvider.GetBillabilityList()
                totalMonthly = totalMonthly + iBillability.Hours

                If iBillability.Status = 0 Then
                    nonBillable = nonBillable + iBillability.Hours
                Else
                    billable = billable + iBillability.Hours
                End If

                Dim ps As PieSeries = New PieSeries With {
                    .Title = iBillability.Name,
                    .Values = New ChartValues(Of Double) From {
                        Double.Parse(iBillability.Hours)
                    },
                    .DataLabels = False,
                    .Fill = colorList(i)
                }
                projectListName.Add(iBillability.Name)
                i = i + 1
                series.Add(ps)
            Next

            CreateEllipse(projectListName)

            pieChartMonth.Series = series

            percentBillable = (billable / totalMonthly) * 100
            percentNonBillable = (nonBillable / totalMonthly) * 100

            MNonBillableHours.Content = percentNonBillable.ToString("N2") + "%"
            MBillableHours.Content = percentBillable.ToString("N2") + "%"

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadWeeks()
        ' Load Items for Week Range Combobox
        Try
            ' Clear combo box data
            cbDateRange.DataContext = Nothing
            selectedValue = -1
            weeklyReportDBProvider.GetWeekRangeList().Clear()

            Dim listWeekRange As New ObservableCollection(Of WeekRangeModel)
            weekRangeViewModel = New WeekRangeViewModel

            lstWeekRange = client.GetWeekRangeByMonthYear(profile.Emp_ID, month, year)

            For Each objWeekRange As WeekRange In lstWeekRange
                weeklyReportDBProvider.SetWeekRangeList(objWeekRange)
            Next

            For Each weekRange As MyWeekRange In weeklyReportDBProvider.GetWeekRangeList()
                listWeekRange.Add(New WeekRangeModel(weekRange))

                If lastWeekSaturday = weekRange.StartWeek Then
                    selectedValue = weekRange.WeekRangeID
                End If

                weekID = weekRange.WeekRangeID
            Next

            ' Set selectedValue to last week of month
            If selectedValue = -1 Then
                selectedValue = weekID
            End If

            weekRangeViewModel.WeekRangeList = listWeekRange
            cbDateRange.DataContext = weekRangeViewModel
            cbDateRange.SelectedValue = selectedValue
        Catch ex As SystemException
            client.Abort()
        End Try
    End Sub

    Private Sub LoadMonth()
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

    Private Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                Dim nextYear As Integer = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub CreateEllipse(projectName As List(Of String))

        Dim lPoints As New List(Of Point)
        Dim ellipsePoints As New List(Of Ellipse)
        Dim labelProjName As New List(Of Label)

        For j As Integer = 0 To projectName.Count - 1
            For k As Integer = 0 To projectName.Count - 1
                lPoints.Add(New Point(j * 10, k * 20))
            Next
        Next

        For i As Integer = 0 To projectName.Count - 1
            ellipsePoints.Add(New Ellipse With {.Width = 9,
                                                .Height = 9,
                                                .Fill = colorList(i)})

            labelProjName.Add(New Label With {.Content = projectName(i),
                                              .FontSize = 10})

            cCanvas.Children.Add(ellipsePoints(i))
            cCanvas.Children.Add(labelProjName(i))
        Next

        For i As Integer = 0 To projectName.Count - 1
            Canvas.SetLeft(ellipsePoints(i), lPoints(i).X + 15 - ellipsePoints(i).Width / 2)
            Canvas.SetTop(ellipsePoints(i), lPoints(i).Y + 10 - ellipsePoints(i).Height / 2)

            Canvas.SetLeft(labelProjName(i), lPoints(i).X + 25 - ellipsePoints(i).Width / 2)
            Canvas.SetTop(labelProjName(i), lPoints(i).Y + 2 - ellipsePoints(i).Height / 2)
        Next
    End Sub

    Private Sub GenerateColors()
        colorList.Add(Brushes.DodgerBlue)
        colorList.Add(Brushes.Orange)
        colorList.Add(Brushes.BurlyWood)
        colorList.Add(Brushes.CadetBlue)
        colorList.Add(Brushes.Brown)
        colorList.Add(Brushes.Green)
        colorList.Add(Brushes.BlueViolet)
        colorList.Add(Brushes.Magenta)
        colorList.Add(Brushes.DarkSalmon)
        colorList.Add(Brushes.CornflowerBlue)
        colorList.Add(Brushes.SlateBlue)
        colorList.Add(Brushes.Violet)
        colorList.Add(Brushes.YellowGreen)
        colorList.Add(Brushes.Blue)
        colorList.Add(Brushes.Red)
        colorList.Add(Brushes.RoyalBlue)
        colorList.Add(Brushes.LimeGreen)
        colorList.Add(Brushes.MidnightBlue)
        colorList.Add(Brushes.Indigo)
        colorList.Add(Brushes.DimGray)
        colorList.Add(Brushes.OrangeRed)
        colorList.Add(Brushes.DarkOliveGreen)
        colorList.Add(Brushes.Purple)
        colorList.Add(Brushes.Chocolate)
        colorList.Add(Brushes.SeaGreen)
        colorList.Add(Brushes.Tan)
        colorList.Add(Brushes.DarkMagenta)
        colorList.Add(Brushes.Teal)
        colorList.Add(Brushes.MediumSeaGreen)
        colorList.Add(Brushes.Goldenrod)
        colorList.Add(Brushes.Gold)
        colorList.Add(Brushes.Firebrick)
        colorList.Add(Brushes.Chartreuse)
        colorList.Add(Brushes.DarkGoldenrod)
        colorList.Add(Brushes.DarkKhaki)
        colorList.Add(Brushes.Navy)
        colorList.Add(Brushes.ForestGreen)
        colorList.Add(Brushes.LightCoral)
        colorList.Add(Brushes.HotPink)
        colorList.Add(Brushes.SlateGray)
        colorList.Add(Brushes.IndianRed)
        colorList.Add(Brushes.LightSlateGray)
    End Sub
#End Region

#Region "Events"
    Private Sub cbMonth_DropDownClosed(sender As Object, e As EventArgs) Handles cbMonth.DropDownClosed
        month = cbMonth.SelectedValue
        cCanvas.Children.Clear()
        LoadWeeks()
        LoadDataMonthly()
        LoadDataWeekly()
    End Sub

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        year = cbYear.SelectedValue
        cCanvas.Children.Clear()
        LoadWeeks()
        LoadDataMonthly()
        LoadDataWeekly()
    End Sub

    Private Sub cbDateRange_DropDownClosed(sender As Object, e As EventArgs) Handles cbDateRange.DropDownClosed
        cCanvas.Children.Clear()
        LoadDataWeekly()
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
