namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class UIItemBasic_Template : UIItem_Template {
		public float blinkScaleMin;
		public float blinkScale;
		public float blinkPeriod;
		public float blinkMinBlendSpeed;
		public float hightlightAlphaMin;
		public float hightlightAlpha;
		public float hightlightPeriod;
		public float hightlightMinBlendSpeed;
		public float activatingScale;
		public float activatingDuration;
		public uint activatingRebound;
		public StringID animUnselected;
		public StringID animSelected;
		public StringID animLocked;
		public bool needSyncBlink;
		public StringID fontEffectUnselected;
		public StringID fontEffectSelected;
		public Color colorFactorSelected;
		public Color colorFactorLocked;
		public Color colorFactorUnselected;
		public float textLockedAlpha;
		public float animLockedAlpha;
		public float colorBlendTime;
		public float ScaleUnselected;
		public Angle uvRotationSpeedSelected;
		public Angle uvRotationSpeedUnselected;
		public StringID actorIconSelected;
		public StringID actorIconUnselected;
		public bool inverseShadowState;
		public Vec2d actorIconOffset;

		public StringID StringID__29;
		public StringID StringID__30;
		public StringID StringID__31;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion <= EngineVersion.RO) {
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				if(this is UISliderComponent_Template) return;
				blinkScaleMin = s.Serialize<float>(blinkScaleMin, name: "blinkScaleMin");
				blinkScale = s.Serialize<float>(blinkScale, name: "blinkScale");
				blinkPeriod = s.Serialize<float>(blinkPeriod, name: "blinkPeriod");
				blinkMinBlendSpeed = s.Serialize<float>(blinkMinBlendSpeed, name: "blinkMinBlendSpeed");
				hightlightAlphaMin = s.Serialize<float>(hightlightAlphaMin, name: "hightlightAlphaMin");
				hightlightAlpha = s.Serialize<float>(hightlightAlpha, name: "hightlightAlpha");
				hightlightPeriod = s.Serialize<float>(hightlightPeriod, name: "hightlightPeriod");
				hightlightMinBlendSpeed = s.Serialize<float>(hightlightMinBlendSpeed, name: "hightlightMinBlendSpeed");
				activatingScale = s.Serialize<float>(activatingScale, name: "activatingScale");
				activatingDuration = s.Serialize<float>(activatingDuration, name: "activatingDuration");
				activatingRebound = s.Serialize<uint>(activatingRebound, name: "activatingRebound");
				animUnselected = s.SerializeObject<StringID>(animUnselected, name: "animUnselected");
				animSelected = s.SerializeObject<StringID>(animSelected, name: "animSelected");
				fontEffectUnselected = s.SerializeObject<StringID>(fontEffectUnselected, name: "fontEffectUnselected");
				fontEffectSelected = s.SerializeObject<StringID>(fontEffectSelected, name: "fontEffectSelected");
				colorFactorSelected = s.SerializeObject<Color>(colorFactorSelected, name: "colorFactorSelected");
				colorFactorUnselected = s.SerializeObject<Color>(colorFactorUnselected, name: "colorFactorUnselected");
				colorBlendTime = s.Serialize<float>(colorBlendTime, name: "colorBlendTime");
				uvRotationSpeedSelected = s.SerializeObject<Angle>(uvRotationSpeedSelected, name: "uvRotationSpeedSelected");
				uvRotationSpeedUnselected = s.SerializeObject<Angle>(uvRotationSpeedUnselected, name: "uvRotationSpeedUnselected");
				actorIconSelected = s.SerializeObject<StringID>(actorIconSelected, name: "actorIconSelected");
				actorIconUnselected = s.SerializeObject<StringID>(actorIconUnselected, name: "actorIconUnselected");
				actorIconOffset = s.SerializeObject<Vec2d>(actorIconOffset, name: "actorIconOffset");
				inverseShadowState = s.Serialize<bool>(inverseShadowState, name: "inverseShadowState");
			} else if (s.Settings.Game == Game.VH) {
				blinkScaleMin = s.Serialize<float>(blinkScaleMin, name: "blinkScaleMin");
				blinkScale = s.Serialize<float>(blinkScale, name: "blinkScale");
				blinkPeriod = s.Serialize<float>(blinkPeriod, name: "blinkPeriod");
				blinkMinBlendSpeed = s.Serialize<float>(blinkMinBlendSpeed, name: "blinkMinBlendSpeed");
				hightlightAlphaMin = s.Serialize<float>(hightlightAlphaMin, name: "hightlightAlphaMin");
				hightlightAlpha = s.Serialize<float>(hightlightAlpha, name: "hightlightAlpha");
				hightlightPeriod = s.Serialize<float>(hightlightPeriod, name: "hightlightPeriod");
				hightlightMinBlendSpeed = s.Serialize<float>(hightlightMinBlendSpeed, name: "hightlightMinBlendSpeed");
				activatingScale = s.Serialize<float>(activatingScale, name: "activatingScale");
				activatingDuration = s.Serialize<float>(activatingDuration, name: "activatingDuration");
				activatingRebound = s.Serialize<uint>(activatingRebound, name: "activatingRebound");
				animUnselected = s.SerializeObject<StringID>(animUnselected, name: "animUnselected");
				animSelected = s.SerializeObject<StringID>(animSelected, name: "animSelected");
				animLocked = s.SerializeObject<StringID>(animLocked, name: "animLocked");
				needSyncBlink = s.Serialize<bool>(needSyncBlink, name: "needSyncBlink");
				fontEffectUnselected = s.SerializeObject<StringID>(fontEffectUnselected, name: "fontEffectUnselected");
				fontEffectSelected = s.SerializeObject<StringID>(fontEffectSelected, name: "fontEffectSelected");
				colorFactorSelected = s.SerializeObject<Color>(colorFactorSelected, name: "colorFactorSelected");
				colorFactorLocked = s.SerializeObject<Color>(colorFactorLocked, name: "colorFactorLocked");
				colorFactorUnselected = s.SerializeObject<Color>(colorFactorUnselected, name: "colorFactorUnselected");
				textLockedAlpha = s.Serialize<float>(textLockedAlpha, name: "textLockedAlpha");
				animLockedAlpha = s.Serialize<float>(animLockedAlpha, name: "animLockedAlpha");
				colorBlendTime = s.Serialize<float>(colorBlendTime, name: "colorBlendTime");
				ScaleUnselected = s.Serialize<float>(ScaleUnselected, name: "ScaleUnselected");
				uvRotationSpeedSelected = s.SerializeObject<Angle>(uvRotationSpeedSelected, name: "uvRotationSpeedSelected");
				uvRotationSpeedUnselected = s.SerializeObject<Angle>(uvRotationSpeedUnselected, name: "uvRotationSpeedUnselected");
				actorIconSelected = s.SerializeObject<StringID>(actorIconSelected, name: "actorIconSelected");
				actorIconUnselected = s.SerializeObject<StringID>(actorIconUnselected, name: "actorIconUnselected");
				inverseShadowState = s.Serialize<bool>(inverseShadowState, name: "inverseShadowState");
				StringID__29 = s.SerializeObject<StringID>(StringID__29, name: "StringID__29");
				StringID__30 = s.SerializeObject<StringID>(StringID__30, name: "StringID__30");
				StringID__31 = s.SerializeObject<StringID>(StringID__31, name: "StringID__31");
			} else {
				blinkScaleMin = s.Serialize<float>(blinkScaleMin, name: "blinkScaleMin");
				blinkScale = s.Serialize<float>(blinkScale, name: "blinkScale");
				blinkPeriod = s.Serialize<float>(blinkPeriod, name: "blinkPeriod");
				blinkMinBlendSpeed = s.Serialize<float>(blinkMinBlendSpeed, name: "blinkMinBlendSpeed");
				hightlightAlphaMin = s.Serialize<float>(hightlightAlphaMin, name: "hightlightAlphaMin");
				hightlightAlpha = s.Serialize<float>(hightlightAlpha, name: "hightlightAlpha");
				hightlightPeriod = s.Serialize<float>(hightlightPeriod, name: "hightlightPeriod");
				hightlightMinBlendSpeed = s.Serialize<float>(hightlightMinBlendSpeed, name: "hightlightMinBlendSpeed");
				activatingScale = s.Serialize<float>(activatingScale, name: "activatingScale");
				activatingDuration = s.Serialize<float>(activatingDuration, name: "activatingDuration");
				activatingRebound = s.Serialize<uint>(activatingRebound, name: "activatingRebound");
				animUnselected = s.SerializeObject<StringID>(animUnselected, name: "animUnselected");
				animSelected = s.SerializeObject<StringID>(animSelected, name: "animSelected");
				animLocked = s.SerializeObject<StringID>(animLocked, name: "animLocked");
				needSyncBlink = s.Serialize<bool>(needSyncBlink, name: "needSyncBlink");
				fontEffectUnselected = s.SerializeObject<StringID>(fontEffectUnselected, name: "fontEffectUnselected");
				fontEffectSelected = s.SerializeObject<StringID>(fontEffectSelected, name: "fontEffectSelected");
				colorFactorSelected = s.SerializeObject<Color>(colorFactorSelected, name: "colorFactorSelected");
				colorFactorLocked = s.SerializeObject<Color>(colorFactorLocked, name: "colorFactorLocked");
				colorFactorUnselected = s.SerializeObject<Color>(colorFactorUnselected, name: "colorFactorUnselected");
				textLockedAlpha = s.Serialize<float>(textLockedAlpha, name: "textLockedAlpha");
				animLockedAlpha = s.Serialize<float>(animLockedAlpha, name: "animLockedAlpha");
				colorBlendTime = s.Serialize<float>(colorBlendTime, name: "colorBlendTime");
				ScaleUnselected = s.Serialize<float>(ScaleUnselected, name: "ScaleUnselected");
				uvRotationSpeedSelected = s.SerializeObject<Angle>(uvRotationSpeedSelected, name: "uvRotationSpeedSelected");
				uvRotationSpeedUnselected = s.SerializeObject<Angle>(uvRotationSpeedUnselected, name: "uvRotationSpeedUnselected");
				actorIconSelected = s.SerializeObject<StringID>(actorIconSelected, name: "actorIconSelected");
				actorIconUnselected = s.SerializeObject<StringID>(actorIconUnselected, name: "actorIconUnselected");
				inverseShadowState = s.Serialize<bool>(inverseShadowState, name: "inverseShadowState");
			}
		}
		public override uint? ClassCRC => 0x7A821220;
	}
}

