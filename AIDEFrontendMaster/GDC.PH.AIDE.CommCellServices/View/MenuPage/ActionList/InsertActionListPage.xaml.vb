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
    Public _frame As Frame
    Private aide As AideServiceClient
    Private act_ion As New Action
    Private _actionModel As New ActionModel()
    Private __email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profiles As Profile
    Private dsplyByDiv As Integer = 1
#End Region

    Public Sub New(F As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _prof As Profile)
        Try
            __email = _email
            Me._frame = _frame
            Me._addframe = _addframe
            Me._menugrid = _menugrid
            Me._submenuframe = _submenuframe
            Me.profiles = _prof
            _frame = F
            InitializeComponent()
            PopulateComboBox()
            Me.DataContext = _actionModel
            GenerateActionRef()
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

#Region "Main Function/Method"
    Private Sub GenerateActionRef()
        Try
            InitializeService()
            Dim actionModel As New ActionModel
            Dim refCode As String = "ACT"
            Dim refNoCount As Integer
            Dim refNo As String
            Dim action As Action() = aide.GetActionSummary(__email)
            refNoCount = action.Count + 1
            refNo = refCode + "-" + Convert.ToString(Today.Date.ToString("MM/dd/yy")) + "-" + Convert.ToString(refNoCount)
            actionModel.REF_NO = refNo
            actionModel.DUE_DATE = Today.Date
            actionModel.DATE_CLOSED = Today.Date

            DataContext = actionModel
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

    Public Sub PopulateComboBox()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = aide.ViewNicknameByDeptID(__email, dsplyByDiv)
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

            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Function getDataInsert(ByVal ActionModel As ActionModel)
        Try
            InitializeService()
            If ActionModel.REF_NO = Nothing Or ActionModel.ACTION_MESSAGE = Nothing Or Act_AssigneeCB.SelectedValue = Nothing Or ActionModel.DUE_DATE = Nothing Or ActionModel.DATE_CLOSED = Nothing Then
            Else
                act_ion.Act_ID = ActionModel.REF_NO
                act_ion.Act_Message = ActionModel.ACTION_MESSAGE
                act_ion.Act_Assignee = Act_AssigneeCB.SelectedValue
                act_ion.Act_DueDate = ActionModel.DUE_DATE
                act_ion.Act_DateClosed = String.Empty
                act_ion.Act_NickName = Act_AssignedAll.Text
            End If
            Return act_ion
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
            Return ex
        End Try
    End Function

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
    Private Sub AddBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()

            aide.InsertActionList(getDataInsert(Me.DataContext()))
            If act_ion.Act_ID = Nothing Or act_ion.Act_Message = Nothing Or act_ion.Act_Assignee = Nothing Or act_ion.Act_DueDate = Nothing Then
                MsgBox("Please fill up all required fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                MsgBox("Successfully added!", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                GenerateActionRef()
                act_ion.Act_ID = Nothing
                act_ion.Act_Message = Nothing
                act_ion.Act_Assignee = Nothing
                act_ion.Act_DueDate = Nothing
                act_ion.Act_DateClosed = Nothing

                _frame.Navigate(New HomeActionListsPage(_frame, __email, _addframe, _menugrid, _submenuframe, profiles))
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
            Else
            End If
        End Try
    End Sub

    Private Sub backbtn_Click(sender As Object, e As RoutedEventArgs)
        _frame.Navigate(New HomeActionListsPage(_frame, __email, _addframe, _menugrid, _submenuframe, profiles))
        _frame.IsEnabled = True
        _frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub Act_DueDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Act_DueDate.SelectedDate < Today.Date Then
            MsgBox("Due date should be prior ahead to today's date.", MsgBoxStyle.Exclamation, "AIDE")
            Act_DueDate.Text = Today.Date
        End If
    End Sub

    Private Sub btnAddEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnAddEmployee.Click
        e.Handled = True
        If Act_AssignedAll.Text = String.Empty Then
            Act_AssignedAll.Text += Act_AssigneeCB.Text
        Else
            Dim txtBox As String = Act_AssignedAll.Text
            Dim cbBox As String = Act_AssigneeCB.Text
            Dim ifYes As Integer = txtBox.IndexOf(cbBox)
            If ifYes = -1 Then
                Act_AssignedAll.Text += ", " + Act_AssigneeCB.Text
            Else
                MsgBox("Cannot allow duplicate entry!", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub

    Private Sub btnRemovedEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnRemovedEmployee.Click
        e.Handled = True
        If Act_AssignedAll.Text = String.Empty Then
            MsgBox("Textbox is empty", MsgBoxStyle.Exclamation, "AIDE")
        Else
            Dim txtBox As String = Act_AssignedAll.Text
            Dim cbBox As String = String.Empty
            Dim ifYes As Integer = txtBox.IndexOf(Act_AssigneeCB.Text)

            If ifYes <> -1 Then
                If ifYes <> 0 Then
                    cbBox = ", " & Act_AssigneeCB.Text
                    Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                    Act_AssignedAll.Text = Act_AssignedAll.Text.Remove(ifYesAgain, cbBox.Length)
                Else
                    cbBox = Act_AssigneeCB.Text & ", "

                    If txtBox.Length <> Act_AssigneeCB.Text.Length Then
                        cbBox = Act_AssignedAll.Text & ", "
                    Else
                        cbBox = Act_AssignedAll.Text
                    End If
                    Act_AssignedAll.Text = txtBox.Remove(ifYes, cbBox.Length)
                End If
            Else
                MsgBox("Entry already removed", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub
#End Region

End Class
