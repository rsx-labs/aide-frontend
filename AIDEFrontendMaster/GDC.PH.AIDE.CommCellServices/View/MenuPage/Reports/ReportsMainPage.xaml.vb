Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Security.AccessControl

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ReportsMainPage
    Implements ServiceReference1.IAideServiceCallback

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

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    'Private reportsModel As ReportsModel

    Dim lstReports As Reports()
    Dim totalRecords As Integer
    Dim searchLearning = New ObservableCollection(Of Reports)
    Dim paginatedCollection As PaginatedObservableCollection(Of ReportsModel) = New PaginatedObservableCollection(Of ReportsModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me.profile = _profile
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        SetData()
        btnCreate.Visibility = Windows.Visibility.Collapsed
        'If profile.Permission_ID <> 1 Then
        '    btnCreate.Visibility = Windows.Visibility.Collapsed
        'End If
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstReports = _AideService.GetAllReports()
                LoadSabaCourses()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadSabaCourses()
        Try
            Dim lstReportsList As New ObservableCollection(Of ReportsModel)
            Dim ReportsDBProvider As New ReportsDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of ReportsModel)(pagingRecordPerPage)

            For Each objTracker As Reports In lstReports
                ReportsDBProvider._setlistofitems(objTracker)
            Next

            For Each rawUser As myReportsSet In ReportsDBProvider._getobjReports()
                paginatedCollection.Add(New ReportsModel(rawUser))
            Next

            ReportsLV.ItemsSource = paginatedCollection
            'LoadDataForPrint()
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstReports.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String)
        Try
            'Dim items
            Dim ReportsDBProvider As New ReportsDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of ReportsModel)(pagingRecordPerPage)


            Dim items = From i In lstReports Where i.DESCRIPTION.ToLower.Contains(input.ToLower)
            searchLearning = New ObservableCollection(Of Reports)(items)

            For Each saba As Reports In searchLearning
                ReportsDBProvider._setlistofitems(saba)
            Next

            For Each mysaba As myReportsSet In ReportsDBProvider._getobjReports()
                paginatedCollection.Add(New ReportsModel(mysaba))
            Next

            totalRecords = searchLearning.Count
            ReportsLV.ItemsSource = paginatedCollection
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)

            If totalRecords = 0 Then
                txtPageNo.Text = "No Results Found "
                GUISettingsOff()
            Else
                txtPageNo.Text = "page " & currentPage & " of " & lastPage
                GUISettingsOn()
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstReports.Length

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
                    LoadSabaCourses()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadSabaCourses()
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
                            LoadSabaCourses()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstReports.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstReports.Length = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        ReportsLV.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        ReportsLV.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = lstReports.Length

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
#End Region

