Imports System.Windows.Input

Public Class RelayCommand
    Implements ICommand

    Private ReadOnly _execute As Action(Of Object)
    Private ReadOnly _canExecute As Predicate(Of Object)

    Public Sub New(ByVal execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub

    Sub New(ByVal execute As Action(Of Object), ByVal canExecutePredicate As Predicate(Of Object))
        If (execute Is Nothing) Then
            Throw New ArgumentException("Execute is Null")
        End If

        _execute = execute
        _canExecute = canExecutePredicate
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If (_canExecute Is Nothing) Then
            CanExecute = True
        Else
            CanExecute = _canExecute(parameter)
        End If
    End Function

    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        AddHandler(ByVal value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler CommandManager.RequerySuggested, value
        End RemoveHandler

        RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
        End RaiseEvent
    End Event

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute(parameter)
    End Sub

End Class
