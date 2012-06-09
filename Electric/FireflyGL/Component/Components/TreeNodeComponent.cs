using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FireflyGL.Component
{
	public class TreeNodeComponent : Component, IRespondsToUpdateComponent
    {
        public TreeNodeComponent Parent { get; private set; }
        public ObservableCollection<TreeNodeComponent> Children;

        public override void OnCreate(Entity entity, object[] args)
        {
            base.OnCreate(entity, args);
            Children = new ObservableCollection<TreeNodeComponent>();
            Children.CollectionChanged += (sender, eventArgs) =>
                {
                    var oldItems = eventArgs.OldItems;
                    foreach (TreeNodeComponent item in oldItems)
                    {
                        item.Parent = null;
                    }
                    var newItems = eventArgs.NewItems;
                    foreach (TreeNodeComponent item in newItems)
                    {
                        item.Parent = this;
                    }
                };
        }

        public override void OnDestroy()
        {
        }

        public override void UpdateSelf()
        {
        }

        public LinkedList<T> GetChildrenComponents<T>() where T : Component
        {
            var list = new LinkedList<T>();
            foreach (var child in Children)
            {
                var component = child.Host.GetComponent<T>();
                if (component != null) list.AddLast(component);
            }
            return list;
        }

        public T GetParentComponent<T>() where T : Component
        {
            if (Parent != null)
            {
                return Parent.Host.GetComponent<T>();
            }
            return null;
        }
    }
}
