using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelToXML))]
public class LevelToXMLEditor : Editor
{
    [HideInInspector]
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Save level"))
        {
            SaveLevel(LevelToXML.instance.levelNumber);
        }
    }
    public void SaveLevel(int number)
    {
        LevelToXML.instance.levelNumber = number;

        LevelToXML.instance.FindWalls();
        LevelToXML.instance.FindObstacles();
    }
    
}
