Imports System.ComponentModel
Public Class ProblemModel
    Private _employeeID As Integer
    Private _employeeName As String
    Private _problemID As Integer
    Private _problemDescr As String
    Private _problemInvolve As String
    Private _problemInvolveCount As Integer
    Private _causeID As Integer
    Private _causeDescr As String
    Private _causeWhy As String
    Private _optionID As Integer
    Private _optionDescr As String
    Private _optionNumber As Integer = 0
    Private _solutionID As Integer
    Private _solutionDescr As String
    Private _implementID As Integer
    Private _implementDescr As String
    Private _implementAssignee As Integer
    Private _implementValue As String
    Private _resultID As Integer
    Private _resultDescr As String
    Private _resultValue As String

    Public Sub New()
    End Sub
#Region "Problem"
    Public Sub New(ByVal probObj As myProblem)
        _employeeID = probObj.EmployeeID
        _employeeName = probObj.EmployeeName
        _problemID = probObj.ProblemID
        _problemDescr = probObj.ProblemDescr
        _problemInvolve = probObj.ProblemInvolve
    End Sub
#End Region
#Region "Problem Cause"
    Public Sub New(ByVal probObj As myProblemCause)
        _causeID = probObj.CauseID
        _problemID = probObj.ProblemID
        _causeDescr = probObj.CauseDescr
        _causeWhy = probObj.CauseWhy
    End Sub
#End Region
#Region "Problem Option"
    Public Sub New(ByVal probObj As myProblemOption, ByVal count As Integer)
        _optionID = probObj.OptionID
        _problemID = probObj.ProblemID
        _optionDescr = probObj.OptionDescr
        _optionNumber = count
    End Sub
#End Region
#Region "Problem Solution"
    Public Sub New(ByVal probObj As myProblemSolution)
        _solutionID = probObj.SolutionID
        _problemID = probObj.ProblemID
        _optionID = probObj.OptionID
        _solutionDescr = probObj.SolutionDescr
    End Sub
#End Region
#Region "Problem Implement"
    Public Sub New(ByVal probObj As myProblemImplement)
        _implementID = probObj.ImplementID
        _problemID = probObj.ProblemID
        _optionID = probObj.OptionID
        _implementDescr = probObj.ImplementDescr
        _implementAssignee = probObj.ImplementAssignee
        _implementValue = probObj.ImplementValue
    End Sub
#End Region
#Region "Problem Result"
    Public Sub New(ByVal probObj As myProblemResult)
        _resultID = probObj.ResultID
        _problemID = probObj.ProblemID
        _optionID = probObj.OptionID
        _resultDescr = probObj.ResultDescr
        _resultValue = probObj.ResultValue
    End Sub
#End Region
    Public Property EMPLOYEE_ID As Integer
        Get
            Return Me._employeeID
        End Get
        Set(value As Integer)
            Me._employeeID = value
            OnPropertyChanged("EMPLOYEE_ID")
        End Set
    End Property
    Public Property EMPLOYEE_NAME As String
        Get
            Return Me._employeeName
        End Get
        Set(value As String)
            Me._employeeName = value
            OnPropertyChanged("EMPLOYEE_NAME")
        End Set
    End Property
    Public Property PROBLEM_ID As Integer
        Get
            Return Me._problemID
        End Get
        Set(value As Integer)
            Me._problemID = value
            OnPropertyChanged("PROBLEM_ID")
        End Set
    End Property
    Public Property PROBLEM_DESCR As String
        Get
            Return Me._problemDescr
        End Get
        Set(value As String)
            Me._problemDescr = value
            OnPropertyChanged("PROBLEM_DESCR")
        End Set
    End Property
    Public Property PROBLEM_INVOLVE As String
        Get
            Return Me._problemInvolve
        End Get
        Set(value As String)
            Me._problemInvolve = value
            OnPropertyChanged("PROBLEM_INVOLVE")
        End Set
    End Property
    Public ReadOnly Property PROBLEM_INVOLVE_COUNT As Integer
        Get
            Return _problemInvolve.Split(",").Count().ToString()
        End Get
    End Property
    Public Property CAUSE_ID As Integer
        Get
            Return Me._causeID
        End Get
        Set(value As Integer)
            Me._causeID = value
            OnPropertyChanged("CAUSE_ID")
        End Set
    End Property
    Public Property CAUSE_DESCR As String
        Get
            Return Me._causeDescr
        End Get
        Set(value As String)
            Me._causeDescr = value
            OnPropertyChanged("CAUSE_DESCR")
        End Set
    End Property
    Public Property CAUSE_WHY As String
        Get
            Return Me._causeWhy
        End Get
        Set(value As String)
            Me._causeWhy = value
            OnPropertyChanged("CAUSE_WHY")
        End Set
    End Property
    Public Property OPTION_ID As Integer
        Get
            Return Me._optionID
        End Get
        Set(value As Integer)
            Me._optionID = value
            OnPropertyChanged("OPTION_ID")
        End Set
    End Property
    Public Property OPTION_DESCR As String
        Get
            Return Me._optionDescr
        End Get
        Set(value As String)
            Me._optionDescr = value
            OnPropertyChanged("OPTION_DESCR")
        End Set
    End Property
    Public Property OPTION_NUMBER As Integer
        Get
            Return Me._optionNumber
        End Get
        Set(value As Integer)
            Me._optionNumber = value
            OnPropertyChanged("OPTION_NUMBER")
        End Set
    End Property
    Public Property SOLUTION_ID As Integer
        Get
            Return Me._solutionID
        End Get
        Set(value As Integer)
            Me._solutionID = value
            OnPropertyChanged("SOLUTION_ID")
        End Set
    End Property
    Public Property SOLUTION_DESCR As String
        Get
            Return Me._solutionDescr
        End Get
        Set(value As String)
            Me._solutionDescr = value
            OnPropertyChanged("SOLUTION_DESCR")
        End Set
    End Property
    Public Property IMPLEMENT_ID As Integer
        Get
            Return Me._implementID
        End Get
        Set(value As Integer)
            Me._implementID = value
            OnPropertyChanged("IMPLEMENT_ID")
        End Set
    End Property
    Public Property IMPLEMENT_DESCR As String
        Get
            Return Me._implementDescr
        End Get
        Set(value As String)
            Me._implementDescr = value
            OnPropertyChanged("IMPLEMENT_DESCR")
        End Set
    End Property
    Public Property IMPLEMENT_ASSIGNEE As Integer
        Get
            Return Me._implementAssignee
        End Get
        Set(value As Integer)
            Me._implementAssignee = value
            OnPropertyChanged("IMPLEMENT_ASSIGNEE")
        End Set
    End Property
    Public Property IMPLEMENT_VALUE As String
        Get
            Return Me._implementValue
        End Get
        Set(value As String)
            Me._implementValue = value
            OnPropertyChanged("IMPLEMENT_VALUE")
        End Set
    End Property
    Public Property RESULT_ID As Integer
        Get
            Return Me._resultID
        End Get
        Set(value As Integer)
            Me._resultID = value
            OnPropertyChanged("RESULT_ID")
        End Set
    End Property
    Public Property RESULT_DESCR As String
        Get
            Return Me._resultDescr
        End Get
        Set(value As String)
            Me._resultDescr = value
            OnPropertyChanged("RESULT_DESCR")
        End Set
    End Property
    Public Property RESULT_VALUE As String
        Get
            Return Me._resultValue
        End Get
        Set(value As String)
            Me._resultValue = value
            OnPropertyChanged("RESULT_VALUE")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
