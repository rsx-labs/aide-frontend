Imports System.Net.Mail
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports System.Windows.Threading
Imports System.Windows.Media.Animation
Imports System.Media
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Text.RegularExpressions

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Class EmailCodeRequest
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    Private email_frame As Frame
    Private codecombo As Integer
    Private mainwindow As AddEmailWindow
    Private mailConfig As New MailConfig
    Private aide As ServiceReference1.AideServiceClient
    Private mailConfigVM As New MailConfigViewModel
    Private sendVM As New SendCodeViewModel
    Private TimeExpire As Integer
    Private inMinute As TimeSpan
#End Region

#Region "Constructor"
    Public Sub New(emailFrame As Frame, codecom As Integer, main As AddEmailWindow, _sendViewModel As SendCodeViewModel)
        ' This call is required by the designer.
        InitializeComponent()
        'aide = aideService
        mailConfigVM = New MailConfigViewModel()
        sendVM = _sendViewModel
        email_frame = emailFrame
        codecombo = codecom
        mainwindow = main
        GetData()
        SendCodeRequest(sendVM.objectSendCodeSet2.PERSONAL_EMAIL, mailConfigVM)
        readyUI()
        expiryStarts()
        ' Add any initialization after the InitializeComponent() call.
    End Sub
#End Region

#Region "Main Method"
    Private Function GetRandomCode() As Integer
        Dim CodeString As String = String.Empty
        Dim CodeInt As Integer

        Do While CodeString.Length < 6
            Dim ran As New Random
            CodeInt = ran.Next(1, 10)
            If CodeString.Contains(CodeInt.ToString()) Then
                Continue Do
            End If
            CodeString += CodeInt.ToString()
        Loop


        Return CInt(CodeString)
    End Function

    Public Sub expiryStarts()
        Dim timer As DispatcherTimer = New DispatcherTimer(New TimeSpan(0, 0, 1), DispatcherPriority.Normal, Function()

                                                                                                                 TimeExpire -= 1
                                                                                                                 inMinute = New TimeSpan(0, 0, TimeExpire)
                                                                                                                 exptb.Text = inMinute.ToString("mm") + ":" + inMinute.ToString("ss")
                                                                                                                 If TimeExpire = 10 Then
                                                                                                                     exptb.Foreground = New SolidColorBrush(Colors.Red)
                                                                                                                 End If

                                                                                                                 If TimeExpire = 0 Then

                                                                                                                     timer.Stop()
                                                                                                                     NewCodeBtn.IsEnabled = True
                                                                                                                     codecombo = 0
                                                                                                                     ExpireTitle.Text = "Code Expires!"
                                                                                                                 End If
                                                                                                             End Function, Me.Dispatcher)

    End Sub

    Private Sub GetData()
        Try
            'If InitializeService() Then
            mailConfig = AideClient.GetClient().GetMailConfig()
            LoadData()
            'End If

        Catch ex As Exception

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub


    Public Sub readyUI()
        TimeExpire = mailConfigVM.objectMailConfigSet.PASSWORD_EXPIRY
        Emailsent.Text = sendVM.objectSendCodeSet2.PERSONAL_EMAIL
    End Sub
    Private Sub LoadData()

        Dim MConfigModel As New MailConfigModel
        Dim MConfigProvider As New MailConfigDBProvider

        Try
            MConfigProvider._setlistofitems(mailConfig)
            MConfigModel = New MailConfigModel(MConfigProvider._getobjmailconfig)

            mailConfigVM.objectMailConfigSet = MConfigModel

        Catch ex As Exception

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SendCodeRequest(pemail As String, mcVM As MailConfigViewModel)
        Try
            Dim sentTo As String = pemail
            Dim sentFrom As String = mcVM.objectMailConfigSet.SENDER_EMAIL
            Dim subject As String = mcVM.objectMailConfigSet.SUBJECT
            Dim body As String = mcVM.objectMailConfigSet.BODY + " " + codecombo.ToString()
            Dim client As SmtpClient = New SmtpClient()

            client.Port = mcVM.objectMailConfigSet.PORT
            client.Host = mcVM.objectMailConfigSet.HOST
            client.EnableSsl = CBool(mcVM.objectMailConfigSet.ENABLE_SSL)
            client.Timeout = mcVM.objectMailConfigSet.TIMEOUT
            client.DeliveryMethod = SmtpDeliveryMethod.Network
            client.UseDefaultCredentials = CBool(mcVM.objectMailConfigSet.USE_DFLT_CREDENTIALS)
            client.Credentials = New System.Net.NetworkCredential(sentFrom, mcVM.objectMailConfigSet.SENDER_PASSWORD)

            Dim mail As MailMessage = New MailMessage()
            mail.From = New MailAddress(sentFrom)
            mail.To.Add(sentTo)
            mail.Subject = subject
            mail.Body = body

            client.Send(mail)
        Catch ex As Exception
             MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Function appendDigits() As Integer
        Dim AppendedDigits As Integer
        Try
            AppendedDigits = CInt(Num1.Text + Num2.Text + Num3.Text + Num4.Text + Num5.Text + Num6.Text)
        Catch ex As Exception

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return 0
        End Try

        Return AppendedDigits
    End Function

    Private Sub clearNumFields()
        Num1.Text = String.Empty
        Num2.Text = String.Empty
        Num3.Text = String.Empty
        Num4.Text = String.Empty
        Num5.Text = String.Empty
        Num6.Text = String.Empty
    End Sub
