Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
Imports System.Drawing.Printing
''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class SuccessRegisterPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private mainFrame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private profile As Profile
    Private isEmpty As Boolean
    'Private aideService As ServiceReference1.AideServiceClient
    'Private _OptionsViewModel As OptionViewModel

    Dim successRegisterDBProvider As New SuccessRegisterDBProvider
    Dim lstSuccess As SuccessRegister()
    Dim paginatedCollection As PaginatedObservableCollection(Of SuccessRegisterModel) = New PaginatedObservableCollection(Of SuccessRegisterModel)(pagingRecordPerPage)

#End Region

#Region "Constructor"
    Public Sub New(_mainFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)
        InitializeComponent()
        'Me.aideService = aideService
        mainFrame = _mainFrame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        profile = _profile

        pagingRecordPerPage = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PAGING_SREGISTERS)
        paginatedCollection = New PaginatedObservableCollection(Of SuccessRegisterModel)(pagingRecordPerPage)

        LoadSuccessRegister()
        PermissionSettings()
    End Sub
#End Region

#Region "Methods"
    'Public Function InitializeService() As Boolean
    '    'Dim bInitialize As Boolean = False
    '    'Try
    '    '    Dim Context As InstanceContext = New InstanceContext(Me)
    '    '    aideService = New AideServiceClient(Context)
    '    '    aideService.Open()
    '    '    bInitialize = True
    '    'Catch ex As SystemException
    '    '    aideService.Abort()
    '    '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    'End Try
    '    'Return bInitialize
    '    Return True
    'End Function

    Public Sub LoadSuccessRegister()
        Try
            'If InitializeService() Then
            lstSuccess = AideClient.GetClient().ViewSuccessRegisterAll(profile.Email_Address)
            SetLists(lstSuccess)
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetLists(listSuccess As SuccessRegister())
        Try
            successRegisterDBProvider = New SuccessRegisterDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of SuccessRegisterModel)(pagingRecordPerPage)

            For Each objSuccessRegister As SuccessRegister In listSuccess
                successRegisterDBProvider.SetMySuccessRegister(objSuccessRegister)
            Next

            For Each successRegister As MySuccessRegister In successRegisterDBProvider.GetMySuccessRegister()
                paginatedCollection.Add(New SuccessRegisterModel(successRegister))
            Next

            lv_successRegisterAll.ItemsSource = paginatedCollection

            totalRecords = listSuccess.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SearchSuccessRgister(ByVal search As String)
        Try
            Dim items = From i In lstSuccess Where i.Nick_Name.ToLower.Contains(search.ToLower) _
                                            Or i.AdditionalInformation.ToLower.Contains(search.ToLower) _
                                            Or i.DetailsOfSuccess.ToLower.Contains(search.ToLower) _
                                            Or i.WhosInvolve.ToLower.Contains(search.ToLower)

            Dim searchSuccessRegister = New ObservableCollection(Of SuccessRegister)(items)

            SetLists(searchSuccessRegister.ToArray)
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
        lv_successRegisterAll.Visibility = Windows.Visibility.Hidden
        lv_successRegisterAll.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_successRegisterAll.Visibility = Windows.Visibility.Visible
        lv_successRegisterAll.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '    Dim strData As String = String.Empty
    '    Try
    '        _OptionsViewModel = New OptionViewModel
    '        '_OptionsViewModel.Service = aideService
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

    Private Sub NavigatePage()
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(150, 100, 150, 100)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub PermissionSettings()
        Dim guestAccount As Integer = 5

        If profile.Permission_ID = guestAccount Then
            btnSRAdd.Visibility = Windows.Visibility.Hidden
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub btnSRAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnSRAdd.Click
        isEmpty = True

        addframe.Navigate(New NewSuccessRegister(mainFrame, addframe, menugrid, submenuframe, profile))
        NavigatePage()
    End Sub

    Private Sub lv_successRegisterAll_MouseDoubleClick(sender As Object, e As SelectionChangedEventArgs) Handles lv_successRegisterAll.SelectionChanged
        'mainFrame.Navigate(New NewSuccessRegister(lv_successRegisterAll, mainFrame))
        e.Handled = True
        Dim successRegister As New SuccessRegisterModel
        If lv_successRegisterAll.SelectedIndex <> -1 Then
            If lv_successRegisterAll.SelectedItem IsNot Nothing And profile.Emp_ID = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).Emp_ID Then
                successRegister.SuccessID = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).SuccessID
                successRegister.Emp_ID = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).Emp_ID
                successRegister.Nick_Name = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).Nick_Name
                successRegister.DetailsOfSuccess = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).DetailsOfSuccess
                successRegister.WhosInvolve = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).WhosInvolve
                successRegister.AdditionalInformation = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).AdditionalInformation
                successRegister.DateInput = CType(lv_successRegisterAll.SelectedItem, SuccessRegisterModel).DateInput

                addframe.Navigate(New NewSuccessRegister(mainFrame, addframe, menugrid, submenuframe, successRegister, profile))
                NavigatePage()
            End If
        End If

    End Sub

    Private Sub txtSRSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSRSearch.TextChanged
        SearchSuccessRgister(txtSRSearch.Text.Trim)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()
        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape

            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            lv_successRegisterAll.Measure(pageSize)
            lv_successRegisterAll.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(lv_successRegisterAll, "Print Success Register")
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        Dim totalRecords As Integer = lstSuccess.Length

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

#Region "ICallBack Function"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

End Class
