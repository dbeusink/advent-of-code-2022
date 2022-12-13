namespace advent_of_code_2022.Puzzles;

internal class Day8 : PuzzleBase
{
    public Day8() : base(nameof(Day8)) { }

    public override string SolvePart1()
    {
        AssertInputLoaded();
        var grid = GetGrid();
        var rows = grid.GetLength(0);
        var columns = grid.GetLength(1);

        // Count visible trees
        int visibleTrees = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var treeHeight = grid[i, j];
                if (IsEdge() ||
                    VisibleFromLeft() ||
                    VisibleFromRight() ||
                    VisibleFromTop() ||
                    VisibleFromBottom())
                {
                    visibleTrees++;
                }

                // -- Local functions --
                bool IsEdge() => i == 0 || i == rows - 1 || j == 0 || j == columns - 1;

                bool VisibleFromLeft()
                {
                    for (int k = 0; k < j; k++)
                    {
                        if (grid[i, k] >= treeHeight)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                bool VisibleFromRight()
                {
                    for (int k = columns - 1; k > j; k--)
                    {
                        if (grid[i, k] >= treeHeight)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                bool VisibleFromTop()
                {
                    for (int k = 0; k < i; k++)
                    {
                        if (grid[k, j] >= treeHeight)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                bool VisibleFromBottom()
                {
                    for (int k = rows - 1; k > i; k--)
                    {
                        if (grid[k, j] >= treeHeight)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        return visibleTrees.ToString();
    }

    public override string SolvePart2()
    {
        AssertInputLoaded();
        var grid = GetGrid();
        var rows = grid.GetLength(0);
        var columns = grid.GetLength(1);

        // Count visible trees
        var highestScenicScore = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Traverse :)
            }
        }

        return highestScenicScore.ToString();
    }

    private int[,] GetGrid()
    {
        var rows = Input!.Length;
        var columns = Input![0].Length;
        var grid = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j] = int.Parse(Input![i].AsSpan().Slice(j, 1));
            }
        }

        return grid;
    }
}