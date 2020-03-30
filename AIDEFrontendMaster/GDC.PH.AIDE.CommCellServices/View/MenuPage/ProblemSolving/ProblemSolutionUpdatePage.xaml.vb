Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ProblemSolutionUpdatePage
    Implements ServiceReference1.IAideServiceCallback
#Region "Declarations"
    Private mainFrame As Frame
    Private client As ServiceReference1.AideServiceClient
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private profile As Profile
    Private probMod As New ProblemModel

#End Region

#Region "Constructor"
    Public Sub New(_mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _problemModel As ProblemModel)
        ' This call is required by the designer.
        InitializeComponent()
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.addframe = _addframe
        Me.mainFrame = _mainFrame
        Me.profile = _profile
        Me.probMod = _problemModel
        Me.probTitle.Text = _problemModel.OPTION_DESCR
        Me.SolutionDescrTxt.Text = _problemModel.SOLUTION_DESCR
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

#Region "Events"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs) Handles BackBtn.Click
        ExitPage()
    End Sub
    Private Sub CreateBtn_Click(sender As Object, e As RoutedEventArgs) Handles CreateBtn.Click
        Try
            If SolutionDescrTxt.Text = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + vbCritical, "AIDE")
            Else
                If InitializeService() Then
                    Dim _problem As Problem = setData()
                    client.UpdateProblemSolution(_problem)
                    MsgBox("Solution has been updated.", vbOKOnly + vbInformation, "AIDE")
                    ExitPageReload()
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
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

    Public Function setData() As Problem
        Try
            Dim _problem As New Problem
            _problem.SolutionID = probMod.SOLUTION_ID
            _problem.ProblemID = probMod.PROBLEM_ID
            _problem.OptionID = probMod.OPTION_ID
            _problem.SolutionDescr = SolutionDescrTxt.Text

            Return _problem
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Function
    Private Sub ExitPage()
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
    Private Sub ExitPageReload()
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
End Class
