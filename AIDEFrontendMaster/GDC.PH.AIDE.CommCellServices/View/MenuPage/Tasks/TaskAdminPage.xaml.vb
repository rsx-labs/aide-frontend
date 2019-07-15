Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Xps
Imports System.Windows.Xps.Packaging
Imports System.IO
Imports System.Printing



<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class TaskAdminPage
    Implements IAideServiceCallback

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 5

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Fields"
    Public frame As Frame
    Public mainWindow As MainWindow
    Public empID As Integer
    Public email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame

    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim lstTasks As TaskSummary()
    Dim client As AideServiceClient
    Dim paginatedCollection As PaginatedObservableCollection(Of TasksSpModel) = New PaginatedObservableCollection(Of TasksSpModel)(pagingRecordPerPage)

#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _empID As Integer, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        frame = _frame
        mainWindow = _mainWindow
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        empID = _empID
        email = _email
        SetDates()
        LoadEmployeeTaskAll()
    End Sub
#End Region

#Region "Common Methods"
    Private Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
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

    Dim _outstanding As Integer
    Public Property Outstanding As Integer
        Get
            Return _outstanding
        End Get
        Set(value As Integer)
            _outstanding = value
        End Set
    End Property

#Region "Private Methods"

    Private Sub SetDates()
        Dim today As Date = Date.Today

        Dim dayMonDiff As Integer = today.DayOfWeek - DayOfWeek.Monday
        Dim dayTueDiff As Integer = today.DayOfWeek - DayOfWeek.Tuesday
        Dim dayWedDiff As Integer = today.DayOfWeek - DayOfWeek.Wednesday
        Dim dayThuDiff As Integer = today.DayOfWeek - DayOfWeek.Thursday
        Dim dayFriDiff As Integer = today.DayOfWeek - DayOfWeek.Friday

        Dim monday As Date = today.AddDays(-dayMonDiff)
        Dim tuesday As Date = today.AddDays(-dayTueDiff)
        Dim wednesday As Date = today.AddDays(-dayWedDiff)
        Dim thursday As Date = today.AddDays(-dayThuDiff)
        Dim friday As Date = today.AddDays(-dayFriDiff)

        lblMonday.Content = monday
        lblTuesday.Content = tuesday
        lblWednesday.Content = wednesday
        lblThursday.Content = thursday
        lblFriday.Content = friday

        ' Label for Print
        lblMonday2.Content = monday
        lblTuesday2.Content = tuesday
        lblWednesday2.Content = wednesday
        lblThursday2.Content = thursday
        lblFriday2.Content = friday
    End Sub

    Private Sub LoadEmployeeTaskAll()
        Try
            If Me.InitializeService Then
                lstTasks = client.ViewTaskSummaryAll(Convert.ToDateTime(Date.Now).ToString("yyyy-MM-dd"), email)
                LoadData()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadData()
        Try
            Dim taskDBProvider As New TaskDBProvider

            For Each objTask As TaskSummary In lstTasks
                taskDBProvider.SetTasksSpList(objTask)
            Next

            For Each tasks As MyTasksSp In taskDBProvider.GetTasksSp()
                paginatedCollection.Add(New TasksSpModel(tasks))
            Next

            dgTask.ItemsSource = paginatedCollection
            LoadDataForPrint()
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstTasks.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadDataForPrint()
        Try

            Dim lstMyTasks As New ObservableCollection(Of TasksSpModel)
            Dim taskDBProvider As New TaskDBProvider
            Dim taskSpViewModel As New TasksSpViewModel

            Dim objTask As New TaskSummary()

            For i As Integer = 0 To lstTasks.Length - 1
                objTask = lstTasks(i)
                taskDBProvider.SetTasksSpList(objTask)
            Next

            For Each iTasks As MyTasksSp In taskDBProvider.GetTasksSp()
                lstMyTasks.Add(New TasksSpModel(iTasks))
            Next

            'taskSpViewModel.TasksSpList = lstMyTasks
            dgTaskForPrint.ItemsSource = lstMyTasks

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstTasks.Length

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
                    LoadData()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadData()
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
                            LoadData()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstTasks.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select
            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstTasks.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        dgTask.Visibility = Windows.Visibility.Hidden
        dgTask.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        dgTask.Visibility = Windows.Visibility.Visible
        dgTask.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

#End Region

#Region "Buttons"
    Private Sub btnCreateTask_Click(sender As Object, e As RoutedEventArgs) Handles btnCreateTask.Click
        _addframe.Navigate(New TaskAddPage(frame, mainWindow, email, _addframe, _menugrid, _submenuframe, empID))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Visibility = Visibility.Visible
        _addframe.Margin = New Thickness(150, 30, 150, 30)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()
        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            TaskForPrint.Visibility = Windows.Visibility.Visible
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape

            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            TaskForPrint.Measure(pageSize)
            TaskForPrint.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(TaskForPrint, "Print Task")
        End If

        TaskForPrint.Visibility = Windows.Visibility.Hidden
    End Sub

    Private Sub btnViewAll_Click(sender As Object, e As RoutedEventArgs) Handles btnViewAll.Click
        frame.Navigate(New TaskListPage(frame, mainWindow, empID, email, _addframe, _menugrid, _submenuframe))
        'frame.IsEnabled = False
        'frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        '_addframe.Visibility = Visibility.Visible
        '_addframe.Margin = New Thickness(0, 0, 0, 0)
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        Dim totalRecords As Integer = lstTasks.Length

        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
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
