using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public FitType fitType;

    public int rows;

    public int columns;

    public Vector2 cellSize;

    public Vector2 spacing;

    public bool fitX;

    public bool fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if(fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform) 
        {
            fitX = true;
            fitY = true;

            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        if(fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }
        if(fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        float parentWidht = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidht = parentWidht / (float)columns - ((spacing.x / (float)columns) * 2) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float)rows) * 2) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = fitX ? cellWidht : cellSize.x;
        cellSize.y = fitY ? cellHeight: cellSize.y;

        int colllumCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            colllumCount = i % columns;

            var item = rectChildren[i];

            var xpos = (cellSize.x * colllumCount) + (spacing.x * colllumCount) + padding.left;
            var ypos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xpos, cellSize.x);
            SetChildAlongAxis(item, 1, ypos, cellSize.y);
        }


    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }

}
