Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class ProblemDBProvider
    Private _myProblemLst As ObservableCollection(Of myProblem)
    Private _myProblemCauseLst As ObservableCollection(Of myProblemCause)
    Private _myProblemOptionLst As ObservableCollection(Of myProblemOption)
    Private _myProblemSolutionLst As ObservableCollection(Of myProblemSolution)
    Private _myProblemImplementLst As ObservableCollection(Of myProblemImplement)
    Private _myProblemResultLst As ObservableCollection(Of myProblemResult)

    Sub New()
        _myProblemLst = New ObservableCollection(Of myProblem)
        _myProblemCauseLst = New ObservableCollection(Of myProblemCause)
        _myProblemOptionLst = New ObservableCollection(Of myProblemOption)
        _myProblemSolutionLst = New ObservableCollection(Of myProblemSolution)
        _myProblemImplementLst = New ObservableCollection(Of myProblemImplement)
        _myProblemResultLst = New ObservableCollection(Of myProblemResult)
    End Sub

#Region "Problem"
    Public Sub SetProblemList(ByVal _objProblem As Problem)
        Dim _problemObj As myProblem = New myProblem With {.EmployeeID = _objProblem.EmployeeID,
                                                           .EmployeeName = _objProblem.EmployeeName,
                                                           .ProblemID = _objProblem.ProblemID,
                                                           .ProblemDescr = _objProblem.ProblemDescr,
                                                           .ProblemInvolve = _objProblem.ProblemInvolve}
        _myProblemLst.Add(_problemObj)
    End Sub
    Public Function GetProblemList() As ObservableCollection(Of myProblem)
        Return _myProblemLst
    End Function
#End Region

#Region "Problem Cause"
    Public Sub SetProblemCauseList(ByVal _objProblem As Problem)
        Dim _problemObj As myProblemCause = New myProblemCause With {.CauseID = _objProblem.CauseID,
                                                           .ProblemID = _objProblem.ProblemID,
                                                           .CauseDescr = _objProblem.CauseDescr,
                                                           .CauseWhy = _objProblem.CauseWhy}
        _myProblemCauseLst.Add(_problemObj)
    End Sub
    Public Function GetProblemCauseList() As ObservableCollection(Of myProblemCause)
        Return _myProblemCauseLst
    End Function
#End Region

#Region "Problem Option"
    Public Sub SetProblemOptionList(ByVal _objProblem As Problem)
        Dim _problemObj As myProblemOption = New myProblemOption With {.OptionID = _objProblem.OptionID,
                                                           .ProblemID = _objProblem.ProblemID,
                                                           .OptionDescr = _objProblem.OptionDescr}
        _myProblemOptionLst.Add(_problemObj)
    End Sub
    Public Function GetProblemOptionList() As ObservableCollection(Of myProblemOption)
        Return _myProblemOptionLst
    End Function
#End Region

#Region "Problem Solution"
    Public Sub SetProblemSolutionList(ByVal _objProblem As Problem)
        Dim _problemObj As myProblemSolution = New myProblemSolution With {.SolutionID = _objProblem.SolutionID,
                                                           .OptionID = _objProblem.OptionID,
                                                           .ProblemID = _objProblem.ProblemID,
                                                           .SolutionDescr = _objProblem.SolutionDescr}
        _myProblemSolutionLst.Add(_problemObj)
    End Sub
    Public Function GetProblemSolutionList() As ObservableCollection(Of myProblemSolution)
        Return _myProblemSolutionLst
    End Function
#End Region

#Region "Problem Implement"
    Public Sub SetProblemImplementList(ByVal _objProblem As Problem)
        Dim _problemObj As myProblemImplement = New myProblemImplement With {.ImplementID = _objProblem.ImplementID,
                                                           .OptionID = _objProblem.OptionID,
                                                           .ProblemID = _objProblem.ProblemID,
                                                           .ImplementDescr = _objProblem.ImplementDescr,
                                                           .ImplementAssignee = _objProblem.ImplementAssignee,
                                                           .ImplementValue = _objProblem.ImplementValue}
        _myProblemImplementLst.Add(_problemObj)
    End Sub
    Public Function GetProblemImplementList() As ObservableCollection(Of myProblemImplement)
        Return _myProblemImplementLst
    End Function
#End Region

#Region "Problem Result"
    Public Sub SetProblemResultList(ByVal _objProblem As Problem)
        Dim _problemObj As myProblemResult = New myProblemResult With {.ResultID = _objProblem.ResultID,
                                                           .OptionID = _objProblem.OptionID,
                                                           .ProblemID = _objProblem.ProblemID,
                                                           .ResultDescr = _objProblem.ResultDescr,
                                                           .ResultValue = _objProblem.ResultValue}
        _myProblemResultLst.Add(_problemObj)
    End Sub
    Public Function GetProblemResultList() As ObservableCollection(Of myProblemResult)
        Return _myProblemResultLst
    End Function
#End Region
End Class

Public Class myProblem
    Public Property EmployeeID As Integer
    Public Property EmployeeName As String
    Public Property ProblemID As Integer
    Public Property ProblemDescr As String
    Public Property ProblemInvolve As String
End Class
Public Class myProblemCause
    Public Property ProblemID As Integer
    Public Property CauseID As Integer
    Public Property CauseDescr As String
    Public Property CauseWhy As String
End Class
Public Class myProblemOption
    Public Property ProblemID As Integer
    Public Property OptionID As Integer
    Public Property OptionDescr As String
End Class
Public Class myProblemSolution
    Public Property ProblemID As Integer
    Public Property OptionID As Integer
    Public Property SolutionID As Integer
    Public Property SolutionDescr As String
End Class
Public Class myProblemImplement
    Public Property ProblemID As Integer
    Public Property OptionID As Integer
    Public Property ImplementID As Integer
    Public Property ImplementDescr As String
    Public Property ImplementAssignee As Integer
    Public Property ImplementValue As String
End Class
Public Class myProblemResult
    Public Property ProblemID As Integer
    Public Property OptionID As Integer
    Public Property ResultID As Integer
    Public Property ResultDescr As String
    Public Property ResultValue As String
End Class