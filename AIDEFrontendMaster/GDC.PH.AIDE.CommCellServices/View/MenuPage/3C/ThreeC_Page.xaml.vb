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

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private profile As Profile


    Private _OptionsViewModel As OptionViewModel
    Dim guestAccount As Integer = 5
    Dim lstConcern As Concern()
    Dim concernDBProvider As New ConcernDBProvider
    Dim client As AideServiceClient
    Dim paginatedCollection As PaginatedObservableCollection(Of ConcernModel) = New PaginatedObservableCollection(Of ConcernModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, _frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        InitializeService()

        profile = _profile
        frame = _frame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe

        pagingRecordPerPage = GetOptionData(23, 8, 12)

        LoadConcernList()
        PermissionSettings()
    End Sub
#End Region
    
#Region "Methods"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    ''DISPLAY to DATAGIRD VIEW
    Public Sub LoadConcernList()
        Try
            If InitializeService() Then
                lstConcern = client.GetAllConcernLst(profile.Emp_ID)
                SetLists(lstConcern)
            End If
        Catch ex As SystemException
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            client.Abort()
        End Try
    End Sub

    Private Sub SetLists(listConcern As Concern())
        Try
            concernDBProvider = New ConcernDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of ConcernModel)(pagingRecordPerPage)

            For Each objConcern As Concern In listConcern
                concernDBProvider.SetConcernList(objConcern)
            Next

            For Each iConcern As MyConcern In concernDBProvider.GetConcernList()
                paginatedCollection.Add(New ConcernModel(iConcern))
            Next

            dg3C.ItemsSource = paginatedCollection

            totalRecords = listConcern.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub Search3C(ByVal search As String)
        Try
            Dim items = From i In lstConcern Where i.Act_Reference.ToLower.Contains(search.ToLower) _
                                            Or i.RefID.ToLower.Contains(search.ToLower) _
                                            Or i.Concerns.ToLower.Contains(search.ToLower) _
                                            Or i.Cause.ToLower.Contains(search.ToLower) _
                                            Or i.CounterMeasure.ToLower.Contains(search.ToLower) _
                                            Or i.EmpID.ToLower.Contains(search.ToLower)

            Dim search3C = New ObservableCollection(Of Concern)(items)

            SetLists(search3C.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    ''DISPLAY to DATAGIRD VIEW WITH DATE SEARCH
    Public Sub Search3CBetweenDates(ByVal search As String)
        Try
            Dim items = From i In lstConcern Where (i.Act_Reference.ToLower.Contains(search.ToLower) _
                                            Or i.RefID.ToLower.Contains(search.ToLower) _
                                            Or i.Concerns.ToLower.Contains(search.ToLower) _
                                            Or i.Cause.ToLower.Contains(search.ToLower) _
                                            Or i.CounterMeasure.ToLower.Contains(search.ToLower) _
                                            Or i.EmpID.ToLower.Contains(search.ToLower)) _
                                            And i.Due_Date >= dtpFrom.SelectedDate AndAlso i.Due_Date <= dtpTo.SelectedDate

            Dim search3C = New ObservableCollection(Of Concern)(items)

            SetLists(search3C.ToArray)
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

    Private Sub GUISettingsOff()
        dg3C.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        dg3C.Visibility = Windows.Visibility.Visible

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
    Private Sub dg3C_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles dg3C.LoadingRow
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
    Private Sub dg3C_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dg3C.MouseDoubleClick
        If Not dg3C.SelectedIndex = -1 Then
            If Not profile.Permission_ID = guestAccount Then
                If CType(dg3C.SelectedItem, ConcernModel).STATUS = "CLOSED" Then
                    MsgBox("This 3C is already closed. Update is no longer allowed.", MsgBoxStyle.Exclamation + vbCritical, "CLOSED")
                Else
                    Dim concern As New ConcernModel
                    concern.REF_ID = CType(dg3C.SelectedItem, ConcernModel).REF_ID
                    concern.CONCERN = CType(dg3C.SelectedItem, ConcernModel).CONCERN
                    concern.CAUSE = CType(dg3C.SelectedItem, ConcernModel).CAUSE
                    concern.COUNTERMEASURE = CType(dg3C.SelectedItem, ConcernModel).COUNTERMEASURE
                    concern.ACT_REFERENCE = CType(dg3C.SelectedItem, ConcernModel).ACT_REFERENCE
                    concern.DUE_DATE = CType(dg3C.SelectedItem, ConcernModel).DUE_DATE

                    addframe.Navigate(New ThreeC_UpdatePage(frame, addframe, menugrid, submenuframe, concern, profile))
                    frame.IsEnabled = False
                    frame.Opacity = 0.3
                    menugrid.IsEnabled = False
                    menugrid.Opacity = 0.3
                    submenuframe.IsEnabled = False
                    submenuframe.Opacity = 0.3
                    addframe.Visibility = Visibility.Visible
                    addframe.Margin = New Thickness(50, 50, 50, 50)
                End If
            End If
        End If
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        If dtpTo.Text = String.Empty Or dtpFrom.Text = String.Empty Then
            Search3C(txtSearch.Text.Trim)
        Else
            Search3CBetweenDates(txtSearch.Text.Trim)
        End If
    End Sub

    Private Sub btnCreateNew3C(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New ThreeC_InsertPage(profile, frame, addframe, menugrid, submenuframe))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(150, 60, 150, 60)
        addframe.Visibility = Visibility.Visible
    End Sub

    ''SEARCH FILTER DATE
    Private Sub btnFilterDate(sender As Object, e As RoutedEventArgs)
        'isSearchIsUsed = 2
        If dtpTo.Text = String.Empty Or dtpFrom.Text = String.Empty Then
            MsgBox("Please enter a date from and date to.", MsgBoxStyle.Critical, "AIDE")
        Else
            Search3CBetweenDates(txtSearch.Text.Trim)
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
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

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            dg3C.Measure(pageSize)
            dg3C.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(dg3C, "Print 3C's")
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

    Private Sub ReloadConcernList(e)
        If e.Key = Key.Back Then
            If dtpFrom.Text Is String.Empty AndAlso dtpTo.Text Is String.Empty Then
                SetLists(lstConcern)
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
