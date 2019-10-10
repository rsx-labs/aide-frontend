Imports System.IO
Imports System.Data
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Forms
Imports System.Text.RegularExpressions
Imports System.ServiceModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
'Imports System.Drawing.Printing

''' Skills Matrix Manager Page
''' By Jhunell G. Barcenas / Hyacinth Amarles / Richard Espida


<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class SkillsMatrixManagerPage
    Implements IAideServiceCallback

#Region "Fields"
    Private _SkillDBProvider As New SkillsDBProvider
    Private _SkillsViewModel As New SkillsViewModel
    Private _ProfileDBProvider As New ProfileDBProvider
    Private _ProfileViewModel As New ProfileViewModel
    Private _EmployeeListViewModel As New EmployeeListViewModel
    Private _EmployeeListDBProvider As New EmployeeListProvider
    Private client As AideServiceClient

    Dim lstSkillEmployee As Skills()
    Dim lstSkills As Skills()
    Dim isManager As Boolean
    Dim bClick As Boolean
    Dim email As String
    Dim empID As Integer
    Dim skillid As Integer
    Dim proflevel As Integer = 0
    Dim proficiency As Integer
    Dim columnReviewed As String = "Reviewed"
    Dim columnDateReviewed As String = "Date Reviewed"

#End Region

#Region "Constructor"

    Public Sub New(_empID As Integer, _email As String, _isManager As Boolean)
        ' This call is required by the designer.
        Me.empID = _empID
        Me.email = _email
        Me.isManager = _isManager
        Me.InitializeComponent()

        LoadSkillsList()
        LoadEmployeeList()
        LoadProfile()
        grdUpdate.Visibility = Visibility.Collapsed
    End Sub

#End Region

#Region "Function Procedures"

    ''' <summary>
    ''' Loads dictionary data into datatable that will display to datagrid with dynamic column
    ''' </summary>
    '''By Hyacinth Amarles
    Private Function ToDataTable(list As List(Of Dictionary(Of String, String))) As DataTable
        Dim result As New DataTable()

        If list.Count = 0 Then
            Return result
        End If

        Dim columnNames = list.SelectMany(Function(dict) dict.Keys).Distinct()
        'result.Columns.AddRange(columnNames.[Select](Function(c) New DataColumn(c)).ToArray())

        For Each columns In columnNames
            If columns = columnReviewed Then
                result.Columns.Add(New DataColumn(columns, GetType(Boolean)))
            Else
                result.Columns.Add(New DataColumn(columns, GetType(String)))
            End If
        Next

        For Each item As Dictionary(Of String, String) In list
            Dim row = result.NewRow()
            For Each key In item.Keys
                row(key) = item(key)
            Next
            result.Rows.Add(row)
        Next
        Return result

    End Function

    Public Function checkRB() As Boolean
        If rbProfLvl1.IsChecked = False And rbProfLvl2.IsChecked = False And rbProfLvl3.IsChecked = False And rbProfLvl4.IsChecked = False Then
            Return True
        End If
        Return False
    End Function

    Private Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function
#End Region

