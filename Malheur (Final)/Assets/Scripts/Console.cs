using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class Console : MonoBehaviour {

    public InputField inputText;
    public Text mainText;
    public GameObject console;
    public GameObject heroe;
    private int _initialDamage;
    private int _initialHeat;
    private int _initialSpecialHeat;

    //Delegate es un tipo de variable que guarda métodos
    public delegate void ParameterCommandProto(string p);
    public delegate void CommandProto();
    public Dictionary<string, CommandProto> allCommands = new Dictionary<string, CommandProto>();
    public Dictionary<string, ParameterCommandProto> allParameterCommands = new Dictionary<string, ParameterCommandProto>();
    public Dictionary<string, string> allDescriptions = new Dictionary<string, string>();

    void Start()
    {
        heroe = GameObject.Find("Heroe");
        _initialHeat = heroe.GetComponent<Heroe>().bulletHeat;
        _initialSpecialHeat = heroe.GetComponent<Heroe>().specialBulletHeat;
        _initialDamage = heroe.GetComponent<Heroe>().prefabBullet.GetComponent<Bullet>().damage;
        RegisterCommand("_cls", ClearScreen, " ---> Limpia la pantalla");
        RegisterCommand("_help", Help, " ---> Muestra ayuda");
        RegisterCommand("_boss", GoToBoss, " ---> Te lleva al boss");
        RegisterCommand("_nggyu", Nggyu, " ---> ...");
        RegisterParameterCommand("_godMode", GodMode, " ---> 1: Volar y atravesar paredes on / 0: Todo lo anterior off");
        RegisterParameterCommand("_chuckNorris", ChuckNorris, " ---> 1: Fucking Chuck Norris / 0: Not Chuck Norris");
        RegisterParameterCommand("_noHeat", NoHeat, " ---> 1: El arma no recalienta / 0: El arma recalienta");
        RegisterParameterCommand("_loadLvl", LoadLevel, " ---> Carga el nivel 1, 2, o 3, según el parámetro indicado");
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return) && console.activeSelf)
            Write(inputText.text);

        if (Input.GetKeyDown(KeyCode.F8))
        {
            console.SetActive(!console.activeSelf);
            heroe.GetComponent<Heroe>().isLocked = !heroe.GetComponent<Heroe>().isLocked;
            Cursor.visible = !Cursor.visible;
            Analytics.CustomEvent("consoleOn");
        }
    }

    public void Write(string txt)
    {
        //Para los comandos con parametros se guarda el texto del Input
        //en otra variable para removerle el ultimo caracter(parametro)
        string newT = txt;
        newT = newT.Remove(newT.Length - 1);

        if (allCommands.ContainsKey(txt))
            allCommands[txt].Invoke();
        else if (allParameterCommands.ContainsKey(newT))
        {
            //El texto sin el ultimo caracter será el nombre del comando
            //y el primer texto será el comando CON el parámetro
            allParameterCommands[newT].Invoke(txt);
        }
        else
            mainText.text += "\n" + txt;
        inputText.text = "";
    }

    public void RegisterCommand(string name, CommandProto command, string description)
    {
        allCommands[name] = command;
        allDescriptions[name] = description;
    }

    public void RegisterParameterCommand(string name, ParameterCommandProto parameterCommand, string description)
    {
        allParameterCommands[name] = parameterCommand;
        allDescriptions[name] = description;
    }

    public void ClearScreen()
    {
        mainText.text = "";
    }

    public void Help()
    {
        foreach (var c in allDescriptions)
        {
            Write(c.Key + c.Value);
        }
    }

    public void GodMode(string p)
    {
        if (p == "_godMode1")
        {
            heroe.GetComponent<Heroe>().godMode = true;
            Write("God mode: ON");
        }
        else if (p == "_godMode0")
        {
            heroe.GetComponent<Heroe>().godMode = false;
            Write("God mode: OFF");
        }
        else Write("Invalid parameter");
    }

    public void ChuckNorris(string p)
    {
        if (p == "_chuckNorris1")
        {
            heroe.GetComponent<Heroe>().health = 10000000000000001;
            heroe.GetComponent<Heroe>().prefabBullet.GetComponent<Bullet>().damage = 100000000;
            Write("Chuck Norris: ON");
        }
        else if (p == "_chuckNorris0")
        {
            heroe.GetComponent<Heroe>().health = 100;
            Write("Chuck Norris: OFF");
            heroe.GetComponent<Heroe>().prefabBullet.GetComponent<Bullet>().damage = _initialDamage;
        }
        else Write("Invalid parameter");
    }

    public void NoHeat(string p)
    {
        if (p == "_noHeat1")
        {
            heroe.GetComponent<Heroe>().bulletHeat = 0;
            heroe.GetComponent<Heroe>().specialBulletHeat = 0;
            Write("No heat: ON");
        }
        else if (p == "_noHeat0")
        {
            heroe.GetComponent<Heroe>().bulletHeat = _initialHeat;
            heroe.GetComponent<Heroe>().specialBulletHeat = _initialSpecialHeat;
            Write("No heat: OFF");
        }
        else Write("Invalid parameter");
    }

    public void LoadLevel(string p)
    {
        if (p == "_loadLvl1") SceneManager.LoadScene("Lvl1");
        else if (p == "_loadLvl2") SceneManager.LoadScene("Lvl2");
        else if (p == "_loadLvl3") SceneManager.LoadScene("Lvl3");
        else Write("Invalid parameter");
    }

    public void GoToBoss()
    {
        SceneManager.LoadScene("LvlBoss");
    }

    public void Nggyu()
    {
        heroe.GetComponent<Heroe>().PlaySound("nggyu");
    }
}
