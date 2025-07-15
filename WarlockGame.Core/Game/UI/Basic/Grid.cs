using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.UI.Basic;

public sealed class Grid : InterfaceComponent {

    public Cell[,] Cells { get; }
    
    public Grid(int x, int y, int nColumns, int columnWidth, int nRows, int rowHeight) {
        Cells = new Cell[nColumns, nRows];
        int currentHeight = 0;
        for (int c = 0; c < nColumns; c++) {
            int currentWidth = 0;
            for (int r = 0; r < nRows; r++) {
                var cell = new Cell(x + currentWidth, y + currentHeight, columnWidth, rowHeight);
                AddComponent(cell);
                Cells[c, r] = cell;
                currentWidth += columnWidth;
            }
            currentHeight += rowHeight;
        }

        BoundingBox = new Rectangle(x, y, nColumns * columnWidth, nRows * rowHeight);
    }

    public void AddComponent(InterfaceComponent component, int row, int column) {
        Cells[column, row].AddComponent(component);
    }

    public sealed class Cell : InterfaceComponent {
        public Cell(int x, int y, int width, int height) {
            BoundingBox = new Rectangle(x, y, width, height);
        }
    }
}