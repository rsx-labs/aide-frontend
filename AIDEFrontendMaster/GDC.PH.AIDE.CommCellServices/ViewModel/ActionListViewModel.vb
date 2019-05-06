Imports System.ComponentModel
Imports System.Collections.ObjectModel

''' <summary>
''' By Lemuela Abulencia
''' </summary>
''' <remarks></remarks>
Public Class ActionListViewModel
    Implements INotifyPropertyChanged

    Private _objectActionSet As New ObservableCollection(Of ActionModel)
    Private actprov As New ActionListDBProvider

    Sub New()
        For Each _objprovider As myActionSet In actprov._getobAction
            objectActionSet.Add(New ActionModel(_objprovider))
        Next

    End Sub

    Public Property objectActionSet As ObservableCollection(Of ActionModel)
        Get
            Return _objectActionSet
        End Get
        Set(value As ObservableCollection(Of ActionModel))
            _objectActionSet = value
            NotifyPropertyChanged("objectActionSet")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
