﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UbiCanvas.Helpers {
    /// <summary>
    /// Extension methods for <see cref="Enum"/>
    /// </summary>
    public static class EnumExtensions {
        /// <summary>
        /// Gets the first attribute of specified type for the enum value
        /// </summary>
        /// <typeparam name="T">The type of attribute to retrieve</typeparam>
        /// <param name="value">The enum value to get the attribute for</param>
        /// <returns>The attribute instance</returns>
        public static T GetAttribute<T>(this Enum value) where T : Attribute {
            if (value == null)
                return null;

            // Get the member info for the value
            var memberInfo = value.GetType().GetMember(value.ToString());

            // Get the attribute
            var attributes = memberInfo.FirstOrDefault<MemberInfo>()?.GetCustomAttributes<T>(false);

            // Return the first attribute
            return attributes?.FirstOrDefault<T>();
        }

        /// <summary>
        /// Gets the flags from an enum value
        /// </summary>
        /// <param name="e">The enum value</param>
        /// <returns>The flags</returns>
        public static IEnumerable<Enum> GetFlags(this Enum e) => Enum.GetValues(e.GetType()).Cast<Enum>().Where(v => !Equals((int)Convert.ChangeType(v, typeof(int)), 0) && e.HasFlag(v));

		public static T SetFlag<T>(this Enum value, T flag, bool set) {
			Type underlyingType = Enum.GetUnderlyingType(value.GetType());

			// note: AsInt mean: math integer vs enum (not the c# int type)
			dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
			dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
			if (set) {
				valueAsInt |= flagAsInt;
			} else {
				valueAsInt &= ~flagAsInt;
			}

			return (T)valueAsInt;
		}
	}
}