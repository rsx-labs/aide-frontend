Imports UI_AIDE_CommCellServices.ServiceReference1
Class OtherSubMenuPage
    Private email As String
    Private pageframe As Frame
    Private profile As Profile
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame

    Public Sub New(_pageframe As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        ' This call is required by the designer.
        InitializeComponent()
        Me.email = _profile.Email_Address
        Me.pageframe = _pageframe
        Me.addframe = _addframe
        Me.menugrid = _menugrid
        Me.submenuframe = _submenuframe
        Me.profile = _profile
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub Birthday_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New BirthdayPage(pageframe, email))
    End Sub

 	Private Sub Announcement_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Learning_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New SabaLearningMainPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub Comcell_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New ComcellMainPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub


    Private Sub Reports_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New ReportsMainPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub
End Class
