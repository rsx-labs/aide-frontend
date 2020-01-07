Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class CommendationAddPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private client As ServiceReference1.AideServiceClient
    Private commendationModel As New CommendationModel
    Private empID As Integer
    Private position As String
    Private totalCount As Integer
    Private email As String
    Private commendFrame As Frame
    Private profiles As Profile
    'Private srmodel As SuccessRegisterModel

#End Region

#Region "Constructor"
    'Add Commendation
    Public Sub New(_mainFrame As Frame, _position As String, _empID As Integer, _addFrame As Frame, _menuGrid As Grid, _subMenuFrame As Frame, _email As String, _profile As Profile, _commendFrame As Frame)

        InitializeComponent()
        Me.empID = _empID
        Me.mainFrame = _mainFrame
        Me._addframe = _addFrame
        Me._menugrid = _menuGrid
        Me._submenuframe = _subMenuFrame
        Me.position = _position
        Me.profiles = _profile
        Me.commendFrame = _commendFrame
        Me.email = _email
        btnCommendationCreate.Visibility = System.Windows.Visibility.Visible
        tbSuccessForm.Text = "Create Commendation"
        'lblCommID.Visibility = Windows.Visibility.Hidden
        'AssignEvents()
        LoadAllProjectName()
        'PopulateComboBox()
    End Sub

    '
    Public Sub New(_commendation As CommendationModel, mainFrame As Frame,
                   _position As String, _empID As Integer, _addFrame As Frame, _menuGrid As Grid, _subMenuFrame As Frame)

        InitializeComponent()

        Me.mainFrame = mainFrame
        Me._addframe = _addFrame
        Me._menugrid = _menuGrid
        Me._submenuframe = _subMenuFrame
        Me.empID = _empID
        Me.position = _position
        btnCommendationCreate.Visibility = System.Windows.Visibility.Hidden
        Me.commendationModel = _commendation
        AssignEvents()
        'PopulateComboBox()
    End Sub

#End Region

