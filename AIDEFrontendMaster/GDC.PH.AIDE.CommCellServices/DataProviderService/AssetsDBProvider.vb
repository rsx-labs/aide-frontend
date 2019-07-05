Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel

Public Class AssetsDBProvider

    Private _assetsList As ObservableCollection(Of MyAssets)
    Private _assetsInventoryList As ObservableCollection(Of MyAssets)
    Private _assetsHistoryList As ObservableCollection(Of MyAssets)
    Private _assetsTypeList As ObservableCollection(Of MyAssets)
    Private _assetsManufacturerList As ObservableCollection(Of MyAssets)
    Private client As AideServiceClient
    Private UNASSIGNED As String = "Unassigned"
    Private ASSIGNED As String = "Assigned"
    Private PARTIALLY_ASSIGNED As String = "Partially Assigned"
    Private PARTIALLY_UNASSIGNED As String = "Partially Unassigned"
    Private Status_Descr As String
    Private Date_Descr As String

    Public Sub New()
        _assetsList = New ObservableCollection(Of MyAssets)
        _assetsInventoryList = New ObservableCollection(Of MyAssets)
        _assetsHistoryList = New ObservableCollection(Of MyAssets)
        _assetsTypeList = New ObservableCollection(Of MyAssets)
        _assetsManufacturerList = New ObservableCollection(Of MyAssets)
    End Sub

    Public Function GetAssetList() As ObservableCollection(Of MyAssets)
        Return _assetsList
    End Function

    Public Function GetAssetInventoryList() As ObservableCollection(Of MyAssets)
        Return _assetsInventoryList
    End Function

    Public Function GetAssetHistoryList() As ObservableCollection(Of MyAssets)
        Return _assetsHistoryList
    End Function

    Public Function GetAssetTypeList() As ObservableCollection(Of MyAssets)
        Return _assetsTypeList
    End Function

    Public Function GetAssetManufacturerList() As ObservableCollection(Of MyAssets)
        Return _assetsManufacturerList
    End Function

    Public Sub SetAssetList(ByVal _assets As Assets)
        If _assets.STATUS = 1 Then
            Status_Descr = UNASSIGNED
        ElseIf _assets.STATUS = 2 Then
            Status_Descr = ASSIGNED
        ElseIf _assets.STATUS = 3 Then
            Status_Descr = PARTIALLY_ASSIGNED
        ElseIf _assets.STATUS = 4 Then
            Status_Descr = PARTIALLY_UNASSIGNED
        End If

        Dim dateDiffYear As Integer = DateDiff(DateInterval.Year, _assets.DATE_PURCHASED, Date.Now)
        Dim dateDiffMonth As Integer = DateDiff(DateInterval.Month, _assets.DATE_PURCHASED, Date.Now)
        Dim dateDiffDay As Integer = DateDiff(DateInterval.Day, _assets.DATE_PURCHASED, Date.Now)
        If dateDiffYear <> 0 Then
            Date_Descr = dateDiffYear & " year/s"
        ElseIf dateDiffMonth <> 0 Then
            Date_Descr = dateDiffMonth & " month/s"
        ElseIf dateDiffDay <> 0 Then
            Date_Descr = dateDiffDay & " day/s"
        End If

        Dim _objectAsset As MyAssets = New MyAssets With {
            .ASSET_ID = _assets.ASSET_ID,
            .ASSET_TAG = _assets.ASSET_TAG,
            .ASSET_DESC = _assets.ASSET_DESC,
            .DATE_PURCHASED = _assets.DATE_PURCHASED,
            .EMP_ID = _assets.EMP_ID,
            .MANUFACTURER = _assets.MANUFACTURER,
            .MODEL_NO = _assets.MODEL_NO,
            .SERIAL_NO = _assets.SERIAL_NO,
            .FULL_NAME = _assets.FULL_NAME,
            .OTHER_INFO = _assets.OTHER_INFO,
            .STATUS = _assets.STATUS,
            .STATUS_DESCR = Status_Descr,
            .DATE_DESCR = Date_Descr
        }
        _assetsList.Add(_objectAsset)
    End Sub

    Public Sub SetAssetInventoryList(ByVal _assets As Assets)

        Dim isApprove As Boolean

        If _assets.STATUS = 1 Then
            Status_Descr = UNASSIGNED
        ElseIf _assets.STATUS = 2 Then
            Status_Descr = ASSIGNED
        ElseIf _assets.STATUS = 3 Then
            Status_Descr = PARTIALLY_ASSIGNED
        ElseIf _assets.STATUS = 4 Then
            Status_Descr = PARTIALLY_UNASSIGNED
        End If

        If _assets.APPROVAL = 1 Then
            isApprove = True
        Else
            isApprove = False
        End If

        Dim dateDiffYear As Integer = DateDiff(DateInterval.Year, _assets.DATE_PURCHASED, Date.Now)
        Dim dateDiffMonth As Integer = DateDiff(DateInterval.Month, _assets.DATE_PURCHASED, Date.Now)
        Dim dateDiffDay As Integer = DateDiff(DateInterval.Day, _assets.DATE_PURCHASED, Date.Now)
        If dateDiffYear <> 0 Then
            Date_Descr = dateDiffYear & " year/s"
        ElseIf dateDiffMonth <> 0 Then
            Date_Descr = dateDiffMonth & " month/s"
        ElseIf dateDiffDay <> 0 Then
            Date_Descr = dateDiffDay & " day/s"
        End If

        Dim _objectAsset As MyAssets = New MyAssets With {
            .ASSET_ID = _assets.ASSET_ID,
            .ASSET_TAG = _assets.ASSET_TAG,
            .ASSET_DESC = _assets.ASSET_DESC,
            .DATE_ASSIGNED = _assets.DATE_ASSIGNED,
            .EMP_ID = _assets.EMP_ID,
            .MANUFACTURER = _assets.MANUFACTURER,
            .MODEL_NO = _assets.MODEL_NO,
            .SERIAL_NO = _assets.SERIAL_NO,
            .COMMENTS = _assets.COMMENTS,
            .STATUS = _assets.STATUS,
            .FULL_NAME = _assets.FULL_NAME,
            .DEPARTMENT = _assets.DEPARTMENT,
            .STATUS_DESCR = Status_Descr,
            .APPROVAL = _assets.APPROVAL,
            .DATE_DESCR = Date_Descr,
            .ISAPPROVED = isApprove
        }
        _assetsInventoryList.Add(_objectAsset)
    End Sub


    Public Sub SetAssetHistoryList(ByVal _assets As Assets)
        Dim _objectAsset As MyAssets = New MyAssets With {
            .ASSET_ID = _assets.ASSET_ID,
            .ASSET_TAG = _assets.ASSET_TAG,
            .ASSET_DESC = _assets.ASSET_DESC,
            .DATE_ASSIGNED = _assets.DATE_ASSIGNED,
            .EMP_ID = _assets.EMP_ID,
            .MANUFACTURER = _assets.MANUFACTURER,
            .MODEL_NO = _assets.MODEL_NO,
            .SERIAL_NO = _assets.SERIAL_NO,
            .COMMENTS = _assets.COMMENTS,
            .STATUS = _assets.STATUS,
            .FULL_NAME = _assets.FULL_NAME,
            .DEPARTMENT = _assets.DEPARTMENT,
            .STATUS_DESCR = _assets.STATUS_DESCR,
            .APPROVAL = _assets.APPROVAL,
            .TABLE_NAME = _assets.TABLE_NAME
        }
        _assetsHistoryList.Add(_objectAsset)
    End Sub

    Public Sub SetAssetTypeList(ByVal _assets As Assets)

        Dim _objectAsset As MyAssets = New MyAssets With {
            .ASSET_ID = _assets.ASSET_ID,
            .ASSET_DESC = _assets.ASSET_DESC
        }
        _assetsTypeList.Add(_objectAsset)
    End Sub

    Public Sub SetAssetManufacturerList(ByVal _assets As Assets)

        Dim _objectAsset As MyAssets = New MyAssets With {
            .ASSET_ID = _assets.ASSET_ID,
            .MANUFACTURER = _assets.MANUFACTURER
        }
        _assetsManufacturerList.Add(_objectAsset)
    End Sub
End Class

Public Class MyAssets
    Public Property ASSET_ID As Integer
    Public Property EMP_ID As Integer
    Public Property ASSET_DESC As String
    Public Property MANUFACTURER As String
    Public Property MODEL_NO As String
    Public Property SERIAL_NO As String
    Public Property ASSET_TAG As String
    Public Property DATE_PURCHASED As DateTime
    Public Property STATUS As Integer
    Public Property OTHER_INFO As String
    Public Property DATE_ASSIGNED As DateTime
    Public Property COMMENTS As String
    Public Property FULL_NAME As String
    Public Property DEPARTMENT As String
    Public Property STATUS_DESCR As String
    Public Property DATE_DESCR As String
    Public Property ASSIGNED_TO As Integer
    Public Property APPROVAL As Integer
    Public Property ISAPPROVED As Boolean
    Public Property TABLE_NAME As String
End Class
