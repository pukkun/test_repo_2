using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public List<PortalTarget> ListPortal = new List<PortalTarget>();

    public void HandleSnakeEscape(Snake snake)
    {
        foreach (var item in ListPortal)
        {
            item.ActiveTurn();
        }
    }

#if UNITY_EDITOR
    public void ClearPortal()
    {
        FetchOldPortal();
        foreach (var portal in ListPortal)
        {
            if (portal != null)
            {
                DestroyImmediate(portal.gameObject);
            }
        }
        ListPortal.Clear();
        Debug.Log("Cleared all portal in PortalManager");
    }
    public void FetchOldPortal()
    {
        if (true || ListPortal.IsNullOrEmpty())
        {
            PortalTarget[] portals = FindObjectsOfType<PortalTarget>(true);
            foreach (var item in portals)
            {
                ListPortal.Add(item);
            }
        }
    }
    public void CreatePortal(GridManager gridManager, List<PortalData> portalInGrid)
    {
        int maxHeight = gridManager.Grids.Length;
        int maxWidth = gridManager.Grids[0].Widths.Length;
        GameDataSO gameDataSO = UnityEditor.AssetDatabase.LoadAssetAtPath<GameDataSO>("Assets/Scripts/ScriptableObjects/GameDataSO.asset");
        PortalTarget prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<PortalTarget>("Assets/Prefabs/GamePlay/PortalTarget.prefab");
        foreach (var pdt in portalInGrid) 
        {
            if (pdt.Coord.Count == 0) continue;
            Vector2Int coord = pdt.Coord[0];
            Vector3 pos = new Vector3(GridHelper.GetX(coord.x, maxWidth), GridHelper.GetY(coord.y, maxHeight));
            PortalTarget instance = (PortalTarget)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
            pos.z = 0.9f;
            instance.transform.localPosition = pos;
            instance.SetColor(gameDataSO.GetDataMaterial(pdt.SelectedColor).MainColor);
            instance.SetMaterial(pdt.Turn, gameDataSO.GetDataMaterial(pdt.SelectedColor));
            instance.PortalData = pdt;
            ListPortal.Add(instance);
        }
    }
#endif
}