#Region "Sub Procedures"

    ''' <summary>
    ''' Loads employee skills in combo box
    ''' </summary>
    '''By Hyacinth Amarles 
    Public Sub LoadSkillsList()
        Try
            InitializeService()
            _SkillDBProvider.GetSkillList.Clear()

            lstSkills = client.GetSkillsList(empID)
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            For Each objSkill As Skills In lstSkills
                _SkillDBProvider.SetSkillList(objSkill)
            Next

            For Each iSkills As mySkillList In _SkillDBProvider.GetSkillList()
                skillslist.Add(New SkillsModel(iSkills))
            Next

            _SkillsViewModel.SkillList = skillslist

            cbProjectList.DataContext = _SkillsViewModel
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    ''' <summary>
    ''' Loads employee skills proficiency in datagrid
    ''' </summary>
    '''By Hyacinth Amarles 
    Private Sub LoadEmployeeList()
        Try
            InitializeService()

            Dim lstEmployee As Employee() = client.GetNicknameByDeptID(email)
            Dim employeeList As New ObservableCollection(Of EmployeeListModel)
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            For Each objEmployee As Employee In lstEmployee
                _EmployeeListDBProvider.SetEmployeeList(objEmployee)
            Next

            For Each iEmployee As MyEmployeeList In _EmployeeListDBProvider.GetEmployeeList()
                employeeList.Add(New EmployeeListModel(iEmployee))
            Next

            _EmployeeListViewModel.EmployeeList = employeeList

        Catch ex As Exception
            client.Abort()
        End Try
    End Sub

    Public Sub LoadSkillsProf()
        Try
            InitializeService()
            Dim level As String
            Dim isDateReviewed As Boolean
            Dim lstskill As Skills() = client.GetSkillsProfByEmpID(Convert.ToInt32(lblEmpID.Text))

            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            For Each objSkill As Skills In lstskill
                _SkillDBProvider.SetEmpSkillsProf(objSkill)
            Next

            ' Get Skills List
            For Each iSkill As mySkillList In _SkillDBProvider.GetSkillList
                level = ""
                isDateReviewed = False

                ' Get Skill Proficiency
                For i As Integer = 0 To _SkillDBProvider._splist.Count - 1
                    If iSkill.Skill_Descr.Equals(_SkillDBProvider._splist(i).Skill_Descr) Then
                        level = _SkillDBProvider._splist(i).Prof_level
                        If _SkillDBProvider._splist(i).Last_Reviewed.Month = Date.Today.Month Then
                            isDateReviewed = True
                        Else
                            isDateReviewed = False
                        End If

                        _SkillDBProvider._splist.RemoveAt(i)
                    Else
                        level = ""
                    End If

                    Exit For
                Next
                dict.Add(iSkill.Skill_Descr, level)
            Next

            'For Each iSkills As mySkillList In _SkillDBProvider.GetSkillProf()
            '    dict.Add(iSkills.Skill_Descr, iSkills.Prof_level)  ' Add list of dictionary
            'Next


            it.Add(dict)
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)

            dgSkillList.ItemsSource = table.AsDataView



        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

    ''' <summary>
    ''' Loads all employee skills proficiency level 
    ''' </summary>
    '''By Hyacinth Amarles / John Harvey Sanchez
    Public Sub ViewAllEmployee()
        Try
            InitializeService()
            _SkillDBProvider.GetSkillProf.Clear()

            Dim emp_id As String = ""
            Dim level As String
            Dim dateReviewed As String
            Dim isDateReviewed As Boolean

            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            lstSkillEmployee = client.ViewEmpSKills(empID)

            For Each objSkill As Skills In lstSkillEmployee
                _SkillDBProvider.SetEmpSkillsProf(objSkill)
            Next

            ' Get Employee list
            For Each iEmployee As MyEmployeeList In _EmployeeListDBProvider.GetEmployeeList
                dateReviewed = ""
                isDateReviewed = False

                ' Get Skills List
                For Each iSkill As mySkillList In _SkillDBProvider.GetSkillList
                    level = ""
                    ' Get Skill Proficiency
                    For i As Integer = 0 To _SkillDBProvider._splist.Count - 1
                        If iEmployee.EmpID.ToString.Equals(_SkillDBProvider._splist(i).Emp_ID) Then
                            If iSkill.Skill_Descr.Equals(_SkillDBProvider._splist(i).Skill_Descr) Then
                                level = _SkillDBProvider._splist(i).Prof_level
                                dateReviewed = _SkillDBProvider._splist(i).Last_Reviewed

                                If _SkillDBProvider._splist(i).Last_Reviewed.Month = Date.Today.Month Then
                                    isDateReviewed = True
                                Else
                                    isDateReviewed = False
                                End If

                                _SkillDBProvider._splist.RemoveAt(i)
                            Else
                                level = ""
                            End If

                            Exit For
                        Else
                            level = ""
                            Exit For
                        End If
                    Next

                    '' Adding to dictionary every employees skills description and  skills proficiency
                    If emp_id.Equals(iEmployee.EmpID.ToString()) Then
                        dict.Add(iSkill.Skill_Descr, level)
                    End If

                    If Not emp_id.Equals(iEmployee.EmpID.ToString()) Then
                        If Not emp_id.Equals("") Then
                            it.Add(dict)
                        End If
                        dict = New Dictionary(Of String, String)()
                        dict.Add("Employee ID", iEmployee.EmpID)
                        dict.Add("Employee Name", iEmployee.Name)
                        dict.Add(iSkill.Skill_Descr, level)
                    End If

                    emp_id = iEmployee.EmpID
                Next

                ' Add Date Reviewed Column
                dict.Add(columnDateReviewed, dateReviewed)
                dict.Add(columnReviewed, isDateReviewed)
            Next

            it.Add(dict)

            ' Convert stored data in dictionary to datable
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)

            ' Skills_descr as Column Header that is dynamic
            dgSkillList.ItemsSource = table.AsDataView
            dgSkillList.SelectionMode = DataGridSelectionMode.Extended
            dgSkillList.SelectionUnit = DataGridSelectionUnit.FullRow

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    ''' <summary>
    ''' Insert proficiency level to a skill of an employee
    ''' </summary>
    '''By Hyacinth Amarles 
    Public Sub InsertSkillsProficiency()
        Try
            InitializeService()
            Dim Skills As New Skills
            GetProfLevel()
            Skills.DESCR = String.Empty
            Skills.NAME = String.Empty
            Skills.Image_Path = String.Empty
            Skills.EmpID = Convert.ToInt32(lblEmpID.Text)
            Skills.Prof_LVL = proflevel
            Skills.SkillID = cbProjectList.SelectedValue
            Skills.Last_Reviewed = Date.Now
            client.InsertNewSkills(Skills)
            MsgBox("Successfully Added " & cbProjectList.Text.ToUpper & " Skill!", MsgBoxStyle.Information, "Employee Assist Tools")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Employee Assist Tools")
        End Try
    End Sub

    ''' <summary>
    ''' Changing the proficiency level value of a skill into a new proficiency level
    ''' </summary>
    '''By Hyacinth Amarles 
    Public Sub UpdateSkillsProficiency()
        Try
            InitializeService()
            Dim Skills As New Skills
            GetProfLevel()
            Skills.DESCR = String.Empty
            Skills.NAME = String.Empty
            Skills.Image_Path = String.Empty
            Skills.EmpID = Convert.ToInt32(lblEmpID.Text)
            Skills.Prof_LVL = proflevel
            Skills.SkillID = cbProjectList.SelectedValue
            Skills.Last_Reviewed = Date.Now
            client.UpdateSkills(Skills)
            MsgBox("Successfully Updated " & cbProjectList.Text.ToUpper & " Skill!", MsgBoxStyle.Information, "Employee Assist Tools")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Employee Assist Tools")
        End Try
    End Sub

    ''' <summary>
    ''' Changing the proficiency level value of a skill into a new proficiency level
    ''' </summary>
    '''By Hyacinth Amarles 
    Public Sub UpdateAllSkillsProficiency()
        Try
            InitializeService()
            Dim Skills As New Skills
            Skills.DESCR = String.Empty
            Skills.NAME = String.Empty
            Skills.Image_Path = String.Empty
            Skills.EmpID = Convert.ToInt32(lblEmpID.Text)
            Skills.Last_Reviewed = Date.Now
            client.UpdateAllSkills(Skills)
            MsgBox("Successfully Updated " & cbProjectList.Text.ToUpper & " Skill!", MsgBoxStyle.Information, "Employee Assist Tools")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Employee Assist Tools")
        End Try
    End Sub

    ''' <summary>
    ''' Loads essential information of an employee
    ''' </summary>
    ''' By Jhunell G. Barcenas
    Public Sub LoadProfile()
        Try
            InitializeService()
            Dim lstProfile As Profile = client.GetProfileInformation(empID)
            If Not IsNothing(lstProfile) Then
                Dim profileList As New ObservableCollection(Of ProfileModel)
                _ProfileDBProvider = New ProfileDBProvider
                _ProfileViewModel = New ProfileViewModel
                _ProfileDBProvider.SetMyProfile(lstProfile)

                Dim iProfile As MyProfile = _ProfileDBProvider.GetMyProfile()
                profileList.Add(New ProfileModel(iProfile))

                _ProfileViewModel.UsersList = profileList

                Me.DataContext = _ProfileViewModel.UsersList(0)

                ViewAllEmployee()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

