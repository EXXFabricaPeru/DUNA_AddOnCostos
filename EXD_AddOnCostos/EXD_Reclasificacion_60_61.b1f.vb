Option Strict Off
Option Explicit On

Imports SAPbouiCOM.Framework
Imports System.Runtime.InteropServices

'Reclasificación de cuentas 60-61

Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_Reclasificacion_60_61", "EXD_Reclasificacion_60_61.b1f")>
    Friend Class EXD_Reclasificacion_60_61
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private SBO_Company As SAPbobsCOM.Company
        Dim util As New Util

        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        ReadOnly SP_SQL_SBO_EXD_RECLASIFICACION_CTA_60_61 As String = "SBO_EXD_RECLASIFICACION_60_61 "
        ReadOnly SP_HANA_SBO_EXD_RECLASIFICACION_CTA_60_61 As String = "SBO_EXD_RECLASIFICACION_60_61 "


        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)

            UIAPIRawForm.Title = "RECLASIFICACIÓN DE CUENTAS [60-61]"

            SBO_Application = pAplicacion
            SBO_Company = pCompany
            SboGuiApi = New SAPbouiCOM.SboGuiApi

            DT_FECHA_INICIO.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_FIN.String = Now.Date.Date.ToString("ddMMyyyy")

            DT_FECHA_RECLASIFICACION.String = Now.Date.Date.ToString("ddMMyyyy")

            Me.UIAPIRawForm.Left = (SBO_Application.Desktop.Width / 2) - (UIAPIRawForm.Width / 2)
            Me.UIAPIRawForm.Top = (SBO_Application.Desktop.Height / 2) - ((UIAPIRawForm.Height / 2) + 60)

            logger.Info("Formulario RECLASIFICACIÓN DE CUENTAS [60-61 iniciado")

        End Sub

        Public Overrides Sub OnInitializeComponent()
            Me.StaticText0 = CType(Me.GetItem("Item_0").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_INICIO = CType(Me.GetItem("F1").Specific, SAPbouiCOM.EditText)
            Me.StaticText1 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("F2").Specific, SAPbouiCOM.EditText)
            Me.BT_BUSCAR = CType(Me.GetItem("B1").Specific, SAPbouiCOM.Button)
            Me.StaticText2 = CType(Me.GetItem("Item_6").Specific, SAPbouiCOM.StaticText)
            Me.MT_MATRIX = CType(Me.GetItem("MTX").Specific, SAPbouiCOM.Matrix)
            Me.BT_GRABAR = CType(Me.GetItem("B2").Specific, SAPbouiCOM.Button)
            Me.BT_DESMARCAR_TODO = CType(Me.GetItem("B4").Specific, SAPbouiCOM.Button)
            Me.BT_SELECCIONAR_TODO = CType(Me.GetItem("B3").Specific, SAPbouiCOM.Button)
            Me.DT_FECHA_RECLASIFICACION = CType(Me.GetItem("F3").Specific, SAPbouiCOM.EditText)
            Me.StaticText3 = CType(Me.GetItem("Item_2").Specific, SAPbouiCOM.StaticText)
            Me.LBL_RESULTADO = CType(Me.GetItem("LBL_R").Specific, SAPbouiCOM.StaticText)
            Me.OnCustomInitialize()

        End Sub

        Public Overrides Sub OnInitializeFormEvents()

        End Sub
        Private Sub OnCustomInitialize()

        End Sub
        Private WithEvents StaticText0 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_INICIO As SAPbouiCOM.EditText
        Private WithEvents StaticText1 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_FIN As SAPbouiCOM.EditText
        Private WithEvents BT_BUSCAR As SAPbouiCOM.Button
        Private WithEvents StaticText2 As SAPbouiCOM.StaticText
        Private WithEvents MT_MATRIX As SAPbouiCOM.Matrix
        Private WithEvents BT_GRABAR As SAPbouiCOM.Button
        Private WithEvents BT_DESMARCAR_TODO As SAPbouiCOM.Button
        Private WithEvents BT_SELECCIONAR_TODO As SAPbouiCOM.Button
        Private WithEvents DT_FECHA_RECLASIFICACION As SAPbouiCOM.EditText
        Private WithEvents StaticText3 As SAPbouiCOM.StaticText


        Private Sub BT_BUSCAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_BUSCAR.ClickBefore
            ' Throw New System.NotImplementedException()

            'Win32.AllocConsole()
            'Console.WriteLine(SP_SQL_SBO_EXD_RECLASIFICACION_CTA_60_61)
            'Win32.FreeConsole()

            Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
            Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()

            cargarMatrix(stFecha_Inicio, stFecha_Fin)

        End Sub

        Sub cargarMatrix(ByVal sF1 As String, ByVal SF2 As String)
            GC.Collect()

            Dim mquery As String = ""

            Try
                If ConexionSAP = TipoConexion.Sql Then

                    mquery = SP_SQL_SBO_EXD_RECLASIFICACION_CTA_60_61 & "'" & sF1 & "','" & SF2 & "'"
                ElseIf ConexionSAP = TipoConexion.Hana Then
                    mquery = "CALL " + SP_SQL_SBO_EXD_RECLASIFICACION_CTA_60_61 + "('" + sF1 + "'" + "," + "'" + SF2 + "')"
                End If
            Catch ex As Exception
                SBO_Application.StatusBar.SetText(mquery + " : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                logger.Error("mquery: " & mquery & "Descripción: " & ex.Message)
                Return
            End Try

            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_0").ExecuteQuery(mquery)
            Catch ex As Exception
                SBO_Application.StatusBar.SetText("DT_0" + " : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                logger.Error("DT_0: " & ex.Message)
                Return
            End Try

            Try
                'MT_MATRIX.Columns.Item("#").DataBind.Bind("DT_0", "TransId")
                'MT_MATRIX.Columns.Item("#").Visible = False

                MT_MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_0", "Seleccionar")
                MT_MATRIX.Columns.Item("Col_1").ValOn = "0"

                MT_MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_0", "Valor")
                MT_MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_0", "TransId")
                MT_MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_0", "CodigoCTA")
                MT_MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_0", "RefDate")
                MT_MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_0", "NombreCTA")
                MT_MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_0", "CuentaSYS")


                MT_MATRIX.LoadFromDataSource()

                SBO_Application.StatusBar.SetText("Success", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)

                LBL_RESULTADO.Caption = "(" + MT_MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"

            Catch ex As Exception
                SBO_Application.StatusBar.SetText("cargarMatrix, " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

                logger.Error("cargarMatrix: " & ex.Message)
                Return
            End Try
            GC.Collect()
        End Sub
        Public Sub AddChooseFromList(ByVal oColumn As SAPbouiCOM.Column, ByVal strObjectType As String, ByVal strAliasName As String, ByVal oCFLCollection As SAPbouiCOM.ChooseFromListCollection, ByVal bIsMultiSelect As Boolean, ByVal oConditions As SAPbouiCOM.Conditions, ByVal oSBOApplication As SAPbouiCOM.Application)

            'Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams

            oCFLCreationParams = SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            oCFLCreationParams.MultiSelection = bIsMultiSelect
            oCFLCreationParams.ObjectType = strObjectType

            'Dim strCFLID As String

            'strCFLID = GetChooseFromListID(oSBOApplication)

            'oCFLCreationParams.UniqueID = strCLFID
            'oCFL = oCFLCollection.Add(oCFLCreationParams)

            'oCFL.SetConditions(oConditions)

            'oColumn.ChooseFromListUID = strCLFID
            'oColumn.ChooseFromListAlias = strAliasName
        End Sub
  



        Private Sub BT_SELECCIONAR_TODO_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_SELECCIONAR_TODO.ClickBefore
            ' Throw New System.NotImplementedException()

            Try
                For n As Integer = 1 To Me.MT_MATRIX.RowCount
                    CType(Me.MT_MATRIX.Columns.Item(1).Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True
                Next
            Catch ex As Exception
                SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End Try
        End Sub

        Private Sub BT_DESMARCAR_TODO_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_DESMARCAR_TODO.ClickBefore
            'Throw New System.NotImplementedException()

            Try
                For n As Integer = 1 To Me.MT_MATRIX.RowCount
                    CType(Me.MT_MATRIX.Columns.Item(1).Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = False
                Next
            Catch ex As Exception
                SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End Try

        End Sub

        Private Sub BT_GRABAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_GRABAR.ClickBefore
            'Throw New System.NotImplementedException()



            Dim CodeMethod As Integer = 0
            Dim ErrorCode As Integer = 0
            Dim ErrorMsg As String = ""

            'Dim oJournalEntries As SAPbobsCOM.Documents
            'oJournalEntries = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)


            Dim oJournalEntries As SAPbobsCOM.JournalEntries


            Dim stTransId As String
            Dim doValor As Double
            Dim iTransId As Integer
            Dim sOrigen As String

            Dim dtFechaReclasificacion As String = util.formartDate(DT_FECHA_RECLASIFICACION.Value)

            For n As Integer = 1 To Me.MT_MATRIX.RowCount

                oJournalEntries = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)

                'Procesamos solo las lineas marcadas
                If CType(Me.MT_MATRIX.Columns.Item("Col_1").Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True Then

                    Dim vRecOJDT As SAPbobsCOM.Recordset
                    vRecOJDT = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                    stTransId = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    Dim SQL_OJDT As String = ""

                    SQL_OJDT = "SELECT DISTINCT " & _
                    "T0.""TransId"", " & _
                    "T0.""BaseRef"", " & _
                    "T0.""RefDate""," & _
                    "T0.""DueDate""," & _
                    "T0.""TaxDate""," & _
                    "T0.""TransType""," & _
                    "T0.""U_EXX_TIPCON""," & _
                    "T1.""Line_ID""," & _
                    "T1.""Account""," & _
                    "T1.""OcrCode3"" " & _
                    "FROM OJDT T0 " & _
                    "INNER JOIN JDT1 T1 ON T0.""TransId"" = T1.""TransId"" " & _
                    "INNER JOIN OACT T2 ON T1.""Account"" = T2.""AcctCode"" " & _
                    "WHERE T0.""TransId"" = " + stTransId + " " & _
                    "AND LEFT(T2.""Segment_0"",2) IN ('60','61')"

                    If ConexionSAP = TipoConexion.Sql Then
                        vRecOJDT.DoQuery(SQL_OJDT)
                        vRecOJDT.MoveFirst()

                    Else
                        vRecOJDT.DoQuery(SQL_OJDT)
                        vRecOJDT.MoveFirst()
                    End If

                    iTransId = vRecOJDT.Fields.Item("TransId").Value
                    sOrigen = vRecOJDT.Fields.Item("TransType").Value

                    oJournalEntries.ReferenceDate = dtFechaReclasificacion 'vRecOJDT.Fields.Item("RefDate").Value
                    oJournalEntries.DueDate = dtFechaReclasificacion 'vRecOJDT.Fields.Item("DueDate").Value
                    oJournalEntries.TaxDate = dtFechaReclasificacion 'vRecOJDT.Fields.Item("TaxDate").Value
                    oJournalEntries.Memo = "RECLASIFICACION DE CUENTAS 60-61"
                    oJournalEntries.Reference = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    oJournalEntries.Reference2 = vRecOJDT.Fields.Item("BaseRef").Value
                    oJournalEntries.TransactionCode = "REC"
                    oJournalEntries.UserFields.Fields.Item("U_EXX_TIPCON").Value = vRecOJDT.Fields.Item("U_EXX_TIPCON").Value

                    doValor = CType(Me.MT_MATRIX.Columns.Item("Col_0").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    oJournalEntries.Lines.AccountCode = vRecOJDT.Fields.Item("Account").Value

                    'Positivo
                    If doValor > 0 Then

                        oJournalEntries.Lines.Credit = doValor
                        oJournalEntries.Lines.CostingCode3 = vRecOJDT.Fields.Item("OcrCode3").Value

                        oJournalEntries.Lines.Add()

                        oJournalEntries.Lines.AccountCode = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        If doValor > 0 Then
                            oJournalEntries.Lines.Debit = doValor
                        Else
                            oJournalEntries.Lines.Debit = doValor * -1
                        End If
                        oJournalEntries.Lines.CostingCode3 = vRecOJDT.Fields.Item("OcrCode3").Value

                    Else
                        oJournalEntries.Lines.Debit = doValor * -1
                        oJournalEntries.Lines.CostingCode3 = vRecOJDT.Fields.Item("OcrCode3").Value

                        oJournalEntries.Lines.Add()

                        oJournalEntries.Lines.AccountCode = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim()
                        If doValor > 0 Then
                            oJournalEntries.Lines.Credit = doValor
                        Else
                            oJournalEntries.Lines.Credit = doValor * -1
                        End If
                        oJournalEntries.Lines.CostingCode3 = vRecOJDT.Fields.Item("OcrCode3").Value

                    End If

                    CodeMethod = oJournalEntries.Add

                    If CodeMethod <> 0 Then
                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                        SBO_Application.StatusBar.SetText(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

                        logger.Error(ErrorCode & " : " & ErrorMsg)
                    Else
                       
                        SBO_Application.StatusBar.SetText("Documento creado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)

                        If sOrigen.Equals("202") Then
                            'No hace naaa
                        Else
                            oJournalEntries.GetByKey(iTransId)
                            oJournalEntries.TransactionCode = "ANU"

                            CodeMethod = oJournalEntries.Update()

                            If CodeMethod <> 0 Then
                                SBO_Company.GetLastError(ErrorCode, ErrorMsg)

                                SBO_Application.StatusBar.SetText(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Else
                                SBO_Application.StatusBar.SetText("Documento actualizado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                            End If
                        End If
                    End If
                End If
            Next

            cargarMatrix(DT_FECHA_INICIO.Value, DT_FECHA_FIN.Value)

        End Sub

        'Private Sub MT_MATRIX_KeyDownAfter(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg) Handles MT_MATRIX.KeyDownAfter
        '    'Throw New System.NotImplementedException()
        '    Application.SBO_Application.SetStatusBarMessage("   121212", SAPbouiCOM.BoMessageTime.bmt_Short, True)

        'End Sub

        Private WithEvents objEvento As SAPbouiCOM.SBOItemEventArg
        Private WithEvents objDataTable As SAPbouiCOM.DataTable
        Dim val0 As String = ""
        Dim segmento As String = ""

        Private Sub MT_MATRIX_ChooseFromListAfter(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg) Handles MT_MATRIX.ChooseFromListAfter
            'Throw New System.NotImplementedException()

            objEvento = pVal
            objDataTable = objEvento.SelectedObjects

            Try
                If objDataTable Is Nothing Then
                Else
                    val0 = System.Convert.ToString(objDataTable.GetValue("AcctName", 0))
                    MT_MATRIX.Columns.Item("Col_5").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                End If
            Catch ex As Exception
                'MsgBox(ex.ToString)
            End Try

            Try
                If objDataTable Is Nothing Then
                Else
                    val0 = System.Convert.ToString(objDataTable.GetValue("AcctCode", 0))
                    MT_MATRIX.Columns.Item("Col_6").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                End If
            Catch ex As Exception
                'MsgBox(ex.ToString)
            End Try

            Try
                If objDataTable Is Nothing Then
                Else
                    val0 = System.Convert.ToString(objDataTable.GetValue("FormatCode", 0))
                    MT_MATRIX.Columns.Item("Col_3").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                End If
            Catch ex As Exception
                'MsgBox(ex.ToString)
            End Try

        End Sub
        Private WithEvents LBL_RESULTADO As SAPbouiCOM.StaticText
      
    End Class

End Namespace
