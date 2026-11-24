using TRS_Lib.Models;

namespace TRS_Lib.Data.Seed;

// Reference data for Sweden, inserted by the AddGeography migration.
// Ids come from the position in the arrays: only ever append, never reorder or remove entries.
public static class SwedenSeed
{
    public const int SwedenId = 1;

    // The 25 landskap. Id = position + 1.
    private static readonly string[] Provinces =
    [
        // Götaland
        "Skåne", "Blekinge", "Halland", "Småland", "Öland", "Gotland", "Östergötland", "Västergötland", "Bohuslän", "Dalsland",
        // Svealand
        "Värmland", "Närke", "Västmanland", "Södermanland", "Uppland", "Dalarna",
        // Norrland
        "Gästrikland", "Hälsingland", "Härjedalen", "Jämtland", "Medelpad", "Ångermanland", "Västerbotten", "Norrbotten", "Lappland"
    ];

    // The 290 kommuner, grouped by län. Kommuner are organized by län, not landskap, and several lie in more
    // than one landskap; each is linked to the landskap that contains its seat (centralort). Id = position + 1.
    private static readonly (string Name, string Province)[] Municipalities =
    [
        // Stockholms län
        ("Upplands Väsby", "Uppland"), ("Vallentuna", "Uppland"), ("Österåker", "Uppland"), ("Värmdö", "Uppland"),
        ("Järfälla", "Uppland"), ("Ekerö", "Uppland"), ("Huddinge", "Södermanland"), ("Botkyrka", "Södermanland"),
        ("Salem", "Södermanland"), ("Haninge", "Södermanland"), ("Tyresö", "Södermanland"), ("Upplands-Bro", "Uppland"),
        ("Nykvarn", "Södermanland"), ("Täby", "Uppland"), ("Danderyd", "Uppland"), ("Sollentuna", "Uppland"),
        ("Stockholm", "Uppland"), ("Södertälje", "Södermanland"), ("Nacka", "Södermanland"), ("Sundbyberg", "Uppland"),
        ("Solna", "Uppland"), ("Lidingö", "Uppland"), ("Vaxholm", "Uppland"), ("Norrtälje", "Uppland"),
        ("Sigtuna", "Uppland"), ("Nynäshamn", "Södermanland"),

        // Uppsala län
        ("Håbo", "Uppland"), ("Älvkarleby", "Uppland"), ("Knivsta", "Uppland"), ("Heby", "Västmanland"),
        ("Tierp", "Uppland"), ("Uppsala", "Uppland"), ("Enköping", "Uppland"), ("Östhammar", "Uppland"),

        // Södermanlands län
        ("Vingåker", "Södermanland"), ("Gnesta", "Södermanland"), ("Nyköping", "Södermanland"), ("Oxelösund", "Södermanland"),
        ("Flen", "Södermanland"), ("Katrineholm", "Södermanland"), ("Eskilstuna", "Södermanland"), ("Strängnäs", "Södermanland"),
        ("Trosa", "Södermanland"),

        // Östergötlands län
        ("Ödeshög", "Östergötland"), ("Ydre", "Östergötland"), ("Kinda", "Östergötland"), ("Boxholm", "Östergötland"),
        ("Åtvidaberg", "Östergötland"), ("Finspång", "Östergötland"), ("Valdemarsvik", "Östergötland"), ("Linköping", "Östergötland"),
        ("Norrköping", "Östergötland"), ("Söderköping", "Östergötland"), ("Motala", "Östergötland"), ("Vadstena", "Östergötland"),
        ("Mjölby", "Östergötland"),

        // Jönköpings län
        ("Aneby", "Småland"), ("Gnosjö", "Småland"), ("Mullsjö", "Västergötland"), ("Habo", "Västergötland"),
        ("Gislaved", "Småland"), ("Vaggeryd", "Småland"), ("Jönköping", "Småland"), ("Nässjö", "Småland"),
        ("Värnamo", "Småland"), ("Sävsjö", "Småland"), ("Vetlanda", "Småland"), ("Eksjö", "Småland"),
        ("Tranås", "Småland"),

        // Kronobergs län
        ("Uppvidinge", "Småland"), ("Lessebo", "Småland"), ("Tingsryd", "Småland"), ("Alvesta", "Småland"),
        ("Älmhult", "Småland"), ("Markaryd", "Småland"), ("Växjö", "Småland"), ("Ljungby", "Småland"),

        // Kalmar län
        ("Högsby", "Småland"), ("Torsås", "Småland"), ("Mörbylånga", "Öland"), ("Hultsfred", "Småland"),
        ("Mönsterås", "Småland"), ("Emmaboda", "Småland"), ("Kalmar", "Småland"), ("Nybro", "Småland"),
        ("Oskarshamn", "Småland"), ("Västervik", "Småland"), ("Vimmerby", "Småland"), ("Borgholm", "Öland"),

        // Gotlands län
        ("Gotland", "Gotland"),

        // Blekinge län
        ("Olofström", "Blekinge"), ("Karlskrona", "Blekinge"), ("Ronneby", "Blekinge"), ("Karlshamn", "Blekinge"),
        ("Sölvesborg", "Blekinge"),

        // Skåne län
        ("Vellinge", "Skåne"), ("Östra Göinge", "Skåne"), ("Örkelljunga", "Skåne"), ("Bjuv", "Skåne"),
        ("Kävlinge", "Skåne"), ("Lomma", "Skåne"), ("Svedala", "Skåne"), ("Skurup", "Skåne"),
        ("Sjöbo", "Skåne"), ("Hörby", "Skåne"), ("Höör", "Skåne"), ("Tomelilla", "Skåne"),
        ("Bromölla", "Skåne"), ("Osby", "Skåne"), ("Perstorp", "Skåne"), ("Klippan", "Skåne"),
        ("Åstorp", "Skåne"), ("Båstad", "Skåne"), ("Malmö", "Skåne"), ("Lund", "Skåne"),
        ("Landskrona", "Skåne"), ("Helsingborg", "Skåne"), ("Höganäs", "Skåne"), ("Eslöv", "Skåne"),
        ("Ystad", "Skåne"), ("Trelleborg", "Skåne"), ("Kristianstad", "Skåne"), ("Simrishamn", "Skåne"),
        ("Ängelholm", "Skåne"), ("Hässleholm", "Skåne"), ("Staffanstorp", "Skåne"), ("Burlöv", "Skåne"),
        ("Svalöv", "Skåne"),

        // Hallands län
        ("Hylte", "Halland"), ("Halmstad", "Halland"), ("Laholm", "Halland"), ("Falkenberg", "Halland"),
        ("Varberg", "Halland"), ("Kungsbacka", "Halland"),

        // Västra Götalands län
        ("Härryda", "Västergötland"), ("Partille", "Västergötland"), ("Öckerö", "Bohuslän"), ("Stenungsund", "Bohuslän"),
        ("Tjörn", "Bohuslän"), ("Orust", "Bohuslän"), ("Sotenäs", "Bohuslän"), ("Munkedal", "Bohuslän"),
        ("Tanum", "Bohuslän"), ("Dals-Ed", "Dalsland"), ("Färgelanda", "Dalsland"), ("Ale", "Västergötland"),
        ("Lerum", "Västergötland"), ("Vårgårda", "Västergötland"), ("Bollebygd", "Västergötland"), ("Grästorp", "Västergötland"),
        ("Essunga", "Västergötland"), ("Karlsborg", "Västergötland"), ("Gullspång", "Västergötland"), ("Tranemo", "Västergötland"),
        ("Bengtsfors", "Dalsland"), ("Mellerud", "Dalsland"), ("Lilla Edet", "Västergötland"), ("Mark", "Västergötland"),
        ("Svenljunga", "Västergötland"), ("Herrljunga", "Västergötland"), ("Vara", "Västergötland"), ("Götene", "Västergötland"),
        ("Tibro", "Västergötland"), ("Töreboda", "Västergötland"), ("Göteborg", "Västergötland"), ("Mölndal", "Västergötland"),
        ("Kungälv", "Bohuslän"), ("Lysekil", "Bohuslän"), ("Uddevalla", "Bohuslän"), ("Strömstad", "Bohuslän"),
        ("Vänersborg", "Västergötland"), ("Trollhättan", "Västergötland"), ("Alingsås", "Västergötland"), ("Borås", "Västergötland"),
        ("Ulricehamn", "Västergötland"), ("Åmål", "Dalsland"), ("Mariestad", "Västergötland"), ("Lidköping", "Västergötland"),
        ("Skara", "Västergötland"), ("Skövde", "Västergötland"), ("Hjo", "Västergötland"), ("Tidaholm", "Västergötland"),
        ("Falköping", "Västergötland"),

        // Värmlands län
        ("Kil", "Värmland"), ("Eda", "Värmland"), ("Torsby", "Värmland"), ("Storfors", "Värmland"),
        ("Hammarö", "Värmland"), ("Munkfors", "Värmland"), ("Forshaga", "Värmland"), ("Grums", "Värmland"),
        ("Årjäng", "Värmland"), ("Sunne", "Värmland"), ("Karlstad", "Värmland"), ("Kristinehamn", "Värmland"),
        ("Filipstad", "Värmland"), ("Hagfors", "Värmland"), ("Arvika", "Värmland"), ("Säffle", "Värmland"),

        // Örebro län
        ("Lekeberg", "Närke"), ("Laxå", "Närke"), ("Hallsberg", "Närke"), ("Degerfors", "Värmland"),
        ("Hällefors", "Västmanland"), ("Ljusnarsberg", "Västmanland"), ("Örebro", "Närke"), ("Kumla", "Närke"),
        ("Askersund", "Närke"), ("Karlskoga", "Värmland"), ("Nora", "Västmanland"), ("Lindesberg", "Västmanland"),

        // Västmanlands län
        ("Skinnskatteberg", "Västmanland"), ("Surahammar", "Västmanland"), ("Kungsör", "Södermanland"), ("Hallstahammar", "Västmanland"),
        ("Norberg", "Västmanland"), ("Västerås", "Västmanland"), ("Sala", "Västmanland"), ("Fagersta", "Västmanland"),
        ("Köping", "Västmanland"), ("Arboga", "Västmanland"),

        // Dalarnas län
        ("Vansbro", "Dalarna"), ("Malung-Sälen", "Dalarna"), ("Gagnef", "Dalarna"), ("Leksand", "Dalarna"),
        ("Rättvik", "Dalarna"), ("Orsa", "Dalarna"), ("Älvdalen", "Dalarna"), ("Smedjebacken", "Dalarna"),
        ("Mora", "Dalarna"), ("Falun", "Dalarna"), ("Borlänge", "Dalarna"), ("Säter", "Dalarna"),
        ("Hedemora", "Dalarna"), ("Avesta", "Dalarna"), ("Ludvika", "Dalarna"),

        // Gävleborgs län
        ("Ockelbo", "Gästrikland"), ("Hofors", "Gästrikland"), ("Ovanåker", "Hälsingland"), ("Nordanstig", "Hälsingland"),
        ("Ljusdal", "Hälsingland"), ("Gävle", "Gästrikland"), ("Sandviken", "Gästrikland"), ("Söderhamn", "Hälsingland"),
        ("Bollnäs", "Hälsingland"), ("Hudiksvall", "Hälsingland"),

        // Västernorrlands län
        ("Ånge", "Medelpad"), ("Timrå", "Medelpad"), ("Härnösand", "Ångermanland"), ("Sundsvall", "Medelpad"),
        ("Kramfors", "Ångermanland"), ("Sollefteå", "Ångermanland"), ("Örnsköldsvik", "Ångermanland"),

        // Jämtlands län
        ("Ragunda", "Jämtland"), ("Bräcke", "Jämtland"), ("Krokom", "Jämtland"), ("Strömsund", "Jämtland"),
        ("Åre", "Jämtland"), ("Berg", "Jämtland"), ("Härjedalen", "Härjedalen"), ("Östersund", "Jämtland"),

        // Västerbottens län
        ("Nordmaling", "Ångermanland"), ("Bjurholm", "Ångermanland"), ("Vindeln", "Västerbotten"), ("Robertsfors", "Västerbotten"),
        ("Norsjö", "Västerbotten"), ("Malå", "Lappland"), ("Storuman", "Lappland"), ("Sorsele", "Lappland"),
        ("Dorotea", "Lappland"), ("Vännäs", "Västerbotten"), ("Vilhelmina", "Lappland"), ("Åsele", "Lappland"),
        ("Umeå", "Västerbotten"), ("Lycksele", "Lappland"), ("Skellefteå", "Västerbotten"),

        // Norrbottens län
        ("Arvidsjaur", "Lappland"), ("Arjeplog", "Lappland"), ("Jokkmokk", "Lappland"), ("Överkalix", "Norrbotten"),
        ("Kalix", "Norrbotten"), ("Övertorneå", "Norrbotten"), ("Pajala", "Norrbotten"), ("Gällivare", "Lappland"),
        ("Älvsbyn", "Norrbotten"), ("Luleå", "Norrbotten"), ("Piteå", "Norrbotten"), ("Boden", "Norrbotten"),
        ("Haparanda", "Norrbotten"), ("Kiruna", "Lappland")
    ];

    public static Country Country => new() { Id = SwedenId, Name = "Sverige" };

    public static IEnumerable<Province> GetProvinces() =>
        Provinces.Select((name, index) => new Province { Id = index + 1, CountryId = SwedenId, Name = name });

    public static IEnumerable<Municipality> GetMunicipalities() =>
        Municipalities.Select((municipality, index) =>
        {
            var provinceIndex = Array.IndexOf(Provinces, municipality.Province);
            if (provinceIndex < 0)
            {
                throw new InvalidOperationException($"Unknown province '{municipality.Province}' for {municipality.Name}.");
            }

            return new Municipality
            {
                Id = index + 1,
                ProvinceId = provinceIndex + 1,
                CountryId = SwedenId,
                Name = municipality.Name
            };
        });
}
