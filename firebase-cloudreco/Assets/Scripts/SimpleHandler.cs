using UnityEngine;
using System.Collections;
using SimpleJSON;
using Vuforia;
using UnityEngine.Video;
using System.Linq;

public class SimpleHandler : MonoBehaviour, ICloudRecoEventHandler {

    public CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";
    private string videoname;
    private string imagename;
   
    private Texture2D txt;
	ObjectTracker m_ObjectTracker;

    public GameObject loginElement;
    public GameObject Lootbox;
    public string linipoin;
    public string lootbox;
    public string url;
    public VideoPlayer videoContent;
    public UnityEngine.UI.Image imageContent;

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

        Parsing();
        loginElement.SetActive(false);
        string TargetId = targetSearchResult.UniqueTargetId;
        url = "http://35.240.171.245:8080/MacroCloud-trunk/VuforiaWebServices/uploads/" + TargetId;

        StartCoroutine(loadContent());

        // stop the target finder (i.e. stop scanning the cloud)
        mCloudRecoBehaviour.CloudRecoEnabled = false;
	}

    void Parsing()
    {
        var N = JSON.Parse(mTargetMetadata);
        linipoin = N["linipoin"];
        lootbox = N["lootbox"];
        videoname = N["videoFileName"];
        imagename = N["bgFileName"];
    }

    IEnumerator loadContent()
    {
        videoContent.url = url + "/" + videoname;
        videoContent.gameObject.SetActive(true);
        videoContent.loopPointReached += EndReached;
        videoContent.Play();

        yield return new WaitForSeconds(0.3f);

        using (WWW www = new WWW(url + "/" + imagename))
        {
            yield return www;
            txt = www.texture;

            Sprite mySprite = Sprite.Create(txt, new Rect(0.0f, 0.0f, txt.width, txt.height), new Vector2(0.5f, 0.5f), 100.0f);
            imageContent.sprite = mySprite;
        }
        imageContent.gameObject.SetActive(true);
    }

    void EndReached(VideoPlayer vp)
    {
        vp = videoContent.GetComponent<VideoPlayer>();
        vp.gameObject.SetActive(false);
        vp.targetTexture.Release();
        Lootbox.SetActive(true);
    }
}