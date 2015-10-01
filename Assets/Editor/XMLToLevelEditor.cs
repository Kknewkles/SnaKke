using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(XMLToLevel))]
public class XMLToLevelEditor : Editor
{
    [HideInInspector]
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Load level"))
        {
            LoadLevel(XMLToLevel.instance.levelNumber);
        }
    }

    public void LoadLevel(int number)
    {
        XMLToLevel.instance.levelNumber = number;

        XMLToLevel.instance.ReadWallsFromXML();
        XMLToLevel.instance.ReadObstaclesFromXML();
    }
}
