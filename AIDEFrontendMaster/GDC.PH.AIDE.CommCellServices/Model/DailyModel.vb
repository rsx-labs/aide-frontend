Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Jhunell Barcenas
''' </summary>
''' <remarks></remarks>
Public Class DailyModel
    Private _message As String
    Private _emp As String
    Private _mon As Boolean
    Private _tue As Boolean
    Private _wed As Boolean
    Private _thurs As Boolean
    Private _fri As Boolean

    Public Property MESSAGE As String
        Get
            Return _message
        End Get
        Set(value As String)
            _message = value
        End Set
    End Property

    Public Property EMPLOYEE As String
        Get
            Return _emp
        End Get
        Set(value As String)
            _emp = value
        End Set
    End Property
    Public Property MONDAY As Boolean
        Get
            Return _mon
        End Get
        Set(value As Boolean)
            _mon = value
        End Set
    End Property

    Public Property TUESDAY As Boolean
        Get
            Return _tue
        End Get
        Set(value As Boolean)
            _tue = value
        End Set
    End Property

    Public Property WEDNESDAY As Boolean
        Get
            Return _wed
        End Get
        Set(value As Boolean)
            _wed = value
        End Set
    End Property

    Public Property THURSDAY As Boolean
        Get
            Return _thurs
        End Get
        Set(value As Boolean)
            _thurs = value
        End Set
    End Property

    Public Property FRIDAY As Boolean
        Get
            Return _fri
        End Get
        Set(value As Boolean)
            _fri = value
        End Set
    End Property

End Class