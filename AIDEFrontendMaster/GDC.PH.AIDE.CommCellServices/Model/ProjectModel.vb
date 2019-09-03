Imports System.ComponentModel

''' <summary>
'''HYACINTH AMARLES
''' </summary>
''' <remarks></remarks>
Public Class ProjectModel
    Implements INotifyPropertyChanged



    Private _ProjectID As Integer
    Private _ProjectCode As String
    Private _ProjectName As String
    Private _Category As String
    Private _Billability As String

    Public Sub New(ByVal rawProjectList As myProjectList)
        Me._ProjectID = rawProjectList.Project_ID
        Me._ProjectCode = rawProjectList.Project_Code
        Me._ProjectName = rawProjectList.Project_Name
        Me._Category = rawProjectList.category
        Me._Billability = rawProjectList.billability
    End Sub

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property ProjectID As Integer
        Set(value As Integer)
            _ProjectID = value
            NotifyPropertyChanged("ProjectID")
        End Set
        Get
            Return _ProjectID
        End Get
    End Property

    Public Property ProjectCode As String
        Set(value As String)
            _ProjectCode = value
            NotifyPropertyChanged("ProjectCode")
        End Set
        Get
            Return _ProjectCode
        End Get
    End Property

    Public Property ProjectName As String
        Set(value As String)
            _ProjectName = value
            NotifyPropertyChanged("ProjectName")
        End Set
        Get
            Return _ProjectName
        End Get
    End Property
    Public Property Category As String
        Set(value As String)
            _Category = value
            NotifyPropertyChanged("Category")
        End Set
        Get
            Return _Category
        End Get
    End Property
    Public Property Billability As String
        Set(value As String)
            _Billability = value
            NotifyPropertyChanged("Billability")
        End Set
        Get
            Return _Billability
        End Get
    End Property
    Private Sub NotifyPropertyChanged(ByVal propertyname As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyname))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
