Imports System.Data
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
''' <summary>
''' By John Harvey Sanchez / Marivic Espino
''' </summary>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class LessonLearntPage
    Implements IAideServiceCallback

#Region "Paging Declarations"
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
    Dim totalRecords As Integer
#End Region

#Region "Fields"
    Private frame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile

    Dim lstLesson As LessonLearnt()
    Dim lessonLearntDBProvider As New LessonLearntDBProvider
    'Dim client As AideServiceClient
    'Private _OptionsViewModel As OptionViewModel
    Dim paginatedCollection As PaginatedObservableCollection(Of LessonLearntModel) = New PaginatedObservableCollection(Of LessonLearntModel)(pagingRecordPerPage)
#End Region

#Region "Constructor"
    Public Sub New(_frame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)
        InitializeComponent()
        'client = aideService
        frame = _frame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        email = _profile.Email_Address
        profile = _profile

        pagingRecordPerPage = AppState.GetInstance().OptionValueDictionary(Constants.OPT_PAGING_PROJECT)

        LoadLessonLearntList()
        PermissionSettings()
    End Sub
#End Region

#Region "Methods"

    'Private Function InitializeService() As Boolean
    'Dim bInitialize As Boolean = False
    'Try
    '    Dim context As InstanceContext = New InstanceContext(Me)
    '    client = New AideServiceClient(context)
    '    client.Open()
    '    bInitialize = True
    'Catch ex As SystemException
    '    client.Abort()
    '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    'End Try
    'Return bInitialize
    'Return True
    'End Function

    Private Sub LoadLessonLearntList()
        Try
            'If InitializeService() Then
            lstLesson = AideClient.GetClient().GetLessonLearntList(profile.Email_Address)
            SetData(lstLesson)
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SetData(lstLessonsLearnt As LessonLearnt())
        Try
            lessonLearntDBProvider = New LessonLearntDBProvider
            paginatedCollection = New PaginatedObservableCollection(Of LessonLearntModel)(pagingRecordPerPage)

            ' Set the MyLessonLearntList 
            For Each objLessonsLearnt As LessonLearnt In lstLessonsLearnt
                lessonLearntDBProvider.SetLessonLearntList(objLessonsLearnt)
            Next

            ' Set the lstLessonsLearnt
            For Each lessonsLearnt As MyLessonLearntList In lessonLearntDBProvider.GetLessonLearntList()
                paginatedCollection.Add(New LessonLearntModel(lessonsLearnt))
            Next

            dgLessonLearnt.ItemsSource = paginatedCollection

            totalRecords = lstLessonsLearnt.Length
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(totalRecords / pagingRecordPerPage)
            DisplayPagingInfo()

            'LoadDataForPrint()
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SearchProblemEncountered(ByVal search As String)
        Try
            Dim items = From i In lstLesson Where i.Problem.ToLower.Contains(search.ToLower) _
                                            Or i.Resolution.ToLower.Contains(search.ToLower) _
                                            Or i.Nickname.ToLower.Contains(search.ToLower) _
                                            Or i.ReferenceNo.ToLower.Contains(search.ToLower) _
                                            Or i.ActionNo.ToLower.Contains(search.ToLower)

            Dim searchLessonsLearnt = New ObservableCollection(Of LessonLearnt)(items)

            SetData(searchLessonsLearnt.ToArray)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    'Private Function GetOptionData(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As String
    '    Dim strData As String = String.Empty
    '    Try
    '        _OptionsViewModel = New OptionViewModel
    '        '_OptionsViewModel.Service = client
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
        dgLessonLearnt.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        dgLessonLearnt.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

    Private Sub PermissionSettings()
        Dim guestAccount As Integer = 5

        If profile.Permission_ID = guestAccount Then
            btnAddLessonLearnt.Visibility = Windows.Visibility.Hidden
        End If
    End Sub
#End Region

#Region "Events"
    Private Sub btnAddLessonLearnt_Click(sender As Object, e As RoutedEventArgs) Handles btnAddLessonLearnt.Click
        addframe.Navigate(New LessonLearntAddPage(frame, addframe, menugrid, submenuframe, profile))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(50, 50, 50, 50)
    End Sub

    Private Sub dgLessonLearnt_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgLessonLearnt.MouseDoubleClick
        Try
            ' If user double click the rows
            If dgLessonLearnt.SelectedIndex <> -1 Then
                Dim selectedRowEmpID As Integer = CType(dgLessonLearnt.SelectedItem, LessonLearntModel).EmployeeID

                ' The user will update his/her own lesson learnt details
                If profile.Emp_ID = selectedRowEmpID Then
                    Dim lessonLearnt As New LessonLearntModel

                    lessonLearnt.ReferenceNo = CType(dgLessonLearnt.SelectedItem, LessonLearntModel).ReferenceNo
                    lessonLearnt.Problem = CType(dgLessonLearnt.SelectedItem, LessonLearntModel).Problem
                    lessonLearnt.Resolution = CType(dgLessonLearnt.SelectedItem, LessonLearntModel).Resolution
                    lessonLearnt.ActionNo = CType(dgLessonLearnt.SelectedItem, LessonLearntModel).ActionNo

                    addframe.Navigate(New LessonLearntUpdatePage(frame, addframe, menugrid, submenuframe, lessonLearnt, profile))
                    frame.IsEnabled = False
                    frame.Opacity = 0.3
                    menugrid.IsEnabled = False
                    menugrid.Opacity = 0.3
                    submenuframe.IsEnabled = False
                    submenuframe.Opacity = 0.3
                    addframe.Visibility = Visibility.Visible
                    addframe.Margin = New Thickness(50, 50, 50, 50)
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        SearchProblemEncountered(txtSearch.Text.Trim)
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
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

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
            dialog.PrintTicket.PageOrientation = PageOrientation.Landscape

            Dim pageSize As Size = New Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
            dgLessonLearnt.Measure(pageSize)
            dgLessonLearnt.Arrange(New Rect(5, 5, pageSize.Width, pageSize.Height))
            dialog.PrintVisual(dgLessonLearnt, "Print Lesson Learnt")
        End If
    End Sub

#End Region

#Region "Callback Methods"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region
End Class
