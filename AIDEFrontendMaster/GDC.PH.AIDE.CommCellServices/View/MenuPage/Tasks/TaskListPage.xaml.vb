Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class TaskListPage
    Implements ServiceReference1.IAideServiceCallback


#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private mainWindow As MainWindow
    Public empID As Integer
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private isEmpty As Boolean
    Public email As String
    Dim lstTask As Tasks()

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10
#End Region

#Region "Constructor"


    Public Sub New(_frame As Frame, _mainWindow As MainWindow, _empID As Integer, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me.empID = _empID
        Me.mainFrame = _frame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.mainWindow = _mainWindow
        Me.email = _email
        SetData()
    End Sub

#End Region

#Region "Events"

    Private Sub lv_taskList_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lv_taskList.MouseDoubleClick
        e.Handled = True

        Dim rawStatus As String
        Dim rawPhase As String
        Dim tasksListDBProvider As New TaskDBProvider

        If lv_taskList.SelectedIndex <> -1 Then
            If lv_taskList.SelectedItem IsNot Nothing Then

                Dim taskList As New TasksModel

                taskList.EmpID = CType(lv_taskList.SelectedItem, TasksModel).EmpId
                taskList.TaskID = CType(lv_taskList.SelectedItem, TasksModel).TaskId
                taskList.IncId = CType(lv_taskList.SelectedItem, TasksModel).IncId
                taskList.IncDescr = CType(lv_taskList.SelectedItem, TasksModel).IncDescr
                taskList.DateStarted = CType(lv_taskList.SelectedItem, TasksModel).DateStarted
                taskList.DateCreated = CType(lv_taskList.SelectedItem, TasksModel).DateCreated
                taskList.TargetDate = CType(lv_taskList.SelectedItem, TasksModel).TargetDate
                taskList.EffortEst = CType(lv_taskList.SelectedItem, TasksModel).EffortEst
                taskList.ActEffortEstWk = CType(lv_taskList.SelectedItem, TasksModel).ActEffortEstWk
                taskList.ActEffortEst = CType(lv_taskList.SelectedItem, TasksModel).ActEffortEst
                rawPhase = tasksListDBProvider.SetPhaseDesc(CType(lv_taskList.SelectedItem, TasksModel).Phase)
                rawStatus = tasksListDBProvider.SetStatusDesc(CType(lv_taskList.SelectedItem, TasksModel).Status)
                taskList.ProjId = CType(lv_taskList.SelectedItem, TasksModel).ProjId
                taskList.Remarks = CType(lv_taskList.SelectedItem, TasksModel).Remarks
                taskList.Rework = CType(lv_taskList.SelectedItem, TasksModel).Rework
                taskList.TaskType = CType(lv_taskList.SelectedItem, TasksModel).TaskType
                taskList.Phase = rawPhase
                taskList.Status = rawStatus

                _addframe.Navigate(New TaskAddPage(mainFrame, mainWindow, taskList, email, _addframe, _menugrid, _submenuframe, Me.empID))
                _addframe.Margin = New Thickness(100, 50, 100, 50)
                _addframe.IsEnabled = True
                _addframe.Visibility = Visibility.Visible
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
            End If
        End If
    End Sub

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        mainFrame.Navigate(New TaskAdminPage(mainFrame, mainWindow, empID, email, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

    'Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
    '    Dim dialog As PrintDialog = New PrintDialog()
    '    If dialog.ShowDialog() = True Then
    '        dialog.PrintTicket.PageOrientation = PageOrientation.Landscape
    '        dialog.PrintVisual(lv_contacts, "My Canvas")
    '    End If
    'End Sub
#End Region

#Region "Functions"

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstTask = _AideService.GetTaskDetailByIncidentId(empID)
                If lstTask.Count <> 0 Then
                    SetPaging(PagingMode._First)
                Else
                    lbl_noOT.Visibility = Windows.Visibility.Visible
                    lbl_noOT1.Visibility = Windows.Visibility.Visible
                    lbl_noOT2.Visibility = Windows.Visibility.Visible
                    lv_taskList.Visibility = Windows.Visibility.Collapsed
                    btnPrint.Visibility = Windows.Visibility.Collapsed
                    btnNext.Visibility = Windows.Visibility.Collapsed
                    btnPrev.Visibility = Windows.Visibility.Collapsed

                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstTaskList As New ObservableCollection(Of TasksModel)
            Dim tasksListDBProvider As New TaskDBProvider
            Dim taskListVM As New TasksViewModel()

            Dim objTasks As New Tasks()

            For i As Integer = startRowIndex To lastRowIndex
                objTasks = lstTask(i)
                tasksListDBProvider.SetTaskList(objTasks)
            Next

            For Each rawUser As MyTasks In tasksListDBProvider.GetTaskList()
                lstTaskList.Add(New TasksModel(rawUser))
            Next

            taskListVM.TaskList = lstTaskList
            Me.DataContext = taskListVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
        End Try
        Return bInitialize
    End Function

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstTask.Length

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
                    pagingPageIndex = (lstTask.Length / pagingRecordPerPage)
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
        If lstTask.Length = 0 Then
            pagingInfo = "No Results Found "
            GUISettingsOff()
        Else
            pagingInfo = "Displaying " & startRowIndex + 1 & " to " & lastRowIndex + 1
            GUISettingsOn()
        End If

        lblPagingInfo.Content = pagingInfo

    End Sub

    Private Sub GUISettingsOff()
        lv_taskList.Visibility = Windows.Visibility.Hidden

        btnPrev.IsEnabled = False
        btnNext.IsEnabled = False
    End Sub

    Private Sub GUISettingsOn()
        lv_taskList.Visibility = Windows.Visibility.Visible

        btnPrev.IsEnabled = True
        btnNext.IsEnabled = True
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