#Region "Events"
    Private Sub SearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        SetDataForSearch(SearchTextBox.Text)
    End Sub

    Private Sub ReportsLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If ReportsLV.SelectedIndex <> -1 Then
            If ReportsLV.SelectedItem IsNot Nothing Then
                Dim Reports As New ReportsModel
                'Reports.SABA_ID = CType(ReportsLV.SelectedItem, ReportsModel).SABA_ID
                'Reports.EMP_ID = CType(ReportsLV.SelectedItem, ReportsModel).EMP_ID
                'Reports.TITLE = CType(ReportsLV.SelectedItem, ReportsModel).TITLE
                'Reports.END_DATE = CType(ReportsLV.SelectedItem, ReportsModel).END_DATE
                'Reports.DATE_COMPLETED = CType(ReportsLV.SelectedItem, ReportsModel).DATE_COMPLETED
                'Reports.IMAGE_PATH = CType(ReportsLV.SelectedItem, ReportsModel).IMAGE_PATH

                'addframe.Navigate(New TrackerViewPage(Reports, mainframe, addframe, menugrid, submenuframe, profile))
                'mainframe.IsEnabled = False
                'mainframe.Opacity = 0.3
                'menugrid.IsEnabled = False
                'menugrid.Opacity = 0.3
                'submenuframe.IsEnabled = False
                'submenuframe.Opacity = 0.3
                'addframe.Visibility = Visibility.Visible
                'addframe.Margin = New Thickness(150, 60, 150, 60)

            End If
        End If
    End Sub

    'Private Sub DisableBG()
    '    Frame.IsEnabled = False
    '    Frame.Opacity = 0.3
    '    _menugrid.IsEnabled = False
    '    _menugrid.Opacity = 0.3
    '    _submenuframe.IsEnabled = False
    '    _submenuframe.Opacity = 0.3
    '    _addframe.Margin = New Thickness(150, 60, 150, 60)
    '    _addframe.Visibility = Visibility.Visible
    'End Sub

    Private Sub btnCreate_Click_1(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New TrackerAddPage(profile, mainframe, addframe, menugrid, submenuframe))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(150, 150, 150, 150)
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

    Private Sub ReportsLV_MouseDoubleClick_1(sender As Object, e As MouseButtonEventArgs) Handles ReportsLV.MouseDoubleClick

        Dim Reports As New ReportsModel

        If Not (System.IO.Directory.Exists("C:\GeneratedReports")) Then
            System.IO.Directory.CreateDirectory("C:\GeneratedReports")
        End If

        'Process.Start(Excel.exe)


        Dim result As Integer

        If CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Contact List" Then

            Try
                'Dim fPath As String = "C:\Program Files (x86)\GDC PH\AIDE CommCell\bin" + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
                Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
                Dim procInfo As New ProcessStartInfo()
                procInfo.UseShellExecute = True
                procInfo.FileName = (fPath)
                procInfo.WorkingDirectory = ""
                procInfo.CreateNoWindow = True
                procInfo.Verb = "runas"
                Process.Start(procInfo)

                result = MsgBox("Report has been downloaded. Open the Report Now?", MessageBoxButton.YesNo, "AIDE")
                If result = 6 Then
                    Process.Start("C:\GeneratedReports\Retail Services Contact List.xlsx")
                End If
            Catch ex As SystemException
                _AideService.Abort()
                MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Skills Matrix" Then
            Try
                Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
                Dim procInfo As New ProcessStartInfo()
                procInfo.UseShellExecute = True
                procInfo.FileName = (fPath)
                procInfo.WorkingDirectory = ""
                procInfo.CreateNoWindow = True
                procInfo.Verb = "runas"
                Process.Start(procInfo)
                result = MsgBox("Report has been downloaded. Open the Report Now?", MessageBoxButton.YesNo, "AIDE")
                If result = 6 Then
                    Process.Start("C:\GeneratedReports\Retail Services Skills Matrix.xlsx")
                End If
            Catch ex As SystemException
                _AideService.Abort()
                MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Outstanding task" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New OutstandingTasksFilter(Reports, mainframe, profile, "Borrow", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Asset Inventory" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New AssetInventoryFilter(Reports, mainframe, profile, "Borrow", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Resource Planner" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New ResourcePlannerFilter(Reports, mainframe, profile, "Borrow", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible


        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Status Reports" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New StatusReportFilter(Reports, mainframe, profile, "Borrow", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Employee Billability" Then
            'Dim fPath As String = "C:\Program Files (x86)\GDC PH\AIDE CommCell\bin" + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New BillabilityFilter(Reports, mainframe, profile, "Employee", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Project Billability" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New BillabilityFilter(Reports, mainframe, profile, "Project", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "3Cs Report" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New ImprovementsFilter(Reports, mainframe, profile, "Concerns", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Actions List Report" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New ImprovementsFilter(Reports, mainframe, profile, "ActionsList", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Lessons Learnt Report" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New ImprovementsFilter(Reports, mainframe, profile, "LessonsLearnt", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Success Register Report" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New ImprovementsFilter(Reports, mainframe, profile, "Success", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible

        ElseIf CType(ReportsLV.SelectedItem, ReportsModel).DESCRIPTION = "Commendations Report" Then
            Dim fPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + CType(ReportsLV.SelectedItem, ReportsModel).FILE_PATH
            Reports.FILE_PATH = fPath
            addframe.Navigate(New ImprovementsFilter(Reports, mainframe, profile, "Commendations", addframe, menugrid, submenuframe))
            mainframe.IsEnabled = False
            mainframe.Opacity = 0.3
            menugrid.IsEnabled = False
            menugrid.Opacity = 0.3
            submenuframe.IsEnabled = False
            submenuframe.Opacity = 0.3
            addframe.Margin = New Thickness(150, 60, 150, 60)
            addframe.Visibility = Visibility.Visible


        End If











    End Sub
End Class
