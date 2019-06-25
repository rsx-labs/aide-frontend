Imports System.ComponentModel
Imports System.Collections.ObjectModel
Class QuarterlyAuditPage

    Private lstQuestions() As String = {"1. Has there been quarterly review with customer?", "2. Has the quarterly SI, team building or any team bonding activity been conducted?", "3. Has the problem solving been implemented and reviewed?", "4. Has the monthly audit been completed?"}
    Private lstPeople() As String = {"Jiji", "Karen / Jeanette", "Cha / Jiji", "Cha / Jiji"}
    Private lstDates() As String = {"Apr - Jun", "Jul - Sep", "Oct - Dec", "Jan - Mar"}
    Private lstEmployee() As String = {"Hyacinth", "Richard", "Jhunell", "Zed"}
    Private lstQuarter() As String = {"Q1", "Q2", "Q3", "Q4"}
    Private lstNotes() As String = {"Success! no discrepancies encounter on this quarter.", "Success! no discrepancies encounter on this quarter.", "", ""}
    Private dailyVMM As New QuarterlyVM

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        generatedata()
        generateQuestions()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub generateQuestions()
        Dim questModel As New QuestionsMonthlyModel
        Dim x As Integer = 0
        For Each quest As String In lstQuestions

            dailyVMM.QuestionQuarterlyList.Add(New QuestionsQuarterlyModel(quest, lstPeople(x)))
            x += 1

        Next
        QuarterLVQuestions.ItemsSource = dailyVMM.QuestionQuarterlyList
        DataContext = dailyVMM.QuestionQuarterlyList
    End Sub

    Private Sub generatedata()

        'Dim dailly As New dailyMod
        'dailly.Days = "9"
        'dailly.Dates = "5/27 - 5/31"
        'dailyVMM.Daily.Add(dailly)



        'Dim questModel As New QuestionsModel
        Dim x As Integer = 9
        Dim y As Integer = 0
        For Each quest As String In lstDates

            dailyVMM.Quarterly.Add(New QuarterlyMod(lstQuarter(y), quest, lstEmployee(y), lstNotes(y)))
            x += 1
            y += 1
        Next
        QuarterLV.ItemsSource = dailyVMM.Quarterly
        DataContext = dailyVMM.Quarterly
    End Sub

End Class

Public Class QuarterlyVM
    Private _dailyobjlst As ObservableCollection(Of QuarterlyMod)
    Private QtnLst As ObservableCollection(Of QuestionsQuarterlyModel)



    Public Sub New()
        _dailyobjlst = New ObservableCollection(Of QuarterlyMod)
        QtnLst = New ObservableCollection(Of QuestionsQuarterlyModel)
    End Sub

    Public Property Quarterly As ObservableCollection(Of QuarterlyMod)
        Get
            Return _dailyobjlst
        End Get
        Set(value As ObservableCollection(Of QuarterlyMod))
            _dailyobjlst = value
        End Set
    End Property
    Public Property QuestionQuarterlyList As ObservableCollection(Of QuestionsQuarterlyModel)
        Get
            Return QtnLst
        End Get
        Set(value As ObservableCollection(Of QuestionsQuarterlyModel))
            QtnLst = value
        End Set
    End Property
End Class

Public Class QuarterlyMod
    Private _quarters As String
    Private _dates As String
    Private _empName As String
    Private _notes As String


    Public Sub New()

    End Sub

    Public Sub New(quarterss As String, datess As String, emp As String, note As String)
        _quarters = quarterss
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If

    End Sub
    Public Property Quarter As String
        Get
            Return _quarters
        End Get
        Set(value As String)
            _quarters = value
        End Set
    End Property
    Public Property Dates As String
        Get
            Return _dates
        End Get
        Set(value As String)
            _dates = value
        End Set
    End Property
    Public Property EmpName As String
        Get
            Return _empName
        End Get
        Set(value As String)
            _empName = value
        End Set
    End Property
    Public Property Notes As String
        Get
            Return "Notes : " + _notes
        End Get
        Set(value As String)
            _notes = value
        End Set
    End Property
End Class
Public Class QuestionsQuarterlyModel
    Private Qtn As String
    Private Ppl As String
    Public Sub New()

    End Sub
    Public Sub New(st As String, pl As String)
        Qtn = st
        Ppl = pl
    End Sub
    Public Property Questions As String
        Get
            Return Qtn
        End Get
        Set(value As String)
            Qtn = value
        End Set
    End Property

    Public Property People As String
        Get
            Return Ppl
        End Get
        Set(value As String)
            Ppl = value
        End Set
    End Property
End Class