﻿using UnityEngine;
using System.Collections;
using System.Xml;

public class Level
{
    int getSomeInfo = 0;
}

/*
[Serializable] class WallInfo
{
    [XmlAttribute]
    public Vector3 Position;
    [XmlAttribute]
    public Vector3 Scale;
}
*/

public class LevelManager : MonoBehaviour
{
    // This is where we flush info from XMLs from.
    // actually, you'll need to make a dedicated Level class.
    Vector3[] coords = new Vector3[10];
    Quaternion[] orients = new Quaternion[10];
    float[] scales = new float[10];

    void Start()
    {

    }

    void Update()
    {

    }
}
