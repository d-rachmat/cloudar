using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    public FBScript firebasescript;
    public SimpleHandler simplehandler;
    public Loader loader;

    private void Update()
    {
        gameObject.transform.Rotate(0, 0, 1);
    }

    private void OnMouseUp()
    {
        firebasescript.AddPointToSpecifiedUser(simplehandler.linipoin);
        simplehandler.imageContent.gameObject.SetActive(false);
        simplehandler.Lootbox.SetActive(false);
        simplehandler.mCloudRecoBehaviour.CloudRecoEnabled = true;
        firebasescript.loginElement.SetActive(true);
    }
}
