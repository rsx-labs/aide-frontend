Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class MailConfigViewModel
    Implements INotifyPropertyChanged

    Private _objectMailConfigSet As New MailConfigModel
    Private MailConfigProvider As New MailConfigDBProvider

    Public Sub New()
        _objectMailConfigSet = New MailConfigModel(MailConfigProvider._getobjmailconfig)
    End Sub

    Public Property objectMailConfigSet As MailConfigModel
        Get
            Return _objectMailConfigSet
        End Get
        Set(value As MailConfigModel)
            _objectMailConfigSet = value
            NotifyPropertyChanged("objectMailConfigSet")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
