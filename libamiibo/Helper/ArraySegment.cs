/*
 * Copyright (C) 2016 Benjamin Krämer
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace LibAmiibo.Helper
{
    public static class ArraySegment
    {
        public static void Copy<T>(byte[] source, ArraySegment<T> destination, int length)
            => Array.Copy(source, 0, destination.Array, destination.Offset, length);

        public static void Copy<T>(byte[] source, int sourceIndex, ArraySegment<T> destination, int destinationIndex, int length)
            => Array.Copy(source, sourceIndex, destination.Array, destination.Offset + destinationIndex, length);

        public static void Copy<T>(ArraySegment<T> source, ArraySegment<T> destination, int length)
            => Array.Copy(source.Array, source.Offset, destination.Array, destination.Offset, length);

        public static void Copy<T>(ArraySegment<T> source, int sourceIndex, ArraySegment<T> destination, int destinationIndex, int length)
            => Array.Copy(source.Array, source.Offset + sourceIndex, destination.Array, destination.Offset + destinationIndex, length);

        public static void CopyFrom<T>(this ArraySegment<T> destination, byte[] source)
            => Copy(source, destination, source.Length);

        public static void CopyFrom<T>(this ArraySegment<T> destination, byte[] source, int length)
            => Copy(source, destination, length);

        public static void CopyFrom<T>(this ArraySegment<T> destination, byte[] source, int sourceIndex, int destinationIndex, int length)
            => Copy(source, sourceIndex, destination, destinationIndex, length);

        public static void CopyFrom<T>(this ArraySegment<T> destination, ArraySegment<T> source)
            => Copy(source, destination, source.Count);

        public static void CopyFrom<T>(this ArraySegment<T> destination, ArraySegment<T> source, int length)
            => Copy(source, destination, length);

        public static void CopyFrom<T>(this ArraySegment<T> destination, ArraySegment<T> source, int sourceIndex, int destinationIndex, int length)
            => Copy(source, sourceIndex, destination, destinationIndex, length);
    }
}
