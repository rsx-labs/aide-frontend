Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class AddEmailWindow
    Private _emailaddress As String
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        EmailFrame.Navigate(New AddEmailPage(EmailFrame, Me))
        ' Add any initialization after the InitializeComponent() call.

    End Sub



    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub sendBtn_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()

    End Sub
    Public Property GetEmail As String
        Get
            Return _emailaddress
        End Get
        Set(value As String)
            _emailaddress = value
        End Set
    End Property
End Class
