Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel

''' <summary>
''' By Jester Sanchez/ Lemuela Abulencia
''' </summary>
''' <remarks></remarks>
Class InsertActionListPage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback

#Region "Page Declaration"
    Private email As String
    Private profile As Profile
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    'Private aide As AideServiceClient
    Private action As New Action
    Private actionModel As New ActionModel()
    Private dsplyByDiv As Integer = 1
#End Region

    Public Sub New(_frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)
        Try
            frame = _frame
            addframe = _addframe
            menugrid = _menugrid
            submenuframe = _submenuframe
            email = _profile.Email_Address
            profile = _profile
            InitializeComponent()
            PopulateComboBox()
            DataContext = actionModel
            GenerateActionRef()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#Region "Main Function/Method"
    Private Sub GenerateActionRef()
        Try
            'InitializeService()
            Dim actionModel As New ActionModel
            Dim refCode As String = "ACT"
            Dim refNoCount As Integer
            Dim refNo As String
            Dim action As Action() = AideClient.GetClient().GetActionSummary(email)
            refNoCount = action.Count + 1
            refNo = refCode + "-" + Convert.ToString(Today.Date.ToString("MM/dd/yy")) + "-" + Convert.ToString(refNoCount)
            actionModel.REF_NO = refNo
            actionModel.DUE_DATE = Today.Date
            actionModel.DATE_CLOSED = Today.Date

            DataContext = actionModel
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub PopulateComboBox()
        Try
            'If InitializeService() Then
            Dim lstNickname As Nickname() = AideClient.GetClient().ViewNicknameByDeptID(email, dsplyByDiv)
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

            Act_AssigneeCB.DataContext = nicknameVM

            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Function getDataInsert(ByVal ActionModel As ActionModel)
        Try
            'InitializeService()
            If ActionModel.REF_NO = Nothing Or ActionModel.ACTION_MESSAGE = Nothing Or Act_AssigneeCB.SelectedValue = Nothing Or ActionModel.DUE_DATE = Nothing Or ActionModel.DATE_CLOSED = Nothing Then
            Else
                action.Act_ID = ActionModel.REF_NO
                action.Act_Message = ActionModel.ACTION_MESSAGE
                action.Act_Assignee = profile.Emp_ID
                action.Act_DueDate = ActionModel.DUE_DATE
                action.Act_DateClosed = String.Empty
                action.Act_NickName = Act_AssignedAll.Text
            End If
            Return action
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return ex
        End Try
    End Function

    Private Sub ExitPage()
        frame.Navigate(New HomeActionListsPage(frame, addframe, menugrid, submenuframe, profile))
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "Services Function/Method"
    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        aide = New AideServiceClient(Context)
    '        aide.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        aide.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

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

#Region "Events Trigger"
    Private Sub AddBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            'InitializeService()

            AideClient.GetClient().InsertActionList(getDataInsert(Me.DataContext()))
            If action.Act_ID = Nothing Or action.Act_Message = Nothing Or action.Act_Assignee = Nothing Or action.Act_DueDate = Nothing Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                MsgBox("Action item has been added.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                GenerateActionRef()
                action.Act_ID = Nothing
                action.Act_Message = Nothing
                action.Act_Assignee = Nothing
                action.Act_DueDate = Nothing
                action.Act_DateClosed = Nothing

                ExitPage()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub backbtn_Click(sender As Object, e As RoutedEventArgs)
        ExitPage()
    End Sub

    Private Sub Act_DueDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Act_DueDate.SelectedDate < Today.Date Then
            MsgBox("Please enter date on or before the current date.", MsgBoxStyle.Exclamation, "AIDE")
            Act_DueDate.Text = Today.Date
        End If
    End Sub

    Private Sub btnAddEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnAddEmployee.Click
        e.Handled = True
        If Act_AssignedAll.Text = String.Empty Then
            Act_AssignedAll.Text += Act_AssigneeCB.SelectedValue
        Else
            Dim txtBox As String = Act_AssignedAll.Text
            Dim cbBox As String = Act_AssigneeCB.SelectedValue
            Dim ifYes As Integer = txtBox.IndexOf(cbBox)
            If ifYes = -1 Then
                Act_AssignedAll.Text += ", " + Act_AssigneeCB.SelectedValue
            Else
                MsgBox("Employee already assigned. Please select a different employee.", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub

    Private Sub btnRemovedEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnRemovedEmployee.Click
        e.Handled = True
        If Act_AssignedAll.Text = String.Empty Then
            MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
        Else
            Dim txtBox As String = Act_AssignedAll.Text
            Dim cbBox As String = String.Empty
            Dim ifYes As Integer = txtBox.IndexOf(Act_AssigneeCB.SelectedValue)

            If ifYes <> -1 Then
                If ifYes <> 0 Then
                    cbBox = ", " & Act_AssigneeCB.SelectedValue
                    Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                    Act_AssignedAll.Text = Act_AssignedAll.Text.Remove(ifYesAgain, cbBox.Length)
                Else
                    cbBox = Act_AssigneeCB.SelectedValue & ", "

                    If txtBox.Length <> Act_AssigneeCB.SelectedValue.Length Then
                        cbBox = Act_AssignedAll.Text & ", "
                    Else
                        cbBox = Act_AssignedAll.Text
                    End If
                    Act_AssignedAll.Text = txtBox.Remove(ifYes, cbBox.Length)
                End If
            Else
                MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub
#End Region

End Class
