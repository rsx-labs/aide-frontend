Imports System.ComponentModel
Imports System.Collections.ObjectModel

''' <summary>
'''BY GIANN CARLO CAMILO / CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class ConcernViewModel

    Implements INotifyPropertyChanged

    Private _db As New ConcernDBProvider
    Private _concernList As New ObservableCollection(Of ConcernModel)
    Private _selectedConcern As New ConcernModel
    Private _generatedRefNo As New ConcernModel()
    Private _listOfActionModel As New ObservableCollection(Of ConcernModel)
    Private _listofActionInConcern As New ObservableCollection(Of ConcernModel)
    Private _getSelectedAction As New ConcernModel
    Private _getBetweenDate As New ConcernModel

    Sub New()
    End Sub

#Region "Properties"

    Public Property ConcernList As ObservableCollection(Of ConcernModel)
        Get
            Return _concernList
        End Get
        Set(value As ObservableCollection(Of ConcernModel))
            _concernList = value
            NotifyPropertyChanged("ConcernList")
        End Set
    End Property

    Public Property SelectedConcern As ConcernModel
        Get
            Return _selectedConcern
        End Get
        Set(value As ConcernModel)
            _selectedConcern = value
            NotifyPropertyChanged("SelectedConcern")
        End Set
    End Property


    Public Property GeneratedRefNo As ConcernModel
        Get
            Return _generatedRefNo
        End Get
        Set(value As ConcernModel)
            _generatedRefNo = value
            NotifyPropertyChanged("GeneratedRefNo")
        End Set
    End Property

    Public Property listAction As ObservableCollection(Of ConcernModel)
        Get
            Return _listOfActionModel
        End Get
        Set(value As ObservableCollection(Of ConcernModel))
            _listOfActionModel = value
            NotifyPropertyChanged("listAction")
        End Set
    End Property

    Public Property ListOfActionInConcern As ObservableCollection(Of ConcernModel)
        Get
            Return _listofActionInConcern
        End Get
        Set(value As ObservableCollection(Of ConcernModel))
            _listofActionInConcern = value
            NotifyPropertyChanged("ListOfActionInConcern")
        End Set
    End Property

    Public Property GetSelectedAction As ConcernModel
        Get
            Return _getSelectedAction
        End Get
        Set(value As ConcernModel)
            _getSelectedAction = value
            NotifyPropertyChanged("GetSelectedAction")
        End Set
    End Property

    Public Property GetBetWeenDate As ConcernModel
        Get
            Return _getBetweenDate
        End Get
        Set(value As ConcernModel)
            _getBetweenDate = value
            NotifyPropertyChanged("GetBetWeenDate")
        End Set
    End Property

#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