#End Region

#Region "Events"

    'Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
    '    DisableControls()
    '    Try
    '        InitializeService()
    '        If txtSearch.Text = String.Empty Then
    '            LoadProfile()
    '            ClearSelection()
    '        ElseIf txtSearch.Text = " " Then
    '            MsgBox("Please Input Numbers Only!", MsgBoxStyle.Critical, "Employee Assist Tools")
    '            txtSearch.Text = String.Empty
    '        Else
    '            Dim lstProfile As Profile = client.GetProfileInformation(txtSearch.Text)
    '            If Not IsNothing(lstProfile) Or txtSearch.Text = 0 Then
    '                    Dim profileList As New ObservableCollection(Of ProfileModel)
    '                    _ProfileDBProvider = New ProfileDBProvider
    '                    _ProfileViewModel = New ProfileViewModel
    '                    _ProfileDBProvider.SetMyProfile(lstProfile)

    '                    Dim iProfile As MyProfile = _ProfileDBProvider.GetMyProfile()
    '                    profileList.Add(New ProfileModel(iProfile))

    '                    _ProfileViewModel.UsersList = profileList

    '                    Me.DataContext = _ProfileViewModel.UsersList(0)
    '                    EnableControls()
    '                    _SkillDBProvider._splist.Clear()
    '                    LoadSkillsProf()
    '                    ClearSelection()
    '            Else
    '                ClearFields()
    '                dgSkillList.ItemsSource = Nothing
    '            End If
    '        End If
    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
    '    End Try
    'End Sub

    'Private Sub btnEditMySkill_Click(sender As Object, e As RoutedEventArgs) Handles btnEditMySkill.Click
    '    'txtSearch.Text = Nothing
    '    bClick = True
    '    _SkillDBProvider._splist.Clear()
    '    LoadSkillsProf()
    '    EnableControls()
    '    grdUpdate.Visibility = Visibility.Visible
    'End Sub

    Private Sub btnAddUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnAddUpdate.Click
        Try
            InitializeService()
            skillid = cbProjectList.SelectedValue
            GetProfLevel()
            If cbProjectList.SelectedValue = 0 Or checkRB() = True Then
                Dim result = MsgBox("You are going to update your Skills" & vbNewLine & "Do you wish to continue?", MessageBoxButtons.YesNo, "AIDE")

                If bClick = False Then
                    If result = vbYes Then
                        UpdateAllSkillsProficiency()
                        ViewAllEmployee()
                    End If
                Else
                    If result = vbYes Then
                        UpdateAllSkillsProficiency()
                    End If
                End If

            Else
                If proficiency = proflevel Then
                    MsgBox("You've selected your current proficiency!" & vbNewLine & "Please select another one to update " & cbProjectList.Text.ToUpper, MsgBoxStyle.Critical, "AIDE")
                Else
                    InitializeService()
                    If client.GetProfLvlByEmpIDSkillIDs(Convert.ToInt32(lblEmpID.Text), skillid).Prof_LVL = 1 Then
                        Dim result = MsgBox("You are going to update " & cbProjectList.Text.ToUpper & " Skill." & vbNewLine & "Do you wish to Continue?", MessageBoxButtons.YesNo, "AIDE")
                        If result = vbYes Then
                            UpdateSkillsProficiency()
                        Else
                            ClearSelection()
                        End If
                    Else
                        Dim result = MsgBox("You are going to add proficiency level to " & cbProjectList.Text.ToUpper & " skill." & vbNewLine & "Do you wish to Continue?", MessageBoxButtons.YesNo, "AIDE")
                        If result = vbYes Then
                            InsertSkillsProficiency()
                        Else
                            ClearSelection()
                        End If
                    End If
                    _SkillDBProvider._splist.Clear()
                    ClearSelection()
                    LoadSkillsProf()
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub


    Private Sub dgSkillList_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles dgSkillList.LoadingRow
        Dim nIndex As Integer = 0

        For Each isReviewed In DirectCast(e.Row.DataContext, System.Data.DataRowView).Row.ItemArray
            If TypeOf isReviewed Is Boolean Then
                If isReviewed = False Then
                    e.Row.Background = New BrushConverter().ConvertFrom("#CCFFD8D8")
                End If
            End If
        Next

    End Sub

    Private Sub dgSkillList_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgSkillList.MouseDoubleClick
        InitializeService()
        If dgSkillList.Items.Count > 1 Then
            If Not dgSkillList.SelectedItem.Equals(Nothing) Then
                Dim emp_ID As Integer = dgSkillList.SelectedItem(0)

                If isManager = True Or emp_ID = empID Then
                    Dim lstProfile As Profile = client.GetProfileInformation(emp_ID)
                    Dim profileList As New ObservableCollection(Of ProfileModel)

                    _ProfileDBProvider = New ProfileDBProvider
                    _ProfileViewModel = New ProfileViewModel

                    _ProfileDBProvider.SetMyProfile(lstProfile)

                    Dim iProfile As MyProfile = _ProfileDBProvider.GetMyProfile()
                    profileList.Add(New ProfileModel(iProfile))
                    _ProfileViewModel.UsersList = profileList

                    Me.DataContext = _ProfileViewModel.UsersList
                    _SkillDBProvider._splist.Clear()
                    EnableControls()
                    ClearSelection()
                    LoadSkillsProf()

                    lblLastReviewed.Text = dgSkillList.CurrentCell.Item(11)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                    grdUpdate.Visibility = Visibility.Visible
                End If

            End If
        End If
    End Sub


    Private Sub btnViewEmp_Click(sender As Object, e As RoutedEventArgs) Handles btnViewEmp.Click
        LoadSkillsList()
        LoadProfile()
        ClearControl()
        DisableControls()
        bClick = False
        'txtSearch.Text = String.Empty
        btnPrint.Visibility = Windows.Visibility.Visible
        grdUpdate.Visibility = Visibility.Collapsed
        dgSkillList.Columns.Item(0).Visibility = Visibility.Collapsed
    End Sub

    Private Sub cbProjectList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProjectList.SelectionChanged
        InitializeService()
        Dim lstSkills As Skills = client.GetSkillsLastReviewByEmpIDSkillID(Convert.ToInt32(lblEmpID.Text), cbProjectList.SelectedValue)
        If IsNothing(lstSkills) Then
            ClearControl()
            proficiency = 0
        Else
            ClearControl()
            Dim empSkillsList As New ObservableCollection(Of SkillsModel)

            '_SkillDBProvider = New SkillsDBProvider
            '_SkillsViewModel = New SkillsViewModel

            _SkillDBProvider.SetSkillsLastReviewedProfLvl(lstSkills)

            Dim iSkills As mySkillList = _SkillDBProvider.GetSkillsLastReviewedProfLvl()
            empSkillsList.Add(New SkillsModel(iSkills))
            _SkillsViewModel.EmpSkillList = empSkillsList

            lblLastReviewed.DataContext = _SkillsViewModel.EmpSkillList
            proficiency = _SkillsViewModel.EmpSkillList(0)._profLevel
            If proficiency = 1 Then
                rbProfLvl1.IsChecked = True
                rbProfLvl1.Foreground = Brushes.Red
            ElseIf proficiency = 2 Then
                rbProfLvl2.IsChecked = True
                rbProfLvl2.Foreground = Brushes.Red
            ElseIf proficiency = 3 Then
                rbProfLvl3.IsChecked = True
                rbProfLvl3.Foreground = Brushes.Red
            ElseIf proficiency = 4 Then
                rbProfLvl4.IsChecked = True
                rbProfLvl4.Foreground = Brushes.Red
            End If
        End If
    End Sub

    'Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
    '    Dim dialog As New System.Windows.Controls.PrintDialog
    '    If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
    '        dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
    '        Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)

    '        spGrid.Measure(pageSize)

    '        spGrid.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))

    '        dialog.PrintVisual(spGrid, "Print Skills Matrix")

    '    End If
    'End Sub

