Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1

''' <summary>
''' By Marivic Espino
''' </summary>
Public Class LessonLearntDBProvider
    Private _lessonLearntList As ObservableCollection(Of MyLessonLearntList)
    Private _MylessonLearntList As New MyLessonLearntList

    Sub New()
        _lessonLearntList = New ObservableCollection(Of MyLessonLearntList)
    End Sub

    Public Function GetLessonLearntList()
        Return _lessonLearntList
    End Function

    Public Sub SetLessonLearntList(ByVal lessonLearnt As LessonLearnt)
        Dim _lessonLearntObj As MyLessonLearntList = New MyLessonLearntList With {.ReferenceNo = lessonLearnt.ReferenceNo, _
                                                                                  .EmpID = lessonLearnt.EmpID, _
                                                                                  .Nickname = lessonLearnt.Nickname, _
                                                                                  .Problem = lessonLearnt.Problem, _
                                                                                  .Resolution = lessonLearnt.Resolution, _
                                                                                  .ActionNo = lessonLearnt.ActionNo}

        _lessonLearntList.Add(_lessonLearntObj)
    End Sub

End Class

Public Class MyLessonLearntList
    Public Property ReferenceNo As String
    Public Property EmpID As Integer
    Public Property Nickname As String
    Public Property Problem As String
    Public Property Resolution As String
    Public Property ActionNo As String
End Class
