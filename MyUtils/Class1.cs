using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtils
{
    public class Utils
    {
        public static bool CheckTimeleft(string strTime)
        {
            char delimiter = ':';
            
            String[] substrings = strTime.Split(delimiter);

            if ((Convert.ToInt32(substrings[0]) + Convert.ToInt32(substrings[1]) + Convert.ToInt32(substrings[2])) > 0)
                return true;
            else return false;
        }
    }
}
