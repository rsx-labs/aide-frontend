Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports System.Data.SqlClient
Imports System.Data
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1


''' <summary>
'''BY GIANN CARLO CAMILO 
''' </summary>
''' <remarks></remarks>

<CallbackBehavior(
ConcurrencyMode:=ConcurrencyMode.Single,
UseSynchronizationContext:=False)>
Public Class ProjectViewModel
    Implements INotifyPropertyChanged, IAideServiceCallback


    Private _categoryList As New ObservableCollection(Of CategoryModel)

    Public _AideServiceClient As ServiceReference1.AideServiceClient
    Private _db As New EmployeeListProvider
    Private _selectedEmployee As EmployeeModel
    Private _selectedEmployees As EmployeeListModel
    Private _selectedAssignedEmployee As EmployeeModel
    Private _selectedAssignedEmployees As EmployeeListModel
    Private _employeeList As New ObservableCollection(Of EmployeeModel)
    Private _employeeLists As New ObservableCollection(Of EmployeeListModel)
    Public _assignedEmployeeList As New ObservableCollection(Of EmployeeModel)
    Public _assignedEmployeeLists As New ObservableCollection(Of EmployeeListModel)

    Private _editCommand As ICommand
    Private _deleteCommand As ICommand
    Private _removeCommand As ICommand
    Private _addCommand As ICommand
    Private _saveCommand As ICommand
    Private _clearCommand As ICommand
    Private _createCommand As ICommand

    Private _projectName As String
    Private _projectCategory As Integer
    Private _projectBillability As Integer
    Private _projectStart As String
    Private _projectEnd As String
   
    Private _productList As New ObservableCollection(Of ProjectModel)
    Private _selectedProject As New ProjectModel

    Sub New()

        'populate some data (NOTE: Data are not yet from the database.)
        Try
            For Each rawUser As MyEmployee In _db.GetEmployees()
                _employeeList.Add(New EmployeeModel(rawUser))
            Next
        Catch ex As Exception
            Console.Write(ex.Message)
        End Try

        AddMode = True

        RemoveMode = True

        ClearMode = False

        InitializeService()
    End Sub

#Region "Properties"

    Public Property CategoryList As ObservableCollection(Of CategoryModel)
        Get
            Return _categoryList
        End Get
        Set(value As ObservableCollection(Of CategoryModel))
            _categoryList = value
            NotifyPropertyChanged("CategoryList")
        End Set
    End Property


    Public Property ProjectList As ObservableCollection(Of ProjectModel)
        Get
            Return _productList
        End Get
        Set(value As ObservableCollection(Of ProjectModel))
            _productList = value
            NotifyPropertyChanged("ProjectList")
        End Set
    End Property


    Public Property SelectedProject As ProjectModel
        Get
            Return _selectedProject
        End Get
        Set(value As ProjectModel)
            _selectedProject = value
            NotifyPropertyChanged("SelectedProject")
        End Set
    End Property


    Public Property EmployeeList As ObservableCollection(Of EmployeeModel)
        Get
            Return _employeeList
        End Get
        Set(value As ObservableCollection(Of EmployeeModel))
            _employeeList = value
            NotifyPropertyChanged("EmployeeList")
        End Set
    End Property

    Public Property EmployeeLists As ObservableCollection(Of EmployeeListModel)
        Get
            Return _employeeLists
        End Get
        Set(value As ObservableCollection(Of EmployeeListModel))
            _employeeLists = value
            NotifyPropertyChanged("EmployeeLists")
        End Set
    End Property


    Public Property AssignedEmployeeList As ObservableCollection(Of EmployeeModel)
        Get
            Return _assignedEmployeeList
        End Get
        Set(value As ObservableCollection(Of EmployeeModel))
            _assignedEmployeeList = value
            NotifyPropertyChanged("AssignedEmployeeList")
        End Set
    End Property

    Public Property AssignedEmployeeLists As ObservableCollection(Of EmployeeListModel)
        Get
            Return _assignedEmployeeLists
        End Get
        Set(value As ObservableCollection(Of EmployeeListModel))
            _assignedEmployeeLists = value
            NotifyPropertyChanged("AssignedEmployeeLists")
        End Set
    End Property

    Public Property SelectedEmployee As EmployeeModel
        Get
            Return _selectedEmployee
        End Get
        Set(value As EmployeeModel)
            _selectedEmployee = value
            NotifyPropertyChanged("SelectedEmployee")
        End Set
    End Property

    Public Property SelectedEmployees As EmployeeListModel
        Get
            Return _selectedEmployees
        End Get
        Set(value As EmployeeListModel)
            _selectedEmployees = value
            NotifyPropertyChanged("SelectedEmployees")
        End Set
    End Property

    Public Property SelectedAssignedEmployee As EmployeeModel
        Get
            Return _selectedAssignedEmployee
        End Get
        Set(value As EmployeeModel)
            _selectedAssignedEmployee = value
            NotifyPropertyChanged("SelectedAssignedEmployee")
        End Set
    End Property

    Public Property SelectedAssignedEmployees As EmployeeListModel
        Get
            Return _selectedAssignedEmployees
        End Get
        Set(value As EmployeeListModel)
            _selectedAssignedEmployees = value
            NotifyPropertyChanged("SelectedAssignedEmployees")
        End Set
    End Property
