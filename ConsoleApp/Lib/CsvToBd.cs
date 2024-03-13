using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp.Lib
{
    public class CsvToBd
    {
        private readonly IEscortRepository _escortRepository;
        private readonly ITextRepository _textRepository;
        private readonly IFileImageRepository _fileImage;
        private readonly IMenuRepository _menuRepository;
        public CsvToBd(IServiceProvider serviceProvider)
        {
            var serviceProvider1 = serviceProvider;
            _escortRepository = serviceProvider1.GetService<IEscortRepository>();
            _fileImage = serviceProvider1.GetService<IFileImageRepository>();
            _textRepository = serviceProvider1.GetService<ITextRepository>();
            _menuRepository = serviceProvider1.GetService<IMenuRepository>();
        }

        public async Task RemoveBad(string siteName)
        {
            var list = await _escortRepository.Escorts
                .Where(z=>z.SiteName == siteName).Include(z=>z.FileImages).ToListAsync();
            foreach (var p in list)
            {
                foreach (var o in p.FileImages)
                {
                    _fileImage.Remove(o);
                }
                _escortRepository.Remove(p);
            }

            await _escortRepository.SaveChangesAsync();

            var list1 = await _textRepository.Texts.Where(z => z.SiteName == siteName).ToListAsync();
            foreach (var p in list1)
            {
                _textRepository.Remove(p);
            }

            await _textRepository.SaveChangesAsync();

            var list2 = await _menuRepository.Menus.Where(z => z.SiteName == siteName).ToListAsync();
            foreach (var p in list2)
            {
                _menuRepository.Remove(p);
            }

            await _menuRepository.SaveChangesAsync();
        }

        public async Task BuildSection(string siteName)
        {
            var list = await _escortRepository.Escorts.Where(z => z.SiteName == siteName).ToListAsync();
            foreach (var p in list)
            {
                if(!string.IsNullOrEmpty(p.Section)) continue;
                var age = "28-30";
                switch (p.Age)
                {
                    case { } n when (n <= 23):
                        age = "20-23";
                        break;
                    case { } n when (n >= 24 && n <= 27):
                        age = "24-27";
                        break;
                }

                var height = "59-6";
                var t = p.Height.Replace("ft ", string.Empty).Replace("inch", string.Empty);
                var h = Convert.ToInt32(t);
                switch (h)
                {
                    case { } n when (n <= 55):
                        height = "50-55";
                        break;
                    case { } n when (n >= 56 && n <= 58):
                        height = "56-58";
                        break;
                }

                var weight = "141-160";
                switch (p.Weight)
                {
                    case { } n when (n <= 100):
                        weight = "80-100";
                        break;
                    case { } n when (n >= 101 && n <= 120):
                        weight = "101-120";
                        break;
                    case { } n when (n >= 121 && n <= 140):
                        weight = "121-140";
                        break;
                }
                p.Section = $"{age} {p.Bust} {height} {weight} {p.Hair}";
            }

            await _escortRepository.SaveChangesAsync();
        }

        public async Task AddEscort(string siteName, string fileName)
        {
            var path = $@"D:\!Download\{fileName}";
            using var reader = File.OpenText(path);
            var fileText = await reader.ReadToEndAsync();
            var list = fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var p in list)
            {
                if(string.IsNullOrEmpty(p)) continue;
                var line = p.Split("~");
               
                if(line[0].Contains("escort_id")) continue;
                for (var i = 0; i < line.Length; i++)
                {
                   if(string.IsNullOrEmpty(line[i])) continue;
                   line[i] = line[i].Replace("\"", string.Empty);
                }

                var es = new Escort {SiteName = siteName};

                var fParse = int.TryParse(line[0], out var value);
                if(!fParse) continue;
                es.EscortId = value;

                if (!string.IsNullOrEmpty(line[1])) es.Section = line[1];
                if (!string.IsNullOrEmpty(line[2])) es.EscortName = line[2];
                if (!string.IsNullOrEmpty(line[3]))
                {
                    var f = int.TryParse(line[3], out var age);
                    if(f) es.Age = age;
                }
                if (!string.IsNullOrEmpty(line[4]))
                {
                    var f = int.TryParse(line[4], out var bust);
                    es.Bust = f ? bust : GetRandom(1, 4);
                }
                else
                {
                    es.Bust = GetRandom(1, 4);
                }
                if (!string.IsNullOrEmpty(line[5])) es.Nationality = line[5];
                if (!string.IsNullOrEmpty(line[6]))
                {
                    var f = int.TryParse(line[6], out var weight);
                    if (f) es.Weight = weight;
                }
                if (!string.IsNullOrEmpty(line[7])) es.Height = line[7];
                if (!string.IsNullOrEmpty(line[8])) es.Hair = line[8];
                if (!string.IsNullOrEmpty(line[9])) es.Languages = line[9];
                if (!string.IsNullOrEmpty(line[10])) es.Statistics = line[10];

                var descriptionPoint = line.Length - 1;
                if (!string.IsNullOrEmpty(line[descriptionPoint])) es.Description = line[descriptionPoint]; //todo line[descriptionPoint];

                for (var i = 11; i < line.Length - 2; i++) //todo line.Length - 2
                {
                    if (string.IsNullOrEmpty(line[i])) continue;
                    var fi = new FileImage {FileName = line[i], Escort = es};
                    _fileImage.Create(fi);
                }

                var f1 = await _escortRepository.Escorts
                    .Where(z => z.SiteName == es.SiteName && z.EscortName == es.EscortName).AnyAsync();
                if(f1) continue;
              
                _escortRepository.Create(es);
                await _escortRepository.SaveChangesAsync();
            }
            await _escortRepository.SaveChangesAsync();
        }

        public async Task AddEscort1(string siteName, string fileName)
        {
            var path = $@"D:\!Download\{fileName}";
            using var reader = File.OpenText(path);
            var fileText = await reader.ReadToEndAsync();
            //var list = fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[] stringSeparators = new string[] { "\n\r" };
            string[] list = fileText.Split(stringSeparators, StringSplitOptions.None);
            foreach (var p in list)
            {
                if (string.IsNullOrEmpty(p)) continue;
                var line = p.Split("~");

                if (line[0].Contains("escort_id")) continue;
                for (var i = 0; i < line.Length; i++)
                {
                    if (string.IsNullOrEmpty(line[i])) continue;
                    line[i] = line[i].Replace("\"", string.Empty);
                }

                var es = new Escort { SiteName = siteName };

                var fParse = int.TryParse(line[0], out var value);
                if (!fParse) continue;
                es.EscortId = value;

                if (!string.IsNullOrEmpty(line[1])) es.Section = line[1];
                if (!string.IsNullOrEmpty(line[2])) es.EscortName = line[2];
                if (!string.IsNullOrEmpty(line[3]))
                {
                    var f = int.TryParse(line[3], out var age);
                    if (f) es.Age = age;
                }
                if (!string.IsNullOrEmpty(line[4]))
                {
                    var f = int.TryParse(line[4], out var bust);
                    es.Bust = f ? bust : GetRandom(1, 4);
                }
                else
                {
                    es.Bust = GetRandom(1, 4);
                }
                if (!string.IsNullOrEmpty(line[4])) es.Nationality = line[4];
                if (!string.IsNullOrEmpty(line[5]))
                {
                    var f = int.TryParse(line[5], out var weight);
                    if (f) es.Weight = weight;
                    if (es.Weight == 0) weight = GetRandom(115, 130);
                }
                if (!string.IsNullOrEmpty(line[6])) es.Height = line[6];
                if (!string.IsNullOrEmpty(line[7])) es.Hair = line[7];
                if (!string.IsNullOrEmpty(line[8])) es.Languages = line[8];
                if (!string.IsNullOrEmpty(line[9])) es.Statistics = line[9];

                
                if (!string.IsNullOrEmpty(line[21])) es.Description = line[21]; 

                for (var i = 11; i < 20; i++) //todo line.Length - 2
                {
                    if (string.IsNullOrEmpty(line[i])) continue;
                    var fi = new FileImage { FileName = line[i], Escort = es };
                    _fileImage.Create(fi);
                }

                var f1 = await _escortRepository.Escorts
                    .Where(z => z.SiteName == es.SiteName && z.EscortName == es.EscortName).AnyAsync();
                if (f1) continue;

                _escortRepository.Create(es);
                await _escortRepository.SaveChangesAsync();
            }
            await _escortRepository.SaveChangesAsync();
        }

        public async Task AddEscort2(string siteName, string fileName)
        {
            var path = $@"D:\!Download\{fileName}";
            using var reader = File.OpenText(path);
            var fileText = await reader.ReadToEndAsync();
            //var list = fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[] stringSeparators = new string[] { "\n\r" };
            string[] list = fileText.Split(stringSeparators, StringSplitOptions.None);
            foreach (var p in list)
            {
                if (string.IsNullOrEmpty(p)) continue;
                var line = p.Split("~");

                if (line[0].Contains("escort_id")) continue;
                for (var i = 0; i < line.Length; i++)
                {
                    if (string.IsNullOrEmpty(line[i])) continue;
                    line[i] = line[i].Replace("\"", string.Empty);
                }

                var es = new Escort { SiteName = siteName };

                var fParse = int.TryParse(line[0], out var value);
                if (!fParse) continue;
                es.EscortId = value;

                if (!string.IsNullOrEmpty(line[1])) es.Section = line[1];
                if (!string.IsNullOrEmpty(line[2])) es.EscortName = line[2];
                if (!string.IsNullOrEmpty(line[3]))
                {
                    var f = int.TryParse(line[3], out var age);
                    if (f) es.Age = age;
                }
                if (!string.IsNullOrEmpty(line[4]))
                {
                    var f = int.TryParse(line[4], out var bust);
                    es.Bust = f ? bust : GetRandom(1, 4);
                }
                else
                {
                    es.Bust = GetRandom(1, 4);
                }
                if (!string.IsNullOrEmpty(line[5])) es.Nationality = line[5];
                if (!string.IsNullOrEmpty(line[6]))
                {
                    var f = int.TryParse(line[6], out var weight);
                    if (f) es.Weight = weight;
                    if (es.Weight == 0) weight = GetRandom(115, 130);
                }
                if (!string.IsNullOrEmpty(line[7])) es.Height = line[7];
                if (!string.IsNullOrEmpty(line[8])) es.Hair = line[8];
                if (!string.IsNullOrEmpty(line[9])) es.Languages = line[9];
                if (!string.IsNullOrEmpty(line[10])) es.Statistics = line[10];

                var descriptionPoint = line.Length - 2;
                if (!string.IsNullOrEmpty(line[22])) es.Description = line[22]; 

                for (var i = 11; i < 21; i++) //todo line.Length - 2
                {
                    if (string.IsNullOrEmpty(line[i])) continue;
                    var fi = new FileImage { FileName = line[i], Escort = es };
                    _fileImage.Create(fi);
                }

                var f1 = await _escortRepository.Escorts
                    .Where(z => z.SiteName == es.SiteName && z.EscortName == es.EscortName).AnyAsync();
                if (f1) continue;

                _escortRepository.Create(es);
                await _escortRepository.SaveChangesAsync();
            }
            await _escortRepository.SaveChangesAsync();
        }

        private int GetRandom(int min, int max)
        {
            var r = new Random();
            return r.Next(min, max);
        }

        public async Task AddSiteTitleSiteDescription(string siteName, string fileName)
        {
            var path = $@"D:\!Download\{fileName}";
            using var reader = File.OpenText(path);
            var fileText = await reader.ReadToEndAsync();
            var list = fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var escorts = await _escortRepository.Escorts.Where(z => z.SiteName == siteName).ToListAsync();
            
            foreach (var p in list)
            {
                if (string.IsNullOrEmpty(p)) continue;
                var line = p.Split("~");
                var txtTitle = new Text{SiteName = siteName};
                var txtDescription = new Text { SiteName = siteName };

                if(!line[0].Contains(siteName)) return;

                if (line[0] == $"https://{siteName}/")
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = "SiteTitleHome";
                    txtDescription.Position = "SiteDescriptionHome";
                }
                else
                if (line[0].Contains("?escort_id"))
                {
                    var index = line[0].IndexOf("?escort_id=", StringComparison.Ordinal);
                    var id = line[0].Substring(index + 11);
                    var fParse = int.TryParse(id, out var value);
                    if (!fParse)
                    {
                        return;
                    };
                    fParse = escorts.Any(z => z.EscortId == value);
                    if (!fParse)
                    {
                        return;
                    }
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleProfile-{id}";
                    txtDescription.Position = $"SiteDescriptionProfile-{id}";
                }
                else
                if (line[0].Contains("profile.php?id="))
                {
                    var index = line[0].IndexOf("?id=", StringComparison.Ordinal);
                    var id = line[0].Substring(index + 4);
                    var fParse = int.TryParse(id, out var value);
                    if (!fParse)
                    {
                        return;
                    };
                    fParse = escorts.Any(z => z.EscortId == value);
                    if (!fParse)
                    {
                        return;
                    }
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleProfile-{id}";
                    txtDescription.Position = $"SiteDescriptionProfile-{id}";
                }
                else
                if (line[0].Contains("employment.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleEmployment";
                    txtDescription.Position = $"SiteDescriptionEmployment";
                }
                else
                if (line[0].Contains("cheap-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleCheap";
                    txtDescription.Position = $"SiteDescriptionCheap";
                }
                else
                if (line[0].Contains("bbw-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBBW";
                    txtDescription.Position = $"SiteDescriptionBbw";
                }
                else
                if (line[0].Contains("ebony-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleEbony";
                    txtDescription.Position = $"SiteDescriptionEbony";
                }
                else
                if (line[0].Contains("about.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleAbout";
                    txtDescription.Position = $"SiteDescriptionAbout";
                }
                else
                if (line[0].Contains("contact.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleContact";
                    txtDescription.Position = $"SiteDescriptionContact";
                }
                else
                if (line[0].Contains("gfe-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleGfeEscorts";
                    txtDescription.Position = $"SiteDescriptionGfeEscorts";
                }
                else
                if (line[0].Contains("asian-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleAsian";
                    txtDescription.Position = $"SiteDescriptionAsian";
                }
                else
                if (line[0].Contains("booking.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBooking";
                    txtDescription.Position = $"SiteDescriptionBooking";
                }
                else
                if (line[0].Contains("mature-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleMature";
                    txtDescription.Position = $"SiteDescriptionMature";
                }
                else
                if (line[0].Contains("latina-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleLatina";
                    txtDescription.Position = $"SiteDescriptionLatina";
                }
                else
                if (line[0].Contains("black-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBlack";
                    txtDescription.Position = $"SiteDescriptionBlack";
                }
                else
                if (line[0].Contains("busty-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBusty";
                    txtDescription.Position = $"SiteDescriptionBusty";
                }
                else
                if (line[0].Contains("girlfriend-experience.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleGirlfriend";
                    txtDescription.Position = $"SiteDescriptionGirlfriend";
                }
                else
                if (line[0].Contains("big-booty-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBigBooty";
                    txtDescription.Position = $"SiteDescriptionBigBooty";
                }
                else
                if (line[0].Contains("checap-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleChecapEscorts";
                    txtDescription.Position = $"SiteDescriptionChecapEscorts";
                }
                else
                if (line[0].Contains("gfe.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleGfe";
                    txtDescription.Position = $"SiteDescriptionGfe";
                }
                else
                if (line[0].Contains("blonde-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBlondeEscorts";
                    txtDescription.Position = $"SiteDescriptionBlondeEscorts";
                }
                else
                if (line[0].Contains("russian-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleRussianEscorts";
                    txtDescription.Position = $"SiteDescriptionRussianEscorts";
                }
                else
                if (line[0].Contains("vip-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleVipEscorts";
                    txtDescription.Position = $"SiteDescriptionVipEscorts";
                }
                else
                if (line[0].Contains("lombard-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleLombardEscorts";
                    txtDescription.Position = $"SiteDescriptionLombardEscorts";
                }
                else
                if (line[0].Contains("independent-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleIndependentEscorts";
                    txtDescription.Position = $"SiteIndependentLombardEscorts";
                }
                else
                if (line[0].Contains("naperville-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleNapervilleEscorts";
                    txtDescription.Position = $"SiteDescriptionNapervilleEscorts";
                }
                else
                if (line[0].Contains("ohare-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleOhareEscorts";
                    txtDescription.Position = $"SiteDescriptionOhareEscorts";
                }
                else
                if (line[0].Contains("schaumburg-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleSchaumburgEscorts";
                    txtDescription.Position = $"SiteDescriptionSchaumburgEscorts";
                }
                else
                if (line[0].Contains("quincy-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleQuincyEscorts";
                    txtDescription.Position = $"SiteDescriptionQuincyEscorts";
                }
                else
                if (line[0].Contains("braintree-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBraintreeEscorts";
                    txtDescription.Position = $"SiteDescriptionBraintreeEscorts";
                }
                else
                if (line[0].Contains("skokie-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleSkokieEscorts";
                    txtDescription.Position = $"SiteDescriptionSkokieEscorts";
                }
                else
                if (line[0].Contains("brunette-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBrunetteEscorts";
                    txtDescription.Position = $"SiteDescriptionBrunetteEscorts";
                }
                else
                if (line[0].Contains("outcall-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleOutcallEscorts";
                    txtDescription.Position = $"SiteDescriptionOutcallEscorts";
                }
                else
                if (line[0].Contains("erotic-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleEroticMassage";
                    txtDescription.Position = $"SiteDescriptionEroticMassage";
                }
                else
                if (line[0].Contains("body-rubs.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBodyRubs";
                    txtDescription.Position = $"SiteDescriptionBodyRubs";
                }
                else
                if (line[0].Contains("asian.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleAsianMassage";
                    txtDescription.Position = $"SiteDescriptionAsianMassage";
                }
                else
                if (line[0].Contains("in-room.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleInRoomMassage";
                    txtDescription.Position = $"SiteDescriptionInRoomMassage";
                }
                else
                if (line[0].Contains("couples-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleCouplesEscorts";
                    txtDescription.Position = $"SiteDescriptionCouplesEscorts";
                }
                else
                if (line[0].Contains("full-body-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleFullBody";
                    txtDescription.Position = $"SiteDescriptionFullBody";
                }
                else
                if (line[0].Contains("full-body.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleFullBody";
                    txtDescription.Position = $"SiteDescriptionFullBody";
                }
                else
                if (line[0].Contains("happy-ending-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleHappyEnding";
                    txtDescription.Position = $"SiteDescriptionHappyEnding";
                }
                else
                if (line[0].Contains("happy-ending.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleHappyEnding";
                    txtDescription.Position = $"SiteDescriptionHappyEnding";
                }
                else
                if (line[0].Contains("nuru-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleNuru";
                    txtDescription.Position = $"SiteDescriptionNuru";
                }
                else
                if (line[0].Contains("nuru.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleNuru";
                    txtDescription.Position = $"SiteDescriptionNuru";
                }
                 else
                if (line[0].Contains("escort-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleEscortMassage";
                    txtDescription.Position = $"SiteDescriptionEscortMassage";
                }
                else
                if (line[0].Contains("tantra-massage.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleTantra";
                    txtDescription.Position = $"SiteDescriptionTantra";
                }
                else
                if (line[0].Contains("tantra.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleTantra";
                    txtDescription.Position = $"SiteDescriptionTantra";
                }
                else
                if (line[0].Contains("outcall.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleOutcall";
                    txtDescription.Position = $"SiteDescriptionOutcall";
                }
                else
                if (line[0].Contains("sex.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleSex";
                    txtDescription.Position = $"SiteDescriptionSex";
                }
                else
                if (line[0].Contains("arab-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleArabEscorts";
                    txtDescription.Position = $"SiteDescriptionArabEscorts";
                }
                else
                if (line[0].Contains("iranian-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleIranianEscorts";
                    txtDescription.Position = $"SiteDescriptionIranianEscorts";
                }
                else
                if (line[0].Contains("strapon-escort.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleStraponEscorts";
                    txtDescription.Position = $"SiteDescriptionStraponEscorts";
                }
                else
                if (line[0].Contains("bisexual-escorts.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleBisexualEscorts";
                    txtDescription.Position = $"SiteDescriptionBisexualEscorts";
                }
                else
                if (line[0].Contains("escorts-for-couples.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleForCouplesEscorts";
                    txtDescription.Position = $"SiteDescriptionForCouplesEscorts";
                }
                else
                if (line[0].Contains("escort.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleEscortMassage";
                    txtDescription.Position = $"SiteDescriptionEscortMassage";
                }
                else
                if (line[0].Contains("couples.php"))
                {
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleCouplesEscorts";
                    txtDescription.Position = $"SiteDescriptionCouplesEscorts";
                }
                else
                if(!string.IsNullOrEmpty(line[0]))
                {
                    var index = line[0].LastIndexOf("/", StringComparison.Ordinal);
                    if(index < 3) continue;
                    var t = line[0].Substring(index + 1);
                    if(string.IsNullOrEmpty(t)) continue;
                    var name = t.Replace(".php", "");
                    if(string.IsNullOrEmpty(name)) continue;
                    var id = escorts.Where(z => z.EscortName.ToLower() == name).Select(z=>z.EscortId).FirstOrDefault();
                    if(id == 0) continue;
                    txtTitle.Description = line[1];
                    txtDescription.Description = line[2];
                    txtTitle.Position = $"SiteTitleProfile-{id}";
                    txtDescription.Position = $"SiteDescriptionProfile-{id}";
                }
                else
                {
                    continue;
                }

                var f = _textRepository.Texts.Any(z => z.SiteName == siteName && (z.Position == txtTitle.Position || z.Position == txtDescription.Position));
                if (f)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(txtTitle.Position) && !string.IsNullOrEmpty(txtTitle.Description))
                {
                    f = _textRepository.Texts.Any(z => z.SiteName == Constants.SiteName &&
                                                       z.Position == txtTitle.Position);
                    if (f) _textRepository.Update(txtTitle);
                    else  _textRepository.Create(txtTitle);
                }

                if (!string.IsNullOrEmpty(txtDescription.Position) && !string.IsNullOrEmpty(txtDescription.Description))
                {
                    f = _textRepository.Texts.Any(z => z.SiteName == Constants.SiteName &&
                                                       z.Position == txtDescription.Position);
                    if (f) _textRepository.Update(txtDescription);
                    else _textRepository.Create(txtDescription);
                }
            }
            await _textRepository.SaveChangesAsync();
        }

        public async Task AddParserTexts(string site)
        {

            //await AddMassagePage($"https://{Constants.SiteName}/body-rubs.php", "PositionBodyRubs");
            //await AddMassagePage($"https://{Constants.SiteName}/erotic-massage.php", "PositionMassage");
            //await AddMassagePage($"https://{Constants.SiteName}/couples-massage.php", "PositionCouples");
            //await AddMassagePage($"https://{Constants.SiteName}/full-body-massage.php", "PositionFullBody");
            //await AddMassagePage($"https://{Constants.SiteName}/happy-ending-massage.php", "PositionHappyEnding");
            //await AddMassagePage($"https://{Constants.SiteName}/nuru-massage.php", "PositionNuru");
            //await AddMassagePage($"https://{Constants.SiteName}/tantra-massage.php", "PositionTantra");

            //await AddMassagePage($"https://{Constants.SiteName}/asian.php", "PositionAsianMassage");
            //await AddMassagePage($"https://{Constants.SiteName}/body-rubs.php", "PositionBodyRubs");
            //await AddMassagePage($"https://{Constants.SiteName}/couples.php", "PositionCouples");
            //await AddMassagePage($"https://{Constants.SiteName}/escort.php", "PositionEscortMassage");
            //await AddMassagePage($"https://{Constants.SiteName}/full-body.php", "PositionFullBody");
            //await AddMassagePage($"https://{Constants.SiteName}/happy-ending.php", "PositionHappyEnding");
            //await AddMassagePage($"https://{Constants.SiteName}/in-room.php", "PositionInRoomMassage");
            //await AddMassagePage($"https://{Constants.SiteName}/nuru.php", "PositionNuru");
            //await AddMassagePage($"https://{Constants.SiteName}/outcall.php", "PositionOutcall");
            //await AddMassagePage($"https://{Constants.SiteName}/tantra.php", "PositionTantra");
            //await AddMassagePage($"https://{Constants.SiteName}/sex.php", "PositionSexMassage");

            //await AddMassagePage($"https://{Constants.SiteName}/escort-massage.php", "PositionEscortMassage");

            //https://www.dreamgirlshawaii.com/#all
            var url = $"https://{site}";
            var s = await ReadPage(url);
            if (!string.IsNullOrEmpty(s))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(s);

                await BuildMenuEscorts(doc);

                var node = doc.DocumentNode.SelectSingleNode(".//div[@class='static_banner_content']");
                var strString = node.SelectNodes(".//h1").First().InnerText; //todo
                //var strString = "Welcome";
                var strHtml = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].OuterHtml;
                //var strHtml = string.Empty;
                //var strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].InnerHtml; //todo 1

                //var node1 = doc.DocumentNode.SelectNodes(".//div[@class='row']")[2];
                //var strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[1].InnerHtml;
                //var strHtml1 = node1.OuterHtml;
                var strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[1].InnerHtml;

                var scriptsList = doc.DocumentNode.Descendants().Where(z => z.Name == "script").ToList();
                foreach (var p in scriptsList)
                {
                    if (!p.InnerText.Contains("GoogleAnalyticsObject")) continue;
                    _textRepository.Create(new Text
                    {
                        SiteName = site, Position = "GoogleAnalyticsObject", 
                        Description = $"<script>{p.InnerText}</script>"
                    });
                    break;
                }

                _textRepository.Create(new Text { SiteName = site, Position = "PositionHomeTopTitle", Description = strString });
                _textRepository.Create(new Text { SiteName = site, Position = "PositionHomeTop", Description = strHtml });
                _textRepository.Create(new Text { SiteName = site, Position = "PositionHomeBottom", Description = strHtml1 });

            }
            else
            {
                _textRepository.Create(new Text { SiteName = site, Position = "PositionHomeTopTitle" });
                _textRepository.Create(new Text {SiteName = site, Position = "PositionHomeTop" });
                _textRepository.Create(new Text { SiteName = site, Position = "PositionHomeBottom" });
            }

            await _textRepository.SaveChangesAsync();

            //Escorts 
            var escortsSub = await _menuRepository.Menus
                .Where(z => z.SiteName == Constants.SiteName && z.MenuName == "Escorts").ToListAsync();
            //WE COVER MENU
            //escortsSub.Add(new Menu { Name = "Asian Escorts", Href = "asian-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Black Escorts", Href = "black-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Russian Escorts", Href = "russian-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Busty Escorts", Href = "busty-escorts.php" });
            //escortsSub.Add(new Menu { Name = "VIP Escorts", Href = "vip-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Cheap Escorts", Href = "cheap-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Outcall Escorts", Href = "outcall-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Arab Escorts", Href = "arab-escorts.php" });
            //escortsSub.Add(new Menu { Name = "BBW Escorts", Href = "bbw-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Iranian Escorts", Href = "iranian-escorts.php" });
            //escortsSub.Add(new Menu { Name = "Mature Escorts", Href = "mature-escorts.php" });

            //escortsSub.Add(new Menu { Name = "Strapon Escorts", Href = "strapon-escort.php" });
            //escortsSub.Add(new Menu { Name = "Mistress Escorts", Href = "mistress-escorts.php" });


            //todo if menu Escorts
            foreach (var p in escortsSub)
            {
                var name = p.Name.Replace(" ", "-");
                await AddEscorts($"https://{site}/{p.Href}", $"EscortsHeading{name}", $"PositionEscorts{name}Top", $"PositionEscorts{name}Bottom");
            }

            await _textRepository.SaveChangesAsync();

            await AddAboutPage();
        }

        private async Task AddAboutPage()
        {
            var url = $"https://{Constants.SiteName}/about.php";
            var s = await ReadPage(url);
            if (!string.IsNullOrEmpty(s))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(s);
                var strString = doc.DocumentNode.SelectNodes(".//h1[@class='heading']").First().InnerText;
                //var strHtml = doc.DocumentNode.SelectNodes(".//div[@class='common_page_container clearfix']")[0].InnerHtml;
                //var strHtml = doc.DocumentNode.SelectNodes(".//div[@class='common_page_container clearfix']")[0].OuterHtml;

                var strHtml = doc.DocumentNode.SelectSingleNode(".//div[@class='container_area border_radius bg_new_york']").InnerHtml;

                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = "PositionAbout", Description = strString });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = "SiteDescriptionPageAbout", Description = strHtml });

            }
            else
            {
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = "PositionAbout" });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = "SiteDescriptionPageAbout" });
            }

            await _textRepository.SaveChangesAsync();
        }

        private async Task AddMassagePage(string url, string name)
        {
            
            var s = await ReadPage(url);
            if (!string.IsNullOrEmpty(s))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(s);
                var strString = doc.DocumentNode.SelectNodes(".//h1[@class='heading']").First().InnerText;
                //var strHtml = doc.DocumentNode.SelectNodes(".//div[@class='container_area border_radius bg_new_york']")[0].InnerHtml; //todo 1
                //var strHtml = doc.DocumentNode.SelectSingleNode(".//div[@class='common_page_container']").OuterHtml;
                var strHtml = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].InnerHtml;

                //var node = doc.DocumentNode.SelectNodes(".//div[@class='row']")[0];
                //var strHtml = $"{node.ChildNodes[1].OuterHtml} {node.ChildNodes[3].OuterHtml} {node.ChildNodes[6].OuterHtml}";

                //var node1 = doc.DocumentNode.SelectNodes(".//div[@class='row']")[2];
                var strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[1].InnerHtml;
                //var strHtml1 = node1.InnerHtml;
                //var strHtml1 = doc.DocumentNode.SelectSingleNode(".//div[@class='site_content_area']").OuterHtml;

                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = $"{name}Title", Description = strString });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = $"{name}Top", Description = strHtml });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = $"{name}Bottom", Description = strHtml1 });
            }
            else
            {
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = $"{name}Title" });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = $"{name}Top" });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = $"{name}Bottom" });
            }

            await _textRepository.SaveChangesAsync();
        }

        private async Task BuildMenuEscorts(HtmlDocument doc)
        {
            var menuEscortLi = doc.DocumentNode.SelectNodes(".//ul[@class='sub-menu']").First()
                .SelectNodes("./li");
            foreach (var p in menuEscortLi)
            {
                var name = p.InnerText;
                var href = p.SelectNodes("./a").First().GetAttributeValue("href", string.Empty);
                _menuRepository.Create(new Menu
                {
                    SiteName = Constants.SiteName, MenuName = "Escorts",
                    Name = name, Href = href.Replace("/", string.Empty)
                });;
            }

            await _menuRepository.SaveChangesAsync();
        }

        private async Task AddEscorts(string url, string heading, string top, string bottom)
        {
            var s = await ReadPage(url);
            if (!string.IsNullOrEmpty(s))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(s);
                var strString = doc.DocumentNode.SelectNodes(".//h1[@class='heading']").First().InnerText;
                //var strHtml = doc.DocumentNode.SelectSingleNode(".//div[@class='col-sm-12 col-md-12 col-lg-12']").OuterHtml;
                //var strHtml = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].InnerHtml;
                //var strString = string.Empty;
                var htmlNodes = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']");
                var strHtml = string.Empty;
                string strHtml1;
                if (htmlNodes.Count > 1)
                {
                    strHtml = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].InnerHtml;
                    strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[1].InnerHtml;
                }
                else
                {
                    strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].InnerHtml;
                }

                //var nodes = doc.DocumentNode.SelectNodes(".//div[@class='row']")[0];
                //var strHtml1 = string.Empty;
                //foreach (var p in nodes.ChildNodes)
                //{
                //    if (p.Name == "div") break;
                //    strHtml1 += $"{p.OuterHtml}";
                //}

                //var strHtml1 = doc.DocumentNode.SelectNodes(".//div[@class='custom_content_block clearfix']")[0].InnerHtml;

                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = heading, Description = strString });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = top, Description = strHtml });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = bottom, Description = strHtml1 });

            }
            else
            {
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = heading });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = top });
                _textRepository.Create(new Text { SiteName = Constants.SiteName, Position = bottom });
            }
        }

        private static async Task<string> ReadPage(string url)
        {
            try
            {
                using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
                var response = await client.GetAsync(url);
                var s = await response.Content.ReadAsStringAsync();
                return !response.IsSuccessStatusCode ? string.Empty : s;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
