Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AnnouncementDashboard
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

    Dim lstAnnouncements As Announcements()
    Dim AnnouncementListVM As New AnnouncementListViewModel()

    'Private Enum PagingMode
    '    _First = 1
    '    _Next = 2
    '    _Previous = 3
    '    _Last = 4
    'End Enum

#End Region

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
        Me.DataContext = AnnouncementListVM

        If profile.Permission = "Manager" Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If

    End Sub

#End Region

#Region "Functions"

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
                'LoadAnnouncements()
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Public Sub LoadAnnouncements()
        Try
            Dim lstAnnouncementList As New ObservableCollection(Of AnnouncementModel)
            Dim announcementDBProvider As New AnnouncementDBProvider
            Dim objAnnounce As New Announcements


            For i As Integer = startRowIndex To lastRowIndex
                objAnnounce = lstAnnouncements(i)
                announcementDBProvider._setlistofitems(objAnnounce)
            Next

            For Each rawUser As myAnnouncementSet In announcementDBProvider._getobjAnnouncement()
                lstAnnouncementList.Add(New AnnouncementModel(rawUser))
            Next

            AnnouncementListVM.ObjectAnnouncementSet = lstAnnouncementList
            AnnouncementLV.ItemsSource = AnnouncementListVM.ObjectAnnouncementSet
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub
    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstAnnouncements.Length

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
                    LoadAnnouncements()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadAnnouncements()
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
                            LoadAnnouncements()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstAnnouncements.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub
#End Region

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

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New AnnouncementDashboardAddPage(mainframe, empID, addframe, menugrid, submenuframe, email, profile))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(200, 100, 200, 100)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub btnPrev_Click_1(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Previous))
    End Sub

    Private Sub btnNext_Click_1(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Next))
    End Sub

    Private Sub AnnouncementLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If AnnouncementLV.SelectedIndex <> -1 Then
            If profile.Permission = "Manager" Then
                Dim announcementList As New AnnouncementModel
                If AnnouncementLV.SelectedItem IsNot Nothing Then
                    For Each _ann As AnnouncementModel In AnnouncementListVM.ObjectAnnouncementSet
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
                    addframe.Margin = New Thickness(200, 100, 200, 100)
                    addframe.Visibility = Visibility.Visible
                End If
            End If
            
        End If
    End Sub
End Class
