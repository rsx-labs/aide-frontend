Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.IO.Directory

Public Class BirthdayListModel
    Implements INotifyPropertyChanged

#Region "Data Members"

    Private _eMP_ID As Integer
    Private _birthday As DateTime
    Private _full_name As String
    Private _first_name As String
    Private _last_name As String
    Private _image_path As String

#End Region

    Public Sub New()
    End Sub

    Public Sub New(ByVal rawContactList As MyBirthdayList)
        Me.BIRTHDAY = rawContactList.Birthday
        Me.EMP_ID = rawContactList.EmpID
        Me.FIRST_NAME = rawContactList.FIRST_NAME
        Me.LAST_NAME = rawContactList.LAST_NAME
        Me.FULL_NAME = rawContactList.FULL_NAME
        Me.IMAGE_PATH = rawContactList.IMAGE_PATH
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

    Public Property BIRTHDAY As String
        Get            'CDate(_birthday.ToString())
            Return _birthday.ToString("M")
        End Get
        Set(value As String)
            _birthday = value
            NotifyPropertyChanged("BIRTHDAY")
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


    Public Property FULL_NAME As String
        Get
            Return _full_name
        End Get
        Set(value As String)
            _full_name = value
            NotifyPropertyChanged("FULL_NAME")
        End Set
    End Property

    Public Property IMAGE_PATH As String
        Get
            '
            Return _image_path
        End Get
        Set(value As String)
            _image_path = value
            NotifyPropertyChanged("IMAGE_PATH")
        End Set
    End Property

#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
