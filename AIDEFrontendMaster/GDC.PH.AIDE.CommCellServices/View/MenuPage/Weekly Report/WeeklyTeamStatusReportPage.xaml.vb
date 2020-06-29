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
Class WeeklyTeamStatusReportPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
    Dim EntryType As Integer = 0

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Fields"
    'Private AideServiceClient As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private isEmpty As Boolean
    Private email As String
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private profile As New Profile
    Private empID As Integer
    Private month As Integer
    Private year As Integer
    Private startFiscalYear As Integer
    Private endFiscalYear As Integer
    'Private _OptionsViewModel As OptionViewModel

    Dim dateToday As Date = Date.Today
    Dim monday As DateTime = Today.AddDays((Today.DayOfWeek - DayOfWeek.Monday) * -1)
    Dim currentWeekSaturday As DateTime = monday.AddDays(-2)

    Dim daySatDiff As Integer = Today.DayOfWeek - DayOfWeek.Saturday
    Dim saturday As Date = Today.AddDays(-daySatDiff)
    Dim lastWeekSaturday As Date = saturday.AddDays(-14)

    Dim dayFriDiff As Integer = Today.DayOfWeek - DayOfWeek.Friday
    Dim friday As Date = Today.AddDays(-dayFriDiff)
    Dim lastWeekFriday As Date = friday.AddDays(-7)

    Dim selectedValue As Integer
    Dim weekID As Integer

    Dim statusID As Integer = 14
    Dim lstWeeklyTeamStatusReport As WeeklyTeamStatusReport()

    Dim lstFiscalYear As FiscalYear()

    Dim lstWeekRange As WeekRange()
    Dim listWeeklyReportStatus As New ObservableCollection(Of WeeklyReportStatusModel)
    Dim weeklyTeamStatusReportCollection As PaginatedObservableCollection(Of WeeklyTeamStatusReportModel)

    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
    Dim weekRangeViewModel As New WeekRangeViewModel

    Dim fiscalyearVM As New SelectionListViewModel
#End Region

#Region "Constructor"
    Public Sub New(_mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        'InitializeService()

        Me.email = _profile.Email_Address
        Me.mainFrame = _mainFrame
        Me.empID = _profile.Emp_ID
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile

        pagingRecordPerPage = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PAGING_WREPORT)
        weeklyTeamStatusReportCollection = New PaginatedObservableCollection(Of WeeklyTeamStatusReportModel)(pagingRecordPerPage)
        selectedValue = -1

        InitializeData()
    End Sub

    Public Sub New(_mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _weekRangeID As Integer, _month As Integer, _year As Integer)
        InitializeComponent()
        'InitializeService()
        Me.email = _profile.Email_Address
        Me.mainFrame = _mainFrame
        Me.empID = _profile.Emp_ID
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile
        Me.weekID = _weekRangeID

        pagingRecordPerPage = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PAGING_WREPORT)
        weeklyTeamStatusReportCollection = New PaginatedObservableCollection(Of WeeklyTeamStatusReportModel)(pagingRecordPerPage)

        selectedValue = _weekRangeID
        month = _month
        year = _year

        InitializeData()
    End Sub

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        AideServiceClient = New AideServiceClient(Context)
    '        AideServiceClient.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        AideServiceClient.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    Public Sub InitializeData()
        LoadMonth()
        LoadYear()

        SetFiscalYear()

        LoadWeeks()
        LoadStatusData()
        SetWeeklyTeamStatusReports()
    End Sub

#End Region

