using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public class Stage : DisplayObject
	{
		public Timeline Timeline;

		public Stage()
			: base()
		{
			Timeline = new Timeline();
		}
	}
}
