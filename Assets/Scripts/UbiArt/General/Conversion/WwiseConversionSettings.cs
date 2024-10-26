﻿using System;
using System.Collections.Generic;
using System.Linq;
using UbiArt.ITF;

namespace UbiArt {
	public class WwiseConversionSettings {
		public Dictionary<StringID, Event> Events { get; set; } = new Dictionary<StringID, Event>();

		public HashSet<StringID> UsedEvents { get; private set; } = new HashSet<StringID>();

		public Dictionary<long, Attenuation> Attenuations { get; set; } = new Dictionary<long, Attenuation>();

		public Dictionary<SoundDescriptor_Template, List<SoundDescriptor_Template>> SoundDescriptorMapping { get; set; } = new Dictionary<SoundDescriptor_Template, List<SoundDescriptor_Template>>();

		public Dictionary<SoundDescriptor_Template, ExtraConversionFunction> ExtraConversionFunctions { get; set; } = new Dictionary<SoundDescriptor_Template, ExtraConversionFunction>();

		//public PlaySound_evtTemplate ConvertWwiseEvent(PlayWwise_evtTemplate wwiseEvt) {
		//}

		public delegate void ExtraConversionFunction(SoundDescriptor_Template template, WwiseConversionSettings.Event wwiseEvent);

		public void ResetUsedEvents() {
			UsedEvents.Clear();
		}

		public void RegisterExtraConversionFunction(SoundDescriptor_Template tpl, ExtraConversionFunction action) {
			if(tpl == null) return;
			ExtraConversionFunctions[tpl] = action;
		}
		public void RegisterExtraConversionFunction(SoundComponent_Template tpl, ExtraConversionFunction action) {
			if(tpl?.soundList == null) return;
			foreach (var snd in tpl.soundList) {
				RegisterExtraConversionFunction(snd, action);
			}
		}
		public void RegisterExtraConversionFunction(Actor_Template tpl, ExtraConversionFunction action) {
			var component = tpl?.GetComponent<SoundComponent_Template>();
			if(component == null) return;
			RegisterExtraConversionFunction(component, action);
		}

		public SoundParams GetParams(Action action, bool? playAfterDestroy) {
			if(action == null) return null;

			var sp = new SoundParams() {
				loop = action.IsLoop ? 1 : 0,
				fadeOutTime = 0.01f,
				playMode = action.IsSequence ? SoundParams.PlayMode.Sequence : (action.AvoidRepeat ? SoundParams.PlayMode.RandomRememberLast : SoundParams.PlayMode.Random),
				playMode2 = action.IsSequence ? SoundParams.PlayMode2.Sequence : (action.AvoidRepeat ? SoundParams.PlayMode2.RandomRememberLast : SoundParams.PlayMode2.Random),
				/*modifiers = new CArrayO<Generic<SoundModifier>>() {
					new Generic<SoundModifier>(new SpatializedPanning() {
						sizeOf = 12,
						widthMin = 0.5f,
						widthMax = 1.8f
					}),
					new Generic<SoundModifier>(new ScreenRollOff() {
						sizeOf = 16,
						distanceMin = 0.5f,
						distanceMax = 2f,
					}),
				}*/
			};
			if (playAfterDestroy.HasValue && playAfterDestroy == true) {
				sp.fadeOutTime = 5f;
				//fadeOutTime = playAfterDestroy == true ? 5f : (playAfterDestroy == false ? (action.IsLoop ? 0.01f : 5f,
			}
			if (action.Attenuation.HasValue) {
				var att = Attenuations[action.Attenuation.Value];
				sp.modifiers = new CArrayO<Generic<SoundModifier>>(att.GetModifiers().Select(m => new Generic<SoundModifier>(m)).ToArray());
			} else {
				// TODO
				sp.modifiers = new CArrayO<Generic<SoundModifier>>() {
					new Generic<SoundModifier>(new SpatializedPanning() {
						widthMin = 100,
						widthMax = 1000
					}),
					new Generic<SoundModifier>(new ScreenRollOff() {
						distanceMin = 100,
						distanceMax = 1000,
					}),
				};
			}

			// Pitch calculation
			float pitchMin = 0f, pitchMax = 0f;
			float CalculateLegendsPitch(float value) => (float)Math.Pow(2, value / 1200f); // wwise pitch is in cents
			foreach (var prop in action.Properties) {
				if (prop.Name == "0x02 [Pitch]") {
					pitchMin = prop.Value;
					pitchMax = prop.Value;
				}
			}
			foreach (var prop in action.RangedProperties) {
				if (prop.Name == "0x02 [Pitch]") {
					pitchMin += prop.Min;
					pitchMax += prop.Max;
				}
			}
			sp.randomPitchMin = CalculateLegendsPitch(pitchMin);
			sp.randomPitchMax = CalculateLegendsPitch(pitchMax);

			// Volume calculation
			float volMin = 0f, volMax = 0f;

			foreach (var prop in action.Properties) {
				if (prop.Name == "0x00 [Volume]") {
					volMin = prop.Value;
					volMax = prop.Value;
				}
			}
			foreach (var prop in action.RangedProperties) {
				if (prop.Name == "0x00 [Volume]") {
					volMin += prop.Min;
					volMax += prop.Max;
				}
			}
			//volMin += 8;
			//volMax += 8; // Volume difference should be about this because Adventures is much quieter
			sp.randomVolMin = new Volume(volMin);
			sp.randomVolMax = new Volume(volMax);
			return sp;
		}

