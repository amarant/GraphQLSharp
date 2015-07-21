using System;
using System.Collections.Immutable;
using System.Diagnostics;
using GraphQLSharp.ImmutableUtils;
using GraphQLSharp.Language.Schema;

namespace GraphQLSharp.Language
{
    public abstract class Visitor<TResult>
    {
        public class BreakException : Exception
        {
        }

        public virtual TResult Visit(INode node)
        {
            return node != null ? node.Accept(this) : default(TResult);
        }

        public virtual TResult VisitWithBreak(INode node)
        {
            try
            {
                return Visit(node);
            }
            catch (BreakException)
            {
                return default(TResult);
            }
        }

        public virtual TResult DefaultVisit(INode node)
        {
            return default(TResult);
        }

        public virtual TResult VisitName(Name node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitDocument(Document node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitOperationDefinition(OperationDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitVariableDefinition(VariableDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitVariable(Variable node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitSelectionSet(SelectionSet node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitField(Field node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitArgument(Argument node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitFragmentSpread(FragmentSpread node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitInlineFragment(InlineFragment node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitFragmentDefinition(FragmentDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitIntValue(IntValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitFloatValue(FloatValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitStringValue(StringValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitBooleanValue(BooleanValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitEnumValue(EnumValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitArrayValue(ArrayValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitObjectValue(ObjectValue node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitObjectField(ObjectField node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitDirective(Directive node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitListType(ListType node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitNonNullType(NonNullType node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitNamedType(NamedType node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitTypeDefinition(TypeDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitInterfaceDefinition(InterfaceDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitFieldDefinition(FieldDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitArgumentDefinition(ArgumentDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitEnumDefinition(EnumDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitScalarDefinition(ScalarDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitInputObjectDefinition(InputObjectDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitSchemaDocument(SchemaDocument node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitUnionDefinition(UnionDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual TResult VisitEnumValueDefinition(EnumValueDefinition node)
        {
            return DefaultVisit(node);
        }

        public virtual  TResult VisitInputFieldDefinition(InputFieldDefinition node)
        {
            return DefaultVisit(node);
        }
    }

    public class RoundTripWalker : Visitor<INode>
    {
        public void VisitList<T>(ImmutableArray<T> list) where T : INode
        {
            if (!list.IsDefault)
            {
                for (var i = 0; i < list.Length; i++)
                {
                    Visit(list[i]);
                }
            }
        }

        public override INode VisitName(Name node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitDocument(Document node)
        {
            VisitList(node.Definitions);
            return DefaultVisit(node);
        }

        public override INode VisitOperationDefinition(OperationDefinition node)
        {
            VisitList(node.VariableDefinitions);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitVariableDefinition(VariableDefinition node)
        {
            Visit(node.Variable);
            Visit(node.Type);
            Visit(node.DefaultValue);
            return DefaultVisit(node);
        }

        public override INode VisitVariable(Variable node)
        {
            Visit(node.Name);
            return DefaultVisit(node);
        }

        public override INode VisitSelectionSet(SelectionSet node)
        {
            VisitList(node.Selections);
            return DefaultVisit(node);
        }

        public override INode VisitField(Field node)
        {
            Visit(node.Alias);
            Visit(node.Name);
            VisitList(node.Arguments);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitArgument(Argument node)
        {
            Visit(node.Name);
            Visit(node.Value);
            return DefaultVisit(node);
        }

        public override INode VisitFragmentSpread(FragmentSpread node)
        {
            Visit(node.Name);
            VisitList(node.Directives);
            return DefaultVisit(node);
        }

        public override INode VisitInlineFragment(InlineFragment node)
        {
            Visit(node.TypeCondition);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitFragmentDefinition(FragmentDefinition node)
        {
            Visit(node.Name);
            Visit(node.TypeCondition);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitIntValue(IntValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitFloatValue(FloatValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitStringValue(StringValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitBooleanValue(BooleanValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitEnumValue(EnumValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitArrayValue(ArrayValue node)
        {
            VisitList(node.Values);
            return DefaultVisit(node);
        }

        public override INode VisitObjectValue(ObjectValue node)
        {
            VisitList(node.Fields);
            return DefaultVisit(node);
        }

        public override INode VisitObjectField(ObjectField node)
        {
            Visit(node.Name);
            Visit(node.Value);
            return DefaultVisit(node);
        }

        public override INode VisitDirective(Directive node)
        {
            Visit(node.Name);
            VisitList(node.Arguments);
            return DefaultVisit(node);
        }

        public override INode VisitListType(ListType node)
        {
            Visit(node.Type);
            return DefaultVisit(node);
        }

        public override INode VisitNonNullType(NonNullType node)
        {
            Visit(node.Type);
            return DefaultVisit(node);
        }

        public override INode VisitNamedType(NamedType node)
        {
            VisitName(node.Name);
            return DefaultVisit(node);
        }
    }

    public class Walker : Visitor<INode>
    {
        public void VisitList<T>(ImmutableArray<T> list) where T : INode
        {
            if (!list.IsDefault)
            {
                for (var i = 0; i < list.Length; i++)
                {
                    Visit(list[i]);
                }
            }
        }

        public override INode VisitName(Name node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitDocument(Document node)
        {
            VisitList(node.Definitions);
            return DefaultVisit(node);
        }

        public override INode VisitOperationDefinition(OperationDefinition node)
        {
            VisitList(node.VariableDefinitions);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitVariableDefinition(VariableDefinition node)
        {
            Visit(node.Variable);
            Visit(node.Type);
            Visit(node.DefaultValue);
            return DefaultVisit(node);
        }

        public override INode VisitVariable(Variable node)
        {
            Visit(node.Name);
            return DefaultVisit(node);
        }

        public override INode VisitSelectionSet(SelectionSet node)
        {
            VisitList(node.Selections);
            return DefaultVisit(node);
        }

        public override INode VisitField(Field node)
        {
            Visit(node.Alias);
            Visit(node.Name);
            VisitList(node.Arguments);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitArgument(Argument node)
        {
            Visit(node.Name);
            Visit(node.Value);
            return DefaultVisit(node);
        }

        public override INode VisitFragmentSpread(FragmentSpread node)
        {
            Visit(node.Name);
            VisitList(node.Directives);
            return DefaultVisit(node);
        }

        public override INode VisitInlineFragment(InlineFragment node)
        {
            Visit(node.TypeCondition);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitFragmentDefinition(FragmentDefinition node)
        {
            Visit(node.Name);
            Visit(node.TypeCondition);
            VisitList(node.Directives);
            Visit(node.SelectionSet);
            return DefaultVisit(node);
        }

        public override INode VisitIntValue(IntValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitFloatValue(FloatValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitStringValue(StringValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitBooleanValue(BooleanValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitEnumValue(EnumValue node)
        {
            return DefaultVisit(node);
        }

        public override INode VisitArrayValue(ArrayValue node)
        {
            VisitList(node.Values);
            return DefaultVisit(node);
        }

        public override INode VisitObjectValue(ObjectValue node)
        {
            VisitList(node.Fields);
            return DefaultVisit(node);
        }

        public override INode VisitObjectField(ObjectField node)
        {
            Visit(node.Name);
            Visit(node.Value);
            return DefaultVisit(node);
        }

        public override INode VisitDirective(Directive node)
        {
            Visit(node.Name);
            VisitList(node.Arguments);
            return DefaultVisit(node);
        }

        public override INode VisitListType(ListType node)
        {
            Visit(node.Type);
            return DefaultVisit(node);
        }

        public override INode VisitNonNullType(NonNullType node)
        {
            Visit(node.Type);
            return DefaultVisit(node);
        }

        public override INode VisitNamedType(NamedType node)
        {
            VisitName(node.Name);
            return DefaultVisit(node);
        }
    }

    public abstract class Rewriter : Visitor<INode>
    {
        private T Visit<T>(T node) where T : class, INode
        {
            return base.Visit(node) as T;
        }

        private T Enter<T>(T node) where T : class, INode
        {
            return Enter((INode)node) as T;
        }

        private T Leave<T>(T node) where T : class, INode
        {
            return Leave((INode)node) as T;
        }

        protected virtual ImmutableArray<T> VisitList<T>(ImmutableArray<T> list) where T : class, INode
        {
            if (list.IsDefault)
            {
                return list;
            }

            ArrayBuilder<T> newList = null;
            for (var i = 0; i < list.Length; i++)
            {
                var item = list[i];
                Debug.Assert(item != null);

                var visited = Visit(item);
                if (item != visited)
                {
                    if (newList == null)
                    {
                        newList = ArrayBuilder<T>.GetInstance();
                        if (i > 0)
                        {
                            newList.AddRange(list, i);
                        }
                    }
                }

                if (newList != null && visited != null)
                {
                    newList.Add(visited);
                }
            }

            if (newList != null)
            {
                return newList.ToImmutableAndFree();
            }

            return list;
        }

        public override INode VisitName(Name node)
        {
            var updatedNode = EnterName(node);
            if (updatedNode == null) return null;
            updatedNode = LeaveName(updatedNode);
            return updatedNode;
        }

        public override INode VisitDocument(Document node)
        {
            var updatedNode = EnterDocument(node);
            if (updatedNode == null) return null;
            var definitions = VisitList(updatedNode.Definitions);
            updatedNode = updatedNode.Update(definitions);
            updatedNode = LeaveDocument(updatedNode);
            return updatedNode;
        }

        public override INode VisitOperationDefinition(OperationDefinition node)
        {
            var updatedNode = EnterOperationDefinition(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            var variableDefinitions = VisitList(updatedNode.VariableDefinitions);
            var directives = VisitList(updatedNode.Directives);
            var selectionSet = Visit(updatedNode.SelectionSet);
            updatedNode = updatedNode.Update(node.Operation, name, variableDefinitions, directives, selectionSet);
            updatedNode = LeaveOperationDefinition(updatedNode);
            return updatedNode;
        }

        public override INode VisitVariableDefinition(VariableDefinition node)
        {
            var updatedNode = EnterVariableDefinition(node);
            if (updatedNode == null) return null;
            var variable = Visit(updatedNode.Variable);
            var type = Visit(updatedNode.Type);
            var defaultValue = Visit(updatedNode.DefaultValue);
            updatedNode = updatedNode.Update(variable, type, defaultValue);
            updatedNode = LeaveVariableDefinition(updatedNode);
            return updatedNode;
        }

        public override INode VisitVariable(Variable node)
        {
            var updatedNode = EnterVariable(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            updatedNode = updatedNode.Update(name);
            updatedNode = LeaveVariable(updatedNode);
            return updatedNode;
        }

        public override INode VisitSelectionSet(SelectionSet node)
        {
            var updatedNode = EnterSelectionSet(node);
            if (updatedNode == null) return null;
            var selections = VisitList(updatedNode.Selections);
            updatedNode = updatedNode.Update(selections);
            updatedNode = LeaveSelectionSet(updatedNode);
            return updatedNode;
        }

        public override INode VisitField(Field node)
        {
            var updatedNode = EnterField(node);
            if (updatedNode == null) return null;
            var alias = Visit(updatedNode.Alias);
            var name = Visit(updatedNode.Name);
            var arguments = VisitList(updatedNode.Arguments);
            var directives = VisitList(updatedNode.Directives);
            var selectionSet = Visit(updatedNode.SelectionSet);
            updatedNode = updatedNode.Update(alias, name, arguments, directives, selectionSet);
            updatedNode = LeaveField(updatedNode);
            return updatedNode;
        }

        public override INode VisitArgument(Argument node)
        {
            var updatedNode = EnterArgument(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            var value = Visit(updatedNode.Value);
            updatedNode = updatedNode.Update(name, value);
            updatedNode = LeaveArgument(updatedNode);
            return updatedNode;
        }

        public override INode VisitFragmentSpread(FragmentSpread node)
        {
            var updatedNode = EnterFragmentSpread(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            var directives = VisitList(updatedNode.Directives);
            updatedNode = updatedNode.Update(name, directives);
            updatedNode = LeaveFragmentSpread(updatedNode);
            return updatedNode;
        }

        public override INode VisitInlineFragment(InlineFragment node)
        {
            var updatedNode = EnterInlineFragment(node);
            if (updatedNode == null) return null;
            var typeCondition = Visit(updatedNode.TypeCondition);
            var directives = VisitList(updatedNode.Directives);
            var selectionSet = Visit(updatedNode.SelectionSet);
            updatedNode = updatedNode.Update(typeCondition, directives, selectionSet);
            updatedNode = LeaveInlineFragment(updatedNode);
            return updatedNode;
        }

        public override INode VisitFragmentDefinition(FragmentDefinition node)
        {
            var updatedNode = EnterFragmentDefinition(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            var typeCondition = Visit(updatedNode.TypeCondition);
            var directives = VisitList(updatedNode.Directives);
            var selectionSet = Visit(updatedNode.SelectionSet);
            updatedNode = updatedNode.Update(name, typeCondition, directives, selectionSet);
            updatedNode = LeaveFragmentDefinition(updatedNode);
            return updatedNode;
        }

        public override INode VisitIntValue(IntValue node)
        {
            var updatedNode = EnterIntValue(node);
            if (updatedNode == null) return null;
            updatedNode = LeaveIntValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitFloatValue(FloatValue node)
        {
            var updatedNode = EnterFloatValue(node);
            if (updatedNode == null) return null;
            updatedNode = LeaveFloatValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitStringValue(StringValue node)
        {
            var updatedNode = EnterStringValue(node);
            if (updatedNode == null) return null;
            updatedNode = LeaveStringValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitBooleanValue(BooleanValue node)
        {
            var updatedNode = EnterBooleanValue(node);
            if (updatedNode == null) return null;
            updatedNode = LeaveBooleanValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitEnumValue(EnumValue node)
        {
            var updatedNode = EnterEnumValue(node);
            if (updatedNode == null) return null;
            updatedNode = LeaveEnumValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitArrayValue(ArrayValue node)
        {
            var updatedNode = EnterArrayValue(node);
            if (updatedNode == null) return null;
            var values = VisitList(updatedNode.Values);
            updatedNode = updatedNode.Update(values);
            updatedNode = LeaveArrayValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitObjectValue(ObjectValue node)
        {
            var updatedNode = EnterObjectValue(node);
            if (updatedNode == null) return null;
            var fields = VisitList(updatedNode.Fields);
            updatedNode = updatedNode.Update(fields);
            updatedNode = LeaveObjectValue(updatedNode);
            return updatedNode;
        }

        public override INode VisitObjectField(ObjectField node)
        {
            var updatedNode = EnterObjectField(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            var value = Visit(updatedNode.Value);
            updatedNode = updatedNode.Update(name, value);
            updatedNode = LeaveObjectField(updatedNode);
            return updatedNode;
        }

        public override INode VisitDirective(Directive node)
        {
            var updatedNode = EnterDirective(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            var arguments = VisitList(updatedNode.Arguments);
            updatedNode = updatedNode.Update(name, arguments);
            updatedNode = LeaveDirective(updatedNode);
            return updatedNode;
        }

        public override INode VisitListType(ListType node)
        {
            var updatedNode = EnterListType(node);
            if (updatedNode == null) return null;
            var type = Visit(updatedNode.Type);
            updatedNode = updatedNode.Update(type);
            updatedNode = LeaveListType(updatedNode);
            return updatedNode;
        }

        public override INode VisitNonNullType(NonNullType node)
        {
            var updatedNode = EnterNonNullType(node);
            if (updatedNode == null) return null;
            var type = Visit(updatedNode.Type);
            updatedNode = updatedNode.Update(type);
            updatedNode = LeaveNonNullType(updatedNode);
            return updatedNode;
        }

        public override INode VisitNamedType(NamedType node)
        {
            var updatedNode = EnterNamedType(node);
            if (updatedNode == null) return null;
            var name = Visit(updatedNode.Name);
            updatedNode = updatedNode.Update(name);
            updatedNode = LeaveNamedType(updatedNode);
            return updatedNode;
        }

        public virtual INode Enter(INode node)
        {
            return node;
        }

        public virtual INode Leave(INode node)
        {
            return node;
        }

        public virtual Name EnterName(Name name)
        {
            return Enter(name);
        }

        public virtual Name LeaveName(Name name)
        {
            return Leave(name);
        }

        public virtual Document EnterDocument(Document document)
        {
            return Enter(document);
        }

        public virtual Document LeaveDocument(Document document)
        {
            return Leave(document);
        }

        public virtual OperationDefinition EnterOperationDefinition(OperationDefinition operationDefinition)
        {
            return Enter(operationDefinition);
        }

        public virtual OperationDefinition LeaveOperationDefinition(OperationDefinition operationDefinition)
        {
            return Leave(operationDefinition);
        }

        public virtual VariableDefinition EnterVariableDefinition(VariableDefinition variableDefinition)
        {
            return Enter(variableDefinition);
        }

        public virtual VariableDefinition LeaveVariableDefinition(VariableDefinition variableDefinition)
        {
            return Leave(variableDefinition);
        }

        public virtual Variable EnterVariable(Variable variable)
        {
            return Enter(variable);
        }

        public virtual Variable LeaveVariable(Variable variable)
        {
            return Leave(variable);
        }

        public virtual SelectionSet EnterSelectionSet(SelectionSet selectionSet)
        {
            return Enter(selectionSet);
        }

        public virtual SelectionSet LeaveSelectionSet(SelectionSet selectionSet)
        {
            return Leave(selectionSet);
        }

        public virtual Field EnterField(Field field)
        {
            return Enter(field);
        }

        public virtual Field LeaveField(Field field)
        {
            return Leave(field);
        }

        public virtual Argument EnterArgument(Argument argument)
        {
            return Enter(argument);
        }

        public virtual Argument LeaveArgument(Argument argument)
        {
            return Leave(argument);
        }

        public virtual FragmentSpread EnterFragmentSpread(FragmentSpread fragmentSpread)
        {
            return Enter(fragmentSpread);
        }

        public virtual FragmentSpread LeaveFragmentSpread(FragmentSpread fragmentSpread)
        {
            return Leave(fragmentSpread);
        }

        public virtual InlineFragment EnterInlineFragment(InlineFragment inlineFragment)
        {
            return Enter(inlineFragment);
        }

        public virtual InlineFragment LeaveInlineFragment(InlineFragment inlineFragment)
        {
            return Leave(inlineFragment);
        }

        public virtual FragmentDefinition EnterFragmentDefinition(FragmentDefinition fragmentDefinition)
        {
            return Enter(fragmentDefinition);
        }

        public virtual FragmentDefinition LeaveFragmentDefinition(FragmentDefinition fragmentDefinition)
        {
            return Leave(fragmentDefinition);
        }

        public virtual IntValue EnterIntValue(IntValue intValue)
        {
            return Enter(intValue);
        }

        public virtual IntValue LeaveIntValue(IntValue intValue)
        {
            return Leave(intValue);
        }

        public virtual FloatValue EnterFloatValue(FloatValue floatValue)
        {
            return Enter(floatValue);
        }

        public virtual FloatValue LeaveFloatValue(FloatValue floatValue)
        {
            return Leave(floatValue);
        }

        public virtual StringValue EnterStringValue(StringValue stringValue)
        {
            return Enter(stringValue);
        }

        public virtual StringValue LeaveStringValue(StringValue stringValue)
        {
            return Leave(stringValue);
        }

        public virtual BooleanValue EnterBooleanValue(BooleanValue booleanValue)
        {
            return Enter(booleanValue);
        }

        public virtual BooleanValue LeaveBooleanValue(BooleanValue booleanValue)
        {
            return Leave(booleanValue);
        }

        public virtual EnumValue EnterEnumValue(EnumValue enumValue)
        {
            return Enter(enumValue);
        }

        public virtual EnumValue LeaveEnumValue(EnumValue enumValue)
        {
            return Leave(enumValue);
        }

        public virtual ArrayValue EnterArrayValue(ArrayValue arrayValue)
        {
            return Enter(arrayValue);
        }

        public virtual ArrayValue LeaveArrayValue(ArrayValue arrayValue)
        {
            return Leave(arrayValue);
        }

        public virtual ObjectValue EnterObjectValue(ObjectValue objectValue)
        {
            return Enter(objectValue);
        }

        public virtual ObjectValue LeaveObjectValue(ObjectValue objectValue)
        {
            return Leave(objectValue);
        }

        public virtual ObjectField EnterObjectField(ObjectField objectField)
        {
            return Enter(objectField);
        }

        public virtual ObjectField LeaveObjectField(ObjectField objectField)
        {
            return Leave(objectField);
        }

        public virtual Directive EnterDirective(Directive directive)
        {
            return Enter(directive);
        }

        public virtual Directive LeaveDirective(Directive directive)
        {
            return Leave(directive);
        }

        public virtual ListType EnterListType(ListType listType)
        {
            return Enter(listType);
        }

        public virtual ListType LeaveListType(ListType listType)
        {
            return Leave(listType);
        }

        public virtual NonNullType EnterNonNullType(NonNullType nonNullType)
        {
            return Enter(nonNullType);
        }

        public virtual NonNullType LeaveNonNullType(NonNullType nonNullType)
        {
            return Leave(nonNullType);
        }

        private NamedType LeaveNamedType(NamedType namedType)
        {
            return Enter(namedType);
        }

        private NamedType EnterNamedType(NamedType namedType)
        {
            return Leave(namedType);
        }
    }
}
