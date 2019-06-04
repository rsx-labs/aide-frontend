Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class ComcellDBProvider
    Private _comcellList As ObservableCollection(Of MyComcell)
    Private client As AideServiceClient

    Public Sub New()
        _comcellList = New ObservableCollection(Of MyComcell)
    End Sub

    Public Function GetMyComcell() As ObservableCollection(Of MyComcell)
        Return _comcellList
    End Function

    Public Sub SetMyComcell(ByVal _comcell As Comcell)
        Dim _comcellObject As MyComcell = New MyComcell With {
                .COMCELL_ID = _comcell.COMCELL_ID,
                .EMP_ID = _comcell.EMP_ID,
                .MONTH = _comcell.MONTH,
                .FACILITATOR = _comcell.FACILITATOR,
                .MINUTES_TAKER = _comcell.MINUTES_TAKER,
                .FY_START = _comcell.FY_START,
                .FY_END = _comcell.FY_END
                }

        _comcellList.Add(_comcellObject)
    End Sub

End Class

Public Class MyComcell
    Property COMCELL_ID As Integer
    Property EMP_ID As Integer
    Property MONTH As String
    Property FACILITATOR As String
    Property MINUTES_TAKER As String
    Property SCHEDULE As Date
    Property FY_START As DateTime
    Property FY_END As DateTime
End Class