#End Region

#Region "Field Properties"

    Public Property ProjectName() As String
        Get
            Return _projectName
        End Get
        Set(ByVal value As String)
            _projectName = value
        End Set
    End Property

    Public Property ProjectCategory() As Integer
        Get
            Return _projectCategory
        End Get
        Set(ByVal value As Integer)
            _projectCategory = value
        End Set
    End Property

    Public Property ProjectBillability() As Integer
        Get
            Return _projectBillability
        End Get
        Set(ByVal value As Integer)
            _projectBillability = value
        End Set
    End Property


    Public Property ProjectStart() As String
        Get
            Return _projectStart
        End Get
        Set(ByVal value As String)
            _projectStart = value
        End Set
    End Property

    Public Property ProjectEnd() As String
        Get
            Return _projectEnd
        End Get
        Set(ByVal value As String)
            _projectEnd = value
        End Set
    End Property

#End Region

#Region "Button Properties"
    Private _addMode As Boolean
    Public Property AddMode As Boolean
        Get
            Return _addMode
        End Get
        Set(value As Boolean)
            _addMode = value
            NotifyPropertyChanged("AddMode")
        End Set
    End Property

    Private _removeMode As Boolean
    Public Property RemoveMode As Boolean
        Get
            Return _removeMode
        End Get
        Set(value As Boolean)
            _removeMode = value
            NotifyPropertyChanged("RemoveMode")
        End Set
    End Property

    Private _clearMode As Boolean
    Public Property ClearMode As Boolean
        Get
            Return _clearMode
        End Get
        Set(value As Boolean)
            _clearMode = value
            NotifyPropertyChanged("ClearMode")
        End Set
    End Property

    Private _createMode As Boolean
    Public Property CreateMode As Boolean
        Get
            Return _createMode
        End Get
        Set(value As Boolean)
            _createMode = value
            NotifyPropertyChanged("CreateMode")
        End Set
    End Property

#End Region

#Region "Command Properties"
    Public Property AddCommand As ICommand
        Get
            If (_addCommand Is Nothing) Then
                _addCommand = New RelayCommand(Sub(param) Me.Add(), Nothing)
            End If

            Return _addCommand
        End Get
        Set(value As ICommand)
            _addCommand = value
        End Set
    End Property

    Public Property RemoveCommand As ICommand
        Get
            If (_removeCommand Is Nothing) Then
                _removeCommand = New RelayCommand(Sub(param) Me.Remove(), Nothing)
            End If

            Return _removeCommand
        End Get
        Set(value As ICommand)
            _removeCommand = value
        End Set
    End Property

    Public Property ClearCommand As ICommand
        Get
            If (_clearCommand Is Nothing) Then
                _clearCommand = New RelayCommand(Sub(param) Me.Clear(), Nothing)
            End If

            Return _clearCommand
        End Get
        Set(value As ICommand)
            _clearCommand = value
        End Set
    End Property

    Public Property CreateCommand() As ICommand
        Get
            If (_createCommand Is Nothing) Then
                _createCommand = New RelayCommand(Sub(param) Me.Create(), Nothing)
            End If

            Return _createCommand
        End Get
        Set(ByVal value As ICommand)
            _createCommand = value
        End Set
    End Property



