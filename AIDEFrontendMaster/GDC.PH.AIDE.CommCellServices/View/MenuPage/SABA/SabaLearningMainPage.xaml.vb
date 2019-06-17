Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class SabaLearningMainPage
    Implements ServiceReference1.IAideServiceCallback


#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile

    Dim lstSabaLearning As SabaLearning()
    Dim lstSabaLearning2 As SabaLearning()
    Dim SabaLearningListVM As New SabaLearningViewModel()

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
        Me.profile = _profile
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        SetData()
        Me.DataContext = SabaLearningListVM

        If profile.Permission <> "Manager" Then
            btnCreate.Visibility = Windows.Visibility.Collapsed
        End If
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
                lstSabaLearning = _AideService.GetAllSabaCourses(profile.Emp_ID)
                'LoadSabaCourses()
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadSabaCourses()
        Try
            Dim lstSabaLearningList As New ObservableCollection(Of SabaLearningModel)
            Dim sabalearningDBProvider As New SabaLearningDBProvider
            Dim objSabaLearning As New SabaLearning
            Dim percentFinished As String

            For i As Integer = startRowIndex To lastRowIndex

                objSabaLearning = lstSabaLearning(i)
                percentFinished = SetData2(objSabaLearning.SABA_ID)
                sabalearningDBProvider._setlistofitems(objSabaLearning, percentFinished)
            Next

            For Each rawUser As mySabaLearningSet In sabalearningDBProvider._getobjSabaLearning()
                lstSabaLearningList.Add(New SabaLearningModel(rawUser))
            Next

            SabaLearningListVM.ObjectSabaLearningSet = lstSabaLearningList
            'SabaLearningLV.ItemsSource = SabaLearningListVM.ObjectSabaLearningSet
            Me.DataContext = SabaLearningListVM
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadSabaCourseByTitle(ByVal title As String)
        Try
            If InitializeService() Then
                lstSabaLearning = _AideService.GetAllSabaCourseByTitle(title, profile.Emp_ID)
                'LoadSabaCourses()
                SetPaging(PagingMode._First)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Function SetData2(sabaid As Integer) As String
        Try
            Dim Completed As Integer

            If InitializeService() Then
                lstSabaLearning2 = _AideService.GetAllSabaXref(profile.Emp_ID, sabaid)
                Completed = checkCompleted(lstSabaLearning2)
            End If
            Return Completed.ToString() + "%"
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return String.Empty
        End Try

    End Function

    Private Function checkCompleted(sabalist As SabaLearning()) As Integer

        For Each sabaitem As SabaLearning In sabalist
            If Not sabaitem.DATE_COMPLETED = String.Empty Then
                checkCompleted += 1
            End If
        Next

        checkCompleted = (checkCompleted / sabalist.Count) * 100

        Return checkCompleted
    End Function

    Private Sub SetPaging(mode As Integer)
        Try
            Dim totalRecords As Integer = lstSabaLearning.Length

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
                    LoadSabaCourses()
                    Exit Select
                Case CInt(PagingMode._Previous)
                    ' Set the Previous Page if the page index is greater than 1
                    If pagingPageIndex > 1 Then
                        pagingPageIndex -= 1

                        startRowIndex = ((pagingPageIndex * pagingRecordPerPage) - pagingRecordPerPage)
                        lastRowIndex = (pagingPageIndex * pagingRecordPerPage) - 1
                        LoadSabaCourses()
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
                            LoadSabaCourses()
                        Else
                            lastRowIndex = 0
                            Me.DataContext = Nothing
                        End If

                    End If
                    Exit Select
                Case CInt(PagingMode._Last)
                    pagingPageIndex = (lstSabaLearning.Length / pagingRecordPerPage)
                    SetPaging(CInt(PagingMode._Next))
                    Exit Select
            End Select

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try

    End Sub

#End Region

#Region "Events"
    Private Sub SearchTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        If SearchTextBox.Text = String.Empty Then
            SetData()
        Else
            LoadSabaCourseByTitle(SearchTextBox.Text)
        End If
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



                addframe.Navigate(New SabaLearningViewPage(sabalearning, mainframe, addframe, menugrid, submenuframe, profile))
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
        addframe.Navigate(New SabaLearningAddPage(profile, mainframe, addframe, menugrid, submenuframe))
        mainframe.IsEnabled = False
        mainframe.Opacity = 0.3
        menugrid.IsEnabled = False
        menugrid.Opacity = 0.3
        submenuframe.IsEnabled = False
        submenuframe.Opacity = 0.3
        addframe.Visibility = Visibility.Visible
        addframe.Margin = New Thickness(200, 100, 200, 100)
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
