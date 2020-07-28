Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ProblemAddPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    Private mainFrame As Frame
    'Private client As ServiceReference1.AideServiceClient
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private profile As Profile
    Private lstnickName As Nickname()
    Private lstOfEmployees As New ObservableCollection(Of NicknameModel)
    Private empid As Integer
    Private email As String

#End Region

#Region "Constructor"
    Public Sub New(_mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        'client = aideService
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.addframe = _addframe
        Me.mainFrame = _mainFrame
        Me.profile = _profile
        Me.email = _profile.Email_Address
        loadAll()
        PopulateComboBox()
    End Sub
#End Region

#Region "Events"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs) Handles BackBtn.Click
        ExitPage()
    End Sub
    Private Sub btnAddEmp_Click(sender As Object, e As RoutedEventArgs) Handles btnAddEmp.Click
        e.Handled = True
        If cboEmployeeListProblem.SelectedIndex <> -1 Then

            If cboEmployeeListProblem.SelectedItem IsNot Nothing Then
                Dim objNickName As NicknameModel = CType(cboEmployeeListProblem.SelectedItem, NicknameModel)
                AddItemToNewEmpList(objNickName)
            End If
        End If
    End Sub
    Private Sub btnRemoveEmp_Click(sender As Object, e As RoutedEventArgs) Handles btnRemoveEmp.Click
        e.Handled = True
        If cboEmployeeListProblem.SelectedIndex <> -1 Then

            If cboEmployeeListProblem.SelectedItem IsNot Nothing Then
                Dim objNickName As NicknameModel = CType(cboEmployeeListProblem.SelectedItem, NicknameModel)
                RemoveItemToNewEmpList(objNickName)
            End If
        End If
    End Sub
    Private Sub CreateBtn_Click(sender As Object, e As RoutedEventArgs) Handles CreateBtn.Click
        Try
            If ProblemStatementTxt.Text = String.Empty Or lstOfEmployees.Count = 0 Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + vbCritical, "AIDE")
            Else
                Dim _problem As Problem = setData()
                AideClient.GetClient().InsertProblem(_problem)
                MsgBox("Problem has been added.", vbOKOnly + vbInformation, "AIDE")
                ExitPage()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

#Region "Methods"
    'Public Function InitializeService() As Boolean
    '    'Dim bInitialize As Boolean = False
    '    'Try
    '    '    Dim Context As InstanceContext = New InstanceContext(Me)
    '    '    client = New AideServiceClient(Context)
    '    '    client.Open()
    '    '    bInitialize = True
    '    'Catch ex As SystemException
    '    '    client.Abort()
    '    '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    'End Try
    '    'Return bInitialize
    '    Return True
    'End Function
    Public Sub loadAll()
        'If InitializeService() Then
        lstnickName = AideClient.GetClient().ViewNicknameByDeptID(email, 2)
        'End If
    End Sub
    Public Function setData() As Problem
        Try
            Dim _problem As New Problem
            _problem.EmployeeID = profile.Emp_ID
            _problem.ProblemDescr = ProblemStatementTxt.Text
            _problem.ProblemInvolve = ConvertEmpListToString()
            Return _problem
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Function
    Public Sub PopulateComboBox()
        Try

            Dim successRegisterDBProvider As New SuccessRegisterDBProvider
            Dim nicknameVM As New NicknameViewModel()

            If Not IsNothing(lstnickName) Then
                For Each objNickName As Nickname In lstnickName
                    If Not objNickName.Emp_ID = profile.Emp_ID Then
                        successRegisterDBProvider.SetMyNickname(objNickName)
                    End If

                Next

                For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
                    nicknameVM.NicknameList.Add(New NicknameModel(rawUser))
                Next

                cboEmployeeListProblem.ItemsSource = nicknameVM.NicknameList
            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub AddItemToNewEmpList(ByVal objEmp As NicknameModel)
        Try
            If Problem_AssignedAll.Text = String.Empty Then
                Problem_AssignedAll.Text += cboEmployeeListProblem.SelectedValue
                If Not lstOfEmployees.Contains(cboEmployeeListProblem.SelectedItem) Then
                    lstOfEmployees.Add(cboEmployeeListProblem.SelectedItem)
                End If
            Else
                Dim txtBox As String = Problem_AssignedAll.Text
                Dim cbBox As String = cboEmployeeListProblem.SelectedValue
                Dim ifYes As Integer = txtBox.IndexOf(cbBox)
                If ifYes = -1 Then
                    Problem_AssignedAll.Text += ", " + cboEmployeeListProblem.SelectedValue
                    If Not lstOfEmployees.Contains(cboEmployeeListProblem.SelectedItem) Then
                        lstOfEmployees.Add(cboEmployeeListProblem.SelectedItem)
                    End If
                Else
                    MsgBox("Employee already assigned. Please select a different employee.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            End If


        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub RemoveItemToNewEmpList(ByVal objEmp As NicknameModel)
        Try
            If Problem_AssignedAll.Text = String.Empty Then
                MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim txtBox As String = Problem_AssignedAll.Text
                Dim cbBox As String = String.Empty
                Dim ifYes As Integer = txtBox.IndexOf(cboEmployeeListProblem.SelectedValue)

                If ifYes <> -1 Then
                    If ifYes <> 0 Then
                        cbBox = ", " & cboEmployeeListProblem.SelectedValue
                        Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                        Problem_AssignedAll.Text = Problem_AssignedAll.Text.Remove(ifYesAgain, cbBox.Length)
                        If lstOfEmployees.Contains(objEmp) Then
                            lstOfEmployees.Remove(objEmp)
                        End If
                    Else
                            cbBox = cboEmployeeListProblem.SelectedValue & ", "

                        If txtBox.Length <> cboEmployeeListProblem.SelectedValue.Length Then
                            cbBox = Problem_AssignedAll.Text & ", "
                        Else
                            cbBox = Problem_AssignedAll.Text
                        End If
                        Problem_AssignedAll.Text = txtBox.Remove(ifYes, cbBox.Length)
                        If lstOfEmployees.Contains(objEmp) Then
                            lstOfEmployees.Remove(objEmp)
                        End If
                    End If
                Else
                    MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Function ConvertEmpListToString() As String
        Dim empString As String = profile.Emp_ID.ToString()
        If lstOfEmployees.Count > 0 Then
            For Each objEmpNickName As NicknameModel In lstOfEmployees
                empString = empString + "," + objEmpNickName.EMP_ID.ToString()
            Next
            Return empString
        End If

    End Function
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
