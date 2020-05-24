Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AboutPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    'Private _AideService As ServiceReference1.AideServiceClient
    Private lstContributors As Contributors()
    Private lstMessage As MessageDetail()
    Private ContriVM As New ContributorsViewModel()
    Private MessageVM As New MessageViewModel()
    Private totalRecords As Integer
    Private messageTotalRecords As Integer
    Private paginatedCollection As PaginatedObservableCollection(Of ContributorsModel) = New PaginatedObservableCollection(Of ContributorsModel)(pagingRecordPerPage)
    Private MessagePaginatedCollection As PaginatedObservableCollection(Of MessageModel) = New PaginatedObservableCollection(Of MessageModel)(1)
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

    Public Sub New()

        InitializeComponent()
        ContributorsTC.SelectedIndex = 0
        Me.DataContext = ContriVM.ObjectContributors
        SetData2()
        DisablePrevBtn()

    End Sub

#End Region

#Region "Main Functions"

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        _AideService = New AideServiceClient(Context)
    '        _AideService.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        _AideService.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    Public Sub SetData2()
        Try
            'If InitializeService() Then
            lstMessage = AideClient.GetClient().GetMessage(1015, 20)
            LoadData2()
            messageTotalRecords = lstMessage.Length
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub


    Public Sub DisablePrevBtn()
        If messageTotalRecords > 1 Then
            NextBtn.Visibility = Windows.Visibility.Visible
        End If
        If totalRecords > pagingRecordPerPage Then
            NBtn.Visibility = Windows.Visibility.Visible
            NBtn2.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Public Sub CollapseAllBtn()
        PBtn.Visibility = Windows.Visibility.Collapsed
        NBtn.Visibility = Windows.Visibility.Collapsed
        PBtn.Visibility = Windows.Visibility.Collapsed
        PBtn2.Visibility = Windows.Visibility.Collapsed
    End Sub

    Public Sub SetData()
        Try
            'If InitializeService() Then
            lstMessage = AideClient.GetClient().GetMessage(1015, 20)
            If ContributorsTC.SelectedIndex = 0 Then
                lstContributors = AideClient.GetClient().GetAllContributors(1)
            Else
                lstContributors = AideClient.GetClient().GetAllContributors(2)
            End If
            LoadData()
            totalRecords = lstContributors.Length
            messageTotalRecords = lstMessage.Length
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub LoadData2()
        Try
            Dim lstMessageList As New ObservableCollection(Of MessageModel)
            Dim messageListDBProvider As New MessageDBProvider
            Dim messageVM As New MessageViewModel()

            MessagePaginatedCollection = New PaginatedObservableCollection(Of MessageModel)(1)

            For Each objMessage As MessageDetail In lstMessage
                messageListDBProvider._setlistofitems(objMessage)
            Next

            For Each msg As myMessageSet In messageListDBProvider._getobjMessage()
                MessagePaginatedCollection.Add(New MessageModel(msg))
            Next
            DataContext = MessagePaginatedCollection
            AboutMessageLV.ItemsSource = MessagePaginatedCollection

            'contactListVM.ContactList = lstContactsList
            'lv_contacts.ItemsSource = lstContactsList
            'Me.DataContext = contactListVM
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstContributorsList As New ObservableCollection(Of ContributorsModel)
            Dim contributorsListDBProvider As New ContributorsDBProvider
            Dim contributorsVM As New ContributorsViewModel()

            paginatedCollection = New PaginatedObservableCollection(Of ContributorsModel)(pagingRecordPerPage)

            For Each objContributors As Contributors In lstContributors
                contributorsListDBProvider._setlistofitems(objContributors)
            Next

            For Each contri As myContributorsSet In contributorsListDBProvider._getobjContributors()
                paginatedCollection.Add(New ContributorsModel(contri))
            Next

            If ContributorsTC.SelectedIndex = 0 Then
                MainContributorLV.ItemsSource = paginatedCollection
            Else
                OtherContributorLV.ItemsSource = paginatedCollection
            End If

        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    
#End Region

#Region "Service Model"
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

#Region "Event Triggered"
    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles NextBtn.Click
        Dim totalRecords As Integer = lstMessage.Length

        If totalRecords > ((MessagePaginatedCollection.CurrentPage * 1) + 1) Then
            MessagePaginatedCollection.CurrentPage = MessagePaginatedCollection.CurrentPage + 1
            If Not totalRecords > ((MessagePaginatedCollection.CurrentPage * 1) + 1) Then
                NextBtn.Visibility = Windows.Visibility.Collapsed
            End If
        End If

        If Not MessagePaginatedCollection.CurrentPage = 0 Then
            previousBtn.Visibility = Windows.Visibility.Visible
        End If

    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles previousBtn.Click
        MessagePaginatedCollection.CurrentPage = MessagePaginatedCollection.CurrentPage - 1
        If MessagePaginatedCollection.CurrentPage = 0 Then
            previousBtn.Visibility = Windows.Visibility.Collapsed
        End If
        NextBtn.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub btnNext2_Click(sender As Object, e As RoutedEventArgs) Handles NBtn2.Click
        Dim totalRecords As Integer = lstContributors.Length

        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            If Not totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
                NBtn2.Visibility = Windows.Visibility.Collapsed
            End If
        End If
        If Not paginatedCollection.CurrentPage = 0 Then
            PBtn2.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub btnPrev2_Click(sender As Object, e As RoutedEventArgs) Handles PBtn2.Click
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If paginatedCollection.CurrentPage = 0 Then
            PBtn2.Visibility = Windows.Visibility.Collapsed
        End If
        NBtn2.Visibility = Windows.Visibility.Visible
    End Sub
    Private Sub btnNext3_Click(sender As Object, e As RoutedEventArgs) Handles NBtn.Click
        Dim totalRecords As Integer = lstContributors.Length

        If totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
            paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
            If Not totalRecords > ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
                NBtn.Visibility = Windows.Visibility.Collapsed
            End If
        End If
        If Not paginatedCollection.CurrentPage = 0 Then
            PBtn.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub btnPrev3_Click(sender As Object, e As RoutedEventArgs) Handles PBtn.Click
        paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        If paginatedCollection.CurrentPage = 0 Then
            PBtn.Visibility = Windows.Visibility.Collapsed
        End If
        NBtn.Visibility = Windows.Visibility.Visible
    End Sub
    Private Sub ContributorsTC_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ContributorsTC.SelectionChanged
        e.Handled = True
        paginatedCollection.Clear()
        totalRecords = 0
        SetData()

        CollapseAllBtn()
        If totalRecords > pagingRecordPerPage Then
            NBtn.Visibility = Windows.Visibility.Visible
            NBtn2.Visibility = Windows.Visibility.Visible
        End If
    End Sub
    Private Sub Close_Window_Click(sender As Object, e As RoutedEventArgs) Handles BackBtn.Click
        Me.Close()
    End Sub
#End Region

End Class
