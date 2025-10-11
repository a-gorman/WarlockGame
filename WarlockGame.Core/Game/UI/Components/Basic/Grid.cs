using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game.UI.Components.Basic;

sealed class Grid : InterfaceComponent {
    public Cell[,] Cells { get; }

    public Grid(Rectangle bounds, int nColumns, int nRows) :
        this(bounds.X, bounds.Y, nColumns, bounds.Width / nColumns, nRows, bounds.Height / nRows) { }
    
    public Grid(int x, int y, int nColumns, int columnWidth, int nRows, int rowHeight) {
        if (nColumns < 1 || nRows < 1) {
            Logger.Warning($"Added column with no cells, Columns: {nColumns}, Rows: {nRows}");
        }

        BoundingBox = new Rectangle(x, y, nColumns * columnWidth, nRows * rowHeight);
        Cells = new Cell[nColumns, nRows];
        int currentWidth = 0;
        for (int c = 0; c < nColumns; c++) {
            int currentHeight = 0;
            for (int r = 0; r < nRows; r++) {
                var cell = new Cell(currentWidth, currentHeight, columnWidth, rowHeight);
                AddComponent(cell);
                Cells[c, r] = cell;
                currentHeight += rowHeight;
            }
            currentWidth += columnWidth;
        }
    }

    public void AddComponent(InterfaceComponent component, int row, int column) {
        Cells[column, row].AddComponent(component);
    }

    public sealed class Cell : InterfaceComponent {
        public Cell(int x, int y, int width, int height) {
            BoundingBox = new Rectangle(x, y, width, height);
            // If the parent grid is clickable, assume we want to be able to click all the individual cells as well.
            Clickable = ClickableState.PassThrough;
        }
    }
}