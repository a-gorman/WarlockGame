using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game.UI.Components.Basic;

sealed class Grid : InterfaceComponent {
    // Columns, rows
    public Cell[,] Cells { get; }

    public Grid(Rectangle bounds, int columns = 1, int rows = 1) :
        this(bounds.X, bounds.Y, columns, bounds.Width / columns, rows, bounds.Height / rows) { }
    
    public Grid(int x, int y, int columns, int columnWidth, int rows, int rowHeight) {
        Layout = Layout.WithBoundingBox(x, y, columns * columnWidth, rows * rowHeight);
        Cells = CreateCells(columns, rows);
    }

    public Grid(int columns = 1, int rows = 1) {
        Cells = CreateCells(columns, rows);
    }
    
    private Cell[,] CreateCells(int columns, int rows) {
        if (columns < 1 || rows < 1) {
            Logger.Warning($"Created grid with no cells, Columns: {columns}, Rows: {rows}", Logger.LogType.Interface);
        }
        
        var cells = new Cell[columns, rows];
        for (int c = 0; c < columns; c++) {
            for (int r = 0; r < rows; r++) {
                var cell = new Cell();
                AddComponent(cell);
                cells[c, r] = cell;
            }
        }

        return cells;
    }

    public void AddComponentToCell(InterfaceComponent component, int row, int column) {
        Cells[column, row].AddComponent(component);
    }

    public override void RefreshBounds(Rectangle parentBounds) {
        base.RefreshBounds(parentBounds);
        
        var columns = Cells.GetLength(0);
        var rows = Cells.GetLength(1);

        var columnWidth = BoundingBox.Width / columns;
        var rowHeight = BoundingBox.Height / rows;
        
        int currentWidth = 0;
        for (int c = 0; c < columns; c++) {
            int currentHeight = 0;
            for (int r = 0; r < rows; r++) {
                var cell = Cells[c, r];
                cell.Layout = Layout.WithBoundingBox(currentWidth, currentHeight, columnWidth, rowHeight);
                currentHeight += rowHeight;
            }

            currentWidth += columnWidth;
        }
    }

    public sealed class Cell : InterfaceComponent {
        public Cell() {
            // If the parent grid is clickable, assume we want to be able to click all the individual cells as well.
            Clickable = ClickableState.PassThrough;
        }
    }
}