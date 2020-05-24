Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Collections.ObjectModel

''' <summary>
''' By Jester Sanchez/ Lemuela Abulencia
''' </summary>
''' <remarks></remarks>
Class UpdateActionListPage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback

#Region "Page Declaration"
    Private email As String
    Private profile As Profile
    Private frame As Frame
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private aide As AideServiceClient
    Private action As New Action
    Private actionModel As New ActionModel()
    Private action_provider As New ActionListDBProvider
    Private actionPage_ As HomeActionListsPage
    Private hold_Duedate As String
    Private dsplyByDiv As Integer = 1
#End Region

    Public Sub New(_frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _action As Action, _profile As Profile)
        Try
            frame = _frame
            addframe = _addframe
            menugrid = _menugrid
            submenuframe = _submenuframe
            action = _action
            email = _profile.Email_Address
            profile = _profile

            InitializeComponent()
            DataContext = actionModel
            showUpdateItems()
            PopulateComboBox()
            hold_Duedate = actionModel.DUE_DATE
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#Region "Main Function/Method"

    Public Function showUpdateItems()
        Try
            InitializeService()
            actionModel.REF_NO = action.Act_ID
            actionModel.ACTION_MESSAGE = action.Act_Message
            actionModel.EMP_ID = profile.Emp_ID
            actionModel.NICK_NAME = action.Act_NickName
            actionModel.DUE_DATE = action.Act_DueDate
            actionModel.DATE_CLOSED = action.Act_DateClosed
            Act_AssignedAll.Text = action.Act_NickName
            Return actionModel
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Function

    Public Function getDataUpdate(ByVal ActionModel As ActionModel)
        Try
            InitializeService()
            If ActionModel.REF_NO = Nothing Or ActionModel.ACTION_MESSAGE = Nothing Or Act_AssignedAll.Text = String.Empty Or ActionModel.DUE_DATE = Nothing Or ActionModel.DATE_CLOSED = Nothing Then
                action.Act_ID = ActionModel.REF_NO
                action.Act_Message = ActionModel.ACTION_MESSAGE
                action.Act_Assignee = ActionModel.EMP_ID
                action.Act_DueDate = ActionModel.DUE_DATE
                action.Act_DateClosed = ActionModel.DATE_CLOSED
                action.Act_NickName = Act_AssignedAll.Text
            Else
                action.Act_ID = ActionModel.REF_NO
                action.Act_Message = ActionModel.ACTION_MESSAGE
                action.Act_Assignee = ActionModel.EMP_ID
                action.Act_DueDate = ActionModel.DUE_DATE
                action.Act_DateClosed = ActionModel.DATE_CLOSED
                action.Act_NickName = Act_AssignedAll.Text
            End If

            Return action
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return ex
        End Try
    End Function

    Public Sub PopulateComboBox()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = aide.ViewNicknameByDeptID(email, dsplyByDiv)
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
                Act_AssigneeCB2.DataContext = nicknameVM

            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ExitPage()
        frame.Navigate(New HomeActionListsPage(frame, addframe, menugrid, submenuframe, profile, aide))
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

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aide = New AideServiceClient(Context)
            aide.Open()
            bInitialize = True
        Catch ex As SystemException
            aide.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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

#Region "Events Trigger"
    Private Sub UpdateBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()
            If actionModel.REF_NO = Nothing Or actionModel.ACTION_MESSAGE = Nothing Or Act_AssignedAll.Text = String.Empty Or actionModel.DUE_DATE = Nothing Or actionModel.DATE_CLOSED = Nothing Then
                If actionModel.DATE_CLOSED = Nothing And actionModel.ACTION_MESSAGE <> Nothing And actionModel.DUE_DATE <> Nothing And Act_AssignedAll.Text <> String.Empty Then
                    If MsgBox("Are you sure you want to proceed without a closing date?", MsgBoxStyle.Information + vbYesNo, "AIDE") = vbYes Then
                        aide.UpdateActionList(getDataUpdate(Me.DataContext()))
                        MsgBox("Action item has been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                        action.Act_ID = Nothing
                        action.Act_Message = Nothing
                        action.Act_Assignee = Nothing
                        action.Act_DueDate = Nothing
                        action.Act_DateClosed = Nothing

                        ExitPage()
                    End If
                Else
                    MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                End If
            Else
                aide.UpdateActionList(getDataUpdate(Me.DataContext()))
                MsgBox("Action item has been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
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

    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        ExitPage()
    End Sub

    'Validates if selected duedate is less than the previous due date
    Private Sub Act_DueDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Act_DueDate.SelectedDate < hold_Duedate Then
            MsgBox("Please enter a date on or after the current date.", MsgBoxStyle.Critical, "AIDE")

            Act_DueDate.Text = hold_Duedate
        End If
        If Act_DueDate.Text = String.Empty Then
            Act_DueDate.Text = actionModel.DUE_DATE
        End If
    End Sub

    Private Sub btnRemovedEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnRemovedEmployee.Click
        Try
            e.Handled = True
            If Act_AssignedAll.Text = String.Empty Then
                MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
            ElseIf Act_AssigneeCB2.SelectedValue = String.Empty Then
                MsgBox("Please select an employee", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim txtBox As String = Act_AssignedAll.Text
                Dim cbBox As String = String.Empty
                Dim ifYes As Integer = txtBox.IndexOf(Act_AssigneeCB2.SelectedValue)

                If ifYes <> -1 Then
                    If ifYes <> 0 Then
                        cbBox = ", " & Act_AssigneeCB2.SelectedValue
                        Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                        Act_AssignedAll.Text = Act_AssignedAll.Text.Remove(ifYesAgain, cbBox.Length)
                    Else
                        If txtBox.Length = Act_AssigneeCB2.SelectedValue.Length Then
                            cbBox = Act_AssignedAll.Text
                        Else
                            cbBox = Act_AssigneeCB2.SelectedValue & ", "
                        End If
                        Act_AssignedAll.Text = txtBox.Remove(ifYes, cbBox.Length)
                    End If
                Else
                    MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnAddEmployee_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Act_AssignedAll.Text = String.Empty Then
            Act_AssignedAll.Text += Act_AssigneeCB2.SelectedValue
        Else
            Dim txtBox As String = Act_AssignedAll.Text
            Dim cbBox As String = Act_AssigneeCB2.SelectedValue
            Dim ifYes As Integer = txtBox.IndexOf(cbBox)
            If ifYes = -1 Then
                Act_AssignedAll.Text += ", " + Act_AssigneeCB2.SelectedValue
            Else
                MsgBox("Employee already assigned. Please select a different employee.", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub

    Private Sub btnRemovedEmployee_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Act_AssignedAll.Text = String.Empty Then
            MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
        Else
            Dim txtBox As String = Act_AssignedAll.Text
            Dim cbBox As String = String.Empty
            Dim ifYes As Integer = txtBox.IndexOf(Act_AssigneeCB2.SelectedValue)

            If ifYes <> -1 Then
                If ifYes <> 0 Then
                    cbBox = ", " & Act_AssigneeCB2.SelectedValue
                    Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                    Act_AssignedAll.Text = Act_AssignedAll.Text.Remove(ifYesAgain, cbBox.Length)
                Else
                    cbBox = Act_AssigneeCB2.SelectedValue & ", "

                    If txtBox.Length <> Act_AssigneeCB2.SelectedValue.Length Then
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
