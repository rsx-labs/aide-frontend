Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AttendanceNotification
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    Private client As AideServiceClient
    Private _ResourceDBProvider As New ResourcePlannerDBProvider
    Private _ResourceViewModel As New ResourcePlannerViewModel
    Private lstresource As ResourcePlanner()
    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private attendanceFrame As Frame
    Private profile As Profile
    Dim paginatedCollection As PaginatedObservableCollection(Of ResourcePlannerModel) = New PaginatedObservableCollection(Of ResourcePlannerModel)(pagingRecordPerPage)


#End Region

#Region "Paging Declarations"
    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
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
        LoadMissingLeave()

    End Sub
#End Region
#Region "Events"
    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstresource.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        noNotiTxt.Visibility = Visibility.Visible
        LeaveNotyLV.Visibility = Visibility.Hidden
        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        noNotiTxt.Visibility = Visibility.Hidden
        LeaveNotyLV.Visibility = Visibility.Visible
        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub
    Private Sub btnUpdateLeaveNoty_Click(sender As Object, e As RoutedEventArgs)
        Dim resourcePlanner As New ResourcePlanner

        If LeaveNotyLV.SelectedItem IsNot Nothing Then
            Dim _date As DateTime = CType(LeaveNotyLV.SelectedItem, ResourcePlannerModel).DATE_ENTRY.ToString("MM/dd/yyyy")

            _addframe.Navigate(New ResourcePlannerAddPage(profile, mainFrame, _addframe, _menugrid, _submenuframe, attendanceFrame, _date.ToString("MM/dd/yyyy")))
            mainFrame.IsEnabled = False
            mainFrame.Opacity = 0.3
            _menugrid.IsEnabled = False
            _menugrid.Opacity = 0.3
            _submenuframe.IsEnabled = False
            _submenuframe.Opacity = 0.3
            _addframe.Margin = New Thickness(150, 80, 150, 80)
            _addframe.Visibility = Visibility.Visible

        End If
    End Sub
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs) Handles BackBtn.Click
        ExitPage()
    End Sub
    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = lstresource.Length

        If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
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

    Public Sub LoadMissingLeave()
        Try
            InitializeService()
            Dim resourcelist As New ObservableCollection(Of ResourcePlannerModel)
            lstresource = client.GetAllNotFiledLeaves(profile.Emp_ID)
            paginatedCollection = New PaginatedObservableCollection(Of ResourcePlannerModel)(pagingRecordPerPage)

            For Each objResource As ResourcePlanner In lstresource
                _ResourceDBProvider.SetAllNotFiledLeaves(objResource)
            Next

            For Each iResource As myResourceList In _ResourceDBProvider.GetAllNotFiledLeaves()
                paginatedCollection.Add(New ResourcePlannerModel(iResource))
            Next

            LeaveNotyLV.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstresource.Length / pagingRecordPerPage)
            DisplayPagingInfo()
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
#Region "Service Callbacks"
    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate
        Throw New NotImplementedException()
    End Sub
#End Region

End Class
