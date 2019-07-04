Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

'''''''''''''''''''''''''''''''''
'   CHRISTIAN LOIS VALONDO      '
'''''''''''''''''''''''''''''''''
Public Class ContactListModel
    Implements INotifyPropertyChanged

#Region "Data Members"

    Private _eMP_ID As Integer
    Private _last_name As String
    Private _first_name As String
    Private _middle_name As String
    Private _nick_name As String
    Private _active As Integer
    Private _bdate As Date
    Private _position As String
    Private _dt_hired As Date
    Private _MARITAL_STATUS As String
    Private _image_path As Object
    Private _permission_group As String
    Private _department As String
    Private _division As String
    Private _shift As String
    Private _eMAIL_ADDRESS As String
    Private _eMAIL_ADDRESS2 As String
    Private _lOCATION As String
    Private _cEL_NO As String
    Private _lOCAL As System.Nullable(Of Integer)
    Private _hOMEPHONE As String
    Private _oTHER_PHONE As String
    Private _dT_REVIEWED As DateTime
    Private _pos_ID As String
    Private _full_name As String
    Private _isReviewed As Boolean



#End Region

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawContactList As MyContactList)
        Me.EMP_ID = rawContactList.EmpID
        Me.FIRST_NAME = rawContactList.FIRST_NAME
        Me.LAST_NAME = rawContactList.LAST_NAME
        Me.MIDDLE_NAME = rawContactList.MIDDLE_NAME
        Me.NICK_NAME = rawContactList.NICK_NAME
        Me.ACTIVE = rawContactList.ACTIVE
        Me.BDATE = rawContactList.BDATE
        Me.POSITION = rawContactList.POSITION
        Me.DT_HIRED = rawContactList.DT_HIRED
        Me.MARITAL_STATUS = rawContactList.MARITAL_STATUS
        Me.IMAGE_PATH = rawContactList.IMAGE_PATH
        Me.PERMISSION_GROUP = rawContactList.PERMISSION_GROUP
        Me.DEPARTMENT = rawContactList.DEPARTMENT
        Me.DIVISION = rawContactList.DIVISION
        Me.SHIFT = rawContactList.SHIFT
        Me.EMAIL_ADDRESS = rawContactList.EMADDRESS
        Me.EMAIL_ADDRESS2 = rawContactList.EMADDRESS2
        Me.LOCATION = rawContactList.LOC
        Me.CEL_NO = rawContactList.CELL_NO
        Me.LOCAL = rawContactList.lOCAL
        Me.HOMEPHONE = rawContactList.HOUSEPHONE
        Me.OTHER_PHONE = rawContactList.OTHERPHONE
        Me.DT_REVIEWED = rawContactList.DateReviewed
        Me.FULL_NAME = rawContactList.FULL_NAME
        Me.IsREVIEWED = rawContactList.IsREVIEWED
    End Sub

