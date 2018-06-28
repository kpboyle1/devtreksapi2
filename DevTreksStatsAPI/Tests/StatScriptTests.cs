using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevTreks.DevTreksStatsApi.Models;
using DevTreks.DevTreksStatsApi.Helpers;
using DevTreks.DevTreksStatsApi.Client;

namespace DevTreks.DevTreksStatsApi.Tests
{
    /// <summary>
    ///Purpose:		Tests of Controller Api actions
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///References:	CTA Algorithm 2, subalgo 2 (R webapi) and subalgo 3 (python webap)
    ///Notes:       
    ///</summary>
    public class StatScriptTests
    {
        public static async Task<StatScript> GetAllTest(IStatScriptRepository StatScriptRep, string statType)
        {
            //only runs when first stat.IsDevelopment = true;
            StatScript testStat = StatScript.FillInDebugStatScript(StatScriptRep, statType);

            //also runs the Create controller action to test running the stat scripts
            if (testStat.IsDevelopment && (!string.IsNullOrEmpty(testStat.Key)))
            {
                Uri uri = await ClientProgram.ClientCreate(testStat);
            }

            return testStat;
        }
        
        
        public static async Task<StatScript> UpdateTest(IStatScriptRepository StatScriptRep, 
            StatScript statScript)
        {
            StatScript testStat = new StatScript(statScript);
            if (!statScript.IsDevelopment)
            {
                return testStat;
            }
            Uri uri = await ClientProgram.ClientUpdate(testStat);
            return testStat;
        }
        public static async Task<StatScript> DeleteTest(IStatScriptRepository StatScriptRep, 
            StatScript statScript)
        {
            StatScript testStat = new StatScript(statScript);
            if (!statScript.IsDevelopment)
            {
                return testStat;
            }
            Uri uri = await ClientProgram.ClientDelete(testStat);
            return testStat;
        }
    }
}
