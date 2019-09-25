Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class ComcellMainPage
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
    Dim year As Integer = Date.Now.Year
    Dim startYear As Integer = 2019 'Default Start Year
    Dim lstComcell As Comcell()
    Dim ComcellVM As New ComcellViewModel()



#End Region

#Region "Paging Declarations"
    Dim startRowIndex As Integer
    Dim lastRowIndex As Integer
    Dim pagingPageIndex As Integer
    Dim pagingRecordPerPage As Integer = 12

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
        Me.DataContext = ComcellVM
        Me.profile = _profile

        If profile.Permission_ID = 1 Then
            btnCreate.Visibility = Windows.Visibility.Visible
        End If

        year = Date.Now.Year
        cbYear.SelectedValue = year
        LoadYears()
        SetData()

    End Sub

#End Region

#Region "Functions"

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
                lstComcell = _AideService.GetComcellMeeting(empID, year)
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadComcell()
        Try
            Dim lstComcellList As New ObservableCollection(Of ComcellModel)
            Dim ComcellDBProvider As New ComcellDBProvider
            Dim objComcell As New Comcell


            For i As Integer = startRowIndex To lastRowIndex
                objComcell = lstComcell(i)
                ComcellDBProvider.SetMyComcell(objComcell)
            Next

            For Each rawUser As MyComcell In ComcellDBProvider.GetMyComcell()
                lstComcellList.Add(New ComcellModel(rawUser))
            Next

            ComcellVM.ComcellList = lstComcellList

            Me.DataContext = ComcellVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = startYear To DateTime.Today.Year
                Dim nextYear As Integer = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstComcell.Length

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
                    LoadComcell()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadComcell()
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
                            LoadComcell()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstComcell.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

#End Region

#Region "Events"
    Private Sub ComcellLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If ComcellLV.SelectedIndex <> -1 Then
            If ComcellLV.SelectedItem IsNot Nothing Then
                Dim comcell As New ComcellModel
                comcell.MONTH = CType(ComcellLV.SelectedItem, ComcellModel).MONTH
                comcell.FACILITATOR = CType(ComcellLV.SelectedItem, ComcellModel).FACILITATOR
                comcell.MINUTES_TAKER = CType(ComcellLV.SelectedItem, ComcellModel).MINUTES_TAKER
                comcell.FACILITATOR_NAME = CType(ComcellLV.SelectedItem, ComcellModel).FACILITATOR_NAME
                comcell.MINUTES_TAKER_NAME = CType(ComcellLV.SelectedItem, ComcellModel).MINUTES_TAKER_NAME
                comcell.COMCELL_ID = CType(ComcellLV.SelectedItem, ComcellModel).COMCELL_ID
                comcell.FY_START = CType(ComcellLV.SelectedItem, ComcellModel).FY_START

                addframe.Navigate(New ComcellAddPage(profile, mainframe, addframe, menugrid, submenuframe, comcell))
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
        addframe.Navigate(New ComcellAddPage(profile, mainframe, addframe, menugrid, submenuframe))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(150, 60, 150, 60)
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        year = cbYear.SelectedValue
        SetData()
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
