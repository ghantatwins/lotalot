using System;
using System.Security.Cryptography;

namespace LottoData.Lib
{
    public class SecureRandom:Random
    {
        private const string ParamName = @"{0} must be lower than {1}";
        readonly RNGCryptoServiceProvider _csp;

        public SecureRandom()
        {
            _csp = new RNGCryptoServiceProvider("TANYA");
        }

        public override int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
                throw new ArgumentOutOfRangeException(string.Format(ParamName,minValue,maxExclusiveValue));

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        public override double NextDouble()
        {
            var bytes = new byte[8];
            _csp.GetBytes(bytes);
            // Step 2: bit-shift 11 and 53 based on double's mantissa bits
            var ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
           return ul / (double)(1UL << 53);
        }

        public override int Next()
        {
            return Next(0, int.MaxValue);
        }

        public override int Next(int maxValue)
        {
            return Next(0,maxValue);
        }

        
        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            _csp.GetBytes(buffer);
            return buffer;
        }
    }
}