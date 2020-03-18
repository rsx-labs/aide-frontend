Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class NewSuccessRegister
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private mainFrame As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private email As String
    Private profile As Profile
    Private client As ServiceReference1.AideServiceClient
    Private successRegister As New SuccessRegisterModel
    Private dsplyByDiv As Integer = 1
    'Private srmodel As SuccessRegisterModel

#End Region

#Region "Constructor"

    Public Sub New(_mainFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _profile As Profile)

        InitializeComponent()
        mainFrame = _mainFrame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        email = _profile.Email_Address
        profile = _profile

        btnSRCreate.Visibility = System.Windows.Visibility.Visible
        btnSRUpdate.Visibility = System.Windows.Visibility.Collapsed
        btnSRDelete.Visibility = System.Windows.Visibility.Collapsed
        comboRaisedBy.Visibility = System.Windows.Visibility.Visible
        txtRaisedBy.Visibility = System.Windows.Visibility.Hidden
        tbSuccessForm.Text = "Create Success Register"
        AssignEvents()
        PopulateComboBox()
    End Sub

    Public Sub New(_mainFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _successRegister As SuccessRegisterModel, _profile As Profile)
        InitializeComponent()
        mainFrame = _mainFrame
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        successRegister = _successRegister
        email = _profile.Email_Address
        profile = _profile

        tbSuccessForm.Text = "Update Success Register"
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
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                SuccessRegisters.SuccessID = txtSRID.Text
                SuccessRegisters.Emp_ID = comboRaisedBy.SelectedValue
                SuccessRegisters.DateInput = dateInput.SelectedDate
                SuccessRegisters.DetailsOfSuccess = txtSRDetails.Text
                SuccessRegisters.WhosInvolve = txtSRWhosInvolve.Text
                SuccessRegisters.AdditionalInformation = txtSRAdditional.Text
                client.UpdateSuccessRegisterByEmpID(SuccessRegisters)
                MsgBox("Success register has been updated.", MsgBoxStyle.Information, "AIDE")
                ClearFields()
                ExitPage()
            End If
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
            If txtSRDetails.Text = String.Empty Or dateInput.Text = String.Empty Or comboRaisedBy.Text = Nothing Or txtSRWhosInvolve.Text = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                SuccessRegisters.Emp_ID = comboRaisedBy.SelectedValue
                SuccessRegisters.DateInput = dateInput.SelectedDate
                SuccessRegisters.DetailsOfSuccess = txtSRDetails.Text
                SuccessRegisters.WhosInvolve = txtSRWhosInvolve.Text
                SuccessRegisters.AdditionalInformation = txtSRAdditional.Text

                client.CreateNewSuccessRegister(SuccessRegisters)
                MsgBox("Success Register has been added.", MsgBoxStyle.Information, "AIDE")
                ClearFields()
                ExitPage()
            End If
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub btnSRCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnSRCancel.Click
        ExitPage()
    End Sub

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub btnSRAddEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnSRAddEmployee.Click
        e.Handled = True
        If txtSRWhosInvolve.Text = String.Empty Then
            txtSRWhosInvolve.Text += comboAddEmployee.SelectedValue
        Else
            Dim txtBox As String = txtSRWhosInvolve.Text
            Dim cbBox As String = comboAddEmployee.SelectedValue
            Dim ifYes As Integer = txtBox.IndexOf(cbBox)
            If ifYes = -1 Then
                txtSRWhosInvolve.Text += ", " + comboAddEmployee.SelectedValue
            Else
                MsgBox("Employee is already involved. Please select a different employee.", MsgBoxStyle.Exclamation, "AIDE")
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
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.YesNo, "AIDE")
                If result = 6 Then
                    client.DeleteSuccessRegisterBySuccessID(CUInt(txtSRID.Text))
                    ClearFields()
                    ExitPage()
                Else
                    Exit Sub
                End If
            End If
            'End If
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    ''' <summary>
    ''' By Krizza Tolento
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub dateInput_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateInput.SelectedDateChanged
        e.Handled = True
        If dateInput.SelectedDate > Date.Now Then
            MsgBox("Please enter a date on or before the current date.", MsgBoxStyle.Exclamation, "AIDE")
            dateInput.SelectedDate = Date.Now
        Else
            Exit Sub
        End If
    End Sub


    Private Sub btnRemovedEmployee_Click(sender As Object, e As RoutedEventArgs) Handles btnRemovedEmployee.Click
        Try
            e.Handled = True
            If txtSRWhosInvolve.Text = String.Empty Then
                MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim txtBox As String = txtSRWhosInvolve.Text
                Dim cbBox As String = String.Empty
                Dim ifYes As Integer = txtBox.IndexOf(comboAddEmployee.SelectedValue)

                If ifYes <> -1 Then
                    If ifYes <> 0 Then
                        cbBox = ", " & comboAddEmployee.SelectedValue
                        Dim ifYesAgain As Integer = txtBox.IndexOf(cbBox)
                        txtSRWhosInvolve.Text = txtSRWhosInvolve.Text.Remove(ifYesAgain, cbBox.Length)
                    Else
                        If txtBox.Length = comboAddEmployee.SelectedValue.Length Then
                            cbBox = txtSRWhosInvolve.Text
                        Else
                            cbBox = comboAddEmployee.SelectedValue & ", "
                        End If
                        txtSRWhosInvolve.Text = txtBox.Remove(ifYes, cbBox.Length)

                        cbBox = comboAddEmployee.SelectedValue & ", "
                    End If
                Else
                    MsgBox("No assigned employee to remove.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
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
        btnSRCreate.Visibility = System.Windows.Visibility.Collapsed
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
        comboRaisedTBlock.Visibility = Windows.Visibility.Hidden
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
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
                Dim lstNickname As Nickname() = client.ViewNicknameByDeptID(email, dsplyByDiv)
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ExitPage()
        mainFrame.Navigate(New SuccessRegisterPage(mainFrame, addframe, menugrid, submenuframe, profile))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1
        addframe.Visibility = Visibility.Hidden
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