#Region "Events"
    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        'mainFrame.Navigate(New HomePage(mainFrame, position, empID, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub comboProject_DropDownClosed(sender As Object, e As EventArgs) Handles comboProject.DropDownClosed
        PopulateComboBox()
        comboAddEmployee.IsEnabled = True
        btnCommendationAddEmployee.IsEnabled = True
    End Sub

    Private Sub btnCommendationCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCommendationCreate.Click
        If FindMissingFields() Then
            Dim ans = MsgBox("Are you sure you want to create commendation?", MsgBoxStyle.YesNo, "AIDE")
            If ans = MsgBoxResult.Yes Then
                'CreateTaskID()
                Dim comm As New Commendations
                Dim textRange As New TextRange(txtCommendationReason.Document.ContentStart, txtCommendationReason.Document.ContentEnd)

                'comm.COMMEND_ID = totalCount
                comm.SENT_BY = txtSentBy.Text
                comm.PROJECT = comboProject.Text
                comm.REASON = textRange.Text
                comm.EMPLOYEE = UCase(txtCommendationEmployees.Text)
                comm.DATE_SENT = dateInput.SelectedDate
                comm.EMP_ID = empID
                client.InsertCommendations(comm)
                'mainFrame.Navigate(New HomePage(mainFrame, position, empID, _addframe, _menugrid, _submenuframe, _))
                commendFrame.Navigate(New CommendationDashBoard(mainFrame, Me.position, Me.empID, _addframe, _menugrid, _submenuframe, Me.profiles.Email_Address, Me.profiles, commendFrame))
                mainFrame.IsEnabled = True
                mainFrame.Opacity = 1
                _menugrid.IsEnabled = True
                _menugrid.Opacity = 1
                _submenuframe.IsEnabled = True
                _submenuframe.Opacity = 1

                _addframe.Visibility = Visibility.Hidden
            End If
        End If
    End Sub

    Private Sub btnCommendationAddEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnCommendationAddEmployee.Click
        e.Handled = True
        If txtCommendationEmployees.Text = String.Empty Then
            txtCommendationEmployees.Text += comboAddEmployee.Text
        Else
            Dim txtBox As String = txtCommendationEmployees.Text
            Dim cbBox As String = comboAddEmployee.Text
            Dim ifYes As Integer = txtBox.IndexOf(cbBox)
            If ifYes = -1 Then
                txtCommendationEmployees.Text += ", " + comboAddEmployee.Text
            Else
                MsgBox("Cannot allow duplicate entry!", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub

    Private Sub txtCommendationReason_KeyDown(sender As Object, e As KeyEventArgs)
        Dim textRange As New TextRange(txtCommendationReason.Document.ContentStart, txtCommendationReason.Document.ContentEnd)
        If textRange.Text.Length >= 10 Then
            e.Handled = False
        End If
    End Sub

    Private Sub txtCommendationReason_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtCommendationReason.TextChanged

    End Sub
#End Region

#Region "Functions"
    Public Sub AssignEvents()
        tbSuccessForm.Text = "View Commendation"
        txtCommendationEmployees.Text = commendationModel.Employees
        txtCommendationEmployees.IsReadOnly = True
        txtSentBy.Text = commendationModel.SentBy
        txtSentBy.IsReadOnly = True
        txtCommendationReason.AppendText(commendationModel.Reason)
        txtCommendationReason.IsReadOnly = True
        dateInput.SelectedDate = commendationModel.DateSent
        dateInput.IsEnabled = False
        'txtProject.Text = commendationModel.Project
        'txtProject.Visibility = Windows.Visibility.Visible
        comboProject.Visibility = Windows.Visibility.Hidden
        comboAddEmployee.IsEnabled = False
        btnCommendationAddEmployee.IsEnabled = False
        'comboxProjName.Visibility = Windows.Visibility.Collapsed
        comboAddEmployee.Visibility = Windows.Visibility.Collapsed
        btnCommendationAddEmployee.Visibility = Windows.Visibility.Collapsed
        txtemployeeWM.Visibility = Windows.Visibility.Collapsed
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

    ''' <summary>
    ''' load employee per project in comboProject
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateComboBox()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = client.GetEmployeePerProject(empID, comboProject.SelectedValue)
                Dim lstNicknameList As New ObservableCollection(Of NicknameModel)
                Dim successRegisterDBProvider As New SuccessRegisterDBProvider
                Dim nicknameVM As New NicknameViewModel()

                For Each objLessonLearnt As Nickname In lstNickname
                    successRegisterDBProvider.SetMyNickname(objLessonLearnt)
                Next

                For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
                    lstNicknameList.Add(New NicknameModel(rawUser))
                Next

                nicknameVM.NicknameList = lstNicknameList

                comboAddEmployee.ItemsSource = nicknameVM.NicknameList
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadAllProjectName()

        Try
            If InitializeService() Then
                Dim _GetAllConcernDBProvider As New ProjectDBProvider
                Dim _projectViewModel As New ProjectViewModel

                Dim displayStatus As Integer = 0
                Dim lstConcern As Project() = client.GetAllListOfProject(empID, displayStatus)
                Dim lstConcernList As New ObservableCollection(Of ProjectModel)


                For Each objConcern As Project In lstConcern
                    _GetAllConcernDBProvider.setProjectList(objConcern)
                Next

                For Each iConcern As myProjectList In _GetAllConcernDBProvider.getProjectList()

                    lstConcernList.Add(New ProjectModel(iConcern))

                Next
                _projectViewModel.ProjectList = lstConcernList

                comboProject.ItemsSource = _projectViewModel.ProjectList
            End If
        Catch ex As SystemException

            MsgBox(ex.Message)
            client.Abort()

        End Try
    End Sub

    Private Sub CreateTaskID()
        Try
            If Me.InitializeService() Then
                Dim lstTasks As Tasks() = client.GetAllTasks()

                totalCount = lstTasks.Length + 1
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "AIDE")
        End Try
    End Sub

    Private Function FindMissingFields() As Boolean
        Dim textRange As New TextRange(txtCommendationReason.Document.ContentStart, txtCommendationReason.Document.ContentEnd)
        If dateInput.SelectedDate Is Nothing Or
           txtSentBy.Text = String.Empty Or
           comboProject.Text = String.Empty Or
           textRange.Text = String.Empty Or
            txtCommendationEmployees.Text = String.Empty Then
            MsgBox("Please fill up all required fields!", MsgBoxStyle.Exclamation, "AIDE")
            Return False
        End If
        Return True
    End Function
#End Region

#Region "Callback Functions"

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