#End Region

#Region "Service Methods"
    'Public Function InitializeService() As Boolean
    '    'Dim bInitialize As Boolean = False
    '    'Try
    '    '    Dim Context As InstanceContext = New InstanceContext(Me)
    '    '    aide = New AideServiceClient(Context)
    '    '    aide.Open()
    '    '    bInitialize = True
    '    'Catch ex As SystemException
    '    '    aide.Abort()
    '    '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    'End Try
    '    'Return bInitialize
    '    Return True
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
#End Region
    
#Region "Text Input Validation"
    Private Sub Num1_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub

    Private Sub Num2_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub

    Private Sub Num3_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub

    Private Sub Num4_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub

    Private Sub Num5_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub

    Private Sub Num6_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim _regex As Regex = New Regex("[^0-9]+")
        e.Handled = _regex.IsMatch(e.Text)
    End Sub
#End Region

#Region "Event Handling"
    Private Sub BackBtn_Click(sender As Object, e As RoutedEventArgs)
        If MsgBox("A new access code will be required for your next login once you exit.", vbYesNo, "AIDE") = vbYes Then
            email_frame.Navigate(New AddEmailPage(email_frame, mainwindow))
        End If
    End Sub

    Private Sub NewCodeBtn_Click(sender As Object, e As RoutedEventArgs)
        codecombo = GetRandomCode()
        If Not codecombo = 0 Then
            Try
                SendCodeRequest(sendVM.objectSendCodeSet2.PERSONAL_EMAIL, mailConfigVM)
                MsgBox("New access code has been sent to your email.", vbOKOnly, "AIDE")
                NewCodeBtn.IsEnabled = False
                TimeExpire = mailConfigVM.objectMailConfigSet.PASSWORD_EXPIRY
                expiryStarts()
                exptb.Foreground = New SolidColorBrush(Colors.Black)
                ExpireTitle.Text = "Code expires in"
            Catch ex As Exception

                MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            End Try

        End If

    End Sub

    Private Sub ApplyBtn_Click(sender As Object, e As RoutedEventArgs)
        Dim CodeEntry As Integer
        CodeEntry = appendDigits()
        If Not codecombo = 0 Then
            If CheckEntry() Then
                If Not CodeEntry = codecombo Then
                    MsgBox("Please enter a valid code.", MsgBoxStyle.Exclamation, "AIDE")
                    clearNumFields()
                Else
                    mainwindow.GetEmail = sendVM.objectSendCodeSet2.WORK_EMAIL
                    mainwindow.Close()
                End If
            Else
                MsgBox("Please enter all required fields. Ensure all required fields have * indicated.", MsgBoxStyle.Exclamation, "AIDE")
            End If
            
        Else
            MsgBox("Access code is expired. Please request for a new access code.", vbOKOnly, "AIDE")
        End If

    End Sub
    Private Function CheckEntry() As Boolean
        If Num1.Text = String.Empty OrElse Num2.Text = String.Empty OrElse Num3.Text = String.Empty OrElse Num4.Text = String.Empty OrElse _
            Num5.Text = String.Empty OrElse Num6.Text = String.Empty Then
            Return False
        End If
        Return True
    End Function
#End Region

End Class
