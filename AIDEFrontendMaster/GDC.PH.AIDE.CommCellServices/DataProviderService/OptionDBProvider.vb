Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class OptionDBProvider
    Private _objOption As ObservableCollection(Of myOptionSet)

    Public Sub New()
        _objOption = New ObservableCollection(Of myOptionSet)
    End Sub
    Public Function _getObjOption() As ObservableCollection(Of myOptionSet)
        Return _objOption
    End Function

    Public Function _setlistofitems(ByRef opt As Options)
        Dim _myOptionSet As New myOptionSet With {._optionID = opt.OptionID,
                                                  ._moduleID = opt.ModuleID,
                                                  ._functionID = opt.FunctionID,
                                                  ._description = opt.Description,
                                                  ._value = opt.Value,
                                                  ._moduleDescr = opt.ModuleDescr,
                                                  ._functionDescr = opt.FunctionDescr}

        _objOption.Add(_myOptionSet)
        Return _myOptionSet
    End Function
End Class
Public Class myOptionSet
    Property _optionID As Integer
    Property _moduleID As Integer
    Property _functionID As Integer
    Property _description As String
    Property _value As String
    Property _moduleDescr As String
    Property _functionDescr As String
End Class
