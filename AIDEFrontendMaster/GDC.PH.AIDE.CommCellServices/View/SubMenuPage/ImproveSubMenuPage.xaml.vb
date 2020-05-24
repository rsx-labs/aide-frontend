Imports UI_AIDE_CommCellServices.ServiceReference1

Class ImproveSubMenuPage
    Private mainFrame As New Frame
    Private email As String
    Private addframe As New Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private profile As Profile
    'Private _client As AideServiceClient

    Public Sub New(_mainFrame As Frame, _email As String, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        mainFrame = _mainFrame
        email = _email
        profile = _profile
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        InitializeComponent()

        '_client = aideService
    End Sub

    Private Sub _3CBtn_Click(sender As Object, e As RoutedEventArgs)
        mainFrame.Navigate(New ThreeC_Page(profile, mainFrame, addframe, menugrid, submenuframe))
    End Sub

    Private Sub ActionlistBtn_Click(sender As Object, e As RoutedEventArgs)
        mainFrame.Navigate(New HomeActionListsPage(mainFrame, addframe, menugrid, submenuframe, profile))
    End Sub

    Private Sub LessonLearntBtn_Click(sender As Object, e As RoutedEventArgs)
        mainFrame.Navigate(New LessonLearntPage(mainFrame, addframe, menugrid, submenuframe, profile))
    End Sub

    Private Sub SuccessRegister_Click(sender As Object, e As RoutedEventArgs)
        mainFrame.Navigate(New SuccessRegisterPage(mainFrame, addframe, menugrid, submenuframe, profile))
    End Sub

    Private Sub ProblemSolvingBtn_Click(sender As Object, e As RoutedEventArgs) Handles ProblemSolvingBtn.Click
        mainFrame.Navigate(New ProblemSolvingPage(profile, mainFrame, addframe, menugrid, submenuframe))
    End Sub
End Class
