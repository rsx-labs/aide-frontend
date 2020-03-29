Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class ProblemViewModel
    Implements INotifyPropertyChanged

    Private _problemlist As ObservableCollection(Of ProblemModel)
    Private _problemCauselist As ObservableCollection(Of ProblemModel)
    Private _problemOptionlist As ObservableCollection(Of ProblemModel)
    Private _problemSolutionlist As ObservableCollection(Of ProblemModel)
    Private _problemImplementlist As ObservableCollection(Of ProblemModel)
    Private _problemResultlist As ObservableCollection(Of ProblemModel)


    Sub New()
        _problemlist = New ObservableCollection(Of ProblemModel)
        _problemCauselist = New ObservableCollection(Of ProblemModel)
        _problemOptionlist = New ObservableCollection(Of ProblemModel)
        _problemSolutionlist = New ObservableCollection(Of ProblemModel)
        _problemImplementlist = New ObservableCollection(Of ProblemModel)
        _problemResultlist = New ObservableCollection(Of ProblemModel)
    End Sub

    Public Property PROBLEM_LIST As ObservableCollection(Of ProblemModel)
        Get
            Return _problemlist
        End Get
        Set(value As ObservableCollection(Of ProblemModel))
            _problemlist = value
            NotifyPropertyChanged("PROBLEM_LIST")
        End Set
    End Property

    Public Property PROBLEM_CAUSE_LIST As ObservableCollection(Of ProblemModel)
        Get
            Return _problemCauselist
        End Get
        Set(value As ObservableCollection(Of ProblemModel))
            _problemCauselist = value
            NotifyPropertyChanged("PROBLEM_CAUSE_LIST")
        End Set
    End Property

    Public Property PROBLEM_OPTION_LIST As ObservableCollection(Of ProblemModel)
        Get
            Return _problemOptionlist
        End Get
        Set(value As ObservableCollection(Of ProblemModel))
            _problemOptionlist = value
            NotifyPropertyChanged("PROBLEM_OPTION_LIST")
        End Set
    End Property

    Public Property PROBLEM_SOLUTION_LIST As ObservableCollection(Of ProblemModel)
        Get
            Return _problemSolutionlist
        End Get
        Set(value As ObservableCollection(Of ProblemModel))
            _problemSolutionlist = value
            NotifyPropertyChanged("PROBLEM_SOLUTION_LIST")
        End Set
    End Property

    Public Property PROBLEM_IMPLEMENT_LIST As ObservableCollection(Of ProblemModel)
        Get
            Return _problemImplementlist
        End Get
        Set(value As ObservableCollection(Of ProblemModel))
            _problemImplementlist = value
            NotifyPropertyChanged("PROBLEM_IMPLEMENT_LIST")
        End Set
    End Property

    Public Property PROBLEM_RESULT_LIST As ObservableCollection(Of ProblemModel)
        Get
            Return _problemResultlist
        End Get
        Set(value As ObservableCollection(Of ProblemModel))
            _problemResultlist = value
            NotifyPropertyChanged("PROBLEM_RESULT_LIST")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
