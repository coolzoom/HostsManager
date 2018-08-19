using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HostsManager
{
    class Common
    {
        private static Regex rex = new Regex(@"^\d+$");

        /// <summary>
        /// 判断是否纯数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool isNumberic(string str)
        {
            //System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            int result = -1;
            if (rex.IsMatch(str))
            {
                result = int.Parse(str);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 文本写入指定的路径文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">文本内容</param>
        /// <param name="encoding">文本编码</param>
        public static void write(string path, string content, System.Text.Encoding encoding)
        {
            File.WriteAllText(path, content);
        }

        /// <summary>  
        /// 匹配获取字符串中所有的域名  
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public static List<string> matchsDomain(string input)
        {
            string pattern = @"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+";
            return Matchs(input, pattern);
        }

        /// <summary>  
        /// 匹配结果  返回匹配结果的数组  
        /// </summary>  
        /// <param name="input"></param>  
        /// <param name="expression"></param>  
        /// <returns></returns>  
        private static List<string> Matchs(string input, string expression)
        {
            List<string> list = new List<string>();
            MatchCollection collection = Regex.Matches(input, expression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (Match item in collection)
            {
                if (item.Success)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }
    }
}
