Imports System.Configuration
Public Class AppState

    Private _useOutlook As Boolean = True
    Public ReadOnly Property UseOutlook As Boolean
        Get
            Dim temp As String = ConfigurationManager.AppSettings("enableOutlook")
            Return If(String.Compare(temp.ToLower, "true") = 0, True, False)
        End Get
    End Property

    Private _enableNotification As Boolean = False
    Public ReadOnly Property EnableNotification As Boolean
        Get
            Dim temp As String = CBool(ConfigurationManager.AppSettings("enableNotification"))
            Return If(String.Compare(temp.ToLower, "true") = 0, True, False)
        End Get
    End Property

    Private _useForPublicBoard As Boolean = False
    Public ReadOnly Property UseForPublicBoard As Boolean
        Get
            Dim temp As String = CBool(ConfigurationManager.AppSettings("useForPublicBoard"))
            Return If(String.Compare(temp.ToLower, "true") = 0, True, False)
        End Get
    End Property

    Public CurrentMenu As Integer
    Public CurrentSubMenu As Integer

End Class
