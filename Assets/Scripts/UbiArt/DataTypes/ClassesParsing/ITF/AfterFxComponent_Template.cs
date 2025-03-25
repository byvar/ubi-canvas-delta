namespace UbiArt.ITF {
	public partial class AfterFxComponent_Template {
		public AfterFxComponent_Template() {
			paramc = new CArrayO<Color>();
			paramf = new CArrayP<float>();
			paramv = new CListO<Vec3d>();
			parami = new CArrayP<int>();
			for (int i = 0; i < 8; i++) {
				paramf.Add(1f);
				parami.Add(0);
				paramv.Add(Vec3d.Zero);
			}
			for (int i = 0; i < 2; i++) {
				paramc.Add(new Color());
			}
		}
	}
}
