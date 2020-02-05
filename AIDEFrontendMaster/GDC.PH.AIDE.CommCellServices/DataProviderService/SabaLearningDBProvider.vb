Imports System.Collections.ObjectModel
Imports UI_AIDE_CommCellServices.ServiceReference1
Public Class SabaLearningDBProvider

    Private _objSabaLearning As ObservableCollection(Of mySabaLearningSet)

    Public Sub New()
        _objSabaLearning = New ObservableCollection(Of mySabaLearningSet)
    End Sub

    Public Function _getobjSabaLearning() As ObservableCollection(Of mySabaLearningSet)
        Return _objSabaLearning
    End Function

    Public Function _setlistofitems(ByRef sabalearning As SabaLearning)
        Dim _mySabaLearningSet As New mySabaLearningSet With {._sabaid = sabalearning.SABA_ID,
                                                  ._empid = sabalearning.EMP_ID,
                                                  ._title = sabalearning.TITLE,
                                                  ._enddate = sabalearning.END_DATE,
                                                  ._datecompleted = sabalearning.DATE_COMPLETED,
                                                  ._imagepath = sabalearning.IMAGE_PATH,
                                                  ._totalenroll = sabalearning.TOTAL_ENROLL,
                                                  ._totalcomplete = sabalearning.TOTAL_COMPLETED}

        _objSabaLearning.Add(_mySabaLearningSet)
        Return _mySabaLearningSet
    End Function
End Class

Public Class mySabaLearningSet
    Property _sabaid As Integer
    Property _empid As Integer
    Property _title As String
    Property _enddate As Date
    Property _datecompleted As String
    Property _imagepath As String
    Property _completepercent As String
    Property _totalenroll As Integer
    Property _totalcomplete As Integer
End Class
