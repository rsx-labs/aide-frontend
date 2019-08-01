Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel
''' <summary>
''' By Aevan Camille Batongbacal / JHUNELL BARCENAS
''' </summary>
''' <remarks></remarks>
Public Class SuccessRegisterDBProvider

    Private _successList As ObservableCollection(Of MySuccessRegister)
    Private _nicknameList As ObservableCollection(Of MyNickname)
    Private client As AideServiceClient

    Public Sub New()
        _successList = New ObservableCollection(Of MySuccessRegister)
        _nicknameList = New ObservableCollection(Of MyNickname)
    End Sub

    Public Function GetSuccessRegister() As IEnumerable(Of MySuccessRegister)
        Throw New NotImplementedException()
    End Function

    Public Function GetMyNickname() As ObservableCollection(Of MyNickname)
        Return _nicknameList
    End Function

    Public Function GetMySuccessRegister() As ObservableCollection(Of MySuccessRegister)
        Return _successList
    End Function



    Public Sub SetMySuccessRegister(ByVal _successRegister As SuccessRegister)

        Dim _successObject As MySuccessRegister = New MySuccessRegister With {
                .EmpID = _successRegister.Emp_ID,
                .SuccessID = _successRegister.SuccessID,
                .NickName = _successRegister.Nick_Name,
                .DateInput = _successRegister.DateInput,
                .DetailsOfSuccess = _successRegister.DetailsOfSuccess,
                .WhosInvolve = _successRegister.WhosInvolve,
                .AdditionalInformation = _successRegister.AdditionalInformation
                }
        _successList.Add(_successObject)
    End Sub

    Public Sub SetMyNickname(ByVal _nickname As Nickname)
        Dim _successObject As MyNickname = New MyNickname With {
            .EmpID = _nickname.Emp_ID,
            .NickName = _nickname.Nick_Name,
            .FirstName = _nickname.First_Name,
        .EmployeeName = _nickname.Employee_Name
                }
        _nicknameList.Add(_successObject)
    End Sub


End Class

Public Class MySuccessRegister
    Property EmpID As Integer
    Property SuccessID As Integer
    Property DateInput As DateTime
    Property NickName As String
    Property DetailsOfSuccess As String
    Property WhosInvolve As String
    Property AdditionalInformation As String

End Class

Public Class MyNickname
    Property EmpID As Integer
    Property NickName As String
    Property FirstName As String
    Property EmployeeName As String
End Class
