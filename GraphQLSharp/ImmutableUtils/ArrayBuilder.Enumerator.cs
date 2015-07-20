using System.Collections;
using System.Collections.Generic;

namespace GraphQLSharp.ImmutableUtils
{
    internal partial class ArrayBuilder<T>
    {
        /// <summary>
        /// struct enumerator used in foreach.
        /// </summary>
        internal struct Enumerator : IEnumerator<T>
        {
            private readonly ArrayBuilder<T> builder;
            private int index;

            public Enumerator(ArrayBuilder<T> builder)
            {
                this.builder = builder;
                index = -1;
            }

            public T Current
            {
                get
                {
                    return builder[index];
                }
            }

            public bool MoveNext()
            {
                index++;
                return index < builder.Count;
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
