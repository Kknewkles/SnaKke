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
            LevelToXML.instance.levelNumber = 2;
            
            LevelToXML.instance.FindWalls();
            LevelToXML.instance.FindObstacles();
        }
    }
    public void SaveLevel(int number)
    {

    }
    
}
