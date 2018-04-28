using System.Linq;

namespace GroovyMp3.Types
{
    public static class Arrays
    {
        public static void Fill<T>(T[] array, int start, int end, T value)
        {
            for (var i = start; i < end; i++)
                array[i] = value;
        }

        public static void Fill<T>(T[] array, T value)
        {
            Fill(array, 0, array.Length, value);
        }

        public static void Sort<T>(T[] array, int start, int end)
        {
            var sortedPart = array.Skip(start).Take(array.Length - end).OrderBy(x => x).ToList();
            for (var i = start; i < end; i++)
            {
                array[i] = sortedPart[i - start];
            }
        }

        internal static T[][] ReturnRectangularArray<T>(int size1, int size2)
        {
            var newArray = new T[size1][];
            for (var array1 = 0; array1 < size1; array1++)
                newArray[array1] = new T[size2];

            return newArray;
        }

        internal static T[][][] ReturnRectangularArray<T>(int size1, int size2, int size3)
        {
            var newArray = new T[size1][][];
            for (var array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new T[size2][];
				if (size3 > 0)
                for (var array2 = 0; array2 < size2; array2++)
                    newArray[array1][array2] = new T[size3];
            }

            return newArray;
        }

        internal static T[][][][] ReturnRectangularArray<T>(int size1, int size2, int size3, int size4)
        {
            var newArray = new T[size1][][][];
            for (var array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new T[size2][][];
                for (var array2 = 0; array2 < size2; array2++)
                {
                    newArray[array1][array2] = new T[size3][];
                    for (var array3 = 0; array3 < size3; array3++)
                        newArray[array1][array2][array3] = new T[size4];
                }
            }

            return newArray;
        }

    }
}