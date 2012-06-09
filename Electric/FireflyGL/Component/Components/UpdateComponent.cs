using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FireflyGL.Component
{
	public enum AutoUpdateComponents
    {
        Enabled,
        Disabled
    }

	public class UpdateComponent : Component, IUpdatable
    {
        private Action method;
        private AutoUpdateComponents autoUpdate;

        public LinkedList<Component> ComponentsToUpdate;

        public override void OnCreate(Entity entity, Object[] args)
        {
            base.OnCreate(entity, args);
            ComponentsToUpdate = new LinkedList<Component>();
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
        }

        public override void OnDestroy() { }

        public override void UpdateSelf()
        {
            var treeNode = Host.GetComponent<TreeNodeComponent>();
            if (treeNode != null)
            {
                foreach (var component in treeNode.GetChildrenComponents<UpdateComponent>())
                {
                    component.UpdateSelf();
                }
            } //If the host has any children, call their updates first
            if (method != null) method.Invoke();
            if (autoUpdate == AutoUpdateComponents.Enabled)
            {
                foreach (IRespondsToUpdateComponent comp in Host.GetComponents()) (comp as Component).OnUpdate();
            }
            else
            {
                foreach (Component comp in ComponentsToUpdate) comp.OnUpdate();
            }
        }

        public void Update()
        {
            OnUpdate();
        }
    }
}
