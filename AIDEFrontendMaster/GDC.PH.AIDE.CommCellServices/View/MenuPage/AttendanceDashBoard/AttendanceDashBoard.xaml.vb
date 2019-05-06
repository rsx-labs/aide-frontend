Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AttendanceDashBoard
    Implements ServiceReference1.IAideServiceCallback



#Region "Declarations"
    Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As New Frame
    Dim AttendanceListVM As New AttendanceListViewModel
    Dim setStatus As Integer
    Dim profile As Profile

#End Region


#Region "Constructor"

    Public Sub New(mainFrame As Frame, _profile As Profile)

        InitializeComponent()
        Me.mainFrame = mainFrame
        Me.profile = _profile
        SetData()
        Me.DataContext = AttendanceListVM
        LoadDataForHeader()

    End Sub

#End Region


#Region "Functions"


    Public Function InitializeService() As Boolean
        Dim bInitialize As Boolean = False
        Try
            Dim Context As InstanceContext = New InstanceContext(Me)
            _AideService = New AideServiceClient(Context)
            _AideService.Open()
            bInitialize = True
        Catch ex As SystemException
            _AideService.Abort()
        End Try
        Return bInitialize
    End Function

    Public Sub SetData()
        Try
            If InitializeService() Then
                Dim AEmployee As MyAttendance() = _AideService.GetAttendanceToday(profile.Email_Address)

                Dim lstAEmployeeList As New ObservableCollection(Of AttendanceModel)
                Dim aemployeeListDBProvider As New AttendanceListDBProvider

                For Each objaemp As MyAttendance In AEmployee
                    aemployeeListDBProvider.SetAllAttendanceList(objaemp)
                Next

                For Each rawUser As myAttendanceList In aemployeeListDBProvider.GetAllEmpRPList()
                    setStatus = rawUser.Status
                    SetCategory(rawUser)
                    SetCategoryDisplay(rawUser)
                    lstAEmployeeList.Add(New AttendanceModel(rawUser))


                Next

                AttendanceListVM.EmployeeListAttendance = lstAEmployeeList
                Me.DataContext = AttendanceListVM

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub SetDataForSearch()
        Try
            If InitializeService() Then
                Dim AEmployee As MyAttendance() = _AideService.GetAttendanceTodayBySearch(profile.Email_Address, txtSearch.Text)

                Dim lstAEmployeeList As New ObservableCollection(Of AttendanceModel)
                Dim aemployeeListDBProvider As New AttendanceListDBProvider

                For Each objaemp As MyAttendance In AEmployee
                    aemployeeListDBProvider.SetAllAttendanceList(objaemp)
                Next

                For Each rawUser As myAttendanceList In aemployeeListDBProvider.GetAllEmpRPList()
                    setStatus = rawUser.Status
                    SetCategory(rawUser)
                    SetCategoryDisplay(rawUser)
                    lstAEmployeeList.Add(New AttendanceModel(rawUser))


                Next

                AttendanceListVM.EmployeeListAttendance = lstAEmployeeList
                Me.DataContext = AttendanceListVM

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub LoadDataForHeader()
        Try
            txt_DeptHeader.Text = profile.Department + " - " + profile.Division
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FAILED")
        End Try
    End Sub

    Public Sub SetCategoryDisplay(rawUser_ As myAttendanceList)
        If setStatus = 1 Then
            rawUser_.Desc = "Onsite"
        ElseIf setStatus = 2 Then
            rawUser_.Desc = "Present"
        ElseIf setStatus = 3 Then
            rawUser_.Desc = "Sick Leave"
        ElseIf setStatus = 4 Then
            rawUser_.Desc = "Vacation Leave"
        ElseIf setStatus = 5 Then
            rawUser_.Desc = "(H) Sick Leave"
        ElseIf setStatus = 6 Then
            rawUser_.Desc = "(H) Vacation Leave"
        ElseIf setStatus = 7 Then
            rawUser_.Desc = "Holiday"
        ElseIf setStatus = 8 Then
            rawUser_.Desc = "Emergency Leave"
        ElseIf setStatus = 9 Then
            rawUser_.Desc = "(H) Emergency Leave"
        ElseIf setStatus = 10 Then
            rawUser_.Desc = "Other Leaves"
        End If
    End Sub

    Public Sub SetCategory(rawUser_ As myAttendanceList)
        If setStatus = 1 Then
            rawUser_.Display_Status = "..\..\..\Assets\Attendance\onsite.png"
        ElseIf setStatus = 2 Then
            rawUser_.Display_Status = "..\..\..\Assets\Attendance\present.png"
        ElseIf setStatus = 3 Or setStatus = 5 Then
            rawUser_.Display_Status = "..\..\..\Assets\Attendance\sick.png"
            'displayStatus = Colors.Red.ToString()
        ElseIf setStatus = 4 Or setStatus = 6 Or setStatus = 8 Or setStatus = 9 Or setStatus = 10 Then
            rawUser_.Display_Status = "..\..\..\Assets\Attendance\vacation.png"
        End If
    End Sub
#End Region
#Region "ICallBack Function"
    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline

    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent

    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        If message <> String.Empty Then
            MessageBox.Show(message)
        End If
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate

    End Sub
#End Region

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        If txtSearch.Text = String.Empty Then
            SetData()
        Else
            SetDataForSearch()
        End If
    End Sub
End Class
