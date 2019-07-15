Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Data
Imports System.ServiceModel

'CREATE PROJECT
'MARIVIC ESPINO / HYACINTH AMARLES

Class CreateProjectPage
    Implements IAideServiceCallback

#Region "Paging Declarations"
    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 8
    Dim currentPage As Integer
    Dim lastPage As Integer
#End Region

#Region "Fields"
    Private _pFrame As New Frame
    Public _empID As Integer
    Private _ProjectDBProvider As New ProjectDBProvider
    Private _ProjectViewModel As New ProjectViewModel
    Private client As AideServiceClient

    Dim billabiltiy As Short
    Dim category As Short
    Dim lstProj As Project()
    Dim totalRecords As Integer
    Dim paginatedCollection As PaginatedObservableCollection(Of ProjectModel) = New PaginatedObservableCollection(Of ProjectModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(pFrame As Frame, empID As Integer)
        _pFrame = pFrame
        _empID = empID
        InitializeComponent()
        SetData()
    End Sub
#End Region

#Region "Common Procedure"
    Public Sub ClearSelection()
        txtProjID.IsEnabled = True
        cbBillability.Text = String.Empty
        cbCategory.Text = String.Empty
        txtProjID.Text = String.Empty
        txtProjName.Text = String.Empty
    End Sub

    Public Sub DisableControl()
        txtProjID.IsEnabled = False
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
        End Try
        Return bInitialize
    End Function
    ''' <summary>
    ''' Load project into textbox to be edit or updated
    ''' Hyacinth Amarles
    ''' </summary>
    ''' 
    Public Sub SetData()
        Try
            If InitializeService() Then
                lstProj = client.GetProjectList(_empID)
                LoadProjectList()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadProject(projid As Integer)
        Try
            Dim lstProject As Project = client.GetProjectByID(projid)
            If Not IsNothing(lstProject) Then
                Dim projects As New ObservableCollection(Of ProjectModel)
                _ProjectDBProvider.setProject(lstProject)
                Dim iProjects As myProjectList = _ProjectDBProvider.getProject()
                projects.Add(New ProjectModel(iProjects))
                _ProjectViewModel.ProjectList = projects
                dgProjectList.DataContext = _ProjectViewModel
            Else
                dgProjectList.DataContext = Nothing
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    ''' <summary>
    ''' Load list of project in the datagrid
    ''' Hyacinth Amarles
    ''' </summary>
    Private Sub LoadProjectList()
        Try
            paginatedCollection.Clear()
            Dim lstProjectObs As New ObservableCollection(Of ProjectModel)
            Dim projectDBProvider As New ProjectDBProvider

            For Each objProject As Project In lstProj
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
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstProj.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
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
            Dim Projects As New Project
            Projects.ProjectID = Convert.ToInt32(txtProjID.Text)
            Projects.EmpID = _empID
            Projects.ProjectName = txtProjName.Text
            Projects.Category = category
            Projects.Billability = billabiltiy
            client.CreateProject(Projects)
            _ProjectDBProvider._myprojectlist.Clear()
            LoadProjectList()
            ClearSelection()
            txtSearch.Text = String.Empty
            MsgBox("Project Successfully Added")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Project")
        End Try
    End Sub

    ''' <summary>
    ''' Sub procedure that update existing project
    ''' Marivic Espino
    ''' </summary>
    Public Sub UpdateProjectDetails()
        Try
            GetBillability()
            GetCategory()
            Dim Projects As New Project
            Projects.ProjectID = Convert.ToInt32(txtProjID.Text)
            Projects.ProjectName = txtProjName.Text
            Projects.Category = category
            Projects.Billability = billabiltiy
            client.UpdateProject(Projects)
            _ProjectDBProvider._myprojectlist.Clear()

            ClearSelection()
            ' txtSearch.Text = String.Empty
            MsgBox("Project Successfully Updated")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Project")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String)
        Try
            Dim projectDBProvider As New ProjectDBProvider

            paginatedCollection = New PaginatedObservableCollection(Of ProjectModel)(pagingRecordPerPage)

            Dim items = From i In lstProj Where i.ProjectName.ToLower.Contains(input.ToLower) Or i.ProjectID.ToString.ToLower.Contains(input.ToLower)
            Dim searchProjects = New ObservableCollection(Of Project)(items)

            For Each objProject As Project In searchProjects
                projectDBProvider.setProjectList(objProject)
            Next

            For Each objProject As myProjectList In projectDBProvider.getProjectList()
                If objProject.billability = 0 Then
                    objProject.billability = "Internal"
                Else
                    objProject.billability = "External"
                End If
                If objProject.category = 0 Then
                    objProject.category = "Project"
                Else
                    objProject.category = "Task"
                End If

                paginatedCollection.Add(New ProjectModel(objProject))
            Next

            totalRecords = searchProjects.Count
            dgProjectList.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

#End Region

#Region "Events"
    Private Sub txtProjID_KeyDown(sender As Object, e As KeyEventArgs) Handles txtProjID.KeyDown
        Dim key As Integer = CInt(e.Key)
        e.Handled = Not (key >= 34 AndAlso key <= 43 OrElse key >= 74 AndAlso key <= 83 OrElse key = 2 OrElse key = 18)

        If Not e.Handled Then
        Else
            MsgBox("Number/s Only")
        End If

    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        If txtProjName.Text = String.Empty Or cbBillability.Text = String.Empty Or cbCategory.Text = String.Empty Then
            MsgBox("Please fill up all the Fields", MsgBoxStyle.Exclamation, "AIDE")
        ElseIf (String.IsNullOrEmpty((lblProjIdValidation.Content))) = False Then
            MsgBox("Project ID isn't available", MsgBoxStyle.Critical, "AIDE")
        Else
            InsertNewProject()
        End If
        SetPaging(PagingMode._First)
    End Sub

    Private Sub btnNewProject_Click(sender As Object, e As RoutedEventArgs) Handles btnNewProject.Click
        txtSearch.Text = String.Empty
        ClearSelection()
        btnUpdate.Visibility = Windows.Visibility.Hidden
        btnCreate.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        If txtSearch.Text = String.Empty Or txtSearch.Text = " " Then
            txtSearch.Text = String.Empty
            SetData()
            ClearSelection()
        Else
            ClearSelection()
            SetDataForSearch(txtSearch.Text)
        End If
    End Sub

    Private Sub txtProjID_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtProjID.TextChanged
        If txtProjID.Text = String.Empty Then
            btnUpdate.Visibility = Windows.Visibility.Hidden
            btnCreate.Visibility = Windows.Visibility.Visible
            ClearSelection()
            lblProjIdValidation.Content = String.Empty
        Else
            Try
                Dim lstProject As Project = client.GetProjectByID(txtProjID.Text)
                If Not IsNothing(lstProject) Then
                    lblProjIdValidation.Content = "Project ID isn't Available"
                Else
                    lblProjIdValidation.Content = String.Empty
                End If
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
            End Try

        End If
    End Sub

    Private Sub dgProjectList_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgProjectList.MouseDoubleClick
        Try
            If dgProjectList.SelectedIndex <> -1 Then

                btnUpdate.Visibility = Windows.Visibility.Visible
                btnCreate.Visibility = Windows.Visibility.Hidden

                txtProjID.IsEnabled = False
                txtProjID.Text = CType(dgProjectList.SelectedItem, ProjectModel).ProjectID
                txtProjName.Text = CType(dgProjectList.SelectedItem, ProjectModel).ProjectName


                If CType(dgProjectList.SelectedItem, ProjectModel).Category = " Task" Then
                    cbCategory.SelectedIndex = 0
                Else
                    cbCategory.SelectedIndex = 1
                End If
                If CType(dgProjectList.SelectedItem, ProjectModel).Billability = " Internal" Then
                    cbBillability.SelectedIndex = 0
                Else
                    cbBillability.SelectedIndex = 1
                End If

                lblProjIdValidation.Content = String.Empty
                txtProjID.IsEnabled = False
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        If txtProjName.Text = Nothing Or cbBillability.Text = Nothing Or cbCategory.Text = Nothing Then
            MsgBox("Please fill out completely")
        Else
            If (String.IsNullOrEmpty((txtSearch.Text))) = False Then
                UpdateProjectDetails()
                LoadProject(Convert.ToInt32(txtSearch.Text))
            ElseIf (String.IsNullOrEmpty((txtSearch.Text))) = True Then
                UpdateProjectDetails()
                LoadProjectList()
            End If
            btnCreate.Visibility = Windows.Visibility.Visible
            btnUpdate.Visibility = Windows.Visibility.Hidden
            txtProjID.IsEnabled = True
        End If
    End Sub
#End Region

#Region "Paging Method"
    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstProj.Length

            Select Case mode
                Case CInt(PagingMode._Next)
                    ' Set the rows to be displayed if the total records is more than the (Record per Page * Page Index)
                    If totalRecords > (pagingPageIndex * pagingRecordPerPage) Then

                        ' Set the last row to be displayed if the total records is more than the (Record per Page * Page Index) + Record per Page
                        If totalRecords >= ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) Then
                            lastRowIndex = ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) - 1
                        Else
                            lastRowIndex = totalRecords - 1
                        End If

                        startRowIndex = pagingPageIndex * pagingRecordPerPage
                        pagingPageIndex += 1
                    Else
                        startRowIndex = (pagingPageIndex - 1) * pagingRecordPerPage
                        lastRowIndex = totalRecords - 1
                    End If
                    ' Bind data to the Data Grid
                    LoadProjectList()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadProjectList()
                    End If
                    Exit Select
                Case CInt(PagingMode._First)
                    If totalRecords > pagingRecordPerPage Then
                        pagingPageIndex = 2
                        SetPaging(CInt(PagingMode._Previous))
                    Else
                        pagingPageIndex = 1
                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)

                        If Not totalRecords = 0 Then
                            lastRowIndex = totalRecords - 1
                            LoadProjectList()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstProj.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

            'DisplayPagingInfo()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstProj.Length = 0 Then
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

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        Dim totalRecords As Integer = lstProj.Length

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

    Private Sub btnFirst_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._First))
    End Sub

    Private Sub btnLast_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Last))
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
