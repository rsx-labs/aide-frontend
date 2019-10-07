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

Public Class BillabilityVacationLeavePage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame
    Private profile As Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _attendanceFrame As Frame

    Dim month As Integer = Date.Now.Month
    Dim displayFiscalYear As Integer = 3
    Dim vlStatus As Integer = 4
    Dim year As Integer
    Dim day As Integer
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame, attendanceFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me._addframe = addframe
        Me._menugrid = menugrid
        Me._submenuframe = submenuframe
        Me._attendanceFrame = attendanceFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year

        LoadYears()
        LoadData()

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
            lblYear.Content = "Vacation Leave For Fiscal Year " + year.ToString + "-" + nextYear.ToString
        Else
            lblYear.Content = "Vacation Leave For Fiscal Year " + prevYear.ToString + "-" + year.ToString
        End If
    End Sub

    Public Sub LoadYears()
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

    Private Sub LoadData()
        LoadDataVLYearly()
        SetTitle()
    End Sub

    Private Sub LoadDataVLYearly()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource = client.GetResourcePlanner(profile.Email_Address, vlStatus, displayFiscalYear, year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()
            Dim UsedVL As New ChartValues(Of Double)()
            Dim HalfBalance As New ChartValues(Of Double)()
            Dim TotalBalance As New ChartValues(Of Double)()
            Dim SeriesCollection As SeriesCollection

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim employee(lstresource.Length) As String
            Dim usedLeaves(lstresource.Length) As String
            Dim i As Integer = 0

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                UsedVL.Add(iResource.UsedVL)
                HalfBalance.Add(iResource.HalfBalance)
                TotalBalance.Add(iResource.TotalBalance)
                employee(i) = iResource.Emp_Name
                i += 1
                If Me.profile.Emp_ID = iResource.Emp_ID Then
                    Dim totRemBal As Double = iResource.HalfBalance + iResource.TotalBalance
                    TxtRemBalance.Text = totRemBal.ToString()
                End If
            Next

            SeriesCollection = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = UsedVL,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Used VL"
                },
                New StackedColumnSeries With {
                    .Values = HalfBalance,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Half Balance",
                    .Fill = Brushes.Goldenrod
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

            chartVL.Series = SeriesCollection
            chartVL.AxisX.First().Labels = employee
            chartVL.AxisY.First().LabelFormatter = Function(value) value
            chartVL.AxisX.First().LabelsRotation = 135

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub
#End Region

#Region "Private Functions"

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        If Not cbYear.SelectedIndex = -1 Then
            year = cbYear.SelectedValue
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

    Private Sub btnManage_Click(sender As Object, e As RoutedEventArgs)
        _addframe.Navigate(New BillabilityManagerVLLeavePage(profile, mainFrame, _addframe, _menugrid, _submenuframe, _attendanceFrame))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(100, 60, 100, 60)
        _addframe.Visibility = Visibility.Visible
    End Sub
End Class