#Region "Properties"

    Public Property EMP_ID As Integer
        Get
            Return _eMP_ID
        End Get
        Set(value As Integer)
            _eMP_ID = value
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property
    Public Property FIRST_NAME As String
        Get
            Return _first_name
        End Get
        Set(value As String)
            _first_name = value
            NotifyPropertyChanged("FIRST_NAME")
        End Set
    End Property

    Public Property LAST_NAME As String
        Get
            Return _last_name
        End Get
        Set(value As String)
            _last_name = value
            NotifyPropertyChanged("LAST_NAME")
        End Set
    End Property

    Public Property MIDDLE_NAME As String
        Get
            Return _middle_name
        End Get
        Set(value As String)
            _middle_name = value
            NotifyPropertyChanged("MIDDLE_NAME")
        End Set
    End Property

    Public Property NICK_NAME As String
        Get
            Return _nick_name
        End Get
        Set(value As String)
            _nick_name = value
            NotifyPropertyChanged("NICK_NAME")
        End Set
    End Property

    Public Property ACTIVE As Integer
        Get
            Return _active
        End Get
        Set(value As Integer)
            _active = value
            NotifyPropertyChanged("ACTIVE")
        End Set
    End Property

    Public Property BDATE As Date
        Get
            Return _bdate
        End Get
        Set(value As Date)
            _bdate = value
            NotifyPropertyChanged("BDATE")
        End Set
    End Property

    Public Property POSITION As String
        Get
            Return _pos_ID
        End Get
        Set(value As String)
            _pos_ID = value
            NotifyPropertyChanged("POSITION")
        End Set
    End Property

    Public Property DT_HIRED As Date
        Get
            Return _dt_hired
        End Get
        Set(value As Date)
            _dt_hired = value
            NotifyPropertyChanged("DT_HIRED")
        End Set
    End Property

    Public Property MARITAL_STATUS As String
        Get
            Return _MARITAL_STATUS
        End Get
        Set(value As String)
            _MARITAL_STATUS = value
            NotifyPropertyChanged("MARITAL_STATUS")
        End Set
    End Property


    Public Property IMAGE_PATH As String
        Get
            '
            Return "C:\Programs\BOOTCAMP_AIDE\AIDE\GDC.PH.AIDE.CommCellServices\GDC.PH.AIDE.CommCellServices\Assets\EmployeePhotos\" + _image_path
        End Get
        Set(value As String)
            _image_path = value
            NotifyPropertyChanged("IMAGE_PATH")
        End Set
    End Property

    Public Property PERMISSION_GROUP As String
        Get
            Return _permission_group
        End Get
        Set(value As String)
            _permission_group = value
            NotifyPropertyChanged("PERMISSION_GROUP")
        End Set
    End Property

    Public Property DEPARTMENT As String
        Get
            Return _department
        End Get
        Set(value As String)
            _department = value
            NotifyPropertyChanged("DEPARTMENT")
        End Set
    End Property

    Public Property DIVISION As String
        Get
            Return _division
        End Get
        Set(value As String)
            _division = value
            NotifyPropertyChanged("DIVISION")
        End Set
    End Property

    Public Property SHIFT As String
        Get
            Return _shift
        End Get
        Set(value As String)
            _shift = value
            NotifyPropertyChanged("SHIFT")
        End Set
    End Property

    Public Property EMAIL_ADDRESS As String
        Get
            Return _eMAIL_ADDRESS
        End Get
        Set(value As String)
            _eMAIL_ADDRESS = value
            NotifyPropertyChanged("EMAIL_ADDRESS")
        End Set
    End Property

    Public Property EMAIL_ADDRESS2 As String
        Get
            Return _eMAIL_ADDRESS2
        End Get
        Set(value As String)
            _eMAIL_ADDRESS2 = value
            NotifyPropertyChanged("EMAIL_ADDRESS2")
        End Set
    End Property

    Public Property LOCATION As String
        Get
            Return _lOCATION
        End Get
        Set(value As String)
            _lOCATION = value
            NotifyPropertyChanged("LOCATION")
        End Set
    End Property

    Public Property CEL_NO As String
        Get
            Return _cEL_NO
        End Get
        Set(value As String)
            _cEL_NO = value
            NotifyPropertyChanged("CEL_NO")
        End Set
    End Property

    Public Property LOCAL As System.Nullable(Of Integer)
        Get
            Return _lOCAL
        End Get
        Set(value As System.Nullable(Of Integer))
            _lOCAL = value
            NotifyPropertyChanged("LOCAL")
        End Set
    End Property

    Public Property HOMEPHONE As String
        Get
            Return _hOMEPHONE
        End Get
        Set(value As String)
            _hOMEPHONE = value
            NotifyPropertyChanged("HOMEPHONE")
        End Set
    End Property

    Public Property OTHER_PHONE As String
        Get
            Return _oTHER_PHONE
        End Get
        Set(value As String)
            _oTHER_PHONE = value
            NotifyPropertyChanged("OTHER_PHONE")
        End Set
    End Property

    Public Property DT_REVIEWED As Date
        Get
            Return _dT_REVIEWED
        End Get
        Set(value As Date)
            _dT_REVIEWED = value
            NotifyPropertyChanged("DT_REVIEWED")
        End Set
    End Property

    Public Property IsREVIEWED As Boolean
        Get
            Return _isReviewed
        End Get
        Set(value As Boolean)
            _isReviewed = value
            NotifyPropertyChanged("IsREVIEWED")
        End Set
    End Property

    Public Property FULL_NAME As String
        Get
            Return _full_name
        End Get
        Set(value As String)
            _full_name = value
            NotifyPropertyChanged("FULL_NAME")
        End Set
    End Property

#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
'''''''''''''''''''''''''''''''''
'   CHRISTIAN LOIS VALONDO      '
'''''''''''''''''''''''''''''''''
