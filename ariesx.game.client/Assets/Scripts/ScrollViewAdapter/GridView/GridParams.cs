using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView {
    public class GridParams {
        public string cellPrefabPath;
        public CustomScrollRect customScrollRect;
        public GridLayoutGroup gridLayout;
        public bool cellWidthForceExpandInGroup;
        public bool cellHeightForceExpandInGroup;
        public int numCellsPerGroup;
        public TextAnchor childAlignment;


        //private HorizontalOrVerticalLayoutGroup _TheOnlyGroupPrefab;
        private GameObject go;
        public GameObject GetGroupPrefab() {
            if (go == null) {
                go = new GameObject("GridAdapter");
                go.SetActive(false);
                CanvasGroup canvasGroup = go.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                go.transform.SetParent(customScrollRect.transform, false);
                RectTransform goRoot = go.AddComponent<RectTransform>();
                goRoot.sizeDelta = new Vector2(
                    customScrollRect.content.rect.width, gridLayout.cellSize.y);

                GridLayoutGroup _TheOnlyGridPrefab = go.AddComponent<GridLayoutGroup>();
                _TheOnlyGridPrefab.childAlignment = childAlignment;
                _TheOnlyGridPrefab.startCorner = gridLayout.startCorner;
                _TheOnlyGridPrefab.cellSize = gridLayout.cellSize;
                _TheOnlyGridPrefab.spacing = gridLayout.spacing;
                _TheOnlyGridPrefab.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                _TheOnlyGridPrefab.constraintCount = 1;
            }

            return go;
        }

        public virtual int GetGroupIndex(int cellIndex) {
            return cellIndex / numCellsPerGroup;
        }

        public virtual int GetNumberOfRequiredGroups(int numberOfCells) {
            return numberOfCells == 0 ? 0 : GetGroupIndex(numberOfCells - 1) + 1;
        }
    }
}
