Imports System.Globalization

Public Class Util

    Private ReadOnly logger As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Function formartDate(ByVal fecha As String) As Date

        Try
            Return DateTime.ParseExact(fecha,
                                 "yyyyMMdd",
                                 CultureInfo.InvariantCulture,
                                 DateTimeStyles.None)
        Catch ex As Exception
            logger.Error("formartDate: " & ex.Message)
        End Try

        Return Nothing

    End Function
End Class
