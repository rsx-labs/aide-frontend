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

#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private isEmpty As Boolean
    Private email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Dim lstSuccess As SuccessRegister()

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

#End Region

#Region "Constructor"

    Public Sub New(mainFrame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me.email = _email
        Me.mainFrame = mainFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
    End Sub

#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
#End Region

#Region "Events"

    Private Sub btnSRAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnSRAdd.Click
        isEmpty = True
        _addframe.Navigate(New NewSuccessRegister(isEmpty, mainFrame, email, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(150, 100, 150, 100)
        _addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub lv_successRegisterOwn_MouseDoubleClick(sender As Object, e As SelectionChangedEventArgs) Handles lv_successRegisterOwn.SelectionChanged
        'mainFrame.Navigate(New NewSuccessRegister(lv_successRegisterOwn, mainFrame))
        e.Handled = True
        If lv_successRegisterOwn.SelectedIndex <> -1 Then

            If lv_successRegisterOwn.SelectedItem IsNot Nothing Then
                Dim successRegister As New SuccessRegisterModel

                successRegister.SuccessID = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).SuccessID
                successRegister.Emp_ID = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).Emp_ID
                successRegister.Nick_Name = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).Nick_Name
                successRegister.DetailsOfSuccess = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).DetailsOfSuccess
                successRegister.WhosInvolve = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).WhosInvolve
                successRegister.AdditionalInformation = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).AdditionalInformation
                successRegister.DateInput = CType(lv_successRegisterOwn.SelectedItem, SuccessRegisterModel).DateInput

                _addframe.Navigate(New NewSuccessRegister(successRegister, mainFrame, email, _addframe, _menugrid, _submenuframe))
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(150, 100, 150, 100)
                _addframe.Visibility = Visibility.Visible
            End If
        End If

    End Sub

    Private Sub SR_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SR.SelectionChanged
        e.Handled = True
        Me.DataContext = Nothing
        SetData()
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
                If SR.SelectedIndex = 0 Then
                    lstSuccess = _AideService.ViewSuccessRegisterByEmpID(email)
                    btnPrint.Visibility = Windows.Visibility.Hidden
                Else
                    lstSuccess = _AideService.ViewSuccessRegisterAll(email)
                    btnPrint.Visibility = Windows.Visibility.Visible
                End If
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstSuccessRegister As New ObservableCollection(Of SuccessRegisterModel)
            Dim successRegisterDBProvider As New SuccessRegisterDBProvider
            Dim successRegisterVM As New SuccessRegisterViewModel()

            Dim objSuccessRegister As New SuccessRegister()

            ' Set the MyLessonLearntList 
            For i As Integer = startRowIndex To lastRowIndex
                objSuccessRegister = lstSuccess(i)
                successRegisterDBProvider.SetMySuccessRegister(objSuccessRegister)
            Next

            For Each rawUser As MySuccessRegister In successRegisterDBProvider.GetMySuccessRegister()
                lstSuccessRegister.Add(New SuccessRegisterModel(rawUser))
            Next

            successRegisterVM.SuccessRegisterList = lstSuccessRegister
            Me.DataContext = successRegisterVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetDataForSearch(input As String, email As String)
        Try
            If SR.SelectedIndex = 0 Then
                Exit Sub
            Else
                If InitializeService() Then
                    lstSuccess = _AideService.ViewSuccessRegisterBySearch(input, email)
                    SetPaging(PagingMode._First)
                End If
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
        Dim pagingInfo As String

        ' If there has no data found
        If lstSuccess.Length = 0 Then
            pagingInfo = "No Results Found "
            GUISettingsOff()
        Else
            pagingInfo = "Displaying " & startRowIndex + 1 & " to " & lastRowIndex + 1
            GUISettingsOn()
        End If



    End Sub

    Private Sub GUISettingsOff()
        lv_successRegisterOwn.Visibility = Windows.Visibility.Hidden
        lv_successRegisterAll.Visibility = Windows.Visibility.Hidden

        btnPrev1.IsEnabled = False
        btnNext1.IsEnabled = False

        btnPrev2.IsEnabled = False
        btnNext2.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_successRegisterOwn.Visibility = Windows.Visibility.Visible
        lv_successRegisterAll.Visibility = Windows.Visibility.Visible

        btnPrev1.IsEnabled = True
        btnNext1.IsEnabled = True

        btnPrev2.IsEnabled = True
        btnNext2.IsEnabled = True
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Next))
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs)
        SetPaging(CInt(PagingMode._Previous))
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
