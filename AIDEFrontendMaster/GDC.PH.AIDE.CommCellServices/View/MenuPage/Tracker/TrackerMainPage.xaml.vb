Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class SabaLearningMainPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    'Private _AideService As ServiceReference1.AideServiceClient
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    'Private _OptionsViewModel As OptionViewModel
    Dim lstSabaLearning As SabaLearning()
    Dim sabalearningDBProvider As New SabaLearningDBProvider
    Dim paginatedCollection As PaginatedObservableCollection(Of SabaLearningModel) = New PaginatedObservableCollection(Of SabaLearningModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        mainframe = _mainframe
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        profile = _profile

        pagingRecordPerPage = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PAGING_TRACKER)
        LoadSabaCourses()
        PermissionSettings()
    End Sub

#End Region

#Region "Functions/Methods"
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

    Public Sub LoadSabaCourses()
        Try
            'If InitializeService() Then
            lstSabaLearning = AideClient.GetClient().GetAllSabaCourses(profile.Emp_ID)
            SetLists(lstSabaLearning)
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetLists(listSabaLearning As SabaLearning())
        Try
            sabalearningDBProvider = New SabaLearningDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of SabaLearningModel)(pagingRecordPerPage)

            For Each objTracker As SabaLearning In listSabaLearning
                sabalearningDBProvider._setlistofitems(objTracker)
            Next

            For Each rawUser As mySabaLearningSet In sabalearningDBProvider._getobjSabaLearning()
                paginatedCollection.Add(New SabaLearningModel(rawUser))
            Next

            SabaLearningLV.ItemsSource = paginatedCollection
            'LoadDataForPrint()
            totalRecords = listSabaLearning.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '    Dim strData As String = String.Empty
    '    Try
    '        _OptionsViewModel = New OptionViewModel
    '        '_OptionsViewModel.Service = _AideService
    '        If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
    '            For Each opt As OptionModel In _OptionsViewModel.OptionList
    '                If Not opt Is Nothing Then
    '                    strData = opt.VALUE
    '                    Exit For
    '                End If
    '            Next
    '        End If
    '    Catch ex As Exception
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return strData
    'End Function

    Private Sub SearchSabaCourses(search As String)
        Try
            Dim items = From i In lstSabaLearning Where i.TITLE.ToLower.Contains(search.ToLower)

            Dim searchSabaCourse = New ObservableCollection(Of SabaLearning)(items)

            SetLists(searchSabaCourse.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If totalRecords = 0 Then
            txtPageNo.Text = "No Results Found "
            GUISettingsOff()
        Else
            txtPageNo.Text = "page " & currentPage & " of " & lastPage
            GUISettingsOn()
        End If
    End Sub

    Private Sub GUISettingsOff()
        SabaLearningLV.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        SabaLearningLV.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub PermissionSettings()
        If profile.Permission_ID <> 1 Then
            btnCreate.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub SearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        SearchSabaCourses(SearchTextBox.Text.Trim)
    End Sub

    Private Sub SabaLearningLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If SabaLearningLV.SelectedIndex <> -1 Then
            If SabaLearningLV.SelectedItem IsNot Nothing Then
                Dim sabalearning As New SabaLearningModel
                sabalearning.SABA_ID = CType(SabaLearningLV.SelectedItem, SabaLearningModel).SABA_ID
                sabalearning.EMP_ID = CType(SabaLearningLV.SelectedItem, SabaLearningModel).EMP_ID
                sabalearning.TITLE = CType(SabaLearningLV.SelectedItem, SabaLearningModel).TITLE
                sabalearning.END_DATE = CType(SabaLearningLV.SelectedItem, SabaLearningModel).END_DATE
                sabalearning.DATE_COMPLETED = CType(SabaLearningLV.SelectedItem, SabaLearningModel).DATE_COMPLETED
                sabalearning.IMAGE_PATH = CType(SabaLearningLV.SelectedItem, SabaLearningModel).IMAGE_PATH

                addframe.Navigate(New TrackerViewPage(sabalearning, mainframe, addframe, menugrid, submenuframe, profile))
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
    End Sub

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

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
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
