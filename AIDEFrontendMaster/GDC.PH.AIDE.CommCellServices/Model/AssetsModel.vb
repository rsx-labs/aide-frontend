Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class AssetsModel
    Implements INotifyPropertyChanged

#Region "FIELDS"
    Public _asset_id As Integer
    Public _emp_id As Integer
    Public _previous_id As Integer
    Public _previous_owner As String
    Public _asset_desc As String
    Public _manufacturer As String
    Public _model_no As String
    Public _serial_no As String
    Public _asset_tag As String
    Public _date_purchased As DateTime
    Public _status As Integer
    Public _other_info As String
    Public _date_assigned As DateTime
    Public _comments As String
    Public _fullname As String
    Public _department As String
    Public _statusDescr As String
    Public _dateDescr As String
    Public _assigned_to As Integer
    Public _approval As Integer
    Public _isapproved As Boolean
    Public _table_name As String
    Public _firstName As String
    Public _nickName As String
    Public _employeeName As String
    Public _date_borrowed As DateTime
    Public _date_returned As DateTime
    Public _asset_borrowing_id As Integer
    Public _trans_fg As Integer

#End Region

#Region "CONSTRUCTOR"
    Public Sub New()
    End Sub

    Public Sub New(ByVal rawAssets As MyAssets)
        Me.ASSET_ID = rawAssets.ASSET_ID
        Me.ASSET_TAG = rawAssets.ASSET_TAG
        Me.ASSET_DESC = rawAssets.ASSET_DESC
        Me.MANUFACTURER = rawAssets.MANUFACTURER
        Me.MODEL_NO = rawAssets.MODEL_NO
        Me.DATE_ASSIGNED = rawAssets.DATE_ASSIGNED
        Me.DATE_PURCHASED = rawAssets.DATE_PURCHASED
        Me.COMMENTS = rawAssets.COMMENTS
        Me.DEPARTMENT = rawAssets.DEPARTMENT
        Me.OTHER_INFO = rawAssets.OTHER_INFO
        Me.SERIAL_NO = rawAssets.SERIAL_NO
        Me.STATUS = rawAssets.STATUS
        Me.EMP_ID = rawAssets.EMP_ID
        Me.FULL_NAME = rawAssets.FULL_NAME
        Me.STATUS_DESCR = rawAssets.STATUS_DESCR
        Me.DATE_DESCR = rawAssets.DATE_DESCR
        Me.ASSIGNED_TO = rawAssets.ASSIGNED_TO
        Me.APPROVAL = rawAssets.APPROVAL
        Me.ISAPPROVED = rawAssets.ISAPPROVED
        Me.TABLE_NAME = rawAssets.TABLE_NAME
        Me.EMPLOYEE_NAME = rawAssets.EMPLOYEE_NAME
        Me.FIRST_NAME = rawAssets.FIRST_NAME
        Me.NICK_NAME = rawAssets.NICK_NAME
        Me.PREVIOUS_ID = rawAssets.PREVIOUS_ID
        Me.PREVIOUS_OWNER = rawAssets.PREVIOUS_OWNER
        Me.DATE_BORROWED = rawAssets.DATE_BORROWED
        Me.DATE_RETURNED = rawAssets.DATE_RETURNED
        Me.ASSET_BORROWING_ID = rawAssets.ASSET_BORROWING_ID

    End Sub
#End Region

