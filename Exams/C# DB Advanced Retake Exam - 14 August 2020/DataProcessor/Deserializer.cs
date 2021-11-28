namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            DepartmentImportDTO[] departmentsDTO = JsonConvert.DeserializeObject<DepartmentImportDTO[]>(jsonString);

            HashSet<Department> departments = new HashSet<Department>();

            foreach (DepartmentImportDTO departmentDTO in departmentsDTO)
            {
                bool isInvalid = false;
                if (!IsValid(departmentDTO))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                foreach (CellImportDTO cell in departmentDTO.Cells)
                {
                    if (!IsValid(cell))
                    {
                        isInvalid = true;
                        break;
                    }
                }
                if (isInvalid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Cell[] cells = departmentDTO.Cells.Select(x => new Cell()
                {
                    CellNumber = x.CellNumber,
                    HasWindow = x.HasWindow,
                })
                .ToArray();

                Department department = new Department()
                {
                    Name = departmentDTO.Name
                };
                department.Cells = cells;

                departments.Add(department);
                sb.AppendLine($"Imported {departmentDTO.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            string test = sb.ToString().TrimEnd();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            PrisonerImportDTO[] prisonersDTOs = JsonConvert.DeserializeObject<PrisonerImportDTO[]>(jsonString);

            HashSet<Prisoner> prisoners = new HashSet<Prisoner>();

            foreach (PrisonerImportDTO prisonerDTO in prisonersDTOs)
            {
                bool isValid = true;
                if (!IsValid(prisonerDTO))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                foreach (MaiImportDTO mail in prisonerDTO.Mails)
                {
                    if (!IsValid(mail))
                    {
                        sb.AppendLine("Invalid Data");
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    continue;
                }

                bool isValidReleaseDate = DateTime.TryParseExact(prisonerDTO.ReleaseDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ReleaseDate);


                DateTime releaseDate = DateTime.ParseExact(prisonerDTO.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);

                Prisoner prisoner = new Prisoner();

                prisoner.FullName = prisonerDTO.FullName;
                prisoner.Nickname = prisonerDTO.Nickname;
                prisoner.Age = prisonerDTO.Age;
                prisoner.IncarcerationDate = releaseDate;
                prisoner.ReleaseDate = isValidReleaseDate ? (DateTime?)releaseDate : null;
                prisoner.Bail = prisonerDTO.Bail;
                prisoner.CellId = prisonerDTO.CellId;

                Mail[] mails = prisonerDTO.Mails.Select(m => new Mail()
                {
                    Description = m.Description,
                    Sender = m.Sender,
                    Address = m.Address
                }).ToArray();

                prisoner.Mails = mails;

                prisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            string result = sb.ToString().TrimEnd();
            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();


            XmlRootAttribute root = new XmlRootAttribute("Officers");
            XmlSerializer serializer = new XmlSerializer(typeof(HashSet<OfficerImportDTO>), root);

            using StringReader reader = new StringReader(xmlString);

            HashSet<OfficerImportDTO> importDTOs = (HashSet<OfficerImportDTO>)serializer
                .Deserialize(reader);

            HashSet<Officer> officers = new HashSet<Officer>();

            foreach (OfficerImportDTO dto in importDTOs)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                int officerPosition = GetPosition(dto.Position);

                if (officerPosition == -1)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }


                int officerWeapon = GetWeapon(dto.Weapon);

                if (officerWeapon == -1)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Officer officer = new Officer()
                {
                    FullName = dto.Name,
                    Salary = dto.Money,
                    Position = (Position)officerPosition,
                    Weapon = (Weapon)officerWeapon,
                    DepartmentId = dto.DepartmentId,
                    OfficerPrisoners = dto.Prisoners.Select(p => new OfficerPrisoner()
                    {
                        PrisonerId = p.Prisoner
                    }).ToArray()
                };
                officers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }
            context.Officers.AddRange(officers);
            context.SaveChanges();
            string result = sb.ToString().TrimEnd();
            return sb.ToString().TrimEnd();
        }
        public static int GetWeapon(string weapon)
        {

            if (weapon == "Knife")
            {
                return 0;
            }
            else if (weapon == "FlashPulse")
            {
                return 1;
            }
            else if (weapon == "ChainRifle")
            {
                return 2;
            }
            else if (weapon == "Pistol")
            {
                return 3;
            }
            else if (weapon == "Sniper")
            {
                return 4;
            }
            return -1;
        }
        public static int GetPosition(string position)
        {
            if (position == "Overseer")
            {
                return 0;
            }
            else if (position == "Guard")
            {
                return 1;
            }
            else if (position == "Watcher")
            {
                return 2;
            }
            else if (position == "Labour")
            {
                return 3;
            }
            return -1;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}