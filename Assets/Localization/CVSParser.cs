using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CVSParser : MonoBehaviour
{
    public static CVSParser Instance;
    static private List<string> LanguageList = new List<string>();
    static private Dictionary<string, List<string>> LanguageDictionary = new Dictionary<string, List<string>>();

    public void OnEnable()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    static public string[] SplitLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            pattern:@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
            select m.Groups[1].Value).ToArray();
    }

    static public List <string> GetAvailableLanguages()
    {
        if(LanguageList.Count==0)
        {
            var cvsFile = Resources.Load<TextAsset>("QuizManiaLocalization");
            string[] lines = cvsFile.text.Split("\n"[0]);
            LanguageList = new List<string>(SplitLine(lines[0]));
            LanguageList.RemoveAt(0);
        }

        for (int i = 0; i < LanguageList.Count; i++)
        {
            if(LanguageList[i]=="" || LanguageList[i] == null)
            {
                LanguageList.RemoveAt(i);
            }
        }

        return LanguageList;
    }

    static public string GetTextFromId(string Id, int languageIndex)
    {
        if (LanguageDictionary.Count == 0)
        {
            var cvsFile = Resources.Load<TextAsset>("QuizManiaLocalization");
            string[] lines = cvsFile.text.Split("\n"[0]);

            for (int i = 1; i < lines.Length; i++)
            {

                string[] row = SplitLine(lines[i]);

                if(row.Length>1)
                {
                    List<string> worlds = new List<string>(row);
                    worlds.RemoveAt(0);
                    LanguageDictionary.Add(row[0], worlds);
                }
            }
        }

        var values = LanguageDictionary[Id];
        return values[languageIndex];
    }
}
