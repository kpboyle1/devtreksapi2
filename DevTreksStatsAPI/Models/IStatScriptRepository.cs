using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevTreks.DevTreksStatsApi.Models
{
    /// <summary>
    ///Purpose:		Data repository interface for running statistical scripts
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///References:	CTA agorithms 2 (R) and 3 (Python) in Tech Assess 01 tutorial
    ///</summary>
    public interface IStatScriptRepository
    {
        void Add(StatScript item);
        IEnumerable<StatScript> GetAll();
        StatScript Find(string key);
        StatScript Remove(string key);
        void Update(StatScript item);
    }
}
