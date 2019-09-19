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
    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
    Dim currentPage As Integer
    Dim lastPage As Integer
#End Region

#Region "Fields"
    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private isEmpty As Boolean
    Private email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile

    Dim lstSuccess As SuccessRegister()
    Dim paginatedCollection As PaginatedObservableCollection(Of SuccessRegisterModel) = New PaginatedObservableCollection(Of SuccessRegisterModel)(pagingRecordPerPage)

#End Region

#Region "Constructor"

    Public Sub New(mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        Me.profile = _profile
        Me.email = profile.Email_Address
        Me.mainFrame = mainFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        SetData()
    End Sub

#End Region

#Region "Events"

    Private Sub btnSRAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnSRAdd.Click
        isEmpty = True
        _addframe.Navigate(New NewSuccessRegister(isEmpty, mainFrame, profile, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 60, 150, 60)
        _addframe.Visibility = Visibility.Visible
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

                _addframe.Navigate(New NewSuccessRegister(successRegister, mainFrame, profile, _addframe, _menugrid, _submenuframe))
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(150, 60, 150, 60)
                _addframe.Visibility = Visibility.Visible
            End If
        End If

    End Sub

    Private Sub txtSRSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSRSearch.TextChanged
        e.Handled = True
        SetDataForSearch(txtSRSearch.Text, email)
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

#End Region

#Region "Functions"

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstSuccess = _AideService.ViewSuccessRegisterAll(email)
                btnPrint.Visibility = Windows.Visibility.Visible
                LoadData()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadData()
        Try
            paginatedCollection.Clear()
            Dim lstSuccessRegister As New ObservableCollection(Of SuccessRegisterModel)
            Dim successRegisterDBProvider As New SuccessRegisterDBProvider

            ' Set the MyLessonLearntList 
            For Each objSuccessRegister As SuccessRegister In lstSuccess
                successRegisterDBProvider.SetMySuccessRegister(objSuccessRegister)
            Next

            For Each lessonLearnt As MySuccessRegister In successRegisterDBProvider.GetMySuccessRegister()
                paginatedCollection.Add(New SuccessRegisterModel(lessonLearnt))
            Next

            lv_successRegisterAll.ItemsSource = paginatedCollection
            'LoadDataForPrint()
            currentPage = paginatedCollection.CurrentPage + 1
            lastPage = Math.Ceiling(lstSuccess.Length / pagingRecordPerPage)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String, email As String)
        Try
            If InitializeService() Then
                lstSuccess = _AideService.ViewSuccessRegisterBySearch(input, email)
                LoadData()
                DisplayPagingInfo()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            _AideService.Abort()
        End Try
        Return bInitialize
    End Function

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstSuccess.Length

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
                    LoadData()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadData()
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
                            LoadData()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstSuccess.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

            'DisplayPagingInfo()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub DisplayPagingInfo()
        ' If there has no data found
        If lstSuccess.Length = 0 Then
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

    Private Sub btnFirst_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._First))
    End Sub

    Private Sub btnLast_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Last))
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
