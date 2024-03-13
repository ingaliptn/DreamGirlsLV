namespace Domain.Models
{
    public static class Constants
    {
#if DEBUG
        //public const string DefaultConnection = "Server=DESKTOP-7NM2Q2N;Database=DreamGirlsBd;Trusted_Connection=True;MultipleActiveResultSets=true";
        public const string DefaultConnection = "Server=12.2.222.14;Database=DreamGirlsBd;Persist Security Info=True;User ID=eau199;Password=SafeBroker%100";
#else
        //.102
        //public const string DefaultConnection =
        //    "Server=SP-VL;Database=DreamGirlsBd;Persist Security Info=True;User ID=eau199;Password=SafeBroker%100";
        //185.165.168.2
        public const string DefaultConnection =
            "Server=SP-DB;Database=DreamGirlsBd;Persist Security Info=True;User ID=eau199;Password=SafeBroker%100";
#endif
        //public const string SiteName = "www.dreamgirlsphoenix.com";
        //public const string SitePhone = "(480) 565-8850";
        //public const string SitePhoneRef = "+14805658850";
        //public const string SiteEmail = "info@dreamgirlsphoenix.com";
        //public const string SiteContentLocation = "Phoenix";

        //public const string SiteName = "www.dreamgirlshawaii.com";
        //public const string SitePhone = "(880) 427-9980";
        //public const string SitePhoneRef = "+18084279980";
        //public const string SiteEmail = "info@.dreamgirlshawaii.com";
        //public const string SiteContentLocation = "Hawaii";

        //public const string SiteName = "www.dreamgirlsseattle.com";
        //public const string SitePhone = "(206) 486-1971";
        //public const string SitePhoneRef = "+12064861971";
        //public const string SiteEmail = "info@dreamgirlsseattle.com";
        //public const string SiteContentLocation = "Seattle";
        //public const string SiteDreamGirlsLocation = "DreamGirlsSeattle";

        //https://www.dreamgirlsdetroit.com
        //public const string SiteName = "www.dreamgirlsdetroit.com";
        //public const string SitePhone = "(313) 424-4461";
        //public const string SitePhoneRef = "+13134244461";
        //public const string SiteEmail = "info@dreamgirlsdetroit.com";
        //public const string SiteContentLocation = "Detroit";
        //public const string SiteDreamGirlsLocation = "DreamGirlsDetroit";

        //https://www.dreamgirlschicago.com
        //public const string SiteName = "www.dreamgirlschicago.com";
        //public const string SitePhone = "(312) 667-9107";
        //public const string SitePhoneRef = "+13126679107";
        //public const string SiteEmail = "info@dreamgirlschicago.com";
        //public const string SiteContentLocation = "Chicago";
        //public const string SiteDreamGirlsLocation = "DreamGirlsChicago";

        //https://www.dreamgirlslosangeles.com
        //public const string SiteName = "www.dreamgirlslosangeles.com";
        //public const string SitePhone = "(213) 985-2226";
        //public const string SitePhoneRef = "+12139852226";
        //public const string SiteEmail = "info@dreamgirlslosangeles.com";
        //public const string SiteContentLocation = "LosAngeles";
        //public const string SiteDreamGirlsLocation = "DreamGirlsLosAngeles";

        //https://www.dreamgirlsneworleans.com
        //public const string SiteName = "www.dreamgirlsneworleans.com";
        //public const string SitePhone = "(504) 681-9810";
        //public const string SitePhoneRef = "+1504681-9810";
        //public const string SiteEmail = "info@dreamgirlsneworleans.com";
        //public const string SiteContentLocation = "NewOrleans";
        //public const string SiteDreamGirlsLocation = "DreamGirlsNewOrleans";

        //https://www.dreamgirlspalmsprings.com
        //public const string SiteName = "www.dreamgirlspalmsprings.com";
        //public const string SitePhone = "(760) 607-3490";
        //public const string SitePhoneRef = "+17606073490";
        //public const string SiteEmail = "info@dreamgirlspalmsprings.com";
        //public const string SiteContentLocation = "Palmsprings";
        //public const string SiteDreamGirlsLocation = "DreamGirlsPalmsprings";

        //https://www.dreamgirlsreno.com
        //public const string SiteName = "www.dreamgirlsreno.com";
        //public const string SitePhone = "(775) 372-9570";
        //public const string SitePhoneRef = "+17753729570";
        //public const string SiteEmail = "info@dreamgirlsreno.com";
        //public const string SiteContentLocation = "Reno";
        //public const string SiteDreamGirlsLocation = "DreamGirlsReno";

        //https://www.dreamgirlsorlando.com
        //public const string SiteName = "www.dreamgirlsorlando.com";
        //public const string SitePhone = "(407) 845-9430";
        //public const string SitePhoneRef = "+14078459430";
        //public const string SiteEmail = "info@dreamgirlsorlando.com";
        //public const string SiteContentLocation = "Orlando";
        //public const string SiteDreamGirlsLocation = "DreamGirlsOrlando";

        //https://www.dreamgirlsatlanta.com/
        //public const string SiteName = "www.dreamgirlsatlanta.com";
        //public const string SitePhone = "(404) 891-1380";
        //public const string SitePhoneRef = "+14048911380";
        //public const string SiteEmail = "info@dreamgirlsatlanta.com";
        //public const string SiteContentLocation = "Atlanta";
        //public const string SiteDreamGirlsLocation = "DreamGirlsAtlanta";

        //https://www.dreamgirlssandiego.com
        //public const string SiteName = "www.dreamgirlssandiego.com";
        //public const string SitePhone = "(619) 916-3125";
        //public const string SitePhoneRef = "+16199163125";
        //public const string SiteEmail = "info@dreamgirlssandiego.com";
        //public const string SiteContentLocation = "SanDiego";
        //public const string SiteDreamGirlsLocation = "DreamGirlsSanDiego";

        //https://www.dreamgirlsboston.com
        //public const string SiteName = "www.dreamgirlsboston.com";
        //public const string SitePhone = "(617) 996-8472";
        //public const string SitePhoneRef = "+16179968472";
        //public const string SiteEmail = "info@dreamgirlsboston.com";
        //public const string SiteContentLocation = "Boston";
        //public const string SiteDreamGirlsLocation = "DreamGirlsBoston";

        //https://www.dreamgirlshouston.com
        //public const string SiteName = "www.dreamgirlshouston.com";
        //public const string SitePhone = "(713) 955-2588";
        //public const string SitePhoneRef = "+17139552588";
        //public const string SiteEmail = "info@dreamgirlshouston.com";
        //public const string SiteContentLocation = "Houston";
        //public const string SiteDreamGirlsLocation = "DreamGirlsHouston";

        //https://www.atlantamassagegirls.com
        //public const string SiteName = "www.atlantamassagegirls.com";
        //public const string SitePhone = "(404) 891-1382";
        //public const string SitePhoneRef = "+14048911382";
        //public const string SiteEmail = "ams@atlantamassagegirls.com";
        //public const string SiteContentLocation = "Atlanta";
        //public const string SiteDreamGirlsLocation = "DreamGirlsAtlanta";


        //https://www.bostonmassagegirls.com
        //public const string SiteName = "www.bostonmassagegirls.com";
        //public const string SitePhone = "(617) 996-8471";
        //public const string SitePhoneRef = "+16179968471";
        //public const string SiteEmail = "bmg@bostonmassagegirls.com";
        //public const string SiteContentLocation = "Boston";
        //public const string SiteDreamGirlsLocation = "Boston Massage Girls";

        //https://www.chicagomassagegirls.com
        //public const string SiteName = "www.chicagomassagegirls.com";
        //public const string SitePhone = "(312) 392-0720";
        //public const string SitePhoneRef = "+13123920720";
        //public const string SiteEmail = "info@chicagomassagegirls.com";
        //public const string SiteContentLocation = "Chicago";
        //public const string SiteDreamGirlsLocation = "Chicago Massage Girls";

        //https://www.houstonmassagegirls.com
        //public const string SiteName = "www.houstonmassagegirls.com";
        //public const string SitePhone = "(713) 357-6886";
        //public const string SitePhoneRef = "+17133576880";
        //public const string SiteEmail = "info@houstonmassagegirls.com";
        //public const string SiteContentLocation = "Houston";
        //public const string SiteDreamGirlsLocation = "Houston Massage Girls";

        //https://www.denvermassagegirls.com
        //public const string SiteName = "www.denvermassagegirls.com";
        //public const string SitePhone = "(720) 679-8808";
        //public const string SitePhoneRef = "+17206798808";
        //public const string SiteEmail = "info@denvermassagegirls.com";
        //public const string SiteContentLocation = "Denver";
        //public const string SiteDreamGirlsLocation = "Denver Massage Girls";

        //https://www.losangelesmassagegirls.com
        //public const string SiteName = "www.losangelesmassagegirls.com";
        //public const string SitePhone = "(323) 417-5255";
        //public const string SitePhoneRef = "+13234175255";
        //public const string SiteEmail = "info@losangelesmassagegirls.com";
        //public const string SiteContentLocation = "Los Angeles";
        //public const string SiteDreamGirlsLocation = "Los Angeles Massage Girls";

        //https://www.newyorkmassagegirls.com
        //public const string SiteName = "www.newyorkmassagegirls.com";
        //public const string SitePhone = "(929) 399-2740";
        //public const string SitePhoneRef = "+19293992740";
        //public const string SiteEmail = "info@newyorkmassagegirls.com";
        //public const string SiteContentLocation = "New York";
        //public const string SiteDreamGirlsLocation = "New York Massage Girls";

        //https://www.sandiegomassagegirls.com
        //public const string SiteName = "www.sandiegomassagegirls.com";
        //public const string SitePhone = "(619) 720-6330";
        //public const string SitePhoneRef = "+16197206330";
        //public const string SiteEmail = "info@sandiegomassagegirls.com";
        //public const string SiteContentLocation = "San Diego";
        //public const string SiteDreamGirlsLocation = "San Diego Massage Girls";

        //https://www.dreamgirlsriga.com
        //public const string SiteName = "www.dreamgirlsriga.com";
        //public const string SitePhone = "+3(725) 321-3558";
        //public const string SitePhoneRef = "+37253213558";
        //public const string SiteEmail = "info@dreamgirlsriga.com";
        //public const string SiteContentLocation = "Riga";
        //public const string SiteDreamGirlsLocation = "DreamGirlsRiga";

        //https://www.dreamgirlsistanbul.com
        //public const string SiteName = "www.dreamgirlsistanbul.com";
        //public const string SitePhone = "+90(546) 638-8900";
        //public const string SitePhoneRef = "+905466388900";
        //public const string SiteEmail = "info@dreamgirlsistanbul.com";
        //public const string SiteContentLocation = "Istanbul";
        //public const string SiteDreamGirlsLocation = "DreamGirlsIstanbul";

        //https://www.dreamgirlslasvegas.com
        public const string SiteName = "www.dreamgirlslasvegas.com";
        public const string SitePhone = "(702) 718-2634";
        public const string SitePhoneRef = "+17027182634";
        public const string SiteEmail = "info@dreamgirlslasvegas.com";
        public const string SiteContentLocation = "Las Vegas";
        public const string SiteDreamGirlsLocation = "DreamGirlsLasVegas";
    }
}