#Region "Functions"

    Public Sub LoadStatusData()
        Try
            Dim lstStatus As StatusGroup() = AideClient.GetClient().GetStatusList(statusID)

            For Each objStatus As StatusGroup In lstStatus
                weeklyReportDBProvider.SetMyWeeklyReportStatusList(objStatus)
            Next

            For Each myStatus As MyWeeklyReportStatusList In weeklyReportDBProvider.GetWeeklyReportStatusList()
                listWeeklyReportStatus.Add(New WeeklyReportStatusModel(myStatus))
            Next

            weekRangeViewModel.WeeklyReportStatusList = listWeeklyReportStatus
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetWeeklyTeamStatusReports()
        Try
            If profile.Permission_ID <> 1 Then
                EntryType = 1
            End If

            'If InitializeService() Then
            lstWeeklyTeamStatusReport = AideClient.GetClient().GetWeeklyTeamStatusReport(empID, month, startFiscalYear, selectedValue, EntryType)
            LoadWeeklyTeamStatusReports()

            totalRecords = lstWeeklyTeamStatusReport.Length
            DisplayPagingInfo()
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadWeeklyTeamStatusReports()
        Try
            weeklyTeamStatusReportCollection = New PaginatedObservableCollection(Of WeeklyTeamStatusReportModel)(pagingRecordPerPage)
            weeklyReportDBProvider.GetWeeklyTeamStatusReportList().Clear()

            For Each objWeeklyTeamStatusReport As WeeklyTeamStatusReport In lstWeeklyTeamStatusReport
                weeklyReportDBProvider.SetWeeklyTeamStatusReportList(objWeeklyTeamStatusReport)
            Next

            For Each weeklyTeamStatusReport As MyWeeklyTeamStatusReport In weeklyReportDBProvider.GetWeeklyTeamStatusReportList()
                weeklyTeamStatusReportCollection.Add(New WeeklyTeamStatusReportModel With {.WeekRangeID = weeklyTeamStatusReport.WeekRangeID,
                                                                                           .EmployeeID = weeklyTeamStatusReport.EmployeeID,
                                                                                           .EmployeeName = weeklyTeamStatusReport.EmployeeName,
                                                                                           .TotalHours = weeklyTeamStatusReport.TotalHours,
                                                                                           .Status = weeklyTeamStatusReport.Status,
                                                                                           .StatusDesc = getStatusValue(weeklyTeamStatusReport.Status),
                                                                                           .DateSubmitted = weeklyTeamStatusReport.DateSubmitted
                                                                                          })
            Next

            dgWeeklyTeamStatusReports.ItemsSource = weeklyTeamStatusReportCollection

            currentPage = weeklyTeamStatusReportCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstWeeklyTeamStatusReport.Length / pagingRecordPerPage)
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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

    Public Sub LoadYear()
        Try
            'If InitializeService() Then
            lstFiscalYear = CommonUtility.Instance().FiscalYears 'AideClient.GetClient().GetAllFiscalYear()
            LoadFiscalYear()
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub LoadWeeks()
        ' Load Items for Week Range Combobox
        Try
            'If InitializeService() Then
            ' Clear combo box data
            cbDateRange.DataContext = Nothing
            weeklyReportDBProvider.GetWeekRangeList().Clear()

            Dim listWeekRange As New ObservableCollection(Of WeekRangeModel)
            weekRangeViewModel = New WeekRangeViewModel

            lstWeekRange = AideClient.GetClient().GetWeekRangeByMonthYear(profile.Emp_ID, month, startFiscalYear)

            For Each objWeekRange As WeekRange In lstWeekRange
                weeklyReportDBProvider.SetWeekRangeList(objWeekRange)
            Next

            For Each weekRange As MyWeekRange In weeklyReportDBProvider.GetWeekRangeList()
                listWeekRange.Add(New WeekRangeModel(weekRange))

                'If monday.Month = dateToday.Month Then
                If currentWeekSaturday = weekRange.StartWeek Then
                    If selectedValue = -1 Then
                        selectedValue = weekRange.WeekRangeID
                    End If
                End If
                'Else
                '    If lastWeekSaturday = weekRange.StartWeek Then
                '        If selectedValue = -1 Then
                '            selectedValue = weekRange.WeekRangeID
                '        End If
                '    End If
                'End If

                weekID = weekRange.WeekRangeID
            Next

            ' Set selectedValue to last week of month
            If selectedValue = -1 AndAlso lstWeekRange.Count > 0 Then
                selectedValue = weekID
            End If

            weekRangeViewModel.WeekRangeList = listWeekRange
            cbDateRange.DataContext = weekRangeViewModel
            cbDateRange.SelectedValue = selectedValue
            'End If
        Catch ex As SystemException
            'AideServiceClient.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadFiscalYear()
        Try
            Dim lstFiscalYearList As New ObservableCollection(Of FiscalYearModel)
            Dim FYDBProvider As New SelectionListDBProvider

            For Each objFiscalYear As FiscalYear In lstFiscalYear
                FYDBProvider._setlistofFiscal(objFiscalYear)
            Next

            For Each rawFiscalYear As myFiscalYearSet In FYDBProvider._getobjFiscal()
                lstFiscalYearList.Add(New FiscalYearModel(rawFiscalYear))
            Next

            fiscalyearVM.ObjectFiscalYearSet = lstFiscalYearList
            cbYear.ItemsSource = fiscalyearVM.ObjectFiscalYearSet
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetFiscalYear()
        Try
            If month = 0 Then
                If monday.Month = dateToday.Month Then
                    month = dateToday.Month
                    year = dateToday.Year
                Else
                    month = monday.Month
                    year = lastWeekSaturday.Year
                End If

                If month >= 4 Then
                    startFiscalYear = year
                    endFiscalYear = year + 1
                Else
                    startFiscalYear = year - 1
                    endFiscalYear = year
                End If
            Else
                startFiscalYear = year
                endFiscalYear = year + 1
            End If

            cbYear.DisplayMemberPath = "FISCAL_YEAR"
            cbYear.SelectedValuePath = "FISCAL_YEAR"

            cbMonth.SelectedValue = month
            cbYear.SelectedValue = startFiscalYear & "-" & endFiscalYear

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

#End Region

#Region "Events"
    Private Sub dgWeeklyTeamStatusReports_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgWeeklyTeamStatusReports.MouseDoubleClick
        If dgWeeklyTeamStatusReports.SelectedIndex <> -1 Then
            If dgWeeklyTeamStatusReports.SelectedItem IsNot Nothing Then
                Dim weekRangeID As Integer
                Dim empID As Integer
                weekRangeID = CType(dgWeeklyTeamStatusReports.SelectedItem, WeeklyTeamStatusReportModel).WeekRangeID
                empID = CType(dgWeeklyTeamStatusReports.SelectedItem, WeeklyTeamStatusReportModel).EmployeeID

                addframe.Navigate(New WeeklyReportUpdatePage(weekRangeID, month, startFiscalYear, mainFrame, profile, addframe, menugrid, submenuframe, empID))
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
        Dim totalRecords As Integer = lstWeeklyTeamStatusReport.Length

        If totalRecords >= ((weeklyTeamStatusReportCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            weeklyTeamStatusReportCollection.CurrentPage = weeklyTeamStatusReportCollection.CurrentPage + 1
            currentPage = weeklyTeamStatusReportCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        weeklyTeamStatusReportCollection.CurrentPage = weeklyTeamStatusReportCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnMyReports_Click(sender As Object, e As RoutedEventArgs) Handles btnMyReports.Click
        mainFrame.Navigate(New WeeklyReportPage(mainFrame, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub GUISettingsOff()
        dgWeeklyTeamStatusReports.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        dgWeeklyTeamStatusReports.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub cbMonth_DropDownClosed(sender As Object, e As EventArgs) Handles cbMonth.DropDownClosed
        month = cbMonth.SelectedValue
        selectedValue = -1
        LoadWeeks()
        SetWeeklyTeamStatusReports()
    End Sub

    Private Sub cbYear_DropDownClosed(sender As Object, e As EventArgs) Handles cbYear.DropDownClosed
        startFiscalYear = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
        selectedValue = -1
        LoadWeeks()
        SetWeeklyTeamStatusReports()
    End Sub

    Private Sub cbDateRange_DropDownClosed(sender As Object, e As EventArgs) Handles cbDateRange.DropDownClosed
        selectedValue = cbDateRange.SelectedValue
        SetWeeklyTeamStatusReports()
    End Sub
#End Region

#Region "Paging"
    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstWeeklyTeamStatusReport.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
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

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '    Dim strData As String = String.Empty
    '    Try
    '        _OptionsViewModel = New OptionViewModel
    '        '_OptionsViewModel.Service = AideServiceClient
    '        If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
    '            For Each opt As OptionModel In _OptionsViewModel.OptionList
    '                If Not opt Is Nothing Then
    '                    strData = opt.VALUE
    '                    Exit For
    '                End If
    '            Next
    '        End If
    '    Catch ex As Exception
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return strData
    'End Function
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
