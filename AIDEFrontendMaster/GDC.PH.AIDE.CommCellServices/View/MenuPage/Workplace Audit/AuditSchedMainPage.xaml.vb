Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports NLog
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AuditSchedMainPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Fields"

    Private _client As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    Private year As Integer
    Private month As Integer
    Private nextYear As Integer

    Dim lstauditSched As AuditSched()
    Dim lstauditSchedList As PaginatedObservableCollection(Of AuditSchedModel) = New PaginatedObservableCollection(Of AuditSchedModel)(pagingRecordPerPage)
    Dim auditSchedVM As New AuditSchedViewModel()
    Dim lstFiscalYear As FiscalYear()
    Dim fiscalyearVM As New SelectionListViewModel
    Dim guestAccount As Integer = 5

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _profile As Profile,
                   _addframe As Frame, _menugrid As Grid,
                   _submenuframe As Frame, aideService As AideServiceClient)

        _logger.Debug("Start : Constructor")

        InitializeComponent()

        _client = aideService
        Me.empID = _profile.Emp_ID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.DataContext = auditSchedVM
        Me.profile = _profile

        If profile.Permission_ID = 1 Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If

        year = Date.Now.Year
        LoadYear()
        SetFiscalYear()
        SetData()

        _logger.Debug("End : Constructor")

    End Sub

#End Region

#Region "Functions/Methods"

    Public Function InitializeService() As Boolean
        _logger.Debug("Start : InitializeService")

        Dim bInitialize As Boolean = False
        Try

            If _client.State = CommunicationState.Faulted Then

                _logger.Debug("Service is faulted, reinitializing ...")

                Dim Context As InstanceContext = New InstanceContext(Me)
                _client = New AideServiceClient(Context)
                _client.Open()
            End If

            bInitialize = True
        Catch ex As SystemException
            _logger.Error(ex.ToString())

            _client.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : InitializeService")

        Return bInitialize
    End Function

    Public Sub LoadYear()

        _logger.Debug("Start : LoadYear")

        Try
            If InitializeService() Then
                lstFiscalYear = _client.GetAllFiscalYear()
                LoadFiscalYear()
            End If
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : LoadYear")
    End Sub

    Public Sub SetData()

        _logger.Debug("Start : SetData")

        Try
            If InitializeService() Then
                lstauditSched = _client.GetAuditSched(empID, year)
                LoadAuditSched()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : SetData")

    End Sub

    Public Sub LoadAuditSched()

        _logger.Debug("Start : LoadAuditSchedule")

        Try
            Dim auditSchedDBProvider As New AuditSchedDBProvider

            lstauditSchedList = New PaginatedObservableCollection(Of AuditSchedModel)(pagingRecordPerPage)

            For Each objAuditSched As AuditSched In lstauditSched
                auditSchedDBProvider.SetMyAuditSched(objAuditSched)
            Next

            For Each auditSched As MyAuditSched In auditSchedDBProvider.GetMyAuditSched()
                lstauditSchedList.Add(New AuditSchedModel(auditSched))
            Next

            currentPage = lstauditSchedList.CurrentPage + 1
            lastPage = Math.Ceiling(lstauditSched.Length / pagingRecordPerPage)

            AuditSchedLV.ItemsSource = lstauditSchedList
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : LoadAuditSchedule")
    End Sub

    Public Sub LoadFiscalYear()

        _logger.Debug("Start : LoadFiscalYear")

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
            _logger.Debug(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : LoadFiscalYear")

    End Sub

    Public Sub SetFiscalYear()

        _logger.Debug("Start : SetFiscalYear")

        Try
            month = Date.Now.Month

            If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
                cbYear.Text = (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
            Else
                cbYear.Text = (Date.Now.Year).ToString() + "-" + (Date.Now.Year + 1).ToString()
            End If

            year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))

            'lblYear.Text = "Fiscal Year: " + (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : SetFiscalYear")

    End Sub

    Private Sub SetPaging(mode As Integer)

        _logger.Debug("Start : SetPaging")

        Try
            Dim totalRecords As Integer = lstauditSched.Length

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
                    LoadAuditSched()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadAuditSched()
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
                            LoadAuditSched()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstauditSched.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

            DisplayPagingInfo()
        Catch ex As Exception

            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : SetPaging")

    End Sub
    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstauditSched.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub
    Private Sub GUISettingsOff()
        'lv_team.Visibility = Windows.Visibility.Hidden
        'lv_all.Visibility = Windows.Visibility.Hidden
        'lv_unapproved.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        'lv_team.Visibility = Windows.Visibility.Visible
        'lv_all.Visibility = Windows.Visibility.Visible
        'lv_unapproved.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub
#End Region

#Region "Events"
    Private Sub AuditSched_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)

        _logger.Debug("Start : AuditSched_MouseDoubleClick")

        Try

            e.Handled = True
            If Not profile.Permission_ID = guestAccount Then
                If AuditSchedLV.SelectedIndex <> -1 Then
                    If AuditSchedLV.SelectedItem IsNot Nothing Then
                        Dim auditSched As New AuditSchedModel
                        auditSched.PERIOD_START = CType(AuditSchedLV.SelectedItem, AuditSchedModel).PERIOD_START
                        auditSched.PERIOD_END = CType(AuditSchedLV.SelectedItem, AuditSchedModel).PERIOD_END
                        auditSched.DAILY = CType(AuditSchedLV.SelectedItem, AuditSchedModel).DAILY
                        auditSched.WEEKLY = CType(AuditSchedLV.SelectedItem, AuditSchedModel).WEEKLY
                        auditSched.MONTHLY = CType(AuditSchedLV.SelectedItem, AuditSchedModel).MONTHLY
                        auditSched.AUDIT_SCHED_ID = CType(AuditSchedLV.SelectedItem, AuditSchedModel).AUDIT_SCHED_ID
                        auditSched.FY_START = CType(AuditSchedLV.SelectedItem, AuditSchedModel).FY_START

                        addframe.Navigate(New AuditSchedAddPage(profile, mainframe, addframe, menugrid, submenuframe, auditSched, _client))
                        mainframe.IsEnabled = False
                        mainframe.Opacity = 0.3
                        menugrid.IsEnabled = False
                        menugrid.Opacity = 0.3
                        submenuframe.IsEnabled = False
                        submenuframe.Opacity = 0.3
                        addframe.Visibility = Visibility.Visible
                        addframe.Margin = New Thickness(150, 60, 150, 60)
                    End If
                End If
            End If
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : AuditSched_MouseDoubleClick")
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New AuditSchedAddPage(profile, mainframe, addframe, menugrid, submenuframe, lstauditSched, _client))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(150, 60, 150, 60)
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
        'lblYear.Text = "Fiscal Year: " + year.ToString + " - " + (year + 1).ToString
        SetData()
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        SetPaging(CInt(PagingMode._Next))
        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        SetPaging(CInt(PagingMode._Previous))
        DisplayPagingInfo()
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
