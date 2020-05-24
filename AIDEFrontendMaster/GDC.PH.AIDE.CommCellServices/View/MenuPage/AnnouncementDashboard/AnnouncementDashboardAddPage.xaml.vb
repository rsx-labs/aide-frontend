Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel

Class AnnouncementDashboardAddPage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback


    'Private aide As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private _announcemodel As New AnnouncementModel
    Private _announce As New Announcements
    Private email As String
    Private profile As Profile

    Public Sub New(_mainframe As Frame, _empID As Integer, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _email As String, _profile As Profile)

        ' This call is required by the designer.
        InitializeComponent()
        Me.empID = _empID
        Me.mainframe = _mainframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.email = _email
        Me.profile = _profile
        Me.DataContext = _announcemodel


        ' Add any initialization after the InitializeComponent() call.

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

    Public Sub NotifyError(message As String) Implements ServiceReference1.IAideServiceCallback.NotifyError

    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements ServiceReference1.IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements ServiceReference1.IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements ServiceReference1.IAideServiceCallback.NotifySuccess

    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements ServiceReference1.IAideServiceCallback.NotifyUpdate

    End Sub

    Public Function getDataInsert(ByVal _announcementmodel As AnnouncementModel)
        Try
            'InitializeService()
            Dim textRange As New TextRange(txtAnnouncementMessage.Document.ContentStart, txtAnnouncementMessage.Document.ContentEnd)
            If Not IsNothing(textRange.Text) Or Not _announcementmodel.TITLE = Nothing Then
                _announce.MESSAGE = textRange.Text
                _announce.TITLE = _announcementmodel.TITLE
                _announce.END_DATE = DateTime.Today
                _announce.EMP_ID = Me.empID
            End If
            Return _announce
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return ex
        End Try
    End Function

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

    Private Sub btnAnnouncementCreate_Click(sender As Object, e As RoutedEventArgs)
        Try
            'InitializeService()
            Dim textRange As New TextRange(txtAnnouncementMessage.Document.ContentStart, txtAnnouncementMessage.Document.ContentEnd)

            If _announcemodel.TITLE = Nothing Or textRange.Text.Trim() = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                AideClient.GetClient().InsertAnnouncements(getDataInsert(Me.DataContext()))
                MsgBox("Announcement has been added.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
End Class
