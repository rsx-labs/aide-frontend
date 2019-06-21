Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1


Public Class SendCodeDBProvider
    Private _obSendCode As mySendCodeSet


    Public Sub New()
        _obSendCode = New mySendCodeSet
    End Sub

    Public Function _getobSendCode() As mySendCodeSet
        Return _obSendCode
    End Function

    Public Function _setlistofitems(ByRef sendcode As SendCode, personalEmail As String)
        _obSendCode._sendcodeWorkEmail = sendcode.Work_Email
        _obSendCode._sendcodeFName = sendcode.FName
        _obSendCode._sendcodeLName = sendcode.LName
        _obSendCode._sendcodePersonalEmail = personalEmail
        Return _obSendCode
    End Function
End Class

Public Class mySendCodeSet
    Property _sendcodeWorkEmail As String
    Property _sendcodeFName As String
    Property _sendcodeLName As String
    Property _sendcodePersonalEmail As String

End Class
