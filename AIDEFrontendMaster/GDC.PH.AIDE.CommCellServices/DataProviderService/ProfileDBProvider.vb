Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class ProfileDBProvider

    Private _statusList As ObservableCollection(Of MyStatusList)
    Private _MyStatusList As New MyStatusList
    Private _MyProfile As New MyProfile

    Public Sub New()
        _statusList = New ObservableCollection(Of MyStatusList)
    End Sub

    Public Function GetStatusList() As ObservableCollection(Of MyStatusList)
        Return _statusList
    End Function

    Public Function GetUsers() As IEnumerable(Of MyProfile)
        Throw New NotImplementedException()
    End Function

    Public Function GetMyProfile() As MyProfile
        Return _MyProfile
    End Function

    Public Sub SetMyProfile(ByVal _profile As Profile)
        _MyProfile.UserID = _profile.Emp_ID
        _MyProfile.WsEmpID = _profile.Ws_EMP_ID
        _MyProfile.DeptID = _profile.Dept_ID
        _MyProfile.LastName = _profile.LastName
        _MyProfile.FirstName = _profile.FirstName
        _MyProfile.MiddleName = _profile.MiddleName
        _MyProfile.NickName = _profile.Nickname
        _MyProfile.EmpName = _profile.LastName + " " + _profile.FirstName + " " + _profile.MiddleName + "."
        _MyProfile.Birthdate = _profile.Birthdate
        _MyProfile.DateHired = _profile.DateHired
        _MyProfile.ImagePath = _profile.ImagePath
        _MyProfile.Department = _profile.Department
        _MyProfile.Division = _profile.Division
        _MyProfile.Position = _profile.Position
        _MyProfile.EmailAddress = _profile.Email_Address
        _MyProfile.EmailAddress2 = _profile.Email_Address2
        _MyProfile.Location = _profile.Location
        _MyProfile.CelNo = _profile.Cel_No
        _MyProfile.Local = _profile.Local
        _MyProfile.HomePhone = _profile.Home_Phone
        _MyProfile.OtherPhone = _profile.Other_Phone
        _MyProfile.DtReviewed = _profile.Dt_Reviewed
        _MyProfile.Permission = _profile.Permission
        _MyProfile.Permission_ID = _profile.Permission_ID
        _MyProfile.CivilStatus = _profile.CivilStatus
        _MyProfile.ShiftStatus = _profile.ShiftStatus

    End Sub

End Class

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class MyProfile
    Property UserID As Integer
    Property WsEmpID As String
    Property DeptID As Short
    Property LastName As String
    Property FirstName As String
    Property MiddleName As String
    Property NickName As String
    Property EmpName As String
    Property BirthDate As DateTime
    Property DateHired As DateTime
    Property ImagePath As String
    Property Department As String
    Property Division As String
    Property Position As String
    Property EmailAddress As String
    Property EmailAddress2 As String
    Property Location As String
    Property CelNo As String
    Property Local As Integer
    Property HomePhone As String
    Property OtherPhone As String
    Property DtReviewed As DateTime
    Property Permission As String
    Property Permission_ID As Integer
    Property CivilStatus As String
    Property ShiftStatus As String

End Class

''' <summary>
''' MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class MyStatusList
    Public Property Key As Integer
    Public Property Value As String
End Class