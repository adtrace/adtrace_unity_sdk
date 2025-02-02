﻿using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using io.adtrace.sdk;

public class ExampleGUI : MonoBehaviour
{
    private int numberOfButtons = 8;
    private bool isEnabled;
    private bool showPopUp = false;
    private string txtSetEnabled = "Disable SDK";
    private string txtManualLaunch = "Manual Launch";
    private string txtSetOfflineMode = "Turn Offline Mode ON";

    void OnGUI()
    {
        if (showPopUp)
        {
            GUI.Window(0, new Rect((Screen.width / 2) - 150, (Screen.height / 2) - 65, 300, 130), ShowGUI, "Is SDK enabled?");
        }

        if (GUI.Button(new Rect(0, Screen.height * 0 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), txtManualLaunch))
        {
            if (!string.Equals(txtManualLaunch, "SDK Launched", StringComparison.OrdinalIgnoreCase))
            {
                AdTraceConfig adtraceConfig = new AdTraceConfig("2fm9gkqubvpc", AdTraceEnvironment.Sandbox);
                adtraceConfig.setLogLevel(AdTraceLogLevel.Verbose);
                adtraceConfig.setLogDelegate(msg => Debug.Log(msg));
                adtraceConfig.setEventSuccessDelegate(EventSuccessCallback);
                adtraceConfig.setEventFailureDelegate(EventFailureCallback);
                adtraceConfig.setSessionSuccessDelegate(SessionSuccessCallback);
                adtraceConfig.setSessionFailureDelegate(SessionFailureCallback);
                adtraceConfig.setDeferredDeeplinkDelegate(DeferredDeeplinkCallback);
                adtraceConfig.setAttributionChangedDelegate(AttributionChangedCallback);
                AdTrace.start(adtraceConfig);

                isEnabled = true;
                txtManualLaunch = "SDK Launched";
            }
        }
        
        if (GUI.Button(new Rect(0, Screen.height * 1 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Simple Event"))
        {
            AdTraceEvent adtraceEvent = new AdTraceEvent("g3mfiw");
            AdTrace.trackEvent(adtraceEvent);
        }

        if (GUI.Button(new Rect(0, Screen.height * 2 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Revenue Event"))
        {
            AdTraceEvent adtraceEvent = new AdTraceEvent("a4fd35");
            adtraceEvent.setRevenue(0.25, "EUR");
            AdTrace.trackEvent(adtraceEvent);
        }

        if (GUI.Button(new Rect(0, Screen.height * 3 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Callback Event"))
        {
            AdTraceEvent adtraceEvent = new AdTraceEvent("34vgg9");
            adtraceEvent.addCallbackParameter("key", "value");
            adtraceEvent.addCallbackParameter("foo", "bar");
            AdTrace.trackEvent(adtraceEvent);
        }

        if (GUI.Button(new Rect(0, Screen.height * 4 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Track Value Event"))
        {
            AdTraceEvent adtraceEvent = new AdTraceEvent("w788qs");
            adtraceEvent.addValueParameter("key", "value");
            adtraceEvent.addValueParameter("foo", "bar");
            AdTrace.trackEvent(adtraceEvent);
        }

        if (GUI.Button(new Rect(0, Screen.height * 5 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), txtSetOfflineMode))
        {
            if (string.Equals(txtSetOfflineMode, "Turn Offline Mode ON", StringComparison.OrdinalIgnoreCase))
            {
                AdTrace.setOfflineMode(true);
                txtSetOfflineMode = "Turn Offline Mode OFF";
            }
            else
            {
                AdTrace.setOfflineMode(false);
                txtSetOfflineMode = "Turn Offline Mode ON";
            }
        }

        if (GUI.Button(new Rect(0, Screen.height * 6 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), txtSetEnabled))
        {
            if (string.Equals(txtSetEnabled, "Disable SDK", StringComparison.OrdinalIgnoreCase))
            {
                AdTrace.setEnabled(false);
                txtSetEnabled = "Enable SDK";
            }
            else
            {
                AdTrace.setEnabled(true);
                txtSetEnabled = "Disable SDK";
            }
        }

        if (GUI.Button(new Rect(0, Screen.height * 7 / numberOfButtons, Screen.width, Screen.height / numberOfButtons), "Is SDK Enabled?"))
        {
            isEnabled = AdTrace.isEnabled();
            showPopUp = true;
        }
    }

    void ShowGUI(int windowID)
    {
        if (isEnabled)
        {
            GUI.Label(new Rect(65, 40, 200, 30), "AdTrace SDK is ENABLED!");
        }
        else
        {
            GUI.Label(new Rect(65, 40, 200, 30), "AdTrace SDK is DISABLED!");
        }
       
        if (GUI.Button(new Rect(90, 75, 120, 40), "OK"))
        {
            showPopUp = false;
        }
    }

    public void HandleGooglePlayId(String adId)
    {
        Debug.Log("Google Play Ad ID = " + adId);
    }

    public void AttributionChangedCallback(AdTraceAttribution attributionData)
    {
        Debug.Log("Attribution changed!");

        if (attributionData.trackerName != null)
        {
            Debug.Log("Tracker name: " + attributionData.trackerName);
        }
        if (attributionData.trackerToken != null)
        {
            Debug.Log("Tracker token: " + attributionData.trackerToken);
        }
        if (attributionData.network != null)
        {
            Debug.Log("Network: " + attributionData.network);
        }
        if (attributionData.campaign != null)
        {
            Debug.Log("Campaign: " + attributionData.campaign);
        }
        if (attributionData.adgroup != null)
        {
            Debug.Log("Adgroup: " + attributionData.adgroup);
        }
        if (attributionData.creative != null)
        {
            Debug.Log("Creative: " + attributionData.creative);
        }
        if (attributionData.clickLabel != null)
        {
            Debug.Log("Click label: " + attributionData.clickLabel);
        }
        if (attributionData.adid != null)
        {
            Debug.Log("ADID: " + attributionData.adid);
        }
    }

    public void EventSuccessCallback(AdTraceEventSuccess eventSuccessData)
    {
        Debug.Log("Event tracked successfully!");

        if (eventSuccessData.Message != null)
        {
            Debug.Log("Message: " + eventSuccessData.Message);
        }
        if (eventSuccessData.Timestamp != null)
        {
            Debug.Log("Timestamp: " + eventSuccessData.Timestamp);
        }
        if (eventSuccessData.Adid != null)
        {
            Debug.Log("Adid: " + eventSuccessData.Adid);
        }
        if (eventSuccessData.EventToken != null)
        {
            Debug.Log("EventToken: " + eventSuccessData.EventToken);
        }
        if (eventSuccessData.CallbackId != null)
        {
            Debug.Log("CallbackId: " + eventSuccessData.CallbackId);
        }
        if (eventSuccessData.JsonResponse != null)
        {
            Debug.Log("JsonResponse: " + eventSuccessData.GetJsonResponse());
        }
    }

    public void EventFailureCallback(AdTraceEventFailure eventFailureData)
    {
        Debug.Log("Event tracking failed!");

        if (eventFailureData.Message != null)
        {
            Debug.Log("Message: " + eventFailureData.Message);
        }
        if (eventFailureData.Timestamp != null)
        {
            Debug.Log("Timestamp: " + eventFailureData.Timestamp);
        }
        if (eventFailureData.Adid != null)
        {
            Debug.Log("Adid: " + eventFailureData.Adid);
        }
        if (eventFailureData.EventToken != null)
        {
            Debug.Log("EventToken: " + eventFailureData.EventToken);
        }
        if (eventFailureData.CallbackId != null)
        {
            Debug.Log("CallbackId: " + eventFailureData.CallbackId);
        }
        if (eventFailureData.JsonResponse != null)
        {
            Debug.Log("JsonResponse: " + eventFailureData.GetJsonResponse());
        }

        Debug.Log("WillRetry: " + eventFailureData.WillRetry.ToString());
    }

    public void SessionSuccessCallback(AdTraceSessionSuccess sessionSuccessData)
    {
        Debug.Log("Session tracked successfully!");

        if (sessionSuccessData.Message != null)
        {
            Debug.Log("Message: " + sessionSuccessData.Message);
        }
        if (sessionSuccessData.Timestamp != null)
        {
            Debug.Log("Timestamp: " + sessionSuccessData.Timestamp);
        }
        if (sessionSuccessData.Adid != null)
        {
            Debug.Log("Adid: " + sessionSuccessData.Adid);
        }
        if (sessionSuccessData.JsonResponse != null)
        {
            Debug.Log("JsonResponse: " + sessionSuccessData.GetJsonResponse());
        }
    }

    public void SessionFailureCallback(AdTraceSessionFailure sessionFailureData)
    {
        Debug.Log("Session tracking failed!");

        if (sessionFailureData.Message != null)
        {
            Debug.Log("Message: " + sessionFailureData.Message);
        }
        if (sessionFailureData.Timestamp != null)
        {
            Debug.Log("Timestamp: " + sessionFailureData.Timestamp);
        }
        if (sessionFailureData.Adid != null)
        {
            Debug.Log("Adid: " + sessionFailureData.Adid);
        }
        if (sessionFailureData.JsonResponse != null)
        {
            Debug.Log("JsonResponse: " + sessionFailureData.GetJsonResponse());
        }

        Debug.Log("WillRetry: " + sessionFailureData.WillRetry.ToString());
    }

    private void DeferredDeeplinkCallback(string deeplinkURL)
    {
        Debug.Log("Deferred deeplink reported!");

        if (deeplinkURL != null)
        {
            Debug.Log("Deeplink URL: " + deeplinkURL);
        }
        else
        {
            Debug.Log("Deeplink URL is null!");
        }
    }
}
