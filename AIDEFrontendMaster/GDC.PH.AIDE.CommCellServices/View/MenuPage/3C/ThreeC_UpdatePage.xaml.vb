Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports System.Reflection
Imports System.IO
Imports System.Data

''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO / JHUNELL BARCENAS
''' </summary>
''' <remarks></remarks>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single,UseSynchronizationContext:=False)>
Class ThreeC_UpdatePage
    Implements ServiceReference1.IAideServiceCallback

    Private email As String
    Private profile As Profile
    Private frame As Frame
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    'Private AIDEClientService As ServiceReference1.AideServiceClient
    Private concernRefID As String
    Private isSearchTextIsUsed As Integer = 0

    Private concernDBProvider As New ConcernDBProvider
    Private concernViewModel As New ConcernViewModel
    Private concernModel As New ConcernModel

    Private lstConcernActionList As New ObservableCollection(Of ConcernModel)
    Private lstReferenceActionList As New ObservableCollection(Of ConcernModel)

    Public Sub New(_frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _concernModel As ConcernModel, _profile As Profile)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        frame = _frame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        concernModel = _concernModel
        email = _profile.Email_Address
        profile = _profile

        GetActionList()
        GetReferenceActionList()
        SetDataContext()
        ConfigureButtons()
    End Sub

#Region "Initialize Service"
    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        AIDEClientService = New AideServiceClient(Context)
    '        AIDEClientService.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        AIDEClientService.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function
#End Region

#Region "Methods"
    Private Sub GetActionList()
        Try
            'If InitializeService() Then
            lstConcernActionList.Clear()
            concernDBProvider = New ConcernDBProvider

            Dim lstConcern As Concern() = AideClient.GetClient().GetListOfACtion(concernModel.REF_ID, email)

            For Each objConcern As Concern In lstConcern
                    concernDBProvider.SetToComBoBox(objConcern)
                Next

                For Each iConcern As MyConcern In concernDBProvider.GetConcernActionList()
                    lstConcernActionList.Add(New ConcernModel(iConcern))
                Next

                lvAction.ItemsSource = lstConcernActionList
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub GetReferenceActionList()
        Try
            'If InitializeService() Then
            lstReferenceActionList.Clear()
            concernDBProvider = New ConcernDBProvider

            Dim lstConcern As Concern() = AideClient.GetClient().GetListOfACtionsReferences(concernModel.REF_ID)

            For Each objConcern As Concern In lstConcern
                    concernDBProvider.SetTollistViewActionReference(objConcern)
                Next

                For Each iConcern As MyActionReference In concernDBProvider.GetConcernActionReferenceList()
                    lstReferenceActionList.Add(New ConcernModel(iConcern))
                Next

                lvActionRef.ItemsSource = lstReferenceActionList
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SetDataContext()
        concernViewModel.SelectedConcern = concernModel
        Me.DataContext = concernViewModel
    End Sub

    'Insert Action Reference to concern
    Public Function InsertSelectedAction()
        Dim selectedAction As ConcernModel = lvAction.SelectedValue
        Dim addSelectedAction As New Concern

        addSelectedAction.RefID = concernModel.REF_ID
        addSelectedAction.ACTREF = selectedAction.ACTREF
        addSelectedAction.ACT_MESSAGE = selectedAction.ACT_MESSAGE

        Return addSelectedAction
    End Function

    'Update concern
    Public Function UpdateSelectedConcern(_updatedSelectedConcern As ConcernViewModel)
        Dim updatedSelectedConcern As New Concern

        updatedSelectedConcern.RefID = _updatedSelectedConcern.SelectedConcern.REF_ID
        updatedSelectedConcern.Concerns = _updatedSelectedConcern.SelectedConcern.CONCERN
        updatedSelectedConcern.Cause = _updatedSelectedConcern.SelectedConcern.CAUSE
        updatedSelectedConcern.CounterMeasure = _updatedSelectedConcern.SelectedConcern.COUNTERMEASURE
        If dtDate.SelectedDate.HasValue Then
            updatedSelectedConcern.Due_Date = dtDate.SelectedDate.Value
        Else
            updatedSelectedConcern.Due_Date = _updatedSelectedConcern.SelectedConcern.DUE_DATE
        End If


        Return updatedSelectedConcern
    End Function

    'Delete Selected Action Ref in Concern
    Public Function DeleteSelectedActionReference()
        Dim selectedAction As ConcernModel = lvActionRef.SelectedValue
        Dim removeSelectedAction As New Concern

        removeSelectedAction.RefID = concernModel.REF_ID
        removeSelectedAction.ACTREF = selectedAction.ACTREF
        removeSelectedAction.ACT_MESSAGE = selectedAction.ACT_MESSAGE

        Return removeSelectedAction
    End Function

    Private Sub ConfigureButtons()
        If lstConcernActionList.Count > 0 Then
            btnAddAction.IsEnabled = True
        Else
            btnAddAction.IsEnabled = False
        End If

        If lstReferenceActionList.Count > 0 Then
            btnRemoveAction.IsEnabled = True
        Else
            btnRemoveAction.IsEnabled = False
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

#Region "Events"
    Private Sub btnUpdateClick(sender As Object, e As RoutedEventArgs)
        Try
            ' If InitializeService() Then
            AideClient.GetClient().UpdateSelectedConcern(UpdateSelectedConcern(Me.DataContext()))
            MsgBox("3C has been updated.", MsgBoxStyle.Information, "AIDE")
            ExitPage()
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnSaveActionRef(sender As Object, e As RoutedEventArgs)
        'InitializeService()
        Try
            If lvAction.SelectedIndex = -1 Then
                MsgBox("Please select an action item.")
            Else
                AideClient.GetClient().insertAndDeleteSelectedAction(InsertSelectedAction())
                MsgBox("Action reference has been added to 3C.", MsgBoxStyle.Information, "AIDE")
                'AIDEClientService.Close()

                GetActionList()
                GetReferenceActionList()
                ConfigureButtons()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnRemoveSelectedAction(sender As Object, e As RoutedEventArgs)
        Try
            'If InitializeService() Then
            If lvActionRef.SelectedIndex = -1 Then
                MsgBox("Please select an item first!", MsgBoxStyle.Exclamation, "AIDE")
            Else
                If MsgBox("Are you sure you want to remove action from 3C?", MsgBoxStyle.Question + vbYesNo, "AIDE") = vbYes Then
                    AideClient.GetClient().insertAndDeleteSelectedAction(DeleteSelectedActionReference())
                    MsgBox("Action has been removed from the 3C.", MsgBoxStyle.Information, "AIDE")
                    'AIDEClientService.Close()

                    GetActionList()
                    GetReferenceActionList()
                    ConfigureButtons()
                End If
            End If
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnBackClick(sender As Object, e As RoutedEventArgs)
        ExitPage()
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

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
