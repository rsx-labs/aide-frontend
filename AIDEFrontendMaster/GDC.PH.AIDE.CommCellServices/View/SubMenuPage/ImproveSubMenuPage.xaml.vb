Imports UI_AIDE_CommCellServices.ServiceReference1

Class ImproveSubMenuPage
    Private _MainFrame As New Frame
    Private _email As String
    Private _addframe As New Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile
    Public Sub New(main As Frame, email As String, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        Me._addframe = _addframe
        Me._MainFrame = main
        Me._email = email
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        Me.profile = _profile
        InitializeComponent()
    End Sub


    Private Sub _3CBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New ThreeC_Page(_email, _MainFrame, _addframe, _menugrid, _submenuframe))
    End Sub

    Private Sub ActionlistBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New HomeActionListsPage(_MainFrame, _email, _addframe, _menugrid, _submenuframe, Me.profile))
    End Sub

    Private Sub LessonLearntBtn_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New LessonLearntPage(_MainFrame, _email, _addframe, _menugrid, _submenuframe, profile))
    End Sub

    Private Sub SuccessRegister_Click(sender As Object, e As RoutedEventArgs)
        _MainFrame.Navigate(New SuccessRegisterPage(_MainFrame, _email, _addframe, _menugrid, _submenuframe))
    End Sub
End Class
