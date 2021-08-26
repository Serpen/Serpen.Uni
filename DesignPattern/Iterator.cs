using System.Collections;
using System.Collections.Generic;

namespace Serpen.Uni.DesignPattern {

    interface IIterator<T> {
        bool MoveNext();
        T Current { get; }
    }

    class StringList : IEnumerable<string> {
        private readonly string[] items = { "one", "two", "three" };

        public IEnumerator<string> GetEnumerator() => new Iterator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        class Iterator : IIterator<string>, IEnumerator<string> {

            int index;
            private readonly StringList stringList;

            internal Iterator(StringList stringList) {
                Reset();
                this.stringList = stringList;
            }

            public string Current => stringList.items[index];

            object IEnumerator.Current => Current;

            public bool MoveNext() => ++index < stringList.items.Length;

            public void Reset() {
                index = 0;
            }

            public void Dispose() { }
        }
    }
}