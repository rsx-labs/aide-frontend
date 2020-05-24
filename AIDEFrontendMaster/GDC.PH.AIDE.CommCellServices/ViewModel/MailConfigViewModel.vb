Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Net.Mail
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class MailConfigViewModel
    Implements INotifyPropertyChanged

    Private _objectMailConfigSet As New MailConfigModel
    Private MailConfigProvider As New MailConfigDBProvider
    Private _OptionsViewModel As New OptionViewModel
    'Private _client As AideServiceClient

    'Public Sub New()
    '    '_client = aideService
    '    _objectMailConfigSet = New MailConfigModel(MailConfigProvider._getobjmailconfig)
    'End Sub

    Public Sub New()
        _objectMailConfigSet = New MailConfigModel(MailConfigProvider._getobjmailconfig)
    End Sub

    Public Property objectMailConfigSet As MailConfigModel
        Get
            Return _objectMailConfigSet
        End Get
        Set(value As MailConfigModel)
            _objectMailConfigSet = value
            NotifyPropertyChanged("objectMailConfigSet")
        End Set
    End Property
    Public Function isSendEmail(ByVal optID As Integer, ByVal moduleID As Integer, ByVal funcID As Integer) As Boolean
        Try
            Dim allowSend As Boolean = False
            _OptionsViewModel = New OptionViewModel
            '_OptionsViewModel.Service = _client
            If _OptionsViewModel.GetOptions(optID, moduleID, funcID) Then
                For Each opt As OptionModel In _OptionsViewModel.OptionList
                    If CBool(opt.VALUE) Then
                        allowSend = True
                    End If
                Next
            End If
            Return allowSend
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return False
        End Try
    End Function
    Public Function composeBody(ByVal optmodel As OptionModel, ByVal choice As Integer, Optional ByVal objOptional As Object = Nothing, Optional ByVal objOptional2 As Object = Nothing) As String
        Dim body As String
        Dim bodyList As New List(Of String)(optmodel.VALUE.Split(","c))
        Dim strOption As String = String.Empty
        Dim strOptionLst As List(Of String) = Nothing
        Dim strOption2 As String = String.Empty
        Dim strOptionLst2 As List(Of String) = Nothing

        If Not objOptional Is Nothing Then
            strOption = objOptional
            strOptionLst = New List(Of String)(strOption.Split(","c))
        End If

        If Not objOptional2 Is Nothing Then
            strOption2 = objOptional2
            strOptionLst2 = New List(Of String)(strOption2.Split(","c))
        End If

        Select Case choice
            Case 1
                body = "<html><body><div style=""margin:30px 0px""><center><div style=""background-color:steelblue""><font size=""6"" color=""white"">" + optmodel.MODULE_DESCR + " - " + optmodel.FUNCTION_DESCR + "</font></div><div style=""background-color:#fcfff9""><font size=""4"">" + bodyList(0) + " " + strOption.ToString() + " " + bodyList(1) + "</font></div></center></div></body></html>"
            Case 2
                Dim optList As String = ""
                For Each objopt As String In strOptionLst
                    optList = optList + "<tr><td><center>" + objopt + "</center></td></tr>"
                Next
                body = "<html><body><div style=""margin:30px 0px""><center><div style=""background-color:steelblue""><font size=""6"" color=""white"">" + optmodel.MODULE_DESCR + " - " + optmodel.FUNCTION_DESCR + "</font></div><div style=""background-color:#fcfff9""><font size=""4"">" + bodyList(0) + "</font><table style=""width:100%"">" + optList + "</table></div></center></div></body></html>"
            Case 3
                Dim optList As String = ""
                For Each objopt As String In strOptionLst
                    optList = optList + "<tr><td><center>" + objopt + "</center></td></tr>"
                Next
                body = "<html><body><div style=""margin:30px 0px""><center><div style=""background-color:steelblue""><font size=""6"" color=""white"">" + optmodel.MODULE_DESCR + " - " + optmodel.FUNCTION_DESCR + "</font></div><div style=""background-color:#fcfff9""><font size=""4"">" + bodyList(0) + " " + strOption2 + " " + bodyList(1) + "</font><table style=""width:100%"">" + optList + "</table></div></center></div></body></html>"
            Case 4
                Dim optList As String = ""
                For Each objopt As String In strOptionLst
                    optList = optList + "<tr><td><center>" + objopt + "</center></td></tr>"
                Next
                body = "<html><body><div style=""margin:30px 0px""><center><div style=""background-color:steelblue""><font size=""6"" color=""white"">" + optmodel.MODULE_DESCR + " - " + optmodel.FUNCTION_DESCR + "</font></div><div style=""background-color:#fcfff9""><font size=""4"">" + bodyList(0) + " " + strOptionLst2(0) + " " + bodyList(1) + " " + strOptionLst2(1) + " " + bodyList(2) + " " + strOptionLst2(2) + "." + "</font><table style=""width:100%"">" + optList + "</table></div></center></div></body></html>"
        End Select

        Return body
    End Function

    Public Sub SendEmail(ByVal mcVM As MailConfigViewModel, ByVal optmodel As OptionModel, emailTo As String, emailCC As String, ByVal bodyType As Integer, Optional ByVal optionaObj As Object = Nothing, Optional ByVal optionaObj2 As Object = Nothing)
        Try

            Dim lstSentTo As List(Of String) = Nothing
            If Not emailTo = String.Empty Then
                lstSentTo = New List(Of String)(emailTo.Split(","c))
            End If

            Dim lstSentCC As List(Of String) = Nothing
            If Not emailCC = String.Empty Then
                lstSentCC = New List(Of String)(emailCC.Split(","c))
            End If

            Dim sentFrom As String = mcVM.objectMailConfigSet.SENDER_EMAIL
            Dim subject As String = mcVM.objectMailConfigSet.SUBJECT

            Dim body As String = composeBody(optmodel, bodyType, optionaObj, optionaObj2)
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

            If Not lstSentTo Is Nothing Then
                For Each objSentTo As String In lstSentTo
                    mail.To.Add(objSentTo)
                Next
            End If

            If Not lstSentCC Is Nothing Then
                For Each objSentCC As String In lstSentCC
                    mail.CC.Add(objSentCC)
                Next
            End If

            mail.Subject = subject
            mail.IsBodyHtml = True
            mail.Body = body

            client.Send(mail)
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
