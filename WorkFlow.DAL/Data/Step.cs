using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkFlow.DAL.Data
{
    public class Step
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public XElement Definition { set; get; } // this particular step definition
    }
}
