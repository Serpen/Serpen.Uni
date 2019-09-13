using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni {
    public static class SortingAlgorithms {

        [AlgorithmComplexity("O²")]
        public static T[] InsertionSort<T>(T[] array) where T : System.IComparable {
            var nextList = new LinkedList<T>();
            LinkedListNode<T> lowest;

            nextList.AddFirst(array[0]);

            for (int i = 1; i < array.Length; i++) {
                if (array[i].CompareTo(nextList.First.Value) < 0)
                    nextList.AddBefore(nextList.First, array[i]);
                else {
                    lowest = nextList.First;
                    while (lowest.Next != null && lowest.Next.Value.CompareTo(array[i]) < 0)
                        lowest = lowest.Next;
                    nextList.AddAfter(lowest, array[i]);
                }
            }

            return nextList.ToArray();
        }

        [AlgorithmComplexity("O²")]
        public static T[] SelectionSort<T>(T[] array) where T : struct, System.IComparable {
            T[] nextList = (T[])array.Clone();

            for (int i = 0; i < nextList.Length; i++) {
                int lowestIndex = i;
                for (int j = i+1; j < nextList.Length; j++) {
                    if (nextList[j].CompareTo(nextList[lowestIndex])<0)
                        lowestIndex = j;
                }
                (nextList[lowestIndex], nextList[i]) = (nextList[i], nextList[lowestIndex]);
                
            }
            return nextList;
        }

        [AlgorithmComplexity("O²")]
        public static T[] BubbleSort<T>(T[] array) where T : struct, System.IComparable {
            T[] nextList = (T[])array.Clone();

            for (int i = 0; i < nextList.Length; i++)
            {
                for (int j = i+1; j < nextList.Length; j++)
                {
                    if (nextList[j].CompareTo(nextList[i]) < 0)
                        (nextList[i], nextList[j]) = (nextList[j], nextList[i]);
                }
            }
            return nextList;
        }
        public static T[] MergeSort<T>(T[] array) => throw new System.NotImplementedException();
        public static T[] QuickSort<T>(T[] array) => throw new System.NotImplementedException();
        public static T[] HeapSort<T>(T[] array) => throw new System.NotImplementedException();
    }
}