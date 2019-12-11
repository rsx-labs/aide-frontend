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

    Private _email As String
    Private objData As New ConcernViewModel
    Private _frame As Frame
    Private _menugrid As Grid
    Private _addframe As Frame
    Private _submenuframe As Frame
    Private AIDEClientService As ServiceReference1.AideServiceClient
    Private getRefID As String
    Private isSearchTextIsUsed As Integer = 0

    Public Sub New(getPassedSelectedData As ConcernViewModel, _frame As Frame, email As String, _menugrid As Grid, _submenuframe As Frame, _addframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me._frame = _frame
        Me._menugrid = _menugrid
        Me._email = email
        Me._submenuframe = _submenuframe
        Me._addframe = _addframe
        Me.objData = getPassedSelectedData

        'Load Selected Concern from ThreeC Page
        LoadSelectedConcern()
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
        InitializeService()

        Dim _newProvider As New ConcernDBProvider
        Dim _newViewModel As New ConcernViewModel
        Dim _concern As New Concern

        _concern.RefID = objData.SelectedConcern.REF_ID
        _concern.Concerns = objData.SelectedConcern.CONCERN
        _concern.Cause = objData.SelectedConcern.CAUSE
        _concern.CounterMeasure = objData.SelectedConcern.COUNTERMEASURE
        _concern.Act_Reference = objData.SelectedConcern.ACT_REFERENCE
        _concern.Due_Date = objData.SelectedConcern.DUE_DATE

        _newProvider.SetConcernText(_concern)
        _newViewModel.SelectedConcern = New ConcernModel(_newProvider.GetSelectedConcern())

        If isSearchTextIsUsed = 0 Then
            ''LISTVIEW CHOICES OF ACTION TO ADD IN CONCERN BOX
            Dim lstConcern As Concern() = AIDEClientService.GetListOfACtion(_concern.RefID, _email)
            Dim lstConcernList As New ObservableCollection(Of ConcernModel)

            For Each objConcern As Concern In lstConcern
                _newProvider.SetToComBoBox(objConcern)
            Next

            For Each iConcern As MyConcern In _newProvider.GetConcernActionList()
                lstConcernList.Add(New ConcernModel(iConcern))
            Next
            _newViewModel.listAction = lstConcernList
        Else
            'DISPLAY LIST OF ACTION VIA SEARCH
            Dim lstConcern As Concern() = AIDEClientService.GetSearchAction(getRefID, txtSearchAction.Text, _email)
            Dim lstConcernList As New ObservableCollection(Of ConcernModel)

            For Each objConcern As Concern In lstConcern
                _newProvider.SetToComBoBox(objConcern)
            Next

            For Each iConcern As MyConcern In _newProvider.GetConcernActionList()
                lstConcernList.Add(New ConcernModel(iConcern))
            Next
            _newViewModel.listAction = lstConcernList
        End If

        ''DISPLAY MY ACTION REFERENCE in LISTVIEW
        getRefID = _concern.RefID

        Dim lstConcernAction As Concern() = AIDEClientService.GetListOfACtionsReferences(getRefID)
        Dim lstConcernListAction As New ObservableCollection(Of ConcernModel)

        For Each objConcern As Concern In lstConcernAction
            _newProvider.SetTollistViewActionReference(objConcern)
        Next

        For Each iConcern As MyActionReference In _newProvider.GetConcernActionReferenceList()
            lstConcernListAction.Add(New ConcernModel(iConcern))
        Next

        _newViewModel.ListOfActionInConcern = lstConcernListAction
        Me.DataContext = _newViewModel
    End Sub

    'INSERT TO CONCERN EACH SELECTED ACTION REF
    Public Function InsertSelectedAction(getSelectedAction As ConcernViewModel)
        Dim _insertSelectedAction As New Concern
        _insertSelectedAction.ACTREF = getSelectedAction.GetSelectedAction.ACTREF
        _insertSelectedAction.RefID = getSelectedAction.SelectedConcern.REF_ID
        _insertSelectedAction.ACT_MESSAGE = getSelectedAction.GetSelectedAction.ACT_MESSAGE
        Return _insertSelectedAction
    End Function

    'SET SELECTED CONCERN
    Public Function UpdateSelectedConcern(getSelectedAction As ConcernViewModel)
        Dim _updateSelectedConcern As New Concern
        _updateSelectedConcern.Concerns = getSelectedAction.SelectedConcern.CONCERN
        _updateSelectedConcern.RefID = getSelectedAction.SelectedConcern.REF_ID
        _updateSelectedConcern.Cause = getSelectedAction.SelectedConcern.CAUSE
        _updateSelectedConcern.CounterMeasure = getSelectedAction.SelectedConcern.COUNTERMEASURE
        _updateSelectedConcern.Due_Date = getSelectedAction.SelectedConcern.DUE_DATE
        Return _updateSelectedConcern
    End Function

    ' UPDATE CONCERN TO DATABASE
    Private Sub getSelectecDATE(_newViewModel As ConcernViewModel)
        Dim _newProvider As New ConcernDBProvider
        Dim _concern As New Concern
        Dim lstConcern As Concern()
        Dim lstConcernList As ObservableCollection(Of ConcernModel)
        Dim getRefID As String = _concern.RefID
        Dim lstConcernAction As Concern()
        Dim lstConcernListAction As ObservableCollection(Of ConcernModel)

        ''BINDING TO TEXT
        _concern.Due_Date = _newViewModel.SelectedConcern.DUE_DATE
        _concern.RefID = objData.SelectedConcern.REF_ID
        _concern.Concerns = objData.SelectedConcern.CONCERN
        _concern.Cause = objData.SelectedConcern.CAUSE
        _concern.CounterMeasure = objData.SelectedConcern.COUNTERMEASURE
        _concern.Act_Reference = objData.SelectedConcern.ACT_REFERENCE
        _newProvider.SetConcernText(_concern)
        _newViewModel.SelectedConcern = New ConcernModel(_newProvider.GetSelectedConcern())

        ''DISPLAY LIST OF ACTION
        lstConcern = AIDEClientService.GetListOfACtion(_concern.RefID, _email)
        lstConcernList = New ObservableCollection(Of ConcernModel)

        For Each objConcern As Concern In lstConcern
            _newProvider.SetToComBoBox(objConcern)
        Next

        For Each iConcern As MyConcern In _newProvider.GetConcernActionList()
            lstConcernList.Add(New ConcernModel(iConcern))
        Next

        _newViewModel.listAction = lstConcernList

        ''DISPLAY MY ACTION REFERENCE in LISTVIEW
        getRefID = _concern.RefID
        lstConcernAction = AIDEClientService.GetListOfACtionsReferences(getRefID)
        lstConcernListAction = New ObservableCollection(Of ConcernModel)

        For Each objConcern As Concern In lstConcernAction
            _newProvider.SetTollistViewActionReference(objConcern)
        Next

        For Each iConcern As MyActionReference In _newProvider.GetConcernActionReferenceList()
            lstConcernListAction.Add(New ConcernModel(iConcern))
        Next

        _newViewModel.ListOfActionInConcern = lstConcernListAction
        Me.DataContext = _newViewModel
    End Sub

    'Delete Selected Action Ref in Concern
    Public Function DeleteSectedActionReference(getSelectedAction As ConcernViewModel)
        Dim _insertSelectedAction As New Concern
        _insertSelectedAction.ACTREF = getSelectedAction.GetSelectedAction.ACTREF
        _insertSelectedAction.RefID = getSelectedAction.SelectedConcern.REF_ID
        _insertSelectedAction.ACT_MESSAGE = getSelectedAction.GetSelectedAction.ACTION_REFERENCES
        Return _insertSelectedAction
    End Function

    Private Sub ExitPage()
        _frame.Navigate(New ThreeC_Page(_email, _frame, _addframe, _menugrid, _submenuframe))
        _frame.IsEnabled = True
        _frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
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

#Region "Buttons/Text - Events"
    Private Sub btnRemoveSelectedAction(sender As Object, e As RoutedEventArgs)
        InitializeService()
        ''INSERT SELECTED ACTION
        If lv.SelectedIndex = -1 Then
            MsgBox("Please select an item first!", MsgBoxStyle.Exclamation, "AIDE")
        Else
            If MsgBox("Are you sure you want to remove?", MsgBoxStyle.Question + vbYesNo, "AIDE") = vbYes Then
                AIDEClientService.insertAndDeleteSelectedAction(DeleteSectedActionReference(Me.DataContext()))
                getSelectecDATE(Me.DataContext())
                MsgBox("Successfully remove action reference in concern", MsgBoxStyle.Information, "AIDE")
                AIDEClientService.Close()
                Return
            End If
        End If
    End Sub

    Private Sub txtSearchAction_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearchAction.TextChanged
        isSearchTextIsUsed = 1
        LoadSelectedConcern()
    End Sub

    Private Sub btnSaveClick(sender As Object, e As RoutedEventArgs)
        InitializeService()
        ''UPDATE SELECTED ACTION
        AIDEClientService.UpdateSelectedConcern(UpdateSelectedConcern(Me.DataContext()))
        getSelectecDATE(Me.DataContext())
        MsgBox("Successfully updated concern", MsgBoxStyle.Information)
        AIDEClientService.Close()

        ExitPage()
    End Sub

    Private Sub btnBackClick(sender As Object, e As RoutedEventArgs)
        ExitPage()
    End Sub

    Private Sub btnSaveActionRef(sender As Object, e As RoutedEventArgs)
        InitializeService()
        ''INSERT SELECTED ACTION
        If lvACtion.SelectedIndex = -1 Then
            MsgBox("Please select an item first.")
        Else
            AIDEClientService.insertAndDeleteSelectedAction(InsertSelectedAction(Me.DataContext()))
            getSelectecDATE(Me.DataContext())
            MsgBox("Successfully added new action reference in concern", MsgBoxStyle.Information)
            AIDEClientService.Close()
        End If
    End Sub
#End Region

End Class
