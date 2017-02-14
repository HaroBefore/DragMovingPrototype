using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using System;

public class EasyMobileManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(CoShowBanner());
	}

    private IEnumerator CoShowBanner()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Show Banner");
        AdManager.ShowBannerAd(BannerAdPosition.Bottom, BannerAdSize.Banner);

        yield return new WaitUntil(() => 
        {
            return AdManager.IsInterstitialAdReady();
        });
        AdManager.ShowInterstitialAd();
    }



    // Update is called once per frame
    void Update () {
		
	}
}
