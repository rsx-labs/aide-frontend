Imports UI_AIDE_CommCellServices.ServiceReference1

Class BillabilitySubMenu
    Private _pFrame As New Frame
    Private _profile As New Profile

    Public Sub New(pFrame As Frame, __profile As Profile)
        _pFrame = pFrame
        _profile = __profile
        InitializeComponent()
    End Sub

    Private Sub NonBillables_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New BillabilityPage(_profile, _pFrame))
    End Sub

    Private Sub VacationLeave_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New BillabilityVacationLeavePage(_profile, _pFrame))
    End Sub

    Private Sub SickLeave_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New BillabilitySickLeavePage(_profile, _pFrame))
    End Sub

    Private Sub Billables_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New BillablesPage(_profile, _pFrame))
    End Sub
End Class
