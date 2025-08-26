using System;
namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class CharacterDebuggerComponent : ActorComponent {
		public CharacterDebug DebugFlags =
			CharacterDebug.Collider | CharacterDebug.Trajectory | CharacterDebug.Controller | CharacterDebug.Anim |
			CharacterDebug.AI | CharacterDebug.SoundInputs | CharacterDebug.ActorPos | CharacterDebug.States;
		public uint trajectoryPointCount = uint.MaxValue;
		public uint barFrameCount = 120;
		public uint beatFrameCount = 30;
		public Color barColor = Color.Black;
		public Color beatColor = Color.White;
		public Color halfBeatColor = Color.Yellow;
		public Color leftButtonColor = Color.Orange;
		public Color rightButtonColor = Color.Magenta;
		public bool logCurrentAnimation;
		public bool alwaysShowDebug;
		public bool writeLog;
		public uint uint__0;
		public uint uint__1;
		public uint uint__2;
		public uint uint__3;
		public bool bool__9;
		public bool bool__10;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				trajectoryPointCount = s.Serialize<uint>(trajectoryPointCount, name: "trajectoryPointCount");
				barFrameCount = s.Serialize<uint>(barFrameCount, name: "barFrameCount");
				beatFrameCount = s.Serialize<uint>(beatFrameCount, name: "beatFrameCount");
				barColor = s.SerializeObject<Color>(barColor, name: "barColor");
				beatColor = s.SerializeObject<Color>(beatColor, name: "beatColor");
				halfBeatColor = s.SerializeObject<Color>(halfBeatColor, name: "halfBeatColor");
				leftButtonColor = s.SerializeObject<Color>(leftButtonColor, name: "leftButtonColor");
				rightButtonColor = s.SerializeObject<Color>(rightButtonColor, name: "rightButtonColor");
			} else if (s.Settings.Game == Game.VH) {
				uint__0 = s.Serialize<uint>(uint__0, name: "uint__0");
				uint__1 = s.Serialize<uint>(uint__1, name: "uint__1");
				uint__2 = s.Serialize<uint>(uint__2, name: "uint__2");
				uint__3 = s.Serialize<uint>(uint__3, name: "uint__3");
				barColor = s.SerializeObject<Color>(barColor, name: "barColor");
				beatColor = s.SerializeObject<Color>(beatColor, name: "beatColor");
				halfBeatColor = s.SerializeObject<Color>(halfBeatColor, name: "halfBeatColor");
				leftButtonColor = s.SerializeObject<Color>(leftButtonColor, name: "leftButtonColor");
				rightButtonColor = s.SerializeObject<Color>(rightButtonColor, name: "rightButtonColor");
				bool__9 = s.Serialize<bool>(bool__9, name: "bool__9");
				bool__10 = s.Serialize<bool>(bool__10, name: "bool__10");
			} else {
				DebugFlags = s.Serialize<CharacterDebug>(DebugFlags, name: "DebugFlags");
				trajectoryPointCount = s.Serialize<uint>(trajectoryPointCount, name: "trajectoryPointCount");
				barFrameCount = s.Serialize<uint>(barFrameCount, name: "barFrameCount");
				beatFrameCount = s.Serialize<uint>(beatFrameCount, name: "beatFrameCount");
				barColor = s.SerializeObject<Color>(barColor, name: "barColor");
				beatColor = s.SerializeObject<Color>(beatColor, name: "beatColor");
				halfBeatColor = s.SerializeObject<Color>(halfBeatColor, name: "halfBeatColor");
				leftButtonColor = s.SerializeObject<Color>(leftButtonColor, name: "leftButtonColor");
				rightButtonColor = s.SerializeObject<Color>(rightButtonColor, name: "rightButtonColor");
				logCurrentAnimation = s.Serialize<bool>(logCurrentAnimation, name: "logCurrentAnimation");
				alwaysShowDebug = s.Serialize<bool>(alwaysShowDebug, name: "alwaysShowDebug");
				writeLog = s.Serialize<bool>(writeLog, name: "writeLog");
			}
		}
		[Flags]
		public enum CharacterDebug {
			[Serialize("CharacterDebuggerComponent::CharacterDebug_Collider"   )] Collider = 1,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_Trajectory" )] Trajectory = 2,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_Controller" )] Controller = 4,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_Anim"       )] Anim = 8,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_AnimInputs" )] AnimInputs = 16,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_AI"         )] AI = 32,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_SoundInputs")] SoundInputs = 64,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_ActorPos"   )] ActorPos = 128,
			[Serialize("CharacterDebuggerComponent::CharacterDebug_States"     )] States = 256,
		}
		public override uint? ClassCRC => 0xDD1CC543;
	}
}

