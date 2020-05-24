Imports UI_AIDE_CommCellServices.ServiceReference1
Imports System.Reflection
Imports System.IO
Imports System.Diagnostics
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports LiveCharts
Imports LiveCharts.Wpf
<CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Single, UseSynchronizationContext:=False)>
Public Class ProblemSolvingPage
    Implements ServiceReference1.IAideServiceCallback

#Region "Declarations"
    'Private client As AideServiceClient
    Private _problemlst As Problem()
    Private _problemCauselst As Problem()
    Private _problemOptionlst As Problem()
    Private _problemSolutionlst As Problem()
    Private _problemImplementlst As Problem()
    Private _mainFrame As Frame
    Private _addFrame As Frame
    Private _menuGrid As Grid
    Private _subMenuFrame As Frame
    Private _profile As Profile
    Private listEmpID As String
#End Region
#Region "Constructor"
    Public Sub New(_profile As Profile, mFrame As Frame, _addframe As Frame, _menugrid As Grid, _submenuframe As Frame)
        Me._profile = _profile
        Me._mainFrame = mFrame
        Me._addFrame = _addframe
        Me._menuGrid = _menugrid
        Me._subMenuFrame = _submenuframe
        'client = aideService
        Me.InitializeComponent()
        ControlsOff()
        LoadProblem()
        loadAll()
        If Me._profile.Permission_ID = 5 Then
            ProblemAddBtn.Visibility = Visibility.Collapsed
            ProblemEditBtn.Visibility = Visibility.Collapsed
        End If
    End Sub
