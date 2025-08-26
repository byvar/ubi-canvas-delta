namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_ShooterPirateShipAIComponent : Ray_MultiPiecesActorAIComponent {
		public int useTempateFogParams;
		public Color dynamicFogColor;
		public float dynamicFogMaxDepth;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				useTempateFogParams = s.Serialize<int>(useTempateFogParams, name: "useTempateFogParams");
				if (useTempateFogParams == 0) {
					dynamicFogColor = s.SerializeObject<Color>(dynamicFogColor, name: "dynamicFogColor");
					dynamicFogMaxDepth = s.Serialize<float>(dynamicFogMaxDepth, name: "dynamicFogMaxDepth");
				}
			}
		}
		public override uint? ClassCRC => 0x4FDAE284;
	}
}

