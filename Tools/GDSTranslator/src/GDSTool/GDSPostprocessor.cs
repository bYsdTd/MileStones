using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GDSTool
{
	public class GDSPostprocessor
	{
		public virtual void Postprocess(GDSTranslator.Input input, GDSTranslator.Output result)
		{
			Check(input, result);
		}

		private void Check(GDSTranslator.Input input, GDSTranslator.Output result)
		{
			// TODO : verification.
		}
	}
}