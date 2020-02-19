Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel

Public Class BillabilityManagerVLLeavePage
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
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private lstresource() As ResourcePlanner
    Private mainFrame As Frame
    Private profile As Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private _attendanceFrame As Frame
    Dim paginatedCollection As PaginatedObservableCollection(Of ResourcePlannerModel) = New PaginatedObservableCollection(Of ResourcePlannerModel)(pagingRecordPerPage)

    Dim month As Integer = Date.Now.Month
    Dim displayFiscalYear As Integer = 3
    Dim vlStatus As Integer = 4
    Dim year As Integer
    Dim day As Integer
    Dim selection As Integer
#End Region

    Public Sub New(_profile As Profile, mFrame As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame, attendanceFrame As Frame)
        Me.profile = _profile
        Me.mainFrame = mFrame
        Me._addframe = addframe
        Me._menugrid = menugrid
        Me._submenuframe = submenuframe
        Me._attendanceFrame = attendanceFrame
        Me.InitializeComponent()

        month = Date.Now.Month
        year = Date.Now.Year
    End Sub

#Region "Private Methods"

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

    Private Sub LoadDataActive()
        Try
            InitializeService()
            _ResourceDBProvider._AllLeavesList.Clear()
            paginatedCollection = New PaginatedObservableCollection(Of ResourcePlannerModel)(pagingRecordPerPage)
            If selection = 0 Then
                lstresource = client.GetAllLeavesByEmployee(profile.Emp_ID, vlStatus)
            Else
                lstresource = client.GetAllLeavesHistoryByEmployee(profile.Emp_ID, vlStatus)
            End If
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            Dim resourceListVM As New ResourcePlannerViewModel()

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllLeavesList(objResource)
            Next

            Dim sStatus As String
            For Each iResource As myResourceList In _ResourceDBProvider.GetAllLeavesList()
                If iResource.statuscd = 0 Then
                    sStatus = "Cancelled"
                Else
                    If iResource.endDate < Today.Date Then
                        sStatus = "Completed"
                    ElseIf iResource.startDate > Today.Date Then
                        sStatus = "Scheduled"
                    ElseIf iResource.Status = 5 Or iResource.Status = 6 Or iResource.Status = 9 Or iResource.Status = 12 And iResource.startDate < DateTime.Now Then
                        sStatus = "Completed"
                    Else
                        sStatus = "Ongoing"
                    End If
                End If
                paginatedCollection.Add(New ResourcePlannerModel(iResource, sStatus))
            Next

            If selection = 0 Then
                lv_ActiveLeaves.ItemsSource = paginatedCollection
            Else
                lv_leaveHistory.ItemsSource = paginatedCollection
            End If

            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstresource.Length / pagingRecordPerPage)
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub GUISettingsOff()
        lv_ActiveLeaves.Visibility = Windows.Visibility.Hidden
        lv_leaveHistory.Visibility = Windows.Visibility.Hidden

        btnPrev1.IsEnabled = False
        btnNext1.IsEnabled = False
        btnPrev2.IsEnabled = False
        btnNext2.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_ActiveLeaves.Visibility = Windows.Visibility.Visible
        lv_leaveHistory.Visibility = Windows.Visibility.Visible
        btnPrev1.IsEnabled = True
        btnNext1.IsEnabled = True
        btnPrev2.IsEnabled = True
        btnNext2.IsEnabled = True
    End Sub
    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstresource.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            txtAllPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            txtAllPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Function CancelLeave() As Boolean
        Dim resourcePlanner As New ResourcePlanner

        If lv_ActiveLeaves.SelectedItem IsNot Nothing Then
            resourcePlanner.StartDate = CType(lv_ActiveLeaves.SelectedItem, ResourcePlannerModel).START_DATE
            resourcePlanner.EndDate = CType(lv_ActiveLeaves.SelectedItem, ResourcePlannerModel).END_DATE
            resourcePlanner.Status = CType(lv_ActiveLeaves.SelectedItem, ResourcePlannerModel).Status
            resourcePlanner.EmpID = Me.profile.Emp_ID

            If InitializeService() Then
                client.CancelLeave(resourcePlanner)
            End If

            Return True
        Else
            Return False
        End If
    End Function
#End Region

#Region "Events Triggered"
    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext1.Click
        Dim totalRecords As Integer = lstresource.Length

        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev1.Click
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnNext_Click2(sender As Object, e As RoutedEventArgs) Handles btnNext2.Click
        Dim totalRecords As Integer = lstresource.Length

        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click2(sender As Object, e As RoutedEventArgs) Handles btnPrev2.Click
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If currentPage > 1 Then
            currentPage -= 1
        End If
        DisplayPagingInfo()
    End Sub

    Private Sub backbtn_Click(sender As Object, e As RoutedEventArgs) Handles btnCCancel.Click
        mainFrame.Navigate(New ResourcePlannerPage(profile, mainFrame, _addframe, _menugrid, _submenuframe, _attendanceFrame))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub SR_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SR.SelectionChanged
        If e.Source Is SR Then
            If SR.SelectedItem.Tag = "Own" Then
                selection = 0
            ElseIf SR.SelectedItem.Tag = "All" Then
                selection = 1
            End If
            LoadDataActive()
            DisplayPagingInfo()
        End If
    End Sub

    Private Sub CancelBtn_Click(sender As Object, e As RoutedEventArgs)
        If MsgBox("Are you sure to cancel this leave?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, "AIDE Leave") = vbYes Then
            If CancelLeave() Then
                MsgBox("Successfully cancelled leave/s", MsgBoxStyle.Information, "AIDE")
                selection = 0
                LoadDataActive()
                DisplayPagingInfo()
            End If
        End If
    End Sub

#End Region

#Region "Callback Functions"
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
