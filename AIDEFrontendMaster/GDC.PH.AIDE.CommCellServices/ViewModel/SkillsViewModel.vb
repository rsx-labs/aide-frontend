Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class SkillsViewModel
    Implements INotifyPropertyChanged
    Implements IAideServiceCallback


    Private _skill As New ObservableCollection(Of SkillsModel)
    Private _empSkill As New ObservableCollection(Of SkillsModel)
    Private _empDetails As New ObservableCollection(Of SkillsModel)
    Private aide As ServiceReference1.AideServiceClient

    Public Property SkillList As ObservableCollection(Of SkillsModel)
        Get
            Return _skill
        End Get
        Set(value As ObservableCollection(Of SkillsModel))
            _skill = value
            NotifyPropertyChanged("SkillList")
        End Set
    End Property

    Public Property EmpSkillList As ObservableCollection(Of SkillsModel)
        Get
            Return _empSkill
        End Get
        Set(value As ObservableCollection(Of SkillsModel))
            _empSkill = value
            NotifyPropertyChanged("EmpSkillList")
        End Set
    End Property

    Public Property EmpDetails As ObservableCollection(Of SkillsModel)
        Get
            Return _empDetails
        End Get
        Set(value As ObservableCollection(Of SkillsModel))
            _empDetails = value
            NotifyPropertyChanged("EmpDetails")
        End Set
    End Property

#Region "Main Methods"
    Public Sub InsertNewSkills(ByVal skill As Skills)

    End Sub

    Public Sub UpdateSkills(ByVal skill As Skills)

    End Sub

    Public Function GetSkillsList(ByVal empID As Integer) As Skills
        Try

        Catch ex As Exception

        End Try
        Return Nothing
    End Function
#End Region

#Region "Service Function/Methods"
    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate
        Throw New NotImplementedException()
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Public WriteOnly Property Service As AideServiceClient
        'Get
        '    Return _optionValue
        'End Get
        Set(value As AideServiceClient)
            aide = value
        End Set
    End Property
#End Region

End Class
