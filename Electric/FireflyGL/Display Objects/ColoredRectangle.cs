using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
    class ColoredRectangle : ColoredShape
    {
        private float topLeftX, topLeftY;

        public float Width { get; private set; }
        public float Height { get; private set; }

        public ColoredRectangle(float x, float y, float width, float height, float r, float g, float b, float a)
        {
            filledPolygons.AddLast(new Polygon(false,
                x, y, r, g, b, a,
                x + width, y, r, g, b, a,
                x + width, y + height, r, g, b, a,
                x, y + height, r, g, b, a));
            SetPolygons();

            topLeftX = x;
            topLeftY = y;
            Width = width;
            Height = height;
        }

        public new void AddOutline(float r, float g, float b, float a)
        {
            outlinePolygons.AddLast(new Polygon(false,
                topLeftX, topLeftY, r, g, b, a,
                topLeftX + Width, topLeftY, r, g, b, a,
                topLeftX + Width, topLeftY + Height, r, g, b, a,
                topLeftX, topLeftY + Height, r, g, b, a,
                topLeftX, topLeftY, r, g, b, a));
            SetPolygons();
        }
    }
}
