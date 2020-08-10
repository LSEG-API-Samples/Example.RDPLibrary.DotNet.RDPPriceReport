using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RdpRealTimePricing.Model.Data;
using System.IO;
using CsvHelper;
using System.Dynamic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Cors;
using RdpPriceReportApp.Pages;

namespace RdpPriceReportApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
       
        [HttpGet("[action]")]
        public FileResult CSV(string username)
        {
            var services = this.HttpContext.RequestServices;
            var AppDataList = (ExportData)services.GetService(typeof(ExportData));
            byte[] data = new byte[] { };
            if (AppDataList.RawData.ContainsKey(username))
            {
                var AppData = AppDataList.RawData[username];
                using (var memroystream = new MemoryStream())
                using (var writer = new StreamWriter(memroystream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    List<dynamic> records = new List<dynamic>();
                    foreach (var entry in AppData.DataCache.OrderBy(x=>x.Key).ToList())
                    {

                        var dataItem = new ExpandoObject() as IDictionary<string, dynamic>;
                        dataItem.Add("RIC", entry.Key);
                        foreach (var key in AppData.columnValues.ToList<string>())
                        {
                            var priceValue = (entry.Value.DynamicFields as IDictionary<string, dynamic>);
                            if (priceValue.ContainsKey(key))
                                dataItem.Add(key, priceValue[key]);
                            else
                                dataItem.Add(key, string.Empty);
                        }
                        records.Add(dataItem);

                    }
                    csv.WriteRecords(records);
                    writer.Flush();
                    data = memroystream.ToArray();

                }
            }
            //result = new FileStreamResult(new MemoryStream(data), "application/octet-stream");
            //result.FileDownloadName = "ExportCSV_"
            //return result;
            AppDataList.RawData.TryRemove(username, out var removedData);
            return File(data, "text/csv", "ExportData.csv");

        }
        [HttpGet("[action]")]
        public FileResult RICList(string username)
        {
            var services = this.HttpContext.RequestServices;
            var AppDataList = (ExportData)services.GetService(typeof(ExportData));
            byte[] data = new byte[] { };
            if (AppDataList.RawData.ContainsKey(username))
            {
                var AppData = AppDataList.RawData[username];
                using (var memroystream = new MemoryStream())
                using (var writer = new StreamWriter(memroystream))
                {
                    var ricList = string.Join(',',AppData.DataCache.Keys.OrderBy(item => item));
                    writer.Write(ricList);
                    writer.Flush();
                    data = memroystream.ToArray();
                }
            }

            AppDataList.RawData.TryRemove(username, out var removedData);
            return File(data, "text/plain", "RIC.txt");

        }
    }
}
