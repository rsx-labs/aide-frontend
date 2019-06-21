Imports System
Imports System.Net.Mail
Imports System.Text
Imports System.Net
Imports System.Net.Mime
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.IO.Ports
Imports System.IO
Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Text.RegularExpressions

Class AddEmailPage
    Implements IAideServiceCallback

    Private email_frame As Frame
    Private mainwindow As AddEmailWindow
    Private SendCodeViewModel As New SendCodeViewModel()
    Private send_code As SendCode
    Private email As String

    Private aide As ServiceReference1.AideServiceClient

    Public Sub New(emailframe As Frame, windows As AddEmailWindow)

        ' This call is required by the designer.
        InitializeComponent()
        email_frame = emailframe
        mainwindow = windows
        Me.DataContext = SendCodeViewModel
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub sendBtn_Click(sender As Object, e As RoutedEventArgs)

        Dim check As Boolean = False
        Dim CodeCombination As Integer
        If UserEmail.Text = String.Empty Then
            MsgBox("Please enter your personal email.", MsgBoxStyle.Exclamation, "AIDE")
        Else
            Try
                If CheckEmailValidity(UserEmail.Text) Then
                    check = checkEmailEntry()
                    If check Then
                        CodeCombination = getCode()
                        email_frame.Navigate(New EmailCodeRequest(email_frame, CodeCombination, mainwindow, SendCodeViewModel))
                    Else
                        MsgBox("Email is not a member of AIDE.", MsgBoxStyle.Exclamation, "AIDE")
                    End If
                Else
                    MsgBox("Email used is not valid.", MsgBoxStyle.Exclamation, "AIDE")
                End If
            Catch ex As Exception

            End Try
            
           
        End If

    End Sub

    Public Function checkEmailEntry() As Boolean
        Try
            If Me.InitializeService() Then
                send_code = aide.GetWorkEmailbyEmail(UserEmail.Text)
                If IsNothing(send_code) Then
                    Return False
                End If
                LoadEmployeeProfile(UserEmail.Text)
                Return True
            End If

        Catch ex As Exception

        End Try
    End Function

    Private Function CheckEmailValidity(email As String) As Boolean
        Static emailSetup As New Regex("^[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z]{2,4})$")

        Return emailSetup.IsMatch(email)
    End Function

    Public Sub LoadEmployeeProfile(personalEmail As String)
        Dim _SendCodeModel As New SendCodeModel
        Dim SendCodeProvider As New SendCodeDBProvider
        Dim getEmail As String = String.Empty

        Try
            SendCodeProvider._setlistofitems(send_code, personalEmail)
            _SendCodeModel = New SendCodeModel(SendCodeProvider._getobSendCode)

            If Not IsNothing(_SendCodeModel) Then
                SendCodeViewModel.objectSendCodeSet2 = _SendCodeModel
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Function getCode() As Integer
        Dim RandomCode As Integer

        RandomCode = GetRandomCode()
        Return RandomCode
    End Function

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

    Public Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function

#Region "Services Function/Method"

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

#End Region

    Private Sub cancelBtn_Click(sender As Object, e As RoutedEventArgs)
        Environment.Exit(0)
    End Sub
End Class
