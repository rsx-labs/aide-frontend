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
    Private _frame As Frame
    Private aide As AideServiceClient
    Private action_provider As New ActionListDBProvider
    Private actionPage_ As HomeActionListsPage
    Private act_ion As New Action
    Private _actionModel As New ActionModel()
    Private _email As String
    Private _menugrid As Grid
    Private _addframe As Frame
    Private _submenuframe As Frame
    Private hold_Duedate As String
    Private profiles As Profile
    Private dsplyByDiv As Integer = 1
#End Region

    Public Sub New(f As Frame, act_ As Action, email As String, _menugrid As Grid, _submenuframe As Frame, _addframe As Frame, _prof As Profile)
        Try
            Me._menugrid = _menugrid
            Me._email = email
            Me._submenuframe = _submenuframe
            Me._addframe = _addframe
            Me.profiles = _prof
            _frame = f
            act_ion = act_

            InitializeComponent()
            DataContext = _actionModel
            showUpdateItems()
            PopulateComboBox()
            hold_Duedate = _actionModel.DUE_DATE
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

#Region "Main Function/Method"

    Public Function showUpdateItems()
        Try
            InitializeService()
            _actionModel.REF_NO = act_ion.Act_ID
            _actionModel.ACTION_MESSAGE = act_ion.Act_Message
            _actionModel.EMP_ID = profiles.Emp_ID
            _actionModel.NICK_NAME = act_ion.Act_NickName
            _actionModel.DUE_DATE = act_ion.Act_DueDate
            _actionModel.DATE_CLOSED = act_ion.Act_DateClosed
            Act_AssignedAll.Text = act_ion.Act_NickName
            Return _actionModel
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            End If
            Return ex
        End Try
    End Function

    Public Function getDataUpdate(ByVal ActionModel As ActionModel)
        Try
            InitializeService()
            If ActionModel.REF_NO = Nothing Or ActionModel.ACTION_MESSAGE = Nothing Or Act_AssignedAll.Text = String.Empty Or ActionModel.DUE_DATE = Nothing Or ActionModel.DATE_CLOSED = Nothing Then
                act_ion.Act_ID = ActionModel.REF_NO
                act_ion.Act_Message = ActionModel.ACTION_MESSAGE
                act_ion.Act_Assignee = ActionModel.EMP_ID
                act_ion.Act_DueDate = ActionModel.DUE_DATE
                act_ion.Act_DateClosed = ActionModel.DATE_CLOSED
                act_ion.Act_NickName = Act_AssignedAll.Text
            Else
                act_ion.Act_ID = ActionModel.REF_NO
                act_ion.Act_Message = ActionModel.ACTION_MESSAGE
                act_ion.Act_Assignee = ActionModel.EMP_ID
                act_ion.Act_DueDate = ActionModel.DUE_DATE
                act_ion.Act_DateClosed = ActionModel.DATE_CLOSED
                act_ion.Act_NickName = Act_AssignedAll.Text

            End If
            Return act_ion
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            End If
            Return ex
        End Try
    End Function

    Public Sub PopulateComboBox()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = aide.ViewNicknameByDeptID(_email, dsplyByDiv)
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
            MessageBox.Show(ex.Message)
        End Try
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
            If _actionModel.REF_NO = Nothing Or _actionModel.ACTION_MESSAGE = Nothing Or Act_AssignedAll.Text = String.Empty Or _actionModel.DUE_DATE = Nothing Or _actionModel.DATE_CLOSED = Nothing Then
                If _actionModel.DATE_CLOSED = Nothing And _actionModel.ACTION_MESSAGE <> Nothing And _actionModel.DUE_DATE <> Nothing And Act_AssignedAll.Text <> String.Empty Then
                    If MsgBox("Are you sure you want to proceed without a closing date?", MsgBoxStyle.Information + vbYesNo, "AIDE") = vbYes Then
                        aide.UpdateActionList(getDataUpdate(Me.DataContext()))
                        MsgBox("Action item has been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                        act_ion.Act_ID = Nothing
                        act_ion.Act_Message = Nothing
                        act_ion.Act_Assignee = Nothing
                        act_ion.Act_DueDate = Nothing
                        act_ion.Act_DateClosed = Nothing

                        _frame.Navigate(New HomeActionListsPage(_frame, _email, _addframe, _menugrid, _submenuframe, profiles))
                        _frame.IsEnabled = True
                        _frame.Opacity = 1
                        _menugrid.IsEnabled = True
                        _menugrid.Opacity = 1
                        _submenuframe.IsEnabled = True
                        _submenuframe.Opacity = 1
                        _addframe.Visibility = Visibility.Hidden
                    End If
                Else
                    MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                End If
            Else
                aide.UpdateActionList(getDataUpdate(Me.DataContext()))
                MsgBox("Action item has been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                act_ion.Act_ID = Nothing
                act_ion.Act_Message = Nothing
                act_ion.Act_Assignee = Nothing
                act_ion.Act_DueDate = Nothing
                act_ion.Act_DateClosed = Nothing

                _frame.Navigate(New HomeActionListsPage(_frame, _email, _addframe, _menugrid, _submenuframe, profiles))
                _frame.IsEnabled = True
                _frame.Opacity = 1
                _menugrid.IsEnabled = True
                _menugrid.Opacity = 1
                _submenuframe.IsEnabled = True
                _submenuframe.Opacity = 1
                _addframe.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "AIDE") = vbYes Then
                Environment.Exit(0)
            End If
        End Try
    End Sub

    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        _frame.Navigate(New HomeActionListsPage(_frame, _email, _addframe, _menugrid, _submenuframe, profiles))
        _frame.IsEnabled = True
        _frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

    'Validates if selected duedate is less than the previous due date
    Private Sub Act_DueDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Act_DueDate.SelectedDate < hold_Duedate Then
            MsgBox("Please enter a date on or after the current date.", MsgBoxStyle.Critical, "AIDE")

            Act_DueDate.Text = hold_Duedate
        End If
        If Act_DueDate.Text = String.Empty Then
            Act_DueDate.Text = _actionModel.DUE_DATE
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
            MessageBox.Show(ex.Message)
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
