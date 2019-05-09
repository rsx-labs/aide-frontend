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

    Dim month As Integer = Date.Now.Month
    Dim setStatus As Integer
    Dim displayStatus As String = String.Empty
    Dim status As Integer
    Dim img As String
    Dim slStatus As Integer = 3
    Dim vlStatus As Integer = 4
    Dim displayData As Integer = 2
    Dim displayMonth As String
    Dim checkStatus As Integer
    Dim count As Integer
    Dim year As Integer
    Dim day As Integer
    Dim displayOption As Integer = 1 'Weekly is the Default Display Options
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
        lblMonthVL.Content = lblMonthVL.Content + " " + MonthName(month)
        lblYear.Content = lblYear.Content + " " + MonthName(month)
        'LoadDataSLMonthly()
        'LoadDataVLMonthly()

        LoadStackSL()
        LoadStackVL()
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

    Public Property SeriesCollection As SeriesCollection
    Public Property Labels As String()
    Public Property Formatter As Func(Of Object, Object)

    Public Property SeriesCollectionVL As SeriesCollection
    Public Property LabelsVL As String()
    Public Property FormatterVL As Func(Of Object, Object)

    Private Sub LoadStackSL()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(profile.Email_Address, slStatus, displayData)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()
            Dim UsedSL As New ChartValues(Of Double)()
            Dim TotalBalance As New ChartValues(Of Double)()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim employee(lstresource.Length) As String
            Dim usedLeaves(lstresource.Length) As String
            Dim i As Integer = 0

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                UsedSL.Add(iResource.UsedVL)
                employee(i) = iResource.Emp_Name
                i += 1
            Next

            SeriesCollection = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = UsedSL,
                    .StackMode = StackMode.Values,
                    .Fill = Brushes.Red,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Used SL"
                }
            }

            Labels = employee
            Formatter = Function(value) value
            DataContext = Me
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadStackVL()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource = client.GetResourcePlanner(profile.Email_Address, vlStatus, displayData)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()
            Dim UsedVL As New ChartValues(Of Double)()
            Dim HalfBalance As New ChartValues(Of Double)()
            Dim TotalBalance As New ChartValues(Of Double)()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim employee(lstresource.Length) As String
            Dim usedLeaves(lstresource.Length) As String
            Dim i As Integer = 0

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                UsedVL.Add(iResource.UsedVL)
                employee(i) = iResource.Emp_Name
                i += 1
            Next

            SeriesCollectionVL = New SeriesCollection From {
                New StackedColumnSeries With {
                    .Values = UsedVL,
                    .StackMode = StackMode.Values,
                    .DataLabels = True,
                    .LabelsPosition = BarLabelPosition.Perpendicular,
                    .Title = "Used VL"
                }
            }

            LabelsVL = employee
            FormatterVL = Function(value) value
            DataContext = Me
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadDataSLMonthly()
        'Try
        '    InitializeService()
        '    _ResourceDBProvider._splist.Clear()
        '    Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(profile.Email_Address, 3, 2)
        '    Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
        '    Dim resourceListVM As New ResourcePlannerViewModel()

        '    For Each objResource As ResourcePlanner In lstresource
        '        _ResourceDBProvider.SetAllEmpRPList(objResource)
        '    Next

        '    For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
        '        resourcelist.Add(New ResourcePlannerModel(iResource))

        '    Next
        '    resourceListVM.ResourceListLeaveCredits = Nothing
        '    resourceListVM.ResourceListLeaveCredits = resourcelist
        '    SLMChartSeries.ItemsSource = resourcelist
        'Catch ex As Exception
        '    MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        'End Try
    End Sub

    Public Sub LoadDataVLMonthly()
        'Try
        '    InitializeService()
        '    _ResourceDBProvider._splist.Clear()
        '    Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(profile.Email_Address, 4, 2)
        '    Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
        '    Dim resourceListVM As New ResourcePlannerViewModel()

        '    For Each objResource As ResourcePlanner In lstresource
        '        _ResourceDBProvider.SetAllEmpRPList(objResource)
        '    Next

        '    For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
        '        resourcelist.Add(New ResourcePlannerModel(iResource))

        '    Next
        '    resourceListVM.ResourceListLeaveCredits = Nothing
        '    resourceListVM.ResourceListLeaveCredits = resourcelist
        '    VLMChartSeries.ItemsSource = resourcelist
        'Catch ex As Exception
        '    MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        'End Try
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
