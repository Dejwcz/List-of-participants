using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Seznam_ucastniku.Entities
{
    public class Record
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NickName { get; set; }
        public DateOnly? InDay { get; set; }
        public bool? InDayLunch { get; set; }
        public DateOnly? OutDay { get; set; }
        public bool? OutDayLunch { get;set; }
    }
}
