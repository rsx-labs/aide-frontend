Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Windows
Imports System.Printing

'''''''''''''''''''''''''''''''''
'       JOHN HARVEY SANCHEZ     '
'''''''''''''''''''''''''''''''''
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class WeeklyReportPage
    Implements ServiceReference1.IAideServiceCallback

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
    Private AideServiceClient As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private isEmpty As Boolean
    Private email As String
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private empID As Integer
    Private month As Integer
    Private year As Integer

    Private displayMonth As String

    Dim dateToday As Date = Date.Today
    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)
    Dim lastWeekSaturday As Date = saturday.AddDays(-14)

    Dim dayFriDiff As Integer = Today.DayOfWeek - DayOfWeek.Friday
    Dim friday As Date = Today.AddDays(-dayFriDiff)
    Dim lastWeekFriday As Date = friday.AddDays(-7)

    Dim statusID As Integer = 14
    Dim lstWeekRange As WeekRange()
    Dim lstMissingReports As ContactList()
    Dim lstWeeklyReports As ObservableCollection(Of WeekRangeModel) = New ObservableCollection(Of WeekRangeModel)

    Dim listWeeklyReportStatus As New ObservableCollection(Of WeeklyReportStatusModel)

    Dim weeklyReportCollection As PaginatedObservableCollection(Of WeekRangeModel) = New PaginatedObservableCollection(Of WeekRangeModel)(pagingRecordPerPage)
    Dim missingReportCollection As PaginatedObservableCollection(Of ContactListModel) = New PaginatedObservableCollection(Of ContactListModel)(pagingRecordPerPage)

    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
    Dim weekRangeViewModel As New WeekRangeViewModel
#End Region

#Region "Constructor"
    Public Sub New(_mainFrame As Frame, _empID As Integer, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        InitializeService()
        Me.email = _email
        Me.mainFrame = _mainFrame
        Me.empID = _empID
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe

        month = Date.Now.Month
        year = Date.Now.Year

        LoadMonth()
        LoadYears()

        LoadStatusData()
        SetWeeklyReports()
        SetMissingReports()

        cbMonth.SelectedValue = month
        cbYear.SelectedValue = year
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            AideServiceClient = New AideServiceClient(Context)
            AideServiceClient.Open()
            bInitialize = True
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try
        Return bInitialize
    End Function

#End Region

#Region "Functions"

    Public Sub LoadStatusData()
        Try
            Dim lstStatus As StatusGroup() = AideServiceClient.GetStatusList(statusID)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMyWeeklyReportStatusList(objStatus)
            Next

            For Each myStatus As MyWeeklyReportStatusList In weeklyReportDBProvider.GetWeeklyReportStatusList()
                listWeeklyReportStatus.Add(New WeeklyReportStatusModel(myStatus))
            Next

            weekRangeViewModel.WeeklyReportStatusList = listWeeklyReportStatus
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try
    End Sub

    Public Sub SetWeeklyReports()
        Try
            If InitializeService() Then
                lstWeekRange = AideServiceClient.GetWeeklyReportsByEmpID(empID, month, year)
                LoadWeeklyReports()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LoadWeeklyReports()
        Try
            weeklyReportCollection.Clear()
            weeklyReportDBProvider.GetWeekRangeList().Clear()

            For Each objWeeklyReport As WeekRange In lstWeekRange
                weeklyReportDBProvider.SetWeekRangeList(objWeeklyReport)
            Next

            For Each weekRange As MyWeekRange In weeklyReportDBProvider.GetWeekRangeList()
                weeklyReportCollection.Add(New WeekRangeModel With {
                                            .StartWeek = weekRange.StartWeek,
                                            .EndWeek = weekRange.EndWeek,
                                            .DateSubmitted = weekRange.DateSubmitted,
                                            .Status = weekRange.Status,
                                            .StatusDesc = getStatusValue(weekRange.Status),
                                            .WeekRangeID = weekRange.WeekRangeID
                                         })
            Next

            dgWeeklyReports.ItemsSource = weeklyReportCollection

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetMissingReports()
        Try
            lstMissingReports = AideServiceClient.GetMissingReportsByEmpID(empID, lastWeekSaturday)
            LoadMissingReports()
            DisplayPagingInfo()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LoadMissingReports()
        Try
            Dim contactListDBProvider As New ContactListDBProvider
            Dim contactListVM As New ContactListViewModel()

            For Each objContacts As ContactList In lstMissingReports
                contactListDBProvider.SetMyContactList(objContacts)
            Next

            For Each contacts As MyContactList In contactListDBProvider.GetMyContactList()
                missingReportCollection.Add(New ContactListModel(contacts))
            Next

            dgMissingReports.ItemsSource = missingReportCollection
            currentPage = missingReportCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstMissingReports.Length / pagingRecordPerPage)
            lblMissingReportsWeek.Content = lastWeekSaturday.ToShortDateString + " - " + lastWeekFriday.ToShortDateString + " Missing Reports"
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub LoadMonth()
        cbMonth.DisplayMemberPath = "Text"
        cbMonth.SelectedValuePath = "Value"
        cbMonth.Items.Add(New With {.Text = "January", .Value = 1})
        cbMonth.Items.Add(New With {.Text = "February", .Value = 2})
        cbMonth.Items.Add(New With {.Text = "March", .Value = 3})
        cbMonth.Items.Add(New With {.Text = "April", .Value = 4})
        cbMonth.Items.Add(New With {.Text = "May", .Value = 5})
        cbMonth.Items.Add(New With {.Text = "June", .Value = 6})
        cbMonth.Items.Add(New With {.Text = "July", .Value = 7})
        cbMonth.Items.Add(New With {.Text = "August", .Value = 8})
        cbMonth.Items.Add(New With {.Text = "September", .Value = 9})
        cbMonth.Items.Add(New With {.Text = "October", .Value = 10})
        cbMonth.Items.Add(New With {.Text = "November", .Value = 11})
        cbMonth.Items.Add(New With {.Text = "December", .Value = 12})
    End Sub

    Private Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                Dim nextYear As Integer = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

