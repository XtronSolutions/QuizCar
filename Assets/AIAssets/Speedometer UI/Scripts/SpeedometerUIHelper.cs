using System;
using System.IO;
using UnityEngine;

public static class SpeedometerUIHelper
{
    public static String ReadJSONFile(String FileName)
    {
        string SpeedoData = "";

        TextAsset JSONDataFile = Resources.Load(FileName) as TextAsset;
       
        StringReader stringReader = null;
        stringReader = new StringReader(JSONDataFile.text);

        if (stringReader == null)
        {
            Debug.LogWarning(FileName + ".txt not found in RESOURCES folder or its not readable!");
        }
        else
        {
            string jLine;
            while ((jLine = stringReader.ReadLine()) != null)
            {
                if (jLine.Length > 2)
                {
                    if (!jLine.Substring(0, 2).Equals("//"))
                    {
                        SpeedoData += jLine;
                    }
                }
                else
                { SpeedoData += jLine; }
            }

        }

        return SpeedoData;
    }
}

