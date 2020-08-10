using System.Text;
using System.Threading.Tasks;
using RdpRealTimePricing.Model.Data;
using Endpoint = Refinitiv.DataPlatform.Delivery.Request.Endpoint;

namespace RdpPriceReportApp.ViewModel
{
    public class RdpCompanyInfo
    {
        private string baseEndpoint = "https://api.refinitiv.com/user-framework/mobile/overview-service/v1/";

        public async Task<CompanyName> GetCompanyNameAsync(Refinitiv.DataPlatform.Core.ISession session, string ricname)
        {
            var companyName = new CompanyName();
            var endpoint = new StringBuilder();
            endpoint.Append(baseEndpoint);
            endpoint.Append($"corp/company-name/{ricname}");
            var response = await Endpoint.SendRequestAsync(session, endpoint.ToString()).ConfigureAwait(true);
            if (response.IsSuccess)
            {
                companyName = response.Data?.Raw?["data"]["companyName"].ToObject<CompanyName>();
            }

            return companyName;
        }

        public async Task<CompanyBusinessSummary> GetCompanyBusinessSummaryAsync(
            Refinitiv.DataPlatform.Core.ISession session, string ricname)
        {
            var companyBusinessSummary = new CompanyBusinessSummary();
            var endpoint = new StringBuilder();
            endpoint.Append(baseEndpoint);
            endpoint.Append($"corp/business-summary/{ricname}");
            var response = await Endpoint.SendRequestAsync(session, endpoint.ToString()).ConfigureAwait(true);
            if (response.IsSuccess)
            {
                companyBusinessSummary =
                    response.Data?.Raw?["data"]["businessSummary"].ToObject<CompanyBusinessSummary>();
            }

            return companyBusinessSummary;
        }
    }
}
