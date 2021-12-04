namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            TheatreExportDTO[] theatreExportDTOs = context
                .Theatres
                .ToArray()
                .Where(theatre => theatre.NumberOfHalls >= numbersOfHalls
                && theatre.Tickets.Count() >= 20)
                .Select(theatre => new TheatreExportDTO()
                {
                    Name = theatre.Name,
                    Halls = theatre.NumberOfHalls,

                    Tickets = theatre.Tickets
                    .OrderByDescending(ticket => ticket.Price)
                    .Where(ticket => ticket.RowNumber >= 1 &&
                             ticket.RowNumber <= 5)
                    .Select(ticket => new TicketExportDTO()
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber
                    })
                    .ToArray(),

                    TotalIncome = theatre.Tickets
                    .Where(ticket => ticket.RowNumber >= 1 &&
                           ticket.RowNumber <= 5)
                    .Sum(ticket => ticket.Price)
                })
                .ToArray()
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToArray();

            string result = JsonConvert.SerializeObject(theatreExportDTOs, Formatting.Indented);

            return result;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            XmlRootAttribute root = new XmlRootAttribute("Plays");

            XmlSerializer serializer = new
                XmlSerializer(typeof(PlayExportDTO[]), root);

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty,string.Empty);

            using StringWriter sr = new StringWriter(sb);

            PlayExportDTO[] playExports = context
                .Plays
                .ToArray()
                .Where(p => p.Rating <= rating)
                .Select(play => new PlayExportDTO()
                {
                    Title = play.Title,
                    Duration= play.Duration.ToString("c"),
                    Rating= play.Rating==0 ? "Premier" : play.Rating.ToString(),
                    Genre= play.Genre.ToString(),
                    Actors= play.Casts
                    .Where(actor=>actor.IsMainCharacter==true)
                    .Select(cast=> new ActorExportDTO()
                    {
                        FullName=cast.FullName,
                        MainCharacter = $"Plays main character in '{play.Title}'."
                    })
                    .OrderByDescending(actor => actor.FullName)
                    .ToArray()
                })
                .ToArray()
                .OrderBy(p=>p.Title)
                .ThenByDescending(p=>p.Genre)
                .ToArray();


            serializer.Serialize(sr, playExports, ns);

            string result = sb.ToString().TrimEnd();
            return result;
        }
    }
}
