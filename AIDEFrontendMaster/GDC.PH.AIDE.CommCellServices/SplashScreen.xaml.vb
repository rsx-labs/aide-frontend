'
Public Class SplashScreen
    Private _dataDict As Dictionary(Of Integer, SplashData) =
        New Dictionary(Of Integer, SplashData)

    Public Sub New(userName As String)

        ' This call is required by the designer.
        InitializeComponent()

        DataContext = GetData(userName)
    End Sub

    Private Function GetData(userName As String) As Object
        Dim data As SplashData = New SplashData()

        data.UserName = userName

        Select Case DateTime.Now.Hour
            Case <= 4
                data.Message = "Good morning,"
                data.ImageSource = ".\Assets\Images\night.png"
                data.Background = "#616161"
                data.Foreground = "#fafafa"
            Case <= 7
                data.Message = "Good morning,"
                data.ImageSource = ".\Assets\Images\sunrise.png"
                data.Background = "#f44336"
                data.Foreground = "#fafafa"
            Case <= 11
                data.Message = "Good morning,"
                data.ImageSource = ".\Assets\Images\morning.png"
                data.Background = "#ffebee"
                data.Foreground = "#4e342e"
            Case <= 14
                data.Message = "Good afternoon,"
                data.ImageSource = ".\Assets\Images\noon.png"
                data.Background = "#ffebee"
                data.Foreground = "#4e342e"
            Case <= 17
                data.Message = "Good afternoon,"
                data.ImageSource = ".\Assets\Images\afternoon.png"
                data.Background = "#f44336"
                data.Foreground = "#fafafa"
            Case <= 20
                data.Message = "Good evening,"
                data.ImageSource = ".\Assets\Images\evening.png"
                data.Background = "#b71c1c"
                data.Foreground = "#fafafa"
            Case Else
                data.Message = "Good evening,"
                data.ImageSource = ".\Assets\Images\night.png"
                data.Background = "#616161"
                data.Foreground = "#fafafa"
        End Select

        Return data
    End Function
End Class

Public Class SplashData
    Public Property Message As String
    Public Property ImageSource As String
    Public Property Background As String
    Public Property Foreground As String
    Public Property UserName As String

End Class
