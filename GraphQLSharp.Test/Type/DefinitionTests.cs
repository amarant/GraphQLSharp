using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GraphQLSharp.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphQLSharp.Test.Type
{
    public class DefinitionTests
    {
        private GraphQLObjectType BlogImage;
        private GraphQLObjectType BlogAuthor;

        public DefinitionTests()
        {
            var BlogImage = new GraphQLObjectType("Image",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("url") {Type = Scalars.GraphQLString,},
                        new GraphQLFieldDefinition("width") {Type = Scalars.GraphQLInt,},
                        new GraphQLFieldDefinition("height") {Type = Scalars.GraphQLInt,},
                    })
                );

            var BlogAuthor = new GraphQLObjectType("Author",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("id") {Type = Scalars.GraphQLString,},
                        new GraphQLFieldDefinition("name") {Type = Scalars.GraphQLString,},
                        new GraphQLFieldDefinition("pic", args: new []
                        {
                            new GraphQLArgument("width", Scalars.GraphQLInt),
                            new GraphQLArgument("height", Scalars.GraphQLInt),
                        }, type: Scalars.GraphQLString),
                        new GraphQLFieldDefinition("recentArticle", type: BlogImage),
                    }));

            var BlogArticle = new GraphQLObjectType("Article",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("id", type: Scalars.GraphQLString), 
                        new GraphQLFieldDefinition("isPublished", type: Scalars.GraphQLBoolean), 
                        new GraphQLFieldDefinition("author", type: BlogAuthor), 
                        new GraphQLFieldDefinition("title", type: Scalars.GraphQLString),
                        new GraphQLFieldDefinition("body", type: Scalars.GraphQLString),
                    }));
            var BlogQuery = new GraphQLObjectType("Query",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("article", args: new []
                        {
                            new GraphQLArgument("id", Scalars.GraphQLString), 
                        }, type: BlogArticle), 
                        new GraphQLFieldDefinition("feed", type: new GraphQLList(BlogArticle)), 
                    }));
            var BlogMutation = new GraphQLObjectType("Mutation",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("writeArticle", type: BlogArticle), 
                    }));
            var ObjectType = new GraphQLObjectType("Object");
            var InterfaceType = new GraphQLObjectType("Interface");
            var UnionType = new GraphQLUnionType("Union", types: ImmutableArray.Create(new GraphQLObjectType()));
        }

        public void TestMethod1()
        {
        }
    }
}
