using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GraphQLSharp.Language
{
    public class SourceLocation
    {
        public static Regex LineRegexp = new Regex(@"\r\n|[\n\r\u2028\u2029]");

        public int Line { get; private set; }
        public int Column { get; private set; }

        public SourceLocation(Source source, int position)
        {
            Line = 1;
            Column = position + 1;
            Match match = LineRegexp.Match(source.Body);
            while (match != Match.Empty 
                && match.Index < position)
            {
                Line += 1;
                Column = position + 1 - (match.Index + match.Groups[0].Length);
                match = match.NextMatch();
            }
        }
    }
}