		public List<SoundDescriptor_Template> CreateSoundDescriptorsFromWwiseDescriptor(SoundDescriptor_Template tpl) {
			var guid = tpl.WwiseEventGUID;
			if (guid == null || guid.IsNull || !Events.ContainsKey(guid)) return new List<SoundDescriptor_Template>();

			if (SoundDescriptorMapping.ContainsKey(tpl)) return SoundDescriptorMapping[tpl];

			var ev = Events[guid];
			if (!UsedEvents.Contains(guid)) UsedEvents.Add(guid);
			List<SoundDescriptor_Template> soundDescriptors = new List<SoundDescriptor_Template>();
			foreach (var act in ev.Actions) {
				if(act.IsStop) continue;
				var newTPL = new SoundDescriptor_Template() {
					name = new StringID((uint)act.ID),
					//maxInstances = 5, // or leave it be
					_params = GetParams(act, tpl.soundPlayAfterdestroy),
					WwiseEventGUID = null, // Mark as processed
					category = act.Bus,
					files = new CListO<Path>(act.Sounds.Select(f => new Path(f.Filename)).ToList()),
					soundPlayAfterdestroy = tpl.soundPlayAfterdestroy,
					limitModeEnum = act.KillNewest ? LimiterDef.LimiterMode.RejectNew : LimiterDef.LimiterMode.StopOldest,
					maxInstances = (uint)(act.MaxInstances ?? uint.MaxValue),
					//limitCategory = new StringID((uint)act.ID)
				};
				newTPL.soundPlayAfterdestroy = newTPL.soundPlayAfterdestroy || (newTPL._params?.loop ?? 0) == 0;
				if (ExtraConversionFunctions.ContainsKey(tpl)) {
					ExtraConversionFunctions[tpl]?.Invoke(newTPL, ev);
				}
				soundDescriptors.Add(newTPL);
			}
			SoundDescriptorMapping[tpl] = soundDescriptors;

			return soundDescriptors;
		}

		public void ConvertSoundDescriptor(SoundDescriptor_Template tpl) {
			var evtGUID = tpl.WwiseEventGUID;
			if(evtGUID == null || evtGUID.IsNull)
				return;
			else
			{
				if(!SoundDescriptorMapping.ContainsKey(tpl))
					CreateSoundDescriptorsFromWwiseDescriptor(tpl);
				tpl.WwiseEventGUID = null; // Mark as processed
			}
		}
		public PlaySound_evtTemplate ConvertPlaySound_evtTemplate(PlayWwise_evtTemplate wwiseEvent) {
			var evtGUID = wwiseEvent.WwiseEventGUID;
			var tpl = new PlaySound_evtTemplate();
			if (evtGUID == null || evtGUID.IsNull)
				return tpl;
			else {
				// Sadly not a great conversion...
				if (Events.ContainsKey(evtGUID) && !(Events[evtGUID].Actions?.FirstOrDefault()?.IsLoop ?? false)) {
					var entry = Events[evtGUID];
					if (entry.Actions.Count > 1) {
						wwiseEvent?.UbiArtContext?.SystemLogger?.LogDebug($"Converting PlayWwise_evtTemplate with GUID {evtGUID}: More than 1 action!");
					}
					var action = entry.Actions.FirstOrDefault(act => !act.IsStop);
					if (action != null) {
						if (!UsedEvents.Contains(evtGUID)) UsedEvents.Add(evtGUID);
						tpl.Params = GetParams(action, null);
						tpl.Category2 = action.Bus;
						tpl.Sound = new Path(action.Sounds.FirstOrDefault()?.Filename);
					}
					return tpl;
				} else {
					return tpl;
				}
			}
		}

		public class Event {
			public WwiseItem Item { get; set; }
			public string Name { get; set; }
			public List<Action> Actions { get; set; } = new List<Action>();
		}
		public class Action {
			public long ID { get; set; }
			public long? ExtID { get; set; }
			public bool IsLoop { get; set; }
			public bool IsStop { get; set; }
			public StringID Bus { get; set; }
			public List<Sound> Sounds { get; set; } = new List<Sound>();
			public List<Property> Properties { get; set; } = new List<Property>();
			public List<RangedProperty> RangedProperties { get; set; } = new List<RangedProperty>();

			public bool KillNewest { get; set; }
			public bool IsSequence { get; set; }
			public long? MaxInstances { get; set; }
			public bool AvoidRepeat { get; set; }
			public long? Attenuation { get; set; }
		}
		public class Sound {
			public string Filename { get; set; }
		}
		public class Property {
			public string Name { get; set; }
			public float Value { get; set; }
		}
		public class RangedProperty {
			public string Name { get; set; }
			public float Min { get; set; }
			public float Max { get; set; }
		}
		public class Attenuation {
			public Curve Volume { get; set; }
			public Curve Spread { get; set; }

			private SoundModifier[] Modifiers { get; set; }
			public SoundModifier[] GetModifiers() {
				if (Modifiers == null) {
					var modifiers = new List<SoundModifier>();

					if (Spread != null) {
						var multi = Spread.Points.Max(p => p.X) / 18f;
						modifiers.Add(new SpatializedPanning() {
							widthMin = 0.5f * multi,
							widthMax = 2f * multi
						});
					}
					if (Volume != null) {
						var multi = Volume.Points.Max(p => p.X) / 18f;
						modifiers.Add(new ScreenRollOff() {
							distanceMin = 0.5f * multi,
							distanceMax = 2f * multi
						});
					}

					Modifiers = modifiers.ToArray();
				}
				return Modifiers;

			}
		}
		public class Curve {
			public CurvePoint[] Points { get; set; }
		}
		public class CurvePoint {
			public float X { get; set; }
			public float Y { get; set; }
		}
	}
}
