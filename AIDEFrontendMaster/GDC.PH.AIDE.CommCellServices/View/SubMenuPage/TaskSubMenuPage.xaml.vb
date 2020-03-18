Imports UI_AIDE_CommCellServices.ServiceReference1

Class TaskSubMenuPage
    Private pFrame As New Frame
    Private profile As New Profile
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private currentWindow As MainWindow

    Public Sub New(_pFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _mainWindow As MainWindow)
        pFrame = _pFrame
        profile = _profile
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        currentWindow = _mainWindow
        InitializeComponent()
    End Sub

    Private Sub Task_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New TaskAdminPage(pFrame, currentWindow, profile, addframe, menugrid, submenuframe))
    End Sub

    Private Sub Weekly_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New WeeklyReportPage(pFrame, profile, addframe, menugrid, submenuframe))
    End Sub

End Class