#End Region

#Region "Events"
    Private Sub btnAddReport_Click(sender As Object, e As RoutedEventArgs) Handles btnAddReport.Click
        addframe.Navigate(New WeeklyReportAddPage(mainFrame, empID, email, addframe, menugrid, submenuframe))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(5, 0, 5, 0)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub dgWeeklyReports_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgWeeklyReports.MouseDoubleClick
        If dgWeeklyReports.SelectedIndex <> -1 Then
            If dgWeeklyReports.SelectedItem IsNot Nothing Then
                Dim weekRangeID As Integer
                weekRangeID = CType(dgWeeklyReports.SelectedItem, WeekRangeModel).WeekRangeID

                addframe.Navigate(New WeeklyReportUpdatePage(weekRangeID, empID, mainFrame, email, addframe, menugrid, submenuframe))
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
                menugrid.IsEnabled = False
                menugrid.Opacity = 0.3
                submenuframe.IsEnabled = False
                submenuframe.Opacity = 0.3
                addframe.Visibility = Visibility.Visible
                addframe.Margin = New Thickness(5, 0, 5, 0)
            End If
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = lstMissingReports.Length

        If totalRecords >= ((missingReportCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            missingReportCollection.CurrentPage = missingReportCollection.CurrentPage + 1
            currentPage = missingReportCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        missingReportCollection.CurrentPage = missingReportCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
    End Sub
#End Region

#Region "Paging"
    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstMissingReports.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub GUISettingsOff()
        dgWeeklyReports.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        dgWeeklyReports.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub cbMonth_DropDownClosed(sender As Object, e As EventArgs) Handles cbMonth.DropDownClosed
        month = cbMonth.SelectedValue
        SetWeeklyReports()
    End Sub

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        year = cbYear.SelectedValue
        SetWeeklyReports()
    End Sub

    Private Sub tcWeeklyReports_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles tcWeeklyReports.SelectionChanged
        If tcWeeklyReports.SelectedIndex = 0 Then
            btnAddReport.Visibility = Windows.Visibility.Visible
        ElseIf tcWeeklyReports.SelectedIndex = 1 Then
            btnAddReport.Visibility = Windows.Visibility.Hidden
        End If
    End Sub
#End Region

#Region "Functions"
    Private Function getStatusValue(key As Integer) As String
        Dim value = listWeeklyReportStatus.Where(Function(x) x.Key = key).FirstOrDefault()

        If value Is Nothing Then
            Return ""
        Else
            Return listWeeklyReportStatus.Where(Function(x) x.Key = key).First().Value
        End If
    End Function
#End Region

#Region "ICallBack Function"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
