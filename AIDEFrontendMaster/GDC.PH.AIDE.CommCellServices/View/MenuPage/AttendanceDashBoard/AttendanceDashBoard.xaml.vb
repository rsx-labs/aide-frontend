Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports System.Windows.Xps.Packaging
Imports System.Windows.Xps
Imports System.Printing
Imports NLog

<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class AttendanceDashBoard
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    'Private _AideService As ServiceReference1.AideServiceClient
    Private mainFrame As New Frame
    Dim AttendanceListVM As New AttendanceListViewModel
    Dim setStatus As Integer
    Dim profile As Profile
    Dim AEmployee As MyAttendance()

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

#Region "Constructor"
    Public Sub New(mainFrame As Frame, profile As Profile)

        _logger.Debug("Start : Constructor")

        InitializeComponent()

        Me.mainFrame = mainFrame
        Me.profile = profile
        '_AideService = aideService

        SetData()

        Me.DataContext = AttendanceListVM

        LoadDataForHeader()

        _logger.Debug("End : Constructor")
    End Sub
#End Region

#Region "Functions"
    'Public Function InitializeService() As Boolean
    '    Dim bInitialize As Boolean = False
    '    Try
    '        Dim Context As InstanceContext = New InstanceContext(Me)
    '        _AideService = New AideServiceClient(Context)
    '        _AideService.Open()
    '        bInitialize = True
    '    Catch ex As SystemException
    '        _AideService.Abort()
    '        MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    End Try
    '    Return bInitialize
    'End Function

    Public Sub SetData()
        Try
            _logger.Debug("Start : SetData")

            AEmployee = AideClient.GetClient().GetAttendanceToday(profile.Emp_ID)

            Dim d As DateTime? = Nothing
            Dim lstAEmployeeList As New ObservableCollection(Of AttendanceModel)
            Dim aemployeeListDBProvider As New AttendanceListDBProvider

            For Each objaemp As MyAttendance In AEmployee
                aemployeeListDBProvider.SetAllAttendanceList(objaemp)
            Next

            For Each rawUser As myAttendanceList In aemployeeListDBProvider.GetAllEmpRPList()
                setStatus = rawUser.Status
                SetCategory(rawUser)
                If rawUser.Status = Constants.STAT_ATT_LPRESENT Or
                    rawUser.Status = Constants.STAT_ATT_LONSITE Or
                    rawUser.Status = Constants.STAT_ATT_LATE Then 'For Late
                    SetCategoryDisplay(rawUser)
                End If

                d = rawUser.Logoff_Time
                If d.Value = Nothing Then
                    rawUser.Logoff_Time = ""
                End If

                lstAEmployeeList.Add(New AttendanceModel(rawUser))
            Next

            AttendanceListVM.EmployeeListAttendance = lstAEmployeeList
            Me.DataContext = AttendanceListVM

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")

            _logger.Error(ex.ToString())
        End Try

        _logger.Debug("End : SetData")
    End Sub

    Public Sub SetDataForSearch(input As String)
        Try

            Dim d As DateTime? = Nothing
            Dim aemployeeListDBProvider As New AttendanceListDBProvider
            Dim lstAEmployeeList As New ObservableCollection(Of AttendanceModel)

            Dim items = From i In AEmployee Where i.Name.ToLower.Contains(input.ToLower) Or i.EmployeeID.ToString.ToLower.Contains(input.ToLower)

            Dim searchAssets = New ObservableCollection(Of MyAttendance)(items)

            For Each objaemp As MyAttendance In searchAssets
                aemployeeListDBProvider.SetAllAttendanceList(objaemp)
            Next

            For Each rawUser As myAttendanceList In aemployeeListDBProvider.GetAllEmpRPList()

                If rawUser.Status = Constants.STAT_ATT_LPRESENT Or
                   rawUser.Status = Constants.STAT_ATT_LONSITE Or
                   rawUser.Status = Constants.STAT_ATT_LATE Then 'For Late
                    SetCategoryDisplay(rawUser)
                End If

                d = rawUser.Logoff_Time
                If d.Value = Nothing Then
                    rawUser.Logoff_Time = ""
                End If

                lstAEmployeeList.Add(New AttendanceModel(rawUser))
            Next
            AttendanceListVM.EmployeeListAttendance = lstAEmployeeList
            Me.DataContext = AttendanceListVM

        Catch ex As Exception
            _logger.Debug($"Error at SetdataForSearch = {ex.ToString()}")

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub LoadDataForHeader()
        Try
            txt_DeptHeader.Text = profile.Department + " - " + profile.Division
        Catch ex As Exception
           MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Public Sub SetCategoryDisplay(rawUser As myAttendanceList)

        Select Case rawUser.Status
            Case Constants.STAT_ATT_ONSITE
                rawUser.Desc = "Onsite"
            Case Constants.STAT_ATT_OBA
                rawUser.Desc = "Onsite"
            Case Constants.STAT_ATT_HOBA
                rawUser.Desc = "(H) Onsite"
            Case Constants.STAT_ATT_LONSITE,
                 Constants.STAT_ATT_LATE
                rawUser.Desc = "(Late)"
            Case Constants.STAT_ATT_PRESENT
                rawUser.Desc = "Present"
            Case Constants.STAT_ATT_LPRESENT
                rawUser.Desc = "(Late)"
            Case Constants.STAT_ATT_HSL
                rawUser.Desc = "(H) Sick Leave"
            Case Constants.STAT_ATT_SL
                rawUser.Desc = "Sick Leave"
            Case Constants.STAT_ATT_HOLIDAY
                rawUser.Desc = "Holiday"
            Case Constants.STAT_ATT_VL
                rawUser.Desc = "Vacation Leave"
            Case Constants.STAT_ATT_HVL
                rawUser.Desc = "(H) Vacation Leave"
            Case Constants.STAT_ATT_EL
                rawUser.Desc = "Emergency Leave"
            Case Constants.STAT_ATT_HEL
                rawUser.Desc = "(H) Emergency Leave"
        End Select
    End Sub

    Public Sub SetCategory(rawUser As myAttendanceList)

        Select Case rawUser.Status
            Case Constants.STAT_ATT_ONSITE,
                Constants.STAT_ATT_OBA,
                Constants.STAT_ATT_HOBA,
                Constants.STAT_ATT_LONSITE

                rawUser.Display_Status = "..\..\..\Assets\Attendance\onsite.png"
            Case Constants.STAT_ATT_PRESENT,
                Constants.STAT_ATT_LPRESENT,
                Constants.STAT_ATT_LATE

                rawUser.Display_Status = "..\..\..\Assets\Attendance\present.png"
            Case Constants.STAT_ATT_HSL,
                Constants.STAT_ATT_SL

                rawUser.Display_Status = "..\..\..\Assets\Attendance\sick.png"
            Case Constants.STAT_ATT_HOLIDAY

                rawUser.Display_Status = "..\..\..\Assets\Attendance\holiday.png"
            Case Constants.STAT_ATT_VL,
                Constants.STAT_ATT_HVL,
                Constants.STAT_ATT_EL,
                Constants.STAT_ATT_HEL,
                Constants.STAT_ATT_OL

                rawUser.Display_Status = "..\..\..\Assets\Attendance\vacation.png"

        End Select

    End Sub

#End Region

#Region "Events"
    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        If txtSearch.Text = String.Empty Then
            SetData()
        Else
            SetDataForSearch(txtSearch.Text)
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

End Class
