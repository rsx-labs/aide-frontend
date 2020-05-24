Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AssignedProjectMainPage
    Implements ServiceReference1.IAideServiceCallback
#Region "Declarations"
    Private _aide As ServiceReference1.AideServiceClient
    Private _assignedProjDB As New AssignedProjectDBProvider
    Private _assignedProjVM As New AssignedProjectViewModel
    Private _profile As Profile

    Private _mainFrame As Frame
    Private _addFrame As Frame
    Private _subMenuFrame As Frame
    Private _menuGrid As Grid

    Public _email As String
    Private _empID As Integer
#End Region
#Region "Constructor"
    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        'InitializeService()
        Me.DataContext = _assignedProjVM
        Me._mainFrame = _mainframe
        Me._profile = _profile
        Me._email = _profile.Email_Address
        Me._addFrame = _addframe
        Me._menuGrid = _menugrid
        Me._subMenuFrame = _submenuframe
        Me._empID = _profile.Emp_ID
        LoadAssignedProjectList()
        If _profile.Permission_ID = 1 Then
            btnAssign.Visibility = Visibility.Visible
        End If
    End Sub
#End Region
#Region "Methods/Functions"
    Public Sub LoadAssignedProjectList()
        Try
            Dim lstAssignedProjects As ViewProject() = AideClient.GetClient().ViewProjectListofEmployee(_empID)
            For Each objAssigned As ViewProject In lstAssignedProjects
                _assignedProjDB.SetAssignedProjectList(objAssigned)
            Next
            Dim lstAssignedProj As New ObservableCollection(Of AssignedProjectModel)
            For Each iAssigned As MyAssignedProjectLists In _assignedProjDB.GetAssignedProjectList()
                lstAssignedProj.Add(New AssignedProjectModel(iAssigned))
            Next

            _assignedProjVM.AssignedProjectList = lstAssignedProj
            AssignedProjectGrid.ItemsSource = _assignedProjVM.AssignedProjectList
        Catch ex As SystemException
            _aide.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _aide = New AideServiceClient(Context)
            _aide.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _aide.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function
#End Region
#Region "Events"
    Private Sub btnAssign_Click(sender As Object, e As RoutedEventArgs) Handles btnAssign.Click
        _addFrame.Navigate(New AssignedProjectAddPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame))
        _mainFrame.IsEnabled = False
        _mainFrame.Opacity = 0.3
        _menuGrid.IsEnabled = False
        _menuGrid.Opacity = 0.3
        _subMenuFrame.IsEnabled = False
        _subMenuFrame.Opacity = 0.3
        _addFrame.Margin = New Thickness(100, 60, 100, 60)
        _addFrame.Visibility = Visibility.Visible
    End Sub
#End Region
#Region "Notification Methods"
    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate
        Throw New NotImplementedException()
    End Sub
#End Region
End Class
