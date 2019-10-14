using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Send_To_Google_Sheet : MonoBehaviour
{
    public Text statusText;

    //GPS Coodinates
    public float latitude, longitude;

    [SerializeField]
    private string url = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSdlDczLLYOczZMXS4RhxM3SSX_j3bsTQZJqRvEo_gbd5CyPzg/formResponse";

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(StartGPS());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private IEnumerator StartGPS()
    {
        //Check if GPS is enabled
        if(!Input.location.isEnabledByUser)
        {
            statusText.text = "Location Services not enabled!";
            yield break;
        }

        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            statusText.text = "Initializing...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            statusText.text = "Initializing Timed Out";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            // Access granted and location value could be retrieved
            statusText.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude;

            //Write to the google sheet
            WWWForm googleForm = new WWWForm();
            googleForm.AddField("entry.488188203", latitude.ToString());
            googleForm.AddField("entry.2083580280", longitude.ToString());
            byte[] rawData = googleForm.data;
            WWW www = new WWW(url, rawData);
            yield return www;
            statusText.text += "\n Data Sent";
        }
    }
}
