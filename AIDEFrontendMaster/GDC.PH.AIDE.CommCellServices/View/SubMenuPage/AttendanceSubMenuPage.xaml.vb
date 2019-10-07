Imports UI_AIDE_CommCellServices.ServiceReference1

Class AttendanceSubMenuPage
    Private pFrame As New Frame
    Private profile As New Profile
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private AttendanceFrame As Frame

    Public Sub New(_pFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)
        pFrame = _pFrame
        profile = _profile
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        AttendanceFrame = _attendanceFrame
        InitializeComponent()
    End Sub

    Private Sub ResourcePlanner_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New ResourcePlannerPage(profile, pFrame, addframe, menugrid, submenuframe, AttendanceFrame))
    End Sub

    Private Sub VacationLeave_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New BillabilityVacationLeavePage(profile, pFrame, addframe, menugrid, submenuframe, AttendanceFrame))
    End Sub

    Private Sub SickLeave_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New BillabilitySickLeavePage(profile, pFrame))
    End Sub



End Class
