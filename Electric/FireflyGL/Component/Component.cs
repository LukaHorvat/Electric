using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL.Component
{
    abstract class Component
    {
        public Entity Host;
        public event Action UpdateEvent;

        public virtual void OnCreate(Entity entity, Object[] args)
        {
            Host = entity;
        }
        public abstract void OnDestroy();
        public virtual void OnUpdate()
        {
            UpdateSelf();
            if (UpdateEvent != null) UpdateEvent.Invoke();
        }
        public abstract void UpdateSelf();
    }
}
