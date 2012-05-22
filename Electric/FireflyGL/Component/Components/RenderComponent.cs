using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL.Component
{
    class RenderComponent : Component, IRenderable
    {
        private Action method;
        private AutoUpdateComponents autoUpdate;

        public LinkedList<Component> ComponentsToUpdate;

        public override void OnCreate(Entity entity, object[] args)
        {
            base.OnCreate(entity, args);
            foreach (var item in args)
            {
                if (item is Action)
                {
                    method = (Action)item;
                }
                else if (item is AutoUpdateComponents)
                {
                    autoUpdate = (AutoUpdateComponents)item;
                }
            }
            ComponentsToUpdate = new LinkedList<Component>();
        }

        public override void OnDestroy()
        {
        }

        public override void UpdateSelf()
        {
            if (method != null) method.Invoke();
            if (autoUpdate == AutoUpdateComponents.Enabled)
            {
                foreach (IRespondsToRenderComponent comp in Host.GetComponents()) (comp as Component).OnUpdate();
            }
            else
            {
                foreach (Component comp in ComponentsToUpdate) comp.OnUpdate();
            }
            //If the host has any children, call their renders after this one
            var treeNode = Host.GetComponent<TreeNodeComponent>();
            if (treeNode != null)
            {
                foreach (var component in treeNode.GetChildrenComponents<RenderComponent>())
                {
                    component.UpdateSelf();
                }
            }
        }

        public void Render()
        {
            OnUpdate();
        }
    }
}
