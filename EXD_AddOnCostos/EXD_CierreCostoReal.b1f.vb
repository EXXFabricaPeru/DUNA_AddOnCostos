Option Strict Off
Option Explicit On

Imports SAPbouiCOM.Framework

Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_CierreCostoReal", "EXD_CierreCostoReal.b1f")>
    Friend Class EXD_CierreCostoReal
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private SBO_Company As SAPbobsCOM.Company
        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


        Dim util As New Util

        ReadOnly SP_SQL_EXD_ANALISIS_EJECUTADO_REAL As String = "EXD_ANALISIS_EJECUTADO_REAL_1 "
        ReadOnly SP_SQL_EXD_PROCESO_SGTE As String = "EXD_PROCESO_SGTE"

        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)

            UIAPIRawForm.Title = "EXD: CIERRE DE COSTOS REAL"

            SBO_Application = pAplicacion
            SBO_Company = pCompany
            SboGuiApi = New SAPbouiCOM.SboGuiApi

            DT_FECHA_INICIO.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_FIN.String = Now.Date.Date.ToString("ddMMyyyy")

            Me.UIAPIRawForm.Left = (SBO_Application.Desktop.Width / 2) - (UIAPIRawForm.Width / 2)
            Me.UIAPIRawForm.Top = (SBO_Application.Desktop.Height / 2) - ((UIAPIRawForm.Height / 2) + 60)

        End Sub

        Public Overrides Sub OnInitializeComponent()
            Me.StaticText0 = CType(Me.GetItem("Item_0").Specific, SAPbouiCOM.StaticText)
            Me.StaticText1 = CType(Me.GetItem("Item_1").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_INICIO = CType(Me.GetItem("DT_1").Specific, SAPbouiCOM.EditText)
            Me.StaticText2 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("DT_2").Specific, SAPbouiCOM.EditText)
            Me.MT_MATRIX = CType(Me.GetItem("MTX").Specific, SAPbouiCOM.Matrix)
            Me.BTN_BUSCAR = CType(Me.GetItem("BT_1").Specific, SAPbouiCOM.Button)
            Me.BTN_GRABAR = CType(Me.GetItem("BT_2").Specific, SAPbouiCOM.Button)
            Me.BTN_SELECCIONAR_TODO = CType(Me.GetItem("B_3").Specific, SAPbouiCOM.Button)
            Me.BTN_DESMARCAR_TODO = CType(Me.GetItem("B_4").Specific, SAPbouiCOM.Button)
            Me.LBL_RESULTADO = CType(Me.GetItem("LBL_R").Specific, SAPbouiCOM.StaticText)
            Me.StaticText3 = CType(Me.GetItem("Item_2").Specific, SAPbouiCOM.StaticText)
            Me.CBO_ESTADO_OT = CType(Me.GetItem("CBO_1").Specific, SAPbouiCOM.ComboBox)
            Me.BTN_PROCESO = CType(Me.GetItem("BT_5").Specific, SAPbouiCOM.Button)
            Me.DT_PROCESO = CType(Me.GetItem("DT_3").Specific, SAPbouiCOM.EditText)
            Me.OnCustomInitialize()

        End Sub




        Private Sub OnCustomInitialize()

        End Sub
        Private WithEvents StaticText0 As SAPbouiCOM.StaticText
        Private WithEvents StaticText1 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_INICIO As SAPbouiCOM.EditText
        Private WithEvents StaticText2 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_FIN As SAPbouiCOM.EditText
        Private WithEvents MT_MATRIX As SAPbouiCOM.Matrix
        Private WithEvents BTN_BUSCAR As SAPbouiCOM.Button
        Private WithEvents BTN_GRABAR As SAPbouiCOM.Button
        Private WithEvents BTN_SELECCIONAR_TODO As SAPbouiCOM.Button
        Private WithEvents BTN_DESMARCAR_TODO As SAPbouiCOM.Button
        Private WithEvents LBL_RESULTADO As SAPbouiCOM.StaticText
        Private WithEvents BTN_PROCESO As SAPbouiCOM.Button
        Private WithEvents DT_PROCESO As SAPbouiCOM.EditText


        Private Sub BTN_BUSCAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BTN_BUSCAR.ClickBefore
            'Throw New System.NotImplementedException()

            Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
            Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()
            Dim stEstadoOT As String = CBO_ESTADO_OT.Value
            Dim stProceso As String = DT_PROCESO.Value

            'If stProceso.Equals("") Then
            '    Application.SBO_Application.SetStatusBarMessage("El campo Proceso es obligatorio, favor de presionar el botón 'Buscar Proceso'", SAPbouiCOM.BoMessageTime.bmt_Short, True)
            '    Return
            'End If

            If stEstadoOT.Equals("") Then
                Application.SBO_Application.SetStatusBarMessage("Seleccione el estado de la orden de trabajo", SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End If

            cargarMatrix(stFecha_Inicio, stFecha_Fin, stEstadoOT, stProceso)

        End Sub
        Sub cargarMatrix(ByVal sF1 As String, ByVal SF2 As String, sF3 As String, sF4 As String)

            GC.Collect()

            Dim mquery As String = ""

            Try
                If ConexionSAP = TipoConexion.Sql Then

                    mquery = SP_SQL_EXD_ANALISIS_EJECUTADO_REAL & "'" & sF1 & "','" & SF2 & "','" & sF3 & "'"
                ElseIf ConexionSAP = TipoConexion.Hana Then
                    mquery = "CALL " + SP_SQL_EXD_ANALISIS_EJECUTADO_REAL + "('" + sF1 + "'" + "," + "'" + SF2 + "','" + sF4 + "')"
                End If
            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al crear :" + mquery + " ,Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_C").ExecuteQuery(mquery)
            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al cargar DT_C " + mquery + ", " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            Try
                'MT_MATRIX.Columns.Item("#").DataBind.Bind("DT_C", "TransId")
                'MT_MATRIX.Columns.Item("#").Visible = False

                MT_MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_C", "Seleccionar")
                MT_MATRIX.Columns.Item("Col_0").ValOn = "0"

                MT_MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_C", "OTEstado")
                MT_MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_C", "CloseDate")
                MT_MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_C", "cultivo")
                MT_MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_C", "Campania")
                MT_MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_C", "codigoCC")
                MT_MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_C", "saldo")
                MT_MATRIX.Columns.Item("Col_7").DataBind.Bind("DT_C", "ItemCode")
                MT_MATRIX.Columns.Item("Col_8").DataBind.Bind("DT_C", "OnHand")

                MT_MATRIX.Columns.Item("Col_9").DataBind.Bind("DT_C", "SYS Costo de Ventas")
                MT_MATRIX.Columns.Item("Col_9").Visible = False

                MT_MATRIX.Columns.Item("Col_10").DataBind.Bind("DT_C", "Costo de Ventas")

                MT_MATRIX.Columns.Item("Col_11").DataBind.Bind("DT_C", "SYS Ajuste de Ventas")
                MT_MATRIX.Columns.Item("Col_11").Visible = False

                MT_MATRIX.Columns.Item("Col_12").DataBind.Bind("DT_C", "Ajuste de Ventas")

                MT_MATRIX.Columns.Item("Col_13").DataBind.Bind("DT_C", "DocEntry")
                MT_MATRIX.Columns.Item("Col_14").DataBind.Bind("DT_C", "DocNum")

                MT_MATRIX.Columns.Item("Col_15").DataBind.Bind("DT_C", "U_EXD_NROCTA")

                MT_MATRIX.Columns.Item("Col_16").DataBind.Bind("DT_C", "U_EXD_NROCTAsys")
                ' MT_MATRIX.Columns.Item("Col_16").Visible = False

                MT_MATRIX.Columns.Item("Col_17").DataBind.Bind("DT_C", "elementoCosto")
                'Col_18
                MT_MATRIX.Columns.Item("Col_18").DataBind.Bind("DT_C", "Unidad Negocio")

                MT_MATRIX.LoadFromDataSource()



                LBL_RESULTADO.Caption = "(" + MT_MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"

            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al cargar Matrix " + ",Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try



            'For i As Integer = 1 To MT_MATRIX.RowCount


            '    MT_MATRIX.CommonSetting.SetRowFontColor(1, 204)

            'Next


            GC.Collect()
        End Sub

        Private Sub BTN_GRABAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BTN_GRABAR.ClickBefore
            'Throw New System.NotImplementedException()


            ' Analisis Cultivo, Campaña,Centro de costo, Tipo.

            '¿Cerro la campaña?  en el rango de fechas
            'OTs Cerradas en ese periodo
            Dim CodeMethod As Integer = 0
            Dim ErrorCode As Integer = 0
            Dim ErrorMsg As String = ""

            Dim iCantOT_O As Integer = 0


            Dim iCantidad As Long
            Dim sEstadoOT As String
            Dim sCodigoArticulo As String
            Dim doSaldo As Decimal
            Dim sCodigoCosto As String
            Dim sDocNum As String

            Dim sCC_CostoVentas69 As String
            Dim sCC_AjusteVentas71 As String
            Dim sCC_Diario23 As String

            Dim dtFecha As String


            Dim oMaterialRevaluation As SAPbobsCOM.MaterialRevaluation
            Dim oJournalEntries As SAPbobsCOM.JournalEntries

            Dim oInventoryGenEntry As SAPbobsCOM.Documents
            Dim oInventoryGenExit As SAPbobsCOM.Documents



            If CBO_ESTADO_OT.Value.Equals("C") Then


                For n As Integer = 1 To MT_MATRIX.RowCount


                    iCantidad = CType(Me.MT_MATRIX.Columns.Item("Col_8").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    sEstadoOT = CType(Me.MT_MATRIX.Columns.Item("Col_1").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    sCodigoArticulo = CType(Me.MT_MATRIX.Columns.Item("Col_7").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    doSaldo = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    sCodigoCosto = CType(Me.MT_MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    sCC_CostoVentas69 = CType(Me.MT_MATRIX.Columns.Item("Col_9").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    sCC_AjusteVentas71 = CType(Me.MT_MATRIX.Columns.Item("Col_11").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    sCC_Diario23 = CType(Me.MT_MATRIX.Columns.Item("Col_16").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value


                    sDocNum = CType(Me.MT_MATRIX.Columns.Item("Col_14").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    dtFecha = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value


                    If CType(Me.MT_MATRIX.Columns.Item("Col_0").Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True Then

                        'OT cerrado
                        If sEstadoOT.Equals("CERRADO") Then

                            If iCantidad > 0.0 Then ' hay stock -> revalorizacion

                                If doSaldo > 0.0 Then ' revalorizacion contra la 71

                                    'Revalorizacion de inventario
                                    oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)

                                    oMaterialRevaluation.DocDate = util.formartDate(dtFecha)
                                    oMaterialRevaluation.TaxDate = util.formartDate(dtFecha)
                                    oMaterialRevaluation.JournalMemo = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo
                                    oMaterialRevaluation.Comments = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo

                                    oMaterialRevaluation.Reference2 = sDocNum

                                    oMaterialRevaluation.RevalType = "M"

                                    oMaterialRevaluation.Lines.ItemCode = sCodigoArticulo
                                    oMaterialRevaluation.Lines.DistributionRule3 = sCodigoCosto
                                    oMaterialRevaluation.Lines.Quantity = iCantidad
                                    oMaterialRevaluation.Lines.RevaluationIncrementAccount = sCC_AjusteVentas71
                                    oMaterialRevaluation.Lines.RevaluationDecrementAccount = sCC_AjusteVentas71
                                    oMaterialRevaluation.Lines.DebitCredit = doSaldo

                                    CodeMethod = oMaterialRevaluation.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else

                                        ' luego ' revalorizacion contra la 69 con signo cambiado

                                        'Revalorizacion de inventario
                                        oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)

                                        oMaterialRevaluation.DocDate = util.formartDate(dtFecha)
                                        oMaterialRevaluation.TaxDate = util.formartDate(dtFecha)
                                        oMaterialRevaluation.JournalMemo = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo
                                        oMaterialRevaluation.Comments = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo

                                        oMaterialRevaluation.Reference2 = sDocNum

                                        oMaterialRevaluation.RevalType = "M"

                                        oMaterialRevaluation.Lines.ItemCode = sCodigoArticulo
                                        oMaterialRevaluation.Lines.DistributionRule3 = sCodigoCosto
                                        oMaterialRevaluation.Lines.Quantity = iCantidad
                                        oMaterialRevaluation.Lines.RevaluationIncrementAccount = sCC_CostoVentas69
                                        oMaterialRevaluation.Lines.RevaluationDecrementAccount = sCC_CostoVentas69
                                        oMaterialRevaluation.Lines.DebitCredit = doSaldo * -1

                                        CodeMethod = oMaterialRevaluation.Add

                                        If CodeMethod <> 0 Then
                                            SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                            Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                        Else
                                            Application.SBO_Application.SetStatusBarMessage("Revalorización creada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                        End If

                                    End If

                                Else 'Saldo negativo

                                    'Revalorizacion de inventario
                                    oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)

                                    oMaterialRevaluation.DocDate = util.formartDate(dtFecha)
                                    oMaterialRevaluation.TaxDate = util.formartDate(dtFecha)
                                    oMaterialRevaluation.JournalMemo = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo
                                    oMaterialRevaluation.Comments = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo

                                    oMaterialRevaluation.Reference2 = sDocNum

                                    oMaterialRevaluation.RevalType = "M"

                                    oMaterialRevaluation.Lines.ItemCode = sCodigoArticulo
                                    oMaterialRevaluation.Lines.DistributionRule3 = sCodigoCosto
                                    oMaterialRevaluation.Lines.Quantity = 1
                                    oMaterialRevaluation.Lines.RevaluationIncrementAccount = sCC_CostoVentas69
                                    oMaterialRevaluation.Lines.RevaluationDecrementAccount = sCC_CostoVentas69
                                    oMaterialRevaluation.Lines.DebitCredit = doSaldo * -1

                                    CodeMethod = oMaterialRevaluation.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else

                                        'Revalorizacion de inventario
                                        oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)

                                        oMaterialRevaluation.DocDate = util.formartDate(dtFecha)
                                        oMaterialRevaluation.TaxDate = util.formartDate(dtFecha)
                                        oMaterialRevaluation.JournalMemo = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo
                                        oMaterialRevaluation.Comments = "REVALORIZACION DE INVENTARIO " + sCodigoArticulo

                                        oMaterialRevaluation.Reference2 = sDocNum

                                        oMaterialRevaluation.RevalType = "M"

                                        oMaterialRevaluation.Lines.ItemCode = sCodigoArticulo
                                        oMaterialRevaluation.Lines.DistributionRule3 = sCodigoCosto
                                        oMaterialRevaluation.Lines.Quantity = 1
                                        oMaterialRevaluation.Lines.RevaluationIncrementAccount = sCC_AjusteVentas71
                                        oMaterialRevaluation.Lines.RevaluationDecrementAccount = sCC_AjusteVentas71
                                        oMaterialRevaluation.Lines.DebitCredit = doSaldo

                                        CodeMethod = oMaterialRevaluation.Add
                                        If CodeMethod <> 0 Then
                                            SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                            Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                        Else
                                            Application.SBO_Application.SetStatusBarMessage("Revalorización creada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                        End If

                                    End If

                                End If

                            Else ' no hay stock - entrada y salida de 


                                If doSaldo > 0.0 Then
                                    ' entrada 71 y salida 69 / sCC_AjusteVentas '71 /sCC_CostoVentas '69

                                    oInventoryGenEntry = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry)


                                    oInventoryGenEntry.DocDate = util.formartDate(dtFecha)
                                    oInventoryGenEntry.TaxDate = util.formartDate(dtFecha)
                                    oInventoryGenEntry.JournalMemo = "ENTRADA DE MERCADERIA " + sCodigoArticulo
                                    oInventoryGenEntry.Reference2 = sDocNum
                                    oInventoryGenEntry.FolioPrefixString = "1"
                                    oInventoryGenEntry.FolioNumber = 1
                                    oInventoryGenEntry.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "02"

                                    oInventoryGenEntry.Lines.ItemCode = sCodigoArticulo
                                    oInventoryGenEntry.Lines.Quantity = 0.001
                                    oInventoryGenEntry.Lines.LineTotal = doSaldo
                                    oInventoryGenEntry.Lines.AccountCode = sCC_AjusteVentas71
                                    oInventoryGenEntry.Lines.CostingCode3 = sCodigoCosto

                                    CodeMethod = oInventoryGenEntry.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else

                                        'Salida de mercaderia 71 
                                        oInventoryGenExit = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit)

                                        oInventoryGenExit.DocDate = util.formartDate(dtFecha)
                                        oInventoryGenExit.TaxDate = util.formartDate(dtFecha)
                                        oInventoryGenExit.FolioPrefixString = "1"
                                        oInventoryGenExit.Reference2 = sDocNum
                                        oInventoryGenExit.FolioNumber = 1
                                        oInventoryGenExit.JournalMemo = "SALIDA DE MERCADERIA " + sCodigoArticulo
                                        oInventoryGenExit.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "91"
                                        oInventoryGenExit.UserFields.Fields.Item("U_EXD_MOVINVT").Value = "04"

                                        oInventoryGenExit.Lines.ItemCode = sCodigoArticulo
                                        oInventoryGenExit.Lines.Quantity = 0.001
                                        oInventoryGenExit.Lines.CostingCode3 = sCodigoCosto
                                        oInventoryGenExit.Lines.AccountCode = sCC_CostoVentas69 ' Nuevo

                                        CodeMethod = oInventoryGenExit.Add

                                        If CodeMethod <> 0 Then
                                            SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                            Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                        Else
                                            Application.SBO_Application.SetStatusBarMessage("Entrada y Salida de mercadería creada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                        End If


                                    End If

                                Else ' saldo negativo
                                    'entrada 69 y salida 71   / sCC_AjusteVentas '71 /sCC_CostoVentas '69

                                    oInventoryGenEntry = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry)


                                    oInventoryGenEntry.DocDate = util.formartDate(dtFecha)
                                    oInventoryGenEntry.TaxDate = util.formartDate(dtFecha)
                                    oInventoryGenEntry.JournalMemo = "ENTRADA DE MERCADERIA " + sCodigoArticulo
                                    oInventoryGenEntry.Reference2 = sDocNum
                                    oInventoryGenEntry.FolioPrefixString = "1"
                                    oInventoryGenEntry.FolioNumber = 1
                                    oInventoryGenEntry.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "02"

                                    oInventoryGenEntry.Lines.ItemCode = sCodigoArticulo
                                    oInventoryGenEntry.Lines.Quantity = 0.001
                                    oInventoryGenEntry.Lines.LineTotal = doSaldo * -1
                                    oInventoryGenEntry.Lines.AccountCode = sCC_CostoVentas69
                                    oInventoryGenEntry.Lines.CostingCode3 = sCodigoCosto

                                    CodeMethod = oInventoryGenEntry.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else

                                        'Salida de mercaderia 71
                                        oInventoryGenExit = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit)

                                        oInventoryGenExit.DocDate = util.formartDate(dtFecha)
                                        oInventoryGenExit.TaxDate = util.formartDate(dtFecha)
                                        oInventoryGenExit.FolioPrefixString = "1"
                                        oInventoryGenExit.Reference2 = sDocNum
                                        oInventoryGenExit.FolioNumber = 1
                                        oInventoryGenExit.JournalMemo = "SALIDA DE MERCADERIA " + sCodigoArticulo
                                        oInventoryGenExit.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "91"
                                        oInventoryGenExit.UserFields.Fields.Item("U_EXD_MOVINVT").Value = "04"

                                        oInventoryGenExit.Lines.ItemCode = sCodigoArticulo
                                        oInventoryGenExit.Lines.Quantity = 0.001
                                        oInventoryGenExit.Lines.CostingCode3 = sCodigoCosto
                                        oInventoryGenExit.Lines.AccountCode = sCC_AjusteVentas71 ' Nuevo

                                        CodeMethod = oInventoryGenExit.Add

                                        If CodeMethod <> 0 Then
                                            SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                            Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                        Else
                                            Application.SBO_Application.SetStatusBarMessage("Entrada y Salida de mercadería creada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                        End If
                                    End If
                                End If
                            End If
                        Else  'OT ABIERTO


                            'SOLO DEBE HABER UN ASIENTO 

                        End If


                    End If   ' end valida check 

                Next ' end for grilla

            Else ' P

                'If doSaldo < 0.0 Then
                '    doSaldo = doSaldo * -1
                'End If



                Dim currentDate As DateTime
                Dim unidadNegocio As String

                ' Get the current date from the textbox
                currentDate = Convert.ToDateTime(util.formartDate(DT_FECHA_FIN.Value))
                ' Add one day
                currentDate = currentDate.AddDays(1)

                'asinto 
                oJournalEntries = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)

                oJournalEntries.ReferenceDate = util.formartDate(DT_FECHA_FIN.Value)
                oJournalEntries.DueDate = util.formartDate(DT_FECHA_FIN.Value)
                oJournalEntries.TaxDate = util.formartDate(DT_FECHA_FIN.Value)
                oJournalEntries.Memo = "CIERRE DE COSTO REAL DE OT EN PROCESO"

                oJournalEntries.UseAutoStorno = SAPbobsCOM.BoYesNoEnum.tYES

                oJournalEntries.StornoDate = currentDate ' util.formartDate(DT_FECHA_FIN.Value)


                'oJournalEntries.Reference = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                oJournalEntries.TransactionCode = "PPC"
                oJournalEntries.UserFields.Fields.Item("U_EXX_TIPCON").Value = "1"

                For x As Integer = 1 To MT_MATRIX.RowCount

                    If CType(Me.MT_MATRIX.Columns.Item("Col_0").Cells.Item(x).Specific, SAPbouiCOM.CheckBox).Checked = True Then

                        iCantOT_O += 1

                        'sCC_CostoVentas69 = CType(Me.MT_MATRIX.Columns.Item("Col_9").Cells.Item(x).Specific, SAPbouiCOM.EditText).Value
                        sCC_AjusteVentas71 = CType(Me.MT_MATRIX.Columns.Item("Col_11").Cells.Item(x).Specific, SAPbouiCOM.EditText).Value
                        sCodigoCosto = CType(Me.MT_MATRIX.Columns.Item("Col_5").Cells.Item(x).Specific, SAPbouiCOM.EditText).Value
                        sCC_Diario23 = CType(Me.MT_MATRIX.Columns.Item("Col_16").Cells.Item(x).Specific, SAPbouiCOM.EditText).Value
                        doSaldo = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(x).Specific, SAPbouiCOM.EditText).Value

                        unidadNegocio = CType(Me.MT_MATRIX.Columns.Item("Col_18").Cells.Item(x).Specific, SAPbouiCOM.EditText).Value

                        If doSaldo > 0 Then
                            oJournalEntries.Lines.AccountCode = sCC_AjusteVentas71
                            oJournalEntries.Lines.CostingCode3 = sCodigoCosto
                            oJournalEntries.Lines.Credit = doSaldo
                            oJournalEntries.Lines.CostingCode = unidadNegocio

                            oJournalEntries.Lines.Add()

                            oJournalEntries.Lines.AccountCode = sCC_Diario23
                            oJournalEntries.Lines.CostingCode3 = sCodigoCosto
                            oJournalEntries.Lines.CostingCode = unidadNegocio
                            oJournalEntries.Lines.Debit = doSaldo

                        Else

                            oJournalEntries.Lines.AccountCode = sCC_AjusteVentas71
                            oJournalEntries.Lines.CostingCode3 = sCodigoCosto
                            oJournalEntries.Lines.CostingCode = unidadNegocio
                            oJournalEntries.Lines.Debit = doSaldo * -1

                            oJournalEntries.Lines.Add()
                            oJournalEntries.Lines.AccountCode = sCC_Diario23
                            oJournalEntries.Lines.CostingCode3 = sCodigoCosto
                            oJournalEntries.Lines.CostingCode = unidadNegocio
                            oJournalEntries.Lines.Credit = doSaldo * -1


                        End If

                        oJournalEntries.Lines.Add()


                    End If
                Next


                If iCantOT_O > 0 Then
                    CodeMethod = oJournalEntries.Add

                    If CodeMethod <> 0 Then
                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                        Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                    Else

                        Application.SBO_Application.SetStatusBarMessage("Asiento creado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, False)

                        Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
                        Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()
                        Dim stEstadoOT As String = CBO_ESTADO_OT.Value
                        Dim stProceso As String = DT_PROCESO.Value

                        cargarMatrix(stFecha_Inicio, stFecha_Fin, stEstadoOT, stProceso)

                    End If

                Else
                    Application.SBO_Application.SetStatusBarMessage("debe seleccionar al menos una fila", SAPbouiCOM.BoMessageTime.bmt_Short, True)
                End If

            End If

        End Sub

        Private Sub BTN_SELECCIONAR_TODO_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BTN_SELECCIONAR_TODO.ClickBefore
            'Throw New System.NotImplementedException()
            For n As Integer = 1 To Me.MT_MATRIX.RowCount
                CType(Me.MT_MATRIX.Columns.Item(1).Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True
            Next
        End Sub

        Private Sub BTN_DESMARCAR_TODO_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BTN_DESMARCAR_TODO.ClickBefore
            'Throw New System.NotImplementedException()
            For n As Integer = 1 To Me.MT_MATRIX.RowCount
                CType(Me.MT_MATRIX.Columns.Item(1).Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = False
            Next
        End Sub

        Private WithEvents objEvento As SAPbouiCOM.SBOItemEventArg
        Private WithEvents objDataTable As SAPbouiCOM.DataTable
        Dim val0 As String = ""
        Dim segmento As String = ""


        Private Sub MT_MATRIX_ChooseFromListAfter(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg) Handles MT_MATRIX.ChooseFromListAfter
            'Throw New System.NotImplementedException()


            If pVal.ColUID = "Col_10" Then

                objEvento = pVal
                objDataTable = objEvento.SelectedObjects


                Try
                    If objDataTable Is Nothing Then
                    Else
                        val0 = System.Convert.ToString(objDataTable.GetValue("AcctCode", 0))
                        MT_MATRIX.Columns.Item("Col_9").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try

                Try
                    If objDataTable Is Nothing Then
                    Else
                        val0 = System.Convert.ToString(objDataTable.GetValue("FormatCode", 0))
                        MT_MATRIX.Columns.Item("Col_10").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try


            End If



            If pVal.ColUID = "Col_12" Then

                objEvento = pVal
                objDataTable = objEvento.SelectedObjects

                Try
                    If objDataTable Is Nothing Then
                    Else
                        val0 = System.Convert.ToString(objDataTable.GetValue("AcctCode", 0))
                        MT_MATRIX.Columns.Item("Col_11").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try

                Try
                    If objDataTable Is Nothing Then
                    Else
                        val0 = System.Convert.ToString(objDataTable.GetValue("FormatCode", 0))
                        MT_MATRIX.Columns.Item("Col_12").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try

            End If


            If pVal.ColUID = "Col_15" Then

                objEvento = pVal
                objDataTable = objEvento.SelectedObjects

                Try
                    If objDataTable Is Nothing Then
                    Else
                        val0 = System.Convert.ToString(objDataTable.GetValue("AcctCode", 0))
                        MT_MATRIX.Columns.Item("Col_16").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try

                Try
                    If objDataTable Is Nothing Then
                    Else
                        val0 = System.Convert.ToString(objDataTable.GetValue("FormatCode", 0))
                        MT_MATRIX.Columns.Item("Col_15").Cells.Item(pVal.Row).Specific.Value = val0.ToString

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try

            End If

        End Sub


        Private WithEvents StaticText3 As SAPbouiCOM.StaticText
        Public Overrides Sub OnInitializeFormEvents()

        End Sub
        Private WithEvents CBO_ESTADO_OT As SAPbouiCOM.ComboBox

        Private Sub BTN_PROCESO_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BTN_PROCESO.ClickBefore
            'Throw New System.NotImplementedException()
            Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
            Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()
            Dim stEstadoOT As String = CBO_ESTADO_OT.Value

            'If stFecha_Inicio.Equals("") Then
            '    Application.SBO_Application.SetStatusBarMessage("Ingrese Fecha Inicio", SAPbouiCOM.BoMessageTime.bmt_Short, True)
            '    Return
            'End If


            'If stFecha_Fin.Equals("") Then
            '    Application.SBO_Application.SetStatusBarMessage("Ingrese Fecha Fin", SAPbouiCOM.BoMessageTime.bmt_Short, True)
            '    Return
            'End If

            'GC.Collect()

            'Dim mquery As String = ""

            'Try
            '    If ConexionSAP = TipoConexion.Sql Then

            '        mquery = SP_SQL_EXD_PROCESO_SGTE & "'" & stFecha_Inicio & "'"
            '    ElseIf ConexionSAP = TipoConexion.Hana Then
            '        mquery = "CALL " + SP_SQL_EXD_PROCESO_SGTE & "('" & stFecha_Inicio & "')"
            '    End If
            'Catch ex As Exception
            '    Application.SBO_Application.SetStatusBarMessage("Error al crear :" + mquery + " ,Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
            '    Return
            'End Try

            'Dim oRecordSet As SAPbobsCOM.Recordset = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'oRecordSet.DoQuery(mquery)
            'If oRecordSet.RecordCount = 0 Then
            '    Application.SBO_Application.SetStatusBarMessage("No existen procesos creados", SAPbouiCOM.BoMessageTime.bmt_Short, True)
            '    Return
            'Else
            '    oRecordSet.MoveFirst()
            '    Me.DT_PROCESO.Value = oRecordSet.Fields(0).Value.ToString()
            'End If

        End Sub
    End Class
End Namespace
