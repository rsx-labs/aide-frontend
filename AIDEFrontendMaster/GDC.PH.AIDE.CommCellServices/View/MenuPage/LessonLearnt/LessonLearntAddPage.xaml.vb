Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By John Harvey Sanchez / Marivic Espino
''' </summary>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class LessonLearntAddPage
    Implements IAideServiceCallback

#Region "Fields"
    Public frame As Frame
    Public mainWindow As MainWindow
    Public email As String
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private profile As Profile

    Dim lstActionList As New ObservableCollection(Of ActionModel)
    Dim lessonLearnt As New LessonLearnt
    Dim client As AideServiceClient
#End Region

#Region "Provider Declaration"
    Dim actionListProvider As New ActionListDBProvider
#End Region

#Region "View Model Declarations"
    Dim lessonLearntViewModel As New LessonLearntViewModel
    Dim actionViewModel As New ActionListViewModel
#End Region

#Region "Model Declarations"
    Dim actionModel As New ActionListViewModel
    Dim lessonLearntModel As New LessonLearntModel
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)
        InitializeComponent()

        frame = _frame
        email = _email
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        profile = _profile

        CreateReferenceNo()
        GetActionLists()
        SetDataContext()
        ConfigureButtons()
    End Sub

    Private Function InitializeService() As Boolean
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
#End Region

#Region "Private Functions"
    Private Sub CreateReferenceNo()
        Try
            If InitializeService() Then
                Dim refNo As String
                Dim dateNow As String = Date.Today.ToString("MM/dd/yy")
                Dim totalCount As Integer
                Dim lstLesson As LessonLearnt() = client.GetLessonLearntList(profile.Email_Address)

                totalCount = lstLesson.Length + 1

                If totalCount < 10 Then
                    refNo = "LL-" & dateNow & "-0" & totalCount
                Else
                    refNo = "LL-" & dateNow & "-" & totalCount
                End If

                lessonLearntModel.ReferenceNo = refNo
                lessonLearntModel.EmployeeID = profile.Emp_ID
            End If
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub GetActionLists()
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
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SetDataContext()
        lessonLearntViewModel.SelectedLessonLearnt = lessonLearntModel
        Me.DataContext = lessonLearntViewModel
    End Sub

    Private Sub GetDataContext(ByVal objLessonLearnt As LessonLearntViewModel)
        Try
            lessonLearnt.ReferenceNo = objLessonLearnt.SelectedLessonLearnt.ReferenceNo
            lessonLearnt.EmpID = objLessonLearnt.SelectedLessonLearnt.EmployeeID

            ' Check if the Problem Encountered Field has a value
            If Not IsNothing(objLessonLearnt.SelectedLessonLearnt.Problem) Then
                lessonLearnt.Problem = objLessonLearnt.SelectedLessonLearnt.Problem.Trim
            Else
                lessonLearnt.Problem = ""
            End If

            ' Check if the Resolution Field has a value
            If Not IsNothing(objLessonLearnt.SelectedLessonLearnt.Resolution) Then
                lessonLearnt.Resolution = objLessonLearnt.SelectedLessonLearnt.Resolution.Trim
            Else
                lessonLearnt.Resolution = ""
            End If

            If lvActionRef.ItemsSource IsNot Nothing Then
                Dim lstActionList As New ObservableCollection(Of ActionModel)
                lstActionList = lvActionRef.ItemsSource

                For Each objAction As ActionModel In lstActionList
                    lessonLearnt.ActionNo = objAction.REF_NO
                Next
            Else
                lessonLearnt.ActionNo = ""
            End If

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ConfigureButtons()
        If lstActionList.Count > 0 Then
            btnAddAction.IsEnabled = True
        Else
            btnAddAction.IsEnabled = False
        End If

        If lvActionRef.ItemsSource IsNot Nothing Then
            btnRemoveAction.IsEnabled = True
        Else
            btnRemoveAction.IsEnabled = False
        End If
    End Sub

    Private Sub ClearValues()
        lessonLearntModel.Problem = ""
        lessonLearntModel.Resolution = ""
        lvActionRef.ItemsSource = Nothing

        CreateReferenceNo()
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
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Get all the data in DataContext
            GetDataContext(Me.DataContext())

            If lessonLearnt.Problem.Trim = String.Empty Or lessonLearnt.Resolution.Trim = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Try
                    If Me.InitializeService Then
                        client.CreateLessonLearnt(lessonLearnt)
                        MsgBox("Lesson learned has been added.", MsgBoxStyle.Information, "AIDE")
                        ClearValues()
                        ExitPage()
                    End If
                Catch ex As Exception
                    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
                End Try
            End If
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnAddAction_Click(sender As Object, e As RoutedEventArgs) Handles btnAddAction.Click
        'INSERT SELECTED ACTION
        If lvAction.SelectedIndex = -1 Then
            MsgBox("Please select an action item.")
        Else
            Dim selectedAction As ActionModel = lvAction.SelectedValue
            Dim selectedActionList As New ObservableCollection(Of ActionModel)

            GetActionLists()

            For Each actionList In lstActionList
                If actionList.REF_NO = selectedAction.REF_NO Then
                    lstActionList.Remove(actionList)
                    Exit For
                End If
            Next

            selectedActionList.Add(selectedAction)
            lvActionRef.ItemsSource = selectedActionList

            ConfigureButtons()
        End If
    End Sub

    Private Sub btnRemoveAction_Click(sender As Object, e As RoutedEventArgs) Handles btnRemoveAction.Click
        GetActionLists()
        lvActionRef.ItemsSource = Nothing
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        ExitPage()
    End Sub

    Private Sub txtProblemEncountered_KeyDown(sender As Object, e As KeyEventArgs) Handles txtProblemEncountered.KeyDown
        Dim textRange As New TextRange(txtProblemEncountered.Document.ContentStart, txtProblemEncountered.Document.ContentEnd)
        If textRange.Text.Length >= 10 Then
            e.Handled = False
        End If
    End Sub

    Private Sub txtResolution_KeyDown(sender As Object, e As KeyEventArgs) Handles txtResolution.KeyDown
        Dim textRange As New TextRange(txtResolution.Document.ContentStart, txtResolution.Document.ContentEnd)
        If textRange.Text.Length >= 10 Then
            e.Handled = False
        End If
    End Sub

    'Private Sub Action_No_DropDownClosed(sender As Object, e As EventArgs) Handles Action_No.DropDownClosed
    '    'ActionDesc.Text = Action_No.SelectedValue
    'End Sub
#End Region

#Region "Common Methods"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
