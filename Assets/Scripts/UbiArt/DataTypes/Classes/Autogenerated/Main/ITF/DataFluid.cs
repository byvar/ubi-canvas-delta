namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class DataFluid : CSerializable {
		public CListO<EdgeFluid> EdgeFluidList;
		public CListO<EdgeFluidLevel> EdgeFluidListLevels;
		public bool IsCushion;
		public float WeightMultiplier;
		public CListO<FluidFriseLayer> LayerInfos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.DataBin)) {
				EdgeFluidList = s.SerializeObject<CListO<EdgeFluid>>(EdgeFluidList, name: "EdgeFluidList");
				EdgeFluidListLevels = s.SerializeObject<CListO<EdgeFluidLevel>>(EdgeFluidListLevels, name: "EdgeFluidListLevels");
				IsCushion = s.Serialize<bool>(IsCushion, name: "IsCushion");
				WeightMultiplier = s.Serialize<float>(WeightMultiplier, name: "WeightMultiplier");
				LayerInfos = s.SerializeObject<CListO<FluidFriseLayer>>(LayerInfos, name: "LayerInfos");
			}
		}
	}
}

