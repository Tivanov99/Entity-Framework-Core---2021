namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            PrisonerExportDTO[] prisoners = context
                .Prisoners
                .ToArray()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new PrisonerExportDTO()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    TotalOfficerSalary = p.PrisonerOfficers.Select(x => x.Officer)
                    .Sum(of => of.Salary),
                    Officers = p.PrisonerOfficers
                    .Select(of => of.Officer)
                    .Select(x => new PrisonerOfficerExportDTO()
                    {
                        OfficerName = x.FullName,
                        Department = x.Department.Name
                    })
                    .ToArray()
                    .OrderBy(of => of.OfficerName)
                    .ToArray()

                })
                .ToArray()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };
            string result = JsonConvert.SerializeObject(prisoners, settings);

            return result.ToString();
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] names = prisonersNames.Split(",");

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Prisoners");

            XmlSerializer serializer = new XmlSerializer(typeof(PrisonerDTO[]), xmlRoot);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            PrisonerDTO[] prisoners = context
                .Prisoners
                .ToArray()
                .Where(p => names.Contains(p.FullName))
                .Select(p => new PrisonerDTO()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = p.Mails.Select(m => new Message()
                    {
                        Description = ReverseString(m.Description),
                    }).ToArray()
                })
                .ToArray()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, prisoners, ns);

            string result = sb.ToString().TrimEnd();

            return sb.ToString().TrimEnd();
        }
        public static string ReverseString(string desctiption)
        {
            char[] letters = desctiption.ToCharArray();
            Array.Reverse(letters);
            return new string(letters);
        }
    }
}