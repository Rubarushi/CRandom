using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CRandom
{
    public class CRandom
    {
        const int CMATH_N = 624;
        const int CMATH_M = 397;
        const uint CMATH_MATRIX_A = 0x9908b0df;
        const uint CMATH_UPPER_MASK = 0x80000000;
        const uint CMATH_LOWER_MASK = 0x7fffffff;

        const uint CMATH_TEMPERING_MASK_B = 0x9b2c5680;
        const uint CMATH_TEMPERING_MASK_C = 0xefc60000;

        private Dictionary<uint, uint> ItemDictionary = new Dictionary<uint, uint>();
        private Dictionary<uint, uint> IndexDictionary = new Dictionary<uint, uint>();

        private uint CurrentMaxValue = 0;

        private uint m_rseed;
        private uint[] m_mt = new uint[CMATH_N];
        private int m_mti;

        private static bool SetPeroid = false;

        public CRandom()
        {
            m_rseed = 1;
            m_mti = CMATH_N + 1;

            if(SetPeroid == false)
            {
                timeBeginPeriod(1);
                SetPeroid = true;
            }
        }

        static bool COMPARE(uint x, uint min, uint max)
        {
            if ((x >= min) && (x < max))
            {
                return true;
            }
            return false;
        }

        public uint GetItem()
        {
            int cnt = ItemDictionary.Count;
            uint dwRand = Random(CurrentMaxValue);
            uint dwCurValue = 0;

            for (uint i = 0; i < cnt; i++)
            {
                uint dwValue = ItemDictionary[IndexDictionary[i]];
                if (COMPARE(dwRand, dwCurValue, dwCurValue + dwValue))
                {
                    return i;
                }
                dwCurValue += dwValue;
            }
            return uint.MaxValue; //?
        }

        private uint CurrentIndex = 0;

        public void AddItem(uint idx, uint dwRand)
        {
            if (ItemDictionary.ContainsKey(idx))//If idx is exists, just add rand value
            {
                ItemDictionary[idx] += dwRand;
            }
            else
            {
                ItemDictionary.Add(idx, dwRand);
                IndexDictionary.Add(CurrentIndex, idx);
                CurrentIndex++;
            }
            CurrentMaxValue += dwRand;
        }

        static uint[] mag01 = new uint[2] { 0x0, CMATH_MATRIX_A };

        [DllImport("winmm.dll")]
        public static extern uint timeBeginPeriod(uint uPeroid);

        [DllImport("winmm.dll")]
        public static extern uint timeGetTime();

        public void Randomize()
        {
            SetRandomSeed(timeGetTime());
        }

        public uint Random(uint n)
        {
            if (n == 0)
                return 0;

            uint y;

            if (m_mti >= CMATH_N)
            {
                int kk;

                if (m_mti == CMATH_N + 1)
                    SetRandomSeed(4357);

                for (kk = 0; kk < CMATH_N - CMATH_M; kk++)
                {
                    y = (m_mt[kk] & CMATH_UPPER_MASK) | (m_mt[kk + 1] & CMATH_LOWER_MASK);
                    m_mt[kk] = m_mt[kk + CMATH_M] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                for (; kk < CMATH_N - 1; kk++)
                {
                    y = (m_mt[kk] & CMATH_UPPER_MASK) | (m_mt[kk + 1] & CMATH_LOWER_MASK);
                    m_mt[kk] = m_mt[kk + (CMATH_M - CMATH_N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                y = (m_mt[CMATH_N - 1] & CMATH_UPPER_MASK) | (m_mt[0] & CMATH_LOWER_MASK);
                m_mt[CMATH_N - 1] = m_mt[CMATH_M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                m_mti = 0;
            }

            y = m_mt[m_mti++];
            y ^= y >> 11;
            y ^= y >> 7 & CMATH_TEMPERING_MASK_B;
            y ^= y >> 0xF & CMATH_TEMPERING_MASK_C;
            y ^= y >> 0x12;

            return y % n;
        }

        private uint max(uint min, uint max)
        {
            if (min > max)
                return min;
            return max;
        }

        public bool RandomByPercent(uint perf)
        {
            perf = 100 - perf;
            if (Random(0, 10000) > (perf * 100))
            {
                return true;
            }
            return false;
        }
        
        public uint Random(uint nMin, uint nMax)
        {
            uint nDiff = max(0, nMax - nMin);
            uint nRnd = Random(nDiff);
            return nRnd + nMin;
        }

        public void SetRandomSeed(uint seed)
        {
            m_mt[0] = seed & 0xffffffff;
            for (m_mti = 1; m_mti < CMATH_N; m_mti++)
                m_mt[m_mti] = (0x10DCD * m_mt[m_mti - 1]) & 0xffffffff;

            m_rseed = seed;
        }

        public uint GetRandomSeed()
        {
            return m_rseed;
        }
    }
}
