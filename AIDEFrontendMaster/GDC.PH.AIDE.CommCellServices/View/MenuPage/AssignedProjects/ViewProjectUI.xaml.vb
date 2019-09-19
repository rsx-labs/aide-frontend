Imports UI_AIDE_CommCellServices.ServiceReference1
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


#Region "Fields"
    Private _AideServiceClient As ServiceReference1.AideServiceClient
    Private _ViewProjectProvider As New ViewProjectProvider
    Private _ViewProjectViewModel As New ViewProjectViewModel
    Private FrameNavi As Frame
    Public email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _empID As Integer
    Private profile As Profile
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        InitializeService()
        Me.DataContext = _ViewProjectViewModel
        Me.FrameNavi = _frame
        Me.profile = _profile
        email = profile.Email_Address
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me._empID = profile.Emp_ID
        LoadProjectList()
    End Sub
#End Region

#Region "Methods/Functions"
    Public Sub LoadProjectList()
        Try
            Dim lstProjects As ViewProject() = _AideServiceClient.ViewProjectListofEmployee(_empID)
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
#End Region

#Region "Events"
    Private Sub btnAssign_Click(sender As Object, e As RoutedEventArgs) Handles btnAssign.Click
        _addframe.Navigate(New NewProject(FrameNavi, profile, _addframe, _menugrid, _submenuframe))
        FrameNavi.IsEnabled = False
        FrameNavi.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 60, 150, 60)
        _addframe.Visibility = Visibility.Visible
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
