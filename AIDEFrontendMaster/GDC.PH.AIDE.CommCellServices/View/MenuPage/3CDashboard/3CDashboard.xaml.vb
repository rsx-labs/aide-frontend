Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
Imports System
Imports System.Linq
Imports System.Windows
Imports LiveCharts
Imports LiveCharts.Defaults
Imports LiveCharts.Wpf


<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class _3CDashboard
    Implements ServiceReference1.IAideServiceCallback


    Public _AIDEClientService As ServiceReference1.AideServiceClient
    Private email As String
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private profile As Profile

    Private PastdueDate As Integer = 0
    Private DuetodayDate As Integer = 0
    Private CompletedDate As Integer = 0

    Private max As Integer
    Private incVal As Integer = 0
    Private isSearchIsUsed As Integer = 0
    Private isDateBetweenUsed As Integer = 0


#Region "constructor"
    Public Sub New(_email As String, _frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)

        ' This call is required by the designer.
        InitializeComponent()
        Me.InitializeService()

        Dim offsetVal As Integer = 0
        Dim nextVal As Integer = 10
        email = _email
        frame = _frame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        profile = _profile
        CalculateConcernList(offsetVal, nextVal)
        LoadPieChartData()

        'UpdateAllOnClick()


        ' Add any initialization after the InitializeComponent() call.

    End Sub
#End Region



#Region "Service methods"
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

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AIDEClientService = New AideServiceClient(Context)
            _AIDEClientService.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _AIDEClientService.Abort()
        End Try
        Return bInitialize
    End Function


    Public Sub CalculateConcernList(offSet As Integer, NextVal As Integer)

        Try
            Dim lstConcern As Concern() = _AIDEClientService.selectAllConcern(email, offSet, NextVal)

            For Each objConcern As Concern In lstConcern
                Select Case objConcern.Due_Date
                    Case Is < Date.Today()
                        If objConcern.Status = "OPEN" Then
                            PastdueDate += 1
                        End If
                    Case Date.Today()
                        If objConcern.Status = "OPEN" Then
                            DuetodayDate += 1
                        End If
                    Case Is > Date.Today()
                        If objConcern.Status = "OPEN" Then
                            CompletedDate += 1
                        End If

                End Select
            Next

            'PastDue.Text = PastdueDate.ToString()
            'DueToday.Text = DuetodayDate.ToString()
            'Completed.Text = CompletedDate.ToString()

        Catch ex As SystemException

            MsgBox(ex.Message)
            _AIDEClientService.Abort()

        End Try
    End Sub

    Public Sub CheckData()
        If PastdueDate = 0 AndAlso DuetodayDate = 0 AndAlso CompletedDate = 0 Then
            NoConcernLbl.Visibility = Windows.Visibility.Visible
            NoConcernImg.Visibility = Windows.Visibility.Visible
            Chart.Visibility = Windows.Visibility.Hidden
            Chart.ChartLegend.Visibility = Windows.Visibility.Hidden
        Else
            NoConcernImg.Visibility = Windows.Visibility.Hidden
            NoConcernLbl.Visibility = Windows.Visibility.Hidden
            Chart.Visibility = Windows.Visibility.Visible
            Chart.ChartLegend.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub LoadPieChartData()
        SeriesCollection = New SeriesCollection From {
        New PieSeries With {
            .Title = "Past due and open",
            .Values = New ChartValues(Of ObservableValue) From {
                New ObservableValue(PastdueDate)
            },
            .DataLabels = True, .Fill = New BrushConverter().ConvertFromString("#FFFF3838")
        },
        New PieSeries With {
            .Title = "Due today and open",
            .Values = New ChartValues(Of ObservableValue) From {
                New ObservableValue(DuetodayDate)
            },
            .DataLabels = True, .Fill = New BrushConverter().ConvertFromString("#FFFDD652")
        },
        New PieSeries With {
            .Title = "Open",
            .Values = New ChartValues(Of ObservableValue) From {
                New ObservableValue(CompletedDate)
            },
            .DataLabels = True, .Fill = New BrushConverter().ConvertFromString("#FF3EFF6A")
        }
    }
        DataContext = Me
        CheckData()
    End Sub

    Public Property SeriesCollection As SeriesCollection

    Private Sub btnthreeC(sender As Object, e As RoutedEventArgs)
        frame.Navigate(New ThreeC_Page(email, frame, addframe, menugrid, submenuframe))
        submenuframe.Navigate(New ImproveSubMenuPage(frame, email, Profile, addframe, menugrid, submenuframe))
    End Sub



End Class



