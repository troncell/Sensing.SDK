using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SensingBase.Utils
{
    public class TemplateHelper
    {
        private static string _strRegex = @"{\$\w+}";
        private static Regex regex ;
        private static IDictionary<string, string> variables;

        static TemplateHelper()
        {
            regex = new Regex(_strRegex);
        }
        #region ITemplateNodeHandler Members

        public static string Handle(XElement inputNode, IDictionary<string,string> context)
        {
            //if (string.IsNullOrEmpty(node.Value))
            //    return;
            variables = context;
            StringBuilder Output = new StringBuilder("");
            Output.Append(regex.Replace(inputNode.ToString(), new MatchEvaluator(MatchEvaluatorHandler)));
            return Output.ToString();
        }


        public static string Handle(string inputText, IDictionary<string, string> context)
        {
            if (context == null) return inputText;
            if (context.Count < 1) return inputText;
            variables = context;
            StringBuilder Output = new StringBuilder("");
            Output.Append(regex.Replace(inputText, new MatchEvaluator(MatchEvaluatorHandler)));
            return Output.ToString();

        }

        #endregion

        static string MatchEvaluatorHandler(Match match)
        {
            // remove "$" from beggining to get name
            string name = match.Value.Replace("{", "");//.Remove(0, 1);
            name = name.Replace("}", "");
            name = name.Replace("$", "");
            string value = null;

            // try to get variable
            if (!variables.TryGetValue(name, out value))
            {
                return match.Value;
                //throw new Exception("Variable " + match.Value + " not defined");
            }

            // search if variable exists in value and replace it with value
            if (regex.IsMatch(value))
            {
                StringBuilder res = new StringBuilder();
                res.Append(regex.Replace(value, new MatchEvaluator(MatchEvaluatorHandler)));
                return res.ToString();
            }

            return value;
        }
    }
}
