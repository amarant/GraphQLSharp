using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace GraphQLSharp.Test
{
    public static class FluentAssertionsUtils
    {
        public static void ShouldBeEquivalentToDeepDynamic<T>(this T subject, object expectation, string because = "",
            params object[] reasonArgs)
        {
            subject.ShouldBeEquivalentTo(expectation,
                options => options.AllowingInfiniteRecursion().RespectingRuntimeTypes());
        }

    }
}
