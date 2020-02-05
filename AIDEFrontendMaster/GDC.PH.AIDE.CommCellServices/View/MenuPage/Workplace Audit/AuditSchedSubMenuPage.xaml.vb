Imports UI_AIDE_CommCellServices.ServiceReference1
Class AuditSchedSubMenuPage
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

    Private Sub Schedule_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New AuditSchedMainPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub Daily_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New DailyAuditPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub Weekly_Click(sender As Object, e As RoutedEventArgs)
        'pageframe.Navigate(New AuditSchedMainPage(pageframe, profile, addframe, menugrid, submenuframe))
        pageframe.Navigate(New WeeklyAuditPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub Monthly_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New MonthlyAuditPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub Quarterly_Click(sender As Object, e As RoutedEventArgs)
        pageframe.Navigate(New QuarterlyAuditPage(pageframe, profile, addframe, menugrid, submenuframe))
    End Sub
End Class
