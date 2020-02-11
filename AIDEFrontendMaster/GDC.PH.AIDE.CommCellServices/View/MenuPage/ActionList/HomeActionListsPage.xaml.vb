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
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Fields"
    Private _frame As Frame
    Private _email As String

    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private aide As ServiceReference1.AideServiceClient
    Private _ActionViewModel As New ActionListViewModel
    Private action_provider As New ActionListDBProvider
    Private EnableRowHeaderDoubleClick As Boolean = False
    Private lstAction As Action()
    Private profiles As Profile

    Dim paginatedCollection As PaginatedObservableCollection(Of ActionModel) = New PaginatedObservableCollection(Of ActionModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _prof As Profile)
        Try
            Me._email = email
            Me._frame = _frame
            Me._addframe = _addframe
            Me._menugrid = _menugrid
            Me._submenuframe = _submenuframe
            Me.profiles = _prof
            InitializeComponent()
            LoadActionList(_email)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

#Region "Main Function/ Method"

    'Load all action list from service data contract to datagrid
    Private Sub LoadActionList(ByVal email As String)
        Try
            If Me.InitializeService Then
                lstAction = aide.GetActionSummary(email)
                SetLists()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadActionByMessage(ByVal _message As String, ByVal email As String)
        Try
            If Me.InitializeService Then
                lstAction = aide.GetActionListByMessage(_message, email)
                SetLists()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#End Region

#Region "Paging Function/Method"

    Private Sub SetLists()
        Try
            paginatedCollection.Clear()
            Dim lstactionlist As New ObservableCollection(Of ActionModel)
            Dim lstactionprovider As New ActionListDBProvider

            For Each iactionlist As myActionSet In lstactionprovider._getobAction()
                lstactionlist.Add(New ActionModel(iactionlist))
            Next

            For Each objAct As Action In lstAction
                lstactionprovider._setlistofitems(objAct)
            Next

            For Each actions As myActionSet In lstactionprovider._getobAction()
                paginatedCollection.Add(New ActionModel(actions))
            Next

            ActionLV.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstAction.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstAction.Length

            Select Case mode
                Case CInt(PagingMode._Next)
                    ' Set the rows to be displayed if the total records is more than the (Record per Page * Page Index)
                    If totalRecords > (pagingPageIndex * pagingRecordPerPage) Then

                        ' Set the last row to be displayed if the total records is more than the (Record per Page * Page Index) + Record per Page
                        If totalRecords >= ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) Then
                            lastRowIndex = ((pagingPageIndex * pagingRecordPerPage) + pagingRecordPerPage) - 1
                        Else
                            lastRowIndex = totalRecords - 1
                        End If

                        startRowIndex = pagingPageIndex * pagingRecordPerPage
                        pagingPageIndex += 1
                    Else
                        startRowIndex = (pagingPageIndex - 1) * pagingRecordPerPage
                        lastRowIndex = totalRecords - 1
                    End If
                    ' Bind data to the Data Grid
                    SetLists()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        SetLists()
                    End If
                    Exit Select
                Case CInt(PagingMode._First)
                    If totalRecords > pagingRecordPerPage Then
                        pagingPageIndex = 2
                        SetPaging(CInt(PagingMode._Previous))
                    Else
                        pagingPageIndex = 1
                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)

                        If Not totalRecords = 0 Then
                            lastRowIndex = totalRecords - 1
                            SetLists()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstAction.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstAction.Length = 0 Then
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
#End Region

#Region "Services Function/Method"
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

#Region "Events Trigger"
    Private Sub ActionLV_MouseDoubleClick(sender As Object, e As MouseEventArgs)
        Try
            If ActionLV.SelectedIndex = -1 Then
            Else
                InitializeService()
                Dim _SelectedAction As New Action

                _SelectedAction.Act_ID = CType(ActionLV.SelectedItem, ActionModel).REF_NO
                _SelectedAction.Act_Message = CType(ActionLV.SelectedItem, ActionModel).ACTION_MESSAGE
                _SelectedAction.Act_NickName = CType(ActionLV.SelectedItem, ActionModel).NICK_NAME
                _SelectedAction.Act_DueDate = CType(ActionLV.SelectedItem, ActionModel).DUE_DATE
                _SelectedAction.Act_DateClosed = CType(ActionLV.SelectedItem, ActionModel).DATE_CLOSED
                If Not _SelectedAction.Act_DateClosed = String.Empty Then
                    MsgBox("Selected action list has already been closed. Please select open action list.", vbOKOnly + vbInformation, "Action List")
                Else
                    _addframe.Navigate(New UpdateActionListPage(_frame, _SelectedAction, _email, _menugrid, _submenuframe, _addframe, Me.profiles))
                    _frame.IsEnabled = False
                    _frame.Opacity = 0.3
                    _menugrid.IsEnabled = False
                    _menugrid.Opacity = 0.3
                    _submenuframe.IsEnabled = False
                    _submenuframe.Opacity = 0.3
                    _addframe.Margin = New Thickness(200, 30, 200, 30)
                    _addframe.Visibility = Visibility.Visible
                End If

            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        If SearchTextBox.Text = String.Empty Then
            LoadActionList(_email)
        Else
            LoadActionByMessage(SearchTextBox.Text, _email)
        End If
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
        _addframe.Navigate(New InsertActionListPage(_frame, _email, _addframe, _menugrid, _submenuframe, Me.profiles))
        _frame.IsEnabled = False
        _frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(200, 50, 200, 50)
        _addframe.Visibility = Visibility.Visible
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
        Dim totalRecords As Integer = lstAction.Length

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

    Private Sub btnFirst_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._First))
    End Sub

    Private Sub btnLast_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Last))
    End Sub

#End Region

End Class
