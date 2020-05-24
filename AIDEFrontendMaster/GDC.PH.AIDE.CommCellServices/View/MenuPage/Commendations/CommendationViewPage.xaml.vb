Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class CommendationViewPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    'Private client As ServiceReference1.AideServiceClient
    Private commendationModel As New CommendationModel
    Private empID As Integer
    Private position As String
    Private totalCount As Integer
    'Private srmodel As SuccessRegisterModel

#End Region

#Region "Constructor"
    'Add Commendation
    'Public Sub New(_mainFrame As Frame, _position As String, _empID As Integer, _addFrame As Frame, _menuGrid As Grid, _subMenuFrame As Frame)

    '    InitializeComponent()
    '    Me.empID = _empID
    '    Me.mainFrame = _mainFrame
    '    Me._addframe = _addFrame
    '    Me._menugrid = _menuGrid
    '    Me._submenuframe = _subMenuFrame
    '    Me.position = _position
    '    btnCommendationCreate.Visibility = System.Windows.Visibility.Visible
    '    tbSuccessForm.Text = "Create Commendation"
    '    txtCommendationID.Visibility = Windows.Visibility.Hidden
    '    comboxProjName.Visibility = Windows.Visibility.Visible
    '    txtemployeeWM.Visibility = Windows.Visibility.Visible
    '    'lblCommID.Visibility = Windows.Visibility.Hidden
    '    'AssignEvents()
    '    LoadAllProjectName()
    '    'PopulateComboBox()
    'End Sub

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
        _addframe.Margin = New Thickness(150, 60, 150, 60)
        _addframe.Visibility = Visibility.Hidden
    End Sub

#End Region

#Region "Functions"
    Public Sub AssignEvents()
        tbSuccessForm.Text = "View Commendation"
        txtCommendationID.Text = commendationModel.CommendID
        txtCommendationID.IsReadOnly = True
        txtCommendationEmployees.Text = commendationModel.Employees
        txtCommendationEmployees.IsReadOnly = True
        txtSentBy.Text = commendationModel.SentBy
        txtSentBy.IsReadOnly = True
        txtCommendationReason.AppendText(commendationModel.Reason)
        txtCommendationReason.IsReadOnly = True
        dateInput.SelectedDate = commendationModel.DateSent
        dateInput.IsEnabled = False
        txtProject.Text = commendationModel.Project
        txtProject.Visibility = Windows.Visibility.Visible
        comboProject.Visibility = Windows.Visibility.Hidden
        comboxProjName.Visibility = Windows.Visibility.Collapsed
        txtBlockMsg.Visibility = Windows.Visibility.Collapsed


    End Sub

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        'DisplayText("Opening client service...")
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        client = New AideServiceClient(Context)
    '        client.Open()
    '        bInitialize = True
    '        'DisplayText("Service opened successfully...")
    '        'Return True
    '    Catch ex As SystemException
    '        client.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    ''' <summary>
    ''' load employee per project in comboProject
    ''' </summary>
    ''' <remarks></remarks>


    Public Sub LoadAllProjectName()

        Try
            'If InitializeService() Then
            Dim _GetAllConcernDBProvider As New ProjectDBProvider
            Dim _projectViewModel As New ProjectViewModel

            Dim displayStatus As Integer = 0
            Dim lstConcern As Project() = AideClient.GetClient().GetAllListOfProject(empID, displayStatus)
            Dim lstConcernList As New ObservableCollection(Of ProjectModel)


            For Each objConcern As Project In lstConcern
                _GetAllConcernDBProvider.setProjectList(objConcern)
            Next

            For Each iConcern As myProjectList In _GetAllConcernDBProvider.getProjectList()

                lstConcernList.Add(New ProjectModel(iConcern))

            Next
            _projectViewModel.ProjectList = lstConcernList

            comboProject.ItemsSource = _projectViewModel.ProjectList
            'End If
        Catch ex As SystemException

             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            'client.Abort()

        End Try
    End Sub

    Private Sub CreateTaskID()
        Try
            'If Me.InitializeService() Then
            Dim lstTasks As Tasks() = AideClient.GetClient().GetAllTasks()

            totalCount = lstTasks.Length + 1
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Function FindMissingFields() As Boolean
        Dim textRange As New TextRange(txtCommendationReason.Document.ContentStart, txtCommendationReason.Document.ContentEnd)
        If dateInput.SelectedDate Is Nothing Or
           txtSentBy.Text = String.Empty Or
           comboProject.Text = String.Empty Or
           textRange.Text = String.Empty Or
            txtCommendationEmployees.Text = String.Empty Then
            MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
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
