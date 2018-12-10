using System;
using TeamcityDataExecution.Helper;
using System.Management.Automation;

namespace TeamcityDataExecution
{
    class Program
    {
        static void Main(string[] args)
        {
            //Avoid excuteion policy
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                string script = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted; Get-ExecutionPolicy"; // the second command to know the ExecutionPolicy level
                PowerShellInstance.AddScript(script);
                var someResult = PowerShellInstance.Invoke();
            }

            //Pull data from teamcity to SQLite database
            new ExcuteShellScriptToDB().GetFailureReport();
        }
    }
}
