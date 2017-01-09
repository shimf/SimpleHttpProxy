using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SimpleHttpProxy
{
    public static class Extensions
    {
        
        public static int ToInt(this string number)
        {
            return int.Parse(number);
        }
        

    }
}
