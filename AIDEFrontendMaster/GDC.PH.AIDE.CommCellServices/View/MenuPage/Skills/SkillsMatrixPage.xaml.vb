Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices
Imports System.Windows.Forms
Imports System.Data

''' Skills Matrix Page
''' By Jhunell G. Barcenas / Hyacinth Amarles / Richard Espida

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class SkillsMatrixPage
    Implements IAideServiceCallback

#Region "Fields"
    Private _SkillDBProvider As New SkillsDBProvider
    Private _SkillsViewModel As New SkillsViewModel
    Private _ProfileDBProvider As New ProfileDBProvider
    Private _ProfileViewModel As New ProfileViewModel
    Private client As AideServiceClient

    Dim emails As String
    Dim empid As Integer
    Dim proflevel As Integer = 0
    Dim proficiency As Integer
#End Region

#Region "Constructor"

    Public Sub New(empid As Integer)
        ' This call is required by the designer.
        Me.empid = empid
        InitializeComponent()

        LoadProjectList()
        LoadSkillsProf()
        loadProfile()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

#End Region

#Region "Function Procedures"
    ''' <summary>
    ''' Loads dictionary into data table
    ''' </summary>
    ''' By Hyacinth Amarles
    Private Function ToDataTable(list As List(Of Dictionary(Of String, Integer))) As DataTable
        Dim result As New DataTable()

        If list.Count = 0 Then
            Return result
        End If

        Dim columnNames = list.SelectMany(Function(dict) dict.Keys).Distinct()
        result.Columns.AddRange(columnNames.[Select](Function(c) New DataColumn(c)).ToArray())
        For Each item As Dictionary(Of String, Integer) In list
            Dim row = result.NewRow()
            For Each key In item.Keys
                row(key) = item(key)
            Next
            result.Rows.Add(row)
        Next
        Return result
    End Function
    ''' <summary>
    ''' Initialize Service
    ''' </summary>
    ''' By Hyacinth Amarles
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

    Public Function checkRB() As Boolean
        If rbSkill1.IsChecked = False And rbSkill2.IsChecked = False And rbSkill3.IsChecked = False And rbSkill4.IsChecked = False Then
            Return True
        End If
        Return False
    End Function

#End Region

