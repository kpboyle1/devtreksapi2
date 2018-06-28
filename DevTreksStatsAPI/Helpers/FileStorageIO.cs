using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Net.Http;
using DevTreks.DevTreksStatsApi.Models;

namespace DevTreks.DevTreksStatsApi.Helpers
{
    /// <summary>
    ///Purpose:		File storage utilities
    ///Author:		www.devtreks.org
    ///Date:		2016, September
    ///</summary>
    public class FileStorageIO
    {
        public const string FILE_PATH_DELIMITER = @"\";
        public static char[] FILE_PATH_DELIMITERS = FILE_PATH_DELIMITER.ToCharArray();
        public const string WEBFILE_PATH_DELIMITER = "/";
        public static char[] WEBFILE_PATH_DELIMITERS = new char[] { '/' };
        public const string DOUBLEQUOTE = "\"";

        public static async Task<string> SaveURLInTempFile(StatScript statScript,
            string url)
        {
            //statistical packages run using console process must be in filesystem
            //r can't handle urls to script files
            //async url to filesystem conversion
            string sURLContent = await ReadTextAsync(url);
            string sTempFilePath = GetTempFilePath(statScript, url);
            bool bIsSaved = await SaveStringInFilePath(statScript, sTempFilePath, sURLContent);
            return sTempFilePath;
        }
        public static async Task<string> SaveURLInTempFile(StatScript statScript,
            string url, string existingFilePath)
        {
            //statistical packages run using console process must be in filesystem
            //r can't handle urls to script files
            //async url to filesystem conversion
            string sURLContent = await ReadTextAsync(url);
            string sWebFileName = GetLastSubString(url, WEBFILE_PATH_DELIMITER);
            string sFileName = GetLastSubString(existingFilePath, FILE_PATH_DELIMITER);
            string sTempFilePath = existingFilePath.Replace(sFileName, sWebFileName);
            bool bIsSaved = await SaveStringInFilePath(statScript, sTempFilePath, sURLContent);
            return sTempFilePath;
        }
       
        private static async Task<string> ReadTextAsync(string dataURL)
        {
            string sContent = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(dataURL);
                //could also check request.haveresponse
                if (response != null)
                {
                    //retrieve the website contents from the HttpResponseMessage. 
                    byte[] buffer = await response.Content.ReadAsByteArrayAsync();
                    //standard protocol in ReadTextAsync
                    sContent = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                }
            }
            return sContent;
        }
        public static async Task<bool> SaveContentInFile(StatScript statScript, 
            string dataFilePath, string content)
        {
            bool bHasSaved = false;
            //datafilepath must always be a filesystem csv file, but output can be just text
            string sFilePath = dataFilePath.Replace(".csv", "out.txt");
            bHasSaved = await SaveStringInFilePath(statScript, sFilePath, content);
            //convert the filepath to a url for auditing and retrieval
            statScript.OutputURL = GetURLFromFilePath(statScript, sFilePath);
            return bHasSaved;
        }
        private static async Task<bool> SaveStringInFilePath(StatScript statScript,
            string filePath, string content)
        {
            bool bIsSaved = false;
            using (FileStream fileStream = new FileStream(filePath,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    await sw.WriteAsync(content);
                    bIsSaved = true;
                }
            }
            return bIsSaved;
        }
        
