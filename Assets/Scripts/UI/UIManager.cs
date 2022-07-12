using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    endGamePanel,
}

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject EndGamePanel;

    private static IDictionary<PanelType, GameObject> panelsByPanelType;

    private void Awake()
    {
        panelsByPanelType = new Dictionary<PanelType, GameObject>()
        {
            {PanelType.endGamePanel, EndGamePanel},
        };

        foreach (GameObject panel in panelsByPanelType.Values)
            panel.SetActive(false);
    }

    public static GameObject GetPanel(PanelType panelType)
    {
        return panelsByPanelType[panelType];
    }

    public static void SetActivePanel(PanelType panelType, bool flag)
    {
        if(panelsByPanelType[panelType] != null)
            panelsByPanelType[panelType].SetActive(flag);
    }
}
