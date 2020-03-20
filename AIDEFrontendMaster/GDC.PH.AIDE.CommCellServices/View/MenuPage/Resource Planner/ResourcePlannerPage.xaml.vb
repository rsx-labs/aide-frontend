Imports System.IO
Imports System.Data
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Forms
Imports System.Text.RegularExpressions
Imports System.ServiceModel
Imports System.Windows.Xps
Imports System.Windows.Xps.Packaging
Imports System.Printing
Class ResourcePlannerPage
    Implements IAideServiceCallback

#Region "Fields"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourcePADBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private lstresource As ResourcePlanner()
    Private _OptionsViewModel As OptionViewModel
    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private attendanceFrame As Frame
    Private profile As Profile
    Dim lstFiscalYear As FiscalYear()
    Dim commendationVM As New CommendationViewModel()
    Dim fiscalyearVM As New SelectionListViewModel

    Dim month As Integer = Date.Now.Month
    Dim setStatus As String
    Dim displayStatus As String = String.Empty
    Dim status As Integer
    Dim img As String
    Dim displayMonth As String
    Dim checkStatus As Integer
    Dim count As Integer
    Dim year As Integer
    Dim day As Integer
    Dim displayOption As Integer = 1 'Weekly is the Default Display Options
    Dim perfectAttendanceID As Integer

#End Region

    Public Sub New(_profile As Profile, mFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.attendanceFrame = _attendanceFrame
        Me.InitializeComponent()

        LoadMonth()
        LoadYear()
        SetFiscalYear()

        LoadAllEmpResourcePlanner()
        CountMissingLeave()

        PermissionSettings()
    End Sub

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

#Region "Methods"
    Public Sub LoadMonth()
        cbDisplayMonth.DisplayMemberPath = "Text"
        cbDisplayMonth.SelectedValuePath = "Value"
        cbDisplayMonth.Items.Add(New With {.Text = "January", .Value = 1})
        cbDisplayMonth.Items.Add(New With {.Text = "February", .Value = 2})
        cbDisplayMonth.Items.Add(New With {.Text = "March", .Value = 3})
        cbDisplayMonth.Items.Add(New With {.Text = "April", .Value = 4})
        cbDisplayMonth.Items.Add(New With {.Text = "May", .Value = 5})
        cbDisplayMonth.Items.Add(New With {.Text = "June", .Value = 6})
        cbDisplayMonth.Items.Add(New With {.Text = "July", .Value = 7})
        cbDisplayMonth.Items.Add(New With {.Text = "August", .Value = 8})
        cbDisplayMonth.Items.Add(New With {.Text = "September", .Value = 9})
        cbDisplayMonth.Items.Add(New With {.Text = "October", .Value = 10})
        cbDisplayMonth.Items.Add(New With {.Text = "November", .Value = 11})
        cbDisplayMonth.Items.Add(New With {.Text = "December", .Value = 12})
    End Sub

    Public Sub CountMissingLeave()
        Try
            InitializeService()
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            lstresource = client.GetAllNotFiledLeaves(profile.Emp_ID)

            If lstresource.Count = 0 Then
                NotiCountBorder.Visibility = Visibility.Hidden
            Else
                NotiCount.Text = lstresource.Count.ToString()
            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadYear()
        Try
            If InitializeService() Then
                lstFiscalYear = client.GetAllFiscalYear()
                LoadFiscalYear()
            End If
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadFiscalYear()
        Try
            Dim lstFiscalYearList As New ObservableCollection(Of FiscalYearModel)
            Dim FYDBProvider As New SelectionListDBProvider

            For Each objFiscal As FiscalYear In lstFiscalYear
                FYDBProvider._setlistofFiscal(objFiscal)
            Next

            For Each rawUser As myFiscalYearSet In FYDBProvider._getobjFiscal()
                lstFiscalYearList.Add(New FiscalYearModel(rawUser))
            Next

            fiscalyearVM.ObjectFiscalYearSet = lstFiscalYearList
            cbYear.ItemsSource = fiscalyearVM.ObjectFiscalYearSet

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetFiscalYear()
        Try
            month = Date.Now.Month

            If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
                cbYear.Text = (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
            Else
                cbYear.Text = (Date.Now.Year).ToString() + "-" + (Date.Now.Year + 1).ToString()
            End If

            year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))

            cbDisplayMonth.SelectedValue = month
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetCategory()
        If setStatus = 1 Then
            displayStatus = "O"
        ElseIf setStatus = 2 Then
            displayStatus = "P"
        ElseIf setStatus = 3 Then
            displayStatus = "SL"
        ElseIf setStatus = 4 Then
            displayStatus = "VL"
        ElseIf setStatus = 5 Then
            displayStatus = "HSL"
        ElseIf setStatus = 6 Then
            displayStatus = "HVL"
        ElseIf setStatus = 7 Then
            displayStatus = "H"
        ElseIf setStatus = 8 Then
            displayStatus = "EL"
        ElseIf setStatus = 9 Then
            displayStatus = "HEL"
        ElseIf setStatus = 10 Then
            displayStatus = "OL"
        ElseIf setStatus = 11 Then
            displayStatus = "L"
        ElseIf setStatus = 12 Then
            displayStatus = "HOL"
        ElseIf setStatus = 13 Then
            displayStatus = "OBA"
        ElseIf setStatus = 14 Then
            displayStatus = "HOBA"
        Else
            displayStatus = ""
        End If
    End Sub

    Public Function setDayOfWeekAbbr(dayfull As String)
        Dim abbrDay As String = String.Empty

        Select Case dayfull
            Case "Monday"
                abbrDay = "MON"
            Case "Tuesday"
                abbrDay = "TUE"
            Case "Wednesday"
                abbrDay = "WED"
            Case "Thursday"
                abbrDay = "THU"
            Case "Friday"
                abbrDay = "FRI"
            Case "Saturday"
                abbrDay = "SAT"
            Case "Sunday"
                abbrDay = "SUN"
        End Select

        Return abbrDay
    End Function

    'Public Sub LoadEmpResourcePlanner()
    '    Try
    '        InitializeService()
    '        _ResourceDBProvider._splist.Clear()
    '        Dim lstresource As ResourcePlanner() = client.GetResourcePlannerByEmpID(txtEmpID.Text, deptID, month, year)
    '        Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

    '        Dim it As New List(Of Dictionary(Of String, String))()
    '        Dim dict As New Dictionary(Of String, String)()

    '        For Each objResource As ResourcePlanner In lstresource
    '            _ResourceDBProvider.SetEmpRPList(objResource)
    '        Next
    '        For Each iResource As myResourceList In _ResourceDBProvider.GetEmpRPList()
    '            resourcelist.Add(New ResourcePlannerModel(iResource))
    '            setStatus = iResource.Status
    '            checkStatus = iResource.Status

    '            SetCategory()
    '            'dict.Add(iResource.Date_Entry.DayOfWeek, iResource.Date_Entry.Day)
    '            dict.Add(iResource.Date_Entry.ToLongDateString.Substring(0, iResource.Date_Entry.ToLongDateString.Length - 6), displayStatus)  ' Add list of dictionary
    '        Next
    '        it.Add(dict)
    '        Dim table As DataTable = New DataTable
    '        table = ToDataTable(it)
    '        dgResourcePlanner.ItemsSource = table.AsDataView
    '    Catch ex As Exception
    '       MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    'End Sub

    Private Function getSelectedMonth(year As Integer, month As Integer)
        If month < 4 Then
            year = year + 1
        Else
            year = year
        End If

        Return year
    End Function

    Public Sub LoadAllEmpResourcePlanner()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()
            _ResourcePADBProvider._palist.Clear()
            

            Dim currentDate As DateTime = DateTime.Now
            Dim lstresource As ResourcePlanner() = client.GetAllEmpResourcePlanner(profile.Email_Address, month, year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim emp_id As Integer
            Dim bool As Boolean
            Dim count As Integer = 0
            Dim abbrDay As String = String.Empty
            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next

            Dim dateFirstSTR As String = month.ToString + "/1/" + getSelectedMonth(year, month).ToString()
            Dim dateFirst As Date = Date.Parse(dateFirstSTR)
            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
                bool = False
                setStatus = iResource.Status
                ' SetCategory()
                If emp_id <> iResource.Emp_ID Then
                    If emp_id > 0 Then
                        it.Add(dict)
                    End If
                    dateFirstSTR = month.ToString + "/1/" + getSelectedMonth(year, month).ToString()
                    dateFirst = Date.Parse(dateFirstSTR)
                    dict = New Dictionary(Of String, String)()

                    If cbDisplayMonth.SelectedValue <> currentDate.Month Then
                        _ResourcePADBProvider._palist.Clear()

                        Dim perfectAttendancelstresource As ResourcePlanner() = client.GetAllPerfectAttendance(profile.Email_Address, month, year)
                        Dim resourcePAlist As New ObservableCollection(Of ResourcePlannerModel)

                        If perfectAttendancelstresource.Length = 0 Then
                            dict.Add("Employee Name", iResource.Emp_Name)
                        Else
                            For Each objResource As ResourcePlanner In perfectAttendancelstresource
                                _ResourcePADBProvider.SetPerfectAttendanceList(objResource)
                            Next

                            For Each iPAResource As myResourceList In _ResourcePADBProvider.GetPerfectAttendanceList()
                                resourcePAlist.Add(New ResourcePlannerModel(iPAResource))

                                If dict.ContainsValue((iResource.Emp_Name)) And Not it.Count = 0 Then
                                    If Not it.Item(count - 1).ContainsValue(iResource.Emp_Name) And dict.ContainsValue((iResource.Emp_Name)) Then
                                        dict.Clear()
                                    End If
                                End If

                                If iPAResource.Emp_ID = iResource.Emp_ID Then
                                    dict.Add("Employee Name", iResource.Emp_Name + " ★")
                                    bool = True
                                Else
                                    If Not dict.ContainsValue((iResource.Emp_Name)) And Not bool Then
                                        dict.Add("Employee Name", iResource.Emp_Name)
                                    End If
                                End If
                            Next
                            count += 1
                        End If
                    Else
                        dict.Add("Employee Name", iResource.Emp_Name)
                    End If

                    While iResource.Date_Entry.ToString("yyyy-MM-dd") > dateFirst
                        If dateFirst.DayOfWeek <> DayOfWeek.Saturday Or dateFirst.DayOfWeek <> DayOfWeek.Sunday Then
                            abbrDay = setDayOfWeekAbbr(dateFirst.DayOfWeek.ToString())
                            dict.Add(dateFirst.Day.ToString() + Environment.NewLine + abbrDay, "")
                        End If
                        dateFirst = DateAdd(DateInterval.Day, 1, dateFirst)
                    End While
                    abbrDay = setDayOfWeekAbbr(iResource.Date_Entry.DayOfWeek.ToString())
                    dict.Add(iResource.Date_Entry.Day.ToString() + Environment.NewLine + abbrDay, iResource.Status)
                End If
                If emp_id = iResource.Emp_ID Then
                    abbrDay = setDayOfWeekAbbr(dateFirst.DayOfWeek.ToString())
                    If dict.ContainsKey(dateFirst.Day.ToString() + Environment.NewLine + abbrDay) Then
                        dateFirst = DateAdd(DateInterval.Day, 1, dateFirst)
                    End If
                    While iResource.Date_Entry.ToString("yyyy-MM-dd") <> dateFirst
                        If dateFirst.DayOfWeek <> DayOfWeek.Saturday Or dateFirst.DayOfWeek <> DayOfWeek.Sunday Then
                            abbrDay = setDayOfWeekAbbr(dateFirst.DayOfWeek.ToString())
                            dict.Add(dateFirst.Day.ToString() + Environment.NewLine + abbrDay, "")
                        End If
                        dateFirst = DateAdd(DateInterval.Day, 1, dateFirst)
                    End While
                    abbrDay = setDayOfWeekAbbr(iResource.Date_Entry.DayOfWeek.ToString())
                    dict.Add(iResource.Date_Entry.Day.ToString() + Environment.NewLine + abbrDay, iResource.Status)
                End If
                emp_id = iResource.Emp_ID
            Next
            it.Add(dict)
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)
            dgResourcePlanner.RowBackground = New SolidColorBrush(Colors.White)
            dgResourcePlanner.ItemsSource = table.AsDataView
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Apply in cbFilterSelectionChanged
    'Public Sub LoadAllEmpResourcePlannerByStatus()
    '    Try
    '        InitializeService()
    '        _ResourceDBProvider._splist.Clear()
    '        Dim lstresource As ResourcePlanner() = client.GetResourcePlanner(profile.Email_Address, cbFilterCategory.SelectedValue, displayOption)
    '        Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
    '        Dim resourceListVM As New ResourcePlannerViewModel()

    '        For Each objResource As ResourcePlanner In lstresource
    '            _ResourceDBProvider.SetAllEmpRPList(objResource)
    '        Next

    '        For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
    '            resourcelist.Add(New ResourcePlannerModel(iResource))

    '        Next
    '        resourceListVM.ResourceListLeaveCredits = resourcelist
    '        dgLeaveCredits.ItemsSource = resourcelist

    '    Catch ex As Exception
    '       MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    'End Sub

    Private Function ToDataTable(list As List(Of Dictionary(Of String, String))) As DataTable
        Dim result As New DataTable()

        If list.Count = 0 Then
            Return result
        End If

        Dim columnNames = list.SelectMany(Function(dict) dict.Keys).Distinct()
        result.Columns.AddRange(columnNames.[Select](Function(c) New DataColumn(c)).ToArray())
        For Each item As Dictionary(Of String, String) In list
            Dim row = result.NewRow()
            For Each key In item.Keys
                row(key) = item(key)
            Next
            result.Rows.Add(row)
        Next
        Return result
    End Function

    'Public Sub disableBtns()
    '    'dtpFrom.IsEnabled = False
    '    'dtpTo.IsEnabled = False
    '    btnCreateLeave.IsEnabled = False
    'End Sub

    'Public Sub Clear()
    '    dtpFrom.Text = String.Empty
    '    dtpTo.Text = String.Empty
    '    cbCategory.Text = String.Empty
    'End Sub

    'Private Sub calendar_DisplayDateChanged(sender As Object, e As CalendarDateChangedEventArgs) Handles calendar.DisplayDateChanged
    '    txtDisplayMonth.Text = String.Empty
    '    month = calendar.DisplayDate.Month
    '    year = calendar.DisplayDate.Year
    '    LoadAllEmpResourcePlanner()
    'End Sub

    'Private Sub btnViewAll_Click(sender As Object, e As RoutedEventArgs) Handles btnViewAll.Click
    '    txtDisplayMonth.Text = String.Empty
    '    cbFilterCategory.Text = String.Empty
    '    LoadAllEmpResourcePlanner()
    'End Sub

    Private Function SetDisplayMonthYr(selectedMonth As Integer)
        Dim displayMonth As String = ""
        Dim newYear As Integer

        If selectedMonth < 4 Then
            newYear = year + 1
        Else
            newYear = year
        End If

        displayMonth = MonthName(selectedMonth) + " " + newYear.ToString
       
        Return displayMonth
    End Function

    Private Sub PermissionSettings()
        Dim guestAccount As Integer = 5

        If profile.Permission_ID = guestAccount Then
            btnCreateLeave.Visibility = Windows.Visibility.Hidden
            btnNotification.Visibility = Windows.Visibility.Hidden
            btnManage.Visibility = Windows.Visibility.Hidden
        End If
    End Sub
#End Region

#Region "Button/Event"
    Private Sub cbDisplayMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbDisplayMonth.SelectionChanged

        If Not cbDisplayMonth.SelectedIndex = -1 Then
            month = cbDisplayMonth.SelectedItem.Value
        End If

        If cbYear.SelectedValue IsNot Nothing Then
            year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
        End If

        If year = 0 Then
            year = Date.Now.Year
        End If

        MonthLabel.Text = SetDisplayMonthYr(month)
        LoadAllEmpResourcePlanner()
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
        MonthLabel.Text = SetDisplayMonthYr(month)
        LoadAllEmpResourcePlanner()
    End Sub

    'Private Sub cbFilterCategory_DropDownClosed(sender As Object, e As EventArgs) Handles cbFilterCategory.DropDownClosed
    '    SetMonths()
    'End Sub

    'Private Sub cbFilterDisplay_DropDownClosed(sender As Object, e As EventArgs) Handles cbFilterDsiplay.DropDownClosed
    '    'GetDisplayOptions()
    '    Dim _displayOption As String = cbFilterDsiplay.Text
    '    Select Case _displayOption
    '        Case "Monthly"
    '            displayOption = 2
    '        Case "Fiscal Year"
    '            displayOption = 3
    '        Case Else
    '            displayOption = 1
    '    End Select

    '    LoadAllEmpResourcePlannerByStatus()
    'End Sub

    Private Sub btnCreateLeave_Click(sender As Object, e As RoutedEventArgs) Handles btnCreateLeave.Click
        _addframe.Navigate(New ResourcePlannerAddPage(profile, mainFrame, _addframe, _menugrid, _submenuframe, attendanceFrame))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 80, 150, 80)
        _addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As New System.Windows.Controls.PrintDialog
        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape

            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            dgResourcePlanner.Measure(pageSize)
            dgResourcePlanner.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(dgResourcePlanner, "Print Leave Credits")
        End If
    End Sub

    Private Sub btnManage_Click(sender As Object, e As RoutedEventArgs)
        _addframe.Navigate(New BillabilityManagerVLLeavePage(profile, mainFrame, _addframe, _menugrid, _submenuframe, attendanceFrame))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(100, 60, 100, 60)
        _addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub btnNotification_Click(sender As Object, e As RoutedEventArgs) Handles btnNotification.Click
        _addframe.Navigate(New AttendanceNotification(profile, mainFrame, _addframe, _menugrid, _submenuframe, attendanceFrame))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(0)
        _addframe.Visibility = Visibility.Visible
    End Sub

#End Region

#Region "ICallback Functions"
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
