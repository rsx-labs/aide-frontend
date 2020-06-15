Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports NLog

Class ComcellAddPage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback

#Region "Page Declaration"
    Public _frame As Frame
    'Private aide As AideServiceClient
    Private comcell As New Comcell
    Private ComcellModel As New ComcellModel
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile
    Private nextYear As Integer
    Private mode As String
    Private comcellID As Integer
    Private dsplyByDiv As Integer = 1
    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

#Region "Constructors"
    'Add Constructor
    Public Sub New(_profile As Profile, mainframe As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame)

        _logger.Info("Start : Add CommCell mode=Add")

        Try

            Me._frame = mainframe
            Me._addframe = addframe
            Me._menugrid = menugrid
            Me._submenuframe = submenuframe
            Me.profile = _profile
            InitializeComponent()

            _logger.Info("Load months ...")

            LoadMonth()

            _logger.Info("Load years ...")

            LoadYears()

            _logger.Info("Load nicknames ...")

            LoadEmpNickName()
            mode = "Add"
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Info("End : Add CommCell")
    End Sub
    'Update Constructor
    Public Sub New(_profile As Profile, mainframe As Frame, addframe As Frame, menugrid As Grid, submenuframe As Frame, _comcell As ComcellModel)

        _logger.Info("Start : Add CommCell mode=Update")

        Try
            Me._frame = mainframe
            Me._addframe = addframe
            Me._menugrid = menugrid
            Me._submenuframe = submenuframe
            Me.profile = _profile
            Me.ComcellModel = _comcell
            Me.comcellID = ComcellModel.COMCELL_ID
            InitializeComponent()

            _logger.Info("Load controls ...")

            LoadControls()

            _logger.Info("Load months ...")

            LoadMonth()

            _logger.Info("Load years ...")

            LoadYears()

            _logger.Info("Load nickname ...")

            LoadEmpNickName()
            mode = "Update"
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Info("End : Add CommCell")
    End Sub
#End Region

