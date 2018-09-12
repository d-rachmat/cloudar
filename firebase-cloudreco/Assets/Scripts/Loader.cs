using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Video;

public class Loader : MonoBehaviour
{

    public string assetBundleName;
    public GameObject videoPlane;
    public GameObject lootBox;
    public Image imagePlane;

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(InstantiateVideoClip());
    }

    // Update is called once per frame
    void Update()
    {
        CloudContentLoader loader = GameObject.Find("EventSystem").GetComponent<CloudContentLoader>();
        if (loader.IsDone)
        {
            if (Input.GetKeyDown("space"))
            {
                AssetBundle bundle = loader.GetAssetBundleByKey(assetBundleName);
                PlayVideoClip(bundle.LoadAsset<VideoClip>("Video"));
                DisplayImageFile(bundle.LoadAsset<Texture2D>("Image"));

                Debug.Log("[Loader] " + assetBundleName + " is loaded");
            }
        }
    }


    void PlayVideoClip(VideoClip videoClip)
    {
        videoPlane.SetActive(true);

        var VideoPlayer = videoPlane.GetComponent<VideoPlayer>();
        VideoPlayer.clip = videoClip;
        VideoPlayer.SetTargetAudioSource(0, VideoPlayer.GetComponent<AudioSource>());
        VideoPlayer.loopPointReached += EndReached;
        VideoPlayer.Play();
    }

    void DisplayImageFile(Texture2D image)
    {
        imagePlane.gameObject.SetActive(true);

        Sprite sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        imagePlane.sprite = sprite;
    }

    void EndReached(VideoPlayer vp)
    {
        vp = videoPlane.GetComponent<VideoPlayer>();
        vp.gameObject.SetActive(false);
        lootBox.SetActive(true);
    }

    public void ShowContent()
    {
        CloudContentLoader loader = GameObject.Find("EventSystem").GetComponent<CloudContentLoader>();
        if (loader.IsDone)
        {
            AssetBundle bundle = loader.GetAssetBundleByKey(assetBundleName);
            PlayVideoClip(bundle.LoadAsset<VideoClip>("Video"));
            DisplayImageFile(bundle.LoadAsset<Texture2D>("Image"));

            Debug.Log("[Loader] " + assetBundleName + " is loaded");
        }
    }
}
