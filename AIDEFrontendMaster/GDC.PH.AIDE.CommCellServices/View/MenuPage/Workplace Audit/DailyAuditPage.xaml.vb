Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Globalization

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class DailyAuditPage
    Implements ServiceReference1.IAideServiceCallback


#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    Private year As Integer
    Private nextYear As Integer
    Private questionGroup As String = "Daily"
#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 15

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        Me.empID = _profile.Emp_ID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile
        Me.year = Date.Now.Year
        SetData()
        SetDates()
        LoadYears()
        EnableTodayColumn()
    End Sub

#End Region

#Region "Functions/Methods"

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False

        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
        End Try
        Return bInitialize
    End Function

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                nextYear = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
            lblYear.Text = "Fiscal Year: " + year.ToString + " - " + nextYear.ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

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

        lblMonday.Content = monday.ToString("MM/dd")
        lblTuesday.Content = tuesday.ToString("MM/dd")
        lblWednesday.Content = wednesday.ToString("MM/dd")
        lblThursday.Content = thursday.ToString("MM/dd")
        lblFriday.Content = friday.ToString("MM/dd")
        lblDept.Content = profile.Department
    End Sub

    Private Sub EnableTodayColumn()
        Select Case Date.Now.DayOfWeek
            Case 1
                TuesdayCB.IsReadOnly = True
                WednesdayCB.IsReadOnly = True
                ThursdayCB.IsReadOnly = True
                FridayCB.IsReadOnly = True
            Case 2


        End Select
    End Sub

    Private Sub SetData()
        Try
            If InitializeService() Then
                Dim lstWorkplaceAudit As WorkplaceAudit() = _AideService.GetAuditQuestions(empID, questionGroup)
                Dim lstWorkplaceAuditList As New ObservableCollection(Of WorkplaceAuditModel)
                Dim workplaceAuditDBProvider As New WorkplaceAuditDBProvider
                Dim WorkplaceAuditVM As New WorkplaceAuditViewModel()

                For Each objLessonLearnt As WorkplaceAudit In lstWorkplaceAudit
                    workplaceAuditDBProvider.SetMyWorkplaceAudit(objLessonLearnt)
                Next

                For Each rawUser As MyWorkplaceAudit In workplaceAuditDBProvider.GetMyWorkplaceAudit()
                    lstWorkplaceAuditList.Add(New WorkplaceAuditModel(rawUser))
                Next

                WorkplaceAuditVM.DMVM = lstWorkplaceAuditList
                Me.DataContext = WorkplaceAuditVM
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

#End Region

#Region "Events"
    Private Sub WorkplaceAudit_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        'e.Handled = True
        'If WorkplaceAuditLV.SelectedIndex <> -1 Then
        '    If WorkplaceAuditLV.SelectedItem IsNot Nothing Then
        '        Dim WorkplaceAudit As New WorkplaceAuditModel
        '        WorkplaceAudit.PERIOD_START = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).PERIOD_START
        '        WorkplaceAudit.PERIOD_END = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).PERIOD_END
        '        WorkplaceAudit.DAILY = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).DAILY
        '        WorkplaceAudit.WEEKLY = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).WEEKLY
        '        WorkplaceAudit.MONTHLY = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).MONTHLY
        '        WorkplaceAudit.AUDIT_SCHED_ID = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).AUDIT_SCHED_ID
        '        WorkplaceAudit.FY_START = CType(WorkplaceAuditLV.SelectedItem, WorkplaceAuditModel).FY_START

        '        addframe.Navigate(New WorkplaceAuditAddPage(profile, mainframe, addframe, menugrid, submenuframe, WorkplaceAudit))
        '        mainframe.IsEnabled = False
        '        mainframe.Opacity = 0.3
        '        menugrid.IsEnabled = False
        '        menugrid.Opacity = 0.3
        '        submenuframe.IsEnabled = False
        '        submenuframe.Opacity = 0.3
        '        addframe.Visibility = Visibility.Visible
        '        addframe.Margin = New Thickness(150, 60, 150, 60)
        '    End If
        'End If
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        'addframe.Navigate(New WorkplaceAuditAddPage(profile, mainframe, addframe, menugrid, submenuframe))
        'mainframe.IsEnabled = False
        'mainframe.Opacity = 0.3
        'menugrid.IsEnabled = False
        'menugrid.Opacity = 0.3
        'submenuframe.IsEnabled = False
        'submenuframe.Opacity = 0.3
        'addframe.Visibility = Visibility.Visible
        'addframe.Margin = New Thickness(200, 100, 200, 100)
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        year = cbYear.SelectedValue
    End Sub

    Private Sub CheckBox_Checked(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Unchecked(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Checked_1(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Unchecked_1(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Checked_2(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Unchecked_2(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Checked_3(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Unchecked_3(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Checked_4(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CheckBox_Unchecked_4(sender As Object, e As RoutedEventArgs)

    End Sub
#End Region

#Region "INotify Methods"
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
