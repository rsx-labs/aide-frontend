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
    Private frame As Frame
    Private menugrid As Grid
    Private addframe As Frame
    Private submenuframe As Frame
    Private AIDEClientService As ServiceReference1.AideServiceClient
    Private concernRefID As String
    Private isSearchTextIsUsed As Integer = 0

    Private concernDBProvider As New ConcernDBProvider
    Private concernViewModel As New ConcernViewModel

    Private lstConcernActionList As New ObservableCollection(Of ConcernModel)
    Private lstReferenceActionList As New ObservableCollection(Of ConcernModel)

    Public Sub New(_concernViewModel As ConcernViewModel, _frame As Frame, _email As String, _menugrid As Grid, _submenuframe As Frame, _addframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        frame = _frame
        menugrid = _menugrid
        email = _email
        submenuframe = _submenuframe
        addframe = _addframe
        concernViewModel = _concernViewModel

        'Load Selected Concern from ThreeC Page
        LoadSelectedConcern()
        LoadActionList()
        LoadReferenceActionList()
        SetDataContext()
        ConfigureButtons()
    End Sub

#Region "Initialize Service"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            AIDEClientService = New AideServiceClient(Context)
            AIDEClientService.Open()
            bInitialize = True
        Catch ex As SystemException
            AIDEClientService.Abort()
        End Try
        Return bInitialize
    End Function
#End Region

#Region "Methods"
    'Set selected Concern from ThreeCPage
    Private Sub LoadSelectedConcern()
        Try
            If InitializeService() Then
                Dim concernData As New Concern

                concernData.RefID = concernViewModel.SelectedConcern.REF_ID
                concernData.Concerns = concernViewModel.SelectedConcern.CONCERN
                concernData.Cause = concernViewModel.SelectedConcern.CAUSE
                concernData.CounterMeasure = concernViewModel.SelectedConcern.COUNTERMEASURE
                concernData.Act_Reference = concernViewModel.SelectedConcern.ACT_REFERENCE
                concernData.Due_Date = concernViewModel.SelectedConcern.DUE_DATE

                concernDBProvider.SetConcernText(concernData)
                concernViewModel.SelectedConcern = New ConcernModel(concernDBProvider.GetSelectedConcern())

                concernRefID = concernData.RefID 'Set concern reference ID
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadActionList()
        Try
            If InitializeService() Then
                lstConcernActionList.Clear()

                Dim lstConcern As Concern()
                concernDBProvider = New ConcernDBProvider

                If isSearchTextIsUsed = 0 Then
                    'DISPLAY LIST OF ACTION
                    lstConcern = AIDEClientService.GetListOfACtion(concernRefID, email)
                Else
                    'DISPLAY LIST OF ACTION VIA SEARCH
                    lstConcern = AIDEClientService.GetSearchAction(concernRefID, txtSearchAction.Text, email)
                End If

                For Each objConcern As Concern In lstConcern
                    concernDBProvider.SetToComBoBox(objConcern)
                Next

                For Each iConcern As MyConcern In concernDBProvider.GetConcernActionList()
                    lstConcernActionList.Add(New ConcernModel(iConcern))
                Next

                lvACtion.ItemsSource = lstConcernActionList
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadReferenceActionList()
        Try
            If InitializeService() Then
                lstReferenceActionList.Clear()
                concernDBProvider = New ConcernDBProvider

                Dim lstConcern As Concern() = AIDEClientService.GetListOfACtionsReferences(concernRefID)

                For Each objConcern As Concern In lstConcern
                    concernDBProvider.SetTollistViewActionReference(objConcern)
                Next

                For Each iConcern As MyActionReference In concernDBProvider.GetConcernActionReferenceList()
                    lstReferenceActionList.Add(New ConcernModel(iConcern))
                Next

                lvActionRef.ItemsSource = lstReferenceActionList
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetDataContext()
        Me.DataContext = concernViewModel
    End Sub

    'Insert Action Reference to concern
    Public Function InsertSelectedAction()
        Dim selectedAction As ConcernModel = lvACtion.SelectedValue
        Dim addSelectedAction As New Concern

        addSelectedAction.RefID = concernRefID
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
        updatedSelectedConcern.Due_Date = _updatedSelectedConcern.SelectedConcern.DUE_DATE

        Return updatedSelectedConcern
    End Function

    'Delete Selected Action Ref in Concern
    Public Function DeleteSelectedActionReference()
        Dim selectedAction As ConcernModel = lvActionRef.SelectedValue
        Dim removeSelectedAction As New Concern

        removeSelectedAction.RefID = concernRefID
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
        frame.Navigate(New ThreeC_Page(email, frame, addframe, menugrid, submenuframe))
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
    Private Sub txtSearchAction_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearchAction.TextChanged
        isSearchTextIsUsed = 1
        LoadActionList()
    End Sub

    Private Sub btnUpdateClick(sender As Object, e As RoutedEventArgs)
        Try
            If InitializeService() Then
                AIDEClientService.UpdateSelectedConcern(UpdateSelectedConcern(Me.DataContext()))
                MsgBox("Successfully updated concern", MsgBoxStyle.Information)
                AIDEClientService.Close()

                ExitPage()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub btnSaveActionRef(sender As Object, e As RoutedEventArgs)
        InitializeService()
        Try
            If lvACtion.SelectedIndex = -1 Then
                MsgBox("Please select an item first.")
            Else
                AIDEClientService.insertAndDeleteSelectedAction(InsertSelectedAction())
                MsgBox("Successfully added new action reference in concern", MsgBoxStyle.Information)
                AIDEClientService.Close()

                LoadActionList()
                LoadReferenceActionList()
                ConfigureButtons()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub btnRemoveSelectedAction(sender As Object, e As RoutedEventArgs)
        Try
            If InitializeService() Then
                If lvActionRef.SelectedIndex = -1 Then
                    MsgBox("Please select an item first!", MsgBoxStyle.Exclamation, "AIDE")
                Else
                    If MsgBox("Are you sure you want to remove?", MsgBoxStyle.Question + vbYesNo, "AIDE") = vbYes Then
                        AIDEClientService.insertAndDeleteSelectedAction(DeleteSelectedActionReference())
                        MsgBox("Successfully remove action reference in concern", MsgBoxStyle.Information, "AIDE")
                        AIDEClientService.Close()

                        LoadActionList()
                        LoadReferenceActionList()
                        ConfigureButtons()
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
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
