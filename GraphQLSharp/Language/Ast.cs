using System;
using System.Collections.Generic;

namespace GraphQLSharp.Language
{
    /// <summary>
    /// Contains a range of UTF-8 character offsets that identify
    /// the region of the source from which the AST derived.
    /// </summary>
    public class Location
    {
        public int Start { get; set; }
        public int End { get; set; }
        public Source Source { get; set; }

        public Location()
        {
        }

        public Location(int start, int end, Source source)
        {
            Start = start;
            End = end;
            Source = source;
        }
    }

    /// <summary>
    /// The list of all possible AST node types.
    /// </summary>
    public enum NodeType
    {
        Name,
        Document,
        OperationDefinition,
        VariableDefinition,
        Variable,
        SelectionSet,
        Field,
        Argument,
        FragmentSpread,
        InlineFragment,
        FragmentDefinition,
        IntValue,
        FloatValue,
        StringValue,
        BooleanValue,
        EnumValue,
        ArrayValue,
        ObjectValue,
        ObjectField,
        Directive,
        ListType,
        NonNullType,
    }

    public class Node
    {
        public Node(NodeType Kind, Location location)
        {
            Properties = new Dictionary<string, object>();
            Location = location;
        }

        public NodeType Kind { get; set; }
        public Location Location { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public object this[string index]
        {
            get { return Properties[index]; }
            set { Properties[index] = value; }
        }

        public static Node CreateName(String value, Location location)
        {
            var name = new Node(NodeType.Name, location);
            name["Value"] = value;
            return name;
        }

        public static Node CreateDocument(List<Node> definitions, Location location)
        {
            var document = new Node(NodeType.Document, location);
            document["Definitions"] = definitions;
            return document;
        }

        public static Node CreateOperationDefinition(
            OperationType operation, 
            Node name,
            List<Node> variableDefinitions,
            List<Node> directives,
            Node selectionSet,
            Location location)
        {
            var operationDefinition = new Node(NodeType.OperationDefinition, location);
            operationDefinition["Operation"] = operation;
            operationDefinition["Name"] = name;
            operationDefinition["VariableDefinitions"] = variableDefinitions;
            operationDefinition["Directives"] = directives;
            operationDefinition["SelectionSet"] = selectionSet;
            return operationDefinition;
        }

        public static Node CreateVariableDefinition(
            Node variable,
            Node type,
            Node defaultValue,
            Location location)
        {
            var variableDefinition = new Node(NodeType.VariableDefinition, location);
            variableDefinition["Variable"] =variable;
            variableDefinition["Type"] = type;
            variableDefinition["DefaultValue"] = defaultValue;
            return variableDefinition;
        }

        public static Node CreateVariable(Node name, Location location)
        {
            var variable = new Node(NodeType.Variable, location);
            variable["Name"] = name;
            return variable;
        }

        public static Node CreateSelectionSet(List<Node> selections, Location location)
        {
            var selectionSet = new Node(NodeType.SelectionSet, location);
            selectionSet["Selections"] = selections;
            return selectionSet;
        }

        public static Node CreateField(Node alias,
            Node name,
            List<Node> arguments,
            List<Node> directives,
            Node selectionSet,
            Location location)
        {
            var field = new Node(NodeType.Field, location);
            field["Alias"] = alias;
            field["Name"] = name;
            field["Arguments"] = arguments;
            field["Directives"] = directives;
            field["SelectionSet"] = selectionSet;
            return field;
        }

        public static Node CreateArgument(
            Node name,
            Node value,
            Location location)
        {
            var argument = new Node(NodeType.SelectionSet, location);
            argument["Name"] = name;
            argument["Value"] = value;
            return argument;
        }

        public static Node CreateFragmentSpread(
            Node name,
            List<Node> directives,
            Location location)
        {
            var FragmentSpread = new Node(NodeType.FragmentSpread, location);
            FragmentSpread["Name"] = name;
            FragmentSpread["Directives"] = directives;
            return FragmentSpread;
        }

        public static Node CreateInlineFragment(
            Node typeCondition,
            List<Node> directives,
            Node selectionSet,
            Location location)
        {
            var inlineFragment = new Node(NodeType.InlineFragment, location);
            inlineFragment["TypeCondition"] = typeCondition;
            inlineFragment["Directives"] = directives;
            inlineFragment["SelectionSet"] = selectionSet;
            return inlineFragment;
        }

        public static Node CreateFragmentDefinition(
            Node name,
            Node typeCondition,
            List<Node> directives,
            Node selectionSet,
            Location location)
        {
            var fragmentDefinition = new Node(NodeType.FragmentDefinition, location);
            fragmentDefinition["Name"] = name;
            fragmentDefinition["TypeCondition"] = typeCondition;
            fragmentDefinition["Directives"] = directives;
            fragmentDefinition["SelectionSet"] = selectionSet;
            return fragmentDefinition;
        }

        public static Node CreateIntValue(
            String value,
            Location location)
        {
            var intValue = new Node(NodeType.IntValue, location);
            intValue["Value"] = value;
            return intValue;
        }

        public static Node CreateFloatValue(
            String value,
            Location location)
        {
            var floatValue = new Node(NodeType.FloatValue, location);
            floatValue["Value"] = value;
            return floatValue;
        }

        public static Node CreateStringValue(
            String value,
            Location location)
        {
            var stringValue = new Node(NodeType.StringValue, location);
            stringValue["Value"] = value;
            return stringValue;
        }

        public static Node CreateBooleanValue(
            Boolean value,
            Location location)
        {
            var booleanValue = new Node(NodeType.BooleanValue, location);
            booleanValue["Value"] = value;
            return booleanValue;
        }

        public static Node CreateEnumValue(
            String value,
            Location location)
        {
            var enumValue = new Node(NodeType.EnumValue, location);
            enumValue["Value"] = value;
            return enumValue;
        }

        public static Node CreateArrayValue(
            List<Node> values,
            Location location)
        {
            var arrayValue = new Node(NodeType.ArrayValue, location);
            arrayValue["Values"] = values;
            return arrayValue;
        }

        public static Node CreateObjectValue(
            List<Node> fields,
            Location location)
        {
            var objectValue = new Node(NodeType.ObjectValue, location);
            objectValue["Fields"] = fields;
            return objectValue;
        }

        public static Node CreateObjectField(
            Node name,
            Node value,
            Location location)
        {
            var objectField = new Node(NodeType.ObjectField, location);
            objectField["Name"] = name;
            objectField["Value"] = value;
            return objectField;
        }

        public static Node CreateDirective(
            Node name,
            Node value,
            Location location)
        {
            var Directive = new Node(NodeType.Directive, location);
            Directive["Name"] = name;
            Directive["Value"] = value;
            return Directive;
        }

        public static Node CreateListType(
            Node type,
            Location location)
        {
            var listType = new Node(NodeType.ListType, location);
            listType["Type"] = type;
            return listType;
        }

        public static Node CreateNonNullType(
            Node type,
            Location location)
        {
            var nonNullType = new Node(NodeType.NonNullType, location);
            nonNullType["Type"] = type;
            return nonNullType;
        }
    }

    public enum OperationType
    {
        Query,
        Mutation,
    }
}
