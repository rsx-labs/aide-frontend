Public Class ComcellClockWindow
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Me.Focus()
        Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterScreen
        Me.Topmost = True
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class
