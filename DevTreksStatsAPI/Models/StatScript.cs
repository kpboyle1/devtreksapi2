using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevTreks.DevTreksStatsApi.Helpers;
using DevTreks.DevTreksStatsApi.Client;

namespace DevTreks.DevTreksStatsApi.Models
{
    /// <summary>
    ///Purpose:		POCO Model for running statistical scripts
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///References:	CTA Algorithm 2, subalgo 2 (R webapi) and subalgo 3 (python webap)
    ///Notes:       More sophisticated webapi apps may want to use standard DevTreks.ContentURI composite model
    ///</summary>
    public class StatScript
    {
        public StatScript()
        {
            this.Key = string.Empty;
            this.Name = string.Empty;
            this.DateCompleted = string.Empty;
            this.DataURL = string.Empty;
            this.ScriptURL = string.Empty;
            this.OutputURL = string.Empty;
            this.StatType = string.Empty;
            this.RExecutablePath = string.Empty;
            this.PyExecutablePath = string.Empty;
            this.JuliaExecutablePath = string.Empty;
            this.DefaultRootFullFilePath = string.Empty;
            this.DefaultRootWebStoragePath = string.Empty;
            this.DefaultWebDomain = string.Empty;
            this.StatisticalResult = string.Empty;
            this.IsComplete = false;
            this.IsDevelopment = false;
            this.ErrorMessage = string.Empty;
        }
        public StatScript(StatScript statScript)
        {
            this.Key = statScript.Key;
            this.Name = statScript.Name;
            this.DateCompleted = statScript.DateCompleted;
            this.DataURL = statScript.DataURL;
            this.ScriptURL = statScript.ScriptURL;
            this.OutputURL = statScript.OutputURL;
            this.StatType = statScript.StatType;
            this.RExecutablePath = statScript.RExecutablePath;
            this.PyExecutablePath = statScript.PyExecutablePath;
            this.JuliaExecutablePath = statScript.JuliaExecutablePath;
            this.DefaultRootFullFilePath = statScript.DefaultRootFullFilePath;
            this.DefaultRootWebStoragePath = statScript.DefaultRootWebStoragePath;
            this.DefaultWebDomain = statScript.DefaultWebDomain;
            this.StatisticalResult = statScript.StatisticalResult;
            this.IsComplete = statScript.IsComplete;
            this.IsDevelopment = statScript.IsDevelopment;
            this.ErrorMessage = statScript.ErrorMessage;
        }
        public enum STAT_TYPE
        {
            none = 0,
            r = 1,
            py = 2,
            aml = 3,
            julia = 4
        }
        //first 1 prop set by api
        public string Key { get; set; }
        //these 4 properties are set by client and sent as POCO object
        public string Name { get; set; }
        public string DateCompleted { get; set; }
        public string DataURL { get; set; }
        public string ScriptURL { get; set; }
        public string OutputURL { get; set; }
        //the client sends this to host
        public string StatType { get; set; }
        //the host sets these 4 properties using di from appsettings
        public string RExecutablePath { get; set; }
        public string PyExecutablePath { get; set; }
        public string JuliaExecutablePath { get; set; }
        public string DefaultRootFullFilePath { get; set; }
        public string DefaultRootWebStoragePath { get; set; }
        public string DefaultWebDomain { get; set; }
        public string StatisticalResult { get; set; }
        //set by api
        public bool IsComplete { get; set; }
        public bool IsDevelopment { get; set; }
        public string ErrorMessage { get; set; }
        public static STAT_TYPE GetStatType(string executablepath)
        {
            STAT_TYPE eStatType = STAT_TYPE.none;
            if (executablepath.Contains("python"))
            {
                eStatType = STAT_TYPE.py;
            }
            else if (executablepath.Contains("julia"))
            {
                eStatType = STAT_TYPE.julia;
            }
            else
            {
                eStatType = STAT_TYPE.r;
            }
            //aml addressed when subalgo 4 is debugged
            return eStatType;
        }
        public static void FillInRepositoryStatScriptProperties(IStatScriptRepository StatScriptRep,
            StatScript newStatScript)
        {
            int i = 0;
            foreach (var statscript in StatScriptRep.GetAll())
            {
                if (i == 0)
                {
                    //repository constructor adds a statscript by default 
                    //which includes host scriptexecutable paths and isdevelopment property
                    newStatScript.RExecutablePath = statscript.RExecutablePath;
                    newStatScript.PyExecutablePath = statscript.PyExecutablePath;
                    newStatScript.JuliaExecutablePath = statscript.JuliaExecutablePath;
                    newStatScript.DefaultRootFullFilePath = statscript.DefaultRootFullFilePath;
                    newStatScript.DefaultRootWebStoragePath = statscript.DefaultRootWebStoragePath;
                    newStatScript.DefaultWebDomain = statscript.DefaultWebDomain;
                    newStatScript.IsDevelopment = statscript.IsDevelopment;
                    break;
                }
            }
        }
        public static StatScript FillInDebugStatScript(IStatScriptRepository StatScriptRep, string statType)
        {
            //used to test the post http (create) controller action in web api
            //client in DevTreks posts directly to create controller and doesn't use this at all
            StatScript testStat = new StatScript();
            int i = 0;
            foreach (var statscript in StatScriptRep.GetAll())
            {
                //first statscript is dep injected into repository with 
                if (i == 0)
                {
                    if (statscript.IsDevelopment)
                    {
                        statscript.Key = Guid.NewGuid().ToString();
                        statscript.Name = "TestGetAll()";
                        //devtreks has to be installed on localhost and these resources previewed
                        statscript.DataURL = "https://devtreks1.blob.core.windows.net/resources/network_carbon/resourcepack_1534/resource_7969/Regress1.csv";
                        //MAKE SURE to run DevTreks.exe to start listening to localhost:5000
                        //statscript.DataURL = "http://localhost:5000/resources/network_carbon/resourcepack_526/resource_1771/Regress1.csv";
                        statscript.OutputURL = string.Empty;

                        if (statType == StatScript.STAT_TYPE.py.ToString())
                        {
                            //pytest
                            statscript.ScriptURL = "https://devtreks1.blob.core.windows.net/resources/network_carbon/resourcepack_1534/resource_7967/PyOLSWeb1.txt";
                            //statscript.ScriptURL = */"http://localhost:5000/resources/network_carbon/resourcepack_526/resource_1767/PyOLSWeb1.txt";
                            statscript.StatType = StatScript.STAT_TYPE.py.ToString();
                        }
                        else if (statType == StatScript.STAT_TYPE.julia.ToString())
                        {
                            //juliatest
                            statscript.ScriptURL = "to do";
                            statscript.StatType = StatScript.STAT_TYPE.julia.ToString();
                        }
                        else
                        {
                            //rtest
                            statscript.ScriptURL = "https://devtreks1.blob.core.windows.net/resources/network_carbon/resourcepack_1534/resource_7963/R1Web.txt";
                            //statscript.ScriptURL = "http://localhost:5000/resources/network_carbon/resourcepack_526/resource_1765/R1Web.txt";
                            statscript.StatType = StatScript.STAT_TYPE.r.ToString();
                        }
                        

                        statscript.IsComplete = false;
                        testStat = new StatScript(statscript);
                    }
                    break;
                }
            }
            return testStat;
        }
    }
}
