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

Public Class BillabilitySickLeavePage
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
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
        SetTitle()
        LoadDataSLYearly()
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

    Public Sub LoadDataSLYearly()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()
            Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(profile.Email_Address, 3, 3)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))

            Next
            resourceListVM.ResourceListLeaveCredits = Nothing
            resourceListVM.ResourceListLeaveCredits = resourcelist
            SLYChartSeries.ItemsSource = resourcelist
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetTitle()

        Dim nextYear As Integer = year + 1
        Dim prevYear As Integer = year - 1

        If Date.Now.Month >= 4 Then
            yearlySL.Title = yearlySL.Title + " " + year.ToString + "-" + nextYear.ToString
        Else
            yearlySL.Title = yearlySL.Title + " " + prevYear.ToString + "-" + year.ToString
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
