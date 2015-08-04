using System;
using System.Collections.Immutable;
using System.Linq;
using static System.String;

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

        public static string JoinNotNull(string separator, ImmutableArray<string> value) 
            => Join(separator, value.Where(x => !IsNullOrEmpty(x)));

        public static string JoinNotNull(string separator, params string[] value) 
            => Join(separator, value.Where(x => !IsNullOrEmpty(x)));

        public static string Block(ImmutableArray<string> array)
            => array.IsEmpty
            ? ""
            : Indent($"{{\n{JoinNotNull("\n", array)}") + "\n}";

        public static string Wrap(string start, string value, string end = null) 
            => IsNullOrEmpty(value) 
            ? ""
            : $"{start}{value}{end ?? ""}";

        public static string Indent(string value)
            => IsNullOrEmpty(value)
            ? ""
            : value.Replace("\n", "\n  ");

        public override string VisitName(Name node) 
            => node.Value;

        public override string VisitDocument(Document node)
        {
            var defs = VisitList(node.Definitions);
            var res = JoinNotNull("\n\n", defs) + "\n";
            return res;
        }

        public override string VisitOperationDefinition(OperationDefinition node)
        {
            var op = node.Operation == OperationType.Query ? "query" : "mutation";
            var name = Visit(node.Name);
            var defs = Wrap("(", JoinNotNull(", ", VisitList(node.VariableDefinitions)), ")");
            var directives = JoinNotNull(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var nameDefsJoin = JoinNotNull("", name, defs);
            var res = IsNullOrEmpty(name) ? selectionSet 
                : JoinNotNull(" ", op, nameDefsJoin, directives, selectionSet);
            return res;
        }

        public override string VisitVariableDefinition(VariableDefinition node)
        {
            var variable = Visit(node.Variable);
            var type = Visit(node.Type);
            var defaultValue = Visit(node.DefaultValue);
            var res = $"{variable}: {type}{Wrap(" = ", defaultValue)}";
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
            return Block(selections);
        }

        public override string VisitField(Field node)
        {
            var nameArgs = Wrap("", Visit(node.Alias), ": ") + Visit(node.Name) +
                           Wrap("(", JoinNotNull(", ", VisitList(node.Arguments)), ")");
            var directives = JoinNotNull(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var res = JoinNotNull(" ", nameArgs, directives, selectionSet);
            return res;
        }

        public override string VisitArgument(Argument node) 
            => Visit(node.Name) + ": " + Visit(node.Value);

        public override string VisitFragmentSpread(FragmentSpread node)
        {
            var name = Visit(node.Name);
            var directives = JoinNotNull(" ", VisitList(node.Directives));
            var res = $"...{name}{Wrap(" ", directives)}";
            return res;
        }

        public override string VisitInlineFragment(InlineFragment node)
        {
            var typeCondition = Visit(node.TypeCondition);
            var directives = JoinNotNull(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var inlineFragment = $"... on {typeCondition} {Wrap("", directives, " ")}{selectionSet}";
            return inlineFragment;
        }

        public override string VisitFragmentDefinition(FragmentDefinition node)
        {
            var name = VisitName(node.Name);
            var typeCondition = Visit(node.TypeCondition);
            var directives = JoinNotNull(" ", VisitList(node.Directives));
            var selectionSet = Visit(node.SelectionSet);
            var fragmentFragment = $"fragment {name} on {typeCondition} {Wrap("", directives, " ")}{selectionSet}";
            return fragmentFragment;
        }

        public override string VisitIntValue(IntValue node) 
            => node.Value;

        public override string VisitFloatValue(FloatValue node) 
            => node.Value;

        public override string VisitStringValue(StringValue node) 
            => $"\"{node.Value}\"";

        public override string VisitBooleanValue(BooleanValue node) 
            => node.Value ? "true" : "false";

        public override string VisitEnumValue(EnumValue node) 
            => node.Value;

        public override string VisitListValue(ListValue node) 
            => $"[{JoinNotNull(", ", VisitList(node.Values))}]";

        public override string VisitObjectValue(ObjectValue node) 
            => $"{{{JoinNotNull(", ", VisitList(node.Fields))}}}";

        public override string VisitObjectField(ObjectField node) 
            => $"{Visit(node.Name)}: {Visit(node.Value)}";

        public override string VisitDirective(Directive node)
        {
            var name = Visit(node.Name);
            var arguments = VisitList(node.Arguments);
            var directive = $"@{name}{Wrap("(", JoinNotNull(", ", arguments), ")")}";
            return directive;
        }

        public override string VisitListType(ListType node) 
            => $"[{Visit(node.Type)}]";

        public override string VisitNonNullType(NonNullType node) 
            => $"{Visit(node.Type)}!";

        public override string VisitNamedType(NamedType node) 
            => Visit(node.Name);
    }
}
