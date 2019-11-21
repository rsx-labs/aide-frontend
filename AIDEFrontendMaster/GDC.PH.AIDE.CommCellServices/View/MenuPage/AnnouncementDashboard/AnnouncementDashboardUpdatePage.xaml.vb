Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.IO
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Class AnnouncementDashboardUpdatePage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback

    Private announcementModel As New AnnouncementListViewModel
    Private aide As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private _announcemodel As New AnnouncementModel
    Private _announce As New Announcements
    Private email As String
    Private profile As Profile

    Public Sub New(_mainframe As Frame, _empID As Integer, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _email As String, _profile As Profile, _annModel As AnnouncementModel)

        InitializeComponent()
        announcementModel.ObjectAnnouncement = _annModel
        DataContext = announcementModel
        txtAnnouncementMessage.AppendText(announcementModel.ObjectAnnouncement.MESSAGE.Trim())
        Me.empID = _empID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.email = _email
        Me.profile = _profile

    End Sub

    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            aide = New AideServiceClient(Context)
            aide.Open()
            bInitialize = True
        Catch ex As SystemException
            aide.Abort()
        End Try
        Return bInitialize
    End Function

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

    Private Sub btnAnnouncementUpdate_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()
            Dim textRange As New TextRange(txtAnnouncementMessage.Document.ContentStart, txtAnnouncementMessage.Document.ContentEnd)


            If announcementModel.ObjectAnnouncement.TITLE = Nothing Or textRange.Text.Trim() = String.Empty Then
                MsgBox("Please fill up all required fields!", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                Me.DataContext = announcementModel.ObjectAnnouncement
                aide.UpdateAnnouncements(getDataUpdate(Me.DataContext()))
                MsgBox("Successfully updated!", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                _announce.TITLE = Nothing
                _announce.MESSAGE = Nothing
                _announce.END_DATE = Nothing
                _announce.EMP_ID = Nothing

                mainframe.Navigate(New HomePage(mainframe, profile.Position, empID, addframe, menugrid, submenuframe, email, profile))
                mainframe.IsEnabled = True
                mainframe.Opacity = 1
                menugrid.IsEnabled = True
                menugrid.Opacity = 1
                submenuframe.IsEnabled = True
                submenuframe.Opacity = 1

                addframe.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "AIDE") = vbYes Then
                Environment.Exit(0)
            Else
            End If
        End Try
    End Sub

    'Public Sub BindMessage()
    '    Dim hasDocument As FlowDocument = New FlowDocument()

    '    Dim bindObject As Binding = New Binding("ObjectAnnouncement.MESSAGE")
    '    bindObject.Source = txtAnnouncementMessage
    '    bindObject.Mode = BindingMode.OneWay
    '    BindingOperations.SetBinding(hasDocument, FlowDocument.proper)




    'End Sub

    Public Function getDataUpdate(ByVal _announcementmodel As AnnouncementModel)
        Try
            InitializeService()
            Dim textRange As New TextRange(txtAnnouncementMessage.Document.ContentStart, txtAnnouncementMessage.Document.ContentEnd)
            If textRange.Text = Nothing Or _announcementmodel.TITLE = Nothing Then
            Else
                _announce.ANNOUNCEMENT_ID = _announcementmodel.ANNOUNCEMENT_ID
                _announce.MESSAGE = textRange.Text.Trim()
                _announce.TITLE = _announcementmodel.TITLE
                _announce.END_DATE = DateTime.Today
                _announce.EMP_ID = Me.empID

            End If
            Return _announce
        Catch ex As Exception
            If MsgBox(ex.Message + " Do you wish to exit?", vbYesNo + vbCritical, "Error Encountered") = vbYes Then
                Environment.Exit(0)
            Else
            End If
            Return ex
        End Try
    End Function
End Class

