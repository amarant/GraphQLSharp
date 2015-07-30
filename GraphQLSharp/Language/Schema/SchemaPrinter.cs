using static System.String;

namespace GraphQLSharp.Language.Schema
{
    public class SchemaPrinter : Printer
    {
        public override string VisitSchemaDocument(SchemaDocument node)
            => $"{JoinNotNull("\n\n", VisitList(node.Definitions))}\n";

        public override string VisitTypeDefinition(TypeDefinition node) 
            => $"type {node.Name} {ManyList("implements ", VisitList(node.Interfaces), ", ", " ")}{Block(VisitList(node.Fields))}";

        public override string VisitFieldDefinition(FieldDefinition node) 
            => $"{VisitName(node.Name)}{ManyList("(", VisitList(node.Arguments), ", ", ")")}: {Visit(node.Type)}";

        public override string VisitInputValueDefinition(InputValueDefinition node) 
            => $"{VisitName(node.Name)}: {Visit(node.Type)}{Wrap(" = ", Visit(node.DefaultValue))}";

        public override string VisitInterfaceDefinition(InterfaceDefinition node) 
            => $"interface {VisitName(node.Name)} {Block(VisitList(node.Fields))}";

        public override string VisitUnionDefinition(UnionDefinition node) 
            => $"union {VisitName(node.Name)} = {Join(" | ", VisitList(node.Types))}";

        public override string VisitScalarDefinition(ScalarDefinition node) 
            => $"scalar {VisitName(node.Name)}";

        public override string VisitEnumDefinition(EnumDefinition node) 
            => $"enum {VisitName(node.Name)} {Block(VisitList(node.Values))}";

        public override string VisitEnumValueDefinition(EnumValueDefinition node) 
            => VisitName(node.Name);

        public override string VisitInputObjectDefinition(InputObjectDefinition node) 
            => $"input {VisitName(node.Name)} {Block(VisitList(node.Fields))}";
    }
}
