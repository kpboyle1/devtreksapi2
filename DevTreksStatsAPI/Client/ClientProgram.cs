using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using DevTreks.DevTreksStatsApi.Models;
using DevTreks.DevTreksStatsApi.Helpers;

namespace DevTreks.DevTreksStatsApi.Client
{
    /// <summary>
    ///Purpose:		Test running webapi using netcore client apps
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///References:	CTA Algorithm 2, subalgo 2 (R webapi) and subalgo 3 (python webapi)
    ///Note:        DevTreks implements a production client version of code
    ///</summary>
    public class ClientProgram
    {
        public static async Task<Uri> ClientCreate(StatScript statScript)
        {
           
            // HTTP POST example
            HttpClient client = new HttpClient();

            var json = JsonConvert.SerializeObject(statScript);

            // Post statscript
            Uri address = new Uri(string.Concat(statScript.DefaultWebDomain, "api/statscript"));
            Uri outputURL = new Uri(address.ToString());
            try
            {
                //create controller actionresult says this only returns a url 
                //to the created statscript referenced in Location Header
                HttpResponseMessage response =
                    await client.PostAsync(address,
                    new StringContent(json, Encoding.UTF8, "application/json"));
                //can also use .PostAsJson(address, statScript) but requires Microsoft.AspNet.WebApi.Client.5.2.3 package

                // Check that response was successful or throw exception
                response.EnsureSuccessStatusCode();

                // the statistical result of running the statscript : the key to the statscript object created
                //{http://localhost:52958/api/statscript/2e100e5e-997f-4b84-ac69-91b8add6bad2}
                outputURL = response.Headers.Location;
            }
            catch(Exception ex)
            {
                statScript.ErrorMessage = ex.Message;
            }
            //expects {http://localhost:52958/api/statscript/2e100e5e-997f-4b84-ac69-91b8add6bad2}
            //the key is used to run GetById and returns the Json statscript in response body
            return outputURL;
        }

        //used for tests
        public static async Task<StatScript> ClientGetById(StatScript statScript)
        {
            StatScript deserializedScript = new StatScript();
            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(statScript);

            Uri address = new Uri(string.Concat(statScript.DefaultWebDomain,
                    "api/statscript", FileStorageIO.WEBFILE_PATH_DELIMITER, statScript.Key));

            HttpResponseMessage response =
                await client.GetAsync(address);

            // Check that response was successful or throw exception
            response.EnsureSuccessStatusCode();

            //the response body contains the json string result
            string statResult = JsonConvert.SerializeObject(response);
            if (!string.IsNullOrEmpty(statResult))
            {
                string body = await response.Content.ReadAsStringAsync();
                deserializedScript = JsonConvert.DeserializeObject<StatScript>(body);
            }
            return deserializedScript;
        }
        public static async Task<Uri> ClientUpdate(StatScript statScript)
        {
            // HTTP PUT example
            Uri address = new Uri(string.Concat(statScript.DefaultWebDomain,
               "api/statscript", "/", statScript.Key));
            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(statScript);

            //update something
            statScript.DataURL = "url2";

            HttpResponseMessage response =
                await client.PutAsync(address,
                new StringContent(json, Encoding.UTF8, "application/json"));
            

            Uri outputURL = response.Headers.Location;
            return outputURL;
        }
        public static async Task<Uri> ClientDelete(StatScript statScript)
        {
            // HTTP DELETE example
            Uri address = new Uri(string.Concat(statScript.DefaultWebDomain,
                "api/statscript", "/", statScript.Key));
            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(statScript);

            HttpResponseMessage response =
                await client.DeleteAsync(address);
            

            Uri outputURL = response.Headers.Location;
            return outputURL;
        }
    }
}
