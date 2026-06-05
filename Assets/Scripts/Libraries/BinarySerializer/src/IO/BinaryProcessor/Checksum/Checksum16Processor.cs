namespace BinarySerializer
{
    public class Checksum16Processor : ChecksumProcessor
    {
        public Checksum16Processor(bool invertBits = false)
        {
            InvertBits = invertBits;
        }

        private ushort _checksumValue;

        /// <summary>
        /// Indicates if the checksum value bits should be inverted
        /// </summary>
        public bool InvertBits { get; }

        public override long CalculatedValue
        {
            get => (ushort)(InvertBits ? ~_checksumValue : _checksumValue);
            set => _checksumValue = (ushort)(InvertBits ? ~value : value);
        }

        public override void ProcessBytes(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; i++)
                _checksumValue = (ushort)(_checksumValue + buffer[i]);
        }
    }
}