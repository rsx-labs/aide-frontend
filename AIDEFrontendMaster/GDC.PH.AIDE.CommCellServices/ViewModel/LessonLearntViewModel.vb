Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

''' <summary>
''' By Marivic Espino
''' </summary>
Public Class LessonLearntViewModel
    Implements INotifyPropertyChanged

    Private _lessonLearnt As New ObservableCollection(Of LessonLearntModel)
    Private _selectedLessonLearnt As LessonLearntModel

    Public Property LessonLearntList As ObservableCollection(Of LessonLearntModel)
        Get
            Return _lessonLearnt
        End Get
        Set(value As ObservableCollection(Of LessonLearntModel))
            _lessonLearnt = value
            NotifyPropertyChanged("LessonLearntList")
        End Set
    End Property

    Public Property SelectedLessonLearnt As LessonLearntModel
        Get
            Return _selectedLessonLearnt
        End Get
        Set(value As LessonLearntModel)
            _selectedLessonLearnt = value
            NotifyPropertyChanged("SelectedLessonLearnt")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
