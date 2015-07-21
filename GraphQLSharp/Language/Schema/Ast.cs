using System.Collections.Immutable;

namespace GraphQLSharp.Language.Schema
{
    public class SchemaDocument : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.SchemaDocument; }
        }

        public ImmutableArray<SchemaDefinition> Definitions { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitSchemaDocument(this);
        }
    }

    public abstract class SchemaDefinition : ANode
    {
        public Name Name { get; set; }
    }

    public class TypeDefinition : SchemaDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.TypeDefinition; }
        }

        public ImmutableArray<NamedType> Interfaces { get; set; }
        public ImmutableArray<FieldDefinition> Fields { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitTypeDefinition(this);
        }
    }


    public class FieldDefinition : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.FieldDefinition; }
        }

        public Name Name { get; set; }
        public IType Type { get; set; }
        public ImmutableArray<ArgumentDefinition> Arguments { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitFieldDefinition(this);
        }
    }

    public class ArgumentDefinition : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.ArgumentDefinition; }
        }

        public Name Name { get; set; }
        public IType Type { get; set; }
        public IValue DefaultValue { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitArgumentDefinition(this);
        }
    }

    public class InterfaceDefinition : SchemaDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.InterfaceDefinition; }
        }

        public ImmutableArray<FieldDefinition> Fields { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInterfaceDefinition(this);
        }
    }

    public class UnionDefinition : SchemaDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.UnionDefinition; }
        }

        public ImmutableArray<Name> Interfaces { get; set; }
        public ImmutableArray<NamedType> Types { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitUnionDefinition(this);
        }
    }

    public class ScalarDefinition : SchemaDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.ScalarDefinition; }
        }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitScalarDefinition(this);
        }
    }

    public class EnumDefinition : SchemaDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.EnumDefinition; }
        }

        public ImmutableArray<EnumValueDefinition> Values { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitEnumDefinition(this);
        }
    }

    public class EnumValueDefinition : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.EnumValueDefinition; }
        }

        public Name Name { get; set; }
        public ImmutableArray<Name> Values { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitEnumValueDefinition(this);
        }
    }

    public class InputObjectDefinition : SchemaDefinition
    {
        public override NodeType Kind
        {
            get { return NodeType.InputObjectDefinition; }
        }

        public ImmutableArray<InputFieldDefinition> Fields { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInputObjectDefinition(this);
        }
    }

    public class InputFieldDefinition : ANode
    {
        public override NodeType Kind
        {
            get { return NodeType.InputFieldDefinition; }
        }

        public Name Name { get; set; }
        public IType Type { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInputFieldDefinition(this);
        }
    }
}
