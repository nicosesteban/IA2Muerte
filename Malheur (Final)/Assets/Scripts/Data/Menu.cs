using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
//using UnityEngine.Advertisements;

public class Menu : MonoBehaviour {

    public GameObject prefabMusicBox;
    public List<string> scenes = new List<string>();
    public List<Button> buttons = new List<Button>();
    public Dictionary<Button, string> buttonScenes = new Dictionary<Button, string>();
    public static bool musicBoxCreated;
    public static string retryScene;
    //private ShowResult result;
	
	void Start ()
    {
        Cursor.visible = true;

        if (SceneManager.GetActiveScene().name == "Defeat")
            scenes[1] = retryScene;
        else
            retryScene = null;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttonScenes[buttons[i]] = scenes[i];
        }

        //La caja de música se crea solo una vez, la variable estática verifica que no se vuelva a crear.
        if(SceneManager.GetActiveScene().name == "MainMenu" && !musicBoxCreated)
        {
            GameObject musicB = Instantiate(prefabMusicBox);
            musicBoxCreated = true;
        }

        /*if (SceneManager.GetActiveScene().name == "Defeat")
        {
            Advertisement.Initialize("IP", true);
            retryButton.SetActive(false);
            Ads();
        }*/
    }
	
    public void LoadScene(Button b)
    {
        SceneManager.LoadScene(buttonScenes[b]);
    }

    public void Exit()
    {
        Application.Quit();
    }

    /*public void Ads()
    {
        Advertisement.Show();

        if (Advertisement.IsReady()) Advertisement.Show();

        switch (result)
            {
                case ShowResult.Finished:
                    print("Se vio todo el anuncio");
                    retryButton.SetActive(true);
                    break;
                case ShowResult.Skipped:
                    print("Se saltó el anuncio");
                    break;
                case ShowResult.Failed:
                    print("El anuncio falló");
                    retryButton.SetActive(true);
                    break;
            }
    }*/
}
