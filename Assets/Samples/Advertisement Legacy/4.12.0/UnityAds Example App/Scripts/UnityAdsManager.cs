using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.UI;

public class UnityAdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    [SerializeField] private string _rewardedPlacementId = "rewardedVideo";
    [SerializeField] EnergyManager energyManager;
    [SerializeField] MenuManager menuManager;

    private string GAME_ID = "3003911"; //replace with your gameID from dashboard. note: will be different for each platform.

    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_LEFT;

    private bool showBanner = false;

    //utility wrappers for debuglog
    public delegate void DebugEvent(string msg);
    public static event DebugEvent OnDebugLog;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
#if UNITY_IOS
    GAME_ID = _iOSGameId;
#elif UNITY_ANDROID
        GAME_ID = _androidGameId;
#elif UNITY_EDITOR
    GAME_ID = _androidGameId; //Only for testing the functionality in the Editor
#endif

        Debug.Log($"GAME_ID: {GAME_ID}");

        if (Advertisement.isInitialized)
        {
            Debug.Log("Ads ya cargaron bro");
        }

        if (!Advertisement.isSupported)
        {
            Debug.Log("Ads no soportados");
        }

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(GAME_ID, _testMode, this);
        }
    }

    public void LoadBanner()
    {
        Advertisement.Banner.SetPosition(bannerPosition);

        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = ShowBannerAd
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(GAME_ID, options);
    }

    void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show("Banner_Android", options);
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(_rewardedPlacementId, this);
    }

    private void GrantReward()
    {
        DebugLog("Reward granted to the player!");
        ProgressionManager.Player_Data.Energy += 100;
        ProgressionManager.Player_Data.Gold += 50;

        menuManager.RefreshGoldTxt();
        energyManager.RefreshEnergyTxt();
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    #region Interface Implementations
    public void OnInitializationComplete()
    {
        DebugLog("Init Success");
        LoadBanner();
        Advertisement.Load(_rewardedPlacementId, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        DebugLog($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        DebugLog($"Load Success: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        DebugLog($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        DebugLog($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        DebugLog($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        DebugLog($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        DebugLog($"OnUnityAdsShowComplete: [{showCompletionState}]: {placementId}");

        if (placementId == _rewardedPlacementId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            GrantReward();
        }
    }
    #endregion

    void DebugLog(string msg)
    {
        OnDebugLog?.Invoke(msg);
        Debug.Log(msg);
    }
}
