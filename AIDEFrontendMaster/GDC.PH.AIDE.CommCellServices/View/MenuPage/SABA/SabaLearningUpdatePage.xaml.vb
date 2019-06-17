Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class SabaLearningUpdatePage
    Implements ServiceReference1.IAideServiceCallback

#Region "Page Declaration"
    Public _frame As Frame
    Private aide As AideServiceClient
    Private sabalearning As New SabaLearning
    Private _sabaModel As New SabaLearningModel()
    Private _email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _empID As Integer
    Private _saba_id As Integer
    Private ss As String
    Private profile As Profile
#End Region

    Public Sub New(mainframe As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame, sabamodel As SabaLearningModel, _profile As Profile)
        Try
            Me._frame = mainframe
            Me._addframe = addframe
            Me._menugrid = menugrid
            Me._submenuframe = submenuframe
            Me._sabaModel = sabamodel
            Me.profile = _profile
            InitializeComponent()
            Me.DataContext = Me._sabaModel
            SetDueDate()

        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

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

    Public Sub SetDueDate()
        Me.dtDate.Text = _sabaModel.END_DATE
    End Sub

    Public Function getDataInsert(ByVal SabaModel As SabaLearningModel)
        Try
            InitializeService()
            If SabaModel.TITLE = Nothing Or SabaModel.END_DATE = Nothing Then
            Else
                sabalearning.SABA_ID = SabaModel.SABA_ID
                sabalearning.TITLE = SabaModel.TITLE
                sabalearning.END_DATE = SabaModel.END_DATE
                sabalearning.EMP_ID = SabaModel.EMP_ID
            End If
            Return sabalearning
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
            Return ex
        End Try
    End Function


    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        _frame.Navigate(New SabaLearningMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
        _frame.IsEnabled = True
        _frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

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

    Private Sub UpdateBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()

            aide.UpdateSabaCourses(getDataInsert(Me.DataContext()))
            If sabalearning.TITLE = Nothing Or sabalearning.END_DATE = Nothing Then
                MsgBox("Please Fill Up All Fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                MsgBox("Successfully Updated!", vbOKOnly + MsgBoxStyle.Information, "AIDE")

                sabalearning.TITLE = Nothing
                sabalearning.END_DATE = Nothing
                sabalearning.EMP_ID = Nothing
                sabalearning.SABA_ID = Nothing

                _frame.Navigate(New SabaLearningMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
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
End Class
