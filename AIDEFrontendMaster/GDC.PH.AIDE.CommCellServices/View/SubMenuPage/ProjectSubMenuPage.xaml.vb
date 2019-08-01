Imports UI_AIDE_CommCellServices.ServiceReference1

Class ProjectSubMenuPage
    Private _pFrame As New Frame
    Private _addframe As New Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Private profile As Profile

    Public Sub New(pFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        _pFrame = pFrame
        profile = _profile
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        InitializeComponent()
    End Sub

    Private Sub AssignedProject_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New ViewProjectUI(_pFrame, profile, _addframe, _menugrid, _submenuframe))
    End Sub

    Private Sub CreateProject_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New CreateProjectPage(_pFrame, profile))
    End Sub
End Class
