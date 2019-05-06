Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Collections.ObjectModel




''' <summary>
''' AUTHOR : GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class ConcernDBProvider

    Private _concernList As ObservableCollection(Of MyConcern)
    Private _setConcernText As New MyConcern
    Private _MyConcernRefNo As New MyGeneratedRefNo

    Private _concernActionlist As ObservableCollection(Of MyConcern)
    Private _concernActionReferenceList As ObservableCollection(Of MyActionReference)

    Public Sub New()

        _concernList = New ObservableCollection(Of MyConcern)
        _MyConcernRefNo = New MyGeneratedRefNo
        _concernActionlist = New ObservableCollection(Of MyConcern)
        _concernActionReferenceList = New ObservableCollection(Of MyActionReference)

    End Sub


    Public Function GetSelectedConcern() As MyConcern
        Return _setConcernText
    End Function

    ''DATAGRID
    Public Function GetConcernActionList() As ObservableCollection(Of MyConcern)
        Return _concernActionlist
    End Function

    ''COMBO BOX
    Public Function GetConcernList() As ObservableCollection(Of MyConcern)
        Return _concernList

    End Function


    Public Function GetRefNo() As MyGeneratedRefNo
        Return _MyConcernRefNo
    End Function

    ''LISTVIEW THE LIST OF ACT REFERENCE in every Concern
    Public Function GetConcernActionReferenceList() As ObservableCollection(Of MyActionReference)
        Return _concernActionReferenceList
    End Function



    ''BINDING TO LISTVIEW THE LIST OF ACTION TO CHOOSE TO ADD IN CONCERN
    Public Sub SetToComBoBox(ByVal _concern As Concern)
        Dim _concernObj As MyConcern = New MyConcern() With {
            .ActMessage = _concern.ACT_MESSAGE,
            .ActRef = _concern.ACTREF,
            .Due_Date = _concern.Due_Date
           }
        _concernActionlist.Add(_concernObj)
    End Sub




    ''BINDING TO LISTVIEW THE ACTION REFERENCE
    Public Sub SetTollistViewActionReference(ByVal _concern As Concern)
        Dim _concernObj As MyActionReference = New MyActionReference() With {
            .Action_References = _concern.ACTION_REFERENCES,
        .ACTREF = _concern.ACTREF
           }
        _concernActionReferenceList.Add(_concernObj)
    End Sub






    ''BINDING TO DATAGRIDVIEW
    Public Sub SetConcernList(ByVal _concern As Concern)
        Dim _concernObj As MyConcern = New MyConcern() With {
            .RefID = _concern.RefID,
            .Concern = _concern.Concerns,
            .Cause = _concern.Cause,
            .CounterMeasure = _concern.CounterMeasure,
            .Act_Reference = _concern.Act_Reference,
            .Status = _concern.Status,
            .EmpID = _concern.EmpID,
            .Due_Date = _concern.Due_Date
           }
        _concernList.Add(_concernObj)
    End Sub



    ''BINDING TO TEXT UPDATE
    Public Sub SetConcernText(ByVal _concern As Concern)
        _setConcernText.RefID = _concern.RefID
        _setConcernText.Concern = _concern.Concerns
        _setConcernText.Cause = _concern.Cause
        _setConcernText.Due_Date = _concern.Due_Date
        _setConcernText.CounterMeasure = _concern.CounterMeasure
        _setConcernText.Act_Reference = _concern.Act_Reference

    End Sub

    ''BINDING TO GENERATEDREF NO TEXT


    Public Sub SetToMyRefNo(ByVal _concern As Concern)
        _MyConcernRefNo.GENERATEDREF_ID = _concern.GENERATEDREF_ID

    End Sub


End Class

''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class MyConcern
    Property RefID As String
    Property Concern As String
    Property Cause As String
    Property CounterMeasure As String
    Property EmpID As String
    Property Act_Reference As String
    Property Status As String
    Property Due_Date As DateTime

    Property ActRef As String
    Property ActMessage As String
End Class

''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class MyGeneratedRefNo
    Property GENERATEDREF_ID As String
End Class


''' <summary>
''' GIANN CARLO CAMILO AND CHRISTIAN VALONDO
''' </summary>
''' <remarks></remarks>
Public Class MyActionReference
    Property Action_References As String
    Property ACTREF As String
End Class