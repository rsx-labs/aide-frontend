Imports System.Data
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
Imports System.ServiceModel

Class KPITargetsAddPage
    Implements UI_AIDE_CommCellServices.ServiceReference1.IAideServiceCallback


    Private aide As ServiceReference1.AideServiceClient
    Private empID As Integer
    Private mainframe As Frame
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private _kpiModel As New KPITargetsModel
    Private _kpitargets As New KPITargets
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
        Me.DataContext = _kpiModel


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

    Public Function getDataInsert(ByVal kpiTarget As KPITargetsModel)
        Try

            Dim textRange As New TextRange(txtDescription.Document.ContentStart, txtDescription.Document.ContentEnd)
            If Not IsNothing(textRange.Text) Or Not kpiTarget.Subject = Nothing Then
                _kpitargets.KPI_Id = "0"
                _kpitargets.EmployeeId = Me.profile.Emp_ID
                _kpitargets.Description = textRange.Text
                _kpitargets.Subject = kpiTarget.Subject
                _kpitargets.DateCreated = Date.Now
                _kpitargets.KPI_ReferenceNo = Date.Now.Year.ToString()
                LoadYear()
            End If
            Return _kpitargets
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return ex
        End Try
    End Function

    Public Sub LoadYear()
        If Today.DayOfYear() <= CDate(Today.Year().ToString + "-03-31").DayOfYear Then
            _kpitargets.FYStart = Convert.ToDateTime((Date.Now.Year - 1).ToString & "-04-01")
            _kpitargets.FYEnd = Convert.ToDateTime(Date.Now.Year.ToString & "-03-31")
        Else
            _kpitargets.FYStart = Convert.ToDateTime((Date.Now.Year).ToString & "-04-01")
            _kpitargets.FYEnd = Convert.ToDateTime((Date.Now.Year + 1).ToString & "-03-31")
        End If
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
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs)
        Try
            InitializeService()
            Dim textRange As New TextRange(txtDescription.Document.ContentStart, txtDescription.Document.ContentEnd)

            If _kpiModel.Subject = Nothing Or textRange.Text.Trim() = String.Empty Then
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", vbOKOnly + MsgBoxStyle.Exclamation, "AIDE")
            Else
                aide.InsertKPITarget(getDataInsert(Me.DataContext()))
                MsgBox("KPI Target has been added.", vbOKOnly + MsgBoxStyle.Information, "AIDE")
                _kpitargets.KPI_Id = Nothing
                _kpitargets.Description = Nothing
                _kpitargets.Subject = Nothing
                _kpitargets.KPI_ReferenceNo = Nothing

                mainframe.Navigate(New HomePage(mainframe, profile.Position, empID, addframe, menugrid, submenuframe, email, profile, aide))
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
