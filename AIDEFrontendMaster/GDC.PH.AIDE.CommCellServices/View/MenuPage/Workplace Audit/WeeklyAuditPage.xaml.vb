Imports System.ComponentModel
Imports System.Collections.ObjectModel
Class WeeklyAuditPage

    Private lstQuestions() As String = {"1. Has the OTL been logged by the team?", "2. Has the SAP been logged by the team?", "3. Has the SAP and OTL report been generated and without discrepancies?",
                                        "4. Have the SAP and OTL discrepancies been corrected?", "5. Has the status reports been submitted by the team?", "6. Are assigned iLearn courses on track for completion, if any?",
                                        "7. Is the Comm. Cell board up to date?" + vbCrLf + "- Evidence that team is having weekly Comm. Cells" + vbCrLf + "- Pending actions of Comm. Cells and problem solving been actioned" + vbCrLf +
                                        "Success been registered in the Comm. Cell Board" + vbCrLf + "- Lessons learned been registered in the Comm. Cell Board", "8. Have all non-conformances from the audit been raised as a concern on the Comm. Cell 3C document?",
                                        "9. Has the daily audit of the previous week been completed?", "10. Has the monthly audit been completed?"}
    Private lstPeople() As String = {"Gigi", "Cha / Jeanette", "Jiji", "Giann / Jiji", "Franz Jeanette", "Cha Jeanette", "Franz QA - host of the week", "Daily Auditor", "Daily Auditor", "Monthly Auditor"}
    Private lstDates() As String = {"5/27 - 5/31", "6/3 - 6/7", "6/10 - 6/14", "6/17 - 6/21", "6/24 - 6/28"}
    Private lstEmployee() As String = {"Nina", "Celso", "Grace", "Boni", "Riz"}
    Private lstNotes() As String = {"Giann and Jester has discrepancies in SAP.", "", "", "", "Giann, Zed, Shei and Rose have discrepancies in SAP and OTL."}
    Private dailyVMM As New dailyVM

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        generatedata()
        generateQuestions()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub generateQuestions()
        Dim questModel As New QuestionsModel
        Dim x As Integer = 0
        For Each quest As String In lstQuestions

            dailyVMM.QuestionList.Add(New QuestionsModel(quest, lstPeople(x)))
            x += 1

        Next
        QuarterLVQuestions.ItemsSource = dailyVMM.QuestionList
        DataContext = dailyVMM.QuestionList
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

            dailyVMM.Daily.Add(New dailyMod(x.ToString(), quest, lstEmployee(y), lstNotes(y)))
            x += 1
            y += 1
        Next
        QuarterLV.ItemsSource = dailyVMM.Daily
        DataContext = dailyVMM.Daily
    End Sub

End Class

Public Class dailyVM
    Private _dailyobjlst As ObservableCollection(Of dailyMod)
    Private QtnLst As ObservableCollection(Of QuestionsModel)



    Public Sub New()
        _dailyobjlst = New ObservableCollection(Of dailyMod)
        QtnLst = New ObservableCollection(Of QuestionsModel)
    End Sub

    Public Property Daily As ObservableCollection(Of dailyMod)
        Get
            Return _dailyobjlst
        End Get
        Set(value As ObservableCollection(Of dailyMod))
            _dailyobjlst = value
        End Set
    End Property
    Public Property QuestionList As ObservableCollection(Of QuestionsModel)
        Get
            Return QtnLst
        End Get
        Set(value As ObservableCollection(Of QuestionsModel))
            QtnLst = value
        End Set
    End Property
End Class

Public Class dailyMod
    Private _days As String
    Private _dates As String
    Private _empName As String
    Private _notes As String


    Public Sub New()

    End Sub

    Public Sub New(dayss As String, datess As String, emp As String, note As String)
        _days = dayss
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If

    End Sub
    Public Property Days As String
        Get
            Return _days
        End Get
        Set(value As String)
            _days = value
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
Public Class QuestionsModel
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

Public Class QuestionsVM
    
End Class
