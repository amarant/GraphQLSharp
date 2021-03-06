﻿using System;
using System.Collections.Immutable;

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

        public Location(int start, int end, Source source = null)
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
        ListValue,
        ObjectValue,
        ObjectField,
        Directive,
        ListType,
        NonNullType,
        NamedType,
        TypeDefinition,
        InterfaceDefinition,
        FieldDefinition,
        InputValueDefinition,
        EnumDefinition,
        ScalarDefinition,
        InputObjectDefinition,
        UnionDefinition,
        EnumValueDefinition,
        InputFieldDefinition,
        SchemaDocument
    }

    public interface INode
    {
        NodeType Kind { get; }
        TResult Accept<TResult>(Visitor<TResult> visitor);
    }

    public abstract class ANode : INode
    {
        public abstract NodeType Kind { get; }
        public Location Location { get; set; }

        public abstract TResult Accept<TResult>(Visitor<TResult> visitor);
    }

    #region Name

    public class Name : ANode
    {
        public Name()
        {
        }

        public Name(String value, Location location)
        {
            Value = value;
            Location = location;
        }

        public override NodeType Kind => NodeType.Name;
        public String Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitName(this);
        }

        public Name Update(String value)
        {
            if (value != Value)
            {
                return new Name
                {
                    Value = value,
                };
            }
            return this;
        }
    }

    #endregion Name

    #region Document

    public class Document : ANode
    {
        public override NodeType Kind => NodeType.Document;
        public ImmutableArray<IDefinition> Definitions { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitDocument(this);
        }

        public Document Update(ImmutableArray<IDefinition> definitions)
        {
            if (Definitions != definitions)
            {
                return new Document
                {
                    Definitions = definitions,
                };
            }
            return this;
        }
    }

    public interface IDefinition : INode
    {
    }

    public enum OperationType
    {
        Query,
        Mutation,
    }

    public class OperationDefinition : ANode, IDefinition
    {
        public override NodeType Kind => NodeType.OperationDefinition;
        public OperationType Operation { get; set; }
        public Name Name { get; set; }
        public ImmutableArray<VariableDefinition> VariableDefinitions { get; set; }
        public ImmutableArray<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitOperationDefinition(this);
        }

        public OperationDefinition Update(OperationType operation, Name name, ImmutableArray<VariableDefinition> variableDefinitions, ImmutableArray<Directive> directives, SelectionSet selectionSet)
        {
            if (Operation != operation ||
                Name != name ||
                VariableDefinitions != variableDefinitions ||
                Directives != directives ||
                SelectionSet != selectionSet)
            {
                return new OperationDefinition
                {
                    Operation = operation,
                    Name = name,
                    VariableDefinitions = variableDefinitions,
                    Directives = directives,
                    SelectionSet = selectionSet,
                };
            }
            return this;
        }
    }

    public class VariableDefinition : ANode
    {
        public override NodeType Kind => NodeType.VariableDefinition;
        public Variable Variable { get; set; }
        public IType Type { get; set; }
        public IValue DefaultValue { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitVariableDefinition(this);
        }

        public VariableDefinition Update(Variable variable, IType type, IValue defaultValue)
        {
            if (Variable != variable ||
                Type != type ||
                DefaultValue != defaultValue)
            {
                return new VariableDefinition
                {
                    Variable = variable,
                    Type = type,
                    DefaultValue = defaultValue,
                };
            }
            return this;
        }
    }

    public class Variable : ANode, IValue
    {
        public override NodeType Kind => NodeType.Variable;
        public Name Name { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitVariable(this);
        }

        public Variable Update(Name name)
        {
            if (Name != name)
            {
                return new Variable
                {
                    Name = name,
                };
            }
            return this;
        }
    }

    public class SelectionSet : ANode
    {
        public override NodeType Kind => NodeType.SelectionSet;
        public ImmutableArray<ISelection> Selections { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitSelectionSet(this);
        }

        public SelectionSet Update(ImmutableArray<ISelection> selections)
        {
            if (Selections != selections)
            {
                return new SelectionSet
                {
                    Selections = selections,
                };
            }
            return this;
        }
    }

    public interface ISelection : INode
    {
    }

    public class Field : ANode, ISelection
    {
        public override NodeType Kind => NodeType.Field;
        public Name Alias { get; set; }
        public Name Name { get; set; }
        public ImmutableArray<Argument> Arguments { get; set; }
        public ImmutableArray<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitField(this);
        }

        public Field Update(Name @alias, Name name, ImmutableArray<Argument> arguments, ImmutableArray<Directive> directives, SelectionSet selectionSet)
        {
            if (Alias != alias ||
                Name != name ||
                Arguments != arguments ||
                Directives != directives ||
                SelectionSet != selectionSet)
            {
                return new Field
                {
                    Alias = alias,
                    Name = name,
                    Arguments = arguments,
                    Directives = directives,
                    SelectionSet = selectionSet,
                };
            }
            return this;
        }
    }

    public class Argument : ANode
    {
        public override NodeType Kind => NodeType.Argument;
        public Name Name { get; set; }
        public IValue Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitArgument(this);
        }

        public Argument Update(Name name, IValue value)
        {
            if (Name != name ||
                Value != value)
            {
                return new Argument
                {
                    Name = name,
                    Value = value,
                };
            }
            return this;
        }

    }

    #endregion Document

    #region Fragments

    public class FragmentSpread : ANode, ISelection
    {
        public override NodeType Kind => NodeType.FragmentSpread;
        public Name Name { get; set; }
        public ImmutableArray<Directive> Directives { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitFragmentSpread(this);
        }

        public FragmentSpread Update(Name name, ImmutableArray<Directive> directives)
        {
            if (Name != name ||
                Directives != directives)
            {
                return new FragmentSpread
                {
                    Name = name,
                    Directives = directives,
                };
            }

            return this;
        }
    }

    public class InlineFragment : ANode, ISelection
    {
        public override NodeType Kind => NodeType.InlineFragment;
        public NamedType TypeCondition { get; set; }
        public ImmutableArray<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInlineFragment(this);
        }

        public InlineFragment Update(NamedType typeCondition, ImmutableArray<Directive> directives,
            SelectionSet selectionSet)
        {
            if (TypeCondition != typeCondition ||
                Directives != directives ||
                SelectionSet != selectionSet)
            {
                return new InlineFragment
                {
                    TypeCondition = typeCondition,
                    Directives = directives,
                    SelectionSet = selectionSet,
                };
            }
            return this;
        }
    }

    public class FragmentDefinition : ANode, IDefinition
    {
        public override NodeType Kind => NodeType.FragmentDefinition;
        public Name Name { get; set; }
        public NamedType TypeCondition { get; set; }
        public ImmutableArray<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitFragmentDefinition(this);
        }

        public FragmentDefinition Update(Name name, NamedType typeCondition, 
            ImmutableArray<Directive> directives, SelectionSet selectionSet)
        {
            if (Name != name ||
                TypeCondition != typeCondition ||
                Directives != directives ||
                SelectionSet != selectionSet)
            {
                return new FragmentDefinition
                {
                    Name = name,
                    TypeCondition = typeCondition,
                    Directives = directives,
                    SelectionSet = selectionSet,
                };
            }
            return this;
        }
    }

    #endregion Fragments

    #region Values

    public interface IValue : INode
    {
    }

    public class IntValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.IntValue;
        public String Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitIntValue(this);
        }
    }

    public class FloatValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.FloatValue;
        public String Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitFloatValue(this);
        }
    }

    public class StringValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.StringValue;
        public String Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitStringValue(this);
        }
    }

    public class BooleanValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.BooleanValue;
        public Boolean Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitBooleanValue(this);
        }
    }

    public class EnumValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.EnumValue;
        public String Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitEnumValue(this);
        }
    }

    public class ListValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.ListValue;
        public ImmutableArray<IValue> Values { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitListValue(this);
        }

        public ListValue Update(ImmutableArray<IValue> values)
        {
            if (Values != values)
            {
                return new ListValue
                {
                    Values = values,
                };
            }
            return this;
        }
    }

    public class ObjectValue : ANode, IValue
    {
        public override NodeType Kind => NodeType.ObjectValue;
        public ImmutableArray<ObjectField> Fields { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitObjectValue(this);
        }

        public ObjectValue Update(ImmutableArray<ObjectField> fields)
        {
            if (Fields != fields)
            {
                return new ObjectValue
                {
                    Fields = fields,
                };
            }
            return this;
        }
    }

    public class ObjectField : ANode
    {
        public override NodeType Kind => NodeType.ObjectField;
        public Name Name { get; set; }
        public IValue Value { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitObjectField(this);
        }

        public ObjectField Update(Name name, IValue value)
        {
            if (Name != name ||
                Value != value)
            {
                return new ObjectField
                {
                    Name = name,
                    Value = value,
                };
            }
            return this;
        }
    }

    #endregion Values

    #region Directives

    public class Directive : ANode
    {
        public override NodeType Kind => NodeType.Directive;
        public Name Name { get; set; }
        public ImmutableArray<Argument> Arguments { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitDirective(this);
        }

        public Directive Update(Name name, ImmutableArray<Argument> arguments)
        {
            if (Name != name ||
                Arguments != arguments)
            {
                return new Directive
                {
                    Name = name,
                    Arguments = arguments,
                };
            }
            return this;
        }
    }

    #endregion Directives

    #region Types

    public interface IType : INode
    {
    }

    public class NamedType : ANode, INameOrListType
    {
        public NamedType()
        {
        }

        public NamedType(String value, int start, int end, Source source = null)
        {
            Location = new Location(start, end, source);
            Name = new Name(value, Location);
        }

        public override NodeType Kind => NodeType.NamedType;
        public Name Name { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitNamedType(this);
        }

        public NamedType Update(Name name)
        {
            if (Name != name)
            {
                return new NamedType
                {
                    Name = name,
                };
            }
            return this;
        }
    }

    public interface INameOrListType : IType
    {
    }

    public class ListType : ANode, INameOrListType
    {
        public override NodeType Kind => NodeType.ListType;
        public IType Type { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return visitor.VisitListType(this);
        }

        public ListType Update(IType type)
        {
            if (Type != type)
            {
                return new ListType
                {
                    Type = type,
                };
            }
            return this;
        }
    }

    public class NonNullType : ANode, IType
    {
        public override NodeType Kind => NodeType.NonNullType;
        public INameOrListType Type { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
           return  visitor.VisitNonNullType(this);
        }

        public NonNullType Update(INameOrListType type)
        {
            if (Type != type)
            {
                return new NonNullType
                {
                    Type = type,
                };
            }
            return this;
        }
    }

    #endregion Types
}
