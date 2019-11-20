Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' BY GIANN CARLO CAMILO / JHUNELL BARCENAS
''' </summary>
''' <remarks></remarks>

<CallbackBehavior(
ConcurrencyMode:=ConcurrencyMode.Single,
UseSynchronizationContext:=False)>
Public Class NewProject
    Implements ServiceReference1.IAideServiceCallback


#Region "Fields"
    Private _AideServiceClient As ServiceReference1.AideServiceClient
    Private _EmployeeListViewModel As New EmployeeListViewModel
    Private _employeeList As New ObservableCollection(Of EmployeeModel)
    Private _ProjectViewModel As New ProjectViewModel
    Public email As String
    Private _Frame As Frame
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
        profile = _profile
        email = profile.Email_Address
        Me._Frame = _frame
        ' Add any initialization after the InitializeComponent() call.
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me._empID = profile.Emp_ID
        InitializeService()
        LoadEmployeeList()
        LoadAllProjectName()
    End Sub
#End Region

#Region "Methods"
    Public Sub LoadEmployeeList()
        Try
            Dim _EmployeeListDBProvider As New EmployeeListProvider

            Dim _Employee1DBProvider As New EmployeeProvider1
            Dim lstEmployees As Employee() = _AideServiceClient.GetNicknameByDeptID(email)

            For Each objEmployee As Employee In lstEmployees
                _EmployeeListDBProvider.SetEmployeeList(objEmployee)
                _Employee1DBProvider.SetEmployeeLists(objEmployee)
            Next

            Dim lstEmployee As New ObservableCollection(Of EmployeeListModel)
            For Each iEmployee As MyEmployeeList In _EmployeeListDBProvider.GetEmployeeList()
                lstEmployee.Add(New EmployeeListModel(iEmployee))
            Next
            _EmployeeListViewModel.EmployeeList = lstEmployee

            Me.DataContext = _EmployeeListViewModel
            'for create project
            Dim lstEmployee2 As New ObservableCollection(Of EmployeeListModel)
            For Each iEmployee2 As MyEmployeeList In _Employee1DBProvider.GetEmployeesLists()
                lstEmployee2.Add(New EmployeeListModel(iEmployee2))
            Next
            _ProjectViewModel.EmployeeLists = lstEmployee2
            Me.DataContext = _ProjectViewModel
        Catch ex As SystemException
            _AideServiceClient.Abort()
        End Try
    End Sub

    Public Sub LoadAllProjectName()
        Try
            InitializeService()
            Dim _GetAllConcernDBProvider As New ProjectDBProvider
            Dim _projectViewModel As New ProjectViewModel

            Dim displayStatus As Integer = 0
            Dim lstProject As Project() = _AideServiceClient.GetAllListOfProject(_empID, displayStatus)
            Dim lstProjectList As New ObservableCollection(Of ProjectModel)

            For Each objProject As Project In lstProject
                _GetAllConcernDBProvider.setProjectList(objProject)
            Next

            For Each iProject As myProjectList In _GetAllConcernDBProvider.getProjectList()
                lstProjectList.Add(New ProjectModel(iProject))
            Next

            _projectViewModel.ProjectList = lstProjectList

            cbProjectName.DataContext = _projectViewModel
        Catch ex As SystemException
            MsgBox(ex.Message)
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

    Private Function LoadAllProjectNameByID()
        Dim _setSelectedProject As New ProjectViewModel

        Dim getID As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectID
        Dim getProjCd As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectCode
        Dim getCategory As Integer = CType(cbProjectName.SelectedItem, ProjectModel).Category
        Dim getBillability As Integer = CType(cbProjectName.SelectedItem, ProjectModel).Billability

        If getCategory = 0 Then
            _ProjectViewModel.SelectedProject.Category = "Task"
        Else
            _ProjectViewModel.SelectedProject.Category = "Project"
        End If

        _ProjectViewModel.SelectedProject.ProjectID = getID
        _ProjectViewModel.SelectedProject.ProjectCode = getProjCd


        If getBillability = 0 Then
            _ProjectViewModel.SelectedProject.Billability = "Internal"
        Else
            _ProjectViewModel.SelectedProject.Billability = "External"

        End If

        _setSelectedProject = _ProjectViewModel
        Me.DataContext = _setSelectedProject
        Return _setSelectedProject
    End Function

    Private Sub LoadAssignedEmployees()
        Dim projID As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectID
        Dim lstAssignedEmployees As Object = _AideServiceClient.GetAssignedProjects(projID)
        If Not IsNothing(lstAssignedEmployees) Then
            For Each assigned As AssignedProject In lstAssignedEmployees

                For Each rawEmployee As EmployeeListModel In _EmployeeListViewModel.EmployeeList
                    If rawEmployee.EmpID = assigned.EmployeeID Then
                        rawEmployee.DateStarted = assigned.StartPeriod
                        rawEmployee.DateFinished = assigned.EndPeriod
                        _ProjectViewModel.AssignedEmployeeLists.Add(rawEmployee)

                        Exit For
                    End If
                Next

                For Each rawEmployee As EmployeeListModel In _ProjectViewModel.EmployeeLists
                    If rawEmployee.EmpID = assigned.EmployeeID Then
                        '_EmployeeListViewModel.EmployeeList.Remove(rawEmployee)
                        _ProjectViewModel.EmployeeLists.Remove(rawEmployee)
                        _ProjectViewModel.RemoveMode = True
                        Exit For
                    End If
                Next
            Next
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        _Frame.Navigate(New ViewProjectUI(_Frame, profile, _addframe, _menugrid, _submenuframe))

        _Frame.IsEnabled = True
        _Frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub cbProjectName_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProjectName.SelectionChanged

        ResetEmployeeList()
        LoadAllProjectNameByID()
        LoadAssignedEmployees()
    End Sub
#End Region

#Region "INotfiy Methods"
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

    Private Sub ResetEmployeeList()
        If _ProjectViewModel.AssignedEmployeeLists.Count > 0 Then
            For Each rawEmployee As EmployeeListModel In _ProjectViewModel.AssignedEmployeeLists
                _ProjectViewModel.EmployeeLists.Add(rawEmployee)
            Next
            _ProjectViewModel.EmployeeLists = New ObservableCollection(Of EmployeeListModel)(_ProjectViewModel.EmployeeLists.OrderBy(Function(f) f.Name).ToList())

            grdEmployees.Items.Refresh()
            _ProjectViewModel.AssignedEmployeeLists.Clear()

        End If
    End Sub
#End Region

End Class
