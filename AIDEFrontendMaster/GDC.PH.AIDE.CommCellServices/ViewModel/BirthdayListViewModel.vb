Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
Public Class BirthdayListViewModel
    Implements INotifyPropertyChanged

    Public _birthdayList As New ObservableCollection(Of BirthdayListModel)
    Public _birthdayListMonth As New ObservableCollection(Of BirthdayListModel)
    'Private _NicknameDBProvider As ObservableCollection(Of MyNickname)
    Private client As AideServiceClient


    Sub New()
        'SuccessRegister(email)
    End Sub

    Public Property BirthdayList As ObservableCollection(Of BirthdayListModel)
        Get
            Return _birthdayList
        End Get
        Set(value As ObservableCollection(Of BirthdayListModel))
            _birthdayList = value
            NotifyPropertyChanged("BirthdayList")
        End Set
    End Property

    Public Property BirthdayListMonth As ObservableCollection(Of BirthdayListModel)
        Get
            Return _birthdayListMonth
        End Get
        Set(value As ObservableCollection(Of BirthdayListModel))
            _birthdayListMonth = value
            NotifyPropertyChanged("BirthdayListMonth")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
