using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Infrastructure;

namespace WebUi.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(IEscortRepository escortRepository,
            IMapper mapper,
            ITextRepository textRepository,
            IMenuRepository menuRepository,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            ILogger<ProfileController> logger) : base(escortRepository, textRepository, menuRepository, memoryCache)
        {
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

#if !DEBUG
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
#endif
        [Route("escorts-profile/{name?}")]
        public async Task<IActionResult> Index(string name)
        {
            var escorts = await GetAllEscorts();
            var texts = await GetAllTexts();

            var escort = escorts.FirstOrDefault(z => z.EscortName.ToLower() == name.Substring(0, name.Length-4));

            if (escort == null) return RedirectToAction("Error", "Home");

            var m = _mapper.Map<ProfileViewModel>(escort);

            var list = escorts.Where(z => z.Id != escort.Id).ToList();

            var rnd = new Random();
            foreach (var r in escorts.Select(p => rnd.Next(escorts.Count - 1)))
            {
                m.List.Add(escorts[r]);
                if (m.List.Count == 8) break;
            }
            m.List = m.List.Where(z=>z.Id != escort.Id).DistinctBy(z => z.Id).Take(4).ToList();

            ViewBag.BackGroundImage = "bg_profile.jpg";
            ViewBag.CanonicalUrl = GetCanonicalUrl();

            ViewBag.SiteTitle = texts.Where(z => z.Position == $"SiteTitleProfile-{escort.EscortId}").Select(z => z.Description)
                .FirstOrDefault();
            ViewBag.SiteDescription = texts.Where(z => z.Position == $"SiteDescriptionProfile-{escort.EscortId}").Select(z => z.Description)
                .FirstOrDefault();

            ViewBag.MenuEscorts = await GetAllMenu();
            ViewBag.GoogleAnalyticsObject = texts.Where(z => z.Position == "GoogleAnalyticsObject").Select(z => z.Description)
                .FirstOrDefault();

            return View(m);
        }

        [Route("escorts-profile.php")]
        //[Route("profile.php")]
        public async Task<IActionResult> Redirect301(int escort_id)
        {
            var escorts = await GetAllEscorts();

            string name;
            var q =escorts.Where(z => z.SiteName == Constants.SiteName);
            var f = escorts.Any(z => z.EscortId == escort_id);
            if(f) name = escorts.Where(z => z.EscortId == escort_id).Select(z=>z.EscortName).Single();
            else
            {
                var r = new Random();
                name = escorts.Skip(r.Next(0, q.Count())).Take(1).Select(z=>z.EscortName).Single();
            }

            //https://metanit.com/sharp/aspnet5/5.5.php
            return RedirectPermanent($"escorts-profile/{name.ToLower()}.php");
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
    }

    public class ProfileViewModel : Escort
    {
        public List<Escort> List { get; set; } = new List<Escort>();
    }
}
