Class OtherSubMenuPage
    Private email As String
    Private pageframe As Frame
    Private empID As Integer
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame

    Public Sub New(_pageframe As Frame, _email As String, _empID As Integer, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)

        ' This call is required by the designer.
        InitializeComponent()
        Me.email = _email
        Me.pageframe = _pageframe
        Me.empID = _empID
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Birthday_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New BirthdayPage(pageframe, email))
    End Sub

    Private Sub Announcement_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Learning_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New SabaLearningMainPage(pageframe, empID, addframe, menugrid, submenuframe))
    End Sub
End Class
