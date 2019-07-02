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
                .EMADDRESS = _contact.EMADDRESS,
                .EMADDRESS2 = _contact.EMADDRESS2,
                .CELL_NO = _contact.CELL_NO,
                .HOUSEPHONE = _contact.HOUSEPHONE,
                .OTHERPHONE = _contact.OTHERPHONE,
                .lOCAL = _contact.lOCAL,
                .LOC = _contact.LOC,
                .DateReviewed = _contact.DateReviewed,
                .POSITION = _contact.POSITION,
                .MARITAL_STATUS = _contact.MARITAL_STATUS,
                .FULL_NAME = _contact.FIRST_NAME + " " + _contact.LAST_NAME,
                .IMAGE_PATH = _contact.IMAGE_PATH,
                .IsREVIEWED = isReviewed,
                .NICK_NAME = _contact.Nick_Name
                }
        _contactList.Add(_contactsObject)
    End Sub

End Class

Public Class MyContactList
    Property CELL_NO As String
    Property DateReviewed As Date
    Property EMADDRESS As String
    Property EMADDRESS2 As String
    Property EmpID As Integer
    Property HOUSEPHONE As String
    Property lOCAL As Integer
    Property LOC As String
    Property MARITAL_STATUS As String
    Property POSITION As String
    Property OTHERPHONE As String
    Property FIRST_NAME As String
    Property LAST_NAME As String
    Property IsREVIEWED As Boolean
    Property FULL_NAME As String
    Property IMAGE_PATH As String
    Property NICK_NAME As String
End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'           END                 '
'''''''''''''''''''''''''''''''''

