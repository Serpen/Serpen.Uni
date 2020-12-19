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
                for (int j = i + 1; j < nextList.Length; j++) {
                    if (nextList[j].CompareTo(nextList[lowestIndex]) < 0)
                        lowestIndex = j;
                }
                (nextList[lowestIndex], nextList[i]) = (nextList[i], nextList[lowestIndex]);

            }
            return nextList;
        }

        [AlgorithmComplexity("O²")]
        public static T[] BubbleSort<T>(T[] array) where T : struct, System.IComparable {
            T[] nextList = (T[])array.Clone();

            for (int i = 0; i < nextList.Length; i++) {
                for (int j = i + 1; j < nextList.Length; j++) {
                    if (nextList[j].CompareTo(nextList[i]) < 0)
                        (nextList[i], nextList[j]) = (nextList[j], nextList[i]);
                }
            }
            return nextList;
        }
        public static T[] MergeSort<T>(T[] array) where T : System.IComparable  => throw new System.NotImplementedException();
        public static T[] QuickSort<T>(T[] array) where T : System.IComparable  => throw new System.NotImplementedException();

        [AlgorithmComplexity("n*log(n)")]
        public static T[] HeapSort<T>(in T[] array) where T : System.IComparable {
            int n = array.Length;
            T[] array2 = (T[])array.Clone();

            for (int i = n / 2; i >= 1; i--)
                Reheap<T>(ref array2, i, n);

            for (int i = n; i >= 2; i--) {
                (array2[0], array2[i - 1]) = (array2[i - 1], array2[0]);
                Reheap<T>(ref array2, 1, i - 1);
            }
            return array2;
        }

        static void Reheap<T>(ref T[] array, int i, int k) where T : System.IComparable {
            int j = i;
            int son = 0;
            bool endLoop = false;

            do {
                if (2 * j > k)
                    break;
                else {
                    if (2 * j + 1 <= k)
                        if (array[2 * j - 1].CompareTo(array[2 * j]) < 0)
                            son = 2 * j;
                        else
                            son = 2 * j + 1;
                    else
                        son = 2 * j;

                    if (array[son - 1].CompareTo(array[j - 1]) < 0) {
                        (array[j - 1], array[son - 1]) = (array[son - 1], array[j - 1]);
                        j = son;
                    } else
                        endLoop = true;
                }
            } while (!endLoop);
            Utils.DebugMessage(System.String.Join(',', array), Utils.eDebugLogLevel.Verbose);
        }
    }
}