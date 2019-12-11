Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By John Harvey Sanchez / Marivic Espino
''' </summary>
<CallbackBehavior(
ConcurrencyMode:=ConcurrencyMode.Single,
UseSynchronizationContext:=False)>
Class LessonLearntUpdatePage
    Implements IAideServiceCallback

#Region "Fields"
    Public frame As Frame
    Public profile As Profile
    Public email As String
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame

    Dim lstActionList As New ObservableCollection(Of ActionModel)
    Dim lstSelectedActionList As New ObservableCollection(Of ActionModel)
    Dim lessonLearnt As New LessonLearnt
    Dim client As AideServiceClient
#End Region

#Region "Provider Declaration"
    Dim actionListProvider As New ActionListDBProvider
#End Region

#Region "View Model Declarations"
    Dim lessonLearntViewModel As New LessonLearntViewModel
#End Region

#Region "Model Declarations"
    Dim actionModel As New ActionModel
    Dim lessonLearntModel As New LessonLearntModel
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _lessonLearntModel As LessonLearntModel, _profile As Profile, _email As String, _menugrid As Grid, _submenuframe As Frame, _addframe As Frame)
        InitializeComponent()
        menugrid = _menugrid
        submenuframe = _submenuframe
        addframe = _addframe
        frame = _frame
        email = _email
        Me.profile = _profile
        lessonLearntModel = _lessonLearntModel

        GetActionLists()
        GetReferenceActionList()
        SetDataContext()
        ConfigureButtons()
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function
#End Region

#Region "Private Methods"
    Private Sub GetActionLists()
        Try
            If InitializeService() Then
                lstActionList.Clear()
                actionListProvider = New ActionListDBProvider

                Dim lstAction As Action() = client.GetLessonLearntListOfActionSummary(profile.Emp_ID)

                For Each objAction As Action In lstAction
                    actionListProvider._setlistofitems(objAction)
                Next

                For Each iAction As myActionSet In actionListProvider._getobAction()
                    lstActionList.Add(New ActionModel(iAction))
                Next

                lvAction.ItemsSource = lstActionList
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub GetReferenceActionList()
        Try
            If InitializeService() Then
                lstSelectedActionList.Clear()
                actionListProvider = New ActionListDBProvider

                If lessonLearntModel.ActionNo IsNot String.Empty Then
                    Dim lstAction As Action() = client.GetActionListByActionNo(lessonLearntModel.ActionNo, profile.Emp_ID)

                    For Each objAction As Action In lstAction
                        actionListProvider._setlistofitems(objAction)
                    Next

                    For Each iAction As myActionSet In actionListProvider._getobAction()
                        lstSelectedActionList.Add(New ActionModel(iAction))
                    Next

                    lvActionRef.ItemsSource = lstSelectedActionList
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetDataContext()
        lessonLearntViewModel.SelectedLessonLearnt = lessonLearntModel
        Me.DataContext = lessonLearntViewModel
    End Sub

    Public Function UpdateSelectedConcern(getSelectedAction As ConcernViewModel)
        Dim _updateSelectedConcern As New Concern
        _updateSelectedConcern.Concerns = getSelectedAction.SelectedConcern.CONCERN
        _updateSelectedConcern.RefID = getSelectedAction.SelectedConcern.REF_ID
        _updateSelectedConcern.Cause = getSelectedAction.SelectedConcern.CAUSE
        _updateSelectedConcern.CounterMeasure = getSelectedAction.SelectedConcern.COUNTERMEASURE
        _updateSelectedConcern.Due_Date = getSelectedAction.SelectedConcern.DUE_DATE
        Return _updateSelectedConcern
    End Function

    Private Sub GetUpdatedData(objLessonLearnt As LessonLearntViewModel)
        Try
            Dim textProblem As New TextRange(txtProblemEncountered.Document.ContentStart, txtProblemEncountered.Document.ContentEnd)
            Dim textResolution As New TextRange(txtResolution.Document.ContentStart, txtResolution.Document.ContentEnd)

            lessonLearnt.ReferenceNo = objLessonLearnt.SelectedLessonLearnt.ReferenceNo
            lessonLearnt.Problem = textProblem.Text.Trim
            lessonLearnt.Resolution = textResolution.Text.Trim
            lessonLearnt.ActionNo = objLessonLearnt.SelectedLessonLearnt.ActionNo

            If lessonLearnt.ActionNo = Nothing Then
                lessonLearnt.ActionNo = ""
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub ConfigureButtons()
        If lstActionList.Count > 0 Then
            btnAddAction.IsEnabled = True
        Else
            btnAddAction.IsEnabled = False
        End If

        If lstSelectedActionList.Count > 0 Then
            btnRemoveAction.IsEnabled = True
        Else
            btnRemoveAction.IsEnabled = False
        End If
    End Sub

    Private Sub ExitPage()
        frame.Navigate(New LessonLearntPage(frame, email, addframe, menugrid, submenuframe, profile))
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "Events"
    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Get updated data
            GetUpdatedData(Me.DataContext)

            If lessonLearnt.Problem.Trim = String.Empty Or lessonLearnt.Resolution.Trim = String.Empty Then
                MsgBox("Please fill up all required fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to update?", MsgBoxStyle.YesNo, "AIDE")

                If result = vbYes Then
                    Try
                        If Me.InitializeService() Then
                            client.UpdateLessonLearntInfo(lessonLearnt)
                            MsgBox("Successfully updated", MsgBoxStyle.Information, "AIDE")
                            ExitPage()
                        End If
                    Catch ex As Exception
                        MsgBox(ex.Message, MsgBoxStyle.Critical, "AIDE")
                    End Try
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Failed")
        End Try
    End Sub

    Private Sub btnAddAction_Click(sender As Object, e As RoutedEventArgs) Handles btnAddAction.Click
        'INSERT SELECTED ACTION

        If lstSelectedActionList.Count > 0 Then
            MsgBox("Lesson learnt already reference to an action list", MsgBoxStyle.Information, "AIDE")
        Else
            If lvAction.SelectedIndex = -1 Then
                MsgBox("Please select an item first.")
            Else
                Try
                    If InitializeService() Then
                        GetUpdatedData(Me.DataContext())
                        Dim selectedAction As ActionModel = lvAction.SelectedValue

                        lessonLearnt.ActionNo = selectedAction.REF_NO
                        lessonLearntModel.ActionNo = lessonLearnt.ActionNo 'Reload Reference Action List

                        client.UpdateLessonLearntInfo(lessonLearnt)
                        MsgBox("Successfully added new action reference in lessons learnt", MsgBoxStyle.Information, "AIDE")

                        GetActionLists()
                        GetReferenceActionList()
                        ConfigureButtons()
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Critical, "Failed")
                End Try
            End If
        End If
    End Sub

    Private Sub btnRemoveAction_Click(sender As Object, e As RoutedEventArgs) Handles btnRemoveAction.Click
        If MsgBox("Are you sure you want to remove?", MsgBoxStyle.Question + vbYesNo, "AIDE") = vbYes Then
            Try
                InitializeService()
                GetUpdatedData(Me.DataContext())

                lessonLearnt.ActionNo = ""
                lessonLearntViewModel.SelectedLessonLearnt.ActionNo = "" 'Clear view model Action No

                client.UpdateLessonLearntInfo(lessonLearnt)
                MsgBox("Successfully remove action reference in lesson learnt", MsgBoxStyle.Information, "AIDE")
                client.Close()

                lstSelectedActionList.Clear()
                GetActionLists()
                ConfigureButtons()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Failed")
            End Try
        End If
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        ExitPage()
    End Sub

#End Region

#Region "Notify Methods"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
