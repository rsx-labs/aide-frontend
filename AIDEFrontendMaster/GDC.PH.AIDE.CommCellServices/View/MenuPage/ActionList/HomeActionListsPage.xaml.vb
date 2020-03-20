Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.IO
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

''' <summary>
''' By Jester Sanchez/ Lemuela Abulencia
''' </summary>
''' <remarks></remarks>
Class HomeActionListsPage
    Implements IAideServiceCallback

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private email As String
    Private profile As Profile
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame

    Private aide As ServiceReference1.AideServiceClient
    Private lstAction As Action()
    Private actionListDBProvider As New ActionListDBProvider

    Private _OptionsViewModel As OptionViewModel
    Dim guestAccount As Integer = 5
    Dim paginatedCollection As PaginatedObservableCollection(Of ActionModel) = New PaginatedObservableCollection(Of ActionModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)
        frame = _frame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        profile = _profile
        InitializeComponent()

        pagingRecordPerPage = GetOptionData(24, 9, 12)
        paginatedCollection = New PaginatedObservableCollection(Of ActionModel)(pagingRecordPerPage)

        LoadActionList()
        PermissionSettings()
    End Sub
#End Region

#Region "Methods"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aide = New AideServiceClient(Context)
            aide.Open()
            bInitialize = True
        Catch ex As SystemException
            aide.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Private Sub LoadActionList()
        Try
            If InitializeService() Then
                lstAction = aide.GetActionSummary(profile.Email_Address)
                SetLists(lstAction)
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
        Dim strData As String = String.Empty
        Try
            _OptionsViewModel = New OptionViewModel
            If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If Not opt Is Nothing Then
                        strData = opt.VALUE
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return strData
    End Function

    Private Sub SetLists(listAction As Action())
        Try
            actionListDBProvider = New ActionListDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of ActionModel)(pagingRecordPerPage)

            For Each objActionList As Action In listAction
                actionListDBProvider._setlistofitems(objActionList)
            Next

            For Each actionsList As MyActionSet In actionListDBProvider._getobAction()
                paginatedCollection.Add(New ActionModel(actionsList))
            Next

            ActionLV.ItemsSource = paginatedCollection

            totalRecords = listAction.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SearchAction(ByVal search As String)
        Try
            Dim items = From i In lstAction Where i.Act_Message.ToLower.Contains(search.ToLower) _
                                            Or i.Act_NickName.ToLower.Contains(search.ToLower) _
                                            Or i.Act_ID.ToLower.Contains(search.ToLower)

            Dim searchAction = New ObservableCollection(Of Action)(items)

            SetLists(searchAction.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If totalRecords = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        ActionLV.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        ActionLV.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub PermissionSettings()
        If profile.Permission_ID = guestAccount Then
            btnCreate.Visibility = Windows.Visibility.Hidden
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub ActionLV_MouseDoubleClick(sender As Object, e As MouseEventArgs)
        Try
            If Not ActionLV.SelectedIndex = -1 Then
                If Not profile.Permission_ID = guestAccount Then
                    Dim selectedAction As New Action

                    selectedAction.Act_ID = CType(ActionLV.SelectedItem, ActionModel).REF_NO
                    selectedAction.Act_Message = CType(ActionLV.SelectedItem, ActionModel).ACTION_MESSAGE
                    selectedAction.Act_NickName = CType(ActionLV.SelectedItem, ActionModel).NICK_NAME
                    selectedAction.Act_DueDate = CType(ActionLV.SelectedItem, ActionModel).DUE_DATE
                    selectedAction.Act_DateClosed = CType(ActionLV.SelectedItem, ActionModel).DATE_CLOSED

                    If Not selectedAction.Act_DateClosed = String.Empty Then
                        MsgBox("Selected action list has already been closed. Please select open action list.", vbOKOnly + vbInformation, "Action List")
                    Else
                        addframe.Navigate(New UpdateActionListPage(frame, addframe, menugrid, submenuframe, selectedAction, profile))
                        frame.IsEnabled = False
                        frame.Opacity = 0.3
                        menugrid.IsEnabled = False
                        menugrid.Opacity = 0.3
                        submenuframe.IsEnabled = False
                        submenuframe.Opacity = 0.3
                        addframe.Margin = New Thickness(200, 30, 200, 30)
                        addframe.Visibility = Visibility.Visible
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        SearchAction(txtSearch.Text.Trim)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            ActionLV.Measure(pageSize)
            ActionLV.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(ActionLV, "Print Action Lists")
        End If
    End Sub

    Private Sub AddActionListBtn_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New InsertActionListPage(frame, addframe, menugrid, submenuframe, profile))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(200, 50, 200, 50)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub ActionLV_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles ActionLV.LoadingRow
        Dim RowDataContaxt As ActionModel = TryCast(e.Row.DataContext, ActionModel)
        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.DUE_DATE = DateTime.Now.ToString("yyyy-MM-dd") And RowDataContaxt.DATE_CLOSED = String.Empty Then
                e.Row.Background = New BrushConverter().ConvertFrom("#FFFDECCE")
            ElseIf DateTime.Now.ToString("yyyy-MM-dd") > RowDataContaxt.DUE_DATE And RowDataContaxt.DATE_CLOSED = String.Empty Then
                e.Row.Background = New BrushConverter().ConvertFrom("#CCFFD8D8")
            End If
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If

        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If

        DisplayPagingInfo()
    End Sub
#End Region

#Region "Callback"
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