#Region "Sub Procedures"
    ''' <summary>
    ''' Insert proficiency level to a skill of an employee
    ''' </summary>
    ''' By Hyacinth Amarles
    Public Sub InsertSkillsProficiency()
        Dim skillid As Integer = cbSkillList.SelectedValue
        Try
            InitializeService()
            Dim Skills As New Skills
            GetProfLevel()
            Skills.DESCR = String.Empty
            Skills.NAME = String.Empty
            Skills.Image_Path = String.Empty
            Skills.EmpID = empid
            Skills.Prof_LVL = proflevel
            Skills.SkillID = cbSkillList.SelectedValue
            Skills.Last_Reviewed = Date.Now
            client.InsertNewSkills(Skills)
            MsgBox(cbSkillList.Text.ToUpper & "has been added. ", MsgBoxStyle.Information, "AIDE")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub
    ''' <summary>
    ''' Changing the proficiency level value of a skill into a new proficiency level
    ''' </summary>
    ''' By Hyacinth Amarles
    Public Sub UpdateSkillsProficiency()
        Try
            InitializeService()
            Dim Skills As New Skills
            GetProfLevel()
            Skills.DESCR = String.Empty
            Skills.NAME = String.Empty
            Skills.Image_Path = String.Empty
            Skills.EmpID = empid
            Skills.Prof_LVL = proflevel
            Skills.SkillID = cbSkillList.SelectedValue
            Skills.Last_Reviewed = Date.Now
            client.UpdateSkills(Skills)
            MsgBox(cbSkillList.Text.ToUpper & "has been updated. ", MsgBoxStyle.Information, "AIDE")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub
    ''' <summary>
    ''' Loads employee skills in combo box
    ''' </summary>
    ''' By Hyacinth Amarles
    Public Sub LoadProjectList()
        Try
            InitializeService()
            Dim lstskill As Skills() = client.GetSkillsList(empid)
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            For Each objSkill As Skills In lstskill
                _SkillDBProvider.SetSkillList(objSkill)
            Next

            For Each iSkills As mySkillList In _SkillDBProvider.GetSkillList()
                skillslist.Add(New SkillsModel(iSkills))
            Next
            _SkillsViewModel.SkillList = skillslist

            cbSkillList.DataContext = _SkillsViewModel
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub


    ''' <summary>
    ''' Loads employee skills proficiency in datagrid
    ''' </summary>
    ''' By Hyacinth Amarles
    Public Sub LoadSkillsProf()
        Try
            InitializeService()
            Dim lstskill As Skills() = client.GetSkillsProfByEmpID(empid)
            Dim skillslist As New ObservableCollection(Of SkillsModel)

            Dim it As New List(Of Dictionary(Of String, Integer))()
            Dim dict As New Dictionary(Of String, Integer)()

            For Each objSkill As Skills In lstskill
                _SkillDBProvider.SetEmpSkillsProf(objSkill)
            Next
            For Each iSkills As mySkillList In _SkillDBProvider.getSkillprof()
                skillslist.Add(New SkillsModel(iSkills))
                dict.Add(iSkills.Skill_Descr, iSkills.Prof_level)  ' Add list of data dictionary
            Next
            it.Add(dict)
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)  ' Convert stored data in dictionary to datable to display in datagrid
            dgSkill.ItemsSource = table.AsDataView
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

    ''' <summary>
    ''' Loads essential information of an employee
    ''' </summary>
    ''' By Jhunell G. Barcenas
    Public Sub loadProfile()
        InitializeService()
        Dim lstProfile As Profile = client.GetProfileInformation(empid)
        Dim profileList As New ObservableCollection(Of ProfileModel)

        _ProfileDBProvider = New ProfileDBProvider
        _ProfileViewModel = New ProfileViewModel

        _ProfileDBProvider.SetMyProfile(lstProfile)

        Dim iProfile As MyProfile = _ProfileDBProvider.GetMyProfile()
        profileList.Add(New ProfileModel(iProfile))
        _ProfileViewModel.UsersList = profileList

        Me.DataContext = _ProfileViewModel.UsersList
    End Sub

    Public Sub GetProfLevel()
        If rbSkill1.IsChecked Then
            proflevel = 1
        ElseIf rbSkill2.IsChecked Then
            proflevel = 2
        ElseIf rbSkill3.IsChecked Then
            proflevel = 3
        ElseIf rbSkill4.IsChecked Then
            proflevel = 4
        End If
    End Sub

    Public Sub ClearSelection()
        rbSkill1.IsChecked = False
        rbSkill2.IsChecked = False
        rbSkill3.IsChecked = False
        rbSkill4.IsChecked = False
        rbSkill1.Foreground = Brushes.Black
        rbSkill2.Foreground = Brushes.Black
        rbSkill3.Foreground = Brushes.Black
        rbSkill4.Foreground = Brushes.Black
        cbSkillList.Text = Nothing
    End Sub

    Public Sub ClearControl()
        rbSkill1.IsChecked = False
        rbSkill2.IsChecked = False
        rbSkill3.IsChecked = False
        rbSkill4.IsChecked = False
        rbSkill1.Foreground = Brushes.Black
        rbSkill2.Foreground = Brushes.Black
        rbSkill3.Foreground = Brushes.Black
        rbSkill4.Foreground = Brushes.Black
        lblLastReviewed.Text = Nothing
    End Sub

    Public Sub EnableControls()
        cbSkillList.IsEnabled = True
        rbSkill1.IsEnabled = True
        rbSkill2.IsEnabled = True
        rbSkill3.IsEnabled = True
        rbSkill4.IsEnabled = True
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
    Private Sub btnAddUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnAddUpdate.Click
        Try
            InitializeService()
            Dim skillid As Integer = cbSkillList.SelectedValue
            GetProfLevel()
            If cbSkillList.SelectedValue = 0 Or checkRB() = True Then
                MsgBox("Please fill items", MsgBoxStyle.Exclamation, "AIDE")
                ClearSelection()
            Else
                If proficiency = proflevel Then
                    MsgBox("There is no change in proficiency level. " & vbNewLine & "Please select another skill to update." & cbSkillList.Text.ToUpper, MsgBoxStyle.Critical, "AIDE")
                Else
                    If client.GetProfLvlByEmpIDSkillIDs(empid, skillid).Prof_LVL = 1 Then

                        UpdateSkillsProficiency()

                        Else

                        InsertSkillsProficiency()

                        End If
                End If

                _SkillDBProvider._splist.Clear()
                LoadSkillsProf()
                ClearSelection()

            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub

    Private Sub cbSkillList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbSkillList.SelectionChanged
        EnableControls()
        InitializeService()
        Dim lstSkills As Skills = client.GetSkillsLastReviewByEmpIDSkillID(empid, cbSkillList.SelectedValue)

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
                rbSkill1.IsChecked = True
                rbSkill1.Foreground = Brushes.Red
            ElseIf proficiency = 2 Then
                rbSkill2.IsChecked = True
                rbSkill2.Foreground = Brushes.Red
            ElseIf proficiency = 3 Then
                rbSkill3.IsChecked = True
                rbSkill3.Foreground = Brushes.Red
            ElseIf proficiency = 4 Then
                rbSkill4.IsChecked = True
                rbSkill4.Foreground = Brushes.Red
            End If
        End If
    End Sub
#End Region

End Class
