Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.ServiceModel
Imports GDC.PH.AIDE.ServiceFactory
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class AideClient
    Implements INotifyPropertyChanged

    Private Shared _context As InstanceContext
    Private Shared _instance As AideClient
    Private Shared _aideClientService As AideServiceClient
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    ''Public Shared Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    'Private Event INotifyPropertyChanged_PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub New()
        CreateClient()
    End Sub

    Public Shared Sub CreateInstance(ByVal context As InstanceContext)
        If _instance Is Nothing Then
            _context = context
            _instance = New AideClient()
        End If
    End Sub

    Public Shared Function GetClient() As AideServiceClient
        If _aideClientService.State <> CommunicationState.Opened Then
            CreateClient()
        End If
        Return _aideClientService
    End Function


    Private Shared Sub CreateClient()
        Try
            Dim factory As ServiceClientFactory = ServiceClientFactory.GetFactory()
            _aideClientService = factory.GetClient(Of IAideService)(_context)
            _aideClientService.Open()
        Catch ex As Exception
            _aideClientService = New AideServiceClient(_context)
            _aideClientService.Open()
        End Try

    End Sub
End Class
