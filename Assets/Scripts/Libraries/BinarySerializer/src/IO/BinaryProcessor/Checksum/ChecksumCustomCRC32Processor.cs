using System;

namespace BinarySerializer
{
    public class ChecksumCustomCRC32Processor : ChecksumProcessor
    {
        public ChecksumCustomCRC32Processor(CRCParameters parameters)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _mask = UInt64.MaxValue >> (64 - Parameters.HashSize);
            CreateTable();
            Initialize();
        }

        private readonly ulong _mask;
        private readonly ulong[] _table = new ulong[256];
        private ulong _currentValue;

        public CRCParameters Parameters { get; }

        public override long CalculatedValue => (long)(_currentValue ^ Parameters.XorOut);

        private static ulong ReverseBits(ulong value, int valueLength)
        {
            ulong newValue = 0;

            for (int i = valueLength - 1; i >= 0; i--)
            {
                newValue |= (value & 1) << i;
                value >>= 1;
            }

            return newValue;
        }

        private void CreateTable()
        {
            int hashSize = Parameters.HashSize;

            for (int i = 0; i < _table.Length; i++)
            {
                ulong r = (ulong)i;

                if (Parameters.RefIn)
                    r = ReverseBits(r, hashSize);
                else if (hashSize > 8)
                    r <<= hashSize - 8;

                ulong lastBit = 1uL << (hashSize - 1);

                for (int j = 0; j < 8; j++)
                {
                    if ((r & lastBit) != 0)
                        r = (r << 1) ^ Parameters.Poly;
                    else
                        r <<= 1;
                }

                if (Parameters.RefIn)
                    r = ReverseBits(r, hashSize);

                _table[i] = r & _mask;
            }
        }

        private void Initialize()
        {
            _currentValue = Parameters.RefOut ? ReverseBits(Parameters.Init, Parameters.HashSize) : Parameters.Init;
        }

        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            if (Parameters.RefOut)
            {
                for (int i = offset; i < offset + count; i++)
                {
                    _currentValue = _table[(_currentValue ^ buffer[i]) & 0xFF] ^ (_currentValue >> 8);
                    _currentValue &= _mask;
                }
            }
            else
            {
                int toRight = Parameters.HashSize - 8;
                toRight = toRight < 0 ? 0 : toRight;
                for (int i = offset; i < offset + count; i++)
                {
                    _currentValue = _table[((_currentValue >> toRight) ^ buffer[i]) & 0xFF] ^ (_currentValue << 8);
                    _currentValue &= _mask;
                }
            }
        }

        public class CRCParameters
        {
            public CRCParameters(int hashSize, ulong poly, ulong init, bool refIn, bool refOut, ulong xorOut)
            {
                HashSize = hashSize;
                Poly = poly;
                Init = init;
                RefIn = refIn;
                RefOut = refOut;
                XorOut = xorOut;
            }

            public int HashSize { get; }
            public ulong Poly { get; }
            public ulong Init { get; }
            public bool RefIn { get; }
            public bool RefOut { get; }
            public ulong XorOut { get; }
        }
    }
}