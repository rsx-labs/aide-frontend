Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class BirthdayPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Fields"

    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As Frame
    Private isEmpty As Boolean
    Private email As String
    Private month As Integer = Date.Now.Month
    Private displayMonth As String
    Dim lstBirthday As BirthdayList()
    Dim lstBirthdayMonth As BirthdayList()
    Dim birthdayListVM As New BirthdayListViewModel()

    Private Enum PagingMode
        _First = 1
        _Next = 2
        _Previous = 3
        _Last = 4
    End Enum

#End Region

#Region "Constructor"

    Public Sub New(mainFrame As Frame, _email As String)

        InitializeComponent()
        Me.email = _email
        Me.mainFrame = mainFrame
        SetData()
        Me.DataContext = birthdayListVM
    End Sub

#End Region

#Region "Events"

    Private Sub cbFilterDsiplay_DropDownClosed(sender As Object, e As EventArgs) Handles cbFilterDsiplay.DropDownClosed
        If cbFilterDsiplay.Text = "Month" And lstBirthdayMonth.Length = 0 Then
            btnPrint.IsEnabled = False
        Else
            btnPrint.IsEnabled = True
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        Dim dialog As PrintDialog = New PrintDialog()

        If cbFilterDsiplay.Text = "Month" Then
            If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
                dialog.PrintTicket.PageOrientation = PageOrientation.Portrait
                dialog.PrintVisual(gridMonth, "Print Birthday For This Month")
            End If
        Else
            If CBool(dialog.ShowDialog().GetValueOrDefault()) Then
                dialog.PrintTicket.PageOrientation = PageOrientation.Portrait
                dialog.PrintVisual(lv_birthdayYear, "Print Birthday For This Year")
            End If
        End If
    End Sub

    Private Sub lv_birthdayMonth_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles lv_birthdayMonth.LoadingRow
        Dim RowDataContaxt As BirthdayListModel = TryCast(e.Row.DataContext, BirthdayListModel)

        If RowDataContaxt IsNot Nothing Then
            If RowDataContaxt.BIRTHDAY = DateTime.Now.ToString("M") Then
                e.Row.Background = New SolidColorBrush(Colors.Red)
                e.Row.Foreground = New SolidColorBrush(Colors.White)
            End If
        End If
    End Sub
#End Region

#Region "Functions"

    Public Sub SetData()
        Try
            'If InitializeService() Then
            lstBirthday = AideClient.GetClient().ViewBirthdayListAll(email)
            lstBirthdayMonth = AideClient.GetClient().ViewBirthdayListByCurrentMonth(email)
            LoadData()
                LoadDataMonthly()
                If lstBirthdayMonth.Length = 0 Then
                    Me.lbl_headerMonth.Foreground = Brushes.White
                    Me.lbl_headerMonth.Text = "No Birthday Celebrants this " + SetMonths()
                Else
                    Me.lbl_headerMonth.Text = "Birthday Celebrant(s) this " + SetMonths() + ": " + lstBirthdayMonth.Length.ToString
                End If
            'End If
            If cbFilterDsiplay.Text = String.Empty Then
                btnPrint.IsEnabled = False
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadData()
        Try
            Dim lstBirthdayList As New ObservableCollection(Of BirthdayListModel)
            Dim birthdayListDBProvider As New BirthdayListDBProvider

            For Each objBirthday As BirthdayList In lstBirthday
                birthdayListDBProvider.SetMyBirthdayList(objBirthday)
            Next

            For Each rawUser As MyBirthdayList In birthdayListDBProvider.GetMyBirthdayList()
                lstBirthdayList.Add(New BirthdayListModel(rawUser))
            Next

            birthdayListVM.BirthdayList = lstBirthdayList
            Me.DataContext = birthdayListVM
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadDataMonthly()
        Try
            Dim lstBirthdayList As New ObservableCollection(Of BirthdayListModel)
            Dim birthdayListDBProvider As New BirthdayListDBProvider

            For Each objBirthday As BirthdayList In lstBirthdayMonth
                birthdayListDBProvider.SetMyBirthdayListMonth(objBirthday)
            Next

            For Each rawUser As MyBirthdayList In birthdayListDBProvider.GetMyBirthdayListMonth()
                lstBirthdayList.Add(New BirthdayListModel(rawUser))
            Next

            birthdayListVM.BirthdayListMonth = lstBirthdayList
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

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

    Public Function SetMonths()
        Select Case month
            Case "1"
                displayMonth = "January"
            Case "2"
                displayMonth = "Febuary"
            Case "3"
                displayMonth = "March"
            Case "4"
                displayMonth = "April"
            Case "5"
                displayMonth = "May"
            Case "6"
                displayMonth = "June"
            Case "7"
                displayMonth = "July"
            Case "8"
                displayMonth = "August"
            Case "9"
                displayMonth = "September"
            Case "10"
                displayMonth = "October"
            Case "11"
                displayMonth = "November"
            Case "12"
                displayMonth = "December"
        End Select
        Return displayMonth
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
