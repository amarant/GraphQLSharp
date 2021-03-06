﻿using System;

namespace GraphQLSharp.Language
{
    /// <summary>
    /// A representation of source input to GraphQL. The name is optional,
    /// but is mostly useful for clients who store GraphQL documents in
    /// source files; for example, if the GraphQL input is in a file Foo.graphql,
    /// it might be useful for name to be "Foo.graphql".
    /// </summary>
    public class Source
    {
        public String Body { get; private set; }
        public String Name { get; private set; }

        public Source(String body, String name = "GraphQL")
        {
            Body = body;
            Name = name;
        }
    }
}