#End Region

#Region "Procedures"
    Public Sub Add()
        Try
            'Move the name of the employee to the assignedemployeelist
            For Each rawEmployee As EmployeeListModel In EmployeeLists
                If SelectedEmployees.Name.Equals(rawEmployee.Name) Then
                    AssignedEmployeeLists.Add(rawEmployee)
                End If
            Next
            AssignedEmployeeLists = New ObservableCollection(Of EmployeeListModel)(AssignedEmployeeLists.OrderBy(Function(f) f.Name).ToList())

            For Each rawEmployee As EmployeeListModel In EmployeeLists
                If SelectedEmployees.Name.Equals(rawEmployee.Name) Then
                    EmployeeLists.Remove(rawEmployee)
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        If AssignedEmployeeLists.Count > 0 Then
            AddMode = True
            RemoveMode = True
            ClearMode = True
        End If
    End Sub

    Private Sub Clear()
        Try
            For Each rawEmployee As EmployeeListModel In AssignedEmployeeLists
                EmployeeLists.Add(rawEmployee)
            Next
            EmployeeLists = New ObservableCollection(Of EmployeeListModel)(EmployeeLists.OrderBy(Function(f) f.Name).ToList())

            AssignedEmployeeLists.Clear()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        AddMode = True
        RemoveMode = False
        ClearMode = False
    End Sub

    Private Sub Remove()
        Try
            For Each rawEmployee As EmployeeListModel In AssignedEmployeeLists
                If SelectedAssignedEmployees.Name.Equals(rawEmployee.Name) Then
                    EmployeeLists.Add(rawEmployee)
                End If
            Next
            EmployeeLists = New ObservableCollection(Of EmployeeListModel)(EmployeeLists.OrderBy(Function(f) f.Name).ToList())
            For Each rawEmployee As EmployeeListModel In AssignedEmployeeLists
                If SelectedAssignedEmployees.Name.Equals(rawEmployee.Name) Then
                    AssignedEmployeeLists.Remove(rawEmployee)
                End If
            Next
            AssignedEmployeeLists = New ObservableCollection(Of EmployeeListModel)(AssignedEmployeeLists.OrderBy(Function(f) f.Name).ToList())

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        If AssignedEmployeeLists.Count > 0 Then
            AddMode = True
            RemoveMode = True
            ClearMode = True
        End If
    End Sub
    Private Sub Create()


        Dim _assign As New List(Of AssignedProject)

        Try
            'set the parameters for project (Create project)

            'End of Create project

            'save each assigned employee
            For Each x In _assignedEmployeeLists
                Dim objassign As New AssignedProject
                objassign.EmployeeID = x.EmpID
                objassign.ProjectID = SelectedProject.ProjectID
                objassign.DateCreated = Date.Now
                objassign.StartPeriod = x.DateStarted
                objassign.EndPeriod = x.DateFinished
                _assign.Add(objassign)
            Next

            If _assign.Count = 0 Then
                If SelectedProject.ProjectID > 0 Then
                    _AideServiceClient.DeleteAllAssignedProject(SelectedProject.ProjectID)
                    MsgBox("Assigned employee(s) to selected project deleted successfully!", MsgBoxStyle.Information)
                Else
                    MsgBox("No Assigned Project yet, Please Select Employee to Assign Project!", MsgBoxStyle.Information)
                End If
            Else
                If _assign(0).ProjectID = 0 Then
                    MsgBox("Please fill up all required fields", MsgBoxStyle.Exclamation, "AIDE")
                    Exit Sub

                End If
                _AideServiceClient.DeleteAllAssignedProject(SelectedProject.ProjectID)
                _AideServiceClient.CreateAssignedProject(_assign.ToArray())
                MsgBox("Assigned Project Successfully saved!", MsgBoxStyle.Information)
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        'Clear()
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

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

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
End Class

''' <summary>
'''BY GIANN CARLO CAMILO 
''' </summary>
''' <remarks></remarks>
Public Class CategoryModel

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Integer
    Public Property Value As String
End Class


