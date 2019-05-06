
''' <summary>
'''MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class ProfileModel

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
    Property CivilStatus As String




    Public Sub New()

    End Sub

    Public Sub New(ByVal aRawUser As MyProfile)
        Me.UserID = aRawUser.UserID
        Me.WsEmpID = aRawUser.WsEmpID
        Me.DeptID = aRawUser.DeptID
        Me.FirstName = aRawUser.FirstName
        Me.LastName = aRawUser.LastName
        Me.NickName = aRawUser.NickName
        Me.MiddleName = aRawUser.MiddleName
        Me.EmpName = aRawUser.EmpName
        Me.BirthDate = aRawUser.BirthDate
        Me.DateHired = aRawUser.DateHired
        Me.ImagePath = aRawUser.ImagePath
        Me.Department = aRawUser.Department
        Me.Division = aRawUser.Division
        Me.Position = aRawUser.Position
        Me.EmailAddress = aRawUser.EmailAddress
        Me.EmailAddress2 = aRawUser.EmailAddress2
        Me.Location = aRawUser.Location
        Me.CelNo = aRawUser.CelNo
        Me.Local = aRawUser.Local
        Me.HomePhone = aRawUser.HomePhone
        Me.OtherPhone = aRawUser.OtherPhone
        Me.DtReviewed = aRawUser.DtReviewed
        Me.Permission = aRawUser.Permission
        Me.CivilStatus = aRawUser.CivilStatus
    End Sub

    Public Function ToMyUser() As MyProfile
        ToMyUser = New MyProfile() With {.UserID = Me.UserID,
                                        .WsEmpID = Me.WsEmpID,
                                        .DeptID = Me.DeptID,
                                        .FirstName = Me.FirstName,
                                        .NickName = Me.NickName,
                                         .LastName = Me.LastName,
                                        .MiddleName = Me.MiddleName,
                                        .BirthDate = Me.BirthDate,
                                        .DateHired = Me.DateHired,
                                        .ImagePath = Me.ImagePath,
                                        .Department = Me.Department,
                                        .Division = Division,
                                        .Position = Me.Position,
                                        .EmailAddress = Me.EmailAddress,
                                        .EmailAddress2 = Me.EmailAddress2,
                                        .Location = Me.Location,
                                        .CelNo = Me.CelNo,
                                        .Local = Me.Local,
                                        .HomePhone = Me.HomePhone,
                                        .OtherPhone = Me.OtherPhone,
                                        .DtReviewed = Me.DtReviewed,
                                        .Permission = Me.Permission,
                                        .CivilStatus = Me.CivilStatus}
    End Function
End Class
''' <summary>
'''MODIFIED BY GIANN CARLO CAMILO
''' </summary>
''' <remarks></remarks>
Public Class StatusModel

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Key As Integer
    Public Property Value As String

    Public Sub New(ByVal aRawstatus As MyStatusList)
        Me.Key = aRawstatus.Key
        Me.Value = aRawstatus.Value
    End Sub

    Public Function ToMyStatus() As MyStatusList
        ToMyStatus = New MyStatusList() With {.Key = Me.Key, .Value = Me.Value}
    End Function

End Class

