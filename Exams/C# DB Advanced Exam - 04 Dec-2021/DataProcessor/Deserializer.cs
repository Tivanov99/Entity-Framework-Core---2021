namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute("Plays");

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(PlayImportDTO[]), root);

            using StringReader sr = new StringReader(xmlString);

            PlayImportDTO[] playsDTOs = (PlayImportDTO[])xmlSerializer.Deserialize(sr);

            HashSet<Play> plays = new HashSet<Play>();

            foreach (PlayImportDTO playDTO in playsDTOs)
            {
                if (!IsValid(playDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isValidGenre = Enum.IsDefined(typeof(Genre), playDTO.Genre);

                if (!isValidGenre)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isValidDuration = TimeSpan
                    .TryParseExact(playDTO.Duration, "c",
                    CultureInfo.InvariantCulture, out TimeSpan Duration);

                if (!isValidDuration || Duration.Hours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Play play = new Play()
                {
                    Title = playDTO.Title,
                    Duration = Duration,
                    Rating = playDTO.Rating,
                    Genre = (Genre)Enum.Parse(typeof(Genre), playDTO.Genre),
                    Description = playDTO.Description,
                    Screenwriter = playDTO.Screenwriter,
                };

                plays.Add(play);
                sb.AppendLine(String
                    .Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating));
            }
            context.Plays.AddRange(plays);
            context.SaveChanges();
            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute("Casts");

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(CastImportDTO[]), root);

            using StringReader sr = new StringReader(xmlString);

            CastImportDTO[] castImports =
                (CastImportDTO[])xmlSerializer.Deserialize(sr);

            HashSet<Cast> casts = new HashSet<Cast>();

            foreach (CastImportDTO castDTO in castImports)
            {
                if (!IsValid(castDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast cast = new Cast()
                {
                    FullName = castDTO.FullName,
                    IsMainCharacter = castDTO.IsMainCharacter,
                    PhoneNumber = castDTO.PhoneNumber,
                    PlayId = castDTO.PlayId
                };

                casts.Add(cast);
                sb.AppendLine(string.Format(SuccessfulImportActor
                    , cast.FullName, cast.IsMainCharacter == true ? "main" : "lesser"));
            }
            context.Casts.AddRange(casts);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {

            HashSet<int> AllPlays = context
                .Plays
                .Select(play => play.Id)
                .ToHashSet();

            StringBuilder sb = new StringBuilder();

            TheatreImportDTO[] theatreImports = JsonConvert
                .DeserializeObject<TheatreImportDTO[]>(jsonString);


            HashSet<Theatre> theatres = new HashSet<Theatre>();

            foreach (TheatreImportDTO theatreDTO in theatreImports)
            {
                if (!IsValid(theatreDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                Theatre theatre = new Theatre()
                {
                    Name = theatreDTO.Name,
                    NumberOfHalls = theatreDTO.NumberOfHalls,
                    Director = theatreDTO.Director,
                };

                foreach (TicketImportDTO ticketDTO in theatreDTO.Tickets)
                {
                    if (!IsValid(ticketDTO) || !AllPlays.Contains(ticketDTO.PlayId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticket = new Ticket()
                    {
                        Price = ticketDTO.Price,
                        RowNumber = ticketDTO.RowNumber,
                        PlayId = ticketDTO.PlayId,
                    };
                    theatre.Tickets.Add(ticket);
                }



                theatres.Add(theatre);
                sb.AppendLine(String.Format(SuccessfulImportTheatre, theatre.Name,
                   theatre.Tickets.Count));
            }

            context.Theatres.AddRange(theatres);
            context.SaveChanges();
            string result = sb.ToString().TrimEnd();
            return result;
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
