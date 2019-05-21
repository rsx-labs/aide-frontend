Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class AttendanceListDBProvider

    Public _attlist As ObservableCollection(Of myAttendanceList)

    Sub New()
        _attlist = New ObservableCollection(Of myAttendanceList)
    End Sub


    Public Sub SetAllAttendanceList(ByVal attendanceLst As MyAttendance)
        Dim _Attendanceobj As myAttendanceList = New myAttendanceList With {.Emp_ID = attendanceLst.EmployeeID,
                                                             .Emp_Name = attendanceLst.Name,
                                                             .Date_Entry = attendanceLst.DateEntry,
                                                             .Status = attendanceLst.Status,
                                                             .Emp_Image = attendanceLst.Image_Path}
        _attlist.Add(_Attendanceobj)

    End Sub

    Public Function GetAllEmpRPList()
        Return _attlist
    End Function

End Class


Public Class myAttendanceList
    Public Emp_ID As Integer
    Public Emp_Name As String
    Public Status As Double
    Public Desc As String
    Public Emp_Image As String
    Public Date_Entry As Date
    Public Display_Status As String
End Class
