Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           START               '
'''''''''''''''''''''''''''''''''
Public Class ContactListDBProvider
    Private _contactList As ObservableCollection(Of MyContactList)
    Private client As AideServiceClient

    Public Sub New()
        _contactList = New ObservableCollection(Of MyContactList)
    End Sub

    Public Function GetContactList() As IEnumerable(Of MyContactList)
        Throw New NotImplementedException()
    End Function

    Public Function GetMyContactList() As ObservableCollection(Of MyContactList)
        Return _contactList
    End Function

    Public Sub SetMyContactList(ByVal _contact As ContactList)
        Dim isReviewed As Boolean
        Dim testDate As DateTime = _contact.DateReviewed
        Dim currDate As DateTime = DateTime.Now.Date

        If testDate.Month = currDate.Month AndAlso testDate.Year = currDate.Year Then
            isReviewed = True
        Else
            isReviewed = False
        End If
        Dim _contactsObject As MyContactList = New MyContactList With {
                .EmpID = _contact.EmpID,
                .LAST_NAME = _contact.LAST_NAME,
                .FIRST_NAME = _contact.FIRST_NAME,
                .MIDDLE_NAME = _contact.MIDDLE_NAME,
                .NICK_NAME = _contact.Nick_Name,
                .ACTIVE = _contact.ACTIVE,
                .BDATE = _contact.BIRTHDATE,
                .POSITION = _contact.POSITION,
                .DT_HIRED = _contact.DT_HIRED,
                .MARITAL_STATUS = _contact.MARITAL_STATUS,
                .IMAGE_PATH = _contact.IMAGE_PATH,
                .PERMISSION_GROUP = _contact.PERMISSION_GROUP,
                .DEPARTMENT = _contact.DEPARTMENT,
                .DIVISION = _contact.DIVISION,
                .SHIFT = _contact.SHIFT,
                .EMADDRESS = _contact.EMADDRESS,
                .EMADDRESS2 = _contact.EMADDRESS2,
                .LOC = _contact.LOC,
                .CELL_NO = _contact.CELL_NO,
                .lOCAL = _contact.lOCAL,
                .HOUSEPHONE = _contact.HOUSEPHONE,
                .OTHERPHONE = _contact.OTHERPHONE,
                .DateReviewed = _contact.DateReviewed,
                .IsREVIEWED = isReviewed,
                .FULL_NAME = _contact.LAST_NAME + ", " + _contact.FIRST_NAME,
                .MARITAL_STATUS_ID = _contact.MARITAL_STATUS_ID,
                .POSITION_ID = _contact.POSITION_ID,
                .PERMISSION_GROUP_ID = _contact.PERMISSION_GROUP_ID,
                .DEPARTMENT_ID = _contact.DEPARTMENT_ID,
                .DIVISION_ID = _contact.DIVISION_ID
                      }
        _contactList.Add(_contactsObject)
    End Sub

End Class

Public Class MyContactList
    Property EmpID As Integer
    Property LAST_NAME As String
    Property FIRST_NAME As String
    Property MIDDLE_NAME As String
    Property NICK_NAME As String
    Property ACTIVE As Integer
    Property BDATE As Date
    Property POSITION As String
    Property DT_HIRED As Date
    Property MARITAL_STATUS As String
    Property IMAGE_PATH As String
    Property PERMISSION_GROUP As String
    Property DEPARTMENT As String
    Property DIVISION As String
    Property SHIFT As String
    Property EMADDRESS As String
    Property EMADDRESS2 As String
    Property LOC As String
    Property CELL_NO As String
    Property lOCAL As Integer
    Property HOUSEPHONE As String
    Property OTHERPHONE As String
    Property DateReviewed As Date
    Property IsREVIEWED As Boolean
    Property FULL_NAME As String
    Property MARITAL_STATUS_ID As String
    Property POSITION_ID As Integer
    Property PERMISSION_GROUP_ID As Integer
    Property DEPARTMENT_ID As Integer
    Property DIVISION_ID As Integer

End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           END                 '
'''''''''''''''''''''''''''''''''

