Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

''' <summary>
''' By Aevan Camille Batongbacal
''' </summary>
''' <remarks></remarks>
Public Class SuccessRegisterViewModel
    Implements INotifyPropertyChanged

    Public _successRegisterList As New ObservableCollection(Of SuccessRegisterModel)
    Private client As AideServiceClient


    Sub New()
        'SuccessRegister(email)
    End Sub

    Public Property SuccessRegisterList As ObservableCollection(Of SuccessRegisterModel)
        Get
            Return _successRegisterList
        End Get
        Set(value As ObservableCollection(Of SuccessRegisterModel))
            _successRegisterList = value
            NotifyPropertyChanged("SuccessRegisterList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class

Public Class NicknameViewModel
    Implements INotifyPropertyChanged

    Private _nicknameList As New ObservableCollection(Of NicknameModel)
    Private client As AIDEServiceClient


    Sub New()
        'SuccessRegister(email)
    End Sub

    Public Property NicknameList As ObservableCollection(Of NicknameModel)
        Get
            Return _nicknameList
        End Get
        Set(value As ObservableCollection(Of NicknameModel))
            _nicknameList = value
            NotifyPropertyChanged("NicknameList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
