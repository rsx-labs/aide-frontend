Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AssetsUpdatePage
    Implements ServiceReference1.IAideServiceCallback
#Region "Fields"

    Private mainFrame As Frame
    Private client As AideServiceClient
    Private assetsModel As New AssetsModel
    Private profile As New Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private status As Integer
    Private empID As Integer
#End Region

#Region "Constructor"
    Public Sub New(_assetsModel As AssetsModel, mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()

        Me.mainFrame = mainFrame
        Me.profile = _profile
        Me.assetsModel = _assetsModel
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        tbSuccessForm.Text = "Update Asset"
        dateInput.IsEnabled = False
        SetData()
        AssignEvents()
    End Sub

#End Region

#Region "Events"
    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        Try
            e.Handled = True
            Dim assets As New Assets
            If CheckMissingField() Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                assets.ASSET_ID = txtID.Text
                assets.EMP_ID = empID
                assets.TRANSFER_ID = cbManager.SelectedValue
                assets.ASSET_DESC = assetsModel.ASSET_DESC
                assets.MANUFACTURER = assetsModel.MANUFACTURER
                assets.MODEL_NO = txtModel.Text
                assets.SERIAL_NO = txtSerial.Text
                assets.ASSET_TAG = txtAssetTag.Text
                assets.DATE_PURCHASED = dateInput.SelectedDate
                assets.OTHER_INFO = txtOtherInfo.Text
                assets.ASSIGNED_TO = 999
                assets.STATUS = status
                assets.PREVIOUS_ID = assetsModel.EMP_ID

                If InitializeService() Then
                        client.UpdateAssets(assets)
                        MsgBox("Assets have been updated.", MsgBoxStyle.Information, "AIDE")
                        ClearFields()
                        mainFrame.Navigate(New AssetsListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe))
                        mainFrame.IsEnabled = True
                        mainFrame.Opacity = 1
                        _menugrid.IsEnabled = True
                        _menugrid.Opacity = 1
                        _submenuframe.IsEnabled = True
                        _submenuframe.Opacity = 1

                        _addframe.Visibility = Visibility.Hidden

                End If

            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        mainFrame.Navigate(New AssetsListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe))
        mainFrame.IsEnabled = True
        mainFrame.Opacity = 1
        _menugrid.IsEnabled = True
        _menugrid.Opacity = 1
        _submenuframe.IsEnabled = True
        _submenuframe.Opacity = 1

        _addframe.Visibility = Visibility.Hidden
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As RoutedEventArgs) Handles btnDelete.Click
        Try
            e.Handled = True
            Dim assets As New Assets
            If txtID.Text = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            Else
                assets.ASSET_ID = txtID.Text
                assets.EMP_ID = assetsModel.EMP_ID
                assets.ASSET_DESC = assetsModel.ASSET_DESC
                assets.MANUFACTURER = assetsModel.MANUFACTURER
                assets.MODEL_NO = txtModel.Text
                assets.SERIAL_NO = txtSerial.Text
                assets.ASSET_TAG = txtAssetTag.Text
                assets.DATE_PURCHASED = dateInput.SelectedDate
                assets.OTHER_INFO = txtOtherInfo.Text
                assets.ASSIGNED_TO = 999

                If InitializeService() Then
                        client.DeleteAsset(assets)
                        MsgBox("Assets have been deleted.", MsgBoxStyle.Information, "AIDE")
                        ClearFields()
                        mainFrame.Navigate(New AssetsListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe))
                        mainFrame.IsEnabled = True
                        mainFrame.Opacity = 1
                        _menugrid.IsEnabled = True
                        _menugrid.Opacity = 1
                        _submenuframe.IsEnabled = True
                    _submenuframe.Opacity = 1
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub dateInput_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateInput.SelectedDateChanged
        e.Handled = True
        If dateInput.SelectedDate > Date.Now Then
            MsgBox("Please enter a date on or before the current date.", MsgBoxStyle.Exclamation, "AIDE")
            dateInput.SelectedDate = Date.Now
        Else
            Exit Sub
        End If
    End Sub

    Private Sub cbDepartment_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbDepartment.SelectionChanged
        LoadDivision()
    End Sub

    Private Sub cbDivision_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbDivision.SelectionChanged
        ListOfManagers()
    End Sub

    Private Sub cbManager_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbManager.SelectionChanged
        If cbManager.SelectedValue = Nothing Then
            status = assetsModel.STATUS
        Else
            status = 4 'This means the asset is transferred so we need to set the status to 1 - Unassigned
        End If
    End Sub
