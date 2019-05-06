Imports System.Windows.Forms
Imports System.Drawing

Public Class Birthday
    Private nIcon As NotifyIcon = New NotifyIcon()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.WindowState = System.Windows.WindowState.Minimized
        Me.nIcon.Icon = New Icon("../../../Assets/Icon/FujitsuLogo.png")
        Me.nIcon.ShowBalloonTip(5000, "Today's Birthday is Jhunell", "This is a BallonTip from Windows Notification", ToolTipIcon.Info)
    End Sub
End Class
