using RestSharp;
using RestSharp.Authenticators;
using EliteSTO.Configuration;
using EliteSTO.umbraco.Models.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Microsoft.Extensions.Logging;

namespace EliteSTO.Controllers.Surface;

public class ContactSurfaceController : SurfaceController
{
    private readonly ILogger<ContactSurfaceController> _logger;
    private readonly EliteSTOConfig _eliteSTOConfig;
    public ContactSurfaceController(
        IUmbracoContextAccessor umbracoContextAccessor, 
        IUmbracoDatabaseFactory databaseFactory, 
        ServiceContext services,
        AppCaches appCaches, 
        IProfilingLogger profilingLogger, 
        IPublishedUrlProvider publishedUrlProvider,
        ILogger<ContactSurfaceController> logger,
        IOptions<EliteSTOConfig> eliteSTOConfig) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _logger = logger;
        _eliteSTOConfig = eliteSTOConfig.Value;
    }

    public async Task<IActionResult> Submit(ContactViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        try
        {
            var subject = string.Format("Contact Form Submission From: {0}", model.Name);
            var emailMessage = 
                $"<div style=\"font-family:Arial;font-size:15px;\">" +
                $"<h2 style=\"color:#C98009\">New EliteSTO Contact Form Submission</h2>" +
                $"<b>Name:</b> {model.Name}<br><br><b>Email:</b> {model.Email}<br><br><b>Phone:</b> {model.Phone}" +
                $"<br><br><b>Company:</b> {model.CompanyName}<br><br><b>Country:</b> {model.Country}<br><hr>" +
                $"<b>Message:</b> {model.Message}" +
                $"</div>";

            var sentEmail = await Send(
                _eliteSTOConfig?.EmailSettings?.From, 
                _eliteSTOConfig?.EmailSettings?.To,
                subject,
                emailMessage
            );
            _logger.LogInformation(sentEmail.ToString());

            TempData["ContactSuccess"] = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Contact Form Submission Error");
            TempData["ContactSuccess"] = false;
        }

        //return RedirectToCurrentUmbracoPage(QueryString.Create("#", ""));
        var url = HttpContext.Request.GetDisplayUrl();
        return Redirect(url + "#contact");
    }

    public async Task<RestResponse> Send(string From, string To, string subject, string message)
    {
        if(_eliteSTOConfig?.EmailSettings?.ApiKey is null)
        {
            throw new BadHttpRequestException("missing api key");
        }

        var options = new RestClientOptions("https://api.mailgun.net")
        {
            Authenticator = new HttpBasicAuthenticator("api", _eliteSTOConfig?.EmailSettings?.ApiKey)
        };

        var client = new RestClient(options);
        var request = new RestRequest(_eliteSTOConfig?.EmailSettings?.MailgunEndpoint, Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", From);
        request.AddParameter("to", To);
        request.AddParameter("subject", subject);
        request.AddParameter("html", message);
        return await client.ExecuteAsync(request);
    }

    public static readonly List<string> countries =
    [
        "United States of America",
        "United Kingdom",
        "Afghanistan",
        "Albania",
        "Algeria",
        "Andorra",
        "Angola",
        "Antigua and Barbuda",
        "Argentina",
        "Armenia",
        "Australia",
        "Austria",
        "Azerbaijan",
        "Bahamas",
        "Bahrain",
        "Bangladesh",
        "Barbados",
        "Belarus",
        "Belgium",
        "Belize",
        "Benin",
        "Bhutan",
        "Bolivia",
        "Bosnia and Herzegovina",
        "Botswana",
        "Brazil",
        "Brunei",
        "Bulgaria",
        "Burkina Faso",
        "Burundi",
        "Cabo Verde",
        "Cambodia",
        "Cameroon",
        "Canada",
        "Central African Republic",
        "Chad",
        "Chile",
        "China",
        "Colombia",
        "Comoros",
        "Congo (Congo-Brazzaville)",
        "Costa Rica",
        "Croatia",
        "Cuba",
        "Cyprus",
        "Czech Republic",
        "Democratic Republic of the Congo",
        "Denmark",
        "Djibouti",
        "Dominica",
        "Dominican Republic",
        "Ecuador",
        "Egypt",
        "El Salvador",
        "Equatorial Guinea",
        "Eritrea",
        "Estonia",
        "Eswatini (fmr. Swaziland)",
        "Ethiopia",
        "Fiji",
        "Finland",
        "France",
        "Gabon",
        "Gambia",
        "Georgia",
        "Germany",
        "Ghana",
        "Greece",
        "Grenada",
        "Guatemala",
        "Guinea",
        "Guinea-Bissau",
        "Guyana",
        "Haiti",
        "Honduras",
        "Hungary",
        "Iceland",
        "India",
        "Indonesia",
        "Iran",
        "Iraq",
        "Ireland",
        "Israel",
        "Italy",
        "Ivory Coast",
        "Jamaica",
        "Japan",
        "Jordan",
        "Kazakhstan",
        "Kenya",
        "Kiribati",
        "Kuwait",
        "Kyrgyzstan",
        "Laos",
        "Latvia",
        "Lebanon",
        "Lesotho",
        "Liberia",
        "Libya",
        "Liechtenstein",
        "Lithuania",
        "Luxembourg",
        "Madagascar",
        "Malawi",
        "Malaysia",
        "Maldives",
        "Mali",
        "Malta",
        "Marshall Islands",
        "Mauritania",
        "Mauritius",
        "Mexico",
        "Micronesia",
        "Moldova",
        "Monaco",
        "Mongolia",
        "Montenegro",
        "Morocco",
        "Mozambique",
        "Myanmar (formerly Burma)",
        "Namibia",
        "Nauru",
        "Nepal",
        "Netherlands",
        "New Zealand",
        "Nicaragua",
        "Niger",
        "Nigeria",
        "North Korea",
        "North Macedonia",
        "Norway",
        "Oman",
        "Pakistan",
        "Palau",
        "Palestine State",
        "Panama",
        "Papua New Guinea",
        "Paraguay",
        "Peru",
        "Philippines",
        "Poland",
        "Portugal",
        "Qatar",
        "Romania",
        "Russia",
        "Rwanda",
        "Saint Kitts and Nevis",
        "Saint Lucia",
        "Saint Vincent and the Grenadines",
        "Samoa",
        "San Marino",
        "Sao Tome and Principe",
        "Saudi Arabia",
        "Senegal",
        "Serbia",
        "Seychelles",
        "Sierra Leone",
        "Singapore",
        "Slovakia",
        "Slovenia",
        "Solomon Islands",
        "Somalia",
        "South Africa",
        "South Korea",
        "South Sudan",
        "Spain",
        "Sri Lanka",
        "Sudan",
        "Suriname",
        "Sweden",
        "Switzerland",
        "Syria",
        "Taiwan",
        "Tajikistan",
        "Tanzania",
        "Thailand",
        "Timor-Leste",
        "Togo",
        "Tonga",
        "Trinidad and Tobago",
        "Tunisia",
        "Turkey",
        "Turkmenistan",
        "Tuvalu",
        "Uganda",
        "Ukraine",
        "United Arab Emirates",
        "United Kingdom",
        "United States of America",
        "Uruguay",
        "Uzbekistan",
        "Vanuatu",
        "Vatican City",
        "Venezuela",
        "Vietnam",
        "Yemen",
        "Zambia",
        "Zimbabwe"
    ];
}
