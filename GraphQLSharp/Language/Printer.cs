using System;
using System.Collections.Immutable;
using System.Linq;

namespace GraphQLSharp.Language
{
    public class Printer : Visitor<String>
    {
        
        protected virtual ImmutableArray<String> VisitList<T>(ImmutableArray<T> list) where T : class, INode
        {
            var stringList = ImmutableArray<string>.Empty;
            if (list.IsDefault)
            {
                return stringList;
            }

            foreach (var elem in list)
            {
                stringList = stringList.Add(Visit(elem));
            }

            return stringList;
        }

        private static string ManyList(string start, ImmutableArray<String> list, string separator, string end)
        {
            return list.Any() ? start + Join(separator, list) + end : null;
        }

        public static string Join(string separator, ImmutableArray<String> value)
        {
            return String.Join(separator, value.Where(x => !string.IsNullOrEmpty(x)));
        }

        public static string Join(string separator, params string[] value)
        {
            return String.Join(separator, value.Where(x => !string.IsNullOrEmpty(x)));
        }

        public override string VisitName(Name node)
        {
            return node.Value;
        }

        public override string VisitDocument(Document node)
        {
            var defs = VisitList(node.Definitions);
            var res = Join("\n\n", defs) + "\n";
            return res;
        }

        public override string VisitOperationDefinition(OperationDefinition node)
        {
            var op = node.Operation == OperationType.Query ? "query" : "mutation";
            var name = Visit(node.Name);
            //TODO: verify with spec for unamed queries
            op = string.IsNullOrEmpty(name) ? null : op;
            var defs = ManyList("(", VisitList(node.VariableDefinitions), ", ", ")");
            var directives = Join(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var nameDefsJoin = Join("", name, defs);
            var res = Join(" ", op, nameDefsJoin, directives, selectionSet);
            return res;
        }

        public override string VisitVariableDefinition(VariableDefinition node)
        {
            var variable = Visit(node.Variable);
            var type = Visit(node.Type);
            var defaultValue = Visit(node.DefaultValue);
            var res = Join(" = ", variable + ": " + type, defaultValue);
            return res;
        }

        public override string VisitVariable(Variable node)
        {
            var name = Visit(node.Name);
            return "$" + name;
        }

        public override string VisitSelectionSet(SelectionSet node)
        {
            var selections = VisitList(node.Selections);
            var res = selections.Any()
                ? "{\n  " + Join("\n", selections).Replace("\n", "\n  ") + "\n}"
                : null;
            return res;
        }

        public override string VisitField(Field node)
        {
            var nameAlias = Join(": ", Visit(node.Alias), Visit(node.Name));
            var argList = VisitList(node.Arguments);
            var args = ManyList("(", argList, ", ", ")");
            var nameArgs = Join("", nameAlias, args);
            var directives = Join(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var res = Join(" ", nameArgs, directives, selectionSet);
            return res;
        }

        public override string VisitArgument(Argument node)
        {
            var name = Visit(node.Name);
            var value = Visit(node.Value);
            return name + ": " + value;
        }

        public override string VisitFragmentSpread(FragmentSpread node)
        {
            var name = Visit(node.Name);
            var directives = Join(" ", VisitList(node.Directives));
            var res = Join(" ", "..." + name, directives);
            return res;
        }

        public override string VisitInlineFragment(InlineFragment node)
        {
            var typeCondition = Visit(node.TypeCondition);
            var directives = Join(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var inlineFragment = Join(" ", "... on", typeCondition, directives, selectionSet);
            return inlineFragment;
        }

        public override string VisitFragmentDefinition(FragmentDefinition node)
        {
            var name = Visit(node.Name);
            var typeCondition = Visit(node.TypeCondition);
            var directives = Join(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var fragmentDefinition = Join(" ", "fragment", name, "on", typeCondition, directives, selectionSet);
            return fragmentDefinition;
        }

        public override string VisitIntValue(IntValue node)
        {
            return node.Value;
        }

        public override string VisitFloatValue(FloatValue node)
        {
            return node.Value;
        }

        public override string VisitStringValue(StringValue node)
        {
            return "\"" + node.Value + "\"";
        }

        public override string VisitBooleanValue(BooleanValue node)
        {
            return node.Value ? "true" : "false";
        }

        public override string VisitEnumValue(EnumValue node)
        {
            return node.Value;
        }

        public override string VisitArrayValue(ArrayValue node)
        {
            var values = Join(", ", VisitList(node.Values));
            return "[" + values + "]";
        }

        public override string VisitObjectValue(ObjectValue node)
        {
            var values = Join(", ", VisitList(node.Fields));
            return "{" + values + "}";
        }

        public override string VisitObjectField(ObjectField node)
        {
            return Visit(node.Name) + ": " + Visit(node.Value);
        }

        public override string VisitDirective(Directive node)
        {
            var name = Visit(node.Name);
            var arguments = ManyList("(", VisitList(node.Arguments), ", ", ")");
            var directive = Join("", "@" + name, arguments);
            return directive;
        }

        public override string VisitListType(ListType node)
        {
            var type = Visit(node.Type);
            return "[" + type + "]";
        }

        public override string VisitNonNullType(NonNullType node)
        {
            var type = Visit(node.Type);
            return type + "!";
        }
    }
}
