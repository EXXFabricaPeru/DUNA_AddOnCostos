Option Strict Off
Option Explicit On

Imports SAPbouiCOM.Framework
Imports System.Globalization
Imports SAPbobsCOM
Imports SAPbouiCOM

'07.12 Producción - Analsis de ordenes de fabricación pendientes pro revalorizar

Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_AjusteCtaWIP", "EXD_AjusteCtaWIP.b1f")>
    Friend Class EXD_AjusteCtaWIP
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private WithEvents SBO_Company As SAPbobsCOM.Company
        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        Dim util As New Util


        ReadOnly SP_SQL_EXD_ORDENES_POR_REVALORIZAR As String = "EXD_ORDENES_POR_REVALORIZAR "

        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)
            UIAPIRawForm.Title = "EXD: AJUSTE DE CUENTAS WIP"

            SBO_Application = pAplicacion
            SBO_Company = pCompany
            SboGuiApi = New SAPbouiCOM.SboGuiApi


            DT_FECHA_INICIO.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_FIN.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_DOCUMENTO.String = Now.Date.Date.ToString("ddMMyyyy")

            Me.UIAPIRawForm.Left = (SBO_Application.Desktop.Width / 2) - (UIAPIRawForm.Width / 2)
            Me.UIAPIRawForm.Top = (SBO_Application.Desktop.Height / 2) - ((UIAPIRawForm.Height / 2) + 60)

        End Sub

        Public Overrides Sub OnInitializeComponent()
            Me.StaticText0 = CType(Me.GetItem("Item_0").Specific, SAPbouiCOM.StaticText)
            Me.StaticText1 = CType(Me.GetItem("Item_1").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_INICIO = CType(Me.GetItem("F1").Specific, SAPbouiCOM.EditText)
            Me.StaticText2 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("F2").Specific, SAPbouiCOM.EditText)
            Me.MT_MATRIX = CType(Me.GetItem("MTX").Specific, SAPbouiCOM.Matrix)
            Me.BT_BUSCAR = CType(Me.GetItem("BT1").Specific, SAPbouiCOM.Button)
            Me.BTN_GRABAR = CType(Me.GetItem("BT2").Specific, SAPbouiCOM.Button)
            Me.StaticText3 = CType(Me.GetItem("Item_8").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_DOCUMENTO = CType(Me.GetItem("F3").Specific, SAPbouiCOM.EditText)
            Me.BT_SELECCIONAR_TODO = CType(Me.GetItem("B3").Specific, SAPbouiCOM.Button)
            Me.BT_DESMARCAR_TODO = CType(Me.GetItem("B4").Specific, SAPbouiCOM.Button)
            Me.LBL_RESULTADO = CType(Me.GetItem("LBL_R").Specific, SAPbouiCOM.StaticText)
            Me.OnCustomInitialize()

        End Sub

        Public Overrides Sub OnInitializeFormEvents()

        End Sub


        Private Sub OnCustomInitialize()

        End Sub
        Private WithEvents StaticText0 As SAPbouiCOM.StaticText
        Private WithEvents StaticText1 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_INICIO As SAPbouiCOM.EditText
        Private WithEvents StaticText2 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_FIN As SAPbouiCOM.EditText
        Private WithEvents MT_MATRIX As SAPbouiCOM.Matrix
        Private WithEvents BT_BUSCAR As SAPbouiCOM.Button
        Private WithEvents BTN_GRABAR As SAPbouiCOM.Button
        Private WithEvents StaticText3 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_DOCUMENTO As SAPbouiCOM.EditText
        Private WithEvents BT_SELECCIONAR_TODO As SAPbouiCOM.Button
        Private WithEvents BT_DESMARCAR_TODO As SAPbouiCOM.Button

        Private Sub BT_BUSCAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_BUSCAR.ClickBefore
            'Throw New System.NotImplementedException()

            Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
            Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()


            cargarMatrix(stFecha_Inicio, stFecha_Fin, pVal)

        End Sub

        Sub cargarMatrix(ByVal sF1 As String, ByVal SF2 As String, ByVal pVal As SAPbouiCOM.SBOItemEventArg)

            GC.Collect()

            Dim mquery As String = ""

            Try
                If ConexionSAP = TipoConexion.Sql Then

                    mquery = SP_SQL_EXD_ORDENES_POR_REVALORIZAR & "'" & sF1 & "','" & SF2 & "'"
                ElseIf ConexionSAP = TipoConexion.Hana Then
                    mquery = "CALL " + SP_SQL_EXD_ORDENES_POR_REVALORIZAR + "('" + sF1 + "'" + "," + "'" + SF2 + "')"
                End If
            Catch ex As Exception
                SBO_Application.SetStatusBarMessage("Error al crear :" + mquery + " ,Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_0").ExecuteQuery(mquery)
            Catch ex As Exception
                SBO_Application.SetStatusBarMessage("Error al cargar DT_0 " + ",Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            Try
                'MT_MATRIX.Columns.Item("#").DataBind.Bind("DT_0", "TransId")
                'MT_MATRIX.Columns.Item("#").Visible = False

                MT_MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_0", "Seleccionar")
                MT_MATRIX.Columns.Item("Col_0").ValOn = "0"

                MT_MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_0", "Nro OT")
                MT_MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_0", "Codigo Articulo")
                MT_MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_0", "Nombre")
                MT_MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_0", "Centro de Costo")
                MT_MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_0", "Nombre Centro de Costo")
                MT_MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_0", "Fecha Revalorizacion")
                MT_MATRIX.Columns.Item("Col_7").DataBind.Bind("DT_0", "Cuenta")
                MT_MATRIX.Columns.Item("Col_8").DataBind.Bind("DT_0", "AcctName")
                MT_MATRIX.Columns.Item("Col_9").DataBind.Bind("DT_0", "Saldo")
                MT_MATRIX.Columns.Item("Col_10").DataBind.Bind("DT_0", "Account")
                MT_MATRIX.Columns.Item("Col_11").DataBind.Bind("DT_0", "Cantidad")
                MT_MATRIX.Columns.Item("Col_12").DataBind.Bind("DT_0", "DocEntry")
                MT_MATRIX.Columns.Item("Col_13").DataBind.Bind("DT_0", "OcrCode")
                MT_MATRIX.Columns.Item("Col_14").DataBind.Bind("DT_0", "SYS Ajuste de Ventas")
                'nuevo 202203
                'MT_MATRIX.Columns.Item("Col_15").DataBind.Bind("DT_0", "OcrCode2")
                'MT_MATRIX.Columns.Item("Col_16").DataBind.Bind("DT_0", "OcrCode4")
                'MT_MATRIX.Columns.Item("Col_17").DataBind.Bind("DT_0", "OcrCode5")

                MT_MATRIX.Columns.Item("Col_15").Visible = False
                MT_MATRIX.Columns.Item("Col_16").Visible = False
                MT_MATRIX.Columns.Item("Col_17").Visible = False


                MT_MATRIX.LoadFromDataSource()


                'Dim oEdit As SAPbouiCOM.Matrix

                'For columnindex = 1 To MT_MATRIX.Columns.Count
                '    oEdit = MT_MATRIX.Columns.Item(columnindex).Cells.Item("Col_1").Specific
                '    oEdit.ForeColor = System.Drawing.Color.Blue.ToArgb

                'Next

                LBL_RESULTADO.Caption = "(" + MT_MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"

            Catch ex As Exception
                SBO_Application.SetStatusBarMessage("Error al cargar Matrix " + ",Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            GC.Collect()
        End Sub

        Private Sub BTN_GRABAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BTN_GRABAR.ClickBefore
            'Throw New System.NotImplementedException()

            Dim CodeMethod As Integer = 0
            Dim ErrorCode As Integer = 0
            Dim ErrorMsg As String = ""

            Dim oMaterialRevaluation As SAPbobsCOM.MaterialRevaluation
            Dim oInventoryGenEntry As SAPbobsCOM.Documents
            Dim oInventoryGenExit As SAPbobsCOM.Documents

            Dim NroOT As Integer = 0
            Dim Codigo_Articulo As String = ""
            Dim Nombre As String = ""
            Dim Centro_Costo As String = ""
            Dim Nombre_Centrode_Costo As String = ""
            Dim Fecha_Revalorizacion As String = ""
            Dim Cuenta As String = ""
            Dim AcctName As String = ""
            Dim Saldo As Double = 0
            Dim Account71 As String = ""
            Dim Account69 As String = ""
            Dim Cantidad As Double = 0
            Dim unidadNegocio As String = ""
            'nuevo 202203
            'Dim Dimension2 As String = ""
            'Dim Dimension4 As String = ""
            'Dim Dimension5 As String = ""



            Dim fecha_documento As String = DT_FECHA_DOCUMENTO.Value

            Try
                For n As Integer = 1 To Me.MT_MATRIX.RowCount

                    'Procesamos solo las lineas marcadas
                    If CType(Me.MT_MATRIX.Columns.Item("Col_0").Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True Then

                   
                        NroOT = CType(Me.MT_MATRIX.Columns.Item("Col_1").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Codigo_Articulo = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Nombre = CType(Me.MT_MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Centro_Costo = CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Nombre_Centrode_Costo = CType(Me.MT_MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Fecha_Revalorizacion = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Cuenta = CType(Me.MT_MATRIX.Columns.Item("Col_7").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        AcctName = CType(Me.MT_MATRIX.Columns.Item("Col_8").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Saldo = CType(Me.MT_MATRIX.Columns.Item("Col_9").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Account71 = CType(Me.MT_MATRIX.Columns.Item("Col_10").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Account69 = CType(Me.MT_MATRIX.Columns.Item("Col_14").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        Cantidad = CType(Me.MT_MATRIX.Columns.Item("Col_11").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        unidadNegocio = CType(Me.MT_MATRIX.Columns.Item("Col_13").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        'nuevo 202203
                        'Dimension2 = CType(Me.MT_MATRIX.Columns.Item("Col_15").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        'Dimension4 = CType(Me.MT_MATRIX.Columns.Item("Col_16").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                        'Dimension5 = CType(Me.MT_MATRIX.Columns.Item("Col_17").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                        'Validacion de stock (saldo inventario)

                        'If Saldo < 0.0 Then
                        '    Saldo = Saldo * -1
                        'End If

                        'Debemos validar la cantidad para saber el iventario 
                        'Saldo >


                        If Cantidad > 0.0 Then ' hay stock

                            If Saldo > 0 Then

                                'Revalorizacion de inventario
                                oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)

                                oMaterialRevaluation.DocDate = util.formartDate(Fecha_Revalorizacion)
                                oMaterialRevaluation.TaxDate = util.formartDate(Fecha_Revalorizacion)

                                oMaterialRevaluation.Reference2 = NroOT
                                oMaterialRevaluation.RevalType = "M"
                                oMaterialRevaluation.Comments = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()
                                oMaterialRevaluation.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                oMaterialRevaluation.Lines.ItemCode = Codigo_Articulo
                                'oMaterialRevaluation.Lines.WarehouseCode = Centro_Costo
                                oMaterialRevaluation.Lines.Quantity = Cantidad
                                oMaterialRevaluation.Lines.DistributionRule = unidadNegocio
                                oMaterialRevaluation.Lines.RevaluationIncrementAccount = Account71
                                oMaterialRevaluation.Lines.RevaluationDecrementAccount = Account71
                                oMaterialRevaluation.Lines.DistributionRule3 = Centro_Costo
                                oMaterialRevaluation.Lines.DebitCredit = Saldo
                                'nuevo 202203
                                'If Dimension2 <> "" Then
                                '    oMaterialRevaluation.Lines.DistributionRule2 = Dimension2
                                'End If
                                'If Dimension4 <> "" Then
                                '    oMaterialRevaluation.Lines.DistributionRule4 = Dimension4
                                'End If
                                'If Dimension5 <> "" Then
                                '    oMaterialRevaluation.Lines.DistributionRule5 = Dimension5
                                'End If

                                CodeMethod = oMaterialRevaluation.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else


                                        'Revalorizacion de inventario
                                        oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)


                                        oMaterialRevaluation.DocDate = util.formartDate(Fecha_Revalorizacion)
                                        oMaterialRevaluation.TaxDate = util.formartDate(Fecha_Revalorizacion)

                                        oMaterialRevaluation.Reference2 = NroOT
                                        oMaterialRevaluation.RevalType = "M"
                                        oMaterialRevaluation.Comments = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()
                                        oMaterialRevaluation.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                        oMaterialRevaluation.Lines.ItemCode = Codigo_Articulo
                                        'oMaterialRevaluation.Lines.WarehouseCode = Centro_Costo
                                        oMaterialRevaluation.Lines.Quantity = Cantidad
                                        oMaterialRevaluation.Lines.DistributionRule = unidadNegocio
                                        oMaterialRevaluation.Lines.RevaluationIncrementAccount = Account69
                                        oMaterialRevaluation.Lines.RevaluationDecrementAccount = Account69
                                        oMaterialRevaluation.Lines.DistributionRule3 = Centro_Costo
                                        oMaterialRevaluation.Lines.DebitCredit = Saldo * -1
                                    'nuevo 202203
                                    'nuevo 202203
                                    'If Dimension2 <> "" Then
                                    '    oMaterialRevaluation.Lines.DistributionRule2 = Dimension2
                                    'End If
                                    'If Dimension4 <> "" Then
                                    '    oMaterialRevaluation.Lines.DistributionRule4 = Dimension4
                                    'End If
                                    'If Dimension5 <> "" Then
                                    '    oMaterialRevaluation.Lines.DistributionRule5 = Dimension5
                                    'End If

                                    CodeMethod = oMaterialRevaluation.Add


                                        If CodeMethod <> 0 Then
                                            SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                            SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                        Else
                                            SBO_Application.SetStatusBarMessage("Revalorizaciones creadas con exito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                        End If
                                    End If




                                Else ' SALDO NEGATIVO

                                    'Revalorizacion de inventario
                                    oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)


                                oMaterialRevaluation.DocDate = util.formartDate(Fecha_Revalorizacion)
                                oMaterialRevaluation.TaxDate = util.formartDate(Fecha_Revalorizacion)

                                oMaterialRevaluation.Reference2 = NroOT
                                oMaterialRevaluation.RevalType = "M"
                                oMaterialRevaluation.Comments = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()
                                oMaterialRevaluation.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                oMaterialRevaluation.Lines.ItemCode = Codigo_Articulo
                                'oMaterialRevaluation.Lines.WarehouseCode = Centro_Costo
                                oMaterialRevaluation.Lines.Quantity = Cantidad
                                oMaterialRevaluation.Lines.DistributionRule = unidadNegocio
                                oMaterialRevaluation.Lines.RevaluationIncrementAccount = Account69
                                oMaterialRevaluation.Lines.RevaluationDecrementAccount = Account69

                                oMaterialRevaluation.Lines.DistributionRule3 = Centro_Costo
                                oMaterialRevaluation.Lines.DebitCredit = Saldo * -1
                                'nuevo 202203
                                'If Dimension2 <> "" Then
                                '    oMaterialRevaluation.Lines.DistributionRule2 = Dimension2
                                'End If
                                'If Dimension4 <> "" Then
                                '    oMaterialRevaluation.Lines.DistributionRule4 = Dimension4
                                'End If
                                'If Dimension5 <> "" Then
                                '    oMaterialRevaluation.Lines.DistributionRule5 = Dimension5
                                'End If

                                CodeMethod = oMaterialRevaluation.Add

                                If CodeMethod <> 0 Then
                                    SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                    SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                Else

                                    'Revalorizacion de inventario
                                    oMaterialRevaluation = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oMaterialRevaluation)


                                    oMaterialRevaluation.DocDate = util.formartDate(Fecha_Revalorizacion)
                                    oMaterialRevaluation.TaxDate = util.formartDate(Fecha_Revalorizacion)

                                    oMaterialRevaluation.Reference2 = NroOT
                                    oMaterialRevaluation.RevalType = "M"
                                    oMaterialRevaluation.Comments = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()
                                    oMaterialRevaluation.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                    oMaterialRevaluation.Lines.ItemCode = Codigo_Articulo
                                    'oMaterialRevaluation.Lines.WarehouseCode = Centro_Costo
                                    oMaterialRevaluation.Lines.Quantity = Cantidad
                                    oMaterialRevaluation.Lines.DistributionRule = unidadNegocio
                                    oMaterialRevaluation.Lines.RevaluationIncrementAccount = Account71
                                    oMaterialRevaluation.Lines.RevaluationDecrementAccount = Account71
                                    oMaterialRevaluation.Lines.DistributionRule3 = Centro_Costo
                                    oMaterialRevaluation.Lines.DebitCredit = Saldo
                                    'nuevo 202203
                                    'If Dimension2 <> "" Then
                                    '    oMaterialRevaluation.Lines.DistributionRule2 = Dimension2
                                    'End If
                                    'If Dimension4 <> "" Then
                                    '    oMaterialRevaluation.Lines.DistributionRule4 = Dimension4
                                    'End If
                                    'If Dimension5 <> "" Then
                                    '    oMaterialRevaluation.Lines.DistributionRule5 = Dimension5
                                    'End If

                                    CodeMethod = oMaterialRevaluation.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else
                                        SBO_Application.SetStatusBarMessage("Revalorizaciones creadas con exito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                    End If
                                End If

                            End If

                        Else ' no hay stock


                            If Saldo < 0 Then

                                If Saldo < 0 Then
                                    Saldo = Saldo * -1
                                End If
                                'Entrada de mercaderia
                                oInventoryGenEntry = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry)

                                oInventoryGenEntry.DocDate = util.formartDate(Fecha_Revalorizacion)
                                oInventoryGenEntry.TaxDate = util.formartDate(Fecha_Revalorizacion)
                                oInventoryGenEntry.Reference2 = NroOT
                                oInventoryGenEntry.FolioPrefixString = "1"
                                oInventoryGenEntry.FolioNumber = 1
                                oInventoryGenEntry.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "02"
                                oInventoryGenEntry.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()
                                oInventoryGenEntry.Comments = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                oInventoryGenEntry.Lines.ItemCode = Codigo_Articulo

                                oInventoryGenEntry.Lines.Quantity = 0.001
                                oInventoryGenEntry.Lines.CostingCode = unidadNegocio
                                oInventoryGenEntry.Lines.LineTotal = Saldo
                                oInventoryGenEntry.Lines.AccountCode = Account69
                                oInventoryGenEntry.Lines.CostingCode3 = Centro_Costo
                                'nuevo 202203
                                'If Dimension2 <> "" Then
                                '    oInventoryGenEntry.Lines.CostingCode2 = Dimension2
                                'End If
                                'If Dimension4 <> "" Then
                                '    oInventoryGenEntry.Lines.CostingCode4 = Dimension4
                                'End If
                                'If Dimension5 <> "" Then
                                '    oInventoryGenEntry.Lines.CostingCode5 = Dimension5
                                'End If


                                CodeMethod = oInventoryGenEntry.Add

                                If CodeMethod <> 0 Then
                                    SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                    SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                Else



                                    'Salida de mercaderia
                                    oInventoryGenExit = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit)

                                    oInventoryGenExit.DocDate = util.formartDate(Fecha_Revalorizacion)
                                    oInventoryGenExit.TaxDate = util.formartDate(Fecha_Revalorizacion)
                                    oInventoryGenExit.Reference2 = NroOT
                                    oInventoryGenExit.FolioPrefixString = "1"
                                    oInventoryGenExit.FolioNumber = 1
                                    oInventoryGenExit.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "91"
                                    oInventoryGenExit.UserFields.Fields.Item("U_EXD_MOVINVT").Value = "04"
                                    oInventoryGenExit.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                    oInventoryGenExit.Lines.ItemCode = Codigo_Articulo
                                    oInventoryGenExit.Lines.Quantity = 0.001
                                    oInventoryGenExit.Lines.CostingCode = unidadNegocio
                                    oInventoryGenExit.Lines.LineTotal = Saldo
                                    oInventoryGenExit.Lines.CostingCode3 = Centro_Costo
                                    oInventoryGenExit.Lines.AccountCode = Account71
                                    'nuevo 202203
                                    'If Dimension2 <> "" Then
                                    '    oInventoryGenExit.Lines.CostingCode2 = Dimension2
                                    'End If
                                    'If Dimension4 <> "" Then
                                    '    oInventoryGenExit.Lines.CostingCode4 = Dimension4
                                    'End If
                                    'If Dimension5 <> "" Then
                                    '    oInventoryGenExit.Lines.CostingCode5 = Dimension5
                                    'End If

                                    CodeMethod = oInventoryGenExit.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else
                                        SBO_Application.SetStatusBarMessage("Entrada y salida de mercaderia creado con exito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                    End If

                                End If

                            Else



                                If Saldo < 0 Then
                                    Saldo = Saldo * -1
                                End If


                                'Entrada de mercaderia
                                oInventoryGenEntry = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry)

                                oInventoryGenEntry.DocDate = util.formartDate(Fecha_Revalorizacion)
                                oInventoryGenEntry.TaxDate = util.formartDate(Fecha_Revalorizacion)
                                oInventoryGenEntry.Reference2 = NroOT
                                oInventoryGenEntry.FolioPrefixString = "1"
                                oInventoryGenEntry.FolioNumber = 1
                                oInventoryGenEntry.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "02"
                                oInventoryGenEntry.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()
                                oInventoryGenEntry.Comments = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                oInventoryGenEntry.Lines.ItemCode = Codigo_Articulo

                                oInventoryGenEntry.Lines.Quantity = 0.001
                                oInventoryGenEntry.Lines.CostingCode = unidadNegocio
                                oInventoryGenEntry.Lines.LineTotal = Saldo
                                oInventoryGenEntry.Lines.AccountCode = Account71
                                oInventoryGenEntry.Lines.CostingCode3 = Centro_Costo
                                'nuevo 202203
                                'If Dimension2 <> "" Then
                                '    oInventoryGenEntry.Lines.CostingCode2 = Dimension2
                                'End If
                                'If Dimension4 <> "" Then
                                '    oInventoryGenEntry.Lines.CostingCode4 = Dimension4
                                'End If
                                'If Dimension5 <> "" Then
                                '    oInventoryGenEntry.Lines.CostingCode5 = Dimension5
                                'End If

                                CodeMethod = oInventoryGenEntry.Add

                                If CodeMethod <> 0 Then
                                    SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                    SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                Else

                                    'Salida de mercaderia
                                    oInventoryGenExit = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit)

                                    oInventoryGenExit.DocDate = util.formartDate(Fecha_Revalorizacion)
                                    oInventoryGenExit.TaxDate = util.formartDate(Fecha_Revalorizacion)
                                    oInventoryGenExit.Reference2 = NroOT
                                    oInventoryGenExit.FolioPrefixString = "1"
                                    oInventoryGenExit.FolioNumber = 1
                                    oInventoryGenExit.UserFields.Fields.Item("U_EXX_TIPOOPER").Value = "91"
                                    oInventoryGenExit.UserFields.Fields.Item("U_EXD_MOVINVT").Value = "04"
                                    oInventoryGenExit.JournalMemo = "AJUSTE DE CUENTAS WIP, OT: " + NroOT.ToString()

                                    oInventoryGenExit.Lines.ItemCode = Codigo_Articulo
                                    oInventoryGenExit.Lines.Quantity = 0.001
                                    oInventoryGenExit.Lines.CostingCode = unidadNegocio
                                    oInventoryGenExit.Lines.LineTotal = Saldo
                                    oInventoryGenExit.Lines.CostingCode3 = Centro_Costo
                                    oInventoryGenExit.Lines.AccountCode = Account69
                                    'nuevo 202203
                                    'If Dimension2 <> "" Then
                                    '    oInventoryGenExit.Lines.CostingCode2 = Dimension2
                                    'End If
                                    'If Dimension4 <> "" Then
                                    '    oInventoryGenExit.Lines.CostingCode4 = Dimension4
                                    'End If
                                    'If Dimension5 <> "" Then
                                    '    oInventoryGenExit.Lines.CostingCode5 = Dimension5
                                    'End If

                                    CodeMethod = oInventoryGenExit.Add

                                    If CodeMethod <> 0 Then
                                        SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                                        SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                                    Else
                                        SBO_Application.SetStatusBarMessage("Entrada y salida de mercaderia creado con exito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                                    End If

                                End If

                            End If



                        End If
                    End If
                Next

                Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
                Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()

                cargarMatrix(stFecha_Inicio, stFecha_Fin, pVal)


            Catch ex As Exception
                SBO_Application.SetStatusBarMessage(ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
            End Try
        End Sub

   
        
        Private Sub BT_SELECCIONAR_TODO_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BT_SELECCIONAR_TODO.ClickBefore
            'Throw New System.NotImplementedException()
            For n As Integer = 1 To Me.MT_MATRIX.RowCount
                CType(Me.MT_MATRIX.Columns.Item(1).Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True
            Next
        End Sub

        Private Sub BT_DESMARCAR_TODO_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BT_DESMARCAR_TODO.ClickBefore
            'Throw New System.NotImplementedException()
            For n As Integer = 1 To Me.MT_MATRIX.RowCount
                CType(Me.MT_MATRIX.Columns.Item(1).Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = False
            Next

        End Sub
        Private WithEvents LBL_RESULTADO As SAPbouiCOM.StaticText

        'Private Sub MT_MATRIX_LinkPressedBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles MT_MATRIX.LinkPressedBefore
        '    'Throw New System.NotImplementedException()

        '    If pVal.EventType = SBO_Application.BoEventTypes.et_MATRIX_LINK_PRESSED And pVal.ColUID = "Col_1" And pVal.BeforeAction = True Then

        '        Dim oRecordSet As SAPbobsCOM.Recordset

        '        oRecordSet.DoQuery("SELECT DocEntry FROM OWOR WHERE DocNum=" &
        '      MT_MATRIX.Items.Item(pVal.ItemUID).Specific.Columns(pVal.ColUID).Cells.Item(pVal.Row).Specific.Value)

        '        MT_MATRIX.Items.Item("txtOpch").Specific.Value = oRecordSet.Fields("DocEntry").Value
        '        MT_MATRIX.Items.Item("lnkOpch").Click()

        '    End If
        'End Sub
    End Class
End Namespace
