Option Strict Off
Option Explicit On
Imports System.Threading
Imports System.Threading.Tasks


Imports SAPbouiCOM.Framework

Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_MovimientoCosto", "EXD_MovimientoCosto.b1f")>
    Friend Class EXD_MovimientoCosto
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private SBO_Company As SAPbobsCOM.Company
        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)




        ReadOnly SP_SQL_CURSOR As String = " SP_CURSOR_MOVIMIENTO "


        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)

            UIAPIRawForm.Title = "TRASPASO DE MOVIMIENTOS"



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
            Me.DT_FECHA_INICIO = CType(Me.GetItem("DT_1").Specific, SAPbouiCOM.EditText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("DT_2").Specific, SAPbouiCOM.EditText)
            Me.StaticText1 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.StaticText2 = CType(Me.GetItem("Item_4").Specific, SAPbouiCOM.StaticText)
            Me.MT_MATRIX = CType(Me.GetItem("MTX").Specific, SAPbouiCOM.Matrix)
            Me.BTN_GRABAR = CType(Me.GetItem("BT_1").Specific, SAPbouiCOM.Button)
            Me.BTN_BUSCAR = CType(Me.GetItem("BT_0").Specific, SAPbouiCOM.Button)
            Me.LBL_RESULTADO = CType(Me.GetItem("LBL_R").Specific, SAPbouiCOM.StaticText)
            Me.OnCustomInitialize()

        End Sub

        Public Overrides Sub OnInitializeFormEvents()
            AddHandler LoadAfter, AddressOf Me.Form_LoadAfter

        End Sub
        Private WithEvents StaticText0 As SAPbouiCOM.StaticText

        Private Sub OnCustomInitialize()

        End Sub
        Private WithEvents DT_FECHA_INICIO As SAPbouiCOM.EditText
        Private WithEvents DT_FECHA_FIN As SAPbouiCOM.EditText
        Private WithEvents StaticText1 As SAPbouiCOM.StaticText
        Private WithEvents StaticText2 As SAPbouiCOM.StaticText
        Private WithEvents MT_MATRIX As SAPbouiCOM.Matrix
        Private WithEvents BTN_BUSCAR As SAPbouiCOM.Button
        Private WithEvents BTN_GRABAR As SAPbouiCOM.Button

        Private Sub BTN_BUSCAR_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BTN_BUSCAR.ClickBefore
            '  Throw New System.NotImplementedException()

            Dim FechaInicio As String = DT_FECHA_INICIO.Value
            Dim FechaFin As String = DT_FECHA_FIN.Value

            cargarMatrix(FechaInicio, FechaFin)



        End Sub

        Sub cargarMatrix(ByVal sF1 As String, ByVal SF2 As String)
            GC.Collect()

            Dim mquery As String = ""

            Try
                If ConexionSAP = TipoConexion.Sql Then

                    mquery = "SELECT * FROM [dbo].[TABLE_0] WHERE FContabilizacion BETWEEN " & "'" & sF1 & "' AND '" & SF2 & "'"

                ElseIf ConexionSAP = TipoConexion.Hana Then

                    mquery = "SELECT * FROM ""TABLE_0"" WHERE ""FContabilizacion"" BETWEEN " & "'" & sF1 & "' AND '" & SF2 & "'"

                    'mquery = SP_SQL_EXD_ANALISIS_EJECUTADO_REAL_MOV + "'" + sF1 + "'" + "," + "'" + SF2 + "'"

                End If
            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al crear :" + mquery + " ,Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_0").ExecuteQuery(mquery)
            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al cargar DT_0 " + mquery + ", " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Return
            End Try

            Try
                'MT_MATRIX.Columns.Item("#").DataBind.Bind("DT_0", "TransId")
                'MT_MATRIX.Columns.Item("#").Visible = False

                'MT_MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_0", "Code")
                'MT_MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_0", "Name")

                MT_MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_0", "nroAsiento")
                MT_MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_0", "nroLinea")
                MT_MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_0", "FContabilizacion")
                MT_MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_0", "elementoCosto")
                MT_MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_0", "cuenta")
                MT_MATRIX.Columns.Item("Col_7").DataBind.Bind("DT_0", "nombreCuenta")
                MT_MATRIX.Columns.Item("Col_8").DataBind.Bind("DT_0", "cultivo")
                MT_MATRIX.Columns.Item("Col_9").DataBind.Bind("DT_0", "codigoCC")
                MT_MATRIX.Columns.Item("Col_10").DataBind.Bind("DT_0", "nombreCC")
                MT_MATRIX.Columns.Item("Col_11").DataBind.Bind("DT_0", "Campania")
                MT_MATRIX.Columns.Item("Col_12").DataBind.Bind("DT_0", "saldo")
                MT_MATRIX.Columns.Item("Col_13").DataBind.Bind("DT_0", "tipo")


                MT_MATRIX.LoadFromDataSource()

                LBL_RESULTADO.Caption = "(" + MT_MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"

                SBO_Application.MessageBox("Se han encontrado: " & LBL_RESULTADO.Caption.ToString() & "registro(s)")

            Catch ex As Exception
                Application.SBO_Application.SetStatusBarMessage("Error al cargar Matrix " + ",Detalle: " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, True)
                SBO_Application.MessageBox("Error: " & ex.Message)
                Return
            End Try

            GC.Collect()

        End Sub



        Sub aaaaa(ByVal i As Integer)
            Console.WriteLine(i.ToString)
        End Sub
        Private Sub BTN_GRABAR_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BTN_GRABAR.ClickBefore
            ' Throw New System.NotImplementedException()

            GC.Collect()

            Dim rs_pom As SAPbobsCOM.Recordset

            Dim stFF As String = DT_FECHA_FIN.Value
            Dim stFI As String = DT_FECHA_INICIO.Value


            If stFI.Equals("") Then
                SBO_Application.MessageBox("Debe ingresar una fecha de inicio")
                Return
            End If

            If stFF.Equals("") Then
                SBO_Application.MessageBox("Debe ingresar una fecha de finalización")
                Return
            End If

            If MT_MATRIX.RowCount = 0 Then
                SBO_Application.MessageBox("No hay datos para procesar")
                Return
            End If


            Dim stSQLStament As String = ""

            If ConexionSAP = TipoConexion.Sql Then
                stSQLStament = SP_SQL_CURSOR & "'" & stFI & "','" & stFF & "'"
            ElseIf ConexionSAP = TipoConexion.Hana Then
                stSQLStament = "CALL " + SP_SQL_CURSOR & "('" & stFI & "','" & stFF & "')"
            End If


            Try
                rs_pom = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                rs_pom.DoQuery(stSQLStament)

            Catch ex As Exception
                SBO_Application.MessageBox("stSQLStament: " & stSQLStament & " Detalle: " & ex.Message)
                logger.Error("stSQLStament: " & stSQLStament & " Detalle: " & ex.Message)
                Return
            End Try



            SBO_Application.MessageBox("Proceso completado")



            cargarMatrix(stFI, stFF)


