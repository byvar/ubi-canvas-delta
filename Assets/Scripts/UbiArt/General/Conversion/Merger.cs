using System;
using System.Linq;
using System.Reflection;

namespace UbiArt {
	public static class Merger {
		public static TTarget Merge<TTarget>(object copyFrom, bool throwExceptions = false, TTarget targetObject = default) where TTarget : new() {
			var flags = BindingFlags.Instance | BindingFlags.Public |
						BindingFlags.NonPublic;
			var targetDic = typeof(TTarget).GetFields(flags)
										   .ToDictionary(f => f.Name);
			var ret = targetObject ?? new TTarget();
			foreach (var f in copyFrom.GetType().GetFields(flags)) {
				if (throwExceptions) {
					if (targetDic.ContainsKey(f.Name)) {
						var targetType = targetDic[f.Name].FieldType;
						targetDic[f.Name].SetValue(ret, ConvertType(f.GetValue(copyFrom), f.FieldType, targetType));
					} else
						throw new InvalidOperationException(string.Format(
							"The field “{0}” has no corresponding field in the type “{1}”.",
							f.Name, typeof(TTarget).FullName));
				} else {
					try {
						if (targetDic.ContainsKey(f.Name)) {
							var targetType = targetDic[f.Name].FieldType;
							targetDic[f.Name].SetValue(ret, ConvertType(f.GetValue(copyFrom), f.FieldType, targetType));
						}
					} catch (Exception) { }
				}
			}
			return ret;
		}

		public static object MergeGeneric(object copyFrom, Type type, bool throwExceptions = false, object targetObject = default) {
			var flags = BindingFlags.Instance | BindingFlags.Public |
						BindingFlags.NonPublic;
			var targetDic = type.GetFields(flags).ToDictionary(f => f.Name);
			var ret = targetObject ?? Activator.CreateInstance(type);
			foreach (var f in copyFrom.GetType().GetFields(flags)) {
				if (throwExceptions) {
					if (targetDic.ContainsKey(f.Name)) {
						var targetType = targetDic[f.Name].FieldType;
						targetDic[f.Name].SetValue(ret, ConvertType(f.GetValue(copyFrom), f.FieldType, targetType));
					} else
						throw new InvalidOperationException(string.Format(
							"The field “{0}” has no corresponding field in the type “{1}”.",
							f.Name, type.FullName));
				} else {
					try {
						if (targetDic.ContainsKey(f.Name)) {
							var targetType = targetDic[f.Name].FieldType;
							targetDic[f.Name].SetValue(ret, ConvertType(f.GetValue(copyFrom), f.FieldType, targetType));
						}
					} catch (Exception) { }
				}
			}
			return ret;
		}

		private static object ConvertType(object value, Type sourceType, Type targetType) {
			if (targetType.IsEnum && sourceType.IsEnum && sourceType != targetType) {
				var newvalue = Convert.ChangeType(value, typeof(int));
				newvalue = Convert.ChangeType(newvalue, targetType);
				return newvalue;
			} else if (sourceType == typeof(int) && targetType == typeof(bool)) {
				return (int)value != 0;
			} else if (sourceType == typeof(bool) && targetType == typeof(int)) {
				return (bool)value ? 1 : 0;
			}
			return value;
		}
	}
}
