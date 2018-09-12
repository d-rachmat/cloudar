using UnityEngine;
using System.Collections;
using SimpleJSON;
using Vuforia;
using System.Linq;

public class SimpleHandler : MonoBehaviour, ICloudRecoEventHandler {
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";
	ObjectTracker m_ObjectTracker;

    public GameObject loginElement;
    public Loader loader;
    public string linipoin;
    public string lootbox;
  

	// Use this for initialization
	void Start () {
		// register this event handler at the cloud reco behaviour
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (mCloudRecoBehaviour)
		{
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}
	}

	public void OnInitialized() {
		Debug.Log ("Cloud Reco initialized");
		m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
	}
	
	public void OnInitError(TargetFinder.InitState initError) {
		Debug.Log ("Cloud Reco init error " + initError.ToString());
	}
	
	public void OnUpdateError(TargetFinder.UpdateState updateError) {
		Debug.Log ("Cloud Reco update error " + updateError.ToString());
        mCloudRecoBehaviour.CloudRecoEnabled = true;
	}

	public void OnStateChanged(bool scanning) {
		mIsScanning = scanning;
		
		if (scanning)
		{
			// clear all known trackables
			m_ObjectTracker  = TrackerManager.Instance.GetTracker<ObjectTracker>();
			m_ObjectTracker.TargetFinder.ClearTrackables(false);
		}
	}

	// Here we handle a cloud target recognition event
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult) {
		// do something with the target metadata
		mTargetMetadata = targetSearchResult.MetaData;

        loader.assetBundleName = "bundle_" + targetSearchResult.TargetName.ToLower();
        Parsing();
        loader.ShowContent();
        loginElement.SetActive(false);

		// stop the target finder (i.e. stop scanning the cloud)
		mCloudRecoBehaviour.CloudRecoEnabled = false;
	}

    void Parsing()
    {
        var N = JSON.Parse(mTargetMetadata);
        linipoin = N["linipoin"];
        lootbox = N["lootbox"];
    }


}
