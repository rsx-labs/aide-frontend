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
    Private client As AideServiceClient

    Dim lstSkill As Skills()
    Dim empid As Integer
    Dim skillid As Integer
    Dim proflevel As Integer = 0
    Dim proficiency As Integer
#End Region

#Region "Constructor"

    Public Sub New(empids As Integer)
        ' This call is required by the designer.
        Me.empid = empids
        Me.InitializeComponent()
        LoadProjectList()
        LoadProfile()
        ' Add any initialization after the InitializeComponent() call.
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
        result.Columns.AddRange(columnNames.[Select](Function(c) New DataColumn(c)).ToArray())
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
    Public Sub LoadProjectList()
        Try
            InitializeService()
            Dim lstskill As Skills() = client.GetSkillsList()
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            For Each objSkill As Skills In lstskill
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
    Public Sub LoadSkillsProf()
        Try
            InitializeService()
            Dim lstskill As Skills() = client.GetSkillsProfByEmpID(Convert.ToInt32(lblEmpID.Text))
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            For Each objSkill As Skills In lstskill
                _SkillDBProvider.SetEmpSkillsProf(objSkill)
            Next
            For Each iSkills As mySkillList In _SkillDBProvider.getSkillprof()
                skillslist.Add(New SkillsModel(iSkills))
                dict.Add(iSkills.Skill_Descr, iSkills.Prof_level)  ' Add list of dictionary
            Next
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
    '''By Hyacinth Amarles 
    Public Sub ViewAllEmployee()
        Try
            InitializeService()
            _SkillDBProvider._splist.Clear()
            Dim emp_id As Integer
            lstSkill = client.ViewEmpSKills(empid)
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            For Each objSkill As Skills In lstSkill
                _SkillDBProvider.SetEmpSkillsProf(objSkill)
            Next

            For Each iSkills As mySkillList In _SkillDBProvider.getSkillprof()
                skillslist.Add(New SkillsModel(iSkills))

                '' Adding to dictionary every employees skills description and  skills proficiency
                If iSkills.Emp_ID = emp_id Then
                    dict.Add(iSkills.Skill_Descr, iSkills.Prof_level.ToString)

                End If
                If emp_id <> iSkills.Emp_ID Then
                    If emp_id > 0 Then
                        it.Add(dict)
                    End If
                    dict = New Dictionary(Of String, String)()
                    dict.Add("Employee ID", iSkills.Emp_ID)
                    dict.Add("Employee Name", iSkills.Emp_Name)
                    dict.Add(iSkills.Skill_Descr, iSkills.Prof_level.ToString)

                End If
                emp_id = iSkills.Emp_ID

                ''END PROBLEM
            Next

            it.Add(dict)
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)  ' Convert stored data in dictionary to datable
            ' Skills_descr as Column Header that is dynamic
            dgSkillList.ItemsSource = table.AsDataView
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
    ''' Loads essential information of an employee
    ''' </summary>
    ''' By Jhunell G. Barcenas
    Public Sub LoadProfile()
        Try
            InitializeService()
            Dim lstProfile As Profile = client.GetProfileInformation(empid)
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
        btnAddUpdate.IsEnabled = True
        rbProfLvl1.IsEnabled = True
        rbProfLvl2.IsEnabled = True
        rbProfLvl3.IsEnabled = True
        rbProfLvl4.IsEnabled = True
    End Sub

    Public Sub DisableControls()
        cbProjectList.IsEnabled = False
        btnAddUpdate.IsEnabled = False
        rbProfLvl1.IsEnabled = False
        rbProfLvl2.IsEnabled = False
        rbProfLvl3.IsEnabled = False
        rbProfLvl4.IsEnabled = False
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
        lblLastReviewed.Text = String.Empty
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

#End Region

