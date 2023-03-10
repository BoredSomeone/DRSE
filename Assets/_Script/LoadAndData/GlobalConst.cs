using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConst
{
    public struct SaveKey
    {
        public static string Name = "ProjectName";
        public static string Path = "ProjectPath";
        public static string Speed = "Speed";
    }

    public static string SaveFileName = "Data";
    public static string SaveFileFullName = SaveFileName + ".std";

    public static float SpeedRatio = 20;
    public static float NoteSizeRatio = 3;
}
