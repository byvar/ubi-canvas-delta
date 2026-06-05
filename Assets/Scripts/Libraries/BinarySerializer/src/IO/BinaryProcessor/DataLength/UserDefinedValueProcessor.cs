using System;

namespace BinarySerializer
{
    public class UserDefinedValueProcessor : CalculatedValueProcessor
    {
        public UserDefinedValueProcessor(Func<long> getValueFunc)
        {
            GetValueFunc = getValueFunc;
        }

        private Func<long> GetValueFunc { get; }

        public override void EndProcessing(SerializerObject s)
        {
            CalculatedValue = GetValueFunc();
            base.EndProcessing(s);
        }
    }
}