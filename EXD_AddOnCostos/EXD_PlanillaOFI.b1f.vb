Option Strict Off
Option Explicit On

Imports SAPbouiCOM.Framework

Namespace EXD_AddOnCostos
    <FormAttribute("EXD_AddOnCostos.EXD_PlanillaOFI", "EXD_PlanillaOFI.b1f")>
    Friend Class EXD_PlanillaOFI
        Inherits UserFormBase

        Private WithEvents SboGuiApi As SAPbouiCOM.SboGuiApi
        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private SBO_Company As SAPbobsCOM.Company
        Dim util As New Util

        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        ReadOnly SP_SQL_EXD_TABLA_NO_ASIGNADOS_V2 As String = "EXD_TABLA_NO_ASIGNADOS_V2 "

        'ReadOnly SP_HANA_SBO_EXD_RECLASIFICACION_CTA_60_61 As String = "SBO_EXD_RECLASIFICACION_60_61 "


        Public Sub New(ByVal pAplicacion As SAPbouiCOM.Application, ByVal pCompany As SAPbobsCOM.Company)

            UIAPIRawForm.Title = "PLANILLA OFISIS"

            SBO_Application = pAplicacion
            SBO_Company = pCompany
            SboGuiApi = New SAPbouiCOM.SboGuiApi

            DT_FECHA_INICIO.String = Now.Date.Date.ToString("ddMMyyyy")
            DT_FECHA_FIN.String = Now.Date.Date.ToString("ddMMyyyy")

            'DT_FECHA_RECLASIFICACION.String = Now.Date.Date.ToString("ddMMyyyy")

            Me.UIAPIRawForm.Left = (SBO_Application.Desktop.Width / 2) - (UIAPIRawForm.Width / 2)
            Me.UIAPIRawForm.Top = (SBO_Application.Desktop.Height / 2) - ((UIAPIRawForm.Height / 2) + 60)

        End Sub

        Public Overrides Sub OnInitializeComponent()
            Me.StaticText0 = CType(Me.GetItem("Item_0").Specific, SAPbouiCOM.StaticText)
            Me.StaticText1 = CType(Me.GetItem("Item_1").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_INICIO = CType(Me.GetItem("TXT_F1").Specific, SAPbouiCOM.EditText)
            Me.StaticText2 = CType(Me.GetItem("Item_3").Specific, SAPbouiCOM.StaticText)
            Me.DT_FECHA_FIN = CType(Me.GetItem("TXT_F2").Specific, SAPbouiCOM.EditText)
            Me.BTN_BUSCAR = CType(Me.GetItem("BTN_B1").Specific, SAPbouiCOM.Button)
            Me.BTN_PROCESAR = CType(Me.GetItem("BTN_B2").Specific, SAPbouiCOM.Button)
            Me.BTN_MARCAR_TODO = CType(Me.GetItem("BTN_3").Specific, SAPbouiCOM.Button)
            Me.BTN_DESMARCAR_TODO = CType(Me.GetItem("BTN_B3").Specific, SAPbouiCOM.Button)
            Me.MTX_MATRIX = CType(Me.GetItem("MTX").Specific, SAPbouiCOM.Matrix)
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
        Private WithEvents BTN_BUSCAR As SAPbouiCOM.Button
        Private WithEvents BTN_PROCESAR As SAPbouiCOM.Button
        Private WithEvents BTN_MARCAR_TODO As SAPbouiCOM.Button
        Private WithEvents BTN_DESMARCAR_TODO As SAPbouiCOM.Button
        Private WithEvents MTX_MATRIX As SAPbouiCOM.Matrix

        Private Sub BTN_BUSCAR_ClickBefore(sboObject As Object, pVal As SAPbouiCOM.SBOItemEventArg, ByRef BubbleEvent As Boolean) Handles BTN_BUSCAR.ClickBefore
            'Throw New System.NotImplementedException()

            Dim stFecha_Inicio As String = DT_FECHA_INICIO.Value.Trim()
            Dim stFecha_Fin As String = DT_FECHA_FIN.Value.Trim()

            Cargar_Matrix(stFecha_Inicio, stFecha_Fin)

        End Sub

        Sub Cargar_Matrix(ByVal sF1 As String, ByVal SF2 As String)

            Dim mquery As String = vbEmpty

            If ConexionSAP = TipoConexion.Sql Then
                mquery = SP_SQL_EXD_TABLA_NO_ASIGNADOS_V2 & "'" & sF1 & "','" & SF2 & "'"
            ElseIf ConexionSAP = TipoConexion.Hana Then
                mquery = "CALL " + SP_SQL_EXD_TABLA_NO_ASIGNADOS_V2 & "('" & sF1 & "','" & SF2 & "')"
            End If

            Try
                UIAPIRawForm.DataSources.DataTables.Item("DT_0").ExecuteQuery(mquery)

                'MTX_MATRIX.Columns.Item("#").DataBind.Bind("DT_0", "Periodo")
                'MTX_MATRIX.Columns.Item("#").Visible = False


                MTX_MATRIX.Columns.Item("Col_0").DataBind.Bind("DT_0", "Seleccionar")
                MTX_MATRIX.Columns.Item("Col_0").ValOn = "0"

                MTX_MATRIX.Columns.Item("Col_1").DataBind.Bind("DT_0", "Periodo")
                MTX_MATRIX.Columns.Item("Col_2").DataBind.Bind("DT_0", "PrcCode")
                MTX_MATRIX.Columns.Item("Col_3").DataBind.Bind("DT_0", "PrcName")
                MTX_MATRIX.Columns.Item("Col_4").DataBind.Bind("DT_0", "Cuenta")
                MTX_MATRIX.Columns.Item("Col_5").DataBind.Bind("DT_0", "Nombre Cuenta")
                MTX_MATRIX.Columns.Item("Col_6").DataBind.Bind("DT_0", "Saldo")
                MTX_MATRIX.Columns.Item("Col_7").DataBind.Bind("DT_0", "U_EXD_UNINEG")
                MTX_MATRIX.Columns.Item("Col_8").DataBind.Bind("DT_0", "Tipo distribucion")
                MTX_MATRIX.Columns.Item("Col_9").DataBind.Bind("DT_0", "u_exd_ctaind")

                'nuevo 202203
                MTX_MATRIX.Columns.Item("Col_10").DataBind.Bind("DT_0", "Dimension 1")
                MTX_MATRIX.Columns.Item("Col_11").DataBind.Bind("DT_0", "Dimension 2")
                MTX_MATRIX.Columns.Item("Col_12").DataBind.Bind("DT_0", "Dimension 4")
                MTX_MATRIX.Columns.Item("Col_13").DataBind.Bind("DT_0", "Dimension 5")

                MTX_MATRIX.LoadFromDataSource()

                SBO_Application.StatusBar.SetText("Success", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)

                'LBL_RESULTADO.Caption = "(" + MT_MATRIX.RowCount.ToString() + ")" + " Encontrado(s)"

            Catch ex As Exception
                SBO_Application.StatusBar.SetText("cargarMatrix, " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                logger.Error("cargarMatrix: " & ex.Message)
                Return
            End Try

            GC.Collect()

        End Sub


    End Class
End Namespace
