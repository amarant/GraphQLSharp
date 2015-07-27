using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQLSharp.Language;

namespace GraphQLSharp.Type
{
    public sealed class GraphQLInt : GraphQLScalarType<int?>
    {
        public static readonly GraphQLInt Instance = new GraphQLInt();
        private GraphQLInt()
            : base("Int", null)
        { }

        public override int? Coerce(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        public override int? Coerce(IValue ast)
        {
            if (ast.Kind == NodeType.IntValue)
            {
                int value;
                if (int.TryParse(((IntValue)ast).Value, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }

    public sealed class GraphQLFloat : GraphQLScalarType<double?>
    {
        public static readonly GraphQLFloat Instance = new GraphQLFloat();
        private GraphQLFloat()
            : base("Float", null)
        { }

        public override double? Coerce(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return null;
            }
        }

        public override double? Coerce(IValue ast)
        {
            if (ast.Kind == NodeType.FloatValue)
            {
                double value;
                if (double.TryParse(((IntValue)ast).Value, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }

    public sealed class GraphQLString : GraphQLScalarType<string>
    {
        public static readonly GraphQLString Instance = new GraphQLString();
        private GraphQLString()
            : base("String", null)
        { }

        public override string Coerce(object value) => value?.ToString();

        public override string Coerce(IValue ast)
        {
            if (ast.Kind == NodeType.StringValue)
            {
                return ((IntValue)ast).Value;
            }
            return null;
        }
    }

    public sealed class GraphQLBoolean : GraphQLScalarType<bool?>
    {
        public static readonly GraphQLBoolean Instance = new GraphQLBoolean();
        private GraphQLBoolean()
            : base("Boolean", null)
        { }

        public override bool? Coerce(object value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return null;
            }
        }

        public override bool? Coerce(IValue ast)
        {
            if (ast.Kind == NodeType.FloatValue)
            {
                bool value;
                if (bool.TryParse(((IntValue)ast).Value, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }

    public sealed class GraphQLID : GraphQLScalarType<string>
    {
        public static readonly GraphQLID Instance = new GraphQLID();
        private GraphQLID()
            : base("ID", null)
        { }

        public override string Coerce(object value) => value?.ToString();

        public override string Coerce(IValue ast)
        {
            if (ast.Kind == NodeType.StringValue)
            {
                return ((StringValue)ast).Value;
            }
            else if (ast.Kind == NodeType.IntValue)
            {
                return ((IntValue)ast).Value;
            }
            return null;
        }
    }

    public static class Scalars
    {
        public static readonly GraphQLInt GraphQLInt = GraphQLInt.Instance;
        public static readonly GraphQLFloat GraphQLFloat = GraphQLFloat.Instance;
        public static readonly GraphQLString GraphQLString = GraphQLString.Instance;
        public static readonly GraphQLBoolean GraphQLBoolean = GraphQLBoolean.Instance;
        public static readonly GraphQLID GraphQLID = GraphQLID.Instance;
    }
}
