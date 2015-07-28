using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using GraphQLSharp.Language;
using JetBrains.Annotations;

namespace GraphQLSharp.Type
{
    public interface IGraphQLType
    {
    }

    public interface IGraphQLInputType
    {
    }

    public interface IGraphQLOutputType
    {
    }

    public interface IGraphQLLeafType
    {
    }

    internal interface IGraphQLCompositeType
    {
    }

    internal interface IGraphQLAbstractType
    {
    }

    public interface IGraphQLNullableType
    {
    }

    public interface IGraphQLUnmodifiedType
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
    public abstract class GraphQLScalarType<T> : IGraphQLType, IGraphQLInputType,
        IGraphQLOutputType, IGraphQLLeafType, IGraphQLNullableType, IGraphQLUnmodifiedType
    {
        [NotNull] public String Name { get; protected set; }
        public String Description { get; protected set; }

        protected GraphQLScalarType(String name, String description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Type must be named.");
            }
            Name = name;
            Description = description;
        }

        public virtual T Coerce(object value) => default(T);
        public virtual T Coerce(IValue value) => default(T);
        public override string ToString() => Name;
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
    public class GraphQLObjectType : IGraphQLType, IGraphQLOutputType, IGraphQLCompositeType,
        IGraphQLNullableType, IGraphQLNamedType
    {
        [NotNull] public String Name { get; set; }
        public String Description { get; set; }

        private Lazy<GraphQLFieldDefinitionMap> _fields;
        public GraphQLFieldDefinitionMap Fields => _fields.Value;

        private Lazy<ImmutableArray<GraphQLInterfaceType>> _interfaces;
        public ImmutableArray<GraphQLInterfaceType> Interfaces => _interfaces.Value;

        private Func<object, bool> _isTypeOf;

        public GraphQLObjectType() {}

        public GraphQLObjectType(string name, string description = null,
            GraphQLFieldDefinitionMap fields = null,
            ImmutableArray<GraphQLInterfaceType> interfaces = default(ImmutableArray<GraphQLInterfaceType>),
            Func<object, bool> isTypeOf = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Type must be named.");
            }
            Name = name;
            Description = description;
            _fields = new Lazy<GraphQLFieldDefinitionMap>(() => fields);
            _interfaces = new Lazy<ImmutableArray<GraphQLInterfaceType>>(() => interfaces);
            _isTypeOf = isTypeOf;
        }

        public bool? IsTypeOf(object value)
        {
            return _isTypeOf?.Invoke(value);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public interface IGraphQLNamedType
    {
    }

    public class GraphQLInterfaceType
    {
    }

    public class GraphQLFieldDefinitionMap
    {
        private ImmutableDictionary<string, GraphQLFieldDefinition> _dict;
        public GraphQLFieldDefinitionMap(IEnumerable<GraphQLFieldDefinition> defs)
        {
            _dict = defs.ToImmutableDictionary(val => val.Name);
        }

        public GraphQLFieldDefinition this[string fieldName] => _dict[fieldName];
    }

    public class GraphQLFieldDefinition
    {
        public GraphQLFieldDefinition(String name, IEnumerable<GraphQLArgument> args = null,
            IGraphQLOutputType type = null)
        {
            Name = name;
            Args = args.ToImmutableDictionary(val => val.Name);
            Type = type;
        }
        public delegate object ResolveFunc(object source, IDictionary<String, object> arg,
            object context, object fieldAST, object fieldType, object parentType,
            GraphQLSchema schema);
        [NotNull] public String Name { get; set; }
        public String Description { get; set; }
        [NotNull] public IGraphQLOutputType Type { get; set;}
        public ImmutableDictionary<string, GraphQLArgument> Args { get; set; } = ImmutableDictionary<string, GraphQLArgument>.Empty;
        public ResolveFunc Resolve { get; set; }
        public string DeprecationReason { get; set; }
    }

    public class GraphQLArgument
    {
        public GraphQLArgument(string name, IGraphQLInputType type)
        {
            Name = name;
            Type = type;
        }

        [NotNull] public String Name { get; set; }
        public IGraphQLInputType Type { get; set; }
        public object DefaultValue { get; set; }
        public string Description { get; set; }
    }

    public interface IGraphQLObjectTypeConfigFields
    {
    }

    public interface IGraphQLObjectTypeConfig
    {
        [NotNull] String Name { get; set; }
        ImmutableArray<GraphQLInterfaceType> Interfaces { get; set; }
        bool isTypeOf(object obj);
        String Description { get; set; }
    }

    public interface IGraphQLFieldConfig
    {
        [NotNull] IGraphQLOutputType Type { get; set; }
        GraphQLFieldConfigArgumentMap Args { get; set; }

        object resolve(object source, object args, object context,
            object fieldAST, object fieldType, object parentType, GraphQLSchema schema);

        string DeprecationReason { get; set; }
        string Description { get; set; }
    }

    public class GraphQLSchema
    {
    }

    public class GraphQLFieldConfigArgumentMap
    {
    }

    /// <summary>
    /// List Modifier
    /// A list is a kind of type marker, a wrapping type which points to another
    /// type. Lists are often created within the context of defining the fields of
    /// an object type.
    /// Example:
    ///     var PersonType = new GraphQLObjectType({
    ///       name: 'Person',
    ///       fields: () => ({
    ///         parents: { type: new GraphQLList(Person) },
    ///         children: { type: new GraphQLList(Person) },
    ///       })
    ///     })
    /// </summary>
    public class GraphQLList : IGraphQLOutputType
    {
        public GraphQLObjectType OfType { get; set; }

        public GraphQLList(GraphQLObjectType ofType)
        {
            OfType = ofType;
        }

        public override string ToString() => $"[ {OfType} ]";
    }

    public class GraphQLUnionType
    {
        [NotNull] public string Name { get; set; }
        public string Description { get; set; }
        public ImmutableArray<GraphQLObjectType> Types { get; set; }
        public Func<object, GraphQLObjectType> Resolver { get; set; }
        public ImmutableDictionary<string, bool> PossibleTypeNames { get; set; }

        public GraphQLUnionType(string name, string description = null,
            ImmutableArray<GraphQLObjectType> types = default(ImmutableArray<GraphQLObjectType>),
            Func<object, GraphQLObjectType> resolver = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name), "Type must be named.");
            }
            if (types.IsEmpty)
            {
                throw new ArgumentException(nameof(types), $"Must provide types for Union ${name}.");
            }
/*            var nullTypes = types.Where(x => x == null).ToList();
            if (nullTypes.Any())
            {
                throw new ArgumentException(nameof(types),
                    $"Union ${name} may only contain object types, it cannot " +
                    $"contain: ${string.Join(", ", nullTypes)}.");
            }*/
            Name = name;
            Description = description;
            Types = types;
            Resolver = resolver;
        }

        public ImmutableArray<GraphQLObjectType> GetPossibleTypes() => Types;

        public bool IsPossibleType(GraphQLObjectType type)
        {
            if (PossibleTypeNames == null)
            {
                PossibleTypeNames = Types.Aggregate(
                    ImmutableDictionary<string, bool>.Empty,
                    (dict, possibleType) => dict.Add(possibleType.Name, true));
            }
            return PossibleTypeNames[type.Name];
        }

        public GraphQLObjectType ResolveType(object value) => 
            Resolver != null ? Resolver(value) : GetTypeOf(value, this);

        public override string ToString() => Name;

        private GraphQLObjectType GetTypeOf(object value, GraphQLUnionType graphQLUnionType)
        {
            throw new NotImplementedException();
        }
    }
}
