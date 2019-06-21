Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class SendCodeViewModel
    Implements INotifyPropertyChanged

    Private _objectSendCodeSet As New ObservableCollection(Of SendCodeModel)
    Private _objectSendCodeSet2 As New SendCodeModel
    Private SendCodeProvider As New SendCodeDBProvider

    Sub New()
        _objectSendCodeSet2 = New SendCodeModel(SendCodeProvider._getobSendCode)
    End Sub

    Public Property objectSendCodeSet As ObservableCollection(Of SendCodeModel)
        Get
            Return _objectSendCodeSet
        End Get
        Set(value As ObservableCollection(Of SendCodeModel))
            _objectSendCodeSet = value
            NotifyPropertyChanged("objectSendCodeSet")
        End Set
    End Property

    Public Property objectSendCodeSet2 As SendCodeModel
        Get
            Return _objectSendCodeSet2
        End Get
        Set(value As SendCodeModel)
            _objectSendCodeSet2 = value
            NotifyPropertyChanged("objectSendCodeSet2")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
