Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports UI_AIDE_CommCellServices.ServiceReference1
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AssignedProjectAddPage
    Implements IAideServiceCallback

#Region "Declarations"
    Private _aide As ServiceReference1.AideServiceClient
    Private _employeeListVM As New EmployeeListViewModel
    Private _employeeList As New ObservableCollection(Of EmployeeModel)
    Private _ProjectVM As New ProjectViewModel
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
        Me._profile = _profile
        Me._email = _profile.Email_Address
        Me._mainFrame = _mainframe
        ' Add any initialization after the InitializeComponent() call.
        Me._addFrame = _addframe
        Me._menuGrid = _menugrid
        Me._subMenuFrame = _submenuframe
        Me._empID = _profile.Emp_ID
        InitializeService()
        LoadEmployeeList()
        LoadAllProjectName()
    End Sub
#End Region

#Region "Methods"
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

    Public Sub LoadEmployeeList()
        Try
            Dim _EmployeeListDBProvider As New EmployeeListProvider

            Dim _Employee1DBProvider As New EmployeeProvider1
            Dim lstEmployees As Employee() = _aide.GetNicknameByDeptID(_email)

            For Each objEmployee As Employee In lstEmployees
                _EmployeeListDBProvider.SetEmployeeList(objEmployee)
                _Employee1DBProvider.SetEmployeeLists(objEmployee)
            Next

            Dim lstEmployee As New ObservableCollection(Of EmployeeListModel)
            For Each iEmployee As MyEmployeeList In _EmployeeListDBProvider.GetEmployeeList()
                lstEmployee.Add(New EmployeeListModel(iEmployee))
            Next
            _employeeListVM.EmployeeList = lstEmployee

            Me.DataContext = _employeeListVM
            'for create project
            Dim lstEmployee2 As New ObservableCollection(Of EmployeeListModel)
            For Each iEmployee2 As MyEmployeeList In _Employee1DBProvider.GetEmployeesLists()
                lstEmployee2.Add(New EmployeeListModel(iEmployee2))
            Next
            _ProjectVM.EmployeeLists = lstEmployee2
            Me.DataContext = _ProjectVM
        Catch ex As SystemException
            _aide.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadAllProjectName()
        Try
            InitializeService()
            Dim _GetAllConcernDBProvider As New ProjectDBProvider
            Dim _projectViewModel As New ProjectViewModel

            Dim displayStatus As Integer = 0
            Dim lstProject As Project() = _aide.GetAllListOfProject(_empID, displayStatus)
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            _aide.Abort()
        End Try
    End Sub

    Private Function LoadAllProjectNameByID()
        Dim _setSelectedProject As New ProjectViewModel

        Dim getID As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectID
        Dim getProjCd As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectCode
        Dim getCategory As Integer = CType(cbProjectName.SelectedItem, ProjectModel).Category
        Dim getBillability As Integer = CType(cbProjectName.SelectedItem, ProjectModel).Billability

        If getCategory = 0 Then
            _ProjectVM.SelectedProject.Category = "Task"
        Else
            _ProjectVM.SelectedProject.Category = "Project"
        End If

        _ProjectVM.SelectedProject.ProjectID = getID
        _ProjectVM.SelectedProject.ProjectCode = getProjCd


        If getBillability = 0 Then
            _ProjectVM.SelectedProject.Billability = "Internal"
        Else
            _ProjectVM.SelectedProject.Billability = "External"

        End If

        _setSelectedProject = _ProjectVM
        Me.DataContext = _setSelectedProject
        Return _setSelectedProject
    End Function

    Private Sub LoadAssignedEmployees()
        Dim projID As Integer = CType(cbProjectName.SelectedItem, ProjectModel).ProjectID
        Dim lstAssignedEmployees As Object = _aide.GetAssignedProjects(projID)
        If Not IsNothing(lstAssignedEmployees) Then
            For Each assigned As AssignedProject In lstAssignedEmployees

                For Each rawEmployee As EmployeeListModel In _employeeListVM.EmployeeList
                    If rawEmployee.EmpID = assigned.EmployeeID Then
                        rawEmployee.DateStarted = assigned.StartPeriod
                        rawEmployee.DateFinished = assigned.EndPeriod
                        _ProjectVM.AssignedEmployeeLists.Add(rawEmployee)

                        Exit For
                    End If
                Next

                For Each rawEmployee As EmployeeListModel In _ProjectVM.EmployeeLists
                    If rawEmployee.EmpID = assigned.EmployeeID Then
                        '_EmployeeListViewModel.EmployeeList.Remove(rawEmployee)
                        _ProjectVM.EmployeeLists.Remove(rawEmployee)
                        _ProjectVM.RemoveMode = True
                        Exit For
                    End If
                Next
            Next
        End If
    End Sub

    Private Sub ResetEmployeeList()
        If _ProjectVM.AssignedEmployeeLists.Count > 0 Then
            For Each rawEmployee As EmployeeListModel In _ProjectVM.AssignedEmployeeLists
                _ProjectVM.EmployeeLists.Add(rawEmployee)
            Next
            _ProjectVM.EmployeeLists = New ObservableCollection(Of EmployeeListModel)(_ProjectVM.EmployeeLists.OrderBy(Function(f) f.Name).ToList())

            grdEmployees.Items.Refresh()
            _ProjectVM.AssignedEmployeeLists.Clear()

        End If
    End Sub
#End Region

#Region "Events"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        _mainFrame.Navigate(New AssignedProjectMainPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame))
        _mainFrame.IsEnabled = True
        _mainFrame.Opacity = 1
        _menuGrid.IsEnabled = True
        _menuGrid.Opacity = 1
        _subMenuFrame.IsEnabled = True
        _subMenuFrame.Opacity = 1
        _addFrame.Visibility = Visibility.Hidden
    End Sub

    Private Sub cbProjectName_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProjectName.SelectionChanged
        ResetEmployeeList()
        LoadAllProjectNameByID()
        LoadAssignedEmployees()
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
