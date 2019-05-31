Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Text.RegularExpressions
Imports System.Configuration
''' <summary>
''' By Christian Lois Valondo
''' </summary>
''' <remarks></remarks>
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class NewContactList
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private mainFrame As Frame
    Private client As ServiceReference1.AideServiceClient
    Private contacts As New ContactListModel
    Private email As String
    Private _menugrid As Grid
    Private _addframe As Frame
    Private _submenuframe As Frame

    Dim locationEco As String = ConfigurationManager.AppSettings("locationEco")
    Dim locationNet As String = ConfigurationManager.AppSettings("locationNet")
    Dim locationDurham As String = ConfigurationManager.AppSettings("locationDurham")
    Dim locationWfh As String = ConfigurationManager.AppSettings("locationWfh")

#End Region

#Region "Constructor"

    Public Sub New(_contacts As ContactListModel, mainFrame As Frame, _email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me._addframe = _addframe
        Me.mainFrame = mainFrame
        Me.email = _email
        Me.contacts = _contacts
        Me.DataContext = contacts
        AssignEvents()
        textLimits()
        SetLocationCB()
    End Sub

#End Region

#Region "Events"

    Private Sub btnCUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnCUpdate.Click
        Try
            e.Handled = True
            Dim contactList As New ContactList
            If txtCCelNo.Text = String.Empty AndAlso cbLocation.Text = String.Empty Then
                MsgBox("Please Fill Up all the Fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                contactList.EmpID = txtCEmpID.Text
                contactList.EMADDRESS = txtCEmail.Text
                contactList.EMADDRESS2 = txtCEmail2.Text
                contactList.LOC = cbLocation.Text
                contactList.lOCAL = txtCLocal.Text
                contactList.CELL_NO = txtCCelNo.Text
                contactList.HOUSEPHONE = txtCHome.Text
                contactList.OTHERPHONE = txtCOther.Text
                contactList.DateReviewed = DateTime.Now.Date

                Dim result As Integer = MsgBox("Are you sure you want to continue?", MsgBoxStyle.OkCancel, "AIDE")
                If result = 1 Then
                    If InitializeService() Then
                        client.UpdateContactListByEmpID(contactList)
                        ClearFields()
                        mainFrame.Navigate(New ContactListPage(mainFrame, email, _addframe, _menugrid, _submenuframe))
                        mainFrame.IsEnabled = True
                        mainFrame.Opacity = 1
                        _menugrid.IsEnabled = True
                        _menugrid.Opacity = 1
                        _submenuframe.IsEnabled = True
                        _submenuframe.Opacity = 1
                        _addframe.Visibility = Visibility.Hidden
                    End If
                Else
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "AIDE")
        End Try
    End Sub

    Private Sub btnCCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCCancel.Click
        mainFrame.Navigate(New ContactListPage(mainFrame, email, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1
        _addframe.Visibility = Visibility.Hidden
    End Sub

#End Region

#Region "Functions/Methods"

    Public Sub textLimits()
        txtCCelNo.MaxLength = 15
        txtCHome.MaxLength = 15
        txtCLocal.MaxLength = 4
        txtCOther.MaxLength = 15
    End Sub

    Public Sub SetLocationCB()
        cbLocation.DisplayMemberPath = "Text"
        cbLocation.SelectedValuePath = "Value"
        cbLocation.Items.Add(New With {.Text = locationEco, .Value = locationEco})
        cbLocation.Items.Add(New With {.Text = locationNet, .Value = locationNet})
        cbLocation.Items.Add(New With {.Text = locationDurham, .Value = locationDurham})
        cbLocation.Items.Add(New With {.Text = locationWfh, .Value = locationWfh})
    End Sub

    Private Sub NumberValidationTextBox(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim regex As Regex = New Regex("[^0-9]+")
        e.Handled = regex.IsMatch(e.Text)
    End Sub

    Private Sub AssignEvents()
        AddHandler btnCCancel.Click, AddressOf btnCCancel_Click
        AddHandler btnCUpdate.Click, AddressOf btnCUpdate_Click
    End Sub 'Assign events to buttons

    Private Sub ClearFields()
        txtCEmpID.Clear()
        txtCCelNo.Clear()
        txtCEmail.Clear()
        txtCEmail2.Clear()
        txtCHome.Clear()
        txtCLocal.Clear()
        cbLocation.Text = String.Empty
    End Sub

    Private Sub LoadData()
        txtCCelNo.Text = contacts.CEL_NO
        txtCEmail.Text = contacts.EMAIL_ADDRESS
        txtCEmail2.Text = contacts.EMAIL_ADDRESS2
        txtCHome.Text = contacts.HOMEPHONE
        txtCOther.Text = contacts.OTHER_PHONE
        txtCEmpID.Text = contacts.EMP_ID
        txtCLocal.Text = contacts.LOCAL
        cbLocation.Text = contacts.LOCATION
    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            client = New AideServiceClient(Context)
            client.Open()
            bInitialize = True
        Catch ex As SystemException
            client.Abort()
        End Try
        Return bInitialize
    End Function

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
