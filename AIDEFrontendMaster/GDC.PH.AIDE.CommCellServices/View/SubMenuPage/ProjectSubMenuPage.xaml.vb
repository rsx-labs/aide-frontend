Class ProjectSubMenuPage
    Private _pFrame As New Frame
    Public _empID As Integer
    Public _email As String
    Private _addframe As New Frame
    Private _menugrid As Grid
    Private _submenuframe As Frame
    Public Sub New(pFrame As Frame, empID As Integer, email As String, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        _pFrame = pFrame
        _empID = empID
        _email = email
        Me._addframe = _addframe
        Me._menugrid = _menugrid
        Me._submenuframe = _submenuframe
        InitializeComponent()
    End Sub

    Private Sub AssignedProject_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New ViewProjectUI(_pFrame, _email, _addframe, _menugrid, _submenuframe, _empID))
    End Sub

    Private Sub CreateProject_Click(sender As Object, e As RoutedEventArgs)
        _pFrame.Navigate(New CreateProjectPage(_pFrame, _empID))
    End Sub
End Class