#End Region
#Region "Function"
    'Public Function InitializeService() As Boolean
    '    'Dim bInitialize As Boolean = False
    '    'Try
    '    '    Dim Context As InstanceContext = New InstanceContext(Me)
    '    '    client = New AideServiceClient(Context)
    '    '    client.Open()
    '    '    bInitialize = True
    '    'Catch ex As SystemException
    '    '    client.Abort()
    '    '    MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
    '    'End Try
    '    'Return bInitialize
    '    Return True
    'End Function
    Public Sub loadAll()
        'If InitializeService() Then
        _problemCauselst = AideClient.GetClient().GetAllProblemCause()
        _problemOptionlst = AideClient.GetClient().GetAllProblemOption()
        _problemSolutionlst = AideClient.GetClient().GetAllProblemSolution()
        _problemImplementlst = AideClient.GetClient().GetAllProblemImplement()
        'End If
    End Sub
    Public Sub LoadProblem()
        Try
            'If InitializeService() Then
            Dim _problemDBProvider As New ProblemDBProvider
            Dim _problemVM As New ProblemViewModel
            _problemlst = Nothing
            _problemlst = AideClient.GetClient().GetAllProblem(_profile.Emp_ID)
            If Not IsNothing(_problemlst) Then
                For Each objProblem As Problem In _problemlst
                    _problemDBProvider.SetProblemList(objProblem)
                Next

                For Each iProblem As myProblem In _problemDBProvider.GetProblemList()
                    _problemVM.PROBLEM_LIST.Add(New ProblemModel(iProblem))
                Next

                ProblemLV.ItemsSource = _problemVM.PROBLEM_LIST
            End If
            'End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub LoadProblemCause(ByVal _probID As Integer)
        Try

            Dim _problemDBProvider As New ProblemDBProvider
                Dim _problemVM As New ProblemViewModel


                If Not IsNothing(_problemCauselst) Then
                    For Each objProblem As Problem In _problemCauselst
                        If objProblem.ProblemID = _probID Then
                            _problemDBProvider.SetProblemCauseList(objProblem)
                        End If
                    Next

                    For Each iProblem As myProblemCause In _problemDBProvider.GetProblemCauseList()
                        _problemVM.PROBLEM_CAUSE_LIST.Add(New ProblemModel(iProblem))
                    Next

                    ProblemCauseLV.ItemsSource = _problemVM.PROBLEM_CAUSE_LIST
                End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub LoadProblemOption(ByVal _probID As Integer)
        Try

            Dim _problemDBProvider As New ProblemDBProvider
                Dim _problemVM As New ProblemViewModel
                Dim _problemCount As Integer = 0

                If Not IsNothing(_problemOptionlst) Then
                    For Each objProblem As Problem In _problemOptionlst
                        If objProblem.ProblemID = _probID Then
                            _problemDBProvider.SetProblemOptionList(objProblem)
                        End If
                    Next

                    For Each iProblem As myProblemOption In _problemDBProvider.GetProblemOptionList()
                        _problemCount = _problemCount + 1
                        _problemVM.PROBLEM_OPTION_LIST.Add(New ProblemModel(iProblem, _problemCount))
                    Next

                    ProblemOptionLV.ItemsSource = _problemVM.PROBLEM_OPTION_LIST
                End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub LoadProblemSolution(ByVal _probID As Integer, ByVal _optionID As Integer)
        Try

            Dim _problemDBProvider As New ProblemDBProvider
                Dim _problemVM As New ProblemViewModel

                If Not IsNothing(_problemSolutionlst) Then
                    For Each objProblem As Problem In _problemSolutionlst
                        If objProblem.ProblemID = _probID And objProblem.OptionID = _optionID Then
                            _problemDBProvider.SetProblemSolutionList(objProblem)
                        End If
                    Next

                    For Each iProblem As myProblemSolution In _problemDBProvider.GetProblemSolutionList()
                        _problemVM.PROBLEM_SOLUTION_LIST.Add(New ProblemModel(iProblem))
                    Next

                    ProblemSolutionLV.ItemsSource = _problemVM.PROBLEM_SOLUTION_LIST
                End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Public Sub LoadProblemImplement(ByVal _probID As Integer, ByVal _optionID As Integer)
        Try

            Dim _problemDBProvider As New ProblemDBProvider
                Dim _problemVM As New ProblemViewModel
                Dim SeriesCollection As SeriesCollection
                Dim resultVal As New ChartValues(Of Double)()
                Dim count As Integer = 0


                If Not IsNothing(_problemImplementlst) Then

                    For Each objProblem As Problem In _problemImplementlst
                        If objProblem.ProblemID = _probID And objProblem.OptionID = _optionID Then
                            _problemDBProvider.SetProblemImplementList(objProblem)
                        End If
                    Next

                    Dim Description(_problemDBProvider.GetProblemImplementList().Count) As String

                    For Each iProblem As myProblemImplement In _problemDBProvider.GetProblemImplementList()
                        _problemVM.PROBLEM_IMPLEMENT_LIST.Add(New ProblemModel(iProblem))
                        resultVal.Add(iProblem.ImplementValue)
                        Description(count) = iProblem.ImplementDescr
                        count += 1
                    Next
                    If _problemDBProvider.GetProblemImplementList().Count > 0 Then
                        chartResult.Visibility = Visibility.Visible
                    Else
                        chartResult.Visibility = Visibility.Hidden
                    End If

                    ProblemImplementLV.ItemsSource = _problemVM.PROBLEM_IMPLEMENT_LIST

                SeriesCollection = New SeriesCollection From {
   New LineSeries With {
                          .Values = resultVal,
                          .DataLabels = True,
                          .Title = "Value",
                          .Fill = Brushes.ForestGreen,
                          .Foreground = Brushes.DimGray}
           }
                chartResult.Series = SeriesCollection
                    chartResult.AxisX.First().Labels = Description
                    chartResult.AxisY.First().LabelFormatter = Function(value) value
                    chartResult.AxisX.First().LabelsRotation = 135

                End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
