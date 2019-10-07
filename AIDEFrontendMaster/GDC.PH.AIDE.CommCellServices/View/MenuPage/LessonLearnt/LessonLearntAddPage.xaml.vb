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
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile

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
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.profile = _profile

        CreateReferenceNo()
        GetActionReference()
        SetDataContext()
    End Sub
#End Region

#Region "Common Methods"
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

#Region "Private Functions"
    Private Sub CreateReferenceNo()
        Try
            If Me.InitializeService() Then
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
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub GetActionReference()
        Try
            If Me.InitializeService() Then
                Dim lstAction As Action() = client.GetActionSummary(profile.Email_Address)
                Dim lstActionList As New ObservableCollection(Of ActionModel)

                For Each objAction As Action In lstAction
                    actionListProvider._setlistofitems(objAction)
                Next

                For Each iAction As myActionSet In actionListProvider._getobAction()
                    lstActionList.Add(New ActionModel(iAction))
                Next

                actionViewModel.objectActionSet = lstActionList
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetDataContext()
        lessonLearntViewModel.SelectedLessonLearnt = lessonLearntModel
        Me.DataContext = New With {actionViewModel, lessonLearntViewModel}
    End Sub

    Private Sub GetDataContext(ByVal objects As Object)
        Try
            lessonLearnt.ReferenceNo = objects.lessonLearntViewModel.SelectedLessonLearnt.ReferenceNo
            lessonLearnt.EmpID = objects.lessonLearntViewModel.SelectedLessonLearnt.EmployeeID

            ' Check if the Problem Encountered Field has a value
            If Not IsNothing(objects.lessonLearntViewModel.SelectedLessonLearnt.Problem) Then
                lessonLearnt.Problem = objects.lessonLearntViewModel.SelectedLessonLearnt.Problem.Trim
            Else
                lessonLearnt.Problem = ""
            End If

            ' Check if the Resolution Field has a value
            If Not IsNothing(objects.lessonLearntViewModel.SelectedLessonLearnt.Resolution) Then
                lessonLearnt.Resolution = objects.lessonLearntViewModel.SelectedLessonLearnt.Resolution.Trim
            Else
                lessonLearnt.Resolution = ""
            End If

            ' Check if the Action Number has a value
            If Not IsNothing(objects.lessonLearntViewModel.SelectedLessonLearnt.ActionNo) Then
                lessonLearnt.ActionNo = objects.lessonLearntViewModel.SelectedLessonLearnt.ActionNo
            Else
                lessonLearnt.ActionNo = ""
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub ClearValues()
        lessonLearntModel.Problem = ""
        lessonLearntModel.Resolution = ""
        lessonLearntModel.ActionNo = ""

        CreateReferenceNo()
    End Sub
#End Region

#Region "Events"
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Get all the data in DataContext
            GetDataContext(Me.DataContext())

            If lessonLearnt.Problem.Trim = String.Empty Or lessonLearnt.Resolution.Trim = String.Empty Then
                MsgBox("Please fill up all required fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to save?", MsgBoxStyle.YesNo, "AIDE")

                If result = vbYes Then
                    Try
                        If Me.InitializeService Then
                            client.CreateLessonLearnt(lessonLearnt)
                            MsgBox("Successfully created", MsgBoxStyle.Information, "AIDE")
                            ClearValues()
                            frame.Navigate(New LessonLearntPage(frame, email, _addframe, _menugrid, _submenuframe, profile))
                            frame.IsEnabled = True
                            frame.Opacity = 1
                            _menugrid.IsEnabled = True
                            _menugrid.Opacity = 1
                            _submenuframe.IsEnabled = True
                            _submenuframe.Opacity = 1
                            _addframe.Visibility = Visibility.Hidden
                        End If
                    Catch ex As Exception
                        MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
                    End Try
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        frame.Navigate(New LessonLearntPage(frame, email, _addframe, _menugrid, _submenuframe, profile))
        frame.IsEnabled = True
        frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub Action_No_DropDownClosed(sender As Object, e As EventArgs) Handles Action_No.DropDownClosed
        ActionDesc.Text = Action_No.SelectedValue
    End Sub
#End Region

End Class
