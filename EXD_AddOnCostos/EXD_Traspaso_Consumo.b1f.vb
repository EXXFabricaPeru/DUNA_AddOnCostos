Option Strict Off
Option Explicit On

Imports SAPbouiCOM.Framework
Imports System.Globalization

'07.01 Traspaso de consumo

Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_Traspaso_Consumo", "EXD_Traspaso_Consumo.b1f")>
    Friend Class EXD_Traspaso_Consumo
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private SBO_Company As SAPbobsCOM.Company
        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


        Dim util As New Util

        ReadOnly SP_SQL_SP_CUENTA_61_COSTO_PROD_DESTINO_71 As String = "SP_CUENTA_61_COSTO_PROD_DESTINO_71 "
        ReadOnly SP_HANA_SP_CUENTA_61_COSTO_PROD_DESTINO_71 As String = "SP_CUENTA_61_COSTO_PROD_DESTINO_71"

        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)

            UIAPIRawForm.Title = "EXD: TRASPASO DE CONSUMOS"

            SBO_Application = pAplicacion
            SBO_Company = pCompany
            SboGuiApi = New SAPbouiCOM.SboGuiApi

            DT_FECHA_INICIO.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_FIN.String = Now.Date.Date.ToString("ddMMyyyy")

            DT_CONTABILIAZACION.String = Now.Date.Date.ToString("ddMMyyyy")

            Me.UIAPIRawForm.Left = (SBO_Application.Desktop.Width / 2) - (UIAPIRawForm.Width / 2)
            Me.UIAPIRawForm.Top = (SBO_Application.Desktop.Height / 2) - ((UIAPIRawForm.Height / 2) + 60)

            CargarProducto()


        End Sub

        Public Overrides Sub OnInitializeComponent()
            Me.StaticText0 = CType(Me.GetItem("Item_0").Specific, SAPbouiCOM.StaticText)
            Me.StaticText1 = CType(Me.GetItem("Item_1").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_INICIO = CType(Me.GetItem("DTI").Specific, SAPbouiCOM.EditText)
            Me.StaticText2 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("DTF").Specific, SAPbouiCOM.EditText)
            Me.MATRIX = CType(Me.GetItem("MTX").Specific, SAPbouiCOM.Matrix)
            Me.BT_BUSCAR = CType(Me.GetItem("B1").Specific, SAPbouiCOM.Button)
            Me.BT_GRABAR = CType(Me.GetItem("B2").Specific, SAPbouiCOM.Button)
            Me.StaticText3 = CType(Me.GetItem("Item_8").Specific, SAPbouiCOM.StaticText)
            Me.DT_CONTABILIAZACION = CType(Me.GetItem("DTC").Specific, SAPbouiCOM.EditText)
            Me.CB_PRODUCTO = CType(Me.GetItem("CB1").Specific, SAPbouiCOM.ComboBox)
            Me.StaticText4 = CType(Me.GetItem("Item_5").Specific, SAPbouiCOM.StaticText)
            Me.LBL_RESULTADO = CType(Me.GetItem("LBL_R").Specific, SAPbouiCOM.StaticText)
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
        Private WithEvents DT_CONTABILIAZACION As SAPbouiCOM.EditText
        Private WithEvents CB_PRODUCTO As SAPbouiCOM.ComboBox
        Private WithEvents StaticText4 As SAPbouiCOM.StaticText


        Private Sub BT_BUSCAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_BUSCAR.ClickBefore
            'Throw New System.NotImplementedException()

            If CB_PRODUCTO.Value = "999999999999" Then
                BT_GRABAR.Item.Visible = False
            Else
                BT_GRABAR.Item.Visible = True
            End If

            If CB_PRODUCTO.Value.Equals("") Then

                SBO_Application.StatusBar.SetText("Seleccione un articulo", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return
            Else

                Dim dtFecha_inicio As String = DT_FECHA_INICIO.Value
                Dim dtFecha_fin As String = DT_FECHA_FIN.Value
                Dim stProducto As String = CB_PRODUCTO.Selected.Description

                If dtFecha_inicio.Equals("") Or dtFecha_fin.Equals("") Then
                    SBO_Application.StatusBar.SetText("Revisar rango de fechas", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return
                End If

                cargarGrilla(dtFecha_inicio, dtFecha_fin, stProducto)
            End If




        End Sub

        Sub cargarGrilla(ByVal stF1 As String, ByVal stF2 As String, ByVal stP1 As String)
            GC.Collect()

            UIAPIRawForm.Freeze(True)


            Dim mquery As String = ""

            Try
                If ConexionSAP = TipoConexion.Sql Then
                    mquery = SP_SQL_SP_CUENTA_61_COSTO_PROD_DESTINO_71 & "'" & stF1 & "','" & stF2 & "','" & stP1 & "'"
                ElseIf ConexionSAP = TipoConexion.Hana Then
                    mquery = "CALL " + SP_HANA_SP_CUENTA_61_COSTO_PROD_DESTINO_71 & "('" & stF1 & "','" & stF2 & "')"
                End If
            Catch ex As Exception
                SBO_Application.StatusBar.SetText(mquery + " : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return
            End Try



            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_0").ExecuteQuery(mquery)
            Catch ex As Exception
                SBO_Application.StatusBar.SetText("DT_0" + " : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return
            End Try

            Try
                MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_0", "Seleccionar")
                MATRIX.Columns.Item("Col_0").ValOn = "0"

                MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_0", "Cuenta sys")
                MATRIX.Columns.Item("Col_1").Visible = False
                MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_0", "Cuenta")
                MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_0", "Centro de Costo") '"Centro de Costo"
                MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_0", "TransValue")
                MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_0", "Cuenta Costo Prod") '"Cuenta Costo Prod"
                MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_0", "Cuenta Costo Prod sys")
                MATRIX.Columns.Item("Col_6").Visible = False
                MATRIX.Columns.Item("Col_7").DataBind.Bind("DT_0", "Cultivo") 'Cultivo
                MATRIX.Columns.Item("Col_8").DataBind.Bind("DT_0", "Unidad")
                MATRIX.Columns.Item("Col_9").Visible = False
                MATRIX.Columns.Item("Col_10").Visible = False
                MATRIX.Columns.Item("Col_11").Visible = False
                'Para HANA 202203
                'MATRIX.Columns.Item("Col_9").DataBind.Bind("DT_0", "OcrCode2")
                'MATRIX.Columns.Item("Col_10").DataBind.Bind("DT_0", "OcrCode4")
                'MATRIX.Columns.Item("Col_11").DataBind.Bind("DT_0", "OcrCode5")



                MATRIX.LoadFromDataSource()
                MATRIX.AutoResizeColumns()
                LBL_RESULTADO.Caption = "(" + MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"

                SBO_Application.StatusBar.SetText("Success", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)

            Catch ex As Exception
                SBO_Application.StatusBar.SetText("MATRIX" + " : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End Try
            GC.Collect()

            UIAPIRawForm.Freeze(False)
        End Sub

        Private Sub CargarProducto()

            '  Dim sql_query As String = " SELECT DISTINCT T1.ItemCode,T1.ItemName FROM OWOR T0 INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode ORDER BY T1.ItemName "

            Try

                Dim sql_query As String = ""
                If ConexionSAP = TipoConexion.Sql Then
                    sql_query = "SELECT '999999999999' AS ItemCode, ' TODOS ' AS ItemName  " +
                                          "UNION ALL " +
                                          "SELECT DISTINCT T1.ItemCode,T1.ItemName " +
                                          "FROM OWOR T0 INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode " +
                                          "ORDER BY 2 "
                ElseIf ConexionSAP = TipoConexion.Hana Then
                    'sql_query = "SELECT '999999999999' AS ""ItemCode"", ' TODOS ' AS ""ItemName"" FROM DUMMY  " +
                    '                     "UNION ALL " +
                    '                     "SELECT DISTINCT T1.""ItemCode"",T1.""ItemName"" " +
                    '                     "FROM OWOR T0 INNER JOIN OITM T1 ON T0.""ItemCode"" = T1.""ItemCode"" " +
                    '                     "ORDER BY 2 "
                    sql_query = "SELECT '999999999999' AS ""ItemCode"", ' TODOS ' AS ""ItemName"" FROM DUMMY  "
                End If

                'Dim sql_query As String = "SELECT '999999999999' AS ItemCode, ' TODOS ' AS ItemName  " +
                '                      "UNION ALL " +
                '                      "SELECT DISTINCT T1.ItemCode,T1.ItemName " +
                '                      "FROM OWOR T0 INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode " +
                '                      "ORDER BY 2 "
                GC.Collect()

                Dim vRec As SAPbobsCOM.Recordset
                vRec = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                logger.Info("sql_query: " & sql_query)
                If ConexionSAP = TipoConexion.Sql Then
                    vRec.DoQuery(sql_query)
                    vRec.MoveFirst()
                Else
                    vRec.DoQuery(sql_query)
                    vRec.MoveFirst()
                End If

                Do Until vRec.EoF = True
                    CB_PRODUCTO.ValidValues.Add(vRec.Fields.Item(0).Value, vRec.Fields.Item(1).Value)
                    vRec.MoveNext()
                Loop

                CB_PRODUCTO.Item.DisplayDesc = True

            Catch ex As Exception
                logger.Error(ex.Message)
            End Try

            GC.Collect()
        End Sub



        Private Sub BT_GRABAR_ClickBefore(ByVal sboObject As System.Object, ByVal pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As System.Boolean) Handles BT_GRABAR.ClickBefore
            'Throw New System.NotImplementedException()

            Dim CodeMethod As Integer = 0
            Dim ErrorCode As Integer = 0
            Dim ErrorMsg As String = ""
            Dim doValor As Double = 0
            Dim doCredit As Double = 0
            Dim sUnidad As String = ""
            'Dim sDescripcion As String = ""
            Dim sDescripcion As String = CB_PRODUCTO.Selected.Description

            Dim dtFechaContabiliazacion As String = util.formartDate(DT_CONTABILIAZACION.Value)


            Dim oJournalEntries As SAPbobsCOM.JournalEntries
            oJournalEntries = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)



            oJournalEntries.ReferenceDate = dtFechaContabiliazacion
            oJournalEntries.DueDate = dtFechaContabiliazacion
            oJournalEntries.TaxDate = dtFechaContabiliazacion

            oJournalEntries.Memo = "TRASPASO DE CONSUMO " + sDescripcion


            oJournalEntries.TransactionCode = "TRC"
            oJournalEntries.UserFields.Fields.Item("U_EXX_TIPCON").Value = "1"

            Dim oProgressBar As SAPbouiCOM.ProgressBar = Nothing
            oProgressBar = SBO_Application.StatusBar.CreateProgressBar("Procesando...", 17, True)
            oProgressBar.Value = 0

            For n As Integer = 1 To Me.MATRIX.RowCount

                oProgressBar.Value = oProgressBar.Value + 1
                oProgressBar.Text = "Procesando " + n.ToString() + " de " + Me.MATRIX.RowCount.ToString()


                'Procesamos solo las lineas marcadas
                If CType(Me.MATRIX.Columns.Item("Col_0").Cells.Item(n).Specific, SAPbouiCOM.CheckBox).Checked = True Then

                    doValor = CType(Me.MATRIX.Columns.Item("Col_4").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value


                    Dim sDimension3 As String = CType(Me.MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    oJournalEntries.Lines.AccountCode = CType(Me.MATRIX.Columns.Item("Col_1").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value
                    oJournalEntries.Lines.CostingCode = CType(Me.MATRIX.Columns.Item("Col_8").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    If doValor < 0 Then
                        doValor = doValor * -1
                    End If

                    oJournalEntries.Lines.Debit = doValor
                    'doCredit += doValor
                    'oJournalEntries.Lines.CostingCode3 = CType(Me.MATRIX.Columns.Item("Col_3").Cells.Item(n).Specific, SAPbouiCOM.EditText).Value

                    If sDimension3 <> "" Then
                        oJournalEntries.Lines.CostingCode3 = sDimension3
                    End If

                    oJournalEntries.Lines.Add()

                    oJournalEntries.Lines.AccountCode = CType(Me.MATRIX.Columns.Item("Col_6").Cells.Item(1).Specific, SAPbouiCOM.EditText).Value
                    oJournalEntries.Lines.Credit = doValor
                    If sDimension3 <> "" Then
                        oJournalEntries.Lines.CostingCode3 = sDimension3
                    End If


                    oJournalEntries.Lines.Add()
                End If

            Next

            CodeMethod = oJournalEntries.Add

            oProgressBar.Stop()

            If Not oProgressBar Is Nothing Then
                oProgressBar.Stop()
            End If


            If CodeMethod <> 0 Then
                SBO_Company.GetLastError(ErrorCode, ErrorMsg)
                SBO_Application.StatusBar.SetText(ErrorCode.ToString() + " : " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Else
                SBO_Application.StatusBar.SetText("Asiento creado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                MATRIX.Clear()

            End If

         

        End Sub

        Private Sub MATRIX_LinkPressedBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles MATRIX.LinkPressedBefore
            'Throw New System.NotImplementedException()

            Try
                MATRIX.GetLineData(pVal.Row)
                Dim oLink As SAPbouiCOM.LinkedButton = MATRIX.Columns.Item("Col_2").ExtendedObject
                oLink.LinkedObject = "1"
                BubbleEvent = True
            Catch ex As Exception
                SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End Try
          

        End Sub
        Private WithEvents LBL_RESULTADO As SAPbouiCOM.StaticText
    End Class
End Namespace
