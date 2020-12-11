using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace OnePortal.Helper
{
    public static class XMLHelper
    {
        public static string GetValue(XElement element)
        {
            if (element.Value != null)
                return element.Value;
            return "";
        }
    }
}