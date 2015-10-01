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
            XMLToLevel.instance.levelNumber = 2;
            
            XMLToLevel.instance.ReadWallsFromXML();
            XMLToLevel.instance.ReadObstaclesFromXML();
        }
    }

    public void LoadLevel(int number)
    {

    }
}
