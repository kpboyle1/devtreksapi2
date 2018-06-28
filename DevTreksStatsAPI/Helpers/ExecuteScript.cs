using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using DevTreks.DevTreksStatsApi.Models;

namespace DevTreks.DevTreksStatsApi.Helpers
{
    /// <summary>
    ///Purpose:		Run webapis using netcore apps
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///References:	CTA Algorithm 2, subalgo 2 (R webapi) and subalgo 3 (python webap)
    ///</summary>
    public class ExecuteScript
    {
        public static async Task<bool> RunScript(IStatScriptRepository StatScriptRep, 
            StatScript initStat)
        {
            //remember: even if the script can't run; still want the error message saved in json output file

            //item contains the scriptURL and the dataURL
            bool bHasStatResult = false;
            //new Stat needs the paths to R and P executable and webRoots that were added 
            //to StatRepo during Startup.cs
            StatScript.FillInRepositoryStatScriptProperties(StatScriptRep, initStat);
            StringBuilder sb = new StringBuilder();
            initStat.IsComplete = false;

            if (string.IsNullOrEmpty(initStat.DataURL) || (!initStat.DataURL.EndsWith(".csv")))
            {
                initStat.ErrorMessage = "The dataset file URL has not been added to the Data URL. The file must be stored in a Resource and use a csv file extension.";
            }
            if (string.IsNullOrEmpty(initStat.ScriptURL) || (!initStat.ScriptURL.EndsWith(".txt")))
            {
                initStat.ErrorMessage += "The script file URL has not been added to the Joint Data.The file must be stored in a Resource and use a txt file extension.";
            }
            if (string.IsNullOrEmpty(initStat.StatType))
            {
                initStat.ErrorMessage += "The type of statistical package to run has not been filled in. Please specify r or py.";
            }
            string sScriptExecutable = string.Empty;
            if (initStat.StatType == StatScript.STAT_TYPE.py.ToString())
            {
                sScriptExecutable = initStat.PyExecutablePath;
            }
            else if (initStat.StatType == StatScript.STAT_TYPE.julia.ToString())
            {
                sScriptExecutable = initStat.JuliaExecutablePath;
            }
            else
            {
                //no harm in filling in again, but client should have sent this
                initStat.StatType = StatScript.STAT_TYPE.r.ToString();
                //default is R because it runs faster than Py
                sScriptExecutable = initStat.RExecutablePath;
            }
            if (string.IsNullOrEmpty(sScriptExecutable)
                || (!File.Exists(sScriptExecutable)))
            {
                initStat.ErrorMessage += "The file path to the script executable could not be found.";
            }
            string sDataURLFilePath = string.Empty;
            string sScriptURLFilePath = string.Empty;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = sScriptExecutable;
                start.RedirectStandardOutput = true;
                start.UseShellExecute = false;

                //task.when.all this
                sDataURLFilePath = await FileStorageIO.SaveURLInTempFile(initStat, initStat.DataURL);
                sScriptURLFilePath = await FileStorageIO.SaveURLInTempFile(initStat,
                    initStat.ScriptURL, sDataURLFilePath);
                //init url where stat results held
                initStat.OutputURL = string.Empty;

                start.Arguments = string.Format("{0} {1}", sScriptURLFilePath, sDataURLFilePath);
                start.CreateNoWindow = true;

                //the scripts are run sync
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        //configure added to ensure results appended to sb
                        sb.Append(await reader.ReadToEndAsync().ConfigureAwait(false));
                    }

                    process.WaitForExit();
                }
                //client accesses results by deserializing Json response body
                //api only returns json statscript and can't access wwwroot except through api
                initStat.StatisticalResult = FileStorageIO.CleanScriptforResponseBody(sb);
                if (string.IsNullOrEmpty(initStat.StatisticalResult))
                {
                    initStat.ErrorMessage += "The script could not be run. Please double check both the script and the dataset"; 
                }
                else
                {
                    //fill in completed date -used to delete completed scripts on server
                    initStat.DateCompleted
                        = DateTime.Now.Date.ToString("d", CultureInfo.InvariantCulture);
                    initStat.IsComplete = true;
                }
                //initStat is added to temp file storage and path is converted to url for auditing
                //the url can't be directly accessed but the file path can be found from outputURL
                var json = JsonConvert.SerializeObject(initStat);
                bool bHasSaved
                    = await FileStorageIO.SaveContentInFile(initStat, sDataURLFilePath, json);
                if (bHasSaved)
                {
                    bHasStatResult = initStat.IsComplete;
                }
                else
                {
                    initStat.ErrorMessage += "The json results could not be saved in file system.";
                }
            }
            catch (Exception x)
            {
                initStat.ErrorMessage += x.Message;
            }
            return bHasStatResult;
        }
    }
}
