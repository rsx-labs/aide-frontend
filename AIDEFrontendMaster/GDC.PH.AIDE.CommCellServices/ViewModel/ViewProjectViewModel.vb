Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input


''' <summary>
''' BY GIANN CARLO CAMILO 
''' </summary>
''' <remarks></remarks>
Public Class ViewProjectViewModel
    Implements INotifyPropertyChanged

    Private _db As New ViewProjectProvider
    Private _employeeList As New ObservableCollection(Of ViewProjectModel)

    Private iName As String
    Private iPosition As String

    Sub New()
        Try
            For Each rawUser As MyProjectLists In _db.GetProjectList()
                _employeeList.Add(New ViewProjectModel(rawUser))
            Next
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

#Region "Properties"
    Public Property ProjectList As ObservableCollection(Of ViewProjectModel)
        Get
            Return _employeeList
        End Get
        Set(value As ObservableCollection(Of ViewProjectModel))
            _employeeList = value
            NotifyPropertyChanged("ProjectList")
        End Set
    End Property




#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
