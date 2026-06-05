using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace BinarySerializer
{
    // TODO: Enabling nullable in this class is a bit difficult due to the Context and Offset properties. These will be null
    //       by default, such as if the class is instantiated manually. However when in a SerializeImpl method we can know
    //       that they won't be null.
    //       Best solution is probably to throw if Context or Offset are null in the getter and then introduce a bool
    //       to check if the class has been initialized (i.e. they are not null). This would however be a major breaking
    //       change, so I've yet to do it.
    //       Same applies for BitSerializable.

    /// <summary>
    /// Base type for serializable structs
    /// </summary>
    public abstract class BinarySerializable 
    {
        /// <summary>
        /// The context
        /// </summary>
        [IgnoreDataMember]
        public Context Context { get; protected set; }
        
        /// <summary>
        /// The struct offset
        /// </summary>
        [IgnoreDataMember]
        public Pointer Offset { get; protected set; }

        /// <summary>
        /// The struct size
        /// </summary>
        [IgnoreDataMember]
        public virtual long SerializedSize { get; protected set; }

        [IgnoreDataMember]
        [MemberNotNullWhen(true, nameof(Context), nameof(Offset))]
        public bool IsInitialized => Context != null && Offset != null;

        /// <summary>
        /// Initializes the struct from an offset
        /// </summary>
        /// <param name="offset">The offset the struct is located at</param>
        public void Init(Pointer offset) 
        {
            Offset = offset ?? throw new ArgumentNullException(nameof(offset));
            if (Context != null && offset.Context != Context) 
                OnChangeContext(Context, offset.Context);
            Context = offset.Context;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public abstract void SerializeImpl(SerializerObject s);

        /// <summary>
        /// Serializes the data struct
        /// </summary>
        /// <param name="s">The serializer</param>
        public void Serialize(SerializerObject s)
        {
            if (!IsInitialized)
                throw new ContextException("Can't serialize an object before it has been initialized");

            OnPreSerialize(s);
            SerializeImpl(s);
            SerializedSize = s.CurrentAbsoluteOffset - Offset.AbsoluteOffset;
            OnPostSerialize(s);
        }

        protected virtual void OnPreSerialize(SerializerObject s) { }
        protected virtual void OnPostSerialize(SerializerObject s) { }

        /// <summary>
        /// Recalculates the <see cref="SerializedSize"/> value of the object
        /// </summary>
        public virtual void RecalculateSize()
        {
            lock (Context._threadLock)
            {
                // Create a serializer for calculating the size
                using var s = new SizeCalculationSerializer(Context);

                // Go to the offset of the object
                s.Goto(Offset);

                // Serialize the object
                Serialize(s);
            }
        }

        protected virtual void OnChangeContext(Context oldContext, Context newContext) { }
    }
}