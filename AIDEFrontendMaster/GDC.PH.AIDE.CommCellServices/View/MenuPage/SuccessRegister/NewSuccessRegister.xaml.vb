Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class NewSuccessRegister
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private mainFrame As Frame
    Private client As ServiceReference1.AideServiceClient
    Private successRegister As New SuccessRegisterModel
    Private email As String
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    'Private srmodel As SuccessRegisterModel

#End Region

#Region "Constructor"

    Public Sub New(isEmpty As Boolean, mainFrame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me.email = _email
        Me.mainFrame = mainFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        btnSRCreate.Visibility = System.Windows.Visibility.Visible
        btnSRUpdate.Visibility = System.Windows.Visibility.Hidden
        btnSRDelete.Visibility = System.Windows.Visibility.Hidden
        comboRaisedBy.Visibility = System.Windows.Visibility.Visible
        txtRaisedBy.Visibility = System.Windows.Visibility.Hidden
        tbSuccessForm.Text = "CREATE SUCCESS REGISTER"
        AssignEvents()
        PopulateComboBox()
    End Sub

    Public Sub New(_successRegister As SuccessRegisterModel, mainFrame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()

        Me.mainFrame = mainFrame
        Me.email = _email
        Me.successRegister = _successRegister
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        tbSuccessForm.Text = "UPDATE SUCCESS REGISTER"
        LoadData()
        AssignEvents()
        PopulateComboBox()
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub btnSRUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnSRUpdate.Click
        Try
            e.Handled = True
            Dim SuccessRegisters As New SuccessRegister
            If txtSRDetails.Text = String.Empty AndAlso txtSRAdditional.Text = String.Empty AndAlso txtSRWhosInvolve.Text = String.Empty Then
                MsgBox("Please Fill up the Fields!", MsgBoxStyle.Exclamation, "AIDE")
            Else
                SuccessRegisters.SuccessID = txtSRID.Text
                SuccessRegisters.Emp_ID = comboRaisedBy.SelectedValue
                SuccessRegisters.DateInput = dateInput.SelectedDate
                SuccessRegisters.DetailsOfSuccess = txtSRDetails.Text
                SuccessRegisters.WhosInvolve = txtSRWhosInvolve.Text
                SuccessRegisters.AdditionalInformation = txtSRAdditional.Text
                client.UpdateSuccessRegisterByEmpID(SuccessRegisters)
                ClearFields()
                _addframe.Navigate(New SuccessRegisterPage(mainFrame, email, _addframe, _menugrid, _submenuframe))
                mainFrame.IsEnabled = True
                mainFrame.Opacity = 1
                _menugrid.IsEnabled = True
                _menugrid.Opacity = 1
                _submenuframe.IsEnabled = True
                _submenuframe.Opacity = 1

                _addframe.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub btnSRCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnSRCreate.Click
        Try
            e.Handled = True
            Dim SuccessRegisters As New SuccessRegister
            If txtSRDetails.Text = String.Empty Or dateInput.Text = String.Empty Or comboRaisedBy.SelectedValue = Nothing Or txtSRWhosInvolve.Text = String.Empty Then
                MsgBox("Please Fill up the Fields!", MsgBoxStyle.Exclamation, "AIDE")
            Else
                SuccessRegisters.Emp_ID = comboRaisedBy.SelectedValue
                SuccessRegisters.DateInput = dateInput.SelectedDate
                SuccessRegisters.DetailsOfSuccess = txtSRDetails.Text
                SuccessRegisters.WhosInvolve = txtSRWhosInvolve.Text
                SuccessRegisters.AdditionalInformation = txtSRAdditional.Text
                Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
                If result = 1 Then
                    client.CreateNewSuccessRegister(SuccessRegisters)
                    ClearFields()
                    _addframe.Navigate(New SuccessRegisterPage(mainFrame, email, _addframe, _menugrid, _submenuframe))
                    mainFrame.IsEnabled = True
                    mainFrame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                Else
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub btnSRCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnSRCancel.Click
        _addframe.Navigate(New SuccessRegisterPage(mainFrame, email, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub btnSRAddEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnSRAddEmployee.Click
        e.Handled = True
        If txtSRWhosInvolve.Text = String.Empty Then
            txtSRWhosInvolve.Text += comboAddEmployee.Text
        Else
            Dim txtBox As String = txtSRWhosInvolve.Text
            Dim cbBox As String = comboAddEmployee.Text
            Dim ifYes As Integer = txtBox.IndexOf(cbBox)
            If ifYes = -1 Then
                txtSRWhosInvolve.Text += ", " + comboAddEmployee.Text
            Else
                MsgBox("Cannot Allow Duplicate Entry!", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub btnSRDelete_Click(sender As Object, e As RoutedEventArgs) Handles btnSRDelete.Click
        Try
            e.Handled = True
            If txtSRID.Text = String.Empty Then
                MsgBox("Please Fill up the Fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
                If result = 1 Then
                    client.DeleteSuccessRegisterBySuccessID(CUInt(txtSRID.Text))
                    ClearFields()
                    _addframe.Navigate(New SuccessRegisterPage(mainFrame, email, _addframe, _menugrid, _submenuframe))
                    mainFrame.IsEnabled = True
                    mainFrame.Opacity = 1
                    _menugrid.IsEnabled = True
                    _menugrid.Opacity = 1
                    _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1

                    _addframe.Visibility = Visibility.Hidden
                Else
                    Exit Sub
                End If
            End If
            'End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub dateInput_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateInput.SelectedDateChanged
        e.Handled = True
        If dateInput.SelectedDate > Date.Now Then
            MsgBox("Date must not be beyond today", MsgBoxStyle.Exclamation, "AIDE")
            dateInput.SelectedDate = Date.Now
        Else
            Exit Sub
        End If
    End Sub


    Private Sub btnRemovedEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnRemovedEmployee.Click
        e.Handled = True
        If txtSRWhosInvolve.Text = String.Empty Then
            MsgBox("Textbox is Empty", MsgBoxStyle.Exclamation, "AIDE")
        Else
            Dim txtBox As String = txtSRWhosInvolve.Text
            Dim cbBox As String = String.Empty
            Dim ifYes As Integer = txtBox.IndexOf(comboAddEmployee.Text)

            If ifYes <> -1 Then
                If ifYes <> 0 Then
                    cbBox = ", " & comboAddEmployee.Text
                    Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                    txtSRWhosInvolve.Text = txtSRWhosInvolve.Text.Remove(ifYesAgain, cbBox.Length)
                Else
                    cbBox = comboAddEmployee.Text & ", "

                    If txtBox.Length <> comboAddEmployee.Text.Length Then
                        cbBox = txtSRWhosInvolve.Text & ", "
                    Else
                        cbBox = txtSRWhosInvolve.Text
                    End If
                    txtSRWhosInvolve.Text = txtBox.Remove(ifYes, cbBox.Length)
                End If
            Else
                MsgBox("Entry already removed", MsgBoxStyle.Exclamation, "AIDE")
            End If
        End If
    End Sub
#End Region

#Region "Functions"

    Private Sub AssignEvents()
        AddHandler btnSRCreate.Click, AddressOf btnSRCreate_Click
        AddHandler btnSRCancel.Click, AddressOf btnSRCancel_Click
        AddHandler btnSRAddEmployee.Click, AddressOf btnSRAddEmployee_Click
        AddHandler btnSRUpdate.Click, AddressOf btnSRUpdate_Click
        AddHandler btnSRDelete.Click, AddressOf btnSRDelete_Click
    End Sub 'Assign events to buttons

    Private Sub ClearFields()
        txtRaisedBy.Clear()
        txtSRAdditional.Clear()
        txtSRDetails.Clear()
        txtSRID.Clear()
        txtSRWhosInvolve.Clear()
        dateInput.Text = String.Empty
    End Sub

    ''' <summary>
    ''' By Aevan Camille Batongbacal
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadData()
        btnSRCreate.Visibility = System.Windows.Visibility.Hidden
        btnSRUpdate.Visibility = System.Windows.Visibility.Visible
        btnSRDelete.Visibility = System.Windows.Visibility.Visible
        comboRaisedBy.Visibility = System.Windows.Visibility.Hidden
        txtRaisedBy.Visibility = System.Windows.Visibility.Visible
        txtSRID.Text = successRegister.SuccessID
        txtRaisedBy.Text = successRegister.Nick_Name
        txtSRDetails.Text = successRegister.DetailsOfSuccess
        txtSRWhosInvolve.Text = successRegister.WhosInvolve
        txtSRAdditional.Text = successRegister.AdditionalInformation
        dateInput.Text = successRegister.DateInput
    End Sub

    ''' <summary>
    ''' By Aevan Camille Batongbacal
    ''' </summary>
    ''' <remarks></remarks>
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            'DisplayText("Opening client service...")
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
            'DisplayText("Service opened successfully...")
            'Return True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

    ''' <summary>
    ''' By Aevan Camille Batongbacal
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateComboBox()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = client.ViewNicknameByDeptID(email)
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

                comboRaisedBy.DataContext = nicknameVM
                comboAddEmployee.DataContext = nicknameVM
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
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
