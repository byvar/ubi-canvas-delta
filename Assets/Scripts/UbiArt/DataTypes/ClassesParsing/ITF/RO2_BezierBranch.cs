using System.Linq;

namespace UbiArt.ITF {
	public partial class RO2_BezierBranch {
		/// <summary>
		/// Gets first component of type T
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <returns>Component of the requested type</returns>
		public T GetComponent<T>() where T : RO2_BezierBranchComponent {
			if (components == null) return null;
			return components.FirstOrDefault(c => (c?.obj as T) != null)?.obj as T;
		}
	}
}
