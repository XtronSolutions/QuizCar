using System.IO;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Helpers;
using UnityEngine;
using System;


namespace RacingGameKit.Helpers
{

    internal static class JsonUtils
    {
        public static string ReadJsonFile(string FileName)
        {
            string jSonData = "";

            StringReader reader = null;

            TextAsset jsondatafile = (TextAsset)Resources.Load(FileName, typeof(TextAsset));

            reader = new StringReader(jsondatafile.text);
            if (reader == null)
            {
                Debug.LogWarning(FileName + ".txt not found in DATA folder or its not readable!");
            }
            else
            {
                // jSonData = reader.ReadToEnd();
                string jLine;
                while ((jLine = reader.ReadLine()) != null)
                {
                    if (jLine.Length > 2)
                    {
                        if (!jLine.Substring(0, 2).Equals("//"))
                        {
                            jSonData += jLine;
                        }
                    }
                    else
                    { jSonData += jLine; }
                }

            }
            return jSonData;
        }

        /// <summary>
        /// Retrieves AI names from json data. If json not available, uses stock ai names instead.
        /// </summary>
        /// <returns></returns>
        public static string[] GetRacerNames()
        {

            string[] StockAINames = new string[] { "AI Player 1", "AI Player 2", "AI Player 3", "AI Player 4", "AI Player 5", "AI Player 6", "AI Player 7", "AI Player 8" };
            try
            {
                List<string> AINames = new List<string>();

                string jSonData = ReadJsonFile("ai_names").Replace("\r\n", "");

                if (jSonData != "")
                {
                    JSONObjectForKit j = new JSONObjectForKit(jSonData);

                    if (j.HasField("ainames"))
                    {
                        for (int i = 0; i < j.GetField("ainames").list.Count; i++)
                        {
                            JSONObjectForKit k = new JSONObjectForKit(j.GetField("ainames").list[i].ToString());
                            AINames.Add((string)k.GetField("ainame").ToString().Replace("\"", ""));
                        }
                    }
                    return AINames.ToArray();
                }
                else
                {
                    return StockAINames;
                }
            }
            catch(Exception _ex)
            {
                Debug.Log("ai_names.txt document have invalid JSON data. Using stock AI Names! Please consult documentation.\r\n" + _ex.Message + "\r\n" + _ex.InnerException + "\r\n" + _ex.StackTrace);
                return StockAINames;
            }
        }

    }

}