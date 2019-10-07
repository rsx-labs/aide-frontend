Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Public Class ResourcePlannerViewModel
    Implements INotifyPropertyChanged

    Private _resource As New ObservableCollection(Of ResourcePlannerModel)
    Private _category As New ObservableCollection(Of ResourcePlannerModel)
    Private _filterCategory As New ObservableCollection(Of ResourcePlannerModel)
    Private _resourceListLeaveCredits As New ObservableCollection(Of ResourcePlannerModel)
    Private _allLeavesList As New ObservableCollection(Of ResourcePlannerModel)

    Public Property ResourceList As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _resource
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _resource = value
            NotifyPropertyChanged("ResourceList")
        End Set
    End Property

    Public Property CategoryList As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _category
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _category = value
            NotifyPropertyChanged("CategoryList")
        End Set
    End Property

    Public Property FilterCategoryList As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _filterCategory
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _filterCategory = value
            NotifyPropertyChanged("FilterCategoryList")
        End Set
    End Property

    Public Property ResourceListLeaveCredits As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _resourceListLeaveCredits
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _resourceListLeaveCredits = value
            NotifyPropertyChanged("ResourceListLeaveCredits")
        End Set
    End Property

    Public Property ResourceListSLYearLy As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _resourceListLeaveCredits
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _resourceListLeaveCredits = value
            NotifyPropertyChanged("ResourceListSLWeekLy")
        End Set
    End Property

    Public Property ResourceListSLMonthLy As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _resourceListLeaveCredits
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _resourceListLeaveCredits = value
            NotifyPropertyChanged("ResourceListSLMonthLy")
        End Set
    End Property

    Public Property ResourceListVLYearLy As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _resourceListLeaveCredits
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _resourceListLeaveCredits = value
            NotifyPropertyChanged("ResourceListVLWeekLy")
        End Set
    End Property

    Public Property ResourceListVLMonthLy As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _resourceListLeaveCredits
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _resourceListLeaveCredits = value
            NotifyPropertyChanged("ResourceListVLMonthLy")
        End Set
    End Property

    Public Property AllLeavesList As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _allLeavesList
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _allLeavesList = value
            NotifyPropertyChanged("AllLeavesList")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
