using System;
using System.Diagnostics;
using GraphQLSharp.Language;
using JetBrains.Annotations;

namespace GraphQLSharp.Type
{
    internal interface IGraphQLType
    {
    }

    internal interface IGraphQLInputType
    {
    }

    internal interface IGraphQLOutputType
    {
    }

    internal interface IGraphQLLeafType
    {
    }

    internal interface IGraphQLCompositeType
    {
    }

    internal interface IGraphQLAbstractType
    {
    }

    internal interface IGraphQLNullableType
    {
    }

    internal interface IGraphQLUnmodifiedType
    {
    }

    /// <summary>
    /// Scalar Type Definition
    /// 
    /// The leaf values of any request and input values to arguments are
    /// Scalars (or Enums) and are defined with a name and a series of coercion
    /// functions used to ensure validity.
    ///
    /// Example:
    /// 
    ///     var OddType = new GraphQLScalarType({
    ///       name: 'Odd',
    ///       coerce(value) {
    ///         return value % 2 === 1 ? value : null;
    ///       }
    ///     });
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GraphQLScalarType<T> : IGraphQLType, IGraphQLInputType,
        IGraphQLOutputType, IGraphQLLeafType, IGraphQLNullableType, IGraphQLUnmodifiedType
    {
        [NotNull]
        public String Name { get; set; }
        public String Description { get; set; }
        private IGraphQLScalarTypeConfig<T> _scalarConfig;

        public GraphQLScalarType(IGraphQLScalarTypeConfig<T> scalarConfig)
        {
            Debug.Assert(!string.IsNullOrEmpty(scalarConfig.Name), "Type must be named.");
            Name = scalarConfig.Name;
            Description = scalarConfig.Name;
            _scalarConfig = scalarConfig;
        }

        public T Coerce(object value)
        {
            return _scalarConfig.Coerce(value);
        }

        public T Coerce(IValue value)
        {
            return _scalarConfig.CoerceLiteral(value);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public interface IGraphQLScalarTypeConfig<T>
    {
        [NotNull]
        String Name { get; set; }

        String Description { get; set; }

        T Coerce(object value);
        T CoerceLiteral(IValue value);
    }

    /// <summary>
    /// Object Type Definition
    ///
    /// Almost all of the GraphQL types you define will be object types. Object types
    /// have a name, but most importantly describe their fields.
    ///
    /// Example:
    ///
    ///     var AddressType = new GraphQLObjectType({
    ///       name: 'Address',
    ///       fields: {
    ///         street: { type: GraphQLString },
    ///         number: { type: GraphQLInt },
    ///         formatted: {
    ///           type: GraphQLString,
    ///           resolve(obj) {
    ///             return obj.number + ' ' + obj.street
    ///           }
    ///         }
    ///       }
    ///     });
    ///
    /// When two types need to refer to each other, or a type needs to refer to
    /// itself in a field, you can use a function expression (aka a closure or a
    /// thunk) to supply the fields lazily.
    ///
    /// Example:
    ///
    ///     var PersonType = new GraphQLObjectType({
    ///       name: 'Person',
    ///       fields: () => ({
    ///         name: { type: GraphQLString },
    ///         bestFriend: { type: PersonType },
    ///       })
    ///     });
    /// </summary>
    public class GraphQLObjectType
    {
        [NotNull]
        public String Name { get; set; }
        public String Description { get; set; }
        private GraphQLObjectTypeConfig _typeConfig;
        private GraphQLFieldDefinitionMap _fields;

        public GraphQLObjectType(GraphQLObjectTypeConfig typeConfig)
        {
            Debug.Assert(!string.IsNullOrEmpty(typeConfig.Name), "Type must be named.");
            _typeConfig = typeConfig;
        }
    }

    internal class GraphQLFieldDefinitionMap
    {
    }

    public class GraphQLObjectTypeConfig
    {
        [NotNull]
        public String Name { get; set; }
        public String Description { get; set; }
    }
}
