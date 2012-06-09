using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireflyGL
{
	public class Table : DisplayObject
	{
		private int columns, rows;
		private LinkedList<DisplayObject>[,] table;

		public float[] ColumnWidth, RowHeight;

		public Table(int columns, int rows, float columnWidth, float rowHeight)
		{
			this.columns = columns;
			this.rows = rows;
			this.ColumnWidth = new float[columns]; for (int i = 0; i < columns; i++) this.ColumnWidth[i] = columnWidth;
			this.RowHeight = new float[rows]; for (int i = 0; i < rows; i++) this.RowHeight[i] = rowHeight;

			table = new LinkedList<DisplayObject>[columns, rows];
			for (int i = 0; i < columns; ++i) for (int j = 0; j < rows; ++j) table[i, j] = new LinkedList<DisplayObject>();
		}

		public void AddObjectAt(int column, int row, DisplayObject target)
		{
            if (column >= columns || row >= rows) throw new Exception("Table not large enough");
			table[column, row].AddLast(target);
			float x = 0;
			for (int i = 0; i < column; ++i) x += ColumnWidth[i];
			float y = 0;
			for (int i = 0; i < row; ++i) y += RowHeight[i];
			target.X = x;
			target.Y = y;
			if (target is Label)
			{
				var lbl = target as Label;
				lbl.WordWrap = new Rectangle(0, 0, ColumnWidth[column], RowHeight[row]);
				lbl.UpdateText();
			}

			AddChild(target);
		}

		public void RemoveObjectAt(DisplayObject target)
		{
			for (int i = 0; i < columns; ++i)
			{
				for (int j = 0; j < rows; ++j)
				{
					if (table[i, j].Contains(target)) table[i, j].Remove(target);
				}
			}
			if (target is Label)
			{
				(target as Label).WordWrap = null;
				(target as Label).UpdateText();
			}
			RemoveChild(target);
		}

		public void UpdateTable()
		{
			float x = 0;
			float y = 0;
			for (int i = 0; i < columns; ++i)
			{
				for (int j = 0; j < rows; ++j)
				{
					foreach (var obj in table[i, j])
					{
						obj.X = x;
						obj.Y = y;
						if (obj is Label)
						{
							var lbl = obj as Label;
							lbl.WordWrap = new Rectangle(0, 0, ColumnWidth[i], RowHeight[j]);
							lbl.UpdateText();
						}
					}
					y += RowHeight[j];
				}
				x += ColumnWidth[i];
			}
		}
	}
}
