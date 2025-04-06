using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignIN : MonoBehaviour
{
   
    public  TMP_InputField id_input;
    public TMP_InputField password_Input;
    public Button signIn_Bbutton;
    public Button signIn_ch_Bbutton;
    public Button signUp_ch_Bbutton;
    public GameObject signUP_UI;


   
    public void SignIn_button_click()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void SignUp_button_ch_click()
    {
        signUP_UI.SetActive(true);
    }
    public void SignIn_button_ch_click()
    {
        signUP_UI.SetActive(false);
    }
}
