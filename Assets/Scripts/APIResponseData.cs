using System;

[Serializable]
public class WeatherData
{
    public float temperature;
    public float windSpeed;
    public float windDirection;
    public int weathercode;
}

[Serializable]
public class APIResponseData
{
    public WeatherData current_weather;
}