using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL.Component
{
    abstract class Entity
    {
        private Dictionary<Type, Component> components;
        private Dictionary<string, Object> componentData;

        public Entity()
        {
            components = new Dictionary<Type, Component>();
            componentData = new Dictionary<string, Object>();
        }

        public virtual void AttachComponent<T>(params Object[] args) where T : Component
        {
            var component = (Component)Activator.CreateInstance<T>();
            AttachComponent(component, args);
        }

        public virtual void AttachComponent(Component component, params Object[] args)
        {
            components[component.GetType()] = component;
            component.OnCreate(this, args);
        }

        public virtual void DetachComponent(Component component)
        {
            if (components.ContainsValue(component))
            {
                components.Remove(component.GetType());
                component.OnDestroy();
            }
        }

        public virtual void DetachComponent<T>() where T : Component
        {
            if (components.ContainsKey(typeof(T)))
            {
                components[typeof(T)].OnDestroy();
                components.Remove(typeof(T));

            }
        }

        public virtual List<Component> GetComponents()
        {
            return components.Values.ToList();
        }

        public virtual T GetComponent<T>() where T : Component
        {
            if (components.ContainsKey(typeof(T))) return (T)components[typeof(T)];
            return null;
        }

        public virtual T GetComponentData<T>(string key)
        {
            if (componentData.ContainsKey(key))
            {
                return (T)componentData[key];
            }
            return default(T);
        }
    }
}
