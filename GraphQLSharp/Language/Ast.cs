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
        VisitAction Accept(Visitor visitor);
    }

    public abstract class ANode : INode
    {
        public abstract NodeType Kind { get; }
        public Location Location { get; set; }

        public abstract VisitAction Accept(Visitor visitor);

        protected VisitAction ReturnAction(VisitAction enterAction)
        {
            switch (enterAction.VisitActionType)
            {
                case VisitActionType.Skip:
                    return VisitAction.NoAction;
                case VisitActionType.Break:
                    return VisitAction.Break;
                case VisitActionType.Replace:
                    return enterAction;
                    //return new VisitAction(
                    //    VisitActionType.Replace,
                    //    enterAction.ReplaceNode);
            }
            throw new Exception("not possible");
        }
    }

    #region Name

    public class Name : ANode, IType, INameOrListType
    {
        public override NodeType Kind
        {
            get { return NodeType.Name; }
        }
        public String Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            var enterAction = visitor.EnterName(this);
            if (enterAction.VisitActionType != VisitActionType.NoAction)
            {
                return ReturnAction(enterAction);
            }
            var leaveAction = visitor.LeaveName(this);
            if (leaveAction.VisitActionType != VisitActionType.NoAction)
            {
                return ReturnAction(leaveAction);
            }
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            var enterAction = visitor.EnterDocument(this);
            if (enterAction.VisitActionType != VisitActionType.NoAction)
            {
                return ReturnAction(enterAction);
            }
            if (Definitions != null)
                foreach (var definition in Definitions)
                {
                    var definitionAction = definition.Accept(visitor);
                    switch (definitionAction.VisitActionType)
                    {
                        case VisitActionType.Break:
                            return VisitAction.Break;
                        case VisitActionType.Replace:
                            return enterAction;
                        //return new VisitAction(
                        //    VisitActionType.Replace,
                        //    enterAction.ReplaceNode);
                    }
                }
            var leaveAction = visitor.LeaveDocument(this);
            if (leaveAction.VisitActionType != VisitActionType.NoAction)
            {
                return ReturnAction(leaveAction);
            }
            return VisitAction.NoAction;
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
        public override NodeType Kind
        {
            get { return NodeType.OperationDefinition; }
        }

        public OperationType Operation { get; set; }
        public Name Name { get; set; }
        public List<VariableDefinition> VariableDefinitions { get; set; }
        public List<Directive> Directives { get; set; }
        public SelectionSet SelectionSet { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterOperationDefinition(this);
            if (Name != null) Name.Accept(visitor);
            if (VariableDefinitions != null)
                foreach (var definition in VariableDefinitions)
                {
                    definition.Accept(visitor);
                }
            if (Directives != null)
                foreach (var directive in Directives)
                {
                    directive.Accept(visitor);
                }
            if (SelectionSet != null) SelectionSet.Accept(visitor);
            visitor.LeaveOperationDefinition(this);
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterVariableDefinition(this);
            if (Variable != null) Variable.Accept(visitor);
            if (Type != null) Type.Accept(visitor);
            if (DefaultValue != null) DefaultValue.Accept(visitor);
            visitor.LeaveVariableDefinition(this);
            return VisitAction.NoAction;
        }
    }

    public class Variable : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.Variable; }
        }

        public Name Name { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterVariable(this);
            if (Name != null) Name.Accept(visitor);
            visitor.LeaveVariable(this);
            return VisitAction.NoAction;
        }
    }

    public class SelectionSet : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.SelectionSet; }
        }

        public List<ISelection> Selections { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterSelectionSet(this);
            if (Selections != null)
                foreach (var selection in Selections)
                {
                    selection.Accept(visitor);
                }
            visitor.LeaveSelectionSet(this);
            return VisitAction.NoAction;
        }
    }

    public interface ISelection : INode
    {
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterField(this);
            if (Alias != null) Alias.Accept(visitor);
            if (Name != null) Name.Accept(visitor);
            if (Arguments != null)
                foreach (var argument in Arguments)
                {
                    argument.Accept(visitor);
                }
            if (Directives != null)
                foreach (var directive in Directives)
                {
                    directive.Accept(visitor);
                }
            if (SelectionSet != null) SelectionSet.Accept(visitor);
            visitor.LeaveField(this);
            return VisitAction.NoAction;
        }
    }

    public class Argument : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.Argument; }
        }

        public Name Name { get; set; }
        public IValue Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterArgument(this);
            if (Name != null) Name.Accept(visitor);
            if (Value != null) Value.Accept(visitor);
            visitor.LeaveArgument(this);
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterFragmentSpread(this);
            if (Name != null) Name.Accept(visitor);
            if (Directives != null)
                foreach (var directive in Directives)
                {
                    directive.Accept(visitor);
                }
            visitor.LeaveFragmentSpread(this);
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterInlineFragment(this);
            if (TypeCondition != null) TypeCondition.Accept(visitor);
            if (Directives != null)
                foreach (var directive in Directives)
                {
                    directive.Accept(visitor);
                }
            if (SelectionSet != null) SelectionSet.Accept(visitor);
            visitor.LeaveInlineFragment(this);
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterFragmentDefinition(this);
            if (Name != null) Name.Accept(visitor);
            if (TypeCondition != null) TypeCondition.Accept(visitor);
            if (Directives != null)
                foreach (var directive in Directives)
                {
                    directive.Accept(visitor);
                }
            if (SelectionSet != null) SelectionSet.Accept(visitor);
            visitor.LeaveFragmentDefinition(this);
            return VisitAction.NoAction;
        }
    }

    #endregion Fragments

    #region Values

    public interface IValue : INode
    {
    }

    public class IntValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.IntValue; }
        }

        public String Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterIntValue(this);
            visitor.LeaveIntValue(this);
            return VisitAction.NoAction;
        }
    }

    public class FloatValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.FloatValue; }
        }

        public String Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterFloatValue(this);
            visitor.LeaveFloatValue(this);
            return VisitAction.NoAction;
        }
    }

    public class StringValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.StringValue; }
        }

        public String Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterStringValue(this);
            visitor.LeaveStringValue(this);
            return VisitAction.NoAction;
        }
    }

    public class BooleanValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.BooleanValue; }
        }

        public Boolean Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterBooleanValue(this);
            visitor.LeaveBooleanValue(this);
            return VisitAction.NoAction;
        }
    }

    public class EnumValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.EnumValue; }
        }

        public String Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterEnumValue(this);
            visitor.LeaveEnumValue(this);
            return VisitAction.NoAction;
        }
    }

    public class ArrayValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.ArrayValue; }
        }

        public List<IValue> Values { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterArrayValue(this);
            if (Values != null)
                foreach (var value in Values)
                {
                    value.Accept(visitor);
                }
            visitor.LeaveArrayValue(this);
            return VisitAction.NoAction;
        }
    }

    public class ObjectValue : ANode, IValue
    {
        public override NodeType Kind
        {
            get { return NodeType.IntValue; }
        }

        public List<ObjectField> Fields { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterObjectValue(this);
            if (Fields != null)
                foreach (var field in Fields)
                {
                    field.Accept(visitor);
                }
            visitor.LeaveObjectValue(this);
            return VisitAction.NoAction;
        }
    }

    public class ObjectField : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.ObjectField; }
        }
        public Name Name { get; set; }
        public IValue Value { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterObjectField(this);
            if (Name != null) Name.Accept(visitor);
            if (Value != null) Value.Accept(visitor);
            visitor.LeaveObjectField(this);
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterDirective(this);
            if (Name != null) Name.Accept(visitor);
            if (Value != null) Value.Accept(visitor);
            visitor.LeaveDirective(this);
            return VisitAction.NoAction;
        }
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

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterListType(this);
            if (Type != null) Type.Accept(visitor);
            visitor.LeaveListType(this);
            return VisitAction.NoAction;
        }
    }

    public class NonNullType : ANode, IType
    {
        public override NodeType Kind
        {
            get { return NodeType.NonNullType; }
        }

        public INameOrListType Type { get; set; }

        public override VisitAction Accept(Visitor visitor)
        {
            visitor.EnterNonNullType(this);
            if (Type != null) Type.Accept(visitor);
            visitor.LeaveNonNullType(this);
            return VisitAction.NoAction;
        }
    }

    #endregion Types
}