#Region "PASO 0"

            'Try

            '    Dim store As String = "EXD_ANALISIS_EJECUTADO_REAL_MEJORADO " & "'20180101'" & "," & "'20181231', 'T'"

            '    '= " EXD_ANALISIS_EJECUTADO_REAL_RR2 " & "'20180201'" & " , " & "'20180228'"

            '    Dim rs_pom As SAPbobsCOM.Recordset
            '    rs_pom = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            '    rs_pom.DoQuery(store)


            '    Dim U_EXD_Asiento As Double
            '    Dim U_EXD_NroLinea As Double
            '    Dim U_EXD_FECCON As Date
            '    Dim U_EXD_ELECOS As String
            '    Dim U_EXD_Cuenta As String
            '    Dim U_EXD_NOMCUE As String
            '    Dim U_EXD_CULTIVO As String
            '    Dim U_EXD_CODCC As String
            '    Dim U_EXD_NOMCC As String
            '    Dim U_EXD_CAMPANIA As String
            '    Dim U_EXD_SALDO As Decimal
            '    Dim U_EXD_TIPO As String

            '    Dim SQL_INSERT As String

            '    Dim Z As Integer = 0

            '    While rs_pom.EoF = False


            '        U_EXD_Asiento = rs_pom.Fields.Item(1).Value
            '        U_EXD_NroLinea = rs_pom.Fields.Item(2).Value
            '        U_EXD_FECCON = rs_pom.Fields.Item(3).Value
            '        U_EXD_ELECOS = rs_pom.Fields.Item(4).Value
            '        U_EXD_Cuenta = rs_pom.Fields.Item(5).Value
            '        U_EXD_NOMCUE = rs_pom.Fields.Item(6).Value
            '        U_EXD_CULTIVO = rs_pom.Fields.Item(7).Value
            '        U_EXD_CODCC = rs_pom.Fields.Item(8).Value
            '        U_EXD_NOMCC = rs_pom.Fields.Item(9).Value
            '        U_EXD_CAMPANIA = rs_pom.Fields.Item(10).Value
            '        U_EXD_SALDO = rs_pom.Fields.Item(11).Value
            '        U_EXD_TIPO = rs_pom.Fields.Item(12).Value


            '        Dim taskRapidito As Task = Task.Factory.StartNew(Sub()


            '                                                             SQL_INSERT = "EXD_INSERT_ANALISIS_EJECUTADO_REAL " &
            '                                                                 U_EXD_Asiento & " , " &
            '                                                                 U_EXD_NroLinea & " , " &
            '                                                                 "'" & U_EXD_FECCON & "'" & " , " &
            '                                                                 "'" & U_EXD_ELECOS & "'" & " , " &
            '                                                                 "'" & U_EXD_Cuenta & "'" & " , " &
            '                                                                 "'" & U_EXD_NOMCUE & "'" & " , " &
            '                                                                 "'" & U_EXD_CULTIVO & "'" & " , " &
            '                                                                 "'" & U_EXD_CODCC & "'" & " , " &
            '                                                                 "'" & U_EXD_NOMCC & "'" & " , " &
            '                                                                 "'" & U_EXD_CAMPANIA & "'" & " , " &
            '                                                                 U_EXD_SALDO & " , " &
            '                                                                 "'" & U_EXD_TIPO & "'"

            '                                                             Dim rs As SAPbobsCOM.Recordset
            '                                                             rs = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            '                                                             rs.DoQuery(SQL_INSERT)



            '                                                         End Sub)

            '        Console.WriteLine(Z & " DE " & rs_pom.RecordCount)

            '        Z += 1

            '        rs_pom.MoveNext()
            '    End While


            'Catch ex As Exception

            'Finally

            '    GC.Collect()


            'End Try


