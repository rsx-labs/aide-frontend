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

    Public frame As Frame
    Public email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
#End Region

    Dim lstLesson As LessonLearnt()
    Dim client As AideServiceClient

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

    Public Sub New(_frame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)
        InitializeComponent()
        frame = _frame
        email = _email
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.profile = _profile
        LoadLessonLearntList()
    End Sub

#Region "Common Methods"

    Private Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

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

#Region "Private Functions"

    Private Sub LoadLessonLearntList()
        Try
            If Me.InitializeService Then
                lstLesson = client.GetLessonLearntList(profile.Email_Address)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub SearchProblemEncountered(ByVal search As String)
        Try
            If Me.InitializeService() Then
                lstLesson = client.GetLessonLearntByProblem(search, profile.Email_Address)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub SetLists()
        Try
            Dim lstLessonLearnt As New ObservableCollection(Of LessonLearntModel)
            Dim lessonLearntDBProvider As New LessonLearntDBProvider
            Dim lessonLearntViewModel As New LessonLearntViewModel

            Dim objLessonLearnt As New LessonLearnt()

            ' Set the MyLessonLearntList 
            For i As Integer = startRowIndex To lastRowIndex
                objLessonLearnt = lstLesson(i)
                lessonLearntDBProvider.SetLessonLearntList(objLessonLearnt)
            Next

            ' Set the lstLessonLearnt
            For Each iLessonLearnt As MyLessonLearntList In lessonLearntDBProvider.GetLessonLearntList()
                lstLessonLearnt.Add(New LessonLearntModel(iLessonLearnt))
            Next

            lessonLearntViewModel.LessonLearntList = lstLessonLearnt

            ' Display the data using binding
            Me.DataContext = lessonLearntViewModel
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub btnAddLessonLearnt_Click(sender As Object, e As RoutedEventArgs) Handles btnAddLessonLearnt.Click
        _addframe.Navigate(New LessonLearntAddPage(frame, email, _addframe, _menugrid, _submenuframe, profile))
        frame.IsEnabled = False
        frame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Visibility = Visibility.Visible
        _addframe.Margin = New Thickness(200, 80, 200, 80)
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

                    _addframe.Navigate(New LessonLearntUpdatePage(frame, lessonLearnt, profile, email, _menugrid, _submenuframe, _addframe))
                    frame.IsEnabled = False
                    frame.Opacity = 0.3
                    _menugrid.IsEnabled = False
                    _menugrid.Opacity = 0.3
                    _submenuframe.IsEnabled = False
                    _submenuframe.Opacity = 0.3
                    _addframe.Visibility = Visibility.Visible
                    _addframe.Margin = New Thickness(200, 80, 200, 80)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        SearchProblemEncountered(txtSearch.Text.Trim)
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstLesson.Length

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
                    SetLists()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        SetLists()
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
                            SetLists()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstLesson.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

            DisplayPagingInfo()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

    Private Sub DisplayPagingInfo()
        Dim pagingInfo As String

        ' If there has no data found
        If lstLesson.Length = 0 Then
            pagingInfo = "No Results Found "
            GUISettingsOff()
        Else
            pagingInfo = "Displaying " & startRowIndex + 1 & " to " & lastRowIndex + 1
            GUISettingsOn()
        End If



    End Sub

    Private Sub GUISettingsOff()


        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()


        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
    End Sub

#End Region

#Region "Buttons"
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

End Class
