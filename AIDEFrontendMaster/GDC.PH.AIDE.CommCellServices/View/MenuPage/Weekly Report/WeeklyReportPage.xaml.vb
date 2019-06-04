Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Windows
Imports System.Printing

'''''''''''''''''''''''''''''''''
'       JOHN HARVEY SANCHEZ     '
'''''''''''''''''''''''''''''''''
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class WeeklyReportPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private AideServiceClient As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private isEmpty As Boolean
    Private email As String
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private empID As Integer
    Private month As Integer = Date.Now.Month
    Private displayMonth As String

    Dim lstWeekRange As WeekRange()
    Dim lstWeeklyReports As ObservableCollection(Of WeekRangeModel) = New ObservableCollection(Of WeekRangeModel)

    Dim weeklyReportDBProvider As New WeeklyReportDBProvider
    Dim weeklyReportVM As New WeekRangeViewModel()

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

    Public Sub New(_mainFrame As Frame, _empID As Integer, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        InitializeComponent()
        Me.email = _email
        Me.mainFrame = _mainFrame
        Me.empID = _empID
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe

        dgWeeklyReports.ItemsSource = lstWeeklyReports
        SetWeeklyReports()
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            AideServiceClient = New AideServiceClient(Context)
            AideServiceClient.Open()
            bInitialize = True
        Catch ex As SystemException
            AideServiceClient.Abort()
        End Try
        Return bInitialize
    End Function

#End Region

#Region "Functions"

    Public Sub SetWeeklyReports()
        Try
            If InitializeService() Then
                lstWeekRange = AideServiceClient.GetWeeklyReportsByEmpID(empID)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LoadWeeklyReports()
        Try
            Dim objWeeklyReport As New WeekRange()

            For i As Integer = startRowIndex To lastRowIndex
                objWeeklyReport = lstWeekRange(i)
                weeklyReportDBProvider.SetWeekRangeList(objWeeklyReport)
            Next

            For Each weekRange As MyWeekRange In weeklyReportDBProvider.GetWeekRangeList()
                lstWeeklyReports.Add(New WeekRangeModel(weekRange))
            Next

            weeklyReportVM.WeekRangeList = lstWeeklyReports

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

#End Region

#Region "Events"
    Private Sub btnAddReport_Click(sender As Object, e As RoutedEventArgs) Handles btnAddReport.Click
        addframe.Navigate(New WeeklyReportAddPage(mainFrame, empID, email, addframe, menugrid, submenuframe))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Margin = New Thickness(5, 0, 5, 0)
        addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub dgWeeklyReports_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgWeeklyReports.MouseDoubleClick
        If dgWeeklyReports.SelectedIndex <> -1 Then
            If dgWeeklyReports.SelectedItem IsNot Nothing Then
                Dim weekRangeID As Integer
                weekRangeID = CType(dgWeeklyReports.SelectedItem, WeekRangeModel).WeekRangeID

                addframe.Navigate(New WeeklyReportUpdatePage(weekRangeID, empID, mainFrame, email, addframe, menugrid, submenuframe))
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
                menugrid.IsEnabled = False
                menugrid.Opacity = 0.3
                submenuframe.IsEnabled = False
                submenuframe.Opacity = 0.3
                addframe.Visibility = Visibility.Visible
                addframe.Margin = New Thickness(5, 0, 5, 0)
            End If
        End If
    End Sub

#End Region

#Region "Paging"
    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstWeekRange.Length

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
                    LoadWeeklyReports()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadWeeklyReports()
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
                            LoadWeeklyReports()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstWeekRange.Length / pagingRecordPerPage)
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
        If lstWeekRange.Length = 0 Then
            pagingInfo = "No Results Found "
            'GUISettingsOff()
        Else
            pagingInfo = "Displaying " & startRowIndex + 1 & " to " & lastRowIndex + 1
            'GUISettingsOn()
        End If


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
