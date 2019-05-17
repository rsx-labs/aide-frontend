Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class SabaLearningViewModel
    Implements INotifyPropertyChanged


    Private _objSabaLearningset As New ObservableCollection(Of SabaLearningModel)
    Private _sabaVMmodel As New SabaLearningModel
    Private mySabaLearningSet As New SabaLearningDBProvider

    Sub New()
        For Each _mysabaSet As mySabaLearningSet In mySabaLearningSet._getobjSabaLearning
            _objSabaLearningset.Add(New SabaLearningModel(_mysabaSet))
        Next
    End Sub

    Public Property ObjectSabaLearningSet As ObservableCollection(Of SabaLearningModel)
        Get
            Return Me._objSabaLearningset
        End Get
        Set(value As ObservableCollection(Of SabaLearningModel))
            Me._objSabaLearningset = value
            NotifyPropertyChanged("ObjectSabaLearningSet")
        End Set
    End Property

    Public Property ObjectCompletedSabaLearningSet As ObservableCollection(Of SabaLearningModel)
        Get
            Return Me._objSabaLearningset
        End Get
        Set(value As ObservableCollection(Of SabaLearningModel))
            Me._objSabaLearningset = value
            NotifyPropertyChanged("ObjectCompletedSabaLearningSet")
        End Set
    End Property

    Public Property ObjectNotCompletedSabaLearningSet As ObservableCollection(Of SabaLearningModel)
        Get
            Return Me._objSabaLearningset
        End Get
        Set(value As ObservableCollection(Of SabaLearningModel))
            Me._objSabaLearningset = value
            NotifyPropertyChanged("ObjectNotCompletedSabaLearningSet")
        End Set
    End Property
    Public Property SabaLearningVMModel As SabaLearningModel
        Get
            Return Me._sabaVMmodel
        End Get
        Set(value As SabaLearningModel)
            Me._sabaVMmodel = value
            NotifyPropertyChanged("SabaLearningVMModel")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
