﻿Imports System.IO
Imports System.Data
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Forms
Imports System.Text.RegularExpressions
Imports System.ServiceModel

Class ResourcePlannerAddPage
    Implements ServiceReference1.IAideServiceCallback

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
    Private mainwindows As MainWindow

    Dim isManager As Integer = 1
    Dim isHoliday As Integer = 7
    Dim setStatus As Integer
    Dim isHalfDay As Boolean
    Dim displayStatus As String = String.Empty

    Dim dateFrom As Date
    Dim dateTo As Date
#End Region

#Region "Constructor"
    Public Sub New(_profile As Profile, mFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.attendanceFrame = _attendanceFrame
        Me.InitializeComponent()
        LoadData()
        LoadCategory()
        LoadSchedule()
        cbSchedule.IsEnabled = False
    End Sub
#End Region

#Region "Events"
    Private Sub dtpFrom_CalendarClosed(sender As Object, e As RoutedEventArgs) Handles dtpFrom.CalendarClosed
        If Not dtpFrom.SelectedDate Is Nothing Then
            If cbCategory.SelectedValue = 5 Or cbCategory.SelectedValue = 6 Or cbCategory.SelectedValue = 9 Or cbCategory.SelectedValue = 12 Or cbCategory.SelectedValue = 14 Then
                dtpTo.Text = dtpFrom.Text
                dtpTo.IsEnabled = False
            ElseIf cbCategory.SelectedValue = 3 Or cbCategory.SelectedValue = 4 Or cbCategory.SelectedValue = 7 Or cbCategory.SelectedValue = 8 Or cbCategory.SelectedValue = 10 Or cbCategory.SelectedValue = 13 Then
                dtpTo.DisplayDateStart = dtpFrom.Text
                dtpTo.DisplayDateEnd = Date.MaxValue
                dtpTo.IsEnabled = True
            ElseIf (cbCategory.SelectedIndex = 1 Or cbCategory.SelectedIndex = 2 Or cbCategory.SelectedIndex = 3) Then
                dtpTo.DisplayDateStart = dtpFrom.Text
                dtpTo.DisplayDateEnd = Date.MaxValue
                dtpTo.IsEnabled = True
            Else
                dtpTo.IsEnabled = True
            End If
        End If
    End Sub

    Private Sub dtpTo_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dtpTo.SelectedDateChanged
        btnCreateLeave.IsEnabled = True
    End Sub

    Private Sub btnCreateLeave_Click(sender As Object, e As RoutedEventArgs) Handles btnCreateLeave.Click
        Try
            If profile.Permission_ID <> 1 AndAlso cbCategory.DisplayMemberPath = "Holiday" Then
                MsgBox("Access to file Holiday leave has been denied. Please contact your Manager.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                If cbCategory.Text = String.Empty Or dtpFrom.Text = String.Empty Or dtpTo.Text = String.Empty Then
                    MsgBox("Please enter all required fields. Ensure all required fields have * indicated.!", MsgBoxStyle.Exclamation, "AIDE")
                    'ElseIf cbCategory.SelectedValue = setStatus Then
                    '    Dim notify = MsgBox("There is already an existing " & cbCategory.Text & " for this date" & vbNewLine & "Do you wish to proceed?", MsgBoxStyle.YesNo, "AIDE")
                    '    If notify = MsgBoxResult.Yes Then
                    '        InsertResourcePlanner()
                    '    End If
                Else
                    If isHalfDay And cbSchedule.SelectedIndex = -1 Then
                        MsgBox("Please enter all required fields. Ensure all required fields have * indicated.!", MsgBoxStyle.Exclamation, "AIDE")
                    Else
                        SetDates()
                        If CheckLeaveExists() And (Not cbCategory.SelectedValue = isHoliday) Then
                            MsgBox("Unable to file leave. Leaves already applied within the date range", MsgBoxStyle.Exclamation, "AIDE")
                        Else
                            Dim ans = MsgBox("Are you sure you want to file " & cbCategory.Text & "?", MsgBoxStyle.YesNo, "AIDE")

                            If ans = MsgBoxResult.Yes Then
                                InsertResourcePlanner()
                                dtpTo.IsEnabled = True
                                attendanceFrame.Navigate(New AttendanceDashBoard(mainFrame, profile))
                                ExitPage()
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub lstEmployee_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstEmployee.SelectionChanged
        If profile.Permission_ID = 1 Then
            txtEmpID.Text = lstEmployee.SelectedValue
        Else
            txtEmpID.Text = profile.Emp_ID
            MsgBox("Sorry you do not have authorization for this Employee", MsgBoxStyle.Exclamation, "AIDE")
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        ExitPage()
    End Sub

    Private Sub cbCategory_DropDownOpened(sender As Object, e As EventArgs) Handles cbCategory.DropDownOpened
        dtpFrom.IsEnabled = True
        dtpFrom.Text = ""
        dtpTo.Text = ""
    End Sub

    Private Sub cbCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCategory.SelectionChanged
        If cbCategory.SelectedValue = 5 Or cbCategory.SelectedValue = 6 Or cbCategory.SelectedValue = 9 Or cbCategory.SelectedValue = 12 Or cbCategory.SelectedValue = 14 Then
            cbSchedule.IsEnabled = True
            isHalfDay = True
            txtSchedule.Text = "Select Schedule *"
        Else
            cbSchedule.IsEnabled = False
            isHalfDay = False
            txtSchedule.Text = "Select Schedule"
        End If

        If cbCategory.SelectedValue = 3 Or cbCategory.SelectedValue = 5 Then
            dtpFrom.DisplayDateStart = Date.MinValue
            dtpFrom.DisplayDateEnd = Date.Today
        Else
            dtpFrom.DisplayDateStart = Date.Today
            dtpFrom.DisplayDateEnd = Date.MaxValue
        End If

        dtpFrom.IsEnabled = True
    End Sub
#End Region

#Region "Function"

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

    Public Sub LoadEmployee()
        Try
            InitializeService()
            Dim lstresource As ResourcePlanner() = client.ViewEmpResourcePlanner(profile.Email_Address)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetResourceList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetResourceList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
            Next
            _ResourceViewModel.ResourceList = resourcelist
            lstEmployee.DataContext = _ResourceViewModel
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub InsertResourcePlanner()
        Dim Resource As New ResourcePlanner

        Resource.NAME = ""
        Resource.DESCR = ""
        Resource.Image_Path = ""
        Resource.EmpID = txtEmpID.Text
        Resource.dateFrom = dateFrom
        Resource.dateTo = dateTo
        Resource.Status = cbCategory.SelectedValue

        If profile.Permission_ID = isManager Then
            client.InsertResourcePlannerForManager(Resource)
        Else
            client.InsertResourcePlannerForEmployee(Resource)
        End If

        'If profile.Emp_ID <> txtEmpID.Text Then
        '    client.InsertResourcePlanner(Resource)
        'Else
        '    client.UpdateResourcePlanner(Resource)
        'End If

        _ResourceDBProvider._splist.Clear()
        MsgBox(cbCategory.Text & " has been applied. ", MsgBoxStyle.Information, "AIDE")
    End Sub

    Public Sub LoadCategory()
        Try
            InitializeService()
            Dim lstresource As ResourcePlanner() = client.GetStatusResourcePlanner(profile.Emp_ID)
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetCategoryList(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetCategoryList()
                resourcelist.Add(New ResourcePlannerModel(iResource))
            Next
            _ResourceViewModel.CategoryList = resourcelist
            cbCategory.DataContext = _ResourceViewModel
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Function CheckLeaveExists() As Boolean
        Dim bLeaveExists As Boolean
        Dim empID As Integer

        Try
            InitializeService()

            empID = Integer.Parse(txtEmpID.Text)
            Dim lstresource As ResourcePlanner() = client.GetLeavesByDateAndEmpID(empID, cbCategory.SelectedValue, dateFrom, dateTo)

            If lstresource.Count > 0 Then
                bLeaveExists = True
            End If

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        Return bLeaveExists
    End Function

    Public Sub SetDates()
        If cbSchedule.Text = "Morning".Trim.ToString() Then
            dateFrom = dtpFrom.SelectedDate.Value.AddHours(0).AddMinutes(0).AddSeconds(0)
            dateTo = dtpTo.SelectedDate.Value.AddHours(13).AddMinutes(59).AddSeconds(59)
        ElseIf cbSchedule.Text = "Afternoon" Then
            dateFrom = dtpFrom.SelectedDate.Value.AddHours(14).AddMinutes(0).AddSeconds(0)
            dateTo = dtpTo.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59)
        Else
            dateFrom = dtpFrom.SelectedDate.Value.AddHours(0).AddMinutes(0).AddSeconds(0)
            dateTo = dtpTo.SelectedDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59)
        End If
    End Sub

    Public Sub LoadData()
        txtEmpID.Text = profile.Emp_ID
        txtEmpID.IsReadOnly = True
        btnCreateLeave.IsEnabled = False
        If profile.Permission_ID = 1 Then
            txtEmpID.Text = profile.Emp_ID
            LoadEmployee()
        Else
            txtInfo.Text = "Please enter all required fields. Ensure all required fields have * indicated.."
            GridForManagers.Visibility = Windows.Visibility.Collapsed
            GridLine.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub DatePicker_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim datePicker As DatePicker = CType(sender, DatePicker)
        If (Not (datePicker) Is Nothing) Then
            Dim datePickerTextBox As System.Windows.Controls.Primitives.DatePickerTextBox = FindVisualChild(Of System.Windows.Controls.Primitives.DatePickerTextBox)(datePicker)
            If (Not (datePickerTextBox) Is Nothing) Then
                Dim watermark As ContentControl = CType(datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox), ContentControl)
                If (Not (watermark) Is Nothing) Then
                    watermark.Content = String.Empty
                    'or set it some value here...
                End If
            End If
        End If
    End Sub

    Private Function FindVisualChild(Of T)(ByVal depencencyObject As DependencyObject) As T
        If (Not (depencencyObject) Is Nothing) Then
            Dim i As Integer = 0
            Do While (i < VisualTreeHelper.GetChildrenCount(depencencyObject))
                Dim child As DependencyObject = VisualTreeHelper.GetChild(depencencyObject, i)
                Dim result As T
                FindVisualChild(Of T)(child)
                If (Not (result) Is Nothing) Then
                    Return result
                End If
                i = (i + 1)
            Loop
        End If
        Return Nothing
    End Function

    Public Sub LoadSchedule()
        Try
            cbSchedule.DisplayMemberPath = "Text"
            cbSchedule.SelectedValuePath = "Value"
            cbSchedule.Items.Add(New With {.Text = "Morning"})
            cbSchedule.Items.Add(New With {.Text = "Afternoon"})
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ExitPage()
        mainFrame.Navigate(New ResourcePlannerPage(profile, mainFrame, _addframe, _menugrid, _submenuframe, attendanceFrame))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub
#End Region

#Region "ICallBack Functions"
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
