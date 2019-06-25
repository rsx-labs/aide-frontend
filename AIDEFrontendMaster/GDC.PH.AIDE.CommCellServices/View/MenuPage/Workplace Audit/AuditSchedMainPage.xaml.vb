Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class AuditSchedMainPage
    Implements ServiceReference1.IAideServiceCallback


#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    Private year As Integer
    Private nextYear As Integer

    Dim lstauditSched As AuditSched()
    Dim auditSchedVM As New AuditSchedViewModel()



#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 10

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum
#End Region

#Region "Constructor"

    Public Sub New(_mainframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me.empID = _profile.Emp_ID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.DataContext = auditSchedVM
        Me.profile = _profile

        If profile.Permission = "Manager" Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If

        year = Date.Now.Year
        SetData()
        LoadYears()

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
        End Try
        Return bInitialize
    End Function

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstauditSched = _AideService.GetAuditSched(empID, year)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadauditSched()
        Try
            Dim lstauditSchedList As New ObservableCollection(Of AuditSchedModel)
            Dim auditSchedDBProvider As New AuditSchedDBProvider
            Dim objauditSched As New AuditSched


            For i As Integer = startRowIndex To lastRowIndex
                objauditSched = lstauditSched(i)
                auditSchedDBProvider.SetMyAuditSched(objauditSched)
            Next

            For Each rawUser As MyAuditSched In auditSchedDBProvider.GetMyAuditSched()
                lstauditSchedList.Add(New AuditSchedModel(rawUser))
            Next

            auditSchedVM.AuditSchedList = lstauditSchedList

            Me.DataContext = auditSchedVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                nextYear = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
            lblYear.Text = "FY: " + year.ToString + " - " + nextYear.ToString
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstauditSched.Length

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
                    LoadauditSched()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadauditSched()
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
                            LoadauditSched()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstauditSched.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

#End Region

#Region "Events"
    Private Sub AuditSched_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If AuditSchedLV.SelectedIndex <> -1 Then
            If AuditSchedLV.SelectedItem IsNot Nothing Then
                Dim auditSched As New AuditSchedModel
                auditSched.PERIOD_START = CType(AuditSchedLV.SelectedItem, AuditSchedModel).PERIOD_START
                auditSched.PERIOD_END = CType(AuditSchedLV.SelectedItem, AuditSchedModel).PERIOD_END
                auditSched.DAILY = CType(AuditSchedLV.SelectedItem, AuditSchedModel).DAILY
                auditSched.WEEKLY = CType(AuditSchedLV.SelectedItem, AuditSchedModel).WEEKLY
                auditSched.MONTHLY = CType(AuditSchedLV.SelectedItem, AuditSchedModel).MONTHLY
                auditSched.AUDIT_SCHED_ID = CType(AuditSchedLV.SelectedItem, AuditSchedModel).AUDIT_SCHED_ID
                auditSched.FY_START = CType(AuditSchedLV.SelectedItem, AuditSchedModel).FY_START

                addframe.Navigate(New AuditSchedAddPage(profile, mainframe, addframe, menugrid, submenuframe, auditSched))
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

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        addframe.Navigate(New AuditSchedAddPage(profile, mainframe, addframe, menugrid, submenuframe))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(200, 100, 200, 100)
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        year = cbYear.SelectedValue
        SetData()
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        SetPaging(CInt(PagingMode._Next))
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        SetPaging(CInt(PagingMode._Previous))
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
