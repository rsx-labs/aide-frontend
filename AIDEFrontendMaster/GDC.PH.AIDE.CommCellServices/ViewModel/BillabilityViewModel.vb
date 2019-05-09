Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports UI_AIDE_CommCellServices

'''''''''''''''''''''''''''''''''
'   AEVAN CAMILLE BATONGBACAL   '
'''''''''''''''''''''''''''''''''
Public Class BillabilityViewModel
    Implements INotifyPropertyChanged

    Private _billabilityMonthList As ObservableCollection(Of ResourcePlannerModel)
    Private _billabilityWeekList As ObservableCollection(Of ResourcePlannerModel)

    Private client As AideServiceClient

    Sub New()
    End Sub

    Public Property BillabilityMonth As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _billabilityMonthList
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _billabilityMonthList = value
            NotifyPropertyChanged("BillabilityMonth")
        End Set
    End Property

    Public Property BillabilityWeek As ObservableCollection(Of ResourcePlannerModel)
        Get
            Return _billabilityWeekList
        End Get
        Set(value As ObservableCollection(Of ResourcePlannerModel))
            _billabilityWeekList = value
            NotifyPropertyChanged("BillabilityWeek")
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
