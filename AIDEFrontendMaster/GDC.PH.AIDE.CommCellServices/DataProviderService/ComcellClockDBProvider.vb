Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class ComcellClockDBProvider
    Private _objClock As myComcellClockSet


    Public Sub New()
        _objClock = New myComcellClockSet
    End Sub

    Public Function _getobjClock() As myComcellClockSet
        Return _objClock
    End Function

    Public Function _setlistofitems(ByRef clock As ComcellClock)
        Dim _myComcellClockSet As New myComcellClockSet With {._clockDay = clock.Clock_Day, _
                                                  ._clockHour = clock.Clock_Hour, _
                                                  ._clockMinute = clock.Clock_Minute, _
                                                  ._empID = clock.Emp_ID, _
                                                ._midday = clock.MIDDAY}

        _objClock = _myComcellClockSet
        Return _myComcellClockSet
    End Function

End Class

Public Class myComcellClockSet
    Property _clockDay As Integer
    Property _clockHour As Integer
    Property _clockMinute As Integer
    Property _empID As Integer
    Property _midday As String
End Class

