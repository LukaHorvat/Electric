using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FireflyGL.Component
{
    class TransformComponent : Component, IRespondsToUpdateComponent
    {
        private Matrix4 scaleMatrix, rotationMatrix, translationMatrix, modelMatrix, parentMatrix;
        private float scaleX = 1, scaleY = 1, x, y, rotation;
        private bool requiresUpdate = true, ignoresCamera;
        private TreeNodeComponent node;
        private TransformComponent parent
        {
            get
            {
                return node.GetParentComponent<TransformComponent>();
            }
        }

        public Matrix4 ModelMatrix { get { return modelMatrix; } }

        public bool IgnoresCamera
        {
            get { return ignoresCamera; }
            set { requiresUpdate = true; ignoresCamera = value; }
        }

        public float ScaleX
        {
            get { return scaleX; }
            set { requiresUpdate = true; scaleX = value; UpdateScale(); }
        }

        public float ScaleY
        {
            get { return scaleY; }
            set { requiresUpdate = true; scaleY = value; UpdateScale(); }
        }

        public float Scale
        {
            set { requiresUpdate = true; scaleX = value; scaleY = value; UpdateScale(); }
        }

        public float X
        {
            get { return x; }
            set { requiresUpdate = true; x = value; UpdateTranslation(); }
        }

        public float Y
        {
            get { return y; }
            set { requiresUpdate = true; y = value; UpdateTranslation(); }
        }

        public float Rotation
        {
            get { return rotation; }
            set { requiresUpdate = true; rotation = value; Geometry.MakeAngle(ref rotation); UpdateRotation(); }
        }

        private void UpdateScale()
        {
            scaleMatrix = Matrix4.Scale(scaleX, scaleY, 1);
        }

        private void UpdateTranslation()
        {
            translationMatrix = Matrix4.CreateTranslation(x, y, 0);
        }

        private void UpdateRotation()
        {
            rotationMatrix = Matrix4.CreateRotationZ(rotation);
        }

        void UpdateMatrices()
        {
            if (parent != null)
            {
                parentMatrix = parent.modelMatrix;
                if (ignoresCamera == false && parent.ignoresCamera) ignoresCamera = true;
            }
            else parentMatrix = Matrix4.Identity;

            modelMatrix = scaleMatrix * rotationMatrix * translationMatrix * parentMatrix;
            requiresUpdate = false;
            if (node != null)
            {
                foreach (var component in node.GetChildrenComponents<TransformComponent>())
                {
                    component.UpdateMatrices();
                }
            }
        }

        public override void OnCreate(Entity entity, Object[] args)
        {
            base.OnCreate(entity, args);
            node = Host.GetComponent<TreeNodeComponent>();
        }

        public override void OnDestroy()
        {
        }

        public override void UpdateSelf()
        {
            if (requiresUpdate) UpdateMatrices();
        }
    }
}
