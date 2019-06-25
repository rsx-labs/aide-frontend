Imports System.ComponentModel
Imports System.Collections.ObjectModel
Class MonthlyAuditPage

    Private lstQuestions() As String = {"1. Has the Monthly KPI report been submitted?", "2. Has there been monthly staff meeting?", "3. Has the weekly audit been completed?"}
    Private lstPeople() As String = {"Gigi", "Cha / Jeanette", "Weekly Auditor"}
    Private lstDates() As String = {"2019", "2019", "2019", "2019", "2019", "2019"}
    Private lstEmployee() As String = {"Christian", "Harvey", "Ciara", "Jester", "Erell", "Giann"}
    Private lstMonths() As String = {"APR", "MAY", "JUN", "JUL", "AUG", "SEP"}
    Private lstNotes() As String = {"Get everything you need to build and deploy your app on any platform. With state-of-the-art tools, the power of the cloud, training, and support, it's our most comprehensive free developer program ever. ", "", "", "", "", "Success! no discrepancies encounter on this month."}
    Private dailyVMM As New monthlyVM

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

            dailyVMM.QuestionMonthlyList.Add(New QuestionsMonthlyModel(quest, lstPeople(x)))
            x += 1

        Next
        QuarterLVQuestions.ItemsSource = dailyVMM.QuestionMonthlyList
        DataContext = dailyVMM.QuestionMonthlyList
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

            dailyVMM.Monthly.Add(New MonthlyMod(lstMonths(y), quest, lstEmployee(y), lstNotes(y)))
            x += 1
            y += 1
        Next
        QuarterLV.ItemsSource = dailyVMM.Monthly
        DataContext = dailyVMM.Monthly
    End Sub

End Class

Public Class monthlyVM
    Private _dailyobjlst As ObservableCollection(Of MonthlyMod)
    Private QtnLst As ObservableCollection(Of QuestionsMonthlyModel)



    Public Sub New()
        _dailyobjlst = New ObservableCollection(Of MonthlyMod)
        QtnLst = New ObservableCollection(Of QuestionsMonthlyModel)
    End Sub

    Public Property Monthly As ObservableCollection(Of MonthlyMod)
        Get
            Return _dailyobjlst
        End Get
        Set(value As ObservableCollection(Of MonthlyMod))
            _dailyobjlst = value
        End Set
    End Property
    Public Property QuestionMonthlyList As ObservableCollection(Of QuestionsMonthlyModel)
        Get
            Return QtnLst
        End Get
        Set(value As ObservableCollection(Of QuestionsMonthlyModel))
            QtnLst = value
        End Set
    End Property
End Class

Public Class MonthlyMod
    Private _months As String
    Private _dates As String
    Private _empName As String
    Private _notes As String


    Public Sub New()

    End Sub

    Public Sub New(monthss As String, datess As String, emp As String, note As String)
        _months = monthss
        _dates = datess
        _empName = emp
        If note = String.Empty Then
            _notes = "Nothing"
        Else
            _notes = note
        End If

    End Sub
    Public Property Months As String
        Get
            Return _months
        End Get
        Set(value As String)
            _months = value
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
Public Class QuestionsMonthlyModel
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
