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

    public interface INode
    {
        NodeType Kind { get; }
    }

    public abstract class ANode : INode
    {
        public abstract NodeType Kind { get; }
        public Location Location { get; set; }
    }

    #region Name

    public class Name : ANode, IType, INameOrListType
    {
        public override NodeType Kind
        {
            get { return NodeType.Name; }
        }
        public String Value { get; set; }
    }

    #endregion Name

    #region Document

    public class Document : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.Document; }
        }

        public List<IDefinition> Definitions { get; set; }
    }

    public interface IDefinition
    {
        NodeType Kind { get; }
    }

    public enum OperationType
    {
        Query,
        Mutation,
    }

    public class OperationDefinition : ANode, IDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.OperationDefinition; }
        }

        public OperationType Operation { get; set; }
        public Name Name { get; set; }
        public List<VariableDefinition> VariableDefinitions { get; set; }
        public List<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }
    }

    public class VariableDefinition : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.VariableDefinition; }
        }

        public Variable Variable { get; set; }
        public IType Type { get; set; }
        public IValue DefaultValue { get; set; }
    }

    public class Variable : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.Variable; }
        }

        public Name Name { get; set; }
    }

    public class SelectionSet : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.SelectionSet; }
        }

        public List<ISelection> Selections { get; set; }

    }

    public interface ISelection
    {
        NodeType Kind { get; }
    }

    public class Field : ANode, ISelection
    {
        public override NodeType Kind
        {
            get { return NodeType.Field; }
        }
        public Name Alias { get; set; }
        public Name Name { get; set; }
        public List<Argument> Arguments { get; set; }
        public List<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }
    }

    public class Argument : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.Argument; }
        }

        public Name Name { get; set; }
        public IValue Value { get; set; }
    }

    #endregion Document

    #region Fragments

    public class FragmentSpread : ANode, ISelection
    {
        public override NodeType Kind
        {
            get { return NodeType.FragmentSpread; }
        }

        public Name Name { get; set; }
        public List<Directive> Directives { get; set; }
    }

    public class InlineFragment : ANode, ISelection
    {
        public override NodeType Kind
        {
            get { return NodeType.InlineFragment; }
        }

        public Name TypeCondition { get; set; }
        public List<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }
    }

    public class FragmentDefinition : ANode, IDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.FragmentDefinition; }
        }
        public Name Name { get; set; }
        public Name TypeCondition { get; set; }
        public List<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }
    }

    #endregion Fragments

    #region Values

    public interface IValue
    {
        NodeType Kind { get; }
    }

    public class IntValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.IntValue; }
        }

        public String Value { get; set; }
    }

    public class FloatValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.FloatValue; }
        }

        public String Value { get; set; }
    }

    public class StringValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.StringValue; }
        }

        public String Value { get; set; }
    }

    public class BooleanValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.BooleanValue; }
        }

        public Boolean Value { get; set; }
    }

    public class EnumValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.EnumValue; }
        }

        public String Value { get; set; }
    }

    public class ArrayValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.ArrayValue; }
        }

        public List<IValue> Values { get; set; }
    }

    public class ObjectValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.IntValue; }
        }

        public List<ObjectField> Fields { get; set; }
    }

    public class ObjectField : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.ObjectField; }
        }
        public Name Name { get; set; }
        public IValue Value { get; set; }

    }

    #endregion Values

    #region Directives

    public class Directive : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.Directive; }
        }
        public Name Name { get; set; }
        public IValue Value { get; set; }
    }

    #endregion Directives

    #region Types

    public interface IType : INode
    {
    }

    public interface INameOrListType : IType
    {
    }

    public class ListType : ANode, INameOrListType
    {
        public override NodeType Kind
        {
            get { return NodeType.ListType; }
        }

        public IType Type { get; set; }
    }

    public class NonNullType : ANode, IType
    {
        public override NodeType Kind
        {
            get { return NodeType.NonNullType; }
        }

        public INameOrListType Type { get; set; }
    }

    #endregion Types
}
