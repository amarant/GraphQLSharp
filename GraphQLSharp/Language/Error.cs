using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLSharp.Language
{
    public class SyntaxError : Exception
    {
        public SyntaxError(Source source, int position, String description)
        {
            Source = source;
            Position = position;
            Description = description;
            Location = new SourceLocation(source, position);
        }

        public Source Source { get; set; }
        public int Position { get; set; }
        public string Description { get; set; }
        public SourceLocation Location { get; set; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return String.Format("Syntax Error {0} ({1}:{2}) {3}\n\n{4}",
                    Source.Name,
                    Location.Line,
                    Location.Column,
                    Description,
                    HighlightSourceAtLocation(Source, Location));
            }
        }

        /// <summary>
        /// Highlights the source at location.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public static String HighlightSourceAtLocation(Source source, SourceLocation location)
        {
            var line = location.Line;
            var prevLineNum = (line - 1).ToString();
            var lineNum = line.ToString();
            var nextLineNum = (line + 1).ToString();
            var padLen = nextLineNum.Length;
            var lines = SourceLocation.LineRegexp.Split(source.Body);
            var res = (line >= 2
                ? prevLineNum.PadLeft(padLen) + ": " + lines[line - 2] + "\n"
                : "") +
                      lineNum.PadLeft(padLen) + ": " + lines[line - 1] + "\n" +
                      "".PadLeft(2 + padLen + location.Column - 1) + "^\n" +
                      (line < lines.Length
                          ? nextLineNum.PadLeft(padLen) + ": " + lines[line] + "\n"
                          : "");
            return res;
        }
    }
}
