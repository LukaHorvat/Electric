﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public interface IInstanced
	{
		void RegisterToMesh(InstancedMesh mesh);
	}
}
