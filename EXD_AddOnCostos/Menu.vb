Imports SAPbouiCOM.Framework

Namespace EXD_AddOnCostos
    Public Class Menu

        Private WithEvents SBO_Application As SAPbouiCOM.Application
        Private WithEvents SBO_Company As SAPbobsCOM.Company

        Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


#Region "VARIABLES"

        ReadOnly MODULO_ID As String = "EXD_AddOnCostos"
        ReadOnly MODULO_DESCRIPCION As String = "AddOn Ajuste de producción"

        ReadOnly MENU_ID_1 As String = "EXD1"
        ReadOnly MENU_DESCRIPCION_1 As String = "1. Reclasificación de cta 60-61"

        ReadOnly MENU_ID_2 As String = "EXD2"
        ReadOnly MENU_DESCRIPCION_2 As String = "2. Traspaso de consumo"

        ReadOnly MENU_ID_3 As String = "EXD3"
        ReadOnly MENU_DESCRIPCION_3 As String = "3. Ajuste de cuentas WIP"

        ReadOnly MENU_ID_4 As String = "EXD4"
        ReadOnly MENU_DESCRIPCION_4 As String = "4. Distribución de no asignados"

        ReadOnly MENU_ID_5 As String = "EXD5"
        ReadOnly MENU_DESCRIPCION_5 As String = "5. Cierre de costo real"

        ReadOnly MENU_ID_6 As String = "EXD6"
        ReadOnly MENU_DESCRIPCION_6 As String = "6. Traspaso de movimientos"

        ReadOnly MENU_ID_7 As String = "EXD7"
        ReadOnly MENU_DESCRIPCION_7 As String = "7. Planilla"


#End Region

        Public pLote As String

        Sub New()
            SBO_Application = Application.SBO_Application
            SBO_Company = SBO_Application.Company.GetDICompany

            If SBO_Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB Then
                ConexionSAP = TipoConexion.Hana
            Else
                ConexionSAP = TipoConexion.Sql
            End If

        End Sub

        Sub AddMenuItems()
            Dim oMenus As SAPbouiCOM.Menus
            Dim oMenuItem As SAPbouiCOM.MenuItem
            oMenus = Application.SBO_Application.Menus

            Dim oCreationPackage As SAPbouiCOM.MenuCreationParams
            oCreationPackage = (Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams))
            oMenuItem = Application.SBO_Application.Menus.Item("1536") 'Modules


            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP
            oCreationPackage.UniqueID = MODULO_ID
            oCreationPackage.String = MODULO_DESCRIPCION
            oCreationPackage.Image = System.IO.Directory.GetCurrentDirectory & "\banco1.png"
            oCreationPackage.Enabled = True
            oCreationPackage.Position = -1

            oMenus = oMenuItem.SubMenus

            Try
                'If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackage)
                logger.Info("AddOn de costos iniciado con exito")
            Catch

            End Try

            Try
                'Get the menu collection of the newly added pop-up item
                oMenuItem = Application.SBO_Application.Menus.Item(MODULO_ID)
                oMenus = oMenuItem.SubMenus

                'Create s sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                oCreationPackage.UniqueID = MENU_ID_1
                oCreationPackage.String = MENU_DESCRIPCION_1
                oMenus.AddEx(oCreationPackage)

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                oCreationPackage.UniqueID = MENU_ID_2
                oCreationPackage.String = MENU_DESCRIPCION_2
                oMenus.AddEx(oCreationPackage)

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                oCreationPackage.UniqueID = MENU_ID_3
                oCreationPackage.String = MENU_DESCRIPCION_3
                oMenus.AddEx(oCreationPackage)

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                oCreationPackage.UniqueID = MENU_ID_4
                oCreationPackage.String = MENU_DESCRIPCION_4
                oMenus.AddEx(oCreationPackage)

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                oCreationPackage.UniqueID = MENU_ID_5
                oCreationPackage.String = MENU_DESCRIPCION_5
                oMenus.AddEx(oCreationPackage)

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                oCreationPackage.UniqueID = MENU_ID_6
                oCreationPackage.String = MENU_DESCRIPCION_6
                oMenus.AddEx(oCreationPackage)

                'oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                'oCreationPackage.UniqueID = MENU_ID_7
                'oCreationPackage.String = MENU_DESCRIPCION_7
                'oMenus.AddEx(oCreationPackage)


            Catch
                'Menu already exists
                'Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, True)
            End Try
        End Sub

        Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
            BubbleEvent = True

            Try
                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_1) Then
                    Dim activeForm As New EXD_Reclasificacion_60_61(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If

                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_2) Then
                    Dim activeForm As New EXD_Traspaso_Consumo(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If

                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_3) Then
                    Dim activeForm As New EXD_AjusteCtaWIP(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If

                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_4) Then
                    Dim activeForm As New EXD_DistribucionNoAsignados(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If

                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_5) Then
                    Dim activeForm As New EXD_CierreCostoReal(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If

                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_6) Then
                    Dim activeForm As New EXD_MovimientoCosto(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If

                If (pVal.BeforeAction And pVal.MenuUID = MENU_ID_7) Then
                    Dim activeForm As New EXD_PlanillaOFI(SBO_Application, SBO_Company)
                    activeForm.Show()
                End If



            Catch ex As System.Exception
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "")

                logger.Error(ex.Message)

            End Try
        End Sub

    End Class
End Namespace