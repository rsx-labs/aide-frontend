Imports System.Configuration
Public Class AppState
    Private Shared _instance As AppState

    Private Sub New()

    End Sub

    Public Shared Function GetInstance() As AppState
        Dim lock As Object = New Object()
        If (_instance Is Nothing) Then
            ' Mark a critical section where 
            ' threads take turns to execute
            SyncLock (lock)
                If (_instance Is Nothing) Then
                    _instance = New AppState()
                End If
            End SyncLock
        End If
        Return _instance
    End Function

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

    Public OptionValueDictionary As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
    Public OptionDictionary As Dictionary(Of Integer, OptionModel) = New Dictionary(Of Integer, OptionModel)()


End Class
