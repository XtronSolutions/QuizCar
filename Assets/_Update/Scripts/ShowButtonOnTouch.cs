using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowButtonOnTouch : MonoBehaviour {

    public GameObject target;
    private Vector3 defaultPosition;
    public bool AlwaysVisible = true;

    bool isInTouch = false;
    void Start()
    {
        defaultPosition =  new Vector3(target.transform.position.x, target.transform.position.y,0);
    }
    void Update()
    {
        if(isInTouch)
        {
           
        }
    }
    public void OnTouch(bool isPress)
    {
        isInTouch = isPress;
        if (isPress)
        {
            target.SetActive(true);
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
              //  target.transform.position = Input.mousePosition;
            }
            else
            {
             //   target.transform.position = Input.GetTouch(0).position;
            }
            target.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

        }
        else
        {
            target.transform.localScale = new Vector3(1,1, 1);
            target.SetActive(AlwaysVisible);
            target.transform.position = defaultPosition;
        }
    }
}