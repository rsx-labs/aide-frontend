Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AnnouncementDashboard
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 4
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
    Private _AideService As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile

    Dim lstAnnouncements As Announcements()
    Dim AnnouncementListVM As New AnnouncementListViewModel()
    Dim paginatedCollection As PaginatedObservableCollection(Of AnnouncementModel) = New PaginatedObservableCollection(Of AnnouncementModel)(pagingRecordPerPage)

#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _empID As Integer, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _email As String, _profile As Profile)

        InitializeComponent()
        Me.empID = _empID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile
        Me.email = _email
        SetData()

        If profile.Permission_ID = 1 Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If

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

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstAnnouncements = _AideService.GetAnnouncements(empID)
                LoadAnnouncements()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadAnnouncements()
        Try
            Dim lstAnnouncementList As New ObservableCollection(Of AnnouncementModel)
            Dim announcementDBProvider As New AnnouncementDBProvider

            For Each objAnnouncements As Announcements In lstAnnouncements
                announcementDBProvider._setlistofitems(objAnnouncements)
            Next

            For Each rawUser As myAnnouncementSet In announcementDBProvider._getobjAnnouncement()
                paginatedCollection.Add(New AnnouncementModel(rawUser))
            Next

            AnnouncementLV.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstAnnouncements.Length / pagingRecordPerPage)
            'If lstAnnouncements.Length > pagingRecordPerPage Then
            '    lastPage = (lstAnnouncements.Length / pagingRecordPerPage) + 1
            'End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()

        ' If there has no data found
        If lstAnnouncements.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        AnnouncementLV.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        AnnouncementLV.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub
#End Region

#Region "Events"
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New AnnouncementDashboardAddPage(mainframe, empID, addframe, menugrid, submenuframe, email, profile))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(150, 60, 150, 60)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = lstAnnouncements.Length

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

    Private Sub AnnouncementLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If AnnouncementLV.SelectedIndex <> -1 Then
            If profile.Permission_ID = 1 Then
                Dim announcementList As New AnnouncementModel
                If AnnouncementLV.SelectedItem IsNot Nothing Then
                    For Each _ann As AnnouncementModel In paginatedCollection
                        If CType(AnnouncementLV.SelectedItem, AnnouncementModel).ANNOUNCEMENT_ID = _ann.ANNOUNCEMENT_ID Then
                            announcementList.ANNOUNCEMENT_ID = _ann.ANNOUNCEMENT_ID
                            announcementList.EMP_ID = _ann.EMP_ID
                            announcementList.TITLE = _ann.TITLE
                            announcementList.MESSAGE = _ann.MESSAGE
                            announcementList.DATE_POSTED = _ann.DATE_POSTED
                        End If
                    Next
                    addframe.Navigate(New AnnouncementDashboardUpdatePage(mainframe, empID, addframe, menugrid, submenuframe, email, profile, announcementList))
                    mainframe.IsEnabled = False
                    mainframe.Opacity = 0.3
                    menugrid.IsEnabled = False
                    menugrid.Opacity = 0.3
                    submenuframe.IsEnabled = False
                    submenuframe.Opacity = 0.3
                    addframe.Margin = New Thickness(150, 60, 150, 60)
                    addframe.Visibility = Visibility.Visible
                End If
            End If

        End If
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