#End Region

#Region "Functions"
    Private Sub AssignEvents()
        AddHandler btnCancel.Click, AddressOf btnCancel_Click
        AddHandler btnUpdate.Click, AddressOf btnUpdate_Click
        AddHandler btnDelete.Click, AddressOf btnDelete_Click
    End Sub 'Assign events to buttons

    Private Sub ClearFields()
        txtAssetTag.Clear()
        txtModel.Clear()
        txtOtherInfo.Clear()
        txtSerial.Clear()
        dateInput.Text = String.Empty
    End Sub

    Public Sub SetData()
        Try
            If InitializeService() Then
                LoadDepartment()
            End If
            LoadData()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LoadData()
        btnCreate.Visibility = System.Windows.Visibility.Collapsed
        btnUpdate.Visibility = System.Windows.Visibility.Visible
        empID = assetsModel.EMP_ID
        If assetsModel.STATUS = 1 And profile.Permission_ID = 1 Then ' Show only when asset is unassigned and user is manager level
            dateInput.IsEnabled = False
            txtCreatedBy.IsEnabled = False
            txtModel.IsEnabled = False
            txtSerial.IsEnabled = False
            txtAssetTag.IsEnabled = False
            btnDelete.Visibility = System.Windows.Visibility.Visible
            TransferAsset.Visibility = Windows.Visibility.Visible
            empID = profile.Emp_ID
        End If

        txtID.Text = assetsModel.ASSET_ID
        txtCreatedBy.Text = assetsModel.FULL_NAME
        txtModel.Text = assetsModel.MODEL_NO
        txtSerial.Text = assetsModel.SERIAL_NO
        txtAssetTag.Text = assetsModel.ASSET_TAG
        txtOtherInfo.Text = assetsModel.OTHER_INFO
        dateInput.Text = assetsModel.DATE_PURCHASED
    End Sub

    Public Sub LoadDepartment()
        Try
            Dim lstDepartment As DepartmentList() = client.GetAllDepartment()
            Dim lstDepartmentList As New ObservableCollection(Of DepartmentModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objdepartment As DepartmentList In lstDepartment
                selectionDBProvider._setlistofDepartment(objdepartment)
            Next

            For Each rawUser As myDepartmentSet In selectionDBProvider._getobjDepartment()
                lstDepartmentList.Add(New DepartmentModel(rawUser))
            Next

            selectionListVM.ObjectDepartmentSet = lstDepartmentList

            cbDepartment.DataContext = selectionListVM
            cbDepartment.ItemsSource = selectionListVM.ObjectDepartmentSet
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadDivision()
        Try
            Dim lstDivision As DivisionList() = client.GetAllDivision(cbDepartment.SelectedValue)
            Dim lstDivisionList As New ObservableCollection(Of DivisionModel)
            Dim selectionDBProvider As New SelectionListDBProvider
            Dim selectionListVM As New SelectionListViewModel()

            For Each objdivision As DivisionList In lstDivision
                selectionDBProvider._setlistofDivision(objdivision)
            Next

            For Each rawUser As myDivisionSet In selectionDBProvider._getobjDivision()
                lstDivisionList.Add(New DivisionModel(rawUser))
            Next

            selectionListVM.ObjectDivisionSet = lstDivisionList

            cbDivision.DataContext = selectionListVM
            cbDivision.ItemsSource = selectionListVM.ObjectDivisionSet

            If lstDivision.Count = 0 Then
                ListOfManagers()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub ListOfManagers()
        Dim lstMangers As Assets() = client.GetAllManagersByDeptorDiv(cbDepartment.SelectedValue, cbDivision.SelectedValue)
        Dim lstMangersList As New ObservableCollection(Of AssetsModel)
        Dim assetsDBProvider As New AssetsDBProvider
        Dim assetsVM As New AssetsViewModel()

        For Each objAssets As Assets In lstMangers
            assetsDBProvider.SetManagerList(objAssets)
        Next

        For Each rawUser As MyAssets In assetsDBProvider.GetManagerList()
            lstMangersList.Add(New AssetsModel(rawUser))
        Next

        assetsVM.AssetManagerList = lstMangersList

        cbManager.DataContext = assetsVM
        cbManager.ItemsSource = assetsVM.AssetManagerList
    End Sub

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

    Public Function CheckMissingField() As Boolean
        If txtAssetTag.Text = String.Empty Then
            Return True
        Else
            Return False
        End If
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
