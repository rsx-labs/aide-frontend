Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ProblemViewPage
    Implements ServiceReference1.IAideServiceCallback
#Region "Declarations"
    Private mainFrame As Frame
    Private client As ServiceReference1.AideServiceClient
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private profile As Profile
    Private lstnickName As Nickname()
    Private lstOfEmployees As New ObservableCollection(Of NicknameModel)
    Private lstEmpString As List(Of String)
    Private empid As Integer
    Private email As String
    Private problemMod As New ProblemModel

#End Region

#Region "Constructor"
    Public Sub New(_mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _problemMod As ProblemModel)
        ' This call is required by the designer.
        InitializeComponent()
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.addframe = _addframe
        Me.mainFrame = _mainFrame
        Me.profile = _profile
        Me.email = _profile.Email_Address
        Me.problemMod = _problemMod
        ProblemStatementTxt.Text = _problemMod.PROBLEM_DESCR
        loadAll()
        extractParticipants()
    End Sub
#End Region

#Region "Events"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs) Handles BackBtn.Click
        ExitPage()
    End Sub

#End Region
#Region "Methods"
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

    Public Sub extractParticipants()
        Try
            Dim successRegisterDBProvider As New SuccessRegisterDBProvider
            Dim nicknameVM As New NicknameViewModel()
            lstEmpString = New List(Of String)(problemMod.PROBLEM_INVOLVE.Split(","))
            If lstEmpString.Count > 0 Then
                For Each empString As String In lstEmpString
                    If Not IsNothing(lstnickName) Then
                        If empString = profile.Emp_ID Then
                            Continue For
                        End If
                        For Each objNickName As Nickname In lstnickName
                            If empString = objNickName.Emp_ID.ToString() Then
                                successRegisterDBProvider.SetMyNickname(objNickName)
                                Continue For
                            End If
                        Next
                    End If
                Next
                For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
                    lstOfEmployees.Add(New NicknameModel(rawUser))
                Next
                NewListProblemLV.ItemsSource = lstOfEmployees
            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub loadAll()
        If InitializeService() Then
            lstnickName = client.ViewNicknameByDeptID(email, 1)
        End If
    End Sub

    Private Sub ExitPage()
        mainFrame.Navigate(New ProblemSolvingPage(profile, mainFrame, addframe, menugrid, submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "AIDE Services"
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
