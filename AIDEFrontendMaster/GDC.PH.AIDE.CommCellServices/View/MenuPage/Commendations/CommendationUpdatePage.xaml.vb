Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class CommendationUpdatePage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private client As ServiceReference1.AideServiceClient
    Private commendationModel As New CommendationModel
    Private commendationVM As New CommendationViewModel
    Private empID As Integer
    Private position As String
    Private totalCount As Integer
    Private commendFrame As Frame
    Private profiles As Profile
    Private projID As Integer
#End Region

#Region "Constructor"
    Public Sub New(_commendation As CommendationModel, _mainFrame As Frame,
               _position As String, _empID As Integer, _addFrame As Frame, _menuGrid As Grid,
               _subMenuFrame As Frame, _permission As String, _commendFrame As Frame, _profile As Profile)

        ' This call is required by the designer.
        InitializeComponent()
        commendationVM._commendationModel = _commendation
        DataContext = commendationVM
        Me.profiles = _profile
        Me.empID = _empID
        Me.mainFrame = _mainFrame
        Me._addframe = _addFrame
        Me._menugrid = _menuGrid
        Me._submenuframe = _subMenuFrame
        Me.position = _position
        Me.commendFrame = _commendFrame
        Me.comboProject.Text = commendationVM._commendationModel.Project
        LoadAllProjectName()
        AssignEvents(commendationVM._commendationModel.Reason)
        PopulateComboBoxUpdate(Me.projID)
        ' Add any initialization after the InitializeComponent() call.

    End Sub
#End Region

#Region "Main Method"


    Public Sub AssignEvents(Reason As String)
        'txtCommendationEmployees.Text = commendationModel.Employees
        'txtCommendationEmployees.IsReadOnly = True
        'txtSentBy.Text = commendationModel.SentBy
        'txtSentBy.IsReadOnly = True
        txtCommendationReason.AppendText(Reason)
        'txtCommendationReason.IsReadOnly = True
        'dateInput.SelectedDate = commendationModel.DateSent
        'dateInput.IsEnabled = False
        If Not Me.profiles.Permission_ID = 1 Then
            btnCommendationUpdate.Visibility = Windows.Visibility.Hidden
            comboAddEmployee.Visibility = Windows.Visibility.Hidden
            txtemployeeWM.Visibility = Windows.Visibility.Hidden
            txtlabelSelectedEmployee.Visibility = Windows.Visibility.Hidden
            btnCommendationAddEmployee.Visibility = Windows.Visibility.Hidden
            ManagerAuthScreen.Visibility = Windows.Visibility.Visible
        End If


    End Sub
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

    Public Sub PopulateComboBoxUpdate(_projID As Integer)
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = client.GetEmployeePerProject(empID, _projID)
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
                    If iConcern.Project_Name = commendationVM._commendationModel.Project Then
                        Me.projID = iConcern.Project_ID
                    End If
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

    Private Function FindMissingFields() As Boolean
        Dim textRange As New TextRange(txtCommendationReason.Document.ContentStart, txtCommendationReason.Document.ContentEnd)
        If dateInput.SelectedDate Is Nothing Or
           txtSentBy.Text = String.Empty Or
           comboProject.Text = String.Empty Or
           textRange.Text = String.Empty Or
            txtCommendationEmployees.Text = String.Empty Then
            MsgBox("Please fill up all required fields", MsgBoxStyle.Exclamation, "AIDE")
            Return False
        End If
        Return True
    End Function
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

    Private Sub btnCommendationCreate_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub btnCommendationUpdate_Click(sender As Object, e As RoutedEventArgs)
        If FindMissingFields() Then
            Dim ans = MsgBox("Are you sure you want to update commendation?", MsgBoxStyle.YesNo, "AIDE")
            If ans = MsgBoxResult.Yes Then
                'CreateTaskID()
                Dim comm As New Commendations
                Dim textRange As New TextRange(txtCommendationReason.Document.ContentStart, txtCommendationReason.Document.ContentEnd)

                comm.COMMEND_ID = commendationVM._commendationModel.CommendID
                comm.SENT_BY = txtSentBy.Text
                comm.PROJECT = comboProject.Text
                comm.REASON = textRange.Text
                comm.EMPLOYEE = UCase(txtCommendationEmployees.Text)
                comm.DATE_SENT = dateInput.SelectedDate
                comm.EMP_ID = empID
                client.UpdateCommendations(comm)
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
#End Region

#Region "Service Methods"
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
