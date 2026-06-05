namespace BinarySerializer
{
    public class Checksum32Processor : ChecksumProcessor
    {
        public Checksum32Processor(bool invertBits = false, int valueSize = 1)
        {
            InvertBits = invertBits;
            ValueSize = valueSize;
        }

        private int _checksumValue;
        private int _valueIndex;

        /// <summary>
        /// Indicates if the checksum value bits should be inverted
        /// </summary>
        public bool InvertBits { get; }

        /// <summary>
        /// The size of a value to calculate, in bytes
        /// </summary>
        public int ValueSize { get; }

        public override long CalculatedValue
        {
            get => InvertBits ? ~_checksumValue : _checksumValue;
            set => _checksumValue = (int)(InvertBits ? ~value : value);
        }

        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; i++)
                _checksumValue += buffer[i] << ((_valueIndex++ % ValueSize) * 8);
        }
    }
}