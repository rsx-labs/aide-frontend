Imports System.ComponentModel
Imports System.Collections.ObjectModel
Public Class ReportsViewModel
    Implements INotifyPropertyChanged


    Private _objReportsset As New ObservableCollection(Of ReportsModel)
    Private _reportsVMmodel As New ReportsModel
    Private myReportsSet As New ReportsDBProvider

    Sub New()
        For Each _myreportsSet As myReportsSet In myReportsSet._getobjReports
            _objReportsset.Add(New ReportsModel(_myreportsSet))
        Next
    End Sub

    Public Property ObjectReportsSet As ObservableCollection(Of ReportsModel)
        Get
            Return Me._objReportsset
        End Get
        Set(value As ObservableCollection(Of ReportsModel))
            Me._objReportsset = value
            NotifyPropertyChanged("ObjectReportsSet")
        End Set
    End Property

    Public Property ObjectCompletedReportsSet As ObservableCollection(Of ReportsModel)
        Get
            Return Me._objReportsset
        End Get
        Set(value As ObservableCollection(Of ReportsModel))
            Me._objReportsset = value
            NotifyPropertyChanged("ObjectCompletedReportsSet")
        End Set
    End Property

    Public Property ObjectNotCompletedReportsSet As ObservableCollection(Of ReportsModel)
        Get
            Return Me._objReportsset
        End Get
        Set(value As ObservableCollection(Of ReportsModel))
            Me._objReportsset = value
            NotifyPropertyChanged("ObjectNotCompletedReportsSet")
        End Set
    End Property
    Public Property ReportsVMModel As ReportsModel
        Get
            Return Me._reportsVMmodel
        End Get
        Set(value As ReportsModel)
            Me._reportsVMmodel = value
            NotifyPropertyChanged("ReportsVMModel")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
