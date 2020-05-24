Imports UI_AIDE_CommCellServices.ServiceReference1
Imports NLog

Class AttendanceSubMenuPage
    Private pFrame As New Frame
    Private profile As New Profile
    Private addframe As Frame
    Private menugrid As Grid
    Private submenuframe As Frame
    Private AttendanceFrame As Frame

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Public Sub New(_pFrame As Frame, _profile As Profile, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame, _attendanceFrame As Frame)

        _logger.Debug("Start : Constructor")

        pFrame = _pFrame
        profile = _profile
        addframe = _addframe
        menugrid = _menugrid
        submenuframe = _submenuframe
        AttendanceFrame = _attendanceFrame
        InitializeComponent()

        _logger.Debug("End : Constructor")

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

    Private Sub Late_Click(sender As Object, e As RoutedEventArgs)
        pFrame.Navigate(New LatePage(pFrame, profile, addframe, menugrid, submenuframe))
    End Sub

End Class
