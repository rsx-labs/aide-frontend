Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class HomePage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private isEmpty As Boolean
    Private position As String
    Private empID As Integer
    Private month As Integer = Date.Now.Month
    Private displayMonth As String
    Private MANAGER As String = "Manager"

    Dim lstBirthdayMonth As BirthdayList()
    Dim lstCommendation As Commendations()
    Dim birthdayListVM As New BirthdayListViewModel()
    Dim commendationVM As New CommendationViewModel()

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

#End Region

#Region "Constructor"

    Public Sub New(mainFrame As Frame, _position As String, _empID As Integer, _addFrame As Frame, _menuGrid As Grid, _subMenuFrame As Frame)

        InitializeComponent()
        Me.position = _position
        Me.empID = _empID
        Me.mainFrame = mainFrame
        Me._addframe = _addFrame
        Me._menugrid = _menuGrid
        Me._submenuframe = _subMenuFrame
        'SetButtonCreateVisible()
        'SetData()
        Me.DataContext = commendationVM
    End Sub

#End Region

#Region "Events"

    'Private Sub lv_commendation_MouseDoubleClick(sender As Object, e As SelectionChangedEventArgs) Handles lv_commendation.SelectionChanged
    '    'mainFrame.Navigate(New NewSuccessRegister(lv_successRegisterOwn, mainFrame))
    '    e.Handled = True
    '    If lv_commendation.SelectedIndex <> -1 Then
    '        Dim commmendationList As New CommendationModel
    '        If lv_commendation.SelectedItem IsNot Nothing Then
    '            For Each _comm As CommendationModel In commendationVM.CommendationList
    '                If CType(lv_commendation.SelectedItem, CommendationModel).CommendID = _comm.CommendID Then
    '                    commmendationList.CommendID = _comm.CommendID
    '                    commmendationList.Dept_ID = _comm.Dept_ID
    '                    commmendationList.DateSent = _comm.DateSent
    '                    commmendationList.Employees = _comm.Employees
    '                    commmendationList.Project = _comm.Project
    '                    commmendationList.Reason = _comm.Reason
    '                    commmendationList.SentBy = _comm.SentBy
    '                End If
    '            Next
    '            _addframe.Navigate(New CommendationAddPage(commmendationList, mainFrame, position, empID, _addframe, _menugrid, _submenuframe))
    '            mainFrame.IsEnabled = False
    '            mainFrame.Opacity = 0.3
    '            _menugrid.IsEnabled = False
    '            _menugrid.Opacity = 0.3
    '            _submenuframe.IsEnabled = False
    '            _submenuframe.Opacity = 0.3
    '            _addframe.Margin = New Thickness(280, 0, 280, 0)
    '            _addframe.Visibility = Visibility.Visible
    '        End If
    '    End If
    'End Sub

    'Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
    '    _addframe.Navigate(New CommendationAddPage(mainFrame, position, empID, _addframe, _menugrid, _submenuframe))
    '    mainFrame.IsEnabled = False
    '    mainFrame.Opacity = 0.3
    '    _menugrid.IsEnabled = False
    '    _menugrid.Opacity = 0.3
    '    _submenuframe.IsEnabled = False
    '    _submenuframe.Opacity = 0.3
    '    _addframe.Margin = New Thickness(280, 0, 280, 0)
    '    _addframe.Visibility = Visibility.Visible
    'End Sub
#End Region

#Region "Functions"

    'Public Sub SetData()
    '    Try
    '        If InitializeService() Then
    '            lstCommendation = _AideService.GetCommendations(empID)
    '            LoadCommendations()
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show(ex.Message)
    '    End Try
    'End Sub

    'Public Sub LoadCommendations()
    '    Try
    '        Dim lstCommendationList As New ObservableCollection(Of CommendationModel)
    '        Dim commendationsDBProvider As New CommendationDBProvider

    '        For Each objCommendation As Commendations In lstCommendation
    '            commendationsDBProvider.SetMyCommendations(objCommendation)
    '        Next

    '        For Each rawUser As MyCommendations In commendationsDBProvider.GetMyCommendations()
    '            lstCommendationList.Add(New CommendationModel(rawUser))
    '        Next

    '        commendationVM.CommendationList = lstCommendationList
    '        lv_commendation.ItemsSource = commendationVM.CommendationList
    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
    '    End Try
    'End Sub

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        _AideService = New AideServiceClient(Context)
    '        _AideService.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        _AideService.Abort()
    '    End Try
    '    Return bInitialize
    'End Function

    'Public Sub SetButtonCreateVisible()
    '    If position = MANAGER Then
    '        btnCreate.Visibility = Windows.Visibility.Visible
    '    Else
    '        btnCreate.Visibility = Windows.Visibility.Collapsed
    '    End If
    'End Sub
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