#Region "PROPERTIES"

    Public Property ASSET_ID As Integer
        Get
            Return _asset_id
        End Get
        Set(value As Integer)
            _asset_id = value
            NotifyPropertyChanged("ASSET_ID")
        End Set
    End Property

    Public Property EMP_ID As Integer
        Get
            Return _emp_id
        End Get
        Set(value As Integer)
            _emp_id = value
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property PREVIOUS_ID As Integer
        Get
            Return _previous_id
        End Get
        Set(value As Integer)
            _previous_id = value
            NotifyPropertyChanged("PREVIOUS_ID")
        End Set
    End Property

    Public Property PREVIOUS_OWNER As String
        Get
            Return _previous_owner
        End Get
        Set(value As String)
            _previous_owner = value
            NotifyPropertyChanged("PREVIOUS_OWNER")
        End Set
    End Property

    Public Property ASSET_DESC As String
        Get
            Return Trim(_asset_desc)
        End Get
        Set(value As String)
            _asset_desc = Trim(value)
            NotifyPropertyChanged("ASSET_DESC")
        End Set
    End Property

    Public Property MANUFACTURER As String
        Get
            Return _manufacturer
        End Get
        Set(value As String)
            _manufacturer = value
            NotifyPropertyChanged("MANUFACTURER")
        End Set
    End Property

    Public Property MODEL_NO As String
        Get
            Return _model_no
        End Get
        Set(value As String)
            _model_no = value
            NotifyPropertyChanged("MODEL_NO")
        End Set
    End Property

    Public Property SERIAL_NO As String
        Get
            Return _serial_no
        End Get
        Set(value As String)
            _serial_no = value
            NotifyPropertyChanged("SERIAL_NO")
        End Set
    End Property

    Public Property ASSET_TAG As String
        Get
            Return _asset_tag
        End Get
        Set(value As String)
            _asset_tag = value
            NotifyPropertyChanged("ASSET_TAG")
        End Set
    End Property

    Public Property DATE_PURCHASED As DateTime
        Get
            Return _date_purchased
        End Get
        Set(value As DateTime)
            _date_purchased = value
            NotifyPropertyChanged("DATE_PURCHASED")
        End Set
    End Property

    Public Property STATUS As Integer
        Get
            Return _status
        End Get
        Set(value As Integer)
            _status = value
            NotifyPropertyChanged("STATUS")
        End Set
    End Property

    Public Property OTHER_INFO As String
        Get
            Return _other_info
        End Get
        Set(value As String)
            _other_info = value
            NotifyPropertyChanged("OTHER_INFO")
        End Set
    End Property

    Public Property DATE_ASSIGNED As DateTime
        Get
            Return _date_assigned
        End Get
        Set(value As DateTime)
            _date_assigned = value
            NotifyPropertyChanged("DATE_ASSIGNED")
        End Set
    End Property

    Public Property COMMENTS As String
        Get
            Return _comments
        End Get
        Set(value As String)
            _comments = value
            NotifyPropertyChanged("COMMENTS")
        End Set
    End Property

    Public Property FULL_NAME As String
        Get
            Return _fullname
        End Get
        Set(value As String)
            _fullname = value
            NotifyPropertyChanged("FULL_NAME")
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

    Public Property STATUS_DESCR As String
        Get
            Return _statusDescr
        End Get
        Set(value As String)
            _statusDescr = value
            NotifyPropertyChanged("STATUS_DESCR")
        End Set
    End Property

    Public Property DATE_DESCR As String
        Get
            Return _dateDescr
        End Get
        Set(value As String)
            _dateDescr = value
            NotifyPropertyChanged("DATE_DESCR")
        End Set
    End Property

    Public Property ASSIGNED_TO As Integer
        Get
            Return _assigned_to
        End Get
        Set(value As Integer)
            _assigned_to = value
            NotifyPropertyChanged("ASSIGNED_TO")
        End Set
    End Property

    Public Property APPROVAL As Integer
        Get
            Return _approval
        End Get
        Set(value As Integer)
            _approval = value
            NotifyPropertyChanged("APPROVAL")
        End Set
    End Property

    Public Property ISAPPROVED As Boolean
        Get
            Return _isapproved
        End Get
        Set(value As Boolean)
            _isapproved = value
            NotifyPropertyChanged("ISAPPROVED")
        End Set
    End Property

    Public Property TABLE_NAME As String
        Get
            Return _table_name
        End Get
        Set(value As String)
            _table_name = value
            NotifyPropertyChanged("TABLE_NAME")
        End Set
    End Property

    Public Property EMPLOYEE_NAME As String
        Get
            Return _employeeName
        End Get
        Set(value As String)
            _employeeName = value
            NotifyPropertyChanged("EMPLOYEE_NAME")
        End Set
    End Property

    Public Property FIRST_NAME As String
        Get
            Return _firstName
        End Get
        Set(value As String)
            _firstName = value
            NotifyPropertyChanged("FIRST_NAME")
        End Set
    End Property

    Public Property NICK_NAME As String
        Get
            Return _nickName
        End Get
        Set(value As String)
            _nickName = value
            NotifyPropertyChanged("NICK_NAME")
        End Set
    End Property

    Public Property DATE_BORROWED As String
        'Get
        '    Return _date_borrowed
        'End Get
        'Set(value As DateTime)
        '    _date_borrowed = value
        '    NotifyPropertyChanged("DATE_BORROWED")
        'End Set

        Get
            If _date_borrowed = CDate("01/01/9999") Then
                Return String.Empty
            Else
                Return _date_borrowed.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _date_borrowed = Nothing
            Else
                _date_borrowed = CDate(value)
            End If
            NotifyPropertyChanged("DATE_BORROWED")
        End Set
    End Property

    Public Property DATE_RETURNED As String
        'Get
        '    Return _date_returned
        'End Get
        'Set(value As String)
        '    _date_returned = value
        '    NotifyPropertyChanged("DATE_RETURNED")
        'End Set
        Get
            If _date_returned = CDate("01/01/9999") Then
                Return String.Empty
            Else
                Return _date_returned.ToString("MM/dd/yyyy")
            End If
        End Get
        Set(value As String)
            If value = Nothing Then
                _date_returned = Nothing
            Else
                _date_returned = CDate(value)
            End If
            NotifyPropertyChanged("DATE_RETURNED")
        End Set

    End Property

    Public Property ASSET_BORROWING_ID As Integer
        Get
            Return _asset_borrowing_id
        End Get
        Set(value As Integer)
            _asset_borrowing_id = value
            NotifyPropertyChanged("ASSET_BORROWING_ID")
        End Set
    End Property

    Public Property TRANS_FG As Integer
        Get
            Return _trans_fg
        End Get
        Set(value As Integer)
            _trans_fg = value
            NotifyPropertyChanged("TRANS_FG")
        End Set
    End Property
#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
