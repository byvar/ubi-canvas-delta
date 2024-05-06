using System;

namespace UbiArt {
	public class PathConversionRuleFunction : IPathConversionRule {
		public Action<Path> PathAction { get; set; }

		public PathConversionRuleFunction(Action<Path> pathAction) {
			PathAction = pathAction;
		}

		public void Apply(Path path) {
			PathAction(path);
		}
	}
}
