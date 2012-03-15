﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class MobiusRange
    {
        private struct Offsets
        {
            public int Offset;
            public long OffsetSquared;
        }

        private const int blockSize = 1 << 15;

        private long size;
        private int threads;
        private int[] primes;
        private int cycleLimit;
        private int cycleSize;
        private long[] cycle;
        private long[][] productsArray;
        private Offsets[][] offsetsArray;

        public long Size { get { return size; } }

        public MobiusRange(long size, int threads)
        {
            this.size = size;
            this.threads = threads;
            var limit = (int)Math.Ceiling(Math.Sqrt(size));
            primes = new PrimeCollection(limit, 0).Select(p => (int)p).ToArray();
            CreateCycle();
            var arrayLength = Math.Max(1, threads);
            productsArray = new long[arrayLength][];
            offsetsArray = new Offsets[arrayLength][];
            for (var thread = 0; thread < arrayLength; thread++)
            {
                productsArray[thread] = new long[blockSize];
                offsetsArray[thread] = new Offsets[primes.Length];
            }
        }

        public void GetValues(long kmin, long kmax, sbyte[] values)
        {
            var plimit = (int)Math.Ceiling(Math.Sqrt(kmax));
            var pmax = 0;
            while (pmax < primes.Length && primes[pmax] < plimit)
                ++pmax;
            if (threads == 0)
            {
                var products = productsArray[0];
                var offsets = offsetsArray[0];
                ProcessRange(pmax, kmin, kmax, kmin, values, products, offsets);
                return;
            }
            var tasks = new Task[threads];
            var length = kmax - kmin;
            var batchSize = ((length + threads - 1) / threads + 1) & ~1;
            for (var thread = 0; thread < threads; thread++)
            {
                var kstart = (long)thread * batchSize + kmin;
                var kend = Math.Min(kstart + batchSize, kmax);
                var products = productsArray[thread];
                var offsets = offsetsArray[thread];
                tasks[thread] = Task.Factory.StartNew(() => ProcessRange(pmax, kstart, kend, kmin, values, products, offsets));
            }
            Task.WaitAll(tasks);
            if (kmin <= 1)
                values[1 - kmin] = 1;
        }


        private void CreateCycle()
        {
            // Create pre-sieved cycle of the squares of small primes.
            var dmax = 3;
            cycleLimit = Math.Min(primes.Length, dmax);
            cycleSize = 1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = (int)primes[i];
                cycleSize *= p * p;
            }
            cycle = new long[cycleSize];
            for (var i = 0; i < cycleSize; i++)
                cycle[i] = 1;
            for (var i = 0; i < cycleLimit; i++)
            {
                var p = primes[i];
                var pMinus = -p;
                for (var k = 0; k < cycleSize; k += p)
                    cycle[k] *= pMinus;
                var pSquared = (long)p * p;
                for (var k = (long)0; k < cycleSize; k += pSquared)
                    cycle[k] = 0;
            }
        }

        private void ProcessRange(int pmax, long kstart, long kend, long kmin, sbyte[] values, long[] products, Offsets[] offsets)
        {
            var cycleOffset = cycleSize - (int)(kstart % cycleSize);
            if (cycleOffset == cycleSize)
                cycleOffset = 0;
            offsets[0].Offset = cycleOffset;
            for (var i = 1; i < pmax; i++)
            {
                var p = primes[i];
                var offset = p - (int)(kstart % p);
                if (offset == p)
                    offset = 0;
                offsets[i].Offset = offset;
                var pSquared = (long)p * p;
                var offsetSquared = pSquared - kstart % pSquared;
                if (offsetSquared == pSquared)
                    offsetSquared = 0;
                offsets[i].OffsetSquared = offsetSquared;
            }
            for (var k = kstart; k < kend; k += blockSize)
            {
                var length = (int)Math.Min(blockSize, kend - k);
                SieveBlock(pmax, k, length, products, offsets);
                AddValues(k, length, products, kmin, values);
            }
        }

        private void SieveBlock(int pmax, long k0, int length, long[] products, Offsets[] offsets)
        {
            var cycleOffset = offsets[0].Offset;
            Array.Copy(cycle, cycleSize - cycleOffset, products, 0, Math.Min(length, cycleOffset));
            while (cycleOffset < length)
            {
                Array.Copy(cycle, 0, products, cycleOffset, Math.Min(cycleSize, length - cycleOffset));
                cycleOffset += cycleSize;
            }
            offsets[0].Offset = cycleOffset - length;

            for (var i = cycleLimit; i < pmax; i++)
            {
                var p = primes[i];
                var pMinus = -p;
                int k;
                for (k = offsets[i].Offset; k < length; k += p)
                    products[k] *= pMinus;
                offsets[i].Offset = k - length;
                var pSquared = (long)p * p;
                long kk;
                for (kk = offsets[i].OffsetSquared; kk < length; kk += pSquared)
                    products[kk] = 0;
                offsets[i].OffsetSquared = kk - length;
            }
        }

        private void AddValues(long k0, int length, long[] products, long kmin, sbyte[] values)
        {
            // Each product that is square-free can have at most one more
            // prime factor.  It has that factor if the absolute value of
            // the product is less than the full value.
            var k = k0;
            for (var i = 0; i < length; i++, k++)
            {
                var p = products[i];
                var pos = -p >> 63; // pos = -1 if p > 0, zero otherwise
                var neg = p >> 63; // neg = -1 if p is < 0, zero otherwise
                var abs = (p + neg) ^ neg; // abs = |p|
                var flip = (abs - k) >> 63; // flip = -1 if abs < k, zero otherwise
                values[k - kmin] = (sbyte)(((neg - pos) ^ flip) - flip); // values[k] = pos - neg if flip = -1, neg - pos otherwise
                Debug.Assert(values[k - kmin] == Math.Sign(p) * (Math.Abs(p) != k ? -1 : 1));
            }
        }
    }
}
