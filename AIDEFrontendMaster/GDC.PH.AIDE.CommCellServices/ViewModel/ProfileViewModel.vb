Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input

Public Class ProfileViewModel
    Implements INotifyPropertyChanged
    Private _db As New ProfileDBProvider()
    Private _selectedUser As New ProfileModel()
    Private _usersList As New ObservableCollection(Of ProfileModel)
    Private _statusList As New ObservableCollection(Of StatusModel)
    Private _selectedStatus As New StatusModel()
    Private _editCommand As ICommand
    Private _saveCommand As ICommand

    Sub New()
        EditMode = False
        'AddMode = False
        Editable = False
    End Sub

    Public Property UsersList As ObservableCollection(Of ProfileModel)
        Get
            Return _usersList
        End Get
        Set(value As ObservableCollection(Of ProfileModel))
            _usersList = value
            NotifyPropertyChanged("UsersList")
        End Set
    End Property

    Public Property StatusList As ObservableCollection(Of StatusModel)
        Get
            Return _statusList
        End Get
        Set(value As ObservableCollection(Of StatusModel))
            _statusList = value
            NotifyPropertyChanged("StatusList")
        End Set
    End Property

    Public Property SelectedStatus As StatusModel
        Get
            Return _selectedStatus
        End Get
        Set(value As StatusModel)
            _selectedStatus = value
            NotifyPropertyChanged("SelectedStatus")
        End Set
    End Property

    Public Property SelectedUser As ProfileModel
        Get
            Return _selectedUser
        End Get
        Set(value As ProfileModel)
            _selectedUser = value
            NotifyPropertyChanged("SelectedUser")
        End Set
    End Property

    Private _editMode As Boolean
    Public Property EditMode As Boolean
        Get
            Return _editMode
        End Get
        Set(value As Boolean)
            _editMode = value
            NotifyPropertyChanged("EditMode")
        End Set
    End Property

#Region "_addMode"
    'Private _addMode As Boolean
    'Public Property AddMode As Boolean
    '    Get
    '        Return _addMode
    '    End Get
    '    Set(value As Boolean)
    '        _addMode = value
    '        NotifyPropertyChanged("AddMode")
    '    End Set
    'End Property
#End Region

    Private _editable As Boolean
    Public Property Editable As Boolean
        Get
            Return _editable
        End Get
        Set(value As Boolean)
            _editable = value
            NotifyPropertyChanged("Editable")
        End Set
    End Property

    Public Property EditCommand As ICommand
        Get
            If (_editCommand Is Nothing) Then
                _editCommand = New RelayCommand(Sub(param) Me.Edit(), Nothing)
            End If

            Return _editCommand
        End Get
        Set(value As ICommand)
            _editCommand = value
        End Set
    End Property
#Region "Command"
    'Public Property DeleteCommand As ICommand
    '    Get
    '        If (_deleteCommand Is Nothing) Then
    '            _deleteCommand = New RelayCommand(Sub(param) Me.Delete(), Nothing)
    '        End If

    '        Return _deleteCommand
    '    End Get
    '    Set(value As ICommand)
    '        _deleteCommand = value
    '    End Set
    'End Property

    'Public Property AddCommand As ICommand
    '    Get
    '        If (_addCommand Is Nothing) Then
    '            _addCommand = New RelayCommand(Sub(param) Me.Add(), Nothing)
    '        End If

    '        Return _addCommand
    '    End Get
    '    Set(value As ICommand)
    '        _addCommand = value
    '    End Set
    'End Property

    'Private Sub Add()
    '    AddMode = True

    '    Dim Addemp As New UserModel
    '    Addemp.UserID = UsersList.Count + 1

    '    _usersList.Add(Addemp)
    '    SelectedUser = Addemp


    '    EditMode = True
    '    Editable = True

    'End Sub
#End Region
    Private Sub Edit()
        EditMode = True
        'AddMode = False
        Editable = True

    End Sub

#Region "Delete Sub"
    'Private Sub Delete()
    '    _db.DeleteUser(SelectedUser.UserID)
    '    _usersList.Clear()
    '    For Each rawUser As MyUser In _db.GetUsers()
    '        _usersList.Add(New UserModel(rawUser))
    '    Next

    '    If (_usersList.Count > 0) Then
    '        SelectedUser = _usersList(0)
    '    End If
    'End Sub
#End Region

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
