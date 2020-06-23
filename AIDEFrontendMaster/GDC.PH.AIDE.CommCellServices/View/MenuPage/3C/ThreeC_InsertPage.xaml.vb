Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.ServiceModel

''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Class ThreeC_InsertPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"
    'Public _AIDEClientService As ServiceReference1.AideServiceClient

    Private profile As Profile
    Private email As String
    Private objConcern As New Concern
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame

    Dim concern As New Concern
#End Region
   
    Public Sub New(_profile As Profile, _frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()

        email = _profile.Email_Address
        profile = _profile
        frame = _frame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        setDate()

        GetGeneRatedRefNo()
    End Sub

#Region "Initialize Service"
    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        'DisplayText("Opening client service...")
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        _AIDEClientService = New AideServiceClient(Context)
    '        _AIDEClientService.Open()
    '        bInitialize = True
    '        'DisplayText("Service opened successfully...")
    '        'Return True
    '    Catch ex As SystemException
    '        _AIDEClientService.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function
#End Region

#Region "Methods"
    'Get Generated RefNo
    Public Function GetGeneRatedRefNo() As Boolean
        'InitializeService()
        Try
            'DisplayText("Signing on as: " & Globals.OutlookAddIn.EmailAddress)
            Dim objConcern As Object = Nothing
            Dim _concern As ServiceReference1.Concern = AideClient.GetClient().GetGeneratedRefNo()
            SavegeneratedRefNo(_concern)
            Return True
        Catch ex As SystemException
            '_AIDEClientService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return False
        End Try
    End Function

    'Display generated RefNo
    Public Sub SavegeneratedRefNo(ByVal _concern As Concern)
        Try
            'DisplayText("Saving RefNo...")
            If Not IsNothing(_concern) Then
                Dim _ConcernDBProvider = New ConcernDBProvider
                Dim _ConcernViewModel = New ConcernViewModel

                _ConcernDBProvider.SetToMyRefNo(_concern)

                _ConcernViewModel.GeneratedRefNo = New ConcernModel(_ConcernDBProvider.GetRefNo())
                If dtDate.SelectedDate.HasValue Then
                    _ConcernViewModel.SelectedConcern.DUE_DATE = dtDate.SelectedDate.Value
                Else
                    _ConcernViewModel.SelectedConcern.DUE_DATE = DateTime.Now
                End If
                Me.DataContext = _ConcernViewModel
            End If
        Catch ex As SystemException
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Function ValidateFields(ByVal obj As ConcernViewModel)
        If obj.SelectedConcern.CONCERN = "" Or obj.SelectedConcern.CAUSE = "" Or obj.SelectedConcern.COUNTERMEASURE = "" Then
            MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Return False
        ElseIf dtDate.SelectedDate.ToString() = String.Empty Then
            MsgBox("Please enter a due date.", MsgBoxStyle.Exclamation, "AIDE")
            Return False
        Else
            concern.Cause = obj.SelectedConcern.CAUSE
            concern.Concerns = obj.SelectedConcern.CONCERN
            concern.CounterMeasure = obj.SelectedConcern.COUNTERMEASURE
            concern.Due_Date = dtDate.SelectedDate.Value
            Return True
        End If
    End Function

    'Set DateTime Now in DatePicker
    Private Sub setDate()
        Dim ob As New ConcernViewModel

        If dtDate.SelectedDate.HasValue Then
            ob.SelectedConcern.DUE_DATE = dtDate.SelectedDate.Value
        Else
            ob.SelectedConcern.DUE_DATE = DateTime.Now
        End If


    End Sub

    Private Sub ExitPage()
        frame.Navigate(New ThreeC_Page(profile, frame, addframe, menugrid, submenuframe))
        frame.IsEnabled = True
        frame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "Notify Changes"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError

    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        MsgBox("Success", MsgBoxStyle.Information)
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub

#End Region

#Region "Buttons/Events"
    Private Sub btnBackClick(sender As Object, e As RoutedEventArgs)
        If txtConcern.Text <> "" OrElse txtCAUSE.Text.ToString <> "" OrElse txtCounterMeasure.Text.ToString <> "" Then
            ExitPage()
        Else
            ExitPage()
        End If
    End Sub

    Private Sub btnCreate3C(sender As Object, e As RoutedEventArgs)
        'InitializeService()
        Dim isValidate As Boolean
        isValidate = ValidateFields(Me.DataContext())

        If isValidate Then
            AideClient.GetClient().InsertIntoConcern(concern, email)
            MsgBox("3C has been added.", MsgBoxStyle.Information, "AIDE")

            txtRefNo.Text = String.Empty
            txtConcern.Clear()
            txtCAUSE.Clear()
            txtCounterMeasure.Clear()
            GetGeneRatedRefNo()
            setDate()
            '_AIDEClientService.Close()
            ExitPage()
        End If
    End Sub

#End Region

End Class
