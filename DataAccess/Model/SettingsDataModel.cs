using DataAccess.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class SettingsDataModel
    {
        public int Id { get; set; }
        public string BusinessDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public ParameterName Parameter { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }
}
