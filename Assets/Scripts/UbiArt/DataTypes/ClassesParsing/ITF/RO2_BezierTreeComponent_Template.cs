using System.Linq;

namespace UbiArt.ITF {
	public partial class RO2_BezierTreeComponent_Template {
		public override ActorComponent Instantiate(Context context) {

			var component = (RO2_BezierTreeComponent)base.Instantiate(context);
			component.branch = new RO2_BezierBranch() {
				components = new CArrayO<Generic<RO2_BezierBranchComponent>>(
					tree.branchComponents.Select(c => new Generic<RO2_BezierBranchComponent>(InstantiateComponent(c.obj))).ToArray())
			};
			return component;

			RO2_BezierBranchComponent InstantiateComponent(RO2_BezierBranchComponent_Template ctpl) => (RO2_BezierBranchComponent)ctpl?.Instantiate(context);
		}
	}
}
