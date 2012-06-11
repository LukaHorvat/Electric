using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL.Component
{
	public abstract class RenderDataComponent : Component, IRespondsToRenderComponent
    {
        public override void OnDestroy()
        {
        }

        public override void UpdateSelf()
        {
        }
    }
}
