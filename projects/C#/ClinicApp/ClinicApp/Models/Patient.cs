using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicApp.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string PhoneExtra { get; set; }
        public string Address { get; set; }
        public string Policy { get; set; }
        public string Snils { get; set; }
        public string Notes { get; set; }

        // удобное свойство чтобы везде показывать ФИО
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
