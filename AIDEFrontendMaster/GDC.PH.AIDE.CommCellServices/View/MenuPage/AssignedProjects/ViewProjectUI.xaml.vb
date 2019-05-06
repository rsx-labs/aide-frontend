﻿Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel


''' <summary>
''' BY GIANN CARLO CAMILO 
''' </summary>
''' <remarks></remarks>

<CallbackBehavior(
ConcurrencyMode:=ConcurrencyMode.Single,
UseSynchronizationContext:=False)>
Class ViewProjectUI
    Implements ServiceReference1.IAideServiceCallback


    Private _AideServiceClient As ServiceReference1.AideServiceClient
    Private _ViewProjectProvider As New ViewProjectProvider
    Private _ViewProjectViewModel As New ViewProjectViewModel
    Private FrameNavi As Frame
    Public email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame

    Public Sub New(_frame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        InitializeService()
        Me.DataContext = _ViewProjectViewModel
        Me.FrameNavi = _frame
        email = _email
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        LoadProjectList()
    End Sub


    Public Sub LoadProjectList()
        Try
            Dim lstProjects As ViewProject() = _AideServiceClient.ViewProjectListofEmployee()
            For Each objProject As ViewProject In lstProjects
                _ViewProjectProvider.SetProjectList(objProject)
            Next
            Dim lstProject As New ObservableCollection(Of ViewProjectModel)
            For Each iProject As MyProjectLists In _ViewProjectProvider.GetProjectList()
                lstProject.Add(New ViewProjectModel(iProject))
            Next
            _ViewProjectViewModel.ProjectList = lstProject
        Catch ex As SystemException
            _AideServiceClient.Abort()
        End Try

    End Sub


    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideServiceClient = New AideServiceClient(Context)
            _AideServiceClient.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _AideServiceClient.Abort()
        End Try
        Return bInitialize
    End Function






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

    Private Sub btnAssign_Click(sender As Object, e As RoutedEventArgs) Handles btnAssign.Click
        _addframe.Navigate(New NewProject(FrameNavi, email, _addframe, _menugrid, _submenuframe))
        FrameNavi.IsEnabled = False
        FrameNavi.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(100, 50, 100, 50)
        _addframe.Visibility = Visibility.Visible
    End Sub
End Class
