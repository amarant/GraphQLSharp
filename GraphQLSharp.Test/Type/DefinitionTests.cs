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
            GraphQLObjectType BlogImage = new GraphQLObjectType("Image",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("url") {Type = Scalars.GraphQLString,},
                        new GraphQLFieldDefinition("width") {Type = Scalars.GraphQLInt,},
                        new GraphQLFieldDefinition("height") {Type = Scalars.GraphQLInt,},
                    })
                );

            GraphQLObjectType BlogAuthor = new GraphQLObjectType("Author",
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

            GraphQLObjectType BlogArticle = new GraphQLObjectType("Article",
                fields: new GraphQLFieldDefinitionMap(
                    new []
                    {
                        new GraphQLFieldDefinition("id", type: Scalars.GraphQLString), 
                        new GraphQLFieldDefinition("isPublished", type: Scalars.GraphQLBoolean), 
                        new GraphQLFieldDefinition("author", type: BlogAuthor), 
                        new GraphQLFieldDefinition("title", type: Scalars.GraphQLString),
                        new GraphQLFieldDefinition("body", type: Scalars.GraphQLString),
                    }));
        }

        public void TestMethod1()
        {
        }
    }
}