#End Region
#Region "Events"
    Private Sub ProblemLV_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ProblemLV.SelectionChanged
        Try
            e.Handled = True
            If ProblemLV.SelectedIndex <> -1 Then

                If ProblemLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As Integer = CType(ProblemLV.SelectedItem, ProblemModel).PROBLEM_ID
                    Dim objEmpID As Integer = CType(ProblemLV.SelectedItem, ProblemModel).EMPLOYEE_ID
                    listEmpID = CType(ProblemLV.SelectedItem, ProblemModel).PROBLEM_INVOLVE
                    ReadyUI(objEmpID, listEmpID)
                    LoadProblemCause(objProblem)
                    LoadProblemOption(objProblem)
                    ProblemSolutionLV.ItemsSource = Nothing
                    ProblemImplementLV.ItemsSource = Nothing
                    chartResult.Visibility = Visibility.Hidden
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub ProblemOptionLV_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ProblemOptionLV.SelectionChanged
        Try
            e.Handled = True
            If ProblemOptionLV.SelectedIndex <> -1 Then

                If ProblemOptionLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As Integer = CType(ProblemOptionLV.SelectedItem, ProblemModel).PROBLEM_ID
                    Dim objOption As Integer = CType(ProblemOptionLV.SelectedItem, ProblemModel).OPTION_ID
                    ReadyOption()
                    LoadProblemSolution(objProblem, objOption)
                    LoadProblemImplement(objProblem, objOption)
                End If
            End If

        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub ProblemAddBtn_Click(sender As Object, e As RoutedEventArgs) Handles ProblemAddBtn.Click
        Try
            _addFrame.Navigate(New ProblemAddPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame))
            _mainFrame.IsEnabled = False
            _mainFrame.Opacity = 0.3
            _menuGrid.IsEnabled = False
            _menuGrid.Opacity = 0.3
            _subMenuFrame.IsEnabled = False
            _subMenuFrame.Opacity = 0.3
            _addFrame.Margin = New Thickness(0)
            _addFrame.Visibility = Visibility.Visible
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub

    Private Sub CauseAddBtn_Click(sender As Object, e As RoutedEventArgs) Handles CauseAddBtn.Click
        Try
            e.Handled = True
            If ProblemLV.SelectedIndex <> -1 Then
                If ProblemLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemLV.SelectedItem, ProblemModel)

                    _addFrame.Navigate(New ProblemCauseAddPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub CauseEditBtn_Click(sender As Object, e As RoutedEventArgs) Handles CauseEditBtn.Click
        Try
            e.Handled = True
            If ProblemCauseLV.SelectedIndex <> -1 Then
                If ProblemCauseLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemCauseLV.SelectedItem, ProblemModel)
                    objProblem.PROBLEM_DESCR = CType(ProblemLV.SelectedItem, ProblemModel).PROBLEM_DESCR
                    _addFrame.Navigate(New ProblemCauseUpdatePage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub OptionAddBtn_Click(sender As Object, e As RoutedEventArgs) Handles OptionAddBtn.Click
        Try
            e.Handled = True
            If ProblemLV.SelectedIndex <> -1 Then
                If ProblemLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemLV.SelectedItem, ProblemModel)

                    _addFrame.Navigate(New ProblemOptionAddPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub OptionEditBtn_Click(sender As Object, e As RoutedEventArgs) Handles OptionEditBtn.Click
        Try
            e.Handled = True
            If ProblemOptionLV.SelectedIndex <> -1 Then
                If ProblemOptionLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemOptionLV.SelectedItem, ProblemModel)
                    objProblem.PROBLEM_DESCR = CType(ProblemLV.SelectedItem, ProblemModel).PROBLEM_DESCR
                    _addFrame.Navigate(New ProblemOptionUpdatePage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub SolutionAddBtn_Click(sender As Object, e As RoutedEventArgs) Handles SolutionAddBtn.Click
        Try
            e.Handled = True
            If ProblemOptionLV.SelectedIndex <> -1 Then
                If ProblemOptionLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemOptionLV.SelectedItem, ProblemModel)

                    _addFrame.Navigate(New ProblemSolutionAddPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub SolutionEditBtn_Click(sender As Object, e As RoutedEventArgs) Handles SolutionEditBtn.Click
        Try
            e.Handled = True
            If ProblemSolutionLV.SelectedIndex <> -1 Then
                If ProblemSolutionLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemSolutionLV.SelectedItem, ProblemModel)
                    objProblem.OPTION_DESCR = CType(ProblemOptionLV.SelectedItem, ProblemModel).OPTION_DESCR
                    _addFrame.Navigate(New ProblemSolutionUpdatePage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ImplementAddBtn_Click(sender As Object, e As RoutedEventArgs) Handles ImplementAddBtn.Click
        Try
            e.Handled = True
            If ProblemOptionLV.SelectedIndex <> -1 Then
                If ProblemOptionLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemOptionLV.SelectedItem, ProblemModel)

                    _addFrame.Navigate(New ProblemImplementAddPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ImplementEditBtn_Click(sender As Object, e As RoutedEventArgs) Handles ImplementEditBtn.Click
        Try
            e.Handled = True
            If ProblemImplementLV.SelectedIndex <> -1 Then
                If ProblemImplementLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemImplementLV.SelectedItem, ProblemModel)
                    objProblem.OPTION_DESCR = CType(ProblemOptionLV.SelectedItem, ProblemModel).OPTION_DESCR
                    _addFrame.Navigate(New ProblemImplementUpdatePage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub

    Private Sub ProblemEditBtn_Click(sender As Object, e As RoutedEventArgs) Handles ProblemEditBtn.Click
        Try
            e.Handled = True
            If ProblemLV.SelectedIndex <> -1 Then
                If ProblemLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemLV.SelectedItem, ProblemModel)

                    _addFrame.Navigate(New ProblemUpdatePage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try
    End Sub
    Private Sub ProblemViewBtn_Click(sender As Object, e As RoutedEventArgs) Handles ProblemViewBtn.Click
        Try
            e.Handled = True
            If ProblemLV.SelectedIndex <> -1 Then
                If ProblemLV.SelectedItem IsNot Nothing Then
                    Dim objProblem As ProblemModel = CType(ProblemLV.SelectedItem, ProblemModel)

                    _addFrame.Navigate(New ProblemViewPage(_mainFrame, _profile, _addFrame, _menuGrid, _subMenuFrame, objProblem))
                    _mainFrame.IsEnabled = False
                    _mainFrame.Opacity = 0.3
                    _menuGrid.IsEnabled = False
                    _menuGrid.Opacity = 0.3
                    _subMenuFrame.IsEnabled = False
                    _subMenuFrame.Opacity = 0.3
                    _addFrame.Margin = New Thickness(0)
                    _addFrame.Visibility = Visibility.Visible
                End If
            End If
        Catch ex As Exception
            MsgBox("An application error was encountered. Please contact your AIDE Administrator.", vbOKOnly + vbCritical, "AIDE")
        End Try

    End Sub



    Public Sub ReadyUI(ByVal objEmpID As Integer, ByVal emplist As String)
        If Not objEmpID = _profile.Emp_ID Then
            ProblemEditBtn.Visibility = Visibility.Collapsed
        Else
            ProblemEditBtn.Visibility = Visibility.Visible
        End If

        If emplist.Contains(_profile.Emp_ID.ToString()) Then
            CauseAddBtn.Visibility = Visibility.Visible
            OptionAddBtn.Visibility = Visibility.Visible
            CauseEditBtn.Visibility = Visibility.Visible
            OptionEditBtn.Visibility = Visibility.Visible
            SolutionAddBtn.Visibility = Visibility.Hidden
            ImplementAddBtn.Visibility = Visibility.Hidden
            SolutionEditBtn.Visibility = Visibility.Hidden
            ImplementEditBtn.Visibility = Visibility.Hidden
        Else
            CauseAddBtn.Visibility = Visibility.Hidden
            OptionAddBtn.Visibility = Visibility.Hidden
            CauseEditBtn.Visibility = Visibility.Hidden
            OptionEditBtn.Visibility = Visibility.Hidden
            SolutionAddBtn.Visibility = Visibility.Hidden
            ImplementAddBtn.Visibility = Visibility.Hidden
            SolutionEditBtn.Visibility = Visibility.Hidden
            ImplementEditBtn.Visibility = Visibility.Hidden
        End If
        If Me._profile.Permission_ID = 5 Then
            CauseAddBtn.Visibility = Visibility.Hidden
            OptionAddBtn.Visibility = Visibility.Hidden
            CauseEditBtn.Visibility = Visibility.Hidden
            OptionEditBtn.Visibility = Visibility.Hidden
            SolutionAddBtn.Visibility = Visibility.Hidden
            ImplementAddBtn.Visibility = Visibility.Hidden
            SolutionEditBtn.Visibility = Visibility.Hidden
            ImplementEditBtn.Visibility = Visibility.Hidden
        End If
    End Sub
    Public Sub ControlsOff()
        CauseAddBtn.Visibility = Visibility.Hidden
        OptionAddBtn.Visibility = Visibility.Hidden
        SolutionAddBtn.Visibility = Visibility.Hidden
        ImplementAddBtn.Visibility = Visibility.Hidden

        CauseEditBtn.Visibility = Visibility.Hidden
        OptionEditBtn.Visibility = Visibility.Hidden
        SolutionEditBtn.Visibility = Visibility.Hidden
        ImplementEditBtn.Visibility = Visibility.Hidden
    End Sub
    Public Sub ControlsOn()
        CauseAddBtn.Visibility = Visibility.Visible
        OptionAddBtn.Visibility = Visibility.Visible
        SolutionAddBtn.Visibility = Visibility.Visible
        ImplementAddBtn.Visibility = Visibility.Visible

        CauseEditBtn.Visibility = Visibility.Visible
        OptionEditBtn.Visibility = Visibility.Visible
        SolutionEditBtn.Visibility = Visibility.Visible
        ImplementEditBtn.Visibility = Visibility.Visible
    End Sub
    Public Sub ReadyOption()
        If listEmpID.Contains(_profile.Emp_ID.ToString()) Then
            SolutionAddBtn.Visibility = Visibility.Visible
            ImplementAddBtn.Visibility = Visibility.Visible
            SolutionEditBtn.Visibility = Visibility.Visible
            ImplementEditBtn.Visibility = Visibility.Visible
        Else
            SolutionAddBtn.Visibility = Visibility.Hidden
            ImplementAddBtn.Visibility = Visibility.Hidden
            SolutionEditBtn.Visibility = Visibility.Hidden
            ImplementEditBtn.Visibility = Visibility.Hidden
        End If
        If Me._profile.Permission_ID = 5 Then
            SolutionAddBtn.Visibility = Visibility.Hidden
            ImplementAddBtn.Visibility = Visibility.Hidden
            SolutionEditBtn.Visibility = Visibility.Hidden
            ImplementEditBtn.Visibility = Visibility.Hidden
        End If
    End Sub

#End Region

#Region "Service Methods"
    Public Sub NotifySuccess(message As String) Implements IAideServiceCallback.NotifySuccess
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyError(message As String) Implements IAideServiceCallback.NotifyError
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyPresent(EmployeeName As String) Implements IAideServiceCallback.NotifyPresent
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyOffline(EmployeeName As String) Implements IAideServiceCallback.NotifyOffline
        Throw New NotImplementedException()
    End Sub

    Public Sub NotifyUpdate(objData As Object) Implements IAideServiceCallback.NotifyUpdate
        Throw New NotImplementedException()
    End Sub

#End Region
End Class
