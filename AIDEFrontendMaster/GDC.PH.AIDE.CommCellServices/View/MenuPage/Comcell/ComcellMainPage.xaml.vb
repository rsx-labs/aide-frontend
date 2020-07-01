Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports NLog
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellMainPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    'Private _AideService As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    Dim year As Integer = Date.Now.Year
    Dim startYear As Integer = 2019 'Default Start Year
    Dim lstComcell As Comcell()
    Dim ComcellVM As New ComcellViewModel()

    Dim lstFiscalYear As FiscalYear()
    Dim commendationVM As New CommendationViewModel()
    Dim fiscalyearVM As New SelectionListViewModel
    Dim guestAccount As Integer = 5
#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 12
    Dim totalRecords As Integer
    Dim currentPage As Integer
    Dim lastPage As Integer

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
    Dim paginatedCollection As PaginatedObservableCollection(Of ComcellModel) = New PaginatedObservableCollection(Of ComcellModel)(pagingRecordPerPage)

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        _logger.Info("Start : Constructor")

        InitializeComponent()
        Me.empID = _profile.Emp_ID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.DataContext = ComcellVM
        Me.profile = _profile

        _logger.Debug("Load year ...")

        LoadYear()

        _logger.Debug("Set data ...")

        SetData()

        _logger.Debug("Load fiscal year ...")

        LoadFiscalYear()

        _logger.Debug("Load permission setting ...")

        PermissionSettings()

        _logger.Info("End : Constructor")

    End Sub

#End Region

#Region "Functions"

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

    Public Sub SetData()
        Try
            'If InitializeService() Then
            lstComcell = AideClient.GetClient().GetComcellMeeting(empID, year)
            lstFiscalYear = CommonUtility.Instance().FiscalYears 'AideClient.GetClient().GetAllFiscalYear()

            totalRecords = lstComcell.Count
            SetPaging(PagingMode._First)
            DisplayPagingInfo()
            'End If
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadYear()
        If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
            cbYear.SelectedValue = (Date.Now.Year - 1).ToString() + "-" + (Date.Now.Year).ToString()
        Else
            cbYear.SelectedValue = (Date.Now.Year).ToString() + "-" + (Date.Now.Year + 1).ToString()
        End If

        year = CInt(cbYear.SelectedValue.ToString().Substring(0, 4))
    End Sub

    Public Sub LoadComcell()
        Try
            Dim lstComcellList As New ObservableCollection(Of ComcellModel)
            Dim ComcellDBProvider As New ComcellDBProvider
            Dim objComcell As New Comcell

            paginatedCollection = New PaginatedObservableCollection(Of ComcellModel)(pagingRecordPerPage)

            For i As Integer = startRowIndex To lastRowIndex
                objComcell = lstComcell(i)
                ComcellDBProvider.SetMyComcell(objComcell)
            Next

            For Each rawUser As MyComcell In ComcellDBProvider.GetMyComcell()
                'lstComcellList.Add(New ComcellModel(rawUser))
                paginatedCollection.Add(New ComcellModel(rawUser))
            Next

            'currentPage = lstauditSchedList.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)

            'paginatedCollection.CurrentPage = pagingPageIndex
            ComcellVM.ComcellList = paginatedCollection

            Me.DataContext = ComcellVM
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadFiscalYear()
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
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstComcell.Length

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
                    LoadComcell()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadComcell()
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
                            LoadComcell()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstComcell.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub PermissionSettings()
        Dim managerAccount As Integer = 1

        If profile.Permission_ID = managerAccount Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub ComcellLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If ComcellLV.SelectedIndex <> -1 And
            profile.Permission_ID = 1 Or
            profile.FirstName.ToUpper = CType(ComcellLV.SelectedItem, ComcellModel).FACILITATOR_NAME Or
            profile.FirstName.ToUpper = CType(ComcellLV.SelectedItem, ComcellModel).MINUTES_TAKER_NAME Then
            If ComcellLV.SelectedItem IsNot Nothing Then
                Dim comcell As New ComcellModel
                comcell.MONTH = CType(ComcellLV.SelectedItem, ComcellModel).MONTH
                comcell.FACILITATOR = CType(ComcellLV.SelectedItem, ComcellModel).FACILITATOR
                comcell.MINUTES_TAKER = CType(ComcellLV.SelectedItem, ComcellModel).MINUTES_TAKER
                comcell.FACILITATOR_NAME = CType(ComcellLV.SelectedItem, ComcellModel).FACILITATOR_NAME
                comcell.MINUTES_TAKER_NAME = CType(ComcellLV.SelectedItem, ComcellModel).MINUTES_TAKER_NAME
                comcell.COMCELL_ID = CType(ComcellLV.SelectedItem, ComcellModel).COMCELL_ID
                comcell.FY_START = CType(ComcellLV.SelectedItem, ComcellModel).FY_START
                comcell.WEEK = CType(ComcellLV.SelectedItem, ComcellModel).WEEK
                comcell.WEEK_START = CType(ComcellLV.SelectedItem, ComcellModel).WEEK_START

                addframe.Navigate(New ComcellAddPage(profile, mainframe, addframe, menugrid, submenuframe, comcell))
                mainframe.IsEnabled = False
                mainframe.Opacity = 0.3
                menugrid.IsEnabled = False
                menugrid.Opacity = 0.3
                submenuframe.IsEnabled = False
                submenuframe.Opacity = 0.3
                addframe.Visibility = Visibility.Visible
                addframe.Margin = New Thickness(150, 80, 150, 80)
            End If
        End If
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New ComcellAddPage(profile, mainframe, addframe, menugrid, submenuframe))
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
        SetData()
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

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        'If totalRecords >= ((paginatedCollection.CurrentPage * pagingRecordPerPage) + pagingRecordPerPage) Then
        '    paginatedCollection.CurrentPage = paginatedCollection.CurrentPage + 1
        '    currentPage = paginatedCollection.CurrentPage + 1
        '    lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
        'End If
        SetPaging(PagingMode._Next)

        DisplayPagingInfo()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        'paginatedCollection.CurrentPage = paginatedCollection.CurrentPage - 1
        'If currentPage > 1 Then
        '    currentPage -= 1
        'End If
        SetPaging(PagingMode._Previous)

        DisplayPagingInfo()
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If totalRecords = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & pagingPageIndex & " of " & lastPage
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

End Class
