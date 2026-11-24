using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TRS_Lib.Migrations
{
    /// <inheritdoc />
    public partial class AddGeography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provinces_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProvinceId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipalities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Municipalities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Sverige" });

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Skåne" },
                    { 2, 1, "Blekinge" },
                    { 3, 1, "Halland" },
                    { 4, 1, "Småland" },
                    { 5, 1, "Öland" },
                    { 6, 1, "Gotland" },
                    { 7, 1, "Östergötland" },
                    { 8, 1, "Västergötland" },
                    { 9, 1, "Bohuslän" },
                    { 10, 1, "Dalsland" },
                    { 11, 1, "Värmland" },
                    { 12, 1, "Närke" },
                    { 13, 1, "Västmanland" },
                    { 14, 1, "Södermanland" },
                    { 15, 1, "Uppland" },
                    { 16, 1, "Dalarna" },
                    { 17, 1, "Gästrikland" },
                    { 18, 1, "Hälsingland" },
                    { 19, 1, "Härjedalen" },
                    { 20, 1, "Jämtland" },
                    { 21, 1, "Medelpad" },
                    { 22, 1, "Ångermanland" },
                    { 23, 1, "Västerbotten" },
                    { 24, 1, "Norrbotten" },
                    { 25, 1, "Lappland" }
                });

            migrationBuilder.InsertData(
                table: "Municipalities",
                columns: new[] { "Id", "CountryId", "Name", "ProvinceId" },
                values: new object[,]
                {
                    { 1, 1, "Upplands Väsby", 15 },
                    { 2, 1, "Vallentuna", 15 },
                    { 3, 1, "Österåker", 15 },
                    { 4, 1, "Värmdö", 15 },
                    { 5, 1, "Järfälla", 15 },
                    { 6, 1, "Ekerö", 15 },
                    { 7, 1, "Huddinge", 14 },
                    { 8, 1, "Botkyrka", 14 },
                    { 9, 1, "Salem", 14 },
                    { 10, 1, "Haninge", 14 },
                    { 11, 1, "Tyresö", 14 },
                    { 12, 1, "Upplands-Bro", 15 },
                    { 13, 1, "Nykvarn", 14 },
                    { 14, 1, "Täby", 15 },
                    { 15, 1, "Danderyd", 15 },
                    { 16, 1, "Sollentuna", 15 },
                    { 17, 1, "Stockholm", 15 },
                    { 18, 1, "Södertälje", 14 },
                    { 19, 1, "Nacka", 14 },
                    { 20, 1, "Sundbyberg", 15 },
                    { 21, 1, "Solna", 15 },
                    { 22, 1, "Lidingö", 15 },
                    { 23, 1, "Vaxholm", 15 },
                    { 24, 1, "Norrtälje", 15 },
                    { 25, 1, "Sigtuna", 15 },
                    { 26, 1, "Nynäshamn", 14 },
                    { 27, 1, "Håbo", 15 },
                    { 28, 1, "Älvkarleby", 15 },
                    { 29, 1, "Knivsta", 15 },
                    { 30, 1, "Heby", 13 },
                    { 31, 1, "Tierp", 15 },
                    { 32, 1, "Uppsala", 15 },
                    { 33, 1, "Enköping", 15 },
                    { 34, 1, "Östhammar", 15 },
                    { 35, 1, "Vingåker", 14 },
                    { 36, 1, "Gnesta", 14 },
                    { 37, 1, "Nyköping", 14 },
                    { 38, 1, "Oxelösund", 14 },
                    { 39, 1, "Flen", 14 },
                    { 40, 1, "Katrineholm", 14 },
                    { 41, 1, "Eskilstuna", 14 },
                    { 42, 1, "Strängnäs", 14 },
                    { 43, 1, "Trosa", 14 },
                    { 44, 1, "Ödeshög", 7 },
                    { 45, 1, "Ydre", 7 },
                    { 46, 1, "Kinda", 7 },
                    { 47, 1, "Boxholm", 7 },
                    { 48, 1, "Åtvidaberg", 7 },
                    { 49, 1, "Finspång", 7 },
                    { 50, 1, "Valdemarsvik", 7 },
                    { 51, 1, "Linköping", 7 },
                    { 52, 1, "Norrköping", 7 },
                    { 53, 1, "Söderköping", 7 },
                    { 54, 1, "Motala", 7 },
                    { 55, 1, "Vadstena", 7 },
                    { 56, 1, "Mjölby", 7 },
                    { 57, 1, "Aneby", 4 },
                    { 58, 1, "Gnosjö", 4 },
                    { 59, 1, "Mullsjö", 8 },
                    { 60, 1, "Habo", 8 },
                    { 61, 1, "Gislaved", 4 },
                    { 62, 1, "Vaggeryd", 4 },
                    { 63, 1, "Jönköping", 4 },
                    { 64, 1, "Nässjö", 4 },
                    { 65, 1, "Värnamo", 4 },
                    { 66, 1, "Sävsjö", 4 },
                    { 67, 1, "Vetlanda", 4 },
                    { 68, 1, "Eksjö", 4 },
                    { 69, 1, "Tranås", 4 },
                    { 70, 1, "Uppvidinge", 4 },
                    { 71, 1, "Lessebo", 4 },
                    { 72, 1, "Tingsryd", 4 },
                    { 73, 1, "Alvesta", 4 },
                    { 74, 1, "Älmhult", 4 },
                    { 75, 1, "Markaryd", 4 },
                    { 76, 1, "Växjö", 4 },
                    { 77, 1, "Ljungby", 4 },
                    { 78, 1, "Högsby", 4 },
                    { 79, 1, "Torsås", 4 },
                    { 80, 1, "Mörbylånga", 5 },
                    { 81, 1, "Hultsfred", 4 },
                    { 82, 1, "Mönsterås", 4 },
                    { 83, 1, "Emmaboda", 4 },
                    { 84, 1, "Kalmar", 4 },
                    { 85, 1, "Nybro", 4 },
                    { 86, 1, "Oskarshamn", 4 },
                    { 87, 1, "Västervik", 4 },
                    { 88, 1, "Vimmerby", 4 },
                    { 89, 1, "Borgholm", 5 },
                    { 90, 1, "Gotland", 6 },
                    { 91, 1, "Olofström", 2 },
                    { 92, 1, "Karlskrona", 2 },
                    { 93, 1, "Ronneby", 2 },
                    { 94, 1, "Karlshamn", 2 },
                    { 95, 1, "Sölvesborg", 2 },
                    { 96, 1, "Vellinge", 1 },
                    { 97, 1, "Östra Göinge", 1 },
                    { 98, 1, "Örkelljunga", 1 },
                    { 99, 1, "Bjuv", 1 },
                    { 100, 1, "Kävlinge", 1 },
                    { 101, 1, "Lomma", 1 },
                    { 102, 1, "Svedala", 1 },
                    { 103, 1, "Skurup", 1 },
                    { 104, 1, "Sjöbo", 1 },
                    { 105, 1, "Hörby", 1 },
                    { 106, 1, "Höör", 1 },
                    { 107, 1, "Tomelilla", 1 },
                    { 108, 1, "Bromölla", 1 },
                    { 109, 1, "Osby", 1 },
                    { 110, 1, "Perstorp", 1 },
                    { 111, 1, "Klippan", 1 },
                    { 112, 1, "Åstorp", 1 },
                    { 113, 1, "Båstad", 1 },
                    { 114, 1, "Malmö", 1 },
                    { 115, 1, "Lund", 1 },
                    { 116, 1, "Landskrona", 1 },
                    { 117, 1, "Helsingborg", 1 },
                    { 118, 1, "Höganäs", 1 },
                    { 119, 1, "Eslöv", 1 },
                    { 120, 1, "Ystad", 1 },
                    { 121, 1, "Trelleborg", 1 },
                    { 122, 1, "Kristianstad", 1 },
                    { 123, 1, "Simrishamn", 1 },
                    { 124, 1, "Ängelholm", 1 },
                    { 125, 1, "Hässleholm", 1 },
                    { 126, 1, "Staffanstorp", 1 },
                    { 127, 1, "Burlöv", 1 },
                    { 128, 1, "Svalöv", 1 },
                    { 129, 1, "Hylte", 3 },
                    { 130, 1, "Halmstad", 3 },
                    { 131, 1, "Laholm", 3 },
                    { 132, 1, "Falkenberg", 3 },
                    { 133, 1, "Varberg", 3 },
                    { 134, 1, "Kungsbacka", 3 },
                    { 135, 1, "Härryda", 8 },
                    { 136, 1, "Partille", 8 },
                    { 137, 1, "Öckerö", 9 },
                    { 138, 1, "Stenungsund", 9 },
                    { 139, 1, "Tjörn", 9 },
                    { 140, 1, "Orust", 9 },
                    { 141, 1, "Sotenäs", 9 },
                    { 142, 1, "Munkedal", 9 },
                    { 143, 1, "Tanum", 9 },
                    { 144, 1, "Dals-Ed", 10 },
                    { 145, 1, "Färgelanda", 10 },
                    { 146, 1, "Ale", 8 },
                    { 147, 1, "Lerum", 8 },
                    { 148, 1, "Vårgårda", 8 },
                    { 149, 1, "Bollebygd", 8 },
                    { 150, 1, "Grästorp", 8 },
                    { 151, 1, "Essunga", 8 },
                    { 152, 1, "Karlsborg", 8 },
                    { 153, 1, "Gullspång", 8 },
                    { 154, 1, "Tranemo", 8 },
                    { 155, 1, "Bengtsfors", 10 },
                    { 156, 1, "Mellerud", 10 },
                    { 157, 1, "Lilla Edet", 8 },
                    { 158, 1, "Mark", 8 },
                    { 159, 1, "Svenljunga", 8 },
                    { 160, 1, "Herrljunga", 8 },
                    { 161, 1, "Vara", 8 },
                    { 162, 1, "Götene", 8 },
                    { 163, 1, "Tibro", 8 },
                    { 164, 1, "Töreboda", 8 },
                    { 165, 1, "Göteborg", 8 },
                    { 166, 1, "Mölndal", 8 },
                    { 167, 1, "Kungälv", 9 },
                    { 168, 1, "Lysekil", 9 },
                    { 169, 1, "Uddevalla", 9 },
                    { 170, 1, "Strömstad", 9 },
                    { 171, 1, "Vänersborg", 8 },
                    { 172, 1, "Trollhättan", 8 },
                    { 173, 1, "Alingsås", 8 },
                    { 174, 1, "Borås", 8 },
                    { 175, 1, "Ulricehamn", 8 },
                    { 176, 1, "Åmål", 10 },
                    { 177, 1, "Mariestad", 8 },
                    { 178, 1, "Lidköping", 8 },
                    { 179, 1, "Skara", 8 },
                    { 180, 1, "Skövde", 8 },
                    { 181, 1, "Hjo", 8 },
                    { 182, 1, "Tidaholm", 8 },
                    { 183, 1, "Falköping", 8 },
                    { 184, 1, "Kil", 11 },
                    { 185, 1, "Eda", 11 },
                    { 186, 1, "Torsby", 11 },
                    { 187, 1, "Storfors", 11 },
                    { 188, 1, "Hammarö", 11 },
                    { 189, 1, "Munkfors", 11 },
                    { 190, 1, "Forshaga", 11 },
                    { 191, 1, "Grums", 11 },
                    { 192, 1, "Årjäng", 11 },
                    { 193, 1, "Sunne", 11 },
                    { 194, 1, "Karlstad", 11 },
                    { 195, 1, "Kristinehamn", 11 },
                    { 196, 1, "Filipstad", 11 },
                    { 197, 1, "Hagfors", 11 },
                    { 198, 1, "Arvika", 11 },
                    { 199, 1, "Säffle", 11 },
                    { 200, 1, "Lekeberg", 12 },
                    { 201, 1, "Laxå", 12 },
                    { 202, 1, "Hallsberg", 12 },
                    { 203, 1, "Degerfors", 11 },
                    { 204, 1, "Hällefors", 13 },
                    { 205, 1, "Ljusnarsberg", 13 },
                    { 206, 1, "Örebro", 12 },
                    { 207, 1, "Kumla", 12 },
                    { 208, 1, "Askersund", 12 },
                    { 209, 1, "Karlskoga", 11 },
                    { 210, 1, "Nora", 13 },
                    { 211, 1, "Lindesberg", 13 },
                    { 212, 1, "Skinnskatteberg", 13 },
                    { 213, 1, "Surahammar", 13 },
                    { 214, 1, "Kungsör", 14 },
                    { 215, 1, "Hallstahammar", 13 },
                    { 216, 1, "Norberg", 13 },
                    { 217, 1, "Västerås", 13 },
                    { 218, 1, "Sala", 13 },
                    { 219, 1, "Fagersta", 13 },
                    { 220, 1, "Köping", 13 },
                    { 221, 1, "Arboga", 13 },
                    { 222, 1, "Vansbro", 16 },
                    { 223, 1, "Malung-Sälen", 16 },
                    { 224, 1, "Gagnef", 16 },
                    { 225, 1, "Leksand", 16 },
                    { 226, 1, "Rättvik", 16 },
                    { 227, 1, "Orsa", 16 },
                    { 228, 1, "Älvdalen", 16 },
                    { 229, 1, "Smedjebacken", 16 },
                    { 230, 1, "Mora", 16 },
                    { 231, 1, "Falun", 16 },
                    { 232, 1, "Borlänge", 16 },
                    { 233, 1, "Säter", 16 },
                    { 234, 1, "Hedemora", 16 },
                    { 235, 1, "Avesta", 16 },
                    { 236, 1, "Ludvika", 16 },
                    { 237, 1, "Ockelbo", 17 },
                    { 238, 1, "Hofors", 17 },
                    { 239, 1, "Ovanåker", 18 },
                    { 240, 1, "Nordanstig", 18 },
                    { 241, 1, "Ljusdal", 18 },
                    { 242, 1, "Gävle", 17 },
                    { 243, 1, "Sandviken", 17 },
                    { 244, 1, "Söderhamn", 18 },
                    { 245, 1, "Bollnäs", 18 },
                    { 246, 1, "Hudiksvall", 18 },
                    { 247, 1, "Ånge", 21 },
                    { 248, 1, "Timrå", 21 },
                    { 249, 1, "Härnösand", 22 },
                    { 250, 1, "Sundsvall", 21 },
                    { 251, 1, "Kramfors", 22 },
                    { 252, 1, "Sollefteå", 22 },
                    { 253, 1, "Örnsköldsvik", 22 },
                    { 254, 1, "Ragunda", 20 },
                    { 255, 1, "Bräcke", 20 },
                    { 256, 1, "Krokom", 20 },
                    { 257, 1, "Strömsund", 20 },
                    { 258, 1, "Åre", 20 },
                    { 259, 1, "Berg", 20 },
                    { 260, 1, "Härjedalen", 19 },
                    { 261, 1, "Östersund", 20 },
                    { 262, 1, "Nordmaling", 22 },
                    { 263, 1, "Bjurholm", 22 },
                    { 264, 1, "Vindeln", 23 },
                    { 265, 1, "Robertsfors", 23 },
                    { 266, 1, "Norsjö", 23 },
                    { 267, 1, "Malå", 25 },
                    { 268, 1, "Storuman", 25 },
                    { 269, 1, "Sorsele", 25 },
                    { 270, 1, "Dorotea", 25 },
                    { 271, 1, "Vännäs", 23 },
                    { 272, 1, "Vilhelmina", 25 },
                    { 273, 1, "Åsele", 25 },
                    { 274, 1, "Umeå", 23 },
                    { 275, 1, "Lycksele", 25 },
                    { 276, 1, "Skellefteå", 23 },
                    { 277, 1, "Arvidsjaur", 25 },
                    { 278, 1, "Arjeplog", 25 },
                    { 279, 1, "Jokkmokk", 25 },
                    { 280, 1, "Överkalix", 24 },
                    { 281, 1, "Kalix", 24 },
                    { 282, 1, "Övertorneå", 24 },
                    { 283, 1, "Pajala", 24 },
                    { 284, 1, "Gällivare", 25 },
                    { 285, 1, "Älvsbyn", 24 },
                    { 286, 1, "Luleå", 24 },
                    { 287, 1, "Piteå", 24 },
                    { 288, 1, "Boden", 24 },
                    { 289, 1, "Haparanda", 24 },
                    { 290, 1, "Kiruna", 25 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_CountryId",
                table: "Municipalities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_ProvinceId_Name",
                table: "Municipalities",
                columns: new[] { "ProvinceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_CountryId_Name",
                table: "Provinces",
                columns: new[] { "CountryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Municipalities");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
