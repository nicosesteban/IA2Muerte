using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementData : MonoBehaviour {

    public GameObject achievementData;
    public Image achieveName;
    public Image achieveInfo;

    public List<string> names = new List<string>();
    public List<Sprite> achieveNamesL = new List<Sprite>();
    public List<Sprite> achieveInfosL = new List<Sprite>();
    public Dictionary<string, Sprite> achieveNames = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> achieveInfos = new Dictionary<string, Sprite>();

    public static bool spotted;
    public static bool intruder;

    void Start ()
    {
        for (int i = 0; i < achieveNames.Count; i++)
        {
            achieveNames[names[i]] = achieveNamesL[i];
            achieveInfos[names[i]] = achieveInfosL[i];
        }
    }
	
	void Update ()
    {
		if(SceneManager.GetActiveScene().name == "Lvl2" && !spotted && !intruder)
        {
            intruder = true;
            achievementData.SetActive(true);
            SetAchieve("Survivor");
        }
	}

    public void SetAchieve(string name)
    {
        achieveName.sprite = achieveNames[name];
        achieveInfo.sprite = achieveInfos[name];
    }
}
