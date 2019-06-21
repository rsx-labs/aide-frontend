Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class MailConfigDBProvider
    Private _mailConfigSet As myMailConfigSet

    Public Sub New()
        _mailConfigSet = New myMailConfigSet
    End Sub

    Public Function _getobjmailconfig() As myMailConfigSet
        Return _mailConfigSet
    End Function

    Public Function _setlistofitems(ByRef mailconfig As MailConfig)
        _mailConfigSet.scbody = mailconfig.Body
        _mailConfigSet.scenablessl = mailconfig.EnableSSL
        _mailConfigSet.schost = mailconfig.Host
        _mailConfigSet.scport = mailconfig.Port
        _mailConfigSet.scsenderemail = mailconfig.SenderEmail
        _mailConfigSet.scsenderpassword = mailconfig.SenderPassword
        _mailConfigSet.scsubject = mailconfig.Subject
        _mailConfigSet.sctimeout = mailconfig.Timeout
        _mailConfigSet.scusedfltcredentials = mailconfig.UseDfltCredentials
        _mailConfigSet.scpasswordexpiry = mailconfig.PasswordExpiry
        Return _mailConfigSet
    End Function
End Class
Public Class myMailConfigSet
    Property scsenderemail As String
    Property scsubject As String
    Property scbody As String
    Property scport As Integer
    Property schost As String
    Property scenablessl As Integer
    Property sctimeout As Integer
    Property scusedfltcredentials As Integer
    Property scsenderpassword As String
    Property scpasswordexpiry As Integer
End Class