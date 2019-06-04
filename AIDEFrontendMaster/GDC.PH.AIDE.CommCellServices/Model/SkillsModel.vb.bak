Imports System.ComponentModel
Public Class SkillsModel
    Implements INotifyPropertyChanged

    Private _empID As Integer
    Private _skillID As Integer
    Private _skillDescr As String
    Property _lastReviewed As Date
    Property _profLevel As Integer
    Private _EmpName As String
    Private _EmpImage As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal rawSkillList As mySkillList)
        Me.Skill_ID = rawSkillList.Skill_ID
        Me.Skill_Descr = rawSkillList.Skill_Descr
        Me.Emp_ID = rawSkillList.Emp_ID
        Me.Prof_Level = rawSkillList.Prof_level
        Me.Last_Reviewed = rawSkillList.Last_Reviewed
        Me._EmpName = rawSkillList.Emp_Name
        Me._EmpImage = rawSkillList.Emp_Image
    End Sub

    Public Property Last_Reviewed As Date
        Set(value As Date)
            _lastReviewed = value
        End Set
        Get
            _lastReviewed = Last_Reviewed.ToString("MM-dd-yyyy")
            NotifyPropertyChanged("Last_Reviewed")
        End Get
    End Property

    Public Property Prof_Level As Integer
        Set(value As Integer)
            _profLevel = value
        End Set
        Get
            _profLevel = Prof_Level
            NotifyPropertyChanged("Prof_Level")
        End Get
    End Property

    Public Property Skill_ID As Integer
        Set(value As Integer)
            _skillID = value
            NotifyPropertyChanged("Skill_ID")
        End Set
        Get
            Return _skillID
        End Get
    End Property

    Public Property Skill_Descr As String
        Get
            Return _skillDescr
        End Get
        Set(value As String)
            _skillDescr = value
            NotifyPropertyChanged("DESCR")
        End Set
    End Property

    Public Property Emp_ID As Integer
        Get
            Return _empID
        End Get
        Set(value As Integer)
            _empID = Emp_ID
            NotifyPropertyChanged("Emp_ID")
        End Set
    End Property

    Public Property EmpName As String
        Get
            Return _EmpName
        End Get
        Set(value As String)
            _EmpName = value
            NotifyPropertyChanged("EmpName")
        End Set
    End Property

    Public Property EmpImage As String
        Get
            Return _EmpImage
        End Get
        Set(value As String)
            _EmpImage = value
            NotifyPropertyChanged("EmpImage")
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal propertyname As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyname))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
