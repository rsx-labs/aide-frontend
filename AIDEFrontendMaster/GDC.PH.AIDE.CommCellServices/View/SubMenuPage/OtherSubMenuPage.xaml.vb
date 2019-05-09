Class OtherSubMenuPage
    Private email As String
    Private pageframe As Frame
    Public Sub New(_pageframe As Frame, _email As String)

        ' This call is required by the designer.
        InitializeComponent()
        Me.email = _email
        Me.pageframe = _pageframe
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Birthday_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New BirthdayPage(pageframe, email))
    End Sub

    Private Sub Announcement_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Learning_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
