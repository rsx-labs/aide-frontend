Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class ComcellDBProvider
    Private _comcellList As ObservableCollection(Of MyComcell)
    Private _comcellItem As MyComcell
    Private client As AideServiceClient

    Public Sub New()
        _comcellList = New ObservableCollection(Of MyComcell)
        _comcellItem = New MyComcell
    End Sub

    Public Function GetMyComcell() As ObservableCollection(Of MyComcell)
        Return _comcellList
    End Function

    Public Function GetMyComcellItem() As MyComcell
        Return _comcellItem
    End Function


    Public Sub SetMyComcell(ByVal _comcell As Comcell)
        Dim _comcellObject As MyComcell = New MyComcell With {
                .COMCELL_ID = _comcell.COMCELL_ID,
                .EMP_ID = _comcell.EMP_ID,
                .MONTH = _comcell.MONTH,
                .FACILITATOR = _comcell.FACILITATOR,
                .MINUTES_TAKER = _comcell.MINUTES_TAKER,
                .FY_START = _comcell.FY_START,
                .FY_END = _comcell.FY_END,
                .FACILITATOR_NAME = _comcell.FACILITATOR_NAME,
                .MINUTES_TAKER_NAME = _comcell.MINUTES_TAKER_NAME,
                .WEEK = _comcell.WEEK,
                .WEEK_START = _comcell.WEEK_START
                }

        _comcellList.Add(_comcellObject)
    End Sub

    Public Sub SetMyComcellItem(ByVal _comcell As Comcell)
        _comcellItem.COMCELL_ID = _comcell.COMCELL_ID
        _comcellItem.EMP_ID = _comcell.EMP_ID
        _comcellItem.MONTH = _comcell.MONTH
        _comcellItem.FACILITATOR = _comcell.FACILITATOR
        _comcellItem.MINUTES_TAKER = _comcell.MINUTES_TAKER
        _comcellItem.FY_START = _comcell.FY_START
        _comcellItem.FY_END = _comcell.FY_END
        _comcellItem.FACILITATOR_NAME = _comcell.FACILITATOR_NAME
        _comcellItem.MINUTES_TAKER_NAME = _comcell.MINUTES_TAKER_NAME
        _comcellItem.WEEK = _comcell.WEEK
        _comcellItem.WEEK_START = _comcell.WEEK_START
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
    Property FACILITATOR_NAME As String
    Property MINUTES_TAKER_NAME As String
    Property WEEK As Integer
    Property WEEK_START As DateTime
End Class