using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLSharp.Language
{
    public enum VisitActionType
    {
        NoAction,
        Skip,
        Break,
        Replace
    }

    public class VisitAction
    {
        public static readonly VisitAction NoAction = new VisitAction(VisitActionType.NoAction);
        public static readonly VisitAction Skip = new VisitAction(VisitActionType.Skip);
        public static readonly VisitAction Break = new VisitAction(VisitActionType.Break);

        public VisitActionType VisitActionType { get; set; }
        public INode ReplaceNode { get; set; }

        public VisitAction(VisitActionType visitActionType, 
            INode replaceNode = null)
        {
            VisitActionType = visitActionType;
            ReplaceNode = replaceNode;
        }
    }

    public class Visitor
    {
        
    }
}