#Region "Events"

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        DisableControls()
        Try
            InitializeService()
            If txtSearch.Text = String.Empty Then
                LoadProfile()
                ClearSelection()
            ElseIf txtSearch.Text = " " Then
                MsgBox("Please Input Numbers Only!", MsgBoxStyle.Critical, "Employee Assist Tools")
                txtSearch.Text = String.Empty
            Else
                Dim lstProfile As Profile = client.GetProfileInformation(txtSearch.Text)
                If Not IsNothing(lstProfile) Or txtSearch.Text = 0 Then
                        Dim profileList As New ObservableCollection(Of ProfileModel)
                        _ProfileDBProvider = New ProfileDBProvider
                        _ProfileViewModel = New ProfileViewModel
                        _ProfileDBProvider.SetMyProfile(lstProfile)

                        Dim iProfile As MyProfile = _ProfileDBProvider.GetMyProfile()
                        profileList.Add(New ProfileModel(iProfile))

                        _ProfileViewModel.UsersList = profileList

                        Me.DataContext = _ProfileViewModel.UsersList(0)
                        EnableControls()
                        _SkillDBProvider._splist.Clear()
                        LoadSkillsProf()
                        ClearSelection()
                Else
                    ClearFields()
                    dgSkillList.ItemsSource = Nothing
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub btnEditMySkill_Click(sender As Object, e As RoutedEventArgs) Handles btnEditMySkill.Click
        txtSearch.Text = Nothing
        _SkillDBProvider._splist.Clear()
        LoadSkillsProf()
        EnableControls()
    End Sub

    Private Sub btnAddUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnAddUpdate.Click
        Try
            InitializeService()
            skillid = cbProjectList.SelectedValue
            GetProfLevel()
            If cbProjectList.SelectedValue = 0 Or checkRB() = True Then
                MsgBox("Please fill items", MsgBoxStyle.Exclamation, "Employee Assist Tools")
                ClearSelection()
            Else
                If proficiency = proflevel Then
                    MsgBox("You've Selected Your Current Proficiency!" & vbNewLine & "Please Select Another One to Update " & cbProjectList.Text.ToUpper, MsgBoxStyle.Critical, "Employee Assist Tools")
                Else
                    InitializeService()
                    If client.GetProfLvlByEmpIDSkillIDs(Convert.ToInt32(lblEmpID.Text), skillid).Prof_LVL = 1 Then
                        Dim result = MsgBox("You are going to Update " & cbProjectList.Text.ToUpper & " Skill." & vbNewLine & "Do you wish to Continue?", MessageBoxButtons.YesNo, "Employee Assist Tools")
                        If result = vbYes Then
                            UpdateSkillsProficiency()
                        Else
                            ClearSelection()
                        End If
                    Else
                        Dim result = MsgBox("You are going to Add Proficiency Level to " & cbProjectList.Text.ToUpper & " Skill." & vbNewLine & "Do you wish to Continue?", MessageBoxButtons.YesNo, "Employee Assist Tools")
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
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Employee Assist Tools")
        End Try
    End Sub

    Private Sub dgSkillList_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles dgSkillList.LoadingRow
        'Dim RowDataContaxt As SkillsModel = TryCast(e.Row.DataContext, SkillsModel)
        'If RowDataContaxt IsNot Nothing Then
        'If RowDataContaxt.IsREVIEWED = False Then
        'e.Row.Background = New BrushConverter().ConvertFrom("#CCFFD8D8")
        'End If
        'End If
    End Sub

    'Private Sub dgSkillList_AutoGeneratingColumn(sender As Object, e As DataGridAutoGeneratingColumnEventArgs) Handles dgSkillList.AutoGeneratingColumn
    '    Dim tbHeader As TextBlock = New TextBlock()

    '    tbHeader.TextWrapping = TextWrapping.WrapWithOverflow
    '    tbHeader.Text = e.Column.Header.ToString()

    '    e.Column.Header = tbHeader
    'End Sub

    Private Sub dgSkillList_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgSkillList.MouseDoubleClick
        InitializeService()
        If dgSkillList.Items.Count > 1 Then
            If Not dgSkillList.SelectedItem.Equals(Nothing) Then
                Dim emp_id As Integer = dgSkillList.SelectedItem(0)

                Dim lstProfile As Profile = client.GetProfileInformation(emp_id)
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
                btnPrint.Visibility = Windows.Visibility.Hidden
            End If
        End If

    End Sub

    Private Sub btnViewEmp_Click(sender As Object, e As RoutedEventArgs) Handles btnViewEmp.Click
        ViewAllEmployee()
        LoadProfile()
        cbProjectList.IsEnabled = False
        btnAddUpdate.IsEnabled = False
        txtSearch.Text = String.Empty
        btnPrint.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub cbProjectList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProjectList.SelectionChanged
        InitializeService()
        Dim lstSkills As Skills = client.GetSkillsLastReviewByEmpIDSkillID(Convert.ToInt32(lblEmpID.Text), cbProjectList.SelectedValue)
        If IsNothing(lstSkills) Then
            ClearControl()
            proficiency = 0
        Else
            ClearControl()
            Dim SkillsList As New ObservableCollection(Of SkillsModel)

            _SkillDBProvider = New SkillsDBProvider
            _SkillsViewModel = New SkillsViewModel

            _SkillDBProvider.SetSkillsLastReviewedProfLvl(lstSkills)

            Dim iSkills As mySkillList = _SkillDBProvider.GetSkillsLastReviewedProfLvl()
            SkillsList.Add(New SkillsModel(iSkills))
            _SkillsViewModel.SkillList = SkillsList

            lblLastReviewed.DataContext = _SkillsViewModel.SkillList
            proficiency = _SkillsViewModel.SkillList(0)._profLevel
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

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As New System.Windows.Controls.PrintDialog
        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)

            spGrid.Measure(pageSize)

            spGrid.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))

            dialog.PrintVisual(spGrid, "Print Skills Matrix")

        End If
    End Sub

#End Region

End Class
