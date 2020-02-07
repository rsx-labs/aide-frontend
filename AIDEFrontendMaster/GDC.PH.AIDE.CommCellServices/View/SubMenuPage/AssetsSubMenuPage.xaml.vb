Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class AssetsSubMenuPage
    Private _pFrame As New Frame
    Private _profile As New Profile
    Private _addframe As New Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame

    Public Sub New(pFrame As Frame, __profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        _pFrame = pFrame
        _profile = __profile
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        InitializeComponent()
    End Sub

    Private Sub Asset_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetsListPage(_pFrame, _profile, _addframe, _menugrid, _submenuframe))
    End Sub

    Private Sub AssetInventory_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetsInventoryListPage(_pFrame, _profile, _addframe, _menugrid, _submenuframe, ""))
    End Sub

    Private Sub AssetHistory_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetsHistory(_pFrame, _profile))
    End Sub

    Private Sub AssetBorrowing_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New AssetBorrowingPage(_pFrame, _profile, _addframe, _menugrid, _submenuframe, ""))
    End Sub
End Class
