Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class PaginatedObservableCollection(Of T)
    Inherits ObservableCollection(Of T)

#Region "Fields"
    Private originalCollection As List(Of T)
    Private currentPageIndex As Integer
    Private itemCountPerPage As Integer
#End Region

#Region "Constructor"
    Public Sub New(ByVal collecton As IEnumerable(Of T))
        currentPageIndex = 0
        itemCountPerPage = 1
        originalCollection = New List(Of T)(collecton)
        RecalculateThePageItems()
    End Sub

    Public Sub New(ByVal itemsPerPage As Integer)
        currentPageIndex = 0
        itemCountPerPage = itemsPerPage
        originalCollection = New List(Of T)()
    End Sub

    Public Sub New()
        currentPageIndex = 0
        itemCountPerPage = 1
        originalCollection = New List(Of T)()
    End Sub
#End Region

#Region "Properties"
    Public Property PageSize As Integer
        Get
            Return itemCountPerPage
        End Get
        Set(ByVal value As Integer)

            If value >= 0 Then
                itemCountPerPage = value
                RecalculateThePageItems()
                OnPropertyChanged(New PropertyChangedEventArgs("PageSize"))
            End If
        End Set
    End Property

    Public Property CurrentPage As Integer
        Get
            Return currentPageIndex
        End Get
        Set(ByVal value As Integer)

            If value >= 0 Then
                currentPageIndex = value
                RecalculateThePageItems()
                OnPropertyChanged(New PropertyChangedEventArgs("CurrentPage"))
            End If
        End Set
    End Property

    Public Property Collections As List(Of T)
        Get
            Return originalCollection
        End Get
        Set(ByVal value As List(Of T))
            originalCollection = value
            OnPropertyChanged(New PropertyChangedEventArgs("Collections"))
        End Set
    End Property
#End Region

#Region "Sub Procedures"
    Private Sub RecalculateThePageItems()
        Clear()
        Dim startIndex As Integer = currentPageIndex * itemCountPerPage

        For i As Integer = startIndex To startIndex + itemCountPerPage - 1
            If originalCollection.Count > i Then MyBase.InsertItem(i - startIndex, originalCollection(i))
        Next
    End Sub
#End Region

#Region "Overrides"
    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As T)
        Dim startIndex As Integer = currentPageIndex * itemCountPerPage
        Dim endIndex As Integer = startIndex + itemCountPerPage

        If (index >= startIndex) AndAlso (index < endIndex) Then
            MyBase.InsertItem(index - startIndex, item)
            If Count > itemCountPerPage Then MyBase.RemoveItem(endIndex)
        End If

        If index >= Count Then
            originalCollection.Add(item)
        Else
            originalCollection.Insert(index, item)
        End If
    End Sub

    Protected Overrides Sub RemoveItem(ByVal index As Integer)
        Dim startIndex As Integer = currentPageIndex * itemCountPerPage
        Dim endIndex As Integer = startIndex + itemCountPerPage

        If (index >= startIndex) AndAlso (index < endIndex) Then
            MyBase.RemoveAt(index - startIndex)
            If Count <= itemCountPerPage Then MyBase.InsertItem(endIndex - 1, originalCollection(index + 1))
        End If

        originalCollection.RemoveAt(index)
    End Sub
#End Region
    
End Class
