using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[Serializable]
struct Parameter
{
    public string name;
    public string value;
}

public class APIManager : MonoBehaviour
{
    private UnityWebRequest _webRequest;
    private string _queryString;
    private string _reqURL;

    // Button color
    private Color _color;

    [SerializeField] private string _endPoint;
    [SerializeField] private List<Parameter> _parameters;

    // Object related to API : moon / sun / cloud
    [SerializeField] private GameObject _sun;
    [SerializeField] private GameObject _moon;
    [SerializeField] private GameObject _cloud;

    public void CallURL()
    {
        // Faire une get request
        GetQueryString();
        _reqURL = _endPoint + _queryString;
        StartCoroutine(GetRequest(_reqURL));
    }

    private IEnumerator GetRequest(string reqURL)
    {
        using (_webRequest = UnityWebRequest.Get(reqURL)) // Access a website and use UnityWebRequest.Get to download a page.
        {
            // Envoi de la requête
            yield return _webRequest.SendWebRequest();

            // Vérificationd es erreurs
            if (_webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                print("Il y a un problème de connexion");
            }
            else
            {
                HandleResponse(_webRequest);
                HandleMeteoResponse(_webRequest);
                HandleWeatherResponse(_webRequest);
            }

            // Traitement du résultat
        }
    }

    private void HandleResponse(UnityWebRequest webRequest)
    {
        print(webRequest.downloadHandler.text);
    }

    private void HandleColorResponse(UnityWebRequest webRequest)
    {
        ColorUtility.TryParseHtmlString(_webRequest.downloadHandler.text, out _color);
        GetComponent<Image>().color = _color;
        _color = GameObject.Find("Sphere").GetComponent<MeshRenderer>().material.color;
        print("ChangeColor invoked");
    }

    private void HandleMeteoResponse(UnityWebRequest webRequest)
    {
        APIResponseData apiResponseData = JsonUtility.FromJson<APIResponseData>(webRequest.downloadHandler.text);
        print("La température est de : " + apiResponseData.current_weather.temperature);
    }

    private void HandleWeatherResponse(UnityWebRequest webRequest)
    {
        APIResponseData apiResponseData = JsonUtility.FromJson<APIResponseData>(webRequest.downloadHandler.text);
        print("Le weather code est : " + apiResponseData.current_weather.weathercode);

        // 1, 2, 3	Mainly clear, partly cloudy, and overcast
        if (apiResponseData.current_weather.weathercode == 1)
        {
            _sun.SetActive(true);
        }

        if (apiResponseData.current_weather.weathercode == 2)
        {
            _cloud.SetActive(true);
        }

        if (apiResponseData.current_weather.weathercode == 3)
        {
            _moon.SetActive(true);
        }
    }

    private void GetQueryString()
    {
        _queryString = "";

        for (int i = 0; i < _parameters.Count; i++)
        {
            if (i == 0)
            {
                _queryString = "?";
            }
            else
            {
                _queryString = "&";
            }

            _queryString += _parameters[i].name + "=" + _parameters[i].value;
        }
    }
}
