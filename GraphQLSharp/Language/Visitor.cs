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
        Delete,
        Replace
    }

    public class VisitAction
    {
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
        public static readonly VisitAction NoAction = new VisitAction(VisitActionType.NoAction);

        public virtual VisitAction EnterName(Name name)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveName(Name name)
        {
            return NoAction;
        }

        public virtual VisitAction EnterDocument(Document document)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveDocument(Document document)
        {
            return NoAction;
        }

        public virtual VisitAction EnterOperationDefinition(OperationDefinition operationDefinition)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveOperationDefinition(OperationDefinition operationDefinition)
        {
            return NoAction;
        }

        public virtual VisitAction EnterVariableDefinition(VariableDefinition variableDefinition)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveVariableDefinition(VariableDefinition variableDefinition)
        {
            return NoAction;
        }

        public virtual VisitAction EnterVariable(Variable variable)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveVariable(Variable variable)
        {
            return NoAction;
        }

        public virtual VisitAction EnterSelectionSet(SelectionSet selectionSet)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveSelectionSet(SelectionSet selectionSet)
        {
            return NoAction;
        }

        public virtual VisitAction EnterField(Field field)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveField(Field field)
        {
            return NoAction;
        }

        public virtual VisitAction EnterArgument(Argument argument)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveArgument(Argument argument)
        {
            return NoAction;
        }

        public virtual VisitAction EnterFragmentSpread(FragmentSpread fragmentSpread)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveFragmentSpread(FragmentSpread fragmentSpread)
        {
            return NoAction;
        }

        public virtual VisitAction EnterInlineFragment(InlineFragment inlineFragment)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveInlineFragment(InlineFragment inlineFragment)
        {
            return NoAction;
        }

        public virtual VisitAction EnterFragmentDefinition(FragmentDefinition fragmentDefinition)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveFragmentDefinition(FragmentDefinition fragmentDefinition)
        {
            return NoAction;
        }

        public virtual VisitAction EnterIntValue(IntValue intValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveIntValue(IntValue intValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterFloatValue(FloatValue floatValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveFloatValue(FloatValue floatValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterStringValue(StringValue stringValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveStringValue(StringValue stringValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterBooleanValue(BooleanValue booleanValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveBooleanValue(BooleanValue booleanValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterEnumValue(EnumValue enumValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveEnumValue(EnumValue enumValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterArrayValue(ArrayValue arrayValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveArrayValue(ArrayValue arrayValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterObjectValue(ObjectValue objectValue)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveObjectValue(ObjectValue objectValue)
        {
            return NoAction;
        }

        public virtual VisitAction EnterObjectField(ObjectField objectField)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveObjectField(ObjectField objectField)
        {
            return NoAction;
        }

        public virtual VisitAction EnterDirective(Directive directive)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveDirective(Directive directive)
        {
            return NoAction;
        }

        public virtual VisitAction EnterListType(ListType listType)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveListType(ListType listType)
        {
            return NoAction;
        }

        public virtual VisitAction EnterNonNullType(NonNullType nonNullType)
        {
            return NoAction;
        }

        public virtual VisitAction LeaveNonNullType(NonNullType nonNullType)
        {
            return NoAction;
        }
    }

    public abstract class GenericVisitor : Visitor
    {
        public abstract VisitAction Enter(INode node);
        public abstract VisitAction Leave(INode node);

        public override VisitAction EnterName(Name name)
        {
            return Enter(name);
        }

        public override VisitAction LeaveName(Name name)
        {
            return Leave(name);
        }

        public override VisitAction EnterDocument(Document document)
        {
            return Enter(document);
        }

        public override VisitAction LeaveDocument(Document document)
        {
            return Leave(document);
        }

        public override VisitAction EnterOperationDefinition(OperationDefinition operationDefinition)
        {
            return Enter(operationDefinition);
        }

        public override VisitAction LeaveOperationDefinition(OperationDefinition operationDefinition)
        {
            return Leave(operationDefinition);
        }

        public override VisitAction EnterVariableDefinition(VariableDefinition variableDefinition)
        {
            return Enter(variableDefinition);
        }

        public override VisitAction LeaveVariableDefinition(VariableDefinition variableDefinition)
        {
            return Leave(variableDefinition);
        }

        public override VisitAction EnterVariable(Variable variable)
        {
            return Enter(variable);
        }

        public override VisitAction LeaveVariable(Variable variable)
        {
            return Leave(variable);
        }

        public override VisitAction EnterSelectionSet(SelectionSet selectionSet)
        {
            return Enter(selectionSet);
        }

        public override VisitAction LeaveSelectionSet(SelectionSet selectionSet)
        {
            return Leave(selectionSet);
        }

        public override VisitAction EnterField(Field field)
        {
            return Enter(field);
        }

        public override VisitAction LeaveField(Field field)
        {
            return Leave(field);
        }

        public override VisitAction EnterArgument(Argument argument)
        {
            return Enter(argument);
        }

        public override VisitAction LeaveArgument(Argument argument)
        {
            return Leave(argument);
        }

        public override VisitAction EnterFragmentSpread(FragmentSpread fragmentSpread)
        {
            return Enter(fragmentSpread);
        }

        public override VisitAction LeaveFragmentSpread(FragmentSpread fragmentSpread)
        {
            return Leave(fragmentSpread);
        }

        public override VisitAction EnterInlineFragment(InlineFragment inlineFragment)
        {
            return Enter(inlineFragment);
        }

        public override VisitAction LeaveInlineFragment(InlineFragment inlineFragment)
        {
            return Leave(inlineFragment);
        }

        public override VisitAction EnterFragmentDefinition(FragmentDefinition fragmentDefinition)
        {
            return Enter(fragmentDefinition);
        }

        public override VisitAction LeaveFragmentDefinition(FragmentDefinition fragmentDefinition)
        {
            return Leave(fragmentDefinition);
        }

        public override VisitAction EnterIntValue(IntValue intValue)
        {
            return Enter(intValue);
        }

        public override VisitAction LeaveIntValue(IntValue intValue)
        {
            return Leave(intValue);
        }

        public override VisitAction EnterFloatValue(FloatValue floatValue)
        {
            return Enter(floatValue);
        }

        public override VisitAction LeaveFloatValue(FloatValue floatValue)
        {
            return Leave(floatValue);
        }

        public override VisitAction EnterStringValue(StringValue stringValue)
        {
            return Enter(stringValue);
        }

        public override VisitAction LeaveStringValue(StringValue stringValue)
        {
            return Leave(stringValue);
        }

        public override VisitAction EnterBooleanValue(BooleanValue booleanValue)
        {
            return Enter(booleanValue);
        }

        public override VisitAction LeaveBooleanValue(BooleanValue booleanValue)
        {
            return Leave(booleanValue);
        }

        public override VisitAction EnterEnumValue(EnumValue enumValue)
        {
            return Enter(enumValue);
        }

        public override VisitAction LeaveEnumValue(EnumValue enumValue)
        {
            return Leave(enumValue);
        }

        public override VisitAction EnterArrayValue(ArrayValue arrayValue)
        {
            return Enter(arrayValue);
        }

        public override VisitAction LeaveArrayValue(ArrayValue arrayValue)
        {
            return Leave(arrayValue);
        }

        public override VisitAction EnterObjectValue(ObjectValue objectValue)
        {
            return Enter(objectValue);
        }

        public override VisitAction LeaveObjectValue(ObjectValue objectValue)
        {
            return Leave(objectValue);
        }

        public override VisitAction EnterObjectField(ObjectField objectField)
        {
            return Enter(objectField);
        }

        public override VisitAction LeaveObjectField(ObjectField objectField)
        {
            return Leave(objectField);
        }

        public override VisitAction EnterDirective(Directive directive)
        {
            return Enter(directive);
        }

        public override VisitAction LeaveDirective(Directive directive)
        {
            return Leave(directive);
        }

        public override VisitAction EnterListType(ListType listType)
        {
            return Enter(listType);
        }

        public override VisitAction LeaveListType(ListType listType)
        {
            return Leave(listType);
        }

        public override VisitAction EnterNonNullType(NonNullType nonNullType)
        {
            return Enter(nonNullType);
        }

        public override VisitAction LeaveNonNullType(NonNullType nonNullType)
        {
            return Leave(nonNullType);
        }
    }
}
