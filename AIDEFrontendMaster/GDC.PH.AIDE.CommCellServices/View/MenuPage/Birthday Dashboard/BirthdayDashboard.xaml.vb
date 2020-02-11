Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class BirthdayDashboard
    Implements ServiceReference1.IAideServiceCallback

#Region "constructor"
    Private _AideService As ServiceReference1.AideServiceClient
    Private isEmpty As Boolean
    Private email As String
    Private month As Integer = Date.Now.Month
    Private displayMonth As String
    Dim lstBirthdayToday As BirthdayList()
    Dim birthdayListVM As New BirthdayListViewModel()


    Public Sub New(_email As String)

        ' This call is required by the designer.
        InitializeComponent()
        email = _email
        SetData()
        Me.DataContext = birthdayListVM
        ' Add any initialization after the InitializeComponent() call.

    End Sub
#End Region


#Region "common functions"
    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
        Return bInitialize
    End Function




    Public Sub SetData()
        Try
            If InitializeService() Then
                lstBirthdayToday = _AideService.ViewBirthdayListByCurrentDay(email)
                LoadDataDaily()
                If lstBirthdayToday.Length = 0 Then
                    Me.BdayTitle.Text = "No Birthday Today!"
                    Me.Bdayquote.Visibility = Windows.Visibility.Collapsed
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadDataDaily()
        Try
            Dim lstBirthdayList As New ObservableCollection(Of BirthdayListModel)
            Dim birthdayListDBProvider As New BirthdayListDBProvider

            For Each objBirthday As BirthdayList In lstBirthdayToday
                birthdayListDBProvider.SetMyBirthdayListToday(objBirthday)
            Next

            For Each rawUser As MyBirthdayList In birthdayListDBProvider.GetMyBirthdayListToday()
                lstBirthdayList.Add(New BirthdayListModel(rawUser))
            Next

            birthdayListVM.BirthdayListDay = lstBirthdayList
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region

#Region "service methods"
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
#End Region

End Class
