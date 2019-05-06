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

    Public frame As Frame
    Public profile As Profile
    Public email As String
    Private _menugrid As Grid
    Private _addframe As Frame
    Private _submenuframe As Frame

#Region "Provider Declaration"
    Dim actionListProvider As New ActionListDBProvider
#End Region

#Region "View Model Declarations"
    Dim lessonLearntViewModel As New LessonLearntViewModel
    Dim actionViewModel As New ActionListViewModel
#End Region

#Region "Model Declarations"
    Dim actionModel As New ActionModel
    Dim lessonLearntModel As New LessonLearntModel
#End Region

    Dim lessonLearnt As New LessonLearnt
    Dim client As AideServiceClient

    Public Sub New(_frame As Frame, _lessonLearntModel As LessonLearntModel, _profile As Profile, _email As String, _menugrid As Grid, _submenuframe As Frame, _addframe As Frame)
        InitializeComponent()
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me._addframe = _addframe
        frame = _frame
        email = _email
        Me.profile = _profile
        lessonLearntModel = _lessonLearntModel

        GetActionReference()
        SetDataContext()
    End Sub

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

#Region "Private Methods"

    Private Sub GetActionReference()
        Try
            If Me.InitializeService() Then
                Dim lstAction As Action() = client.GetActionSummary(email)
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
            lessonLearnt.Problem = objects.lessonLearntViewModel.SelectedLessonLearnt.Problem.Trim
            lessonLearnt.Resolution = objects.lessonLearntViewModel.SelectedLessonLearnt.Resolution.Trim
            lessonLearnt.ActionNo = objects.lessonLearntViewModel.SelectedLessonLearnt.ActionNo

            If lessonLearnt.ActionNo = Nothing Then
                lessonLearnt.ActionNo = ""
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Get all the data in DataContext
            GetDataContext(Me.DataContext())

            If lessonLearnt.Problem.Trim = String.Empty Or lessonLearnt.Resolution.Trim = String.Empty Then
                MsgBox("Please Enter All Required Fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to Update?", MsgBoxStyle.YesNo, "AIDE")

                If result = vbYes Then
                    Try
                        If Me.InitializeService() Then
                            client.UpdateLessonLearntInfo(lessonLearnt)
                            MsgBox("Successfully Updated", MsgBoxStyle.Information, "AIDE")
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

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs) Handles btnBack.Click
        _addframe.Navigate(New LessonLearntPage(frame, email, _addframe, _menugrid, _submenuframe, profile))
        frame.IsEnabled = True
        frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

#End Region

End Class
