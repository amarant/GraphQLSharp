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
        public virtual VisitAction Enter(Name name)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(Name name)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(Document document)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(Document document)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(OperationDefinition operationDefinition)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(OperationDefinition operationDefinition)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(VariableDefinition variableDefinition)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(VariableDefinition variableDefinition)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(Variable variable)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(Variable variable)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(SelectionSet selectionSet)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(SelectionSet selectionSet)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(Field field)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(Field field)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(Argument argument)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(Argument argument)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(FragmentSpread fragmentSpread)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(FragmentSpread fragmentSpread)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(InlineFragment inlineFragment)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(InlineFragment inlineFragment)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(FragmentDefinition fragmentDefinition)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(FragmentDefinition fragmentDefinition)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(IntValue intValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(IntValue intValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(FloatValue floatValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(FloatValue floatValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(StringValue stringValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(StringValue stringValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(BooleanValue booleanValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(BooleanValue booleanValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(EnumValue enumValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(EnumValue enumValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(ArrayValue arrayValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(ArrayValue arrayValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(ObjectValue objectValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(ObjectValue objectValue)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(ObjectField objectField)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(ObjectField objectField)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(Directive directive)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(Directive directive)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(ListType listType)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(ListType listType)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Enter(NonNullType nonNullType)
        {
            return VisitAction.NoAction;
        }

        public virtual VisitAction Leave(NonNullType nonNullType)
        {
            return VisitAction.NoAction;
        }
    }

    public abstract class GenericVisitor : Visitor
    {
        public abstract VisitAction Enter(INode node);
        public abstract VisitAction Leave(INode node);

        public override VisitAction Enter(Name name)
        {
            return Enter(name);
        }

        public override VisitAction Leave(Name name)
        {
            return Leave(name);
        }

        public override VisitAction Enter(Document document)
        {
            return Enter(document);
        }

        public override VisitAction Leave(Document document)
        {
            return Leave(document);
        }

        public override VisitAction Enter(OperationDefinition operationDefinition)
        {
            return Enter(operationDefinition);
        }

        public override VisitAction Leave(OperationDefinition operationDefinition)
        {
            return Leave(operationDefinition);
        }

        public override VisitAction Enter(VariableDefinition variableDefinition)
        {
            return Enter(variableDefinition);
        }

        public override VisitAction Leave(VariableDefinition variableDefinition)
        {
            return Leave(variableDefinition);
        }

        public override VisitAction Enter(Variable variable)
        {
            return Enter(variable);
        }

        public override VisitAction Leave(Variable variable)
        {
            return Leave(variable);
        }

        public override VisitAction Enter(SelectionSet selectionSet)
        {
            return Enter(selectionSet);
        }

        public override VisitAction Leave(SelectionSet selectionSet)
        {
            return Leave(selectionSet);
        }

        public override VisitAction Enter(Field field)
        {
            return Enter(field);
        }

        public override VisitAction Leave(Field field)
        {
            return Leave(field);
        }

        public override VisitAction Enter(Argument argument)
        {
            return Enter(argument);
        }

        public override VisitAction Leave(Argument argument)
        {
            return Leave(argument);
        }

        public override VisitAction Enter(FragmentSpread fragmentSpread)
        {
            return Enter(fragmentSpread);
        }

        public override VisitAction Leave(FragmentSpread fragmentSpread)
        {
            return Leave(fragmentSpread);
        }

        public override VisitAction Enter(InlineFragment inlineFragment)
        {
            return Enter(inlineFragment);
        }

        public override VisitAction Leave(InlineFragment inlineFragment)
        {
            return Leave(inlineFragment);
        }

        public override VisitAction Enter(FragmentDefinition fragmentDefinition)
        {
            return Enter(fragmentDefinition);
        }

        public override VisitAction Leave(FragmentDefinition fragmentDefinition)
        {
            return Leave(fragmentDefinition);
        }

        public override VisitAction Enter(IntValue intValue)
        {
            return Enter(intValue);
        }

        public override VisitAction Leave(IntValue intValue)
        {
            return Leave(intValue);
        }

        public override VisitAction Enter(FloatValue floatValue)
        {
            return Enter(floatValue);
        }

        public override VisitAction Leave(FloatValue floatValue)
        {
            return Leave(floatValue);
        }

        public override VisitAction Enter(StringValue stringValue)
        {
            return Enter(stringValue);
        }

        public override VisitAction Leave(StringValue stringValue)
        {
            return Leave(stringValue);
        }

        public override VisitAction Enter(BooleanValue booleanValue)
        {
            return Enter(booleanValue);
        }

        public override VisitAction Leave(BooleanValue booleanValue)
        {
            return Leave(booleanValue);
        }

        public override VisitAction Enter(EnumValue enumValue)
        {
            return Enter(enumValue);
        }

        public override VisitAction Leave(EnumValue enumValue)
        {
            return Leave(enumValue);
        }

        public override VisitAction Enter(ArrayValue arrayValue)
        {
            return Enter(arrayValue);
        }

        public override VisitAction Leave(ArrayValue arrayValue)
        {
            return Leave(arrayValue);
        }

        public override VisitAction Enter(ObjectValue objectValue)
        {
            return Enter(objectValue);
        }

        public override VisitAction Leave(ObjectValue objectValue)
        {
            return Leave(objectValue);
        }

        public override VisitAction Enter(ObjectField objectField)
        {
            return Enter(objectField);
        }

        public override VisitAction Leave(ObjectField objectField)
        {
            return Leave(objectField);
        }

        public override VisitAction Enter(Directive directive)
        {
            return Enter(directive);
        }

        public override VisitAction Leave(Directive directive)
        {
            return Leave(directive);
        }

        public override VisitAction Enter(ListType listType)
        {
            return Enter(listType);
        }

        public override VisitAction Leave(ListType listType)
        {
            return Leave(listType);
        }

        public override VisitAction Enter(NonNullType nonNullType)
        {
            return Enter(nonNullType);
        }

        public override VisitAction Leave(NonNullType nonNullType)
        {
            return Leave(nonNullType);
        }
    }
}
