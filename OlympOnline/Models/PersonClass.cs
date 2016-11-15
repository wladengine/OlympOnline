using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OlympOnline.Models
{
    public class PersonClass
    {
        public Guid PersonId { get; set; }
        
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public bool Sex { get; set; }

        public DateTime BirthDate { get; set; }

        public string SchoolName { get; set; }
        public string FacultyName { get; set; }
    }
}