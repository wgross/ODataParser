using Sprache;
using System.Linq.Expressions;

namespace Parser
{
    public class Functions
    {
        public static Parser<string> FunctionName => from name in Parse.Letter.AtLeastOnce().Text().Token()
                                                         select name;
    }
}