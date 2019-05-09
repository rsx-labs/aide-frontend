Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           START               '
'''''''''''''''''''''''''''''''''
Public Class BirthdayListDBProvider
    Private _birthdayList As ObservableCollection(Of MyBirthdayList)
    Private _birthdayListMonth As ObservableCollection(Of MyBirthdayList)
    Private _birthdayListToday As ObservableCollection(Of MyBirthdayList)
    Private client As AideServiceClient

    Public Sub New()
        _birthdayList = New ObservableCollection(Of MyBirthdayList)
        _birthdayListMonth = New ObservableCollection(Of MyBirthdayList)
        _birthdayListToday = New ObservableCollection(Of MyBirthdayList)
    End Sub

    Public Function GetBirthdayList() As IEnumerable(Of MyBirthdayList)
        Throw New NotImplementedException()
    End Function

    Public Function GetMyBirthdayList() As ObservableCollection(Of MyBirthdayList)
        Return _birthdayList
    End Function

    Public Sub SetMyBirthdayList(ByVal _birthday As BirthdayList)
        Dim _birthdayObject As MyBirthdayList = New MyBirthdayList With {
                .EmpID = _birthday.EmpID,
                .Birthday = _birthday.BIRTHDAY,
                .FULL_NAME = _birthday.FIRST_NAME + " " + _birthday.LAST_NAME,
                .IMAGE_PATH = _birthday.IMAGE_PATH
                }
        _birthdayList.Add(_birthdayObject)
    End Sub

    Public Function GetBirthdayListMonth() As IEnumerable(Of MyBirthdayList)
        Throw New NotImplementedException()
    End Function

    Public Function GetMyBirthdayListMonth() As ObservableCollection(Of MyBirthdayList)
        Return _birthdayListMonth
    End Function

    Public Sub SetMyBirthdayListMonth(ByVal _birthday As BirthdayList)
        Dim _birthdayObjectMonth As MyBirthdayList = New MyBirthdayList With {
                .EmpID = _birthday.EmpID,
                .Birthday = _birthday.BIRTHDAY,
                .FULL_NAME = _birthday.FIRST_NAME + " " + _birthday.LAST_NAME,
                .IMAGE_PATH = _birthday.IMAGE_PATH
                }
        _birthdayListMonth.Add(_birthdayObjectMonth)
    End Sub

    Public Sub SetMyBirthdayListToday(ByVal _birthday As BirthdayList)
        Dim _birthdayObjectToday As MyBirthdayList = New MyBirthdayList With {
                .EmpID = _birthday.EmpID,
                .Birthday = _birthday.BIRTHDAY,
                .FULL_NAME = _birthday.FIRST_NAME + " " + _birthday.LAST_NAME,
                .IMAGE_PATH = _birthday.IMAGE_PATH
                }
        _birthdayListToday.Add(_birthdayObjectToday)
    End Sub
    Public Function GetBirthdayListToday() As IEnumerable(Of MyBirthdayList)
        Throw New NotImplementedException()
    End Function

    Public Function GetMyBirthdayListToday() As ObservableCollection(Of MyBirthdayList)
        Return _birthdayListToday
    End Function
End Class

Public Class MyBirthdayList
    Property Birthday As Date
    Property EmpID As Integer
    Property FIRST_NAME As String
    Property LAST_NAME As String
    Property FULL_NAME As String
    Property IMAGE_PATH As String
End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           END                 '
'''''''''''''''''''''''''''''''''

