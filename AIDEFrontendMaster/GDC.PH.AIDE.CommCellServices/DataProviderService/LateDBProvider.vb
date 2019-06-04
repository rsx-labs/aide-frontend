Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class LateDBProvider
    Public _lateList As ObservableCollection(Of MyLate)
    Private client As AideServiceClient

    Public Sub New()
        _lateList = New ObservableCollection(Of MyLate)
    End Sub

    Public Function GetMyLate() As ObservableCollection(Of MyLate)
        Return _lateList
    End Function

    Public Sub SetMyLate(ByVal _late As Late)
        Dim _lateObject As MyLate = New MyLate With {
                .FIRST_NAME = _late.FIRST_NAME,
                .STATUS = _late.STATUS,
                .MONTH = _late.MONTH,
                .NO_OF_LATE = _late.NUMBER_OF_LATE
                }
        _lateList.Add(_lateObject)
    End Sub

End Class

Public Class MyLate
    Property FIRST_NAME As String
    Property STATUS As Integer
    Property DATE_ENTRY As Date
    Property MONTH As String
    Property NO_OF_LATE As Integer
End Class