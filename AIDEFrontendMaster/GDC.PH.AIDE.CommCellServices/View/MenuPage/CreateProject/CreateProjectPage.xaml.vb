Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Data
Imports System.ServiceModel

'CREATE PROJECT
'MARIVIC ESPINO / HYACINTH AMARLES / JHUNELL BARCENAS

Class CreateProjectPage
    Implements IAideServiceCallback

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 8
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private frame As New Frame
    Private profile As Profile
    Private empID As Integer
    Private projectDBProvider As New ProjectDBProvider
    Private client As AideServiceClient
    Private _OptionsViewModel As OptionViewModel

    Dim billabiltiy As Short
    Dim category As Short
    Dim lstProjects As Project()
    Dim paginatedCollection As PaginatedObservableCollection(Of ProjectModel) = New PaginatedObservableCollection(Of ProjectModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _profile As Profile)
        frame = _frame
        profile = _profile
        empID = _profile.Emp_ID

        InitializeComponent()
        
        
        pagingRecordPerPage = GetOptionData(31, 15, 12)

        LoadProject()
        PermissionSettings()
    End Sub
#End Region

#Region "Sub Procedure"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    ''' <summary>
    ''' Load project into textbox to be edit or updated
    ''' Hyacinth Amarles
    ''' </summary>
    ''' 
    Public Sub LoadProject()
        Try
            If InitializeService() Then
                Dim displayStatus As Integer = 1

                lstProjects = client.GetProjectList(empID, displayStatus)
                SetLists(lstProjects)
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    ''' <summary>
    ''' Load list of project in the datagrid
    ''' Hyacinth Amarles
    ''' </summary>
    Private Sub SetLists(listProjects As Project())
        Try
            projectDBProvider = New ProjectDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of ProjectModel)(pagingRecordPerPage)

            For Each objProject As Project In listProjects
                projectDBProvider.setProjectList(objProject)
            Next

            For Each iProject As myProjectList In projectDBProvider.getProjectList()
                If iProject.billability = 0 Then
                    iProject.billability = "Internal"
                Else
                    iProject.billability = "External"
                End If

                If iProject.category = 0 Then
                    iProject.category = "Project"
                Else
                    iProject.category = "Task"
                End If

                paginatedCollection.Add(New ProjectModel(iProject))
            Next

            dgProjectList.ItemsSource = paginatedCollection

            totalRecords = listProjects.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadProject(projid As Integer)
        Try
            Dim lstProject As Project = client.GetProjectByID(projid)
            If Not IsNothing(lstProject) Then
                Dim projects As New ObservableCollection(Of ProjectModel)
                projectDBProvider.setProject(lstProject)
                Dim iProjects As myProjectList = projectDBProvider.getProject()
                projects.Add(New ProjectModel(iProjects))
                '_ProjectViewModel.ProjectList = projects
                'dgProjectList.DataContext = _ProjectViewModel
            Else
                dgProjectList.DataContext = Nothing
            End If
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    ''' <summary>
    ''' Sub procedure that insert new project
    ''' Marivic Espino
    ''' </summary>
    Public Sub InsertNewProject()
        Try
            GetBillability()
            GetCategory()

            Dim newProject As New Project
            newProject.ProjectCode = txtProjCD.Text
            newProject.EmpID = empID
            newProject.ProjectName = txtProjName.Text
            newProject.Category = category
            newProject.Billability = billabiltiy

            client.CreateProject(newProject)

            LoadProject()
            ClearSelection()
            txtSearch.Text = String.Empty

            MsgBox("Project has been added.", MsgBoxStyle.Information, "AIDE")
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    ''' <summary>
    ''' Sub procedure that update existing project
    ''' Marivic Espino
    ''' </summary>
    Public Sub UpdateProject()
        Try
            GetBillability()
            GetCategory()

            Dim selectedProject As New Project
            selectedProject.ProjectID = CType(dgProjectList.SelectedItem, ProjectModel).ProjectID
            selectedProject.ProjectCode = txtProjCD.Text
            selectedProject.ProjectName = txtProjName.Text
            selectedProject.Category = category
            selectedProject.Billability = billabiltiy

            client.UpdateProject(selectedProject)

            ClearSelection()
            LoadProject()

            If txtSearch.Text.Trim IsNot String.Empty Then
                SearchProject(txtSearch.Text.Trim)
            End If

            MsgBox("Project has been updated.", MsgBoxStyle.Information, "AIDE")
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SearchProject(search As String)
        Try
            Dim items = From i In lstProjects Where i.ProjectName.ToLower.Contains(search.ToLower)

            Dim searchProject = New ObservableCollection(Of Project)(items)

            SetLists(searchProject.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If totalRecords = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        dgProjectList.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        dgProjectList.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub PermissionSettings()
        If profile.Permission_ID = 1 Then
            grdCreate.Visibility = Visibility.Visible
        End If
    End Sub
    
    Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
        Dim strData As String = String.Empty
        Try
            _OptionsViewModel = New OptionViewModel
            If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        strData = opt.VALUE
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return strData
    End Function
#End Region

#Region "Common Procedure"
    Public Sub ClearSelection()
        txtProjCD.IsEnabled = True
        cbBillability.Text = String.Empty
        cbCategory.Text = String.Empty
        txtProjCD.Text = String.Empty
        txtProjName.Text = String.Empty
    End Sub

    Public Sub DisableControl()
        txtProjCD.IsEnabled = False
        txtProjName.IsEnabled = False
        cbBillability.IsEnabled = False
        cbCategory.IsEnabled = False
    End Sub

    Public Sub GetBillability()
        If cbBillability.Text = "Internal" Then
            billabiltiy = 0
        ElseIf cbBillability.Text = "External" Then
            billabiltiy = 1
        End If
    End Sub

    Public Sub GetCategory()
        If cbCategory.Text = "Project" Then
            category = 0
        ElseIf cbCategory.Text = "Task" Then
            category = 1
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub txtProjCD_KeyDown(sender As Object, e As KeyEventArgs) Handles txtProjCD.KeyDown
        Dim key As Integer = CInt(e.Key)
        e.Handled = Not (key >= 34 AndAlso key <= 43 OrElse key >= 74 AndAlso key <= 83 OrElse key = 2 OrElse key = 18)

        If Not e.Handled Then
        Else
            MsgBox("Please enter a numeric Project ID.", MsgBoxStyle.Information, "AIDE")
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As RoutedEventArgs) Handles btnClear.Click
        txtSearch.Text = String.Empty
        ClearSelection()
        btnUpdate.Visibility = Windows.Visibility.Hidden
        btnCreate.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        ClearSelection()
        SearchProject(txtSearch.Text.Trim)
    End Sub

    Private Sub txtProjCD_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtProjCD.TextChanged
        If txtProjCD.Text = String.Empty Then
            btnUpdate.Visibility = Windows.Visibility.Hidden
            btnCreate.Visibility = Windows.Visibility.Visible
            ClearSelection()
            lblProjIdValidation.Content = String.Empty
        Else
            Try
                Dim lstProject As Project = client.GetProjectByID(txtProjCD.Text)
                If Not IsNothing(lstProject) Then
                    lblProjIdValidation.Content = "Project ID already exists."
                Else
                    lblProjIdValidation.Content = String.Empty
                End If
            Catch ex As Exception
                MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try
        End If
    End Sub

    Private Sub dgProjectList_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgProjectList.MouseDoubleClick
        Try
            If profile.Permission_ID = 1 Then
                If dgProjectList.SelectedIndex <> -1 Then

                    btnUpdate.Visibility = Windows.Visibility.Visible
                    btnCreate.Visibility = Windows.Visibility.Hidden

                    txtProjCD.IsEnabled = False
                    txtProjCD.Text = CType(dgProjectList.SelectedItem, ProjectModel).ProjectCode
                    txtProjName.Text = CType(dgProjectList.SelectedItem, ProjectModel).ProjectName

                    If CType(dgProjectList.SelectedItem, ProjectModel).Category = "Project" Then
                        cbCategory.SelectedIndex = 0
                    Else
                        cbCategory.SelectedIndex = 1
                    End If

                    If CType(dgProjectList.SelectedItem, ProjectModel).Billability = "Internal" Then
                        cbBillability.SelectedIndex = 0
                    Else
                        cbBillability.SelectedIndex = 1
                    End If

                    lblProjIdValidation.Content = String.Empty
                    txtProjCD.IsEnabled = False
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        If txtProjName.Text = String.Empty Or cbBillability.Text = String.Empty Or cbCategory.Text = String.Empty Then
            MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
        ElseIf (String.IsNullOrEmpty((lblProjIdValidation.Content))) = False Then
            MsgBox("Project ID already exists.", MsgBoxStyle.Critical, "AIDE")
        Else
            InsertNewProject()
        End If
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        If txtProjName.Text = Nothing Or cbBillability.Text = Nothing Or cbCategory.Text = Nothing Then
            MsgBox("Please enter all required fields. Ensure all required fields have * indicated.")
        Else
            UpdateProject()

            btnCreate.Visibility = Windows.Visibility.Visible
            btnUpdate.Visibility = Windows.Visibility.Hidden
            txtProjCD.IsEnabled = True
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If

        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If

        DisplayPagingInfo()
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
