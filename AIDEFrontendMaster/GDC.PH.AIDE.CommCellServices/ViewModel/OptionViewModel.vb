Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.ServiceModel
Imports NLog
Public Class OptionViewModel
    Implements INotifyPropertyChanged
    Implements IAideServiceCallback

#Region "Declarations"
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private _optionLst As ObservableCollection(Of OptionModel)
    Private _optionDB As OptionDBProvider
    Private _optionValue As String
    Private aide As ServiceReference1.AideServiceClient

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

#Region "Constructors"
    Sub New()

        _logger.Debug("Start : Constructor")

        _optionLst = New ObservableCollection(Of OptionModel)
        _optionDB = New OptionDBProvider

        _logger.Debug("End : Constructor")

    End Sub
#End Region

#Region "Properties"
    Public Property OptionList As ObservableCollection(Of OptionModel)
        Get
            Return _optionLst
        End Get
        Set(value As ObservableCollection(Of OptionModel))
            _optionLst = value
            NotifyPropertyChanged("OptionList")
        End Set
    End Property
    Public Property OptionValue As String
        Get
            Return _optionValue
        End Get
        Set(value As String)
            _optionValue = value
            NotifyPropertyChanged("OptionValue")
        End Set
    End Property

    Public WriteOnly Property Service As AideServiceClient
        'Get
        '    Return _optionValue
        'End Get
        Set(value As AideServiceClient)
            aide = value
        End Set
    End Property

#End Region

#Region "Main Methods"
    Public Function GetOptions(ByVal _optionID As Integer, ByVal _moduleID As Integer, ByVal _functionID As Integer) As Boolean

        _logger.Debug("Start : GetOptions")

        Try
            Dim opt As Boolean = False
            'If Me.InitializeService Then
            Dim _option As Options() = AideClient.GetClient().GetOptions(_optionID, _moduleID, _functionID)
            If _option.Length = 0 Then
                opt = False
            End If
            SetOptions(_option)
            opt = True
            'End If
            Return opt
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return False
        End Try

        _logger.Debug("End : GetOptions")

    End Function
    Public Function GetOption(ByVal _optionID As Integer) As String

        _logger.Debug("Start : GetOption")

        Try
            'If Me.InitializeService Then
            Dim _option As Options() = AideClient.GetClient().GetOptions(_optionID, 0, 0)
            If _option.Length = 0 Then
                Return String.Empty
            End If
            SetOption(_option)
            'End If
            Return _optionValue
        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
            Return String.Empty
        End Try

        _logger.Debug("End : GetOption")

    End Function

    Private Sub SetOptions(ByVal _opt As Options())

        _logger.Debug("Start : SetOptions")

        Try
            For Each objOption As Options In _opt
                _optionDB._setlistofitems(objOption)
            Next
            For Each optSet As myOptionSet In _optionDB._getObjOption()
                _optionLst.Add(New OptionModel(optSet))
            Next

        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : SetOptions")

    End Sub
    Private Sub SetOption(ByVal _opt As Options())

        _logger.Debug("Start : SetOption")

        Try
            For Each objOption As Options In _opt
                _optionDB._setlistofitems(objOption)
            Next
            For Each optSet As myOptionSet In _optionDB._getObjOption()
                _optionValue = optSet._value
            Next

        Catch ex As Exception
            _logger.Error(ex.ToString())

            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

        _logger.Debug("End : SetOption")

    End Sub
    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

#Region "Service Function/Methods"

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
#End Region

End Class