#End Region
#Region "PASO 1"


            'Dim taskRR As Task = Task.Factory.StartNew(Sub()


            '                                               Dim util As New Util
            '                                               Dim sboTable As SAPbobsCOM.UserTable
            '                                               Dim SQL As String = ""
            '                                               Dim ErrorCod As Integer = 0
            '                                               Dim ErrorMsg As String = ""


            '                                               For i As Integer = 1 To MT_MATRIX.RowCount

            '                                                   Dim Code As String = ""


            '                                                   Dim vRecCUFD As SAPbobsCOM.Recordset
            '                                                   vRecCUFD = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            '                                                   Try
            '                                                       If SBO_Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB Then

            '                                                           SQL = " SELECT MAX(CONVERT(INT, REPLACE(Code,'''','')))+1 FROM [@EXD_COSTOS] "

            '                                                       Else

            '                                                           SQL = " SELECT MAX(CONVERT(INT, REPLACE(Code,'''','')))+1 FROM [@EXD_COSTOS] "

            '                                                       End If

            '                                                       vRecCUFD.DoQuery(SQL)
            '                                                       vRecCUFD.MoveFirst()

            '                                                       Code = (vRecCUFD.Fields.Item(0).Value)


            '                                                       If Code <> "" Then

            '                                                           sboTable = SBO_Company.UserTables.Item("EXD_COSTOS")
            '                                                           sboTable.Code = Code
            '                                                           sboTable.Name = Code

            '                                                           If CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value Is Nothing Then

            '                                                               Dim a As String = util.formartDate(CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value)

            '                                                           End If

            '                                                           Console.WriteLine(i.ToString() + "-" + CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value)

            '                                                           sboTable.UserFields.Fields.Item("U_EXD_Asiento").Value = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_NroLinea").Value = CType(Me.MT_MATRIX.Columns.Item("Col_3").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_FECCON").Value = util.formartDate(CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value)
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_ELECOS").Value = CType(Me.MT_MATRIX.Columns.Item("Col_5").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_Cuenta").Value = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_NOMCUE").Value = CType(Me.MT_MATRIX.Columns.Item("Col_7").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_CULTIVO").Value = CType(Me.MT_MATRIX.Columns.Item("Col_8").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_CODCC").Value = CType(Me.MT_MATRIX.Columns.Item("Col_9").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_NOMCC").Value = CType(Me.MT_MATRIX.Columns.Item("Col_10").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_CAMPANIA").Value = CType(Me.MT_MATRIX.Columns.Item("Col_11").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_SALDO").Value = CType(Me.MT_MATRIX.Columns.Item("Col_12").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '                                                           sboTable.UserFields.Fields.Item("U_EXD_TIPO").Value = CType(Me.MT_MATRIX.Columns.Item("Col_13").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value


            '                                                           If sboTable.Add() <> 0 Then
            '                                                               ' SBO_Company.GetLastError(ErrorCode, ErrorMsg)
            '                                                               SBO_Company.GetLastError(ErrorCod, ErrorMsg)
            '                                                               SBO_Application.StatusBar.SetText("Error: " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '                                                           Else
            '                                                               SBO_Application.StatusBar.SetText("Procesando... " + i.ToString + " de " + MT_MATRIX.RowCount.ToString, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            '                                                           End If

            '                                                       End If



            '                                                   Catch ex As Exception

            '                                                       SBO_Application.StatusBar.SetText("Error: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

            '                                                   End Try





            '                                               Next


            '                                           End Sub)
#End Region
#Region "PASO 2"

            'For i As Integer = 1 To MT_MATRIX.RowCount


            '    Dim Code As String = ""


            '    Dim vRecCUFD As SAPbobsCOM.Recordset
            '    vRecCUFD = SBO_Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            '    Try
            '        If SBO_Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB Then

            '            SQL = " SELECT MAX(CONVERT(INT, REPLACE(Code,'''','')))+1 FROM [@EXD_COSTOS] "

            '        Else

            '            SQL = " SELECT MAX(CONVERT(INT, REPLACE(Code,'''','')))+1 FROM [@EXD_COSTOS] "

            '        End If

            '        vRecCUFD.DoQuery(SQL)
            '        vRecCUFD.MoveFirst()

            '        Code = (vRecCUFD.Fields.Item(0).Value)


            '        If Code <> "" Then

            '            sboTable = SBO_Company.UserTables.Item("EXD_COSTOS")
            '            sboTable.Code = Code
            '            sboTable.Name = Code

            '            If CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value Is Nothing Then

            '                Dim a As String = util.formartDate(CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value)

            '            End If

            '            Console.WriteLine(i.ToString() + "-" + CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value)

            '            sboTable.UserFields.Fields.Item("U_EXD_Asiento").Value = CType(Me.MT_MATRIX.Columns.Item("Col_2").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_NroLinea").Value = CType(Me.MT_MATRIX.Columns.Item("Col_3").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_FECCON").Value = util.formartDate(CType(Me.MT_MATRIX.Columns.Item("Col_4").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value)
            '            sboTable.UserFields.Fields.Item("U_EXD_ELECOS").Value = CType(Me.MT_MATRIX.Columns.Item("Col_5").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_Cuenta").Value = CType(Me.MT_MATRIX.Columns.Item("Col_6").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_NOMCUE").Value = CType(Me.MT_MATRIX.Columns.Item("Col_7").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_CULTIVO").Value = CType(Me.MT_MATRIX.Columns.Item("Col_8").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_CODCC").Value = CType(Me.MT_MATRIX.Columns.Item("Col_9").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_NOMCC").Value = CType(Me.MT_MATRIX.Columns.Item("Col_10").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_CAMPANIA").Value = CType(Me.MT_MATRIX.Columns.Item("Col_11").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_SALDO").Value = CType(Me.MT_MATRIX.Columns.Item("Col_12").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value
            '            sboTable.UserFields.Fields.Item("U_EXD_TIPO").Value = CType(Me.MT_MATRIX.Columns.Item("Col_13").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value


            '            If sboTable.Add() <> 0 Then
            '                ' SBO_Company.GetLastError(ErrorCode, ErrorMsg)
            '                SBO_Company.GetLastError(ErrorCod, ErrorMsg)
            '                SBO_Application.StatusBar.SetText("Error: " + ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            Else
            '                SBO_Application.StatusBar.SetText("Procesando... " + i.ToString + " de " + MT_MATRIX.RowCount.ToString, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            '            End If

            '        End If



            '    Catch ex As Exception

            '        SBO_Application.StatusBar.SetText("Error: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

            '    End Try





            'Next
#End Region






        End Sub
        Private WithEvents LBL_RESULTADO As SAPbouiCOM.StaticText

        Private Sub Form_LoadAfter(pVal As SAPbouiCOM.SBOItemEventArg)
            Throw New System.NotImplementedException()

        End Sub
    End Class
End Namespace