#End Region

#Region "Controls"
    ''' <summary>
    ''' Validates if the input is not an integer
    ''' </summary>
    ''' By Jhunell G. Barcenas
    Private Sub NumberValidationTextBox(sender As Object, e As TextCompositionEventArgs)
        Dim regex As New Regex("[^0-9]+")
        e.Handled = regex.IsMatch(e.Text)
        If e.Handled = True Then
            MsgBox("Please Input Numbers Only!", MsgBoxStyle.Critical, "Employee Assist Tools")
        End If
    End Sub

    Public Sub GetProfLevel()
        If rbProfLvl1.IsChecked Then
            proflevel = 1
        ElseIf rbProfLvl2.IsChecked Then
            proflevel = 2
        ElseIf rbProfLvl3.IsChecked Then
            proflevel = 3
        ElseIf rbProfLvl4.IsChecked Then
            proflevel = 4
        End If
    End Sub

    Public Sub EnableControls()
        cbProjectList.IsEnabled = True
        rbProfLvl1.IsEnabled = True
        rbProfLvl2.IsEnabled = True
        rbProfLvl3.IsEnabled = True
        rbProfLvl4.IsEnabled = True
        rbProfLvl1.Foreground = Brushes.Black
        rbProfLvl2.Foreground = Brushes.Black
        rbProfLvl3.Foreground = Brushes.Black
        rbProfLvl4.Foreground = Brushes.Black
    End Sub

    Public Sub DisableControls()
        cbProjectList.IsEnabled = False
        rbProfLvl1.IsEnabled = False
        rbProfLvl2.IsEnabled = False
        rbProfLvl3.IsEnabled = False
        rbProfLvl4.IsEnabled = False
        rbProfLvl1.Foreground = Brushes.Gray
        rbProfLvl2.Foreground = Brushes.Gray
        rbProfLvl3.Foreground = Brushes.Gray
        rbProfLvl4.Foreground = Brushes.Gray
    End Sub

    Public Sub ClearSelection()
        rbProfLvl1.IsChecked = False
        rbProfLvl2.IsChecked = False
        rbProfLvl3.IsChecked = False
        rbProfLvl4.IsChecked = False
        rbProfLvl1.Foreground = Brushes.Black
        rbProfLvl2.Foreground = Brushes.Black
        rbProfLvl3.Foreground = Brushes.Black
        rbProfLvl4.Foreground = Brushes.Black
        cbProjectList.Text = String.Empty
    End Sub

    Public Sub ClearControl()
        rbProfLvl1.IsChecked = False
        rbProfLvl2.IsChecked = False
        rbProfLvl3.IsChecked = False
        rbProfLvl4.IsChecked = False
        rbProfLvl1.Foreground = Brushes.Black
        rbProfLvl2.Foreground = Brushes.Black
        rbProfLvl3.Foreground = Brushes.Black
        rbProfLvl4.Foreground = Brushes.Black
        'lblLastReviewed.Text = String.Empty
    End Sub

    Public Sub ClearFields()
        lblEmpID.Text = String.Empty
        lblName.Text = String.Empty
        lblPosition.Text = String.Empty
        lblDept.Text = String.Empty
        lblEmail.Text = String.Empty
        lblPhone.Text = String.Empty
        lblLocal.Text = String.Empty
        picEmp.Source = Nothing
    End Sub

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
    Private Sub DgSkillList_Loaded(sender As Object, e As RoutedEventArgs) Handles dgSkillList.Loaded
        If dgSkillList.Columns.Count > 0 Then
            dgSkillList.Columns.Item(0).Visibility = Visibility.Collapsed
        End If
    End Sub

#End Region

End Class
