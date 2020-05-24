Imports UI_AIDE_CommCellServices.ServiceReference1

Class BillabilitySubMenu
    Private pFrame As New Frame
    Private profile As New Profile
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private _client As AideServiceClient

    Public Sub New(_pFrame As Frame, _profile As Profile, _addframe As Frame,
                   _menugrid As Grid, _submenuframe As Frame, aideService As AideServiceClient)
        pFrame = _pFrame
        profile = _profile
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        InitializeComponent()

        _client = aideService
    End Sub

    Private Sub NonBillables_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New BillabilityPage(profile, pFrame, _client))
    End Sub

    Private Sub Billables_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New BillablesPage(profile, pFrame, _client))
    End Sub

    Private Sub KPISummary_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New KPISummaryPage(profile, pFrame, addframe, menugrid, submenuframe, _client))
    End Sub

End Class
