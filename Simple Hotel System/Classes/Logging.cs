namespace cinema_ticketing.Classes
{
    public class Logging
    {
        public static void Write(string prjName,
                                string message,
                                string methodName,
                                string sql = null,
                                [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                                [System.Runtime.CompilerServices.CallerFilePath] string sourcefilePath = "",
                                [System.Runtime.CompilerServices.CallerLineNumber()] int sourceLineNumber = 0)
        {
            _ = WriteLog(prjName, sourcefilePath, methodName, sourceLineNumber.ToString(), message, sql);
        }

        private static bool WriteLog(string sPrjName, string sClassName, string sMethodName, string lineNum, string sErrMsg, string sSQL = "")
        {
            if (WriteLogOn() == false) return false;
            try
            {
                string dllPath = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
                //string filePath = My.Settings.LogFilePath.ToString;
                string iniPath = Utility.GetConfiguration().GetSection("Data").GetSection("Settings").GetSection("LogFilePath").Value;
                StreamWriter oWriter = new StreamWriter(iniPath, true);
                string sStr;

                //sStr = "\r\n" + Strings.Format(DateTime.Now, "dd MMM yyyy hh:mm:ss") + "\r\n";
                sStr = "\r\n" + DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt") + "\r\n";
                sStr = sStr + sPrjName + ":" + sClassName + ":" + sMethodName + ":" + lineNum + "\r\n";
                if (dllPath != "") sStr = sStr + dllPath + "\r\n";
                sStr = sStr + sErrMsg;
                if (sSQL != "") sStr = sStr + "\r\n" + sSQL;

                oWriter.WriteLine(sStr);
                oWriter.Flush();
                oWriter.Close();
                oWriter = null;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static bool WriteLogOn()
        {
            StreamReader oReader = null;
            try
            {
                var iniPath = Utility.GetConfiguration().GetSection("Data").GetSection("Settings").GetSection("LogFilePath").Value;
                oReader = new StreamReader(iniPath);
                if (oReader.ReadLine().ToUpper() == "ON")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (oReader != null)
                    oReader.Close();
                oReader = null;
            }
        }

    }
}
