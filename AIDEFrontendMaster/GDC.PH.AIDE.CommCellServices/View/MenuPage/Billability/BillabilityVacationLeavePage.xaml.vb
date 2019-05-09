﻿Imports UI_AIDE_CommCellServices.ServiceReference1
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

    Dim month As Integer = Date.Now.Month
    Dim setStatus As Integer
    Dim displayStatus As String = String.Empty
    Dim displayFiscalYear As Integer = 3
    Dim status As Integer
    Dim vlStatus As Integer = 4
    Dim img As String
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
        SetTitle()
        'LoadDataVLYearly()
        LoadStack()
        'LoadAllCategory()
    End Sub

    Public Property SeriesCollection As SeriesCollection
    Public Property Labels As String()
    Public Property Formatter As Func(Of Object, Object)

    Private Sub LoadStack()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()

            Dim lstresource = client.GetResourcePlanner(profile.Email_Address, vlStatus, displayFiscalYear)
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
                HalfBalance.Add(iResource.HalfBalance)
                TotalBalance.Add(iResource.TotalBalance)
                employee(i) = iResource.Emp_Name
                i += 1
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
                    .Title = "Half Balance"
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
            
            Labels = employee
            Formatter = Function(value) value
            DataContext = Me
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
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
            lblYear.Content = lblYear.Content + " " + year.ToString + "-" + nextYear.ToString
        Else
            lblYear.Content = lblYear.Content + " " + prevYear.ToString + "-" + year.ToString
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