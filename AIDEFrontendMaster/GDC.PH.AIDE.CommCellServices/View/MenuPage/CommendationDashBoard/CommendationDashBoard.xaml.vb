Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
Imports System.ComponentModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class CommendationDashBoard
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private email As String
    Private profile As Profile
    Private isEmpty As Boolean
    Private position As String
    Private empID As Integer
    Private commendFrame As Frame
    Private month As Integer = Date.Now.Month
    Private year As Integer = Date.Now.Year
    Private displayMonth As String


    Dim lstBirthdayMonth As BirthdayList()
    Dim lstCommendation As Commendations()
    Dim birthdayListVM As New BirthdayListViewModel()
    Dim commendationVM As New CommendationViewModel()
    Dim startYear As Integer = 2018 'Default Start Year

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

#End Region

#Region "Constructor"

    Public Sub New(mainFrame As Frame, _position As String, _empID As Integer,
                   _addFrame As Frame, _menuGrid As Grid, _subMenuFrame As Frame, _email As String, _profile As Profile, _commendFrame As Frame)

        InitializeComponent()
        Me.position = _position
        Me.empID = _empID
        Me.mainFrame = mainFrame
        Me._addframe = _addFrame
        Me._menugrid = _menuGrid
        Me._submenuframe = _subMenuFrame
        Me.commendFrame = _commendFrame
        Me.email = _email
        Me.profile = _profile
        SetButtonCreateVisible()
        SetData()
        Me.DataContext = commendationVM
        LoadMonth()
        LoadYears()
    End Sub

#End Region

#Region "Methods/Functions"

    Public Sub SetButtonCreateVisible()
        If profile.Permission_ID = 1 Then
            btnCreate.Visibility = Windows.Visibility.Visible
        Else
            btnCreate.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Public Sub SetData()
        Try
            If InitializeService() Then
                lstCommendation = _AideService.GetCommendations(empID)
                LoadCommendations()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                Dim nextYear As Integer = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadMonth()
        cbMonth.DisplayMemberPath = "Text"
        cbMonth.SelectedValuePath = "Value"
        cbMonth.Items.Add(New With {.Text = "January", .Value = 1})
        cbMonth.Items.Add(New With {.Text = "February", .Value = 2})
        cbMonth.Items.Add(New With {.Text = "March", .Value = 3})
        cbMonth.Items.Add(New With {.Text = "April", .Value = 4})
        cbMonth.Items.Add(New With {.Text = "May", .Value = 5})
        cbMonth.Items.Add(New With {.Text = "June", .Value = 6})
        cbMonth.Items.Add(New With {.Text = "July", .Value = 7})
        cbMonth.Items.Add(New With {.Text = "August", .Value = 8})
        cbMonth.Items.Add(New With {.Text = "September", .Value = 9})
        cbMonth.Items.Add(New With {.Text = "October", .Value = 10})
        cbMonth.Items.Add(New With {.Text = "November", .Value = 11})
        cbMonth.Items.Add(New With {.Text = "December", .Value = 12})
    End Sub

    Public Sub LoadCommendations()
        Try
            Dim lstCommendationList As New ObservableCollection(Of CommendationModel)
            Dim commendationsDBProvider As New CommendationDBProvider

            For Each objCommendation As Commendations In lstCommendation
                commendationsDBProvider.SetMyCommendations(objCommendation)
            Next

            For Each rawUser As MyCommendations In commendationsDBProvider.GetMyCommendations()
                lstCommendationList.Add(New CommendationModel(rawUser))
            Next

            commendationVM.CommendationList = lstCommendationList
            CommendationLV.ItemsSource = commendationVM.CommendationList
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub LoadCommendationsBySearch()
        Try
            InitializeService()
            lstCommendation = _AideService.GetCommendationsBySearch(empID, month, year)
            Dim lstCommendationList As New ObservableCollection(Of CommendationModel)
            Dim commendationsDBProvider As New CommendationDBProvider

            For Each objCommendation As Commendations In lstCommendation
                commendationsDBProvider.SetMyCommendations(objCommendation)
            Next

            For Each rawUser As MyCommendations In commendationsDBProvider.GetMyCommendations()
                lstCommendationList.Add(New CommendationModel(rawUser))
            Next

            commendationVM.CommendationList = lstCommendationList
            CommendationLV.ItemsSource = commendationVM.CommendationList
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
#End Region

#Region "Events"
    Private Sub CommendationLV_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If CommendationLV.SelectedIndex <> -1 Then
            Dim commmendationList As New CommendationModel
            If CommendationLV.SelectedItem IsNot Nothing Then
                For Each _comm As CommendationModel In commendationVM.CommendationList
                    If CType(CommendationLV.SelectedItem, CommendationModel).CommendID = _comm.CommendID Then
                        commmendationList.CommendID = _comm.CommendID
                        commmendationList.EMP_ID = _comm.EMP_ID
                        commmendationList.DateSent = _comm.DateSent
                        commmendationList.Employees = _comm.Employees
                        commmendationList.Project = _comm.Project
                        commmendationList.Reason = _comm.Reason
                        commmendationList.SentBy = _comm.SentBy
                    End If
                Next

                _addframe.Navigate(New CommendationUpdatePage(commmendationList, mainFrame, position, empID, _addframe, _menugrid, _submenuframe, Me.profile.Permission, Me.commendFrame, Me.profile))
                mainFrame.IsEnabled = False
                mainFrame.Opacity = 0.3
                _menugrid.IsEnabled = False
                _menugrid.Opacity = 0.3
                _submenuframe.IsEnabled = False
                _submenuframe.Opacity = 0.3
                _addframe.Margin = New Thickness(140, 70, 140, 70)
                _addframe.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        _addframe.Navigate(New CommendationAddPage(mainFrame, position, empID, _addframe, _menugrid, _submenuframe, Me.email, Me.profile, Me.commendFrame))
        mainFrame.IsEnabled = False
        mainFrame.Opacity = 0.3
        _menugrid.IsEnabled = False
        _menugrid.Opacity = 0.3
        _submenuframe.IsEnabled = False
        _submenuframe.Opacity = 0.3
        _addframe.Margin = New Thickness(140, 70, 140, 70)
        _addframe.Visibility = Visibility.Visible
    End Sub

    Private Sub cbMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbMonth.SelectionChanged
        month = cbMonth.SelectedValue
        LoadCommendationsBySearch()
    End Sub

    Private Sub cbYear_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbYear.SelectionChanged
        year = cbYear.SelectedValue
        LoadCommendationsBySearch()
    End Sub
#End Region

#Region "INotify Methods"
    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate
        Throw New NotImplementedException()
    End Sub
#End Region

End Class
