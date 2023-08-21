using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PatientDataModel
    {
        public int Id { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string HospitalName { get; set; }
        public string Section { get; set; }
        public string Bed { get; set; }
        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Weight { get; set; }
    }
}
