using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour {


    public InputField ipField;
    public Toggle isHostField;
	public void ShowSettingsFields()
    {
       
        if (ipField.IsActive()) ipField.gameObject.SetActive(false);
        else ipField.gameObject.SetActive(true);
        if (isHostField.IsActive()) isHostField.gameObject.SetActive(false);
        else isHostField.gameObject.SetActive(true);
    }




}
