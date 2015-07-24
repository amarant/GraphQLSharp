using System;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace GraphQLSharp.Language.Schema
{
    public class SchemaDocument : ANode
    {
        public override NodeType Kind => NodeType.SchemaDocument;
        public ImmutableArray<SchemaDefinition> Definitions { get; set; } = ImmutableArray<SchemaDefinition>.Empty;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitSchemaDocument(this);
        }
    }

    public abstract class SchemaDefinition : ANode
    {
        [NotNull] public Name Name { get; set; }
    }

    public interface ICompositeDefinition : INode
    {
        [NotNull] Name Name { get; set; }
    }

    public class TypeDefinition : SchemaDefinition, ICompositeDefinition
    {
        public override NodeType Kind => NodeType.TypeDefinition;
        public ImmutableArray<NamedType> Interfaces { get; set; } = ImmutableArray<NamedType>.Empty;
        public ImmutableArray<FieldDefinition> Fields { get; set; } = ImmutableArray<FieldDefinition>.Empty;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitTypeDefinition(this);
        }
    }

    public class FieldDefinition : ANode
    {
        public override NodeType Kind => NodeType.FieldDefinition;
        [NotNull] public Name Name { get; set; }
        public ImmutableArray<ArgumentDefinition> Arguments { get; set; } = ImmutableArray<ArgumentDefinition>.Empty;
        public IType Type { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {                              
            return visitor.VisitFieldDefinition(this);
        }
    }

    public class ArgumentDefinition : ANode
    {
        public override NodeType Kind => NodeType.ArgumentDefinition;
        [NotNull] public Name Name { get; set; }
        [NotNull] public IType Type { get; set; }
        public IValue DefaultValue { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitArgumentDefinition(this);
        }
    }

    public class InterfaceDefinition : SchemaDefinition, ICompositeDefinition
    {
        public override NodeType Kind => NodeType.InterfaceDefinition;
        public ImmutableArray<FieldDefinition> Fields { get; set; } = ImmutableArray<FieldDefinition>.Empty;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInterfaceDefinition(this);
        }
    }

    public class UnionDefinition : SchemaDefinition, ICompositeDefinition
    {
        public override NodeType Kind => NodeType.UnionDefinition;
        public ImmutableArray<NamedType> Types { get; set; } = ImmutableArray<NamedType>.Empty;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitUnionDefinition(this);
        }
    }

    public class ScalarDefinition : SchemaDefinition
    {
        public override NodeType Kind => NodeType.ScalarDefinition;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitScalarDefinition(this);
        }
    }

    public class EnumDefinition : SchemaDefinition
    {
        public override NodeType Kind => NodeType.EnumDefinition;
        public ImmutableArray<EnumValueDefinition> Values { get; set; } = ImmutableArray<EnumValueDefinition>.Empty;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitEnumDefinition(this);
        }
    }

    public class EnumValueDefinition : ANode
    {
        public EnumValueDefinition() {}
        public EnumValueDefinition(String value, Location location)
        {
            Name = new Name(value, location);
            Location = location;
        }

        public override NodeType Kind => NodeType.EnumValueDefinition;
        [NotNull] public Name Name { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitEnumValueDefinition(this);
        }
    }

    public class InputObjectDefinition : SchemaDefinition
    {
        public override NodeType Kind => NodeType.InputObjectDefinition;
        public ImmutableArray<InputFieldDefinition> Fields { get; set; } = ImmutableArray<InputFieldDefinition>.Empty;

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInputObjectDefinition(this);
        }
    }

    public class InputFieldDefinition : ANode
    {
        public override NodeType Kind => NodeType.InputFieldDefinition;
        [NotNull] public Name Name { get; set; }
        [NotNull] public IType Type { get; set; }

        public override TResult Accept<TResult>(Visitor<TResult> visitor)
        {
            return visitor.VisitInputFieldDefinition(this);
        }
    }
}
