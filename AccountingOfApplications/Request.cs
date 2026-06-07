using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingOfApplications
{
    public class Request
    {
        public string address { get; set; }
        public string work_type { get; set; }
        public string status { get; set; }
        public string priority { get; set; }
        public string created_date { get; set; }
        public string customer_phone { get; set; }
        public string operator_comment { get; set; }
        public string brigade_comment { get; set; }
        public bool need_approval { get; set; }
        public string assigned_to { get; set; }
    }
}