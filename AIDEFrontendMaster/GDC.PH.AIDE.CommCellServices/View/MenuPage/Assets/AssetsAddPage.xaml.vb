Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ServiceModel

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AssetsAddPage
    Implements ServiceReference1.IAideServiceCallback
#Region "Fields"

    Private mainFrame As Frame
    Private client As AideServiceClient
    Private assetsModel As New AssetsModel
    Private profile As New Profile
    Private _addframe As Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame

#End Region

#Region "Constructor"

    Public Sub New(mainFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        InitializeComponent()
        Me.profile = _profile
        Me.mainFrame = mainFrame
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        btnCreate.Visibility = System.Windows.Visibility.Visible
        btnUpdate.Visibility = System.Windows.Visibility.Hidden
        btnDelete.Visibility = System.Windows.Visibility.Hidden
        tbSuccessForm.Text = "Create new Asset"
        txtCreatedBy.Text = profile.Emp_ID
        AssignEvents()
        ListOfManagers()
        ListOfAssetType()
        ListOfAssetManufacturer()
    End Sub

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
        LoadData()
        AssignEvents()
    End Sub

#End Region

#Region "Events"

    Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnUpdate.Click
        Try
            e.Handled = True
            Dim assets As New Assets
            If CheckMissingField() Then
                MsgBox("Please Fill up the Fields!", MsgBoxStyle.Exclamation, "AIDE")
            Else
                assets.ASSET_ID = txtID.Text
                assets.EMP_ID = txtCreatedBy.Text
                assets.ASSET_DESC = cbAssetType.Text
                assets.MANUFACTURER = cbAssetManufacturer.Text
                assets.MODEL_NO = txtModel.Text
                assets.SERIAL_NO = txtSerial.Text
                assets.ASSET_TAG = txtAssetTag.Text
                assets.DATE_PURCHASED = dateInput.SelectedDate
                assets.OTHER_INFO = txtOtherInfo.Text
                assets.ASSIGNED_TO = 999
                Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
                If result = 1 Then
                    If InitializeService() Then
                        client.UpdateAssets(assets)
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
                Else
                    Exit Sub
                End If

            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Failed")
        End Try
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        Try
            e.Handled = True
            Dim assets As New Assets
            If CheckMissingField() Then
                MsgBox("Please Fill up the Fields!", MsgBoxStyle.Exclamation, "AIDE")
            Else
                assets.EMP_ID = Integer.Parse(txtCreatedBy.Text)
                assets.ASSET_DESC = cbAssetType.Text
                assets.MANUFACTURER = cbAssetManufacturer.Text
                assets.MODEL_NO = txtModel.Text
                assets.SERIAL_NO = txtSerial.Text
                assets.ASSET_TAG = txtAssetTag.Text
                assets.DATE_PURCHASED = Date.Parse(dateInput.SelectedDate)
                assets.ASSIGNED_TO = cbNickname.SelectedValue
                assets.OTHER_INFO = txtOtherInfo.Text
                Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
                If result = 1 Then
                    If InitializeService() Then
                        client.InsertAssets(assets)
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
                Else
                    Exit Sub
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
            If txtID.Text = String.Empty Then
                MsgBox("Please Fill up the Fields", MsgBoxStyle.Exclamation, "AIDE")
            Else
                Dim result As Integer = MsgBox("Are you sure you want to continue?", MessageBoxButton.OKCancel, "AIDE")
                If result = 1 Then
                    client.DeleteSuccessRegisterBySuccessID(CUInt(txtID.Text))
                    ClearFields()
                    mainFrame.Navigate(New AssetsListPage(mainFrame, profile, _addframe, _menugrid, _submenuframe))
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

    Private Sub dateInput_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dateInput.SelectedDateChanged
        e.Handled = True
        If dateInput.SelectedDate > Date.Now Then
            MsgBox("Date must not be beyond today", MsgBoxStyle.Exclamation, "AIDE")
            dateInput.SelectedDate = Date.Now
        Else
            Exit Sub
        End If
    End Sub
#End Region

#Region "Functions"

    Private Sub AssignEvents()
        AddHandler btnCreate.Click, AddressOf btnCreate_Click
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

    Private Sub LoadData()
        btnCreate.Visibility = System.Windows.Visibility.Collapsed
        btnUpdate.Visibility = System.Windows.Visibility.Visible
        btnDelete.Visibility = System.Windows.Visibility.Collapsed
        cbNickname.IsEnabled = False
        txtID.Text = assetsModel.ASSET_ID
        txtCreatedBy.Text = assetsModel.EMP_ID
        cbAssetType.Text = assetsModel.ASSET_DESC
        cbAssetManufacturer.Text = assetsModel.MANUFACTURER
        txtModel.Text = assetsModel.MODEL_NO
        txtSerial.Text = assetsModel.SERIAL_NO
        txtAssetTag.Text = assetsModel.ASSET_TAG
        txtOtherInfo.Text = assetsModel.OTHER_INFO
        dateInput.Text = assetsModel.DATE_PURCHASED
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
        If cbAssetType.SelectedValue = Nothing AndAlso _
               txtAssetTag.Text = String.Empty AndAlso _
               txtSerial.Text = String.Empty AndAlso _
               txtModel.Text = String.Empty AndAlso _
               cbAssetManufacturer.SelectedValue = Nothing Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub ListOfManagers()
        Try
            If InitializeService() Then
                Dim lstNickname As Nickname() = client.GetAllManagers(profile.Emp_ID)
                Dim lstNicknameList As New ObservableCollection(Of NicknameModel)
                Dim successRegisterDBProvider As New SuccessRegisterDBProvider
                Dim nicknameVM As New NicknameViewModel()


                For Each objLessonLearnt As Nickname In lstNickname
                    successRegisterDBProvider.SetMyNickname(objLessonLearnt)
                Next

                For Each rawUser As MyNickname In successRegisterDBProvider.GetMyNickname()
                    lstNicknameList.Add(New NicknameModel(rawUser))
                Next
                nicknameVM.NicknameList = Nothing
                cbNickname.ItemsSource = Nothing
                nicknameVM.NicknameList = lstNicknameList
                cbNickname.ItemsSource = nicknameVM.NicknameList
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub ListOfAssetType()
        Try
            If InitializeService() Then
                Dim lstAssets As Assets() = client.GetAssetDescription()
                Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
                Dim assetsDBProvider As New AssetsDBProvider
                Dim assetsVM As New AssetsViewModel()

                ' Set the MyLessonLearntList 
                For Each objAssets As Assets In lstAssets
                    assetsDBProvider.SetAssetTypeList(objAssets)
                Next

                For Each rawUser As MyAssets In assetsDBProvider.GetAssetTypeList()
                    lstAssetsList.Add(New AssetsModel(rawUser))
                Next

                assetsVM.AssetTypeList = Nothing
                cbAssetType.ItemsSource = Nothing
                assetsVM.AssetTypeList = lstAssetsList
                cbAssetType.ItemsSource = assetsVM.AssetTypeList
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub ListOfAssetManufacturer()
        Try
            If InitializeService() Then
                Dim lstAssets As Assets() = client.GetAssetManufacturer()
                Dim lstAssetsList As New ObservableCollection(Of AssetsModel)
                Dim assetsDBProvider As New AssetsDBProvider
                Dim assetsVM As New AssetsViewModel()

                ' Set the MyLessonLearntList 
                For Each objAssets As Assets In lstAssets
                    assetsDBProvider.SetAssetManufacturerList(objAssets)
                Next

                For Each rawUser As MyAssets In assetsDBProvider.GetAssetManufacturerList()
                    lstAssetsList.Add(New AssetsModel(rawUser))
                Next

                assetsVM.AssetManufacturerList = Nothing
                cbAssetManufacturer.ItemsSource = Nothing
                assetsVM.AssetManufacturerList = lstAssetsList
                cbAssetManufacturer.ItemsSource = assetsVM.AssetManufacturerList
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
