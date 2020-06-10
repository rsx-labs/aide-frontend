Imports Microsoft.Win32
Imports System.Xml
Public NotInheritable Class Helpers

    Public Shared Function GetCurrentVersionFromRegistry() As String
        Const KEY As String = "HKEY_CURRENT_USER\Software\RS Experimental Labs\AIDE CommCell"
        Try
            Return Registry.GetValue(KEY, "version", "")
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

End Class
