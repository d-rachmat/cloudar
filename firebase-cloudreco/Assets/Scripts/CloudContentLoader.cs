using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/*
    This script has responsibility to patch automatically all of the asset updates from the cloud.
    To perform that function these steps are performed:
	1. Check the assetbundle manifest from the cloud and then download all of its dependencies.
*/

public class CloudContentLoader : MonoBehaviour {

    public string AssetBundleUrl;
    public bool CleanCacheOnPlay;
    public Text downloadPercentage;
    public GameObject loginContainer;
    public GameObject loadingBar;

    private bool m_IsDone = false;
    private Dictionary<string, UriComponent> m_DownloadingURLs = new Dictionary<string, UriComponent>();
    private Dictionary<string, AssetBundle> m_DownloadedAssetBundles = new Dictionary<string, AssetBundle>();



    public bool IsDone
    {
        get { return m_IsDone; }
    }

    public struct UriComponent
    {
        public string Uri;
        public Hash128 Hash;
    }

    private void Awake()
    {
        loginContainer.SetActive(false);
    }

    // Use this for initialization
    IEnumerator Start () {
        yield return StartCoroutine(Initialize());
	}

    /*
     * Initialize tries to load download the manifest and then add the download 
     * job for the entire assetbundles
     */
    IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        if(CleanCacheOnPlay)
        {
            Caching.ClearCache();
            Debug.Log("[CloudContentLoader] cleaned local cache."); 
        }

        string platform = (Application.isEditor) ? "StandaloneWindows" : Application.platform.ToString();
        platform = "Android"; // Editor or not we make it Android platform for simplicity


        string uri = AssetBundleUrl + platform + "/" + platform;
        Debug.Log(uri);
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
        yield return request.SendWebRequest();

        if(request.isNetworkError || request.isHttpError)
        {
            Debug.Log("[CloudContentLoader] there was an error while downloading manifest file.");
        }
        else if(request.isDone)
        {
            Debug.Log("[CloudContentLoader] successfully downloaded manifest file");
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            
            foreach(string bundleName in manifest.GetAllAssetBundles())
            {
                UriComponent subUriComponent = new UriComponent();
                subUriComponent.Uri = AssetBundleUrl + platform + "/" + bundleName;
                subUriComponent.Hash = manifest.GetAssetBundleHash(bundleName);
                m_DownloadingURLs.Add(bundleName, subUriComponent);
            }
        }

        StartCoroutine(DownloadAssetBundlesFromList());
    }

    IEnumerator DownloadAssetBundlesFromList()
    {
        downloadPercentage.text = "70%";
        yield return new WaitForSeconds(3);

        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        foreach (var keyValue in m_DownloadingURLs)
        {
            string Uri = keyValue.Value.Uri;
            Hash128 Hash = keyValue.Value.Hash;
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(Uri, Hash, 0);

            yield return www.SendWebRequest();

            // if downloading fails
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.LogWarning("[CloudContentLoader] error while downloading assetbundle:" + keyValue.Key);
                continue;
            }

            if (www.isDone)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                m_DownloadedAssetBundles.Add(keyValue.Key, bundle);
                www.Dispose();
                Debug.Log("[CloudContentLoader] finished downloading assetbundle:" + keyValue.Key);
                downloadPercentage.text = "90%";
            }
        }

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("All assets was loaded successfully in " + elapsedTime + " seconds");

        // Mark the whole loading process as done
        m_IsDone = true;

        StartCoroutine(CompletedAssets());
    }

    IEnumerator CompletedAssets()
    {
        yield return new WaitForSeconds(2);
        downloadPercentage.text = "100%";
        yield return new WaitForSeconds(2);
        loadingBar.SetActive(false);
        loginContainer.SetActive(true);
    }
    

    public AssetBundle GetAssetBundleByKey(string key)
    {
        return m_DownloadedAssetBundles[key];
    }

}
