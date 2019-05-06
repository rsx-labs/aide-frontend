Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

Public Class SkillsDBProvider
    Private _skillList As ObservableCollection(Of mySkillList)
    Private _empList As ObservableCollection(Of mySkillList)
    Public _splist As ObservableCollection(Of mySkillList)
    Private _getlastreviewed As New mySkillList

    Sub New()
        _skillList = New ObservableCollection(Of mySkillList)
        _empList = New ObservableCollection(Of mySkillList)
        _splist = New ObservableCollection(Of mySkillList)
    End Sub

    Public Sub SetSkillList(ByVal skilllist As Skills)
        Dim _skillobj As mySkillList = New mySkillList With {.Skill_Descr = skilllist.DESCR,
                                                             .Skill_ID = skilllist.SkillID}
        _skillList.Add(_skillobj)

    End Sub

    Public Function GetSkillList()
        Return _skillList
    End Function

    Public Sub SetEmpSkillByEmpID(ByVal skilllist As Skills)
        Dim _skillobj As mySkillList = New mySkillList With {.Emp_Name = skilllist.NAME,
                                                             .Emp_Image = skilllist.Image_Path}
        _empList.Add(_skillobj)
    End Sub

    Public Function GetEmpSkillByEmpID()
        Return _empList
    End Function

    Public Sub SetEmpSkillsProf(ByVal skillList As Skills)
        Dim _skillobj As mySkillList = New mySkillList With {.Emp_ID = skillList.EmpID,
                                                             .Emp_Name = skillList.NAME,
                                                             .Skill_Descr = skillList.DESCR,
                                                             .Prof_level = skillList.Prof_LVL,
                                                             .Last_Reviewed = skillList.Last_Reviewed}

        _splist.Add(_skillobj)
    End Sub

    Public Function getSkillprof()
        Return _splist
    End Function

    Public Sub SetSkillsLastReviewedProfLvl(ByVal skillList As Skills)
        _getlastreviewed.Last_Reviewed = skillList.Last_Reviewed
        _getlastreviewed.Prof_level = skillList.Prof_LVL
    End Sub

    Public Function GetSkillsLastReviewedProfLvl()
        Return _getlastreviewed
    End Function
End Class


Public Class mySkillList
    Public Skill_ID As Integer
    Public Skill_Descr As String
    Public Emp_ID As String
    Property Prof_level As Integer
    Property Last_Reviewed As Date
    Public Emp_Name As String
    Public Emp_Image As String
End Class