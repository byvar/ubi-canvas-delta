#nullable enable
using System;

namespace BinarySerializer
{
    /// <summary>
    /// Flags for defining how a <see cref="BinaryProcessor"/> should behave
    /// </summary>
    [Flags]
    public enum BinaryProcessorFlags
    {
        None = 0,

        /// <summary>
        /// The callbacks <see cref="BinaryProcessor.BeginProcessing"/> and <see cref="BinaryProcessor.EndProcessing"/>
        /// are used
        /// </summary>
        Callbacks = 1 << 0,

        /// <summary>
        /// Binary data is being processed in <see cref="BinaryProcessor.ProcessBytes"/>
        /// </summary>
        ProcessBytes = 1 << 1,

        /// <summary>
        /// Binary data is being modified in <see cref="BinaryProcessor.ProcessBytes"/>
        /// </summary>
        ModifyBytes = 1 << 2 | ProcessBytes,
    }
}