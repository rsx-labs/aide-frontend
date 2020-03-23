Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows
Imports LiveCharts
Imports LiveCharts.Defaults
Imports LiveCharts.Wpf

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class TrackerViewPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    Private sabacoursemodel As New SabaLearningModel
    Private saba_id As Integer
    Private saba_completed As Integer
    Private saba_not_completed As Integer
    Private isUpdated As Boolean = False
    Private lstPieSeries As List(Of PieSeries)
    Public Property SeriesCollection As SeriesCollection
    Private chartlst As New List(Of Integer)

    Dim lstSabaLearning As SabaLearning()
    Dim SabaLearningListVM As New SabaLearningViewModel()
    Dim SabaLearning As New SabaLearning


#End Region

#Region "Constructor"

    Public Sub New(_sabacoursemodel As SabaLearningModel, _mainframe As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)

        InitializeComponent()
        Me.sabacoursemodel = _sabacoursemodel
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile
        Me.empID = profile.Emp_ID
        SetData()
        Me.DataContext = SabaLearningListVM
        Me.courseTitle.Text = sabacoursemodel.TITLE
        Me.saba_id = sabacoursemodel.SABA_ID

        LoadPieChartData()
        CheckCourseUpdated()
        BindModel(_sabacoursemodel, profile.Emp_ID)
        PermissionSettings()
    End Sub

#End Region

#Region "Functions/Methods"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Public Sub BindModel(SabaModel As SabaLearningModel, empID As Integer)
        SabaLearningListVM.SabaLearningVMModel.EMP_ID = empID
        SabaLearningListVM.SabaLearningVMModel.SABA_ID = SabaModel.SABA_ID
    End Sub

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstSabaLearning = _AideService.GetAllSabaXref(empID, sabacoursemodel.SABA_ID)
                LoadSabaCourses()
                LoadSabaCoursesNot()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub CheckCourseUpdated()
        If isUpdated Then
            UpdateCourseBtn.IsEnabled = False
            dtDate.IsEnabled = False
        End If
    End Sub

    Public Function getDataUpdate(ByVal SabaModel As SabaLearningModel)
        Try
            InitializeService()
            If dtDate.Text = String.Empty Then
            Else
                SabaLearning.SABA_ID = SabaModel.SABA_ID
                SabaLearning.EMP_ID = SabaModel.EMP_ID
                SabaLearning.DATE_COMPLETED = dtDate.Text
            End If
            Return SabaLearning
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return ex
        End Try
    End Function

    Public Sub LoadSabaCourses()
        Try
            Dim lstSabaLearningList As New ObservableCollection(Of SabaLearningModel)
            Dim sabalearningDBProvider As New SabaLearningDBProvider
            Dim objSabaLearning As New SabaLearning


            For Each objsaba As SabaLearning In lstSabaLearning
                If Not objsaba.DATE_COMPLETED = String.Empty Then
                    sabalearningDBProvider._setlistofitems(objsaba)
                    saba_completed += 1
                    If objsaba.EMP_ID = empID Then
                        isUpdated = True
                    End If
                End If
            Next

            chartlst.Add(saba_completed)

            For Each rawUser As mySabaLearningSet In sabalearningDBProvider._getobjSabaLearning()
                lstSabaLearningList.Add(New SabaLearningModel(rawUser))
            Next

            SabaLearningListVM.ObjectCompletedSabaLearningSet = lstSabaLearningList
            lstEmpCompleted.ItemsSource = SabaLearningListVM.ObjectCompletedSabaLearningSet
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadSabaCoursesNot()
        Try
            Dim lstSabaLearningList As New ObservableCollection(Of SabaLearningModel)
            Dim sabalearningDBProvider As New SabaLearningDBProvider
            Dim objSabaLearning As New SabaLearning


            For Each objsaba As SabaLearning In lstSabaLearning
                If objsaba.DATE_COMPLETED = String.Empty Then
                    sabalearningDBProvider._setlistofitems(objsaba)
                    saba_not_completed += 1
                End If
            Next

            chartlst.Add(saba_not_completed)

            For Each rawUser As mySabaLearningSet In sabalearningDBProvider._getobjSabaLearning()
                lstSabaLearningList.Add(New SabaLearningModel(rawUser))
            Next

            SabaLearningListVM.ObjectNotCompletedSabaLearningSet = lstSabaLearningList
            lstEmpNotCompleted.ItemsSource = SabaLearningListVM.ObjectNotCompletedSabaLearningSet
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadPieChartData()

        SeriesCollection = New SeriesCollection From {
        New PieSeries With {
            .Title = "Completed",
            .Values = New ChartValues(Of ObservableValue) From {
                New ObservableValue(saba_completed)
            },
            .DataLabels = True, .Fill = New BrushConverter().ConvertFromString("#FF3EFF6A")
        },
        New PieSeries With {
            .Title = "Not completed",
            .Values = New ChartValues(Of ObservableValue) From {
                New ObservableValue(saba_not_completed)
            },
            .DataLabels = True, .Fill = New BrushConverter().ConvertFromString("#FFFF3838")
        }
    }

        'Dim EmptyValue = New List(Of ChartValues(Of Integer))
        'Dim walanglaman = New ChartValues(Of Integer)
        'walanglaman.Add(0)
        'For Each value As Integer In chartlst
        '    Dim EmptVal As New ChartValues(Of Integer)
        '    EmptVal.Add(value)
        '    EmptyValue.Add(EmptVal)
        'Next

        'For Each chartval As ChartValues(Of Integer) In EmptyValue
        '    Dim _pieSeries As New PieSeries
        '    _pieSeries.Title = "Completed"
        '    _pieSeries.Values = chartval
        '    _pieSeries.DataLabels = True
        '    SeriesCollection.Add(_pieSeries)
        'Next
        'EmptyValue.

        'For Each pie As PieSeries In SeriesCollection
        '    If pie.Values = walanglaman Then
        '        pie.Visibility = Windows.Visibility.Collapsed
        '    End If
        'Next
        DataContext = Me
    End Sub

    Private Sub PermissionSettings()
        Dim guestAccount As Integer = 5

        If profile.Permission_ID = guestAccount Then
            gUpdateControls.Visibility = Windows.Visibility.Hidden
        End If

    End Sub
#End Region

#Region "Events"
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub UpdateCourseBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()

            _AideService.UpdateSabaXref(getDataUpdate(SabaLearningListVM.SabaLearningVMModel))
            If SabaLearning.DATE_COMPLETED = Nothing Then
                MsgBox("Please Fill Up All Fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                addframe.Navigate(New TrackerViewPage(sabacoursemodel, mainframe, addframe, menugrid, submenuframe, profile))
            Else
                MsgBox("Tracker has been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")

                SabaLearning.DATE_COMPLETED = Nothing
                SabaLearning.SABA_ID = Nothing
                SabaLearning.EMP_ID = Nothing

                addframe.Navigate(New TrackerViewPage(sabacoursemodel, mainframe, addframe, menugrid, submenuframe, profile))

                'mainframe.Navigate(New SabaLearningMainPage(mainframe, empID, addframe, menugrid, submenuframe))
                'mainframe.IsEnabled = True
                'mainframe.Opacity = 1
                'menugrid.IsEnabled = True
                'menugrid.Opacity = 1
                'submenuframe.IsEnabled = True
                'submenuframe.Opacity = 1

                'addframe.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub UpdateEndDateBtn_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New TrackerUpdatePage(mainframe, addframe, menugrid, submenuframe, sabacoursemodel, profile))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(150, 150, 150, 150)
    End Sub

    Private Sub btnCCancel_Click(sender As Object, e As RoutedEventArgs)
        mainframe.Navigate(New SabaLearningMainPage(mainframe, profile, addframe, menugrid, submenuframe))
        mainframe.IsEnabled = True
        mainframe.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1

        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "INotify Methods"
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
