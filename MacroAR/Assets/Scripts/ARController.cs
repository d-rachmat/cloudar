using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARController : MonoBehaviour {

    bool burgerBoel = false;
    public GameObject logOUtpanel;

    private void Update()
    {
        if (burgerBoel)
        {
            logOUtpanel.SetActive(true);
        }else{
            logOUtpanel.SetActive(false);
        }
    }

    public void burger()
    {
        burgerBoel = !burgerBoel;
    }
}
