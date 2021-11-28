using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.DataProcessor.Dto
{
    public class GameDeserializeDTO
    {
        public string Name { get; set; }
        public string Price { get; set; }

        public string ReleaseDate { get; set; }

        public string Developer { get; set; }

        public string Genre { get; set; }

        public string[] Tags { get; set; }
    }
}
