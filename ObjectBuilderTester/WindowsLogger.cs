using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ObjectBuilderTester
{
    class WindowsLogger
    {

        public WindowsLogger() { }

        public void WriteErrorToWindowsApplicationLog(Exception Ein, string Classin = "", string MethodIn = "", string AdditionalInfo = "")
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    string Logstring = "\n"; 
                    if(Classin != "") { Logstring += "Error in class: " + Classin + "\n"; }
                    if(MethodIn != "") { Logstring += "Error in Method: " + MethodIn + "\n"; }
                    if(AdditionalInfo != "") { Logstring += "Additional Info: " + AdditionalInfo + "\n\n"; }
                    Logstring += @Ein;

                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Application Name: " + System.AppDomain.CurrentDomain.FriendlyName + Logstring, EventLogEntryType.Error, 101, 1);
                }
            }
            catch
            { }

        }

        public void WriteInformationToWindowsApplicationLog(string InfoToWrite, string Classin = "", string MethodIn = "", string AdditionalInfo = "")
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    string Logstring = "\n";
                    if (Classin != "") { Logstring += "Calling class: " + Classin + "\n"; }
                    if (MethodIn != "") { Logstring += "Calling Method: " + MethodIn + "\n"; }
                    if (AdditionalInfo != "") { Logstring += "Additional Info: " + AdditionalInfo + "\n\n"; }
                    Logstring += "\n" + @InfoToWrite;

                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Application Name: " + System.AppDomain.CurrentDomain.FriendlyName + Logstring, EventLogEntryType.Information, 101, 1);
                }
            }
            catch
            { }

        }
    }
}
