Imports System.ComponentModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Imports NLog

Public NotInheritable Class CommonUtility
    Implements INotifyPropertyChanged

    Private _logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New()

    End Sub

    Public Sub LoadFiscalYears()
        Try
            Me.FiscalYears = AideClient.GetClient().GetAllFiscalYear()
        Catch ex As Exception
            _logger.Error("Error loading FiscalYears")
            Throw ex
        End Try
    End Sub

    Public Sub LoadMyProfile(ByVal empID As Integer)
        Try
            MyProfile = AideClient.GetClient().GetProfileInformation(empID)
        Catch ex As Exception
            _logger.Error("Error loading MyProfile")
            Throw ex
        End Try

    End Sub

    Public Sub LoadBirthdayToday(ByVal email As String)
        Try
            BirthdayToday = AideClient.GetClient().ViewBirthdayListByCurrentDay(email)
        Catch ex As Exception
            _logger.Error("Error loading BirthdayToday")
            Throw ex
        End Try
    End Sub

    Public Sub LoadBirthdayForTheMonth(ByVal email As String)
        Try
            Me.BirthdayMonth = AideClient.GetClient().ViewBirthdayListByCurrentMonth(email)
        Catch ex As Exception
            _logger.Error("Error loading BirthdayMonth")
            Throw ex
        End Try
    End Sub


    Public Sub LoadBirthdayAll(ByVal email As String)
        Try
            Me.BirthdayAll = AideClient.GetClient().ViewBirthdayListAll(email)
        Catch ex As Exception
            _logger.Error("Error loading BirthdayAll")
            Throw ex
        End Try
    End Sub

    Public Sub LoadKPITargets(ByVal empID As Integer, ByVal fiscalYear As Date)
        Try
            KPITargetList = AideClient.GetClient().GetAllKPITargets(empID, fiscalYear)
        Catch ex As Exception
            _logger.Error("Error loading KPITargetList")
            Throw ex
        End Try

    End Sub

    Public Sub LoadKPISummary(ByVal empID As Integer)
        Try
            Dim lstKpiSummary As Dictionary(Of Integer, KPISummary()) = New Dictionary(Of Integer, KPISummary())

            Dim i As Integer = 0
            For Each fyItem As FiscalYear In Me.FiscalYears
                Dim lstYear As String() = fyItem.FISCAL_YEAR.Split("-")
                Dim dtStart As Date = Convert.ToDateTime(lstYear(0).Trim() & "-04-01")
                Dim dtEnd As Date = Convert.ToDateTime(lstYear(1).Trim() & "-03-31")
                Dim kpiSummary As KPISummary() = AideClient.GetClient().GetKPISummaryList(empID, dtStart, dtEnd)
                If Not IsNothing(kpiSummary) Then
                    lstKpiSummary.Add(i, kpiSummary)
                    i += 1
                End If
            Next
            Me.KPISummaryList = lstKpiSummary
        Catch ex As Exception
            _logger.Error("Error loading KPISummaryList")
            Throw ex
        End Try

    End Sub

    Public Sub LoadAnnouncements(ByVal empID As Integer)
        Try
            Me.Announcements = AideClient.GetClient().GetAnnouncements(empID)
        Catch ex As Exception
            _logger.Error("Error loading Announcements")
            Throw ex
        End Try

    End Sub


    Public Sub LoadCommendations(ByVal empID As Integer)
        Try
            Me.Commendations = AideClient.GetClient().GetCommendations(empID)
        Catch ex As Exception
            _logger.Error("Error loading Commendations")
            Throw ex
        End Try

    End Sub

    Public Sub LoadProjects(ByVal empID As Integer)
        Try
            Dim lstProjects As Dictionary(Of Integer, Project()) = New Dictionary(Of Integer, Project())
            Dim projects As Project() = AideClient.GetClient().GetProjectList(empID, 0)
            lstProjects.Add(0, projects)
            projects = AideClient.GetClient().GetProjectList(empID, 1)
            lstProjects.Add(1, projects)
            projects = AideClient.GetClient().GetProjectList(empID, 2)
            lstProjects.Add(2, projects)
            Me.Projects = lstProjects
        Catch ex As Exception
            _logger.Error("Error loading Projects")
            Throw ex
        End Try
    End Sub

    Public Sub LoadAssignedProjects(ByVal empID As Integer)
        Try
            Me.AssignedProjects = AideClient.GetClient().ViewProjectListofEmployee(empID)
        Catch ex As Exception
            _logger.Error("Error loading AssignedProjects")
            Throw ex
        End Try
    End Sub

    Public Sub LoadAuditQuestions(ByVal empID As Integer)
        Try
            Dim lstAuditQuestion As Dictionary(Of Integer, WorkplaceAudit()) = New Dictionary(Of Integer, WorkplaceAudit())
            Dim auditQuestions As WorkplaceAudit() = AideClient.GetClient().GetAuditQuestions(empID, "1")
            lstAuditQuestion.Add(0, auditQuestions)
            auditQuestions = AideClient.GetClient().GetAuditQuestions(empID, "2")
            lstAuditQuestion.Add(1, auditQuestions)
            auditQuestions = AideClient.GetClient().GetAuditQuestions(empID, "3")
            lstAuditQuestion.Add(2, auditQuestions)
            auditQuestions = AideClient.GetClient().GetAuditQuestions(empID, "4")
            lstAuditQuestion.Add(3, auditQuestions)
            Me.AuditQuestions = lstAuditQuestion
        Catch ex As Exception
            _logger.Error("Error loading AuditQuestions")
            Throw ex
        End Try
    End Sub

    Public Sub LoadNickNames(ByVal email As String)
        Try
            Me.NickNames = AideClient.GetClient().GetNicknameByDeptID(email)
        Catch ex As Exception
            _logger.Error("Error loading AuditQuestions")
            Throw ex

        End Try
    End Sub


    Private _fiscalYears As FiscalYear()
    Public Property FiscalYears() As FiscalYear()
        Get
            Return _fiscalYears
        End Get
        Set(ByVal value As FiscalYear())
            _fiscalYears = value
            NotifyPropertyChanged("FiscalYears")
        End Set
    End Property

    Private _myEmployeeID As Integer
    Public Property MyEmployeeID As Integer
        Get
            Return _myEmployeeID
        End Get
        Set(ByVal value As Integer)
            _myEmployeeID = value
            NotifyPropertyChanged("MyEmployeeID")
        End Set
    End Property

    Private _myEmail As String
    Public Property MyEmail As String
        Get
            Return _myEmail
        End Get
        Set(ByVal value As String)
            _myEmail = value
            NotifyPropertyChanged("MyEmail")
        End Set
    End Property

    Private _myProfile As Profile
    Public Property MyProfile As Profile
        Get
            Return _myProfile
        End Get
        Set(ByVal value As Profile)
            _myProfile = value
            NotifyPropertyChanged("MyProfile")
        End Set
    End Property

    Private _birthdayList As BirthdayList()
    Public Property BirthdayToday As BirthdayList()
        Get
            Return _birthdayList
        End Get
        Set(ByVal value As BirthdayList())
            _birthdayList = value
            NotifyPropertyChanged("BirthdayToday")
        End Set
    End Property

    Private _birthdayMonthList As BirthdayList()
    Public Property BirthdayMonth As BirthdayList()
        Get
            Return _birthdayMonthList
        End Get
        Set(ByVal value As BirthdayList())
            _birthdayMonthList = value
            NotifyPropertyChanged("BirthdayMonth")
        End Set
    End Property

    Private _birthdayAllList As BirthdayList()
    Public Property BirthdayAll As BirthdayList()
        Get
            Return _birthdayAllList
        End Get
        Set(ByVal value As BirthdayList())
            _birthdayAllList = value
            NotifyPropertyChanged("BirthdayAll")
        End Set
    End Property

    Private _kpiTargetsList As KPITargets()
    Public Property KPITargetList As KPITargets()
        Get
            Return _kpiTargetsList
        End Get
        Set(ByVal value As KPITargets())
            _kpiTargetsList = value
            NotifyPropertyChanged("KPITargetList")
        End Set
    End Property

    Private _kpiSummaryList As Dictionary(Of Integer, KPISummary())
    Public Property KPISummaryList As Dictionary(Of Integer, KPISummary())
        Get
            Return _kpiSummaryList
        End Get
        Set(ByVal value As Dictionary(Of Integer, KPISummary()))
            _kpiSummaryList = value
            NotifyPropertyChanged("KPISummaryList")
        End Set
    End Property

    Private _announcements As Announcements()
    Public Property Announcements() As Announcements()
        Get
            Return _announcements
        End Get
        Set(ByVal value As Announcements())
            _announcements = value
            NotifyPropertyChanged("Announcements")
        End Set
    End Property

    Private _commendations As Commendations()
    Public Property Commendations As Commendations()
        Get
            Return _commendations
        End Get
        Set(ByVal value As Commendations())
            _commendations = value
            NotifyPropertyChanged("Commendations")
        End Set
    End Property

    Private _projects As Dictionary(Of Integer, Project())
    Public Property Projects As Dictionary(Of Integer, Project())
        Get
            Return _projects
        End Get
        Set(ByVal value As Dictionary(Of Integer, Project()))
            _projects = value
            NotifyPropertyChanged("Projects")
        End Set
    End Property

    Private _assignedProjects As ViewProject()
    Public Property AssignedProjects As ViewProject()
        Get
            Return _assignedProjects
        End Get
        Set(ByVal value As ViewProject())
            _assignedProjects = value
            NotifyPropertyChanged("AssignedProjects")
        End Set
    End Property

    Private _auditquestions As Dictionary(Of Integer, WorkplaceAudit())
    Public Property AuditQuestions As Dictionary(Of Integer, WorkplaceAudit())
        Get
            Return _auditquestions
        End Get
        Set(ByVal value As Dictionary(Of Integer, WorkplaceAudit()))
            _auditquestions = value
            NotifyPropertyChanged("AuditQuestions")
        End Set
    End Property

    Private _nickNames As Employee()
    Public Property NickNames As Employee()
        Get
            Return _nickNames
        End Get
        Set(ByVal value As Employee())
            _nickNames = value
            NotifyPropertyChanged("NickNames")
        End Set
    End Property

    Public Shared Sub LoadMySkills()

    End Sub

    Private Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private Shared _instance As CommonUtility = Nothing
    Public Shared Function Instance() As CommonUtility
        If IsNothing(_instance) Then
            _instance = New CommonUtility()
        End If
        Return _instance
    End Function


End Class