        private static string GetTempFilePath(StatScript statScript, string urlPath)
        {
            string sURLPath = ChangeScriptExtension(statScript, urlPath);
            string sFileName = GetLastSubString(sURLPath, WEBFILE_PATH_DELIMITER);

           
            string sResourcesDir = string.Concat(statScript.DefaultRootFullFilePath, "resources");
            
            if (!statScript.DefaultRootFullFilePath.Contains("wwwroot"))
            {
                string sWebRoot = string.Concat(statScript.DefaultRootFullFilePath,
                    "wwwroot");
                if (!Directory.Exists(sWebRoot))
                {
                    Directory.CreateDirectory(sWebRoot);
                }
                //the release build doesn't include wwwroot in path
                sResourcesDir = string.Concat(sWebRoot, FILE_PATH_DELIMITER, "resources");
            }
            else
            {
                if (!Directory.Exists(statScript.DefaultRootFullFilePath))
                {
                    Directory.CreateDirectory(statScript.DefaultRootFullFilePath);
                }
            }
            
            if (!Directory.Exists(sResourcesDir))
            {
                Directory.CreateDirectory(sResourcesDir);
            }
            string sTempDir = string.Concat(sResourcesDir,
                FILE_PATH_DELIMITER, "temp");
            if (!Directory.Exists(sTempDir))
            {
                Directory.CreateDirectory(sTempDir);
            }
            //key allows easier debug
            string sGuidDir = string.Concat(sTempDir,
                FILE_PATH_DELIMITER, statScript.Key);
            if (!Directory.Exists(sGuidDir))
            {
                Directory.CreateDirectory(sGuidDir);
            }
            string sTempFilePath = string.Concat(sGuidDir, FILE_PATH_DELIMITER, sFileName);
            return sTempFilePath;
        }
        private static string ChangeScriptExtension(StatScript statScript, string urlPath)
        {
            string sNewURLPath = urlPath;
            //script files from devtreks use "txt" extension
            //convert to either pyw or R extension
            if (statScript.ScriptURL == urlPath)
            {
                if (statScript.StatType == StatScript.STAT_TYPE.py.ToString())
                {
                    sNewURLPath = sNewURLPath.Replace(".txt", ".pyw");
                }
                else if (statScript.StatType == StatScript.STAT_TYPE.julia.ToString())
                {
                    sNewURLPath = sNewURLPath.Replace(".txt", ".j");
                }
                else
                {
                    sNewURLPath = sNewURLPath.Replace(".txt", ".R");
                }
            }
            return sNewURLPath;
        }
        private static string GetURLFromFilePath(StatScript statScript, string filePath)
        {
            string sTempURL = string.Empty;
            //standard convention in devtreks
            sTempURL = ConvertPathFileandWeb(statScript, filePath);
            return sTempURL;
        }
        public static string GetLastSubString(string delimitedString, string delimiter)
        {
            string sSubstring = string.Empty;
            if (delimitedString != string.Empty && delimitedString != null)
            {
                int iParamIndex = delimitedString.LastIndexOf(delimiter);
                if (iParamIndex > 0)
                {
                    int iSubstringLength = delimitedString.Length - iParamIndex - 1;
                    sSubstring = delimitedString.Substring(iParamIndex + 1, iSubstringLength);
                }
                else
                {
                    sSubstring = delimitedString;
                }
            }
            return sSubstring;
        }
        public static string ConvertPathFileandWeb(StatScript statScript, string path)
        {
            string sConvertedPath = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                if (Path.IsPathRooted(path))
                {
                    sConvertedPath = path.Replace(
                        statScript.DefaultRootFullFilePath,
                        statScript.DefaultRootWebStoragePath);
                    sConvertedPath = sConvertedPath.Replace(
                        FILE_PATH_DELIMITER, WEBFILE_PATH_DELIMITER);
                }
                else
                {
                    sConvertedPath = path.Replace(
                        statScript.DefaultRootWebStoragePath,
                        statScript.DefaultRootFullFilePath);
                    sConvertedPath = sConvertedPath.Replace(
                        WEBFILE_PATH_DELIMITER, FILE_PATH_DELIMITER);
                }
            }
            return sConvertedPath;
        }
        public static string CleanScriptforResponseBody(StringBuilder sb)
        {
            //clean anything that interferes with a string response
            string sCleanScript = sb.ToString().Replace(FileStorageIO.DOUBLEQUOTE, string.Empty);
            return sCleanScript;
        }

    }
}
