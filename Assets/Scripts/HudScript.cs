using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class HudScript : MonoBehaviour {
    public Button FireBulletButton , FireMissileButton;
    public Button BackButton;
    public Car_Weapon weaponScript;
    public vehicleHandling PlayerCar;
    // Use this for initialization
    void Start () {

      
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene(Constants.SceneName.MainMenu);
    }
    public void OnFirePressed()
    {
        if (weaponScript != null)
            weaponScript.FireBulletPressed();
    }
    public void OnFireReleased()
    {
        if (weaponScript != null)
            weaponScript.FireBulletReleased();
    }
    public void OnBrakePressed()
    {
        PlayerCar._AccelerationInput = -1;
    }

    public void OnBrakeReleased()
    {
        PlayerCar._AccelerationInput = 1;
    }
}
