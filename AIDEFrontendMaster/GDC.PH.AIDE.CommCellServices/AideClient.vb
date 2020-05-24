Imports System.ServiceModel
Imports GDC.PH.AIDE.ServiceFactory
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class AideClient
    Private Shared _context As InstanceContext
    Private Shared _instance As AideClient
    Private Shared _aideClientService As AideServiceClient
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
        Return _aideClientService
    End Function

    Private Shared Sub CreateClient()
        Dim factory As ServiceClientFactory = ServiceClientFactory.GetFactory()
        _aideClientService = factory.GetClient(Of IAideService)(_context)
    End Sub
End Class
