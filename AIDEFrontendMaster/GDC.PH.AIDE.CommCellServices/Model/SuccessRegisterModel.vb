Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Krizza Tolento
''' </summary>
''' <remarks></remarks>
Public Class SuccessRegisterModel

    Implements INotifyPropertyChanged

    Private _id As Integer
    Private _empID As Integer
    Private _nickname As String
    Private _dateinput As Date
    Private _details As String
    Private _whosinvolve As String
    Private _additional As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawSuccessRegister As MySuccessRegister)
        Me.SuccessID = rawSuccessRegister.SuccessID
        Me.Emp_ID = rawSuccessRegister.EmpID
        Me.Nick_Name = rawSuccessRegister.NickName
        Me.DateInput = rawSuccessRegister.DateInput
        Me.DetailsOfSuccess = rawSuccessRegister.DetailsOfSuccess
        Me.WhosInvolve = rawSuccessRegister.WhosInvolve
        Me.AdditionalInformation = rawSuccessRegister.AdditionalInformation
    End Sub

    Public Property SuccessID As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
            NotifyPropertyChanged("SuccessID")
        End Set
    End Property

    Public Property Emp_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            NotifyPropertyChanged("Emp_ID")
        End Set
    End Property

    Public Property Nick_Name As String
        Get
            Return _nickname
        End Get
        Set(value As String)
            _nickname = value
            NotifyPropertyChanged("Nick_Name")
        End Set
    End Property

    Public Property DateInput As Date
        Get
            _dateinput = _dateinput.ToString("d")
            Return _dateinput
        End Get
        Set(value As Date)
            value = value.ToString("d")
            _dateinput = value
            NotifyPropertyChanged("DateInput")
        End Set
    End Property

    Public Property DetailsOfSuccess As String
        Get
            Return _details
        End Get
        Set(value As String)
            _details = value
            NotifyPropertyChanged("DetailsOfSuccess")
        End Set
    End Property

    Public Property WhosInvolve As String
        Get
            Return _whosinvolve
        End Get
        Set(value As String)
            _whosinvolve = value
            NotifyPropertyChanged("WhosInvolve")
        End Set
    End Property

    Public Property AdditionalInformation As String
        Get
            Return _additional
        End Get
        Set(value As String)
            _additional = value
            NotifyPropertyChanged("AdditionalInformation")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class

Public Class NicknameModel
    Implements INotifyPropertyChanged

    Public Sub New()
    End Sub


    Public Sub New(ByVal rawNickname As MyNickname)
        Me.Emp_ID = rawNickname.EmpID
        Me.First_Name = rawNickname.FirstName
        Me.Nick_Name = rawNickname.NickName
        Me.EMPLOYEE_NAME = rawNickname.EmployeeName
        Me._imagePath = rawNickname.ImagePath
    End Sub

    Private _empID As Integer
    Private _nickname As String
    Private _firstName As String
    Private _employeename As String
    Private _imagePath As String

    Public Property EMP_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = value
            NotifyPropertyChanged("EMP_ID")
        End Set
    End Property

    Public Property Nick_Name As String
        Get
            Return _nickname
        End Get
        Set(value As String)
            _nickname = value
            NotifyPropertyChanged("Nick_Name")
        End Set
    End Property

    Public Property First_Name As String
        Get
            Return _firstName
        End Get
        Set(value As String)
            _firstName = value
            NotifyPropertyChanged("First_Name")
        End Set
    End Property

    Public Property EMPLOYEE_NAME As String
        Get
            Return _employeename
        End Get
        Set(value As String)
            _employeename = value
            NotifyPropertyChanged("EMPLOYEE_NAME")
        End Set
    End Property

    Public Property Image_Path As String
        Get
            If _imagePath = String.Empty Then
                Return "\..\Assets\EmployeePhotos\shadowImage.png"
            End If
            Return _imagePath
        End Get
        Set(value As String)
            _imagePath = value
            NotifyPropertyChanged("Image_Path")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
