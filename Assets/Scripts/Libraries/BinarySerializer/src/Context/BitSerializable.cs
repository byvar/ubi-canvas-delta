using System;
using System.Runtime.Serialization;

namespace BinarySerializer
{
    /// <summary>
    /// Base type for bit serializable structs
    /// </summary>
    public abstract class BitSerializable 
    {
        /// <summary>
        /// Indicates if it's the first time the struct is loaded
        /// </summary>
        [IgnoreDataMember]
        protected bool IsFirstLoad { get; set; } = true;

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
        /// The starting bit of the struct
        /// </summary>
        public long BitOffset { get; protected set; }

        /// <summary>
        /// The struct size in bits
        /// </summary>
        [IgnoreDataMember]
        public virtual long Size { get; protected set; }

        /// <summary>
        /// Indicates whether this object should be logged on one line
        /// </summary>
        [IgnoreDataMember]
        public virtual bool UseShortLog => false;

        /// <summary>
        /// The string for displaying this object on one line
        /// </summary>
        [IgnoreDataMember]
        public virtual string ShortLog => ToString();

        /// <summary>
        /// Initializes the struct from an offset
        /// </summary>
        /// <param name="offset">The offset the struct is located at</param>
        /// <param name="bitOffset">The bit offset for this struct</param>
        public void Init(Pointer offset, long bitOffset) 
        {
            Offset = offset ?? throw new ArgumentNullException(nameof(offset));
            BitOffset = bitOffset;
            if (Context != null && offset.Context != Context) 
                OnChangeContext(Context, offset.Context);
            Context = offset.Context;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="b">The serializer object</param>
        public abstract void SerializeImpl(BitSerializerObject b);

        /// <summary>
        /// Serializes the data struct
        /// </summary>
        /// <param name="b">The serializer</param>
        public void Serialize(BitSerializerObject b) 
        {
            OnPreSerialize(b);
            SerializeImpl(b);
            Size = b.Position - BitOffset;
            OnPostSerialize(b);
            IsFirstLoad = false;
        }

        protected virtual void OnPreSerialize(BitSerializerObject b) { }
        protected virtual void OnPostSerialize(BitSerializerObject b) { }

        protected virtual void OnChangeContext(Context oldContext, Context newContext) { }
    }
}