﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UbiCanvas.Helpers;

namespace UbiArt {
	public class CSerializerObjectUnityEditor : CSerializerObject {
		private Dictionary<object, bool> foldouts = new Dictionary<object, bool>();
		public uint position = 0;
		private LocalisationIdDropdown localisationDropdown = null;
		private GenericClassSelectorDropdown genericDropdown = null;
		public override bool IsCustomSerializer => true;

		public const string UnitySerializerID = nameof(CSerializerObjectUnityEditor);

		public static CSerializerObjectUnityEditor Serializer(Context context) {
			var serializer = context.GetStoredObject<CSerializerObjectUnityEditor>(UnitySerializerID);
			if (serializer == null) {
				serializer = new CSerializerObjectUnityEditor(context);
				serializer.flags |= SerializeFlags.Data_Save;
				context.StoreObject<CSerializerObjectUnityEditor>(UnitySerializerID, serializer);
			}
			return serializer;
		}


		public CSerializerObjectUnityEditor(Context context) : base(context) {
		}

		public override SerializerProperties Properties => base.Properties | SerializerProperties.Binary | SerializerProperties.BinSkip; // 0x11

		public override Pointer CurrentPointer => new Pointer(CurrentPosition, null);
		public override long CurrentPosition => 0;
		public override long Length => 0;

		public override object Serialize(object obj, Type type, string name = null, Options options = Options.None) {
			if (name == null) name = $"({type.GetFormattedName()})";
			if (type.IsEnum) {
				obj = EnumField(name, obj, type);
			} else if (Type.GetTypeCode(type) != TypeCode.Object) {
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean: obj = EditorGUILayout.Toggle(name, (bool)obj); break;
					case TypeCode.Byte: obj = MinMaxField<byte>(name, (byte)obj); break;
					case TypeCode.Char: obj = (char)EditorGUILayout.IntField(name, (char)obj); break;
					case TypeCode.String: obj = EditorGUILayout.TextField(name, (string)obj); break;
					case TypeCode.Single: obj = MinMaxField<float>(name, (float)obj); break;
					case TypeCode.Double: obj = MinMaxField<double>(name, (double)obj); break;
					case TypeCode.UInt16: obj = MinMaxField<ushort>(name, (ushort)obj); break;
					case TypeCode.UInt32: obj = MinMaxField<uint>(name, (uint)obj); break;
					case TypeCode.UInt64: obj = MinMaxField<ulong>(name, (ulong)obj); break;
					case TypeCode.Int16: obj = MinMaxField<short>(name, (short)obj); break;
					case TypeCode.Int32: obj = MinMaxField<int>(name, (int)obj); break;
					case TypeCode.Int64: obj = MinMaxField<long>(name, (long)obj); break;
					default: throw new Exception("Unsupported TypeCode " + Type.GetTypeCode(type));
				}
			} else if(type == typeof(Angle)) {
				Angle a = (Angle)obj;
				a = AngleField(name, a);
				obj = a;
			} else if (type == typeof(AngleAmount)) {
				AngleAmount a = (AngleAmount)obj;
				a = (AngleAmount)AngleField(name, a);
				obj = a;
			} else if (type == typeof(PathRef)) {
				PathRef p = (PathRef)obj;
				DrawPathRef(name, ref p);
				obj = p;
			} else if (type == typeof(Path)) {
				Path p = (Path)obj;
				DrawPath(name, ref p);
				obj = p;
			} else if (type == typeof(ITF.ObjectPath)) {
				ITF.ObjectPath p = (ITF.ObjectPath)obj;
				DrawObjectPath(name, ref p);
				obj = p;
			} else if (type == typeof(StringID)) {
				StringID sid = (StringID)obj;
				DrawStringID(name, ref sid);
				obj = sid;
			} else if (type == typeof(LocalisationId)) {
				LocalisationId locId = (LocalisationId)obj;
				DrawLocId(name, ref locId);
				obj = locId;
			} else if (type == typeof(Vec2d)) {
				obj = Vec2dField(name, (Vec2d)obj);
			} else if (type == typeof(Vec3d)) {
				obj = Vec3dField(name, (Vec3d)obj);
			} else if (type == typeof(Vec4d)) {
				obj = (Vec4d)(EditorGUILayout.Vector4Field(name, ((Vec4d)obj ?? new Vec4d()).GetUnityVector()).GetUbiArtVector());
			} else if (type == typeof(Color)) {
				obj = (Color)(EditorGUILayout.ColorField(name, ((Color)obj ?? new Color()).GetUnityColor()).GetUbiArtColor());
			} else if (type == typeof(ColorInteger)) {
				obj = (ColorInteger)(EditorGUILayout.ColorField(name, ((ColorInteger)obj ?? new ColorInteger()).GetUnityColor()).GetUbiArtColorInteger());
			} else if (type == typeof(CString)) {
				obj = new CString(EditorGUILayout.TextField(name, ((CString)obj ?? new CString()).str));
			} else if (type == typeof(BasicString)) {
				obj = new BasicString(EditorGUILayout.TextField(name, ((BasicString)obj ?? new BasicString()).str));
			} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Generic<>)){
				var t = type.GetGenericArguments()[0];
				IGeneric gen = (IGeneric)obj;
				DrawGenericClassSelector(name, ref gen, type, t);
				obj = gen;
			} else {
				if (obj == null) {
					var ctor = type.GetConstructor(Type.EmptyTypes);
					if (ctor == null) {
						throw new Exception("Constructor is null");
					}
					obj = ctor.Invoke(new object[] { });
				}
				if (obj is ICSerializable) {
					if (!foldouts.ContainsKey(obj)) {
						foldouts[obj] = false;
					}
					foldouts[obj] = EditorGUILayout.Foldout(foldouts[obj], name, true);
					if (foldouts[obj]) {
						EditorGUI.indentLevel++;
						IncreaseLevel();
						((ICSerializable)obj).Serialize(this, name);
						DecreaseLevel();
						EditorGUI.indentLevel--;
					}
				}
			}
			return obj;
		}

		public override T SerializeGeneric<T>(T obj, Type type = null, string name = null, int? index = null) {
			object obj2 = Serialize(obj, type ?? typeof(T), name: name);
			return (T)obj2;
		}

		public override byte[] SerializeBytes(byte[] obj, long count) {
			//obj = reader.ReadBytes(numBytes);
			return obj;
		}

		public void DrawPath(string name, ref Path p) {
			if (p == null) p = new Path();
			//EditorGUILayout.PrefixLabel(name);
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			//texPreviewRect = EditorGUI.PrefixLabel(texPreviewRect, new GUIContent(name));
			string fullPath = p.FullPath;
			//var indent = EditorGUI.indentLevel;
			//EditorGUI.indentLevel = 0;
			string newPath = EditorGUI.TextField(rect, new GUIContent(name), fullPath);
			//EditorGUI.indentLevel = indent;
			if (newPath != fullPath) {
				if(string.IsNullOrEmpty(newPath)) newPath = null;
				p = new Path(newPath);
			}
		}
		public Angle AngleField(string name, Angle value) {
			if (value == null) value = new Angle();
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			var controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, new GUIContent(name));

			var indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var rects = UnityWindow.DivideRectHorizontally(rect, 2, spacing: 16f);
			float val = value.angle;
			val = MinMaxField<float>("rad", value.angle, rectToUse: rects[0], labelWidth: 32);
			if (val != value.angle) {
				value.angle = val;
			}
			val = MinMaxField<float>("deg", value.EulerAngle, rectToUse: rects[1], labelWidth: 32);
			if (val != value.EulerAngle) {
				value.EulerAngle = val;
			}

			EditorGUI.indentLevel = indentLevel;

			return value;
		}
		public Vec2d Vec2dField(string name, Vec2d value) {
			if(value == null) value = new Vec2d();
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			var controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, new GUIContent(name));

			var indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var rects = UnityWindow.DivideRectHorizontally(rect, 2, spacing: 16f);
			value.x = MinMaxField<float>("X", value.x, rectToUse: rects[0], labelWidth: 16);
			value.y = MinMaxField<float>("Y", value.y, rectToUse: rects[1], labelWidth: 16);

			EditorGUI.indentLevel = indentLevel;

			return value;
		}
		public Vec3d Vec3dField(string name, Vec3d value) {
			if (value == null) value = new Vec3d();
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			var controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, new GUIContent(name));

			var indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var rects = UnityWindow.DivideRectHorizontally(rect, 3, spacing: 16f);
			//var rects = UnityWindow.DivideRectHorizontally(rect, 4, spacing: 16f);
			value.x = MinMaxField<float>("X", value.x, rectToUse: rects[0], labelWidth: 16);
			value.y = MinMaxField<float>("Y", value.y, rectToUse: rects[1], labelWidth: 16);
			value.z = MinMaxField<float>("Z", value.z, rectToUse: rects[2], labelWidth: 16);

			/*var rot = MinMaxField<float>("Rot", 0, rectToUse: rects[3], labelWidth: 16);
			if (rot != 0) {
				var value2d = new Vec2d(value.x, value.y).Rotate(rot);
				value.x = value2d.x;
				value.y = value2d.y;
				rot = 0;
			}*/

			EditorGUI.indentLevel = indentLevel;

			return value;
		}
		public static UnityEngine.Color MinMaxHighlightColor = UnityEngine.Color.cyan;
		public T MinMaxField<T>(string name, T value, Rect? rectToUse = null, bool prefixLabel = true, float? labelWidth = null) {
			Rect rect = rectToUse ?? EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			if (prefixLabel) {
				if (labelWidth.HasValue) {
					Rect labelRect = new Rect(rect.x, rect.y, labelWidth.Value, rect.height);
					rect = new Rect(rect.x + labelWidth.Value, rect.y, rect.width - labelWidth.Value, rect.height);
					EditorGUI.LabelField(labelRect, name, EditorStyles.miniLabel);
				} else {
					rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));
				}
			}
			var indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			object maxValue = null;
			object minValue = null;

			switch (Type.GetTypeCode(typeof(T))) {
				// Signed: min & max
				case TypeCode.SByte:
					minValue = sbyte.MinValue;
					maxValue = sbyte.MaxValue;
					break;
				case TypeCode.Int16:
					maxValue = short.MaxValue;
					minValue = short.MinValue;
					break;
				case TypeCode.Int32:
					maxValue = int.MaxValue;
					minValue = int.MinValue;
					break;
				case TypeCode.Int64:
					minValue = long.MinValue;
					maxValue = long.MaxValue;
					break;

				// Unsigned: only max
				case TypeCode.Byte:
					maxValue = byte.MaxValue;
					break;
				case TypeCode.UInt16:
					maxValue = ushort.MaxValue;
					break;
				case TypeCode.UInt32:
					maxValue = uint.MaxValue;
					break;
				case TypeCode.UInt64:
					maxValue = ulong.MaxValue;
					break;

				// Floating point
				case TypeCode.Single:
					maxValue = float.MaxValue;
					minValue = float.MinValue;
					break;
				case TypeCode.Double:
					minValue = double.MinValue;
					maxValue = double.MaxValue;
					break;
				default:
					break;
			}
			bool isMin = minValue != null && value.Equals(minValue);
			bool isMax = maxValue != null && value.Equals(maxValue);

			if (minValue != null || maxValue != null) {
				var backgroundColor = GUI.backgroundColor;
				// Add a dropdown button
				if (isMin || isMax) {
					GUI.backgroundColor = MinMaxHighlightColor;
				}
				string valueText = value.ToString();
				if(isMax) valueText = "MAX";
				if(isMin) valueText = "MIN";
				string ogValueText = valueText;
				valueText = EditorGUI.DelayedTextField(rect, valueText);
				GUI.backgroundColor = backgroundColor;
				if (valueText != ogValueText) {
					if (valueText == "MIN" && minValue != null) {
						value = (T)minValue;
					} else if (valueText == "MAX" && maxValue != null) {
						value = (T)maxValue;
					} else {
						switch (Type.GetTypeCode(typeof(T))) {
							case TypeCode.Int32:
								if (int.TryParse(valueText, out int i)) value = (T)(object)i;
								break;
							case TypeCode.UInt32:
								if (uint.TryParse(valueText, out uint ui)) value = (T)(object)ui;
								break;
							case TypeCode.Single:
								if (float.TryParse(valueText, out float f)) value = (T)(object)f;
								break;
							default:
								break;
						}
					}
				}
			}
			EditorGUI.indentLevel = indentLevel;
			return value;
		}
		public object EnumField(string name, object obj, Type type, Rect? rectToUse = null, bool prefixLabel = true, float? labelWidth = null) {
			Rect rect = rectToUse ?? EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			if (prefixLabel) {
				if (labelWidth.HasValue) {
					Rect labelRect = new Rect(rect.x, rect.y, labelWidth.Value, rect.height);
					rect = new Rect(rect.x + labelWidth.Value, rect.y, rect.width - labelWidth.Value, rect.height);
					EditorGUI.LabelField(labelRect, name, EditorStyles.miniLabel);
				} else {
					rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));
				}
			}
			var indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;


			var enumWidth = rect.width - (EditorGUIUtility.singleLineHeight * 2);
			var enumRect = new Rect(rect.position, new Vector2(enumWidth, rect.height));
			var numberRect = new Rect(rect.position + new Vector2(enumWidth, 0), new Vector2(rect.width - enumWidth, rect.height));

			if (type.GetCustomAttributes<FlagsAttribute>().Any()) {
				obj = EditorGUI.EnumFlagsField(enumRect, (Enum)obj);
			} else {
				obj = EditorGUI.EnumPopup(enumRect, (Enum)obj);
			}
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Byte: obj = MinMaxField<byte>(name, (byte)obj, rectToUse: numberRect, prefixLabel: false); break;
				case TypeCode.UInt16: obj = MinMaxField<ushort>(name, (ushort)obj, rectToUse: numberRect, prefixLabel: false); break;
				case TypeCode.UInt32: obj = MinMaxField<uint>(name, (uint)obj, rectToUse: numberRect, prefixLabel: false); break;
				case TypeCode.UInt64: obj = MinMaxField<ulong>(name, (ulong)obj, rectToUse: numberRect, prefixLabel: false); break;
				case TypeCode.Int16: obj = MinMaxField<short>(name, (short)obj, rectToUse: numberRect, prefixLabel: false); break;
				case TypeCode.Int32: obj = MinMaxField<int>(name, (int)obj, rectToUse: numberRect, prefixLabel: false); break;
				case TypeCode.Int64: obj = MinMaxField<long>(name, (long)obj, rectToUse: numberRect, prefixLabel: false); break;
			}

			EditorGUI.indentLevel = indentLevel;
			return obj;
		}
		
		public void DrawPathRef(string name, ref PathRef p) {
			if (p == null) p = new PathRef();
			//EditorGUILayout.PrefixLabel(name);
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			//texPreviewRect = EditorGUI.PrefixLabel(texPreviewRect, new GUIContent(name));
			string fullPath = p.FullPath;
			//var indent = EditorGUI.indentLevel;
			//EditorGUI.indentLevel = 0;
			string newPath = EditorGUI.TextField(rect, new GUIContent(name), fullPath);
			//EditorGUI.indentLevel = indent;
			if (newPath != fullPath) {
				if (string.IsNullOrEmpty(newPath)) newPath = null;
				p = new PathRef(newPath);
			}
		}
		public void DrawObjectPath(string name, ref ITF.ObjectPath p) {
			if (p == null) p = new ITF.ObjectPath();
			//EditorGUILayout.PrefixLabel(name);
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			//texPreviewRect = EditorGUI.PrefixLabel(texPreviewRect, new GUIContent(name));
			string fullPath = p.FullPath;
			//var indent = EditorGUI.indentLevel;
			//EditorGUI.indentLevel = 0;
			string newPath = EditorGUI.TextField(rect, new GUIContent(name), fullPath);
			//EditorGUI.indentLevel = indent;
			if (newPath != fullPath) {
				p = new ITF.ObjectPath(newPath);
			}
		}
		public void DrawStringID(string name, ref StringID sid) {
			if (sid == null) sid = new StringID();
			var context = this.Context;
			//EditorGUILayout.PrefixLabel(name);
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var sidWidth = EditorGUIUtility.singleLineHeight * 5;
			var sidRect = new Rect(rect.position, new Vector2(sidWidth, rect.height));
			var strRect = new Rect(rect.position + new Vector2(sidWidth, 0), new Vector2(rect.width - sidWidth, rect.height));

			// Draw StringID UI (hex)
			var curSidHex = $"{sid.stringID:X8}";
			var newSidHex = EditorGUI.DelayedTextField(sidRect, curSidHex);
			if (curSidHex != newSidHex) {
				if (uint.TryParse(newSidHex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out uint newHex)) {
					sid = new StringID(newHex);
				}
			}

			// Draw StringID UI (string)
			bool useString = context.StringCache.ContainsKey(sid);
			if (!useString) {
				if (GUI.Button(strRect, new GUIContent("Use string input"))) {
					sid = new StringID("");
					context.AddToStringCache("");
				}
			} else {
				var curSidStr = context.StringCache.TryGetItem(sid, "");
				var newSidStr = EditorGUI.TextField(strRect, curSidStr);
				if (curSidStr != newSidStr) {
					sid = new StringID(newSidStr);
					context.AddToStringCache(newSidStr);
				}
			}

			EditorGUI.indentLevel = indent;
		}
		public void DrawLocId(string name, ref LocalisationId locId) {
			if (locId == null) locId = new LocalisationId();
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));

			
			int indent = EditorGUI.indentLevel;
			var localisation = Context.Loader.localisation;
			string locIdPreview = locId.IsNull ? "-1 - " : locId.id + " - ";
			if (locId.IsNull) {
				locIdPreview += "None";
			} else if (locId.id == 0) {
				locIdPreview += "Empty";
			} else if (localisation != null && localisation.strings.Count > 0 && localisation.strings[0].ContainsKey(locId)) {
				locIdPreview += localisation.strings[0][locId].text.Replace("\n","\\n");
			} else {
				locIdPreview += "Error";
			}

			EditorGUI.indentLevel = 0;
			if (EditorGUI.DropdownButton(rect, new GUIContent(locIdPreview), FocusType.Passive)) {
				if (localisationDropdown == null) {
					localisationDropdown = new LocalisationIdDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState()) {
						name = "Localisation ID",
						context = Context
					};
				}
				localisationDropdown.rect = rect;
				localisationDropdown.Show(rect);
			}
			if (localisationDropdown != null && localisationDropdown.selection != null && localisationDropdown.rect == rect) {
				locId = localisationDropdown.selection;
				localisationDropdown.selection = null;
			}

			EditorGUI.indentLevel = indent;
		}

		public void DrawGenericClassSelector(string name, ref IGeneric gen, Type genType, Type t) {
			if (gen == null) {
				var ctor = genType.GetConstructor(Type.EmptyTypes);
				if (ctor == null) {
					throw new Exception("Constructor is null");
				}
				gen = (IGeneric)ctor.Invoke(new object[] { });
			}

			if (!foldouts.ContainsKey(gen)) {
				foldouts[gen] = false;
			}
			foldouts[gen] = EditorGUILayout.Foldout(foldouts[gen], name, true);
			if (!foldouts[gen]) return;
			// Increase indent level
			EditorGUI.indentLevel++;
			IncreaseLevel();

			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));

			var genTypeName = t.GetFormattedName();


			int indent = EditorGUI.indentLevel;
			string genPreview;
			if (gen.IsNull) {
				genPreview = "None";
			} else {
				Type type = ObjectFactory.classes[gen.GenericClassName.stringID];
				genPreview = type.GetFormattedName();
			}

			EditorGUI.indentLevel = 0;
			if (EditorGUI.DropdownButton(rect, new GUIContent(genPreview), FocusType.Passive)) {
				if (genericDropdown == null || genericDropdown.type != t || genericDropdown.context != Context) {
					genericDropdown = new GenericClassSelectorDropdown(Context, new UnityEditor.IMGUI.Controls.AdvancedDropdownState()) {
						name = $"Generic<{genTypeName}>",
						type = t
					};
				}
				genericDropdown.rect = rect;
				genericDropdown.Show(rect);
			}
			if (genericDropdown != null && genericDropdown.selection != null && genericDropdown.rect == rect) {
				var newGenID = genericDropdown.selection.Value;
				genericDropdown.selection = null;

				if (newGenID != gen.GenericClassName?.stringID) {
					// Create new object
					/*var newType = ObjectFactory.classes[newGenID];
					if (newType.ContainsGenericParameters) {
						if (!genType.IsGenericType) {
							EditorGUI.indentLevel = indent;
							return; // Don't make this change
						}
						newType = newType.MakeGenericType(genType.GetGenericArguments());
					}*/
					gen.GenericClassName = new StringID(newGenID);
					gen.GenericObject = null;
				}
			}

			EditorGUI.indentLevel = indent;

			if (!gen.IsNull) {
				gen.SerializeObject(this);
			}

			// Decrease indent level
			DecreaseLevel();
			EditorGUI.indentLevel--;

		}

		public override T SerializeGenericPureBinary<T>(T obj, Type type = null, string name = null, int? index = null) {
			return SerializeGeneric<T>(obj, type: type, name: name, index: index);
		}

		// Helper method which returns an object so we can cast it
		protected object SerializePrimitiveAsObject<T>(object obj, string name = null, Options options = Options.None) {
			// Get the type
			var type = typeof(T);

			TypeCode typeCode = Type.GetTypeCode(type);

			if (type.IsEnum) {
				obj = EnumField(name, obj, type);
			} else if (typeCode == TypeCode.Object) {
				if (type == typeof(CString)) {
					obj = new CString(EditorGUILayout.TextField(name, ((CString)obj)?.str));
				} else if (type == typeof(BasicString)) {
					obj = new BasicString(EditorGUILayout.TextField(name, ((BasicString)obj)?.str));
				}
			} else {
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean: obj = EditorGUILayout.Toggle(name, (bool)obj); break;
					case TypeCode.Byte: obj = MinMaxField<byte>(name, (byte)obj); break;
					case TypeCode.Char: obj = (char)EditorGUILayout.IntField(name, (char)obj); break;
					case TypeCode.String: obj = EditorGUILayout.TextField(name, (string)obj); break;
					case TypeCode.Single: obj = MinMaxField<float>(name, (float)obj); break;
					case TypeCode.Double: obj = MinMaxField<double>(name, (double)obj); break;
					case TypeCode.UInt16: obj = MinMaxField<ushort>(name, (ushort)obj); break;
					case TypeCode.UInt32: obj = MinMaxField<uint>(name, (uint)obj); break;
					case TypeCode.UInt64: obj = MinMaxField<ulong>(name, (ulong)obj); break;
					case TypeCode.Int16: obj = MinMaxField<short>(name, (short)obj); break;
					case TypeCode.Int32: obj = MinMaxField<int>(name, (int)obj); break;
					case TypeCode.Int64: obj = MinMaxField<long>(name, (long)obj); break;
					default: throw new Exception("Unsupported TypeCode " + Type.GetTypeCode(type));
				}
			}
			return obj;
		}


		public override T Serialize<T>(T obj, string name = null, int? index = null, Options options = Options.None) {
			T t = (T)SerializePrimitiveAsObject<T>(obj, name: name, options: options);
			return t;
		}

		public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null, int? index = null, Options options = Options.None) {
			// Get the type
			var type = typeof(T);
			if(name == null) name = $"({type.GetFormattedName()})";
			if (type == typeof(Angle)) {
				Angle a = (Angle)(object)obj;
				a = AngleField(name, a);
				obj = (T)(object)a;
			} else if (type == typeof(AngleAmount)) {
				AngleAmount a = (AngleAmount)(object)obj;
				a = (AngleAmount)AngleField(name, a);
				obj = (T)(object)a;
			} else if (type == typeof(PathRef)) {
				PathRef p = (PathRef)(object)obj;
				DrawPathRef(name, ref p);
				obj = (T)(object)p;
			} else if (type == typeof(Path)) {
				Path p = (Path)(object)obj;
				DrawPath(name, ref p);
				obj = (T)(object)p;
			} else if (type == typeof(ITF.ObjectPath)) {
				ITF.ObjectPath p = (ITF.ObjectPath)(object)obj;
				DrawObjectPath(name, ref p);
				obj = (T)(object)p;
			} else if (type == typeof(StringID)) {
				StringID sid = (StringID)(object)obj;
				DrawStringID(name, ref sid);
				obj = (T)(object)sid;
			} else if (type == typeof(LocalisationId)) {
				LocalisationId locId = (LocalisationId)(object)obj;
				DrawLocId(name, ref locId);
				obj = (T)(object)locId;
			} else if (type == typeof(Vec2d)) {
				obj = (T)(object)Vec2dField(name, (Vec2d)(object)obj);
			} else if (type == typeof(Vec3d)) {
				obj = (T)(object)Vec3dField(name, (Vec3d)(object)obj);
			} else if (type == typeof(Vec4d)) {
				obj = (T)(object)(Vec4d)(EditorGUILayout.Vector4Field(name, ((Vec4d)(object)obj ?? new Vec4d()).GetUnityVector()).GetUbiArtVector());
			} else if (type == typeof(Color)) {
				obj = (T)(object)(Color)(EditorGUILayout.ColorField(name, ((Color)(object)obj ?? new Color()).GetUnityColor()).GetUbiArtColor());
			} else if (type == typeof(ColorInteger)) {
				obj = (T)(object)(ColorInteger)(EditorGUILayout.ColorField(name, ((ColorInteger)(object)obj ?? new ColorInteger()).GetUnityColor()).GetUbiArtColorInteger());

			} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Generic<>)) {
				var t = type.GetGenericArguments()[0];
				IGeneric gen = (IGeneric)(object)obj;
				DrawGenericClassSelector(name, ref gen, type, t);
				obj = (T)(object)gen;
			} else {
				if (obj == null) {
					obj = new T();
				}
				if (!foldouts.ContainsKey(obj)) {
					foldouts[obj] = false;
				}
				foldouts[obj] = EditorGUILayout.Foldout(foldouts[obj], name, true);
				if (foldouts[obj]) {
					EditorGUI.indentLevel++;
					IncreaseLevel();
					obj.Serialize(this, name);
					DecreaseLevel();
					EditorGUI.indentLevel--;
				}
			}

			return obj;
		}

		public override void Log(string logString, params object[] args) { }

		protected override void DoEncoded(IStreamEncoder encoder, Action action, Endian? endianness = null, string filename = null) {
			if (action == null)
				throw new ArgumentNullException(nameof(action)); 
			action();
		}

		public override void DoEncrypted(uint[] encryptionKey, Action action, string name = null) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			action();
		}

		public override void DoCompressed(Action action, 
			long? compressedSize = null, long? decompressedSize = null, string name = null) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			action();
		}

		public override void DoEndian(Action action, Endian endianness) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			action();
		}

		public override bool SerializeEditorButton(string name) {
			return GUILayout.Button(name);
		}
	}
}
