Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class KPITargetsPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 3
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
    'Private _client As ServiceReference1.AideServiceClient
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private _permissionID As Short
    Private _currentEmployeeID As Integer
    Private _emailAddress As String
    Private _currentProfile As Profile

    Dim _lstKPITargets As KPITargets()
    Dim _KPITargetsVM As New KPITargetsViewModel()
    Dim paginatedCollection As PaginatedObservableCollection(Of KPITargetsModel) = New PaginatedObservableCollection(Of KPITargetsModel)(pagingRecordPerPage)

#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _addframe As Frame,
                   _menugrid As Grid, _submenuframe As Frame,
                   permissionId As Short, employeeID As Integer,
                   _email As String, _profile As Profile)

        InitializeComponent()

        '_client = aideService
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me._permissionID = permissionId
        Me._emailAddress = _email
        _currentProfile = _profile
        _currentEmployeeID = employeeID
        SetData()

        If permissionId = 1 Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If

    End Sub

#End Region

#Region "Functions/Methods"

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        _client = New AideServiceClient(Context)
    '        _client.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        _client.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    Public Sub SetData()
        Try
            'If InitializeService() Then
            'Dim fiscalYear As Date = Date.Now()
            _lstKPITargets = CommonUtility.Instance().KPITargetList 'AideClient.GetClient().GetAllKPITargets(Me._currentEmployeeID, fiscalYear)
            LoadKPITargets()
            DisplayPagingInfo()
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadKPITargets()
        Try
            Dim collKPITargets As New ObservableCollection(Of KPITargetsModel)
            Dim dbProvider As New KPITargetDBProvider

            For Each kpiTarget As KPITargets In _lstKPITargets
                dbProvider.SetKPITargets(kpiTarget)
            Next

            For Each kpi As KPITargetSet In dbProvider.GetAllKPITargets()
                paginatedCollection.Add(New KPITargetsModel(kpi))
            Next

            KPITargetsLV.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(_lstKPITargets.Length / pagingRecordPerPage)
            'If lstAnnouncements.Length > pagingRecordPerPage Then
            '    lastPage = (lstAnnouncements.Length / pagingRecordPerPage) + 1
            'End If
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()

        ' If there has no data found
        If _lstKPITargets.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        KPITargetsLV.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        KPITargetsLV.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub
#End Region

#Region "Events"
    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New KPITargetsAddPage(mainframe, _currentEmployeeID, addframe, menugrid, submenuframe, _emailAddress, _currentProfile))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(150, 140, 150, 140)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = _lstKPITargets.Length

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

    Private Sub KPITargetsLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If KPITargetsLV.SelectedIndex <> -1 Then
            If _permissionID = 1 Then
                Dim kpiTargetsList As New KPITargetsModel
                If KPITargetsLV.SelectedItem IsNot Nothing Then
                    For Each kpi As KPITargetsModel In paginatedCollection
                        If CType(KPITargetsLV.SelectedItem, KPITargetsModel).ID = kpi.ID Then
                            kpiTargetsList.ID = kpi.ID
                            kpiTargetsList.EmployeeID = kpi.EmployeeID
                            kpiTargetsList.KPIReferenceNo = kpi.KPIReferenceNo
                            kpiTargetsList.FYStart = kpi.FYStart
                            kpiTargetsList.FYEnd = kpi.FYEnd
                            kpiTargetsList.Subject = kpi.Subject
                            kpiTargetsList.Description = kpi.Description
                            kpiTargetsList.DateCreated = kpi.DateCreated
                            Exit For
                        End If
                    Next

                    addframe.Navigate(New KPITargetsUpdatePage(mainframe, _currentEmployeeID, addframe, menugrid, submenuframe, _emailAddress, _currentProfile, kpiTargetsList))
                    mainframe.IsEnabled = False
                    mainframe.Opacity = 0.3
                    menugrid.IsEnabled = False
                    menugrid.Opacity = 0.3
                    submenuframe.IsEnabled = False
                    submenuframe.Opacity = 0.3
                    addframe.Margin = New Thickness(150, 140, 150, 140)
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
