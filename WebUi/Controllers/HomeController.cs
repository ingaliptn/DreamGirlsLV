using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUi.Lib;
using WebUi.Models;

namespace WebUi.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;

        public HomeController(IEscortRepository escortRepository,
            ITextRepository textRepository,
            IMenuRepository menuRepository,
            IMapper mapper,
            IWebHostEnvironment env,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HomeController> logger) : base (escortRepository, textRepository, menuRepository, memoryCache)
        {
            _logger = logger;
            _mapper = mapper;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

#if !DEBUG
                [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        public async Task<IActionResult> Index(string name)
        {
            var m = new HomeViewModel();
            var escorts = await GetAllEscorts();

            foreach (var i in escorts.Select(p => _mapper.Map<HomeViewItem>(p)))
            {
                m.List.Add(i);
            }

            var list = await GetAllTexts();

            m.PositionHomeTopTitle = list
                .Where(z => z.Position == "PositionHomeTopTitle").Select(z => z.Description)
                .FirstOrDefault();
            m.PositionHomeTop = list
                .Where(z => z.Position == "PositionHomeTop").Select(z => z.Description)
                .FirstOrDefault();
            m.PositionHomeBottom = list
                .Where(z => z.Position == "PositionHomeBottom").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.BackGroundImage = "bg_home.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();
            ViewBag.SiteTitle = list
                .Where(z => z.Position == "SiteTitleHome").Select(z => z.Description)
                .FirstOrDefault();
            ViewBag.SiteDescription = list
                .Where(z => z.Position == "SiteDescriptionHome").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.MenuEscorts = await GetAllMenu();

            ViewBag.GoogleAnalyticsObject = list
                .Where(z => z.Position == "GoogleAnalyticsObject").Select(z => z.Description)
                .FirstOrDefault();

            return View(m);
        }

        private string GetCanonicalUrl()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var request = _httpContextAccessor.HttpContext.Request;
                return string.Concat(
                    request.Scheme,
                    "://",
                    request.Host.ToUriComponent(),
                    request.PathBase.ToUriComponent(),
                    request.Path.ToUriComponent(),
                    request.QueryString.ToUriComponent());
            }

            return string.Empty;
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("dreamgirls-blog.php")]
        public IActionResult Blog()
        {
            return View();
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("dreamgirls-blog/{name}")]
        public IActionResult DreamgirlsBlog(string name)
        {
            switch (name)
            {
                case "large-breasted-escorts.php":
                    return View("BlogLargeBreasted");
                case "where-to-find-escorts-in-las-vegas.php":
                    return View("BlogFindLasVegas");
                case "pretty-lady-escorts-in-las-vegas.php":
                    return View("BlogPrettyLadys");
                case "picking-up-girls-is-easy-in-las-vegas.php":
                    return View("BlogPicking");
                case "how-to-get-an-escort-in-vegas.php":
                    return View("BlogHowToGet");
                case "why-you-should-avoid-backpage-las-vegas-escorts.php":
                    return View("BlogWhyYou");
                case "las-vegas-the-city-that-never-sleeps.php":
                    return View("BlogNeverSleeps");
                case "las-vegas-escort-prices.php":
                    return View("BlogPicking");
                default:
                    return RedirectToAction("Error");
            }
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("dinner-dates.php")]
        public IActionResult DinnerDates()
        {
            return View();
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("about.php")]
        public async Task<IActionResult> AboutUs()
        {
            var texts = await GetAllTexts();
            var m = new AboutUsViewModel
            {
                PositionAbout = texts.Where(z => z.Position == "PositionAbout")
                    .Select(z => z.Description).Single(),
                SiteDescriptionAbout = texts.Where(z => z.Position == "SiteDescriptionPageAbout")
                    .Select(z => z.Description).Single()
            };

            ViewBag.BackGroundImage = $"{WorkLib.GetRandomNumber(2,15)}.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();
            ViewBag.SiteTitle = texts.Where(z => z.Position == "SiteTitleAbout")
                .Select(z => z.Description).FirstOrDefault();
            ViewBag.SiteDescription = texts.Where(z => z.Position == "SiteDescriptionAbout").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.MenuEscorts = await GetAllMenu();

             ViewBag.GoogleAnalyticsObject = texts.Where(z => z.Position == "GoogleAnalyticsObject").Select(z => z.Description)
                .FirstOrDefault();

            return View(m);
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("body-rubs.php")]
        public async Task<IActionResult> BodyRubs()
        {
            var m = new BodyRubsViewModel();

            var escorts = await GetAllEscorts();
            var texts = await GetAllTexts();
            
            var list = escorts.Where(z => z.Statistics == "C-Cup").ToList();

            foreach (var i in list.Select(p => _mapper.Map<HomeViewItem>(p)))
            {
                m.List.Add(i);
            }

            m.PositionBodyRubsTitle = texts.Where(z => z.Position == "PositionBodyRubsTitle").Select(z => z.Description)
                .FirstOrDefault();
            m.PositionBodyRubsTop = texts.Where(z => z.Position == "PositionBodyRubsTop").Select(z => z.Description)
                .FirstOrDefault();
            m.PositionBodyRubsBottom = texts.Where(z => z.Position == "PositionBodyRubsBottom").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.BackGroundImage = $"{WorkLib.GetRandomNumber(2, 15)}.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();
            ViewBag.SiteTitle = texts.Where(z => z.Position == "SiteTitleBodyRubs").Select(z => z.Description)
                .FirstOrDefault();
            ViewBag.SiteDescription = texts.Where(z => z.Position == "SiteDescriptionBodyRubs").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.MenuEscorts = await GetAllMenu();

            ViewBag.GoogleAnalyticsObject = texts.Where(z => z.Position == "GoogleAnalyticsObject").Select(z => z.Description)
                .FirstOrDefault();

            return View(m);
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("erotic-massage.php")]
        public async Task<IActionResult> Massage()
        {
            var m = new MassageViewModel();

            var escorts = await GetAllEscorts();
            var texts = await GetAllTexts();

            var list = escorts.Where(z => z.SiteName == Constants.SiteName && z.Statistics == "B-Cup")
                .ToList();

            foreach (var i in list.Select(p => _mapper.Map<HomeViewItem>(p)))
            {
                m.List.Add(i);
            }
           
            m.PositionMassageTitle = texts.Where(z => z.Position == "PositionMassageTitle").Select(z => z.Description)
                .FirstOrDefault();
            m.PositionMassageTop = texts.Where(z => z.Position == "PositionMassageTop").Select(z => z.Description)
                .FirstOrDefault();
            m.PositionMassageBottom = texts.Where(z => z.Position == "PositionMassageBottom").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.BackGroundImage = $"{WorkLib.GetRandomNumber(2, 15)}.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();
            ViewBag.SiteTitle = texts.Where(z => z.Position == "SiteTitleEroticMassage").Select(z => z.Description)
                .FirstOrDefault();
            ViewBag.SiteDescription = texts.Where(z => z.Position == "SiteDescriptionEroticMassage").Select(z => z.Description)
                .FirstOrDefault();
            ViewBag.MenuEscorts = await GetAllMenu();
            ViewBag.GoogleAnalyticsObject = texts.Where(z => z.Position == "GoogleAnalyticsObject").Select(z => z.Description)
                .FirstOrDefault();

            return View(m);
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("{name}")]
        public async Task<IActionResult> Escorts(string name)
        {
            var m = new EscortsViewModel();

            var escorts = await GetAllEscorts();
            var texts = await GetAllTexts();
            var menu = await GetAllMenu();

            List<Escort> list;
            string title;
            string description;
            var menuName =menu.Where(z => z.Href == name).Select(z => z.Name).FirstOrDefault();

            if (menuName == null)//WE COVER MENU
            {
                var escortsSub = new List<Menu>
                {
                    new Menu {Name = "Lombard Escorts", Href = "lombard-escorts.php"},
                    new Menu {Name = "Naperville Escorts", Href = "naperville-escorts.php"},
                    new Menu {Name = "O’Hare Escorts", Href = "ohare-escorts.php"},
                    new Menu {Name = "Schaumburg Escorts", Href = "schaumburg-escorts.php"},
                    new Menu {Name = "Skokie Escorts", Href = "skokie-escorts.php"}
                };
                menuName = escortsSub.Where(z => z.Href == name).Select(z => z.Name).Single();
            }

            m.EscortsNavTitle = menuName;
            menuName = menuName.Replace(" ", "-");
            m.EscortsHeading = await GetEscortsHeading($"EscortsHeading{menuName}");

            m.PositionEscortsTop = texts.Where(z => z.Position == $"PositionEscorts{menuName}Top")
                .Select(z => z.Description).Single();
            m.PositionEscortsBottom = texts.Where(z => z.Position == $"PositionEscorts{menuName}Bottom")
                .Select(z => z.Description).Single();


            switch (name)
            {
                case "asian-escorts.php":
                    title = "SiteTitleAsian";
                    description = "SiteDescriptionAsian";
                    list = escorts.Where(z =>z.Nationality == "Asian").ToList();
                    break;

                case "bbw-escorts.php":
                    title = "SiteTitleBBW";
                    description = "SiteDescriptionBbw";
                    list = escorts.Where(z => z.Statistics == "D-Cup").ToList();
                    break;

                case "cheap-escorts.php":
                    title = "SiteTitleCheap";
                    description = "SiteDescriptionCheap";
                    list = escorts.Where(z => z.Statistics == "C-Cup" && z.Hair == "Blonde").ToList();
                    break;

                case "ebony-escorts.php":
                    title = "SiteTitleEbony";
                    description = "SiteDescriptionEbony";
                    list = escorts.Where(z => z.Nationality == "Ebony").ToList();
                    break;

                case "gfe-escorts.php":
                    title = "SiteTitleGfeEscorts";
                    description = "SiteDescriptionGfeEscorts";
                    list = escorts.Where(z => z.Statistics == "B-Cup" && z.Age < 22).ToList();
                    break;

                case "latina-escorts.php":
                    title = "SiteTitleLatina";
                    description = "SiteDescriptionLatina";
                    list = escorts.Where(z => z.Nationality == "Latina").ToList();
                    break;

                case "mature-escorts.php":
                    title = "SiteTitleMature";
                    description = "SiteDescriptionMature";
                    list = escorts.Where(z => z.Age >= 30).ToList();
                    break;

                case "black-escorts.php":
                    title = "SiteTitleBlack";
                    description = "SiteDescriptionBlack";
                    list = escorts.Where(z => z.Nationality == "Ebony").ToList();
                    break;

                case "busty-escorts.php":
                    title = "SiteTitleBusty";
                    description = "SiteDescriptionBusty";
                    list = escorts.Where(z => z.Bust > 3).ToList();
                    break;

                case "girlfriend-experience.php":
                    title = "SiteTitleGirlfriend";
                    description = "SiteDescriptionGirlfriend";
                    list = escorts.Where(z => z.Statistics == "B-Cup" || z.Statistics == "D-Cup").Take(8).ToList();
                    break;

                case "big-booty-escorts.php":
                    title = "SiteTitleBigBooty";
                    description = "SiteDescriptionBigBooty";
                    list = escorts.Where(z => z.Weight > 121).ToList();
                    break;

                case "checap-escorts.php":
                    title = "SiteTitleChecapEscorts";
                    description = "SiteDescriptionChecapEscorts";
                    list = escorts.Where(z => z.Statistics == "D-Cup").ToList();
                    break;

                case "gfe.php":
                    title = "SiteTitleGfe";
                    description = "SiteDescriptionGfe";
                    list = escorts.Where(z => z.Statistics == "B-Cup" && z.Age < 22).ToList();
                    break;

                case "blonde-escorts.php":
                    title = "SiteTitleBlondeEscorts";
                    description = "SiteDescriptionBlondeEscorts";
                    list = escorts.Where(z => z.Hair == "Blonde").ToList();
                    break;

                case "russian-escorts.php":
                    title = "SiteTitleRussianEscorts";
                    description = "SiteDescriptionRussianEscorts";
                    list = escorts.Where(z => z.Nationality == "Russian").ToList();
                    break;

                case "vip-escorts.php":
                    title = "SiteTitleVipEscorts";
                    description = "SiteDescriptionVipEscorts";
                    list = escorts.Where(z => z.Age < 24 && z.Statistics == "C-Cup").ToList();
                    break;

                case "lombard-escorts.php":
                    title = "SiteTitleLombardEscorts";
                    description = "SiteDescriptionLombardEscorts";
                    list = escorts.Where(z => z.Statistics == "B-Cup" && z.Weight < 113).ToList();
                    break;

                case "naperville-escorts.php":
                    title = "SiteTitleNapervilleEscorts";
                    description = "SiteDescriptionNapervilleEscorts";
                    list = escorts.Where(z => z.Statistics == "B-Cup").ToList();
                    break;

                case "ohare-escorts.php":
                    title = "SiteTitleOhareEscorts";
                    description = "SiteDescriptionOhareEscorts";
                    list = escorts.Where(z => z.Age < 22 && z.Statistics == "C-Cup").ToList();
                    break;

                case "schaumburg-escorts.php":
                    title = "SiteTitleSchaumburgEscorts";
                    description = "SiteDescriptionSchaumburgEscorts";
                    list = escorts.Where(z => z.Statistics == "D-Cup").ToList();
                    break;

                case "skokie-escorts.php":
                    title = "SiteTitleSkokieEscorts";
                    description = "SiteDescriptionSkokieEscorts";
                    list = escorts.Where(z => z.Age > 22 && z.Statistics == "C-Cup").ToList();
                    break;

                default:
                    title = "SiteTitleRussianEscorts";
                    description = "SiteDescriptionRussianEscorts";
                    list = escorts.Where(z => z.Nationality == "Russian").ToList();
                    break;
            }


            foreach (var i in list.Select(p => _mapper.Map<HomeViewItem>(p)))
            {
                m.List.Add(i);
            }

            ViewBag.BackGroundImage = $"{WorkLib.GetRandomNumber(2, 15)}.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();

            ViewBag.SiteTitle = texts.Where(z => z.Position == title).Select(z => z.Description)
                .FirstOrDefault();
            ViewBag.SiteDescription = texts.Where(z => z.Position == description).Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.MenuEscorts = menu;
            ViewBag.GoogleAnalyticsObject = texts.Where(z => z.Position == "GoogleAnalyticsObject").Select(z => z.Description)
                .FirstOrDefault();

            return View(m);
        }

        [Route("robots.txt")]
        public ContentResult RobotsTxt()
        {
            var filePath = Path.Combine(_env.WebRootPath,"robots.txt");
            var s = System.IO.File.ReadAllText(filePath);
            return this.Content(s, "text/plain", Encoding.UTF8);
        }

        [Route("sitemap.xml")]
        public ContentResult SiteMap()
        {
            var filePath = Path.Combine(_env.WebRootPath, "sitemap.xml");
            var s = System.IO.File.ReadAllText(filePath);
            return this.Content(s, "text/plain", Encoding.UTF8);
        }

        private async Task<string> GetEscortsHeading(string position)
        {
            var texts = await GetAllTexts();
            return texts.Where(z => z.Position == position)
                .Select(z => z.Description).FirstOrDefault();
        }

        [Route("{seg1?}/{seg2}")]
        public IActionResult BadUrl()
        {
            return RedirectToAction("Error");
        }

        [Route("404.php")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            ViewBag.BackGroundImage = $"{WorkLib.GetRandomNumber(2, 15)}.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();

            ViewBag.SiteTitle = "";
            ViewBag.SiteDescription = "";

            Response.StatusCode = 404;

            ViewBag.MenuEscorts = await GetAllMenu();

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public string PositionHomeTopTitle { get; set; }
        public string PositionHomeTop { get; set; }
        public string PositionHomeBottom { get; set; }
        public List<HomeViewItem> List { get; set; } = new List<HomeViewItem>();
    }

    public class EscortsViewModel
    {
        public string EscortsHeading { get; set; }
        public string EscortsNavTitle { get; set; }
        public string PositionEscortsTop { get; set; }
        public string PositionEscortsBottom { get; set; }
        public List<HomeViewItem> List { get; set; } = new List<HomeViewItem>();
    }

    public class MassageViewModel
    {
        public string EscortsNavTitle { get; set; }
        public string PositionMassageTitle { get; set; }
        public string PositionMassageTop { get; set; }
        public string PositionMassageBottom { get; set; }
        public List<HomeViewItem> List { get; set; } = new List<HomeViewItem>();
    }

    public class BodyRubsViewModel
    {
        public string PositionBodyRubsTitle { get; set; }
        public string PositionBodyRubsTop { get; set; }
        public string PositionBodyRubsBottom { get; set; }
        public List<HomeViewItem> List { get; set; } = new List<HomeViewItem>();
    }

    public class AboutUsViewModel
    {
        public string PositionAbout { get; set; }
        public string SiteDescriptionAbout { get; set; }
        public List<HomeViewItem> List { get; set; } = new List<HomeViewItem>();
    }

    public class HomeViewItem : Escort
    {
        
    }
}
