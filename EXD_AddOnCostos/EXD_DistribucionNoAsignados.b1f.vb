Option Strict Off
Option Explicit On

Imports SAPbouiCOM.Framework
Imports System.Globalization
Imports System.Threading
Imports System.Threading.Tasks
Imports SAPbobsCOM



'07.15 PRODUCCIÓN - DISTRIBUCIÓN DE CIF NO ASIGNADOS POR HECTAREAS
Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_DistribucionNoAsignados", "EXD_DistribucionNoAsignados.b1f")>
    Friend Class EXD_DistribucionNoAsignados
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private SBO_Company As SAPbobsCOM.Company
        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)



        'Dim SP_SQL_EXD_ANALISIS_EJECUTADO_REAL_DISTRIBUCION_CIF_0715_0716 As String = "EXD_ANALISIS_EJECUTADO_REAL_DISTRIBUCION_CIF_0715_0716 "

        Dim SP_SQL_EXD_ANALISIS_EJECUTADO_REAL_DISTRIBUCION_CIF_0715_0716 As String = "EXD_ANALISIS_EJECUTADO_REAL_DISTRIBUCION_CIF_310118 "

        Dim SP_SQL_EXD_TABLA_NO_ASIGNADOS As String = "EXD_TABLA_NO_ASIGNADOS "


        Dim util As New Util

        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)

            UIAPIRawForm.Title = "EXD: DISTRIBUCIÓN DE NO ASIGNADOS"

            SBO_Application = pAplicacion
            SBO_Company = pCompany
            SboGuiApi = New SAPbouiCOM.SboGuiApi

            DT_FECHA_INICIO.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_FIN.String = Now.Date.Date.ToString("ddMMyyyy")

            DT_FECHA_CONTABILIZACION.String = Now.Date.Date.ToString("ddMMyyyy")

            Me.UIAPIRawForm.Left = (SBO_Application.Desktop.Width / 2) - (UIAPIRawForm.Width / 2)
            Me.UIAPIRawForm.Top = (SBO_Application.Desktop.Height / 2) - ((UIAPIRawForm.Height / 2) + 60)

            CargarMetodo()

        End Sub


        Public Overrides Sub OnInitializeComponent()
            Me.StaticText0 = CType(Me.GetItem("Item_0").Specific, SAPbouiCOM.StaticText)
            Me.StaticText1 = CType(Me.GetItem("Item_1").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_INICIO = CType(Me.GetItem("DTI").Specific, SAPbouiCOM.EditText)
            Me.StaticText2 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("DTF").Specific, SAPbouiCOM.EditText)
            Me.MATRIX = CType(Me.GetItem("MATRIX").Specific, SAPbouiCOM.Matrix)
            Me.BT_BUSCAR = CType(Me.GetItem("B1").Specific, SAPbouiCOM.Button)
            Me.BT_GRABAR = CType(Me.GetItem("B2").Specific, SAPbouiCOM.Button)
            Me.StaticText3 = CType(Me.GetItem("Item_2").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_CONTABILIZACION = CType(Me.GetItem("DTC").Specific, SAPbouiCOM.EditText)
            Me.LB_TOTALES_D_H = CType(Me.GetItem("LB1").Specific, SAPbouiCOM.StaticText)
            Me.BT_CARGAR = CType(Me.GetItem("B3").Specific, SAPbouiCOM.Button)
            Me.StaticText4 = CType(Me.GetItem("Item_4").Specific, SAPbouiCOM.StaticText)
            Me.CBO_METODO = CType(Me.GetItem("CB_1").Specific, SAPbouiCOM.ComboBox)
            Me.LBL_RESULTADO = CType(Me.GetItem("LBL_R").Specific, SAPbouiCOM.StaticText)
            Me.BTN_VALIDAR_PONDERADO = CType(Me.GetItem("B4").Specific, SAPbouiCOM.Button)
            Me.OnCustomInitialize()

        End Sub

        Public Overrides Sub OnInitializeFormEvents()

        End Sub
        Private WithEvents StaticText0 As SAPbouiCOM.StaticText

        Private Sub OnCustomInitialize()

        End Sub
        Private WithEvents StaticText1 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_INICIO As SAPbouiCOM.EditText
        Private WithEvents StaticText2 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_FIN As SAPbouiCOM.EditText
        Private WithEvents MATRIX As SAPbouiCOM.Matrix
        Private WithEvents BT_BUSCAR As SAPbouiCOM.Button
        Private WithEvents BT_GRABAR As SAPbouiCOM.Button
        Private WithEvents StaticText3 As SAPbouiCOM.StaticText
        Private WithEvents DT_FECHA_CONTABILIZACION As SAPbouiCOM.EditText
        Private WithEvents LB_TOTALES_D_H As SAPbouiCOM.StaticText
        Private WithEvents BT_CARGAR As SAPbouiCOM.Button
        Private WithEvents StaticText4 As SAPbouiCOM.StaticText
        Private WithEvents CBO_METODO As SAPbouiCOM.ComboBox


        Private Sub CargarMetodo()

            'Dim sql_query As String = "SELECT ""Code"", ""Name"" FROM ""@EXM_CABTRAN"" WHERE IFNULL(""Name"",'') !='' AND   ""Name"" != '*' ORDER BY ""Name"" ASC"
            Dim sql_query As String = "SELECT CODE, NAME FROM [@EXD_METDIST]"

            GC.Collect()

            Dim vRec As SAPbobsCOM.Recordset
            vRec = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            If ConexionSAP = TipoConexion.Sql Then
                vRec.DoQuery(sql_query)
                vRec.MoveFirst()
            Else
                vRec.DoQuery(sql_query)
                vRec.MoveFirst()
            End If

            Do Until vRec.EoF = True
                CBO_METODO.ValidValues.Add(vRec.Fields.Item(0).Value, vRec.Fields.Item(1).Value)
                vRec.MoveNext()
            Loop
            CBO_METODO.Item.DisplayDesc = True

            GC.Collect()
        End Sub

        Private Sub BT_BUSCAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_BUSCAR.ClickBefore
            ' Throw New System.NotImplementedException()

            Dim dtFechaInicio As String = DT_FECHA_INICIO.Value
            Dim dtFechaFin As String = DT_FECHA_FIN.Value

            Dim dtFecha_inicio As String = DT_FECHA_INICIO.Value
            Dim dtFecha_fin As String = DT_FECHA_FIN.Value

            Dim stMetodo As String = CBO_METODO.Value

            If dtFecha_inicio.Equals("") Or dtFecha_fin.Equals("") Then
                Application.SBO_Application.SetStatusBarMessage("Error en rango de fechas", SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End If


            If stMetodo.Equals("") Then
                Application.SBO_Application.SetStatusBarMessage("Seleccione un metodo", SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End If


            'Dim respuesta As String = Await
            cargarGrilla(dtFecha_inicio, dtFecha_fin, stMetodo)

        End Sub

        Sub cargarGrilla(ByVal stF1 As String, ByVal stF2 As String, ByVal stMetodo As String)


            GC.Collect()



            Dim mquery As String = ""
            Dim resultado As String = ""

            Try
                If ConexionSAP = TipoConexion.Sql Then

                    mquery = SP_SQL_EXD_ANALISIS_EJECUTADO_REAL_DISTRIBUCION_CIF_0715_0716 & "'" & stF1 & "','" & stF2 & "','" & stMetodo & "'"
                ElseIf ConexionSAP = TipoConexion.Hana Then
                    mquery = "CALL " + SP_SQL_EXD_ANALISIS_EJECUTADO_REAL_DISTRIBUCION_CIF_0715_0716 & "('" & stF1 & "','" & stF2 & "','" & stMetodo & "')"
                End If

            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al crear :" + mquery + " ,Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                resultado = "ER"
            End Try

            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_0").ExecuteQuery(mquery)
            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al cargar DT_0 " + ",Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                resultado = "ER"
            End Try

            Try
                'MT_MATRIX.Columns.Item("#").DataBind.Bind("DT_0", "TransId")
                'MT_MATRIX.Columns.Item("#").Visible = False

                MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_0", "Seleccionar")
                MATRIX.Columns.Item("Col_0").ValOn = "0"

                MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_0", "syscode")
                MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_0", "U_EXD_CUECON")
                MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_0", "Centro Costo")
                MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_0", "U_EXD_TIPDIS")
                MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_0", "Debe")
                MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_0", "Haber")
                MATRIX.Columns.Item("Col_7").DataBind.Bind("DT_0", "U_EXD_UNINEG")
                '202203
                'MATRIX.Columns.Item("Col_8").DataBind.Bind("DT_0", "U_EXT_DIM1")
                'MATRIX.Columns.Item("Col_9").DataBind.Bind("DT_0", "U_EXT_DIM2")
                'MATRIX.Columns.Item("Col_10").DataBind.Bind("DT_0", "U_EXT_DIM4")
                'MATRIX.Columns.Item("Col_11").DataBind.Bind("DT_0", "U_EXT_DIM5")
                MATRIX.Columns.Item("Col_8").Visible = False
                MATRIX.Columns.Item("Col_9").Visible = False
                MATRIX.Columns.Item("Col_10").Visible = False
                MATRIX.Columns.Item("Col_11").Visible = False



                MATRIX.LoadFromDataSource()

                resultado = "OK"


                LBL_RESULTADO.Caption = "(" + MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"


            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al cargar Matrix " + ",Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                resultado = "ER"
            End Try

          

            'Dim doDebe As Double = 0
            'Dim doHaber As Double = 0

            'For n As Integer = 1 To MATRIX.RowCount
            '    doDebe += CType(Me.MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
            '    doHaber += CType(Me.MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
            'Next

            'doDebe = Math.Round(doDebe, 2, MidpointRounding.ToEven)
            'doHaber = Math.Round(doHaber, 2, MidpointRounding.ToEven)

            'If doDebe = doHaber Then

            '    LB_TOTALES_D_H.Caption = "Debe: " + doDebe.ToString() + " y Haber " + doHaber.ToString() + " ponderados"
            'Else
            '    LB_TOTALES_D_H.Caption = "Debe: " + doDebe.ToString() + " y Haber " + doHaber.ToString() + " no ponderados"
            'End If


            GC.Collect()

        End Sub

        'Suma de Haber y Debe : Poderados
        Sub sumarTotalesDebeHaber()

            Dim doDebe As Double = 0
            Dim doHaber As Double = 0
            Dim doResultado As Double = 0

            For n As Integer = 1 To MATRIX.RowCount
                doDebe += CType(Me.MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                doHaber += CType(Me.MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
            Next


            doResultado = Math.Round(doDebe, 2, MidpointRounding.ToEven) - Math.Round(doHaber, 2, MidpointRounding.ToEven)

            For n As Integer = 1 To MATRIX.RowCount

                'Ajustamos el debe
                If doDebe < doHaber Then
                    Dim val As Double = CType(Me.MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    If val > 0 Then
                        Dim oEdit As SAPbouiCOM.EditText

                        oEdit = MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific
                        oEdit.Value = val + doResultado * -1
                        Return
                    End If
                    'Ajustamos el haber
                Else
                    Dim val As Double = CType(Me.MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    If val > 0 Then
                        Dim oEdit As SAPbouiCOM.EditText

                        oEdit = MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific
                        oEdit.Value = val + doResultado
                        Return

                    End If
                End If
            Next

        End Sub

        Private Sub BT_GRABAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_GRABAR.ClickBefore
            'Throw New System.NotImplementedException()
            Dim CodeMethod As Integer = 0
            Dim ErrorCode As Integer = 0
            Dim ErrorMsg As String = ""


            Dim doDebe As Double
            Dim doHaber As Double


            'Dim a As Double
            'Dim b As Double

            Dim dtFechaContabiliazacion As String = DT_FECHA_CONTABILIZACION.Value

            Dim oJournalEntries As SAPbobsCOM.JournalEntries
            oJournalEntries = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)

            'oJournalEntries.Reference = util.formartDate(dtFechaContabiliazacion)

            oJournalEntries.ReferenceDate = util.formartDate(dtFechaContabiliazacion)
            oJournalEntries.DueDate = util.formartDate(dtFechaContabiliazacion)
            oJournalEntries.TaxDate = util.formartDate(dtFechaContabiliazacion)

            oJournalEntries.Memo = "DISTRIBUCIÓN DE NO ASIGNADOS DE " + CBO_METODO.Value

            oJournalEntries.TransactionCode = "RCC"
            oJournalEntries.UserFields.Fields.Item("U_EXX_TIPCON").Value = "1"


            Dim oProgressBar As SAPbouiCOM.ProgressBar = Nothing
            oProgressBar = SBO_Application.StatusBar.CreateProgressBar("Procesando...", 17, True)
            oProgressBar.Value = 0

            For n As Integer = 1 To Me.MATRIX.RowCount

                oProgressBar.Value = oProgressBar.Value + 1
                oProgressBar.Text = "Procesando " + n.ToString() + " de " + Me.MATRIX.RowCount.ToString()

                'Procesamos solo las lineas marcadas
                If CType(Me.MATRIX.Columns.Item("Col_0").Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True Then

                    doDebe = CType(Me.MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    doHaber = CType(Me.MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    If doDebe = 0.0 And doHaber = 0.0 Then
                        'NO SE DEBE PROCESAR
                    Else
                        If doDebe > 0 Then
                            oJournalEntries.Lines.AccountCode = CType(Me.MATRIX.Columns.Item("Col_1").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            oJournalEntries.Lines.Debit = doDebe '(doDebe / 1000)
                            oJournalEntries.Lines.CostingCode = CType(Me.MATRIX.Columns.Item("Col_7").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                            If CType(Me.MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                                oJournalEntries.Lines.CostingCode3 = CType(Me.MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            End If

                            'nuevo 202203

                            'If CType(Me.MATRIX.Columns.Item("Col_9").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                            '    oJournalEntries.Lines.CostingCode2 = CType(Me.MATRIX.Columns.Item("Col_9").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            'End If
                            'If CType(Me.MATRIX.Columns.Item("Col_10").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                            '    oJournalEntries.Lines.CostingCode4 = CType(Me.MATRIX.Columns.Item("Col_10").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            'End If
                            'If CType(Me.MATRIX.Columns.Item("Col_11").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                            '    oJournalEntries.Lines.CostingCode5 = CType(Me.MATRIX.Columns.Item("Col_11").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            'End If

                            oJournalEntries.Lines.Add()

                        End If

                        If doHaber > 0 Then
                            oJournalEntries.Lines.AccountCode = CType(Me.MATRIX.Columns.Item("Col_1").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            oJournalEntries.Lines.Credit = doHaber '(doHaber / 1000)
                            oJournalEntries.Lines.CostingCode = CType(Me.MATRIX.Columns.Item("Col_7").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                            If CType(Me.MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                                oJournalEntries.Lines.CostingCode3 = CType(Me.MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            End If

                            'nuevo 202203

                            'If CType(Me.MATRIX.Columns.Item("Col_9").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                            '    oJournalEntries.Lines.CostingCode2 = CType(Me.MATRIX.Columns.Item("Col_9").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            'End If
                            'If CType(Me.MATRIX.Columns.Item("Col_10").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                            '    oJournalEntries.Lines.CostingCode4 = CType(Me.MATRIX.Columns.Item("Col_10").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            'End If
                            'If CType(Me.MATRIX.Columns.Item("Col_11").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value.Trim() <> "" Then
                            '    oJournalEntries.Lines.CostingCode5 = CType(Me.MATRIX.Columns.Item("Col_11").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                            'End If

                            oJournalEntries.Lines.Add()

                        End If



                    End If
                    Console.WriteLine("Procesando " + n.ToString() + " de " + Me.MATRIX.RowCount.ToString())
                End If
            Next



            CodeMethod = oJournalEntries.Add

            oProgressBar.Stop()

            If Not oProgressBar Is Nothing Then
                oProgressBar.Stop()
            End If


            If CodeMethod <> 0 Then
                SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                Application.SBO_Application.SetStatusBarMessage(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
            Else
                Application.SBO_Application.SetStatusBarMessage("Proceso completado con exito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                'cargarGrilla(DT_FECHA_INICIO.Value, DT_FECHA_FIN.Value)

                MATRIX.Clear()

            End If




        End Sub


        Private Sub MATRIX_ClickAfter(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg) Handles MATRIX.ClickAfter
            'Throw New System.NotImplementedException()

            'sumarTotalesDebeHaber()
        End Sub

        Private Sub BT_CARGAR_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BT_CARGAR.ClickBefore

            Dim dtFecha_inicio As String = DT_FECHA_INICIO.Value
            Dim dtFecha_fin As String = DT_FECHA_FIN.Value
            Dim SQL As String = ""

            Dim ErrorCode As Integer = 0
            Dim ErrorMsg As String = ""


            If dtFecha_inicio.Equals("") Or dtFecha_fin.Equals("") Then
                Application.SBO_Application.SetStatusBarMessage("Error en rango de fechas", SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End If


            'Throw New System.NotImplementedException()
            'SQL = SP_SQL_EXD_TABLA_NO_ASIGNADOS & "'" & dtFecha_inicio & "','" & dtFecha_fin & "'"

            If ConexionSAP = TipoConexion.Sql Then

                SQL = SP_SQL_EXD_TABLA_NO_ASIGNADOS & "'" & dtFecha_inicio & "','" & dtFecha_fin & "'"
            ElseIf ConexionSAP = TipoConexion.Hana Then
                SQL = "CALL" + SP_SQL_EXD_TABLA_NO_ASIGNADOS & "('" & dtFecha_inicio & "','" & dtFecha_fin & "')"
            End If



            Dim vRecOUQR As SAPbobsCOM.Recordset

            vRecOUQR = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            vRecOUQR.DoQuery(SQL)
            vRecOUQR.MoveFirst()



            Dim Periodo As String = ""
            Dim PrcCode As String = ""
            Dim PrcName As String = ""
            Dim Cuenta As String = ""
            Dim NombreCuenta As String = ""
            Dim Saldo As Double = 0
            ' Dim Tipo As String
            Dim distribucion As String = ""
            Dim unidadNegocio As String = ""
            'nuevo 202203
            Dim Dimension1 As String = ""
            Dim Dimension2 As String = ""
            Dim Dimension4 As String = ""
            Dim Dimension5 As String = ""


            Dim oCompService As SAPbobsCOM.CompanyService
            Dim oGeneralService As SAPbobsCOM.GeneralService
            Dim oGeneralData As SAPbobsCOM.GeneralData
            Dim oChild As SAPbobsCOM.GeneralData
            Dim oChildren As SAPbobsCOM.GeneralDataCollection
            'Dim oGeneralParams As SAPbobsCOM.GeneralDataParams



            oCompService = SBO_Company.GetCompanyService()
            SBO_Company.StartTransaction()
            oGeneralService = oCompService.GetGeneralService("EXT_NOASIGNADO")
            oGeneralData = oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData)


            Try
                'Setting Data to Master Data Table Fields
                Periodo = vRecOUQR.Fields.Item("Periodo").Value()

                oGeneralData.SetProperty("Code", Periodo)
                oGeneralData.SetProperty("Name", Periodo)

                While Not vRecOUQR.EoF

                    PrcCode = vRecOUQR.Fields.Item("PrcCode").Value()
                    PrcName = vRecOUQR.Fields.Item("PrcName").Value()
                    Cuenta = vRecOUQR.Fields.Item("Cuenta").Value()
                    NombreCuenta = vRecOUQR.Fields.Item("Nombre Cuenta").Value()
                    Saldo = vRecOUQR.Fields.Item("Saldo").Value()
                    distribucion = vRecOUQR.Fields.Item("Tipo distribucion").Value()
                    unidadNegocio = vRecOUQR.Fields.Item("U_EXD_UNINEG").Value()
                    'nuevo 202203
                    'Dimension1 = vRecOUQR.Fields.Item("Dimension 1").Value()
                    'Dimension2 = vRecOUQR.Fields.Item("Dimension 2").Value()
                    'Dimension4 = vRecOUQR.Fields.Item("Dimension 4").Value()
                    'Dimension5 = vRecOUQR.Fields.Item("Dimension 5").Value()

                    'Setting Data to Child Table Fields

                    oChildren = oGeneralData.Child("EXT_DETDIC")
                    oChild = oChildren.Add()

                    oChild.SetProperty("U_EXT_CODCC", PrcCode)
                    oChild.SetProperty("U_EXT_NOMCC", PrcName)
                    oChild.SetProperty("U_EXT_CUECON", Cuenta)
                    oChild.SetProperty("U_EXT_NOMCUE", NombreCuenta)
                    oChild.SetProperty("U_EXT_SALDO", Saldo)
                    oChild.SetProperty("U_EXT_TIPDIS", distribucion)
                    oChild.SetProperty("U_EXT_UNINEG", unidadNegocio)
                    'nuevo 202203
                    'oChild.SetProperty("U_EXT_DIM1", Dimension1)
                    'oChild.SetProperty("U_EXT_DIM2", Dimension2)
                    'oChild.SetProperty("U_EXT_DIM4", Dimension4)
                    'oChild.SetProperty("U_EXT_DIM5", Dimension5)

                    'Attempt to Add the Record

                    vRecOUQR.MoveNext()

                End While

                oGeneralService.Add(oGeneralData)
                'oGeneralService.Update(oGeneralData)

                If SBO_Company.InTransaction Then
                    SBO_Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                    Application.SBO_Application.SetStatusBarMessage("Carga realizada con exito", SAPbouiCOM.BoMessageTime.bmt_Short, False)
                Else
                    SBO_Company.GetLastError(ErrorCode, ErrorMsg)

                    SBO_Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    Application.SBO_Application.SetStatusBarMessage("Error add: " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, True)
                End If

            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error General: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, True)
            End Try



        End Sub
        Private WithEvents LBL_RESULTADO As SAPbouiCOM.StaticText
        Private WithEvents BTN_VALIDAR_PONDERADO As SAPbouiCOM.Button

        Private Sub BTN_VALIDAR_PONDERADO_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BTN_VALIDAR_PONDERADO.ClickBefore
            '  Throw New System.NotImplementedException()

            Dim doDebe As Double = 0
            Dim doHaber As Double = 0

            For n As Integer = 1 To MATRIX.RowCount
                doDebe += CType(Me.MATRIX.Columns.Item("Col_5").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                doHaber += CType(Me.MATRIX.Columns.Item("Col_6").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
            Next

            doDebe = Math.Round(doDebe, 2, MidpointRounding.ToEven)
            doHaber = Math.Round(doHaber, 2, MidpointRounding.ToEven)

            If doDebe = doHaber Then
                'Totales poderados
                LB_TOTALES_D_H.Caption = "Debe: " + doDebe.ToString() + " y Haber" + doHaber.ToString() + " ponderados"
            Else
                LB_TOTALES_D_H.Caption = "Debe: " + doDebe.ToString() + " y Haber" + doHaber.ToString() + " no ponderados"
            End If



        End Sub
    End Class
End Namespace
