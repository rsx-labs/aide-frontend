Imports Microsoft.Win32
Imports System.Xml
Public NotInheritable Class Helpers

    Public Shared Function GetCurrentVersionFromRegistry(Optional AppType As Integer = 0) As String
        Dim key As String
        If AppType = Constants.APP_BACKEND Then
            key = "HKEY_CURRENT_USER\Software\RS Experimental Labs\AIDE Backend"
        Else
            key = "HKEY_CURRENT_USER\Software\RS Experimental Labs\AIDE CommCell"
        End If

        Try
            Return Registry.GetValue(key, "version", "")
        Catch ex As Exception
            Return String.Empty
        End Try

    End Function

    Public Shared Function GetUpdateFeedFromURL(URLString As String) As Xml.XmlDocument
        Dim feed As String = URLString + "aideupdate.xml"
        Dim doc As Xml.XmlDocument = New Xml.XmlDocument()

        Try
            doc.Load(feed)
            Return doc
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function GetMonthNumber(MonthName As String) As Integer
        Return DateTime.ParseExact(MonthName, "MMMM", System.Globalization.CultureInfo.CurrentCulture).Month
    End Function

    Public Shared Function GetFYStart(MonthNow As Integer) As Integer
        If MonthNow < 4 Then
            Return DateTime.Now.Year - 1
        Else
            Return DateTime.Now.Year
        End If
    End Function

    Public Shared Function IsCurrentVersionUpToDate(newVersionString As String, Optional appType As Integer = 0) As Boolean
        Dim result As Boolean = False
        Dim currentVersionString As String = GetCurrentVersionFromRegistry(appType)

        Try
            Dim currentVersion As Double = Double.Parse(currentVersionString.Replace(".", ""))
            Dim newVersion As Double = Double.Parse(newVersionString.Replace(".", ""))

            If newVersion > currentVersion Then
                result = False
            Else
                result = True
            End If

            Return result
        Catch ex As Exception
            Return False
        End Try

    End Function

End Class
