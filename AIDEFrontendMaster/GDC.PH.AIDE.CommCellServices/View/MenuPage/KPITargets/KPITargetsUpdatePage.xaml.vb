Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.IO
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Class KPITargetsUpdatePage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback

    Private _kpiTargetsVM As New KPITargetsViewModel
    'Private _client As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private _kpiTargetModel As New KPITargetsModel
    Private _kpiTargets As New KPITargets
    Private email As String
    Private profile As Profile

    Public Sub New(_mainframe As Frame, _empID As Integer, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _email As String, _profile As Profile, _kpiTargetModel As KPITargetsModel)

        InitializeComponent()
        _kpiTargetsVM.KPITargetsSet = _kpiTargetModel
        DataContext = _kpiTargetsVM
        txtKPIDescription.AppendText(_kpiTargetsVM.KPITargetsSet.Description)
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile
        Me.email = _email
        Me.empID = _empID
    End Sub

    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        _client = New AideServiceClient(Context)
    '        _client.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        _client.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

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

    Private Sub btnBack_Click(sender As Object, e As RoutedEventArgs)
        mainframe.IsEnabled = True
        mainframe.Opacity = 1
        menugrid.IsEnabled = True
        menugrid.Opacity = 1
        submenuframe.IsEnabled = True
        submenuframe.Opacity = 1

        addframe.Visibility = Visibility.Hidden
    End Sub


    'Public Sub BindMessage()
    '    Dim hasDocument As FlowDocument = New FlowDocument()

    '    Dim bindObject As Binding = New Binding("ObjectAnnouncement.MESSAGE")
    '    bindObject.Source = txtAnnouncementMessage
    '    bindObject.Mode = BindingMode.OneWay
    '    BindingOperations.SetBinding(hasDocument, FlowDocument.proper)




    'End Sub

    Public Function getDataUpdate(ByVal _kpiTargetModel As KPITargetsModel)
        Try
            'InitializeService()
            Dim textRange As New TextRange(txtKPIDescription.Document.ContentStart, txtKPIDescription.Document.ContentEnd)
            If textRange.Text = Nothing Or _kpiTargetModel.Subject = Nothing Then
            Else
                _kpiTargets.KPI_Id = _kpiTargetModel.ID
                _kpiTargets.EmployeeId = _kpiTargetModel.EmployeeID
                _kpiTargets.KPI_ReferenceNo = _kpiTargetModel.KPIReferenceNo
                _kpiTargets.FYStart = _kpiTargetModel.FYStart
                _kpiTargets.FYEnd = _kpiTargetModel.FYEnd
                _kpiTargets.Description = textRange.Text.Trim()
                _kpiTargets.Subject = _kpiTargetModel.Subject
                _kpiTargets.DateCreated = _kpiTargetModel.DateCreated
            End If
            Return _kpiTargets
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return ex
        End Try
    End Function

    Private Sub btnKPITargetUpdate_Click(sender As Object, e As RoutedEventArgs)
        Try
            'InitializeService()
            Dim textRange As New TextRange(txtKPIDescription.Document.ContentStart, txtKPIDescription.Document.ContentEnd)


            If _kpiTargetsVM.KPITargetsSet.Subject = Nothing Or textRange.Text.Trim() = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                Me.DataContext = _kpiTargetsVM.KPITargetsSet
                AideClient.GetClient().UpdatePITarget(getDataUpdate(Me.DataContext()))
                MsgBox("KPI target have been updated.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                _kpiTargets.Subject = Nothing
                _kpiTargets.Description = Nothing

                mainframe.Navigate(New HomePage(mainframe, profile.Position, Me.empID, addframe, menugrid, submenuframe, Me.email, Me.profile))
                mainframe.IsEnabled = True
                mainframe.Opacity = 1
                menugrid.IsEnabled = True
                menugrid.Opacity = 1
                submenuframe.IsEnabled = True
                submenuframe.Opacity = 1

                addframe.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
End Class

