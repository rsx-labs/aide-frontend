Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.Windows
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Data
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class ThreeC_Page
    Implements ServiceReference1.IAideServiceCallback

    Public _AIDEClientService As ServiceReference1.AideServiceClient
    Private email As String
    Private _frame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame

    Private max As Integer
    Private incVal As Integer = 0
    Private isSearchIsUsed As Integer = 0
    Private isDateBetweenUsed As Integer = 0

    Private offsetVal As Integer = 0
    Private nextVal As Integer = 100

    Private startRowIndex As Integer
    Private lastRowIndex As Integer
    Private pagingPageIndex As Integer
    Private pagingRecordPerPage As Integer = 10
    Private currentPage As Integer
    Private lastPage As Integer
    Private _lstConcern As Concern()

    Dim paginatedCollection As PaginatedObservableCollection(Of ConcernModel) = New PaginatedObservableCollection(Of ConcernModel)(pagingRecordPerPage)

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Public Sub New(email As String, _frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        Dim clear As New ConcernViewModel

        InitializeComponent()
        Me.InitializeService()
        Me.email = email
        Me._frame = _frame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe

        LoadConcernList(offsetVal, nextVal)
    End Sub

#Region "Methods"
    ''DISPLAY to DATAGIRD VIEW
    Public Sub LoadConcernList(offSet As Integer, NextVal As Integer)
        Try
            If InitializeService() Then
                _lstConcern = _AIDEClientService.selectAllConcern(email, offSet, NextVal)
                SetLists()
                DisplayPagingInfo()
            End If
            
        Catch ex As SystemException
            MsgBox(ex.Message)
            _AIDEClientService.Abort()
        End Try
    End Sub

    ''resultSearch
    Private Sub retrieveSearch(offSet As Integer, NextVal As Integer)
        Try
            If InitializeService() Then
                _lstConcern = _AIDEClientService.GetResultSearch(email, txtSearch.Text, offSet, NextVal)
                SetLists()
                DisplayPagingInfo()
            End If
        Catch ex As SystemException
            MsgBox(ex.Message)
            _AIDEClientService.Abort()
        End Try
    End Sub

    ''DISPLAY to DATAGIRD VIEW WITH DATE SEARCH
    Public Sub LoadBetweenSearchDate(offSet As Integer, NextVal As Integer, _concerngetDate As ConcernViewModel)
        Try
            If InitializeService() Then
                _lstConcern = _AIDEClientService.GetBetweenSearchConcern(email, offSet, NextVal, dtpFrom.SelectedDate, dtpTo.SelectedDate)
                SetLists()
                DisplayPagingInfo()
            End If
        Catch ex As SystemException
            MsgBox(ex.Message)
            _AIDEClientService.Abort()
        End Try
    End Sub

    Private Function GetDateTimeNow(_setDateNow As ConcernViewModel)
        _setDateNow.GetBetWeenDate.DATE1 = DateTime.Now
        _setDateNow.GetBetWeenDate.DATE2 = DateTime.Now

        Return _setDateNow
    End Function

#End Region

#Region "Initialize Service"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AIDEClientService = New AideServiceClient(Context)
            _AIDEClientService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AIDEClientService.Abort()
        End Try
        Return bInitialize
    End Function
#End Region

#Region "Buttons/Text - Events"
    'TextChange Search Keyword
    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        If txtSearch.Text = String.Empty Then
            isSearchIsUsed = 0
            LoadConcernList(0, 10)
        Else
            isSearchIsUsed = 1
            retrieveSearch(0, 10)
            dtpFrom.Text = ""
            dtpTo.Text = ""
        End If
    End Sub

    Private Sub ThreeC_DataGridView_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles ThreeC_DataGridView.LoadingRow
        Dim RowDataContaxt As ConcernModel = TryCast(e.Row.DataContext, ConcernModel)
        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.DUE_DATE = DateTime.Now.ToString("yyyy-MM-dd") And RowDataContaxt.STATUS = "OPEN" Then
                e.Row.Background = New BrushConverter().ConvertFrom("#FFFDECCE")

            ElseIf DateTime.Now.ToString("yyyy-MM-dd") > RowDataContaxt.DUE_DATE And RowDataContaxt.STATUS = "OPEN" Then
                e.Row.Background = New BrushConverter().ConvertFrom("#CCFFD8D8")

            End If
        End If
    End Sub

    'NAVIGATE TO UPDATE PAGE
    Private Sub ThreeC_DataGridView_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles ThreeC_DataGridView.MouseDoubleClick

        If Not ThreeC_DataGridView.SelectedIndex = -1 Then
            If CType(ThreeC_DataGridView.SelectedItem, ConcernModel).STATUS = "CLOSED" Then
                MsgBox("This 3C is already closed. Update is no longer allowed.", MsgBoxStyle.Exclamation + vbCritical, "CLOSED")
            Else
                _addframe.Navigate(New ThreeC_UpdatePage((Me.DataContext()), _frame, email, _menugrid, _submenuframe, _addframe))
                _frame.IsEnabled = False
                _frame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Visibility = Visibility.Visible
                _addframe.Margin = New Thickness(50, 50, 50, 50)
            End If
        End If
    End Sub

    ''NAVIGATE
    Private Sub btnCreateNew3C(sender As Object, e As RoutedEventArgs)
        _addframe.Navigate(New ThreeC_InsertPage(email, _frame, _addframe, _menugrid, _submenuframe))
        ''_frame.Navigate(New ThreeC_InsertPage(email, _frame))
        _frame.IsEnabled = False
        _frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 60, 150, 60)
        _addframe.Visibility = Visibility.Visible
    End Sub

    ''SEARCH FILTER DATE
    Private Sub btnFilter(sender As Object, e As RoutedEventArgs)
        isSearchIsUsed = 2
        If dtpTo.Text = String.Empty Or dtpFrom.Text = String.Empty Then
            MsgBox("Please enter a date from and date to.", MsgBoxStyle.Critical, "AIDE")
        Else
            LoadBetweenSearchDate(offsetVal, nextVal, Me.DataContext())
            GetDateTimeNow(Me.DataContext())
        End If
    End Sub

    Private Sub SetLists()
        Try
            paginatedCollection = New PaginatedObservableCollection(Of ConcernModel)(pagingRecordPerPage)
            Dim _concernViewModel As New ConcernViewModel
            Dim concernDBProvider As New ConcernDBProvider

            For Each objConcern As Concern In _lstConcern
                concernDBProvider.SetConcernList(objConcern)
            Next

            For Each iConcern As MyConcern In concernDBProvider.GetConcernList()
                paginatedCollection.Add(New ConcernModel(iConcern))
            Next

            _concernViewModel.ConcernList = paginatedCollection
            Me.DataContext = _concernViewModel

            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(_lstConcern.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    'PAGE NAVIAGTION -NEXT
    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = _lstConcern.Length

        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
        
        GetDateTimeNow(Me.DataContext())
    End Sub

    'PAGE NAVIGATION BACK
    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click

        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
        
        GetDateTimeNow(Me.DataContext())
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            ThreeC_DataGridView.Measure(pageSize)
            ThreeC_DataGridView.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(ThreeC_DataGridView, "Print 3C's")
        End If

    End Sub

    Private Sub dtpFrom_CalendarClosed(sender As Object, e As RoutedEventArgs) Handles dtpFrom.CalendarClosed
        If dtpFrom.Text IsNot String.Empty Then
            dtpTo.DisplayDateStart = dtpFrom.Text
            dtpTo.DisplayDateEnd = Date.MaxValue
        End If
    End Sub

    Private Sub dtpFrom_KeyUp(sender As Object, e As KeyEventArgs) Handles dtpFrom.KeyUp
        ReloadConcernList(e)
    End Sub

    Private Sub dtpTo_KeyUp(sender As Object, e As KeyEventArgs) Handles dtpTo.KeyUp
        ReloadConcernList(e)
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If _lstConcern.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        ThreeC_DataGridView.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        ThreeC_DataGridView.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub ReloadConcernList(e)
        If e.Key = Key.Back Then
            If dtpFrom.Text Is String.Empty AndAlso dtpTo.Text Is String.Empty Then
                LoadConcernList(offsetVal, nextVal)
            End If
        End If
    End Sub
#End Region

#Region "NotifyChanges"
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
