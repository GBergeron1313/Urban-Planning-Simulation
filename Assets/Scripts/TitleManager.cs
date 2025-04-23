using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public Button play, exit;
    // Start is called before the first frame update
    void Start()
    {
        play.onClick.AddListener(StartProgram);
        exit.onClick.AddListener(ExitProgram);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartProgram()
    {
        SceneManager.LoadScene("Graham_Test_Scene", LoadSceneMode.Single);
    }

    void ExitProgram()
    {
        Application.Quit();
    }
}
