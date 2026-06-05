using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer
{
    public abstract class BaseBitwiseColor : BaseColor
    {
        #region Constructors

        protected BaseBitwiseColor() { }
        protected BaseBitwiseColor(float r, float g, float b, float a = 1f) : base(r, g, b, a) { }
        protected BaseBitwiseColor(uint colorValue) 
        {
            ColorValue = colorValue;
        }

        #endregion

        #region Protected Methods

        protected float GetFactor(int count) => (float)(Math.Pow(2, count) - 1);

        protected float GetValue(ColorChannel channel)
        {
            if (!ColorFormatting.ContainsKey(channel))
                return channel == ColorChannel.Alpha ? 1f : 0f;

            return GetValue(ColorFormatting[channel]);
        }
        protected float GetValue(ColorChannelFormat format) => BitHelpers.ExtractBits((int)ColorValue, format.Count, format.Offset) / GetFactor(format.Count);

        protected void SetValue(ColorChannel channel, float value)
        {
            if (!ColorFormatting.ContainsKey(channel))
                return;

            SetValue(ColorFormatting[channel], value);
        }
        protected void SetValue(ColorChannelFormat format, float value)
        {
            int intValue = (int)Math.Round(value * GetFactor(format.Count));
            ColorValue = (uint)BitHelpers.SetBits((int)ColorValue, intValue, format.Count, format.Offset);
        }

        #endregion

        #region Protected Properties

        protected abstract IReadOnlyDictionary<ColorChannel, ColorChannelFormat> ColorFormatting { get; }

        #endregion

        #region Public Properties

        public uint ColorValue { get; set; }

        public override float Red
        {
            get => GetValue(ColorChannel.Red);
            set => SetValue(ColorChannel.Red, value);
        }

        public override float Green
        {
            get => GetValue(ColorChannel.Green);
            set => SetValue(ColorChannel.Green, value);
        }

        public override float Blue
        {
            get => GetValue(ColorChannel.Blue);
            set => SetValue(ColorChannel.Blue, value);
        }

        public override float Alpha
        {
            get => GetValue(ColorChannel.Alpha);
            set => SetValue(ColorChannel.Alpha, value);
        }

        #endregion

        #region Serializable

        public override void SerializeImpl(SerializerObject s)
        {
            var maxBit = ColorFormatting.Values.Max(x => x.Offset + x.Count);

            if (maxBit <= 8)
                ColorValue = s.Serialize<byte>((byte)ColorValue, name: nameof(ColorValue));
            else if (maxBit <= 16)
                ColorValue = s.Serialize<ushort>((ushort)ColorValue, name: nameof(ColorValue));
            else if (maxBit <= 24)
                ColorValue = s.Serialize<UInt24>((UInt24)ColorValue, name: nameof(ColorValue));
            else if (maxBit <= 32)
                ColorValue = s.Serialize<uint>((uint)ColorValue, name: nameof(ColorValue));
            else
                throw new NotImplementedException("Color format with more than 32 bits is currently not supported");
        }

        #endregion

        #region Format Structs

        protected enum ColorChannel
        {
            Red,
            Green,
            Blue,
            Alpha
        }

        protected class ColorChannelFormat
        {
            public ColorChannelFormat(int offset, int count)
            {
                Offset = offset;
                Count = count;
            }

            public int Offset { get; }
            public int Count { get; }
        }

        #endregion
    }
}