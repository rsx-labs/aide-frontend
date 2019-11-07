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
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private attendanceFrame As Frame
    Private profile As Profile

    Dim month As Integer = Date.Now.Month
    Dim setStatus As Integer
    Dim displayStatus As String = String.Empty
    Dim status As Integer
    Dim img As String
    Dim displayMonth As String
    Dim checkStatus As Integer
    Dim count As Integer
    Dim year As Integer
    Dim day As Integer
    Dim displayOption As Integer = 1 'Weekly is the Default Display Options
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.attendanceFrame = _attendanceFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
        MonthLabel.Text = SetMonths() + " " + year.ToString()
        LoadMonth()
        LoadAllEmpResourcePlanner()
        cbDisplayMonth.Text = SetMonths()
        cbDisplayMonth.SelectedValue = "11"
        LoadYears()
        cbYear.SelectedValue = year


      
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
        End Try
        Return bInitialize
    End Function

    Public Sub LoadYears()
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

    'Public Sub LoadCategory()
    '    Try
    '        InitializeService()
    '        Dim lstresource As ResourcePlanner() = client.GetStatusResourcePlanner()
    '        Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

    '        For Each objResource As ResourcePlanner In lstresource
    '            _ResourceDBProvider.SetCategoryList(objResource)
    '        Next

    '        For Each iResource As myResourceList In _ResourceDBProvider.GetCategoryList()
    '            resourcelist.Add(New ResourcePlannerModel(iResource))
    '        Next
    '        _ResourceViewModel.CategoryList = resourcelist
    '        cbFilterCategory.DataContext = _ResourceViewModel
    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
    '    End Try
    'End Sub

    'Loades All Status
    'Public Sub LoadAllCategory()
    '    Try
    '        InitializeService()
    '        Dim lstresource As ResourcePlanner() = client.GetAllStatusResourcePlanner()
    '        Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

    '        For Each objResource As ResourcePlanner In lstresource
    '            _ResourceDBProvider.SetAllCategoryList(objResource)
    '        Next

    '        For Each iResource As myResourceList In _ResourceDBProvider.GetAllCategoryList()
    '            resourcelist.Add(New ResourcePlannerModel(iResource))
    '        Next
    '        _ResourceViewModel.FilterCategoryList = resourcelist
    '        cbFilterCategory.DataContext = _ResourceViewModel
    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
    '    End Try
    'End Sub

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
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
    '    End Try
    'End Sub

    Public Sub LoadAllEmpResourcePlanner()
        Try
            InitializeService()
            _ResourceDBProvider._splist.Clear()
            If year = 0 Then
                year = Date.Now.Year
            End If
            Dim lstresource As ResourcePlanner() = client.GetAllEmpResourcePlanner(profile.Email_Address, month, year)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

            Dim emp_id As Integer
            Dim abbrDay As String = String.Empty
            Dim it As New List(Of Dictionary(Of String, String))()
            Dim dict As New Dictionary(Of String, String)()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllEmpRPList(objResource)
            Next


            Dim dateFirstSTR As String = month.ToString + "/1/" + year.ToString
            Dim dateFirst As Date = Date.Parse(dateFirstSTR)
            For Each iResource As myResourceList In _ResourceDBProvider.GetAllEmpRPList()
                resourcelist.Add(New ResourcePlannerModel(iResource))

                setStatus = iResource.Status
                SetCategory()

                If emp_id <> iResource.Emp_ID Then
                    If emp_id > 0 Then
                        it.Add(dict)
                    End If
                    dateFirstSTR = month.ToString + "/1/" + year.ToString
                    dateFirst = Date.Parse(dateFirstSTR)
                    dict = New Dictionary(Of String, String)()
                    dict.Add("Employee Name", iResource.Emp_Name)

                    While iResource.Date_Entry.ToString("yyyy-MM-dd") > dateFirst
                        If dateFirst.DayOfWeek <> DayOfWeek.Saturday Or dateFirst.DayOfWeek <> DayOfWeek.Sunday Then
                            abbrDay = setDayOfWeekAbbr(dateFirst.DayOfWeek.ToString())
                            dict.Add(dateFirst.Day.ToString() + Environment.NewLine + abbrDay, "")
                        End If
                        dateFirst = DateAdd(DateInterval.Day, 1, dateFirst)
                    End While
                    abbrDay = setDayOfWeekAbbr(iResource.Date_Entry.DayOfWeek.ToString())
                    dict.Add(iResource.Date_Entry.Day.ToString() + Environment.NewLine + abbrDay, displayStatus)
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
                    dict.Add(iResource.Date_Entry.Day.ToString() + Environment.NewLine + abbrDay, displayStatus)
                End If
                emp_id = iResource.Emp_ID
            Next
            it.Add(dict)
            Dim table As DataTable = New DataTable
            table = ToDataTable(it)
            dgResourcePlanner.RowBackground = New SolidColorBrush(Colors.White)

            dgResourcePlanner.ItemsSource = table.AsDataView

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
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
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
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

    Public Function SetMonths()

        Select Case month
            Case "1"
                displayMonth = "January"
            Case "2"
                displayMonth = "Febuary"
            Case "3"
                displayMonth = "March"
            Case "4"
                displayMonth = "April"
            Case "5"
                displayMonth = "May"
            Case "6"
                displayMonth = "June"
            Case "7"
                displayMonth = "July"
            Case "8"
                displayMonth = "August"
            Case "9"
                displayMonth = "September"
            Case "10"
                displayMonth = "October"
            Case "11"
                displayMonth = "November"
            Case "12"
                displayMonth = "December"
        End Select
        Return displayMonth
    End Function

    'Private Sub btnViewAll_Click(sender As Object, e As RoutedEventArgs) Handles btnViewAll.Click
    '    txtDisplayMonth.Text = String.Empty
    '    cbFilterCategory.Text = String.Empty
    '    LoadAllEmpResourcePlanner()
    'End Sub


    Private Function SetDisplayMonthYr(SelectedMonth As String)
        Dim dpsplaymonthYr As String = ""
        Dim newYr As String = ""
        If year = 0 Then
            year = Date.Now.Year
        End If

        newYr = (year + 1).ToString()
        Select Case SelectedMonth
            Case "1"
                dpsplaymonthYr = "January " + (year + 1).ToString()
            Case "2"
                dpsplaymonthYr = "Febuary " + (year + 1).ToString()
            Case "3"
                dpsplaymonthYr = "March " + (year + 1).ToString()
            Case "4"
                dpsplaymonthYr = "April " + year.ToString()
            Case "5"
                dpsplaymonthYr = "May" + year.ToString()
            Case "6"
                dpsplaymonthYr = "June " + year.ToString()
            Case "7"
                dpsplaymonthYr = "July " + year.ToString()
            Case "8"
                dpsplaymonthYr = "August " + year.ToString()
            Case "9"
                dpsplaymonthYr = "September " + year.ToString()
            Case "10"
                dpsplaymonthYr = "October " + year.ToString()
            Case "11"
                dpsplaymonthYr = "November " + year.ToString()
            Case "12"
                dpsplaymonthYr = "December " + year.ToString()
        End Select
        Return dpsplaymonthYr
    End Function

#End Region

#Region "Button/Event"
    Private Sub cbDisplayMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbDisplayMonth.SelectionChanged
        month = cbDisplayMonth.SelectedValue
        year = cbYear.SelectedValue
        If year = 0 Then
            year = Date.Now.Year
        End If
        MonthLabel.Text = SetDisplayMonthYr(month)
        LoadAllEmpResourcePlanner()
    End Sub


    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        month = cbDisplayMonth.SelectedValue
        year = cbYear.SelectedValue
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
#End Region

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
End Class
