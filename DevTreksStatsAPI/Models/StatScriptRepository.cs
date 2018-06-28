using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace DevTreks.DevTreksStatsApi.Models
{
    /// <summary>
    ///Purpose:		Data repository for running statistical scripts
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///References:	CTA Algorithm 2, subalgo 2 (R webapi) and subalgo 3 (python webap)
    ///</summary>
    public class StatScriptRepository : IStatScriptRepository
    {
        private static ConcurrentDictionary<string, StatScript> _statscripts =
              new ConcurrentDictionary<string, StatScript>();
        
        public StatScriptRepository(string name, string dataURL, string scriptURL, string outputURL, 
            string pyExecutable, string rExecutable, string juliaExecutable, string defaultRootFullFilePath, 
            string defaultRootWebStoragePath, string defaultWebDomain, bool isDevelopment)
        {
            Add(new StatScript
            {
                Name = name,
                DataURL = dataURL,
                ScriptURL = scriptURL,
                OutputURL = outputURL,
                PyExecutablePath = pyExecutable,
                RExecutablePath = rExecutable,
                JuliaExecutablePath = juliaExecutable,
                DefaultRootFullFilePath = defaultRootFullFilePath,
                DefaultRootWebStoragePath = defaultRootWebStoragePath,
                DefaultWebDomain = defaultWebDomain,
                IsComplete = false,
                IsDevelopment = isDevelopment
            });
        }
        public IEnumerable<StatScript> GetAll()
        {
            return _statscripts.Values;
        }

        public void Add(StatScript stat)
        {
            stat.Key = Guid.NewGuid().ToString();
            _statscripts[stat.Key] = stat;
        }

        public StatScript Find(string key)
        {
            StatScript stat;
            _statscripts.TryGetValue(key, out stat);
            return stat;
        }

        public StatScript Remove(string key)
        {
            StatScript stat;
            _statscripts.TryRemove(key, out stat);
            return stat;
        }

        public void Update(StatScript stat)
        {
            _statscripts[stat.Key] = stat;
        }
    }
}
