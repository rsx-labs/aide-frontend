Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
Public Class AssetsViewModel
    Implements INotifyPropertyChanged

    Public _assetList As New ObservableCollection(Of AssetsModel)
    Public _assetInventoryList As New ObservableCollection(Of AssetsModel)
    Public _assetHistoryList As New ObservableCollection(Of AssetsModel)
    Public _assetTypeList As New ObservableCollection(Of AssetsModel)
    Public _assetManufacturerList As New ObservableCollection(Of AssetsModel)
    Public _assetManagerList As New ObservableCollection(Of AssetsModel)

    Public Property AssetList As ObservableCollection(Of AssetsModel)
        Get
            Return _assetList
        End Get
        Set(value As ObservableCollection(Of AssetsModel))
            _assetList = value
            NotifyPropertyChanged("AssetList")
        End Set
    End Property

    Public Property AssetInventoryList As ObservableCollection(Of AssetsModel)
        Get
            Return _assetInventoryList
        End Get
        Set(value As ObservableCollection(Of AssetsModel))
            _assetInventoryList = value
            NotifyPropertyChanged("AssetInventoryList")
        End Set
    End Property

    Public Property AssetHistoryList As ObservableCollection(Of AssetsModel)
        Get
            Return _assetHistoryList
        End Get
        Set(value As ObservableCollection(Of AssetsModel))
            _assetHistoryList = value
            NotifyPropertyChanged("AssetHistoryList")
        End Set
    End Property

    Public Property AssetTypeList As ObservableCollection(Of AssetsModel)
        Get
            Return _assetTypeList
        End Get
        Set(value As ObservableCollection(Of AssetsModel))
            _assetTypeList = value
            NotifyPropertyChanged("AssetTypeList")
        End Set
    End Property

    Public Property AssetManufacturerList As ObservableCollection(Of AssetsModel)
        Get
            Return _assetManufacturerList
        End Get
        Set(value As ObservableCollection(Of AssetsModel))
            _assetManufacturerList = value
            NotifyPropertyChanged("AssetManufacturerList")
        End Set
    End Property

    Public Property AssetManagerList As ObservableCollection(Of AssetsModel)
        Get
            Return _assetManagerList
        End Get
        Set(value As ObservableCollection(Of AssetsModel))
            _assetManagerList = value
            NotifyPropertyChanged("AssetManagerList")
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
