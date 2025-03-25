using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class SoundDescriptor_Template {

		public SoundDescriptor_Template() {
			maxInstances = 0xFFFFFFFF;
			outDevices = 0xFFFFFFFF;
			_params = new SoundParams() {
				filterType2 = SoundParams.FilterType2.HighPass,
				randomPitchMax = 1,
				randomPitchMin = 1,
				metronomeType = 0xFFFFFFFF,
				playOnNext = 0xFFFFFFFF,
			};
		}

		public bool HasFiles
			=> ((files?.Any() ?? false) || (filesBody?.Any() ?? false) || (filesIntro?.Any() ?? false) || (filesOutro?.Any() ?? false));
		public bool HasWwiseEvent => !(WwiseEventGUID?.IsNull ?? true);
	}
}