#Region "Methods/Functions"
    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        aide = New AideServiceClient(Context)
    '        aide.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        aide.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    Public Sub LoadControls()
        txtHeader.Text = "Update Facilitator and Minutes Taker"
        txtBlockMonth.Text = ComcellModel.MONTH
        txtBlockWeek.Text = ComcellModel.WEEK_START
        txtBlockFacilitator.Text = ComcellModel.FACILITATOR_NAME
        txtBlockMinsTaker.Text = ComcellModel.MINUTES_TAKER_NAME
        txtBlockButton.Text = "UPDATE"
        txtBlockYear.Text = ComcellModel.FY_START

        Dim selectedWeek As Integer = ComcellModel.WEEK

        AddBtn.Style = FindResource("RoundCornerUpdate")

        cbMonth.SelectedValue = Helpers.GetMonthNumber(txtBlockMonth.Text)
        cbWeek.SelectedValue = selectedWeek
        cbFacilitator.SelectedValue = txtBlockFacilitator.Text
        cbMinTaker.SelectedValue = txtBlockMinsTaker.Text
        cbYear.SelectedValue = ComcellModel.FY_START.Year
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

    Public Sub LoadWeek()
        cbWeek.DisplayMemberPath = "Text"
        cbWeek.SelectedValuePath = "Value"
        cbWeek.Items.Add(New With {.Text = "First Week", .Value = 1})
        cbWeek.Items.Add(New With {.Text = "Second Week", .Value = 2})
        cbWeek.Items.Add(New With {.Text = "Third Week", .Value = 3})
        cbWeek.Items.Add(New With {.Text = "Fourth Week", .Value = 4})
        cbWeek.Items.Add(New With {.Text = "Fifth Week", .Value = 5})

    End Sub

    Public Sub LoadYears()
        Try
            cbYear.DisplayMemberPath = "Text"
            cbYear.SelectedValuePath = "Value"
            For i As Integer = 2019 To DateTime.Today.Year
                nextYear = i + 1
                cbYear.Items.Add(New With {.Text = i.ToString + "-" + nextYear.ToString, .Value = i})
            Next
        Catch ex As Exception
            _logger.Error(ex.ToString())
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadEmpNickName()
        Try
            'If InitializeService() Then
            Dim lstNickname As Nickname() = AideClient.GetClient().ViewNicknameByDeptID(profile.Email_Address, dsplyByDiv)
            Dim lstNicknameList As New ObservableCollection(Of NicknameModel)
            Dim successRegisterDBProvider As New SuccessRegisterDBProvider
            Dim nicknameVM As New NicknameViewModel()

            For Each objLessonLearnt As Nickname In lstNickname
                successRegisterDBProvider.SetMyNickname(objLessonLearnt)
            Next

            For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
                lstNicknameList.Add(New NicknameModel(rawUser))
            Next

            nicknameVM.NicknameList = lstNicknameList

            cbFacilitator.DataContext = nicknameVM
            cbMinTaker.DataContext = nicknameVM
            'End If
        Catch ex As Exception
            _logger.Error(ex.ToString())
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub AddBtn_Click(sender As Object, e As RoutedEventArgs)
        Try
            'InitializeService()
            If mode = "Add" Then
                If cbMonth.Text = Nothing Or cbFacilitator.Text = Nothing Or cbMinTaker.Text = Nothing Or cbYear.Text = Nothing Then
                    MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                    'ElseIf cbFacilitator.Text = cbMinTaker.Text Then
                    '    MsgBox("Selected facilitator and minutes taker are the same", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                Else
                    comcell.EMP_ID = profile.Emp_ID
                    comcell.MONTH = cbMonth.Text
                    comcell.FACILITATOR = CType(cbFacilitator.SelectedItem, NicknameModel).Emp_ID.ToString()
                    comcell.MINUTES_TAKER = CType(cbMinTaker.SelectedItem, NicknameModel).Emp_ID.ToString()
                    comcell.YEAR = cbYear.SelectedValue
                    comcell.WEEK = cbWeek.SelectedValue

                    AideClient.GetClient().InsertComcellMeeting(comcell)

                    MsgBox("Facilitator and Minutes Taker have been added.", vbOKOnly + MsgBoxStyle.Information, "AIDE")


                    _frame.Navigate(New ComcellMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
                    _frame.IsEnabled = True
                    _frame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                End If
            Else
                If cbMonth.Text = Nothing Or cbFacilitator.Text = Nothing Or cbMinTaker.Text = Nothing Or cbYear.Text = Nothing Then
                    MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                    'ElseIf cbFacilitator.Text = cbMinTaker.Text Then
                    '    MsgBox("Selected facilitator and minutes taker are the same", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
                Else
                    comcell.COMCELL_ID = comcellID
                    comcell.MONTH = cbMonth.Text
                    comcell.FACILITATOR = CType(cbFacilitator.SelectedItem, NicknameModel).Emp_ID.ToString()
                    comcell.MINUTES_TAKER = CType(cbMinTaker.SelectedItem, NicknameModel).Emp_ID.ToString()
                    comcell.YEAR = cbYear.SelectedValue
                    comcell.WEEK = cbWeek.SelectedValue

                    AideClient.GetClient().UpdateComcellMeeting(comcell)

                    MsgBox("Facilitator and Minutes Taker have been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")

                    _frame.Navigate(New ComcellMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
                    _frame.IsEnabled = True
                    _frame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            _logger.Error($"Error at addbtn_click = {ex.ToString()}")
        End Try
    End Sub

    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        _frame.Navigate(New ComcellMainPage(_frame, profile, _addframe, _menugrid, _submenuframe))
        _frame.IsEnabled = True
        _frame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
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

    Private Sub cbMonth_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbMonth.SelectionChanged

        Dim listWeekRange As WeekRange()

        listWeekRange = AideClient.GetClient().GetWeekRangeByMonthYear(
            profile.Emp_ID,
            cbMonth.SelectedValue,
            cbYear.SelectedValue
        )

        cbWeek.DisplayMemberPath = "Text"
        cbWeek.SelectedValuePath = "Value"
        cbWeek.Items.Clear()


        For Each objWeekRange As WeekRange In listWeekRange
            cbWeek.Items.Add(
                New With
                {
                    .Text = $"{objWeekRange.StartWeek.ToString("d")} - {objWeekRange.EndWeek.ToString("d")}",
                    .Value = objWeekRange.WeekRangeID
                }
            )
        Next

    End Sub
#End Region

End Class
