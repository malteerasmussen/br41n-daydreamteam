using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
public class udpreceiver : MonoBehaviour
{

[Header("eeg")]
[SerializeField] private float BatteryLevel;
[SerializeField] private float[] eegChannels;
[SerializeField] private float[] channels;
//[SerializeField] private float averageDelta8;
[SerializeField] private float averageTheta8;
[SerializeField] private float averageAlpha8;
//[SerializeField] private float averageBetaLow8;
//[SerializeField] private float averageBetaMid8;
//[SerializeField] private float averageBetaHigh8;
//[SerializeField] private float averageGamme8;

// Start is called before the first frame update
private const int ListenPort = 1000;
private IPEndPoint _groupEp;
private Socket _socket;
byte[] receiveBufferByte;
float[] receiveBufferFloat;


public AudioSource chime;

void Start()
{
        eegChannels = new float[7];
        channels = new float[68];
        _groupEp = new IPEndPoint(IPAddress.Any, ListenPort);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {
                Blocking = false
        };
        _socket.Bind(_groupEp);
        receiveBufferByte = new byte[1024];
        receiveBufferFloat = new float[receiveBufferByte.Length / sizeof(float)];
        //  StartCoroutine(getAverageRealtime());
}

void Awake(){
        //    StartCoroutine(getAverageRealtime());
}
[Header("Currents")]
public float CurrentAlphaPower = 0;
public float CurrentATPower = 0;
// Update is called once per frame
private float count;
public int MovingAverageLength = 1000;
private float movingAverageAll;
private float movingAverageAlpha;
private float movingAverageAlphaTheta;
private float movingRelativePower;
[Header("Moving Averages")]
public float movAll;
public float movAlpha;
public float movAlphaTheta;
public float movPowAT;
public string[] data = new string[70];
public GameObject plane;
void Update()
{
        //floattimeSinceLastUpdate = Time.deltaTime;
        count++;
        //Debug.Log("Time.deltaTime;" + " " + Time.deltaTime);
        if (Input.GetKeyDown("c"))
        {
                print("c key was pressed - closed");
                StartCoroutine(GetAverageClosed());
        }
        if (Input.GetKeyDown("o"))
        {
                print("space key was pressed - open");
                StartCoroutine(GetAverageOpen());
        }
        try {
                byte[] receiveBufferByte = new byte[1024];
                int numberOfBytesReceived = _socket.Receive(receiveBufferByte);
                //Debug.Log(receiveBufferByte);
                string stringByte= BitConverter.ToString(receiveBufferByte);
                string message = Encoding.ASCII.GetString(receiveBufferByte);
                //Debug.Log(message);
                if (numberOfBytesReceived > 0)
                {
                        //  List<string> names = message.Split(',').Reverse().ToList();
                        data = message.Split(',');
                        //convert byte array to float array
                        for (int i = 0; i < numberOfBytesReceived / sizeof(float); i++)
                        {

                                receiveBufferFloat[i] = BitConverter.ToSingle(receiveBufferByte, i *
                                                                              sizeof(float));
                                if (i == 58) {
                                        //  Debug.Log(receiveBufferByte[i].ToString("n2"));
                                }
                                channels[i] = receiveBufferFloat[i];
                                //  if(i+1< numberOfBytesReceived / sizeof(float))
                                //        Debug.Log();
                                //  Debug.Log(receiveBufferFloat[i].ToString("n2"));
                                //  else
                                //      Debug.Log();
                                //    Debug.Log(receiveBufferFloat[i].ToString("n2"));
                        }
                }
        }
        catch (Exception e) {
                // Debug.Log(e);
        }
        float totalBands = 0;

        for (int i = 0; i < 7; i++) {
                eegChannels[i] = float.Parse(data[i + 56]);
                totalBands += float.Parse(data[i + 56]);
        }
        float alpha = float.Parse(data[58]);
        CurrentAlphaPower = ((float.Parse(data[58])) / (totalBands));
        //Debug.Log("CurrentAlphaPower: " + CurrentAlphaPower + " alpha: " + receiveBufferFloat[58] + " totalBands: " + totalBands);
        averageAlpha8 = float.Parse(data[58]);
        averageTheta8 = float.Parse(data[57]);
        CurrentATPower = ((float.Parse(data[58]) + float.Parse(data[57])) / (totalBands));
        //Debug.Log(BatteryLevel);
        //Debug.Log(receiveBufferFloat[0]);
        //Debug.Log(receiveBufferFloat[56].ToString() + " " + receiveBufferFloat[57].ToString() + " " + receiveBufferFloat[58].ToString() + " " + receiveBufferFloat[59].ToString()  + " " + receiveBufferFloat[60].ToString() );

        if (count > MovingAverageLength) {
                movingAverageAlpha = movingAverageAlpha + (averageAlpha8 - movingAverageAlpha) / (MovingAverageLength + 1);
                movingAverageAll = movingAverageAll + (totalBands - movingAverageAll) / (MovingAverageLength + 1);

                movingAverageAlphaTheta = movingAverageAlphaTheta + (totalBands - movingAverageAlphaTheta) / (MovingAverageLength + 1);
        }
        else {
                movingAverageAlpha += averageAlpha8;
                movingAverageAll += totalBands;
                movingAverageAlphaTheta += (averageTheta8 + averageAlpha8);
                if (count == MovingAverageLength) {
                        movingAverageAlpha = movingAverageAlpha / count;
                        movingAverageAll = movingAverageAll / count;
                        movingAverageAlphaTheta = movingAverageAlphaTheta / count;
                }

        }

        movAll = (movingAverageAll/MovingAverageLength) * 10000f;
        movAlpha = (movingAverageAlpha/MovingAverageLength) * 10000f;
        movAlphaTheta = (movingAverageAlphaTheta/MovingAverageLength) * 1000;
        movPowAT = movAlpha / movAll;


        var planeRenderer = plane.GetComponent<Renderer>();
        fromoto1 = Map(movPowAT,RelativePowerAlphaClosed, RelativePowerAlphaOpen, 0.0f, 1.0f);
      //  planeRenderer.material.SetColor("_Color", new Color(1,1,1,fromoto1));
}

[Range(-1.0f, 1.0f)]
public float fromoto1;
public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return(NewValue);
}

public float Map(float x, float in_min, float in_max, float out_min, float out_max)
{
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}
List<float> averageDeltaBufferOpen = new List<float>();
List<float> averageThetaBufferOpen = new List<float>();
List<float> averageAlphaBufferOpen = new List<float>();
List<float> averageBetaLowBufferOpen = new List<float>();
List<float> averageBetaMidBufferOpen = new List<float>();
List<float> averageBetaHighBufferOpen = new List<float>();
List<float> averageGammaBufferOpen = new List<float>();
float time2test = 20;
public float AverageAlpha8AverageOpen;
public float RelativePowerAlphaOpen;
public float RelativePowerAlphaThetaOpen;

public float syncedyet;
private IEnumerator GetAverageOpen() {
        print(Time.time);
        averageDeltaBufferOpen = new List<float>();
        averageThetaBufferOpen = new List<float>();
        averageAlphaBufferOpen = new List<float>();
        averageBetaLowBufferOpen = new List<float>();
        averageBetaMidBufferOpen = new List<float>();
        averageBetaHighBufferOpen = new List<float>();
        averageGammaBufferOpen = new List<float>();
        // add to list for timeTest seconds
        float timeTest = time2test;
        while (timeTest > 0 ) {
                Debug.Log(timeTest);
                timeTest -= Time.deltaTime;
                //averageAlphaBufferOpen.Add(averageAlpha8);
                averageDeltaBufferOpen.Add(eegChannels[0]);
                averageThetaBufferOpen.Add(eegChannels[1]);
                averageAlphaBufferOpen.Add(eegChannels[2]);
                averageBetaLowBufferOpen.Add(eegChannels[3]);
                averageBetaMidBufferOpen.Add(eegChannels[4]);
                averageBetaHighBufferOpen.Add(eegChannels[5]);
                averageGammaBufferOpen.Add(eegChannels[6]);
                yield return null;
        }
        // calc average
        float alphaTotal = 0;
        float allBandsTotal = 0;
        float thetaTotal = 0;
        foreach(float a in averageAlphaBufferOpen) {
                alphaTotal += a;
                allBandsTotal += a;
        }
        foreach(float a in averageDeltaBufferOpen) {
                allBandsTotal += a;
        }
        foreach(float a in averageThetaBufferOpen) {
                thetaTotal += a;
                allBandsTotal += a;
        }
        foreach(float a in averageBetaLowBufferOpen) {
                allBandsTotal += a;
        }
        foreach(float a in averageBetaMidBufferOpen) {
                allBandsTotal += a;
        }
        foreach(float a in averageBetaHighBufferOpen) {
                allBandsTotal += a;
        }
        foreach(float a in averageGammaBufferOpen) {
                allBandsTotal += a;
        }
        //    total /= averageAlphaBufferOpen.Count;
        //RelativePowerAlphaOpen =((alphaTotal/allBandsTotal)/averageGammaBufferClosed.Count) * 10000;
        RelativePowerAlphaOpen =((alphaTotal/allBandsTotal));
        RelativePowerAlphaThetaOpen = (alphaTotal + thetaTotal) / allBandsTotal;
        chime.Play();
        syncedyet  = syncedyet + 0.5f;
        //  AverageAlpha8AverageOpen = total;
        //timeTest = 60;
}

List<float> averageDeltaBufferClosed = new List<float>();
List<float> averageThetaBufferClosed = new List<float>();
List<float> averageAlphaBufferClosed = new List<float>();
List<float> averageBetaLowBufferClosed = new List<float>();
List<float> averageBetaMidBufferClosed = new List<float>();
List<float> averageBetaHighBufferClosed = new List<float>();
List<float> averageGammaBufferClosed = new List<float>();
public float AverageAlpha8AverageClosed;
public float RelativePowerAlphaClosed;
public float RelativePowerAlphaThetaClosed;
public bool calibrationFinished;
private IEnumerator GetAverageClosed() {
        print(Time.time);
        float timeTest = time2test;
        averageDeltaBufferClosed = new List<float>();
        averageThetaBufferClosed = new List<float>();
        averageAlphaBufferClosed = new List<float>();
        averageBetaLowBufferClosed = new List<float>();
        averageBetaMidBufferClosed = new List<float>();
        averageBetaHighBufferClosed = new List<float>();
        averageGammaBufferClosed = new List<float>();
        // add to list for timeTest seconds
        while (timeTest > 0 ) {
                //Debug.Log(timeTest);
                timeTest -= Time.deltaTime;
                averageDeltaBufferClosed.Add(eegChannels[0]);
                averageThetaBufferClosed.Add(eegChannels[1]);
                averageAlphaBufferClosed.Add(eegChannels[2]);
                averageBetaLowBufferClosed.Add(eegChannels[3]);
                averageBetaMidBufferClosed.Add(eegChannels[4]);
                averageBetaHighBufferClosed.Add(eegChannels[5]);
                averageGammaBufferClosed.Add(eegChannels[6]);
                //averageAlphaBufferClosed.Add(averageAlpha8);
                yield return null;
        }
        // calc average
        float alphaTotal = 0;
        float allBandsTotal = 0;
        float thetaTotal = 0;
        foreach(float a in averageAlphaBufferClosed) {
                alphaTotal += a;
                allBandsTotal += a;
        }
        foreach(float a in averageDeltaBufferClosed) {
                allBandsTotal += a;
        }
        foreach(float a in averageThetaBufferClosed) {
                thetaTotal += a;
                allBandsTotal += a;
        }
        foreach(float a in averageBetaLowBufferClosed) {
                allBandsTotal += a;
        }
        foreach(float a in averageBetaMidBufferClosed) {
                allBandsTotal += a;
        }
        foreach(float a in averageBetaHighBufferClosed) {
                allBandsTotal += a;
        }
        foreach(float a in averageGammaBufferClosed) {
                allBandsTotal += a;
        }
        //    total /= averageAlphaBufferClosed.Count;
        // RelativePowerAlphaClosed = ((alphaTotal/allBandsTotal)/averageGammaBufferClosed.Count) * 10000;
        RelativePowerAlphaClosed = ((alphaTotal/allBandsTotal));
        RelativePowerAlphaThetaClosed = (alphaTotal + thetaTotal) / allBandsTotal;
        chime.Play();
        syncedyet  = syncedyet + 0.5f;
        Invoke("FinishCalibration", 10);
}

void FinishCalibration()
{
  calibrationFinished = true;
}

void calcPowerRealTime(){

}

private IEnumerator getAverageRealtime() {
        print(Time.time);
        float timeTest = time2test;
        // add to list for timeTest seconds
        while (timeTest > 0 ) {

                Debug.Log(timeTest);
                timeTest -= Time.deltaTime;
                //averageAlphaBufferClosed.Add(averageAlpha8);
                yield return null;
                //  StartCoroutine(getAverageRealtime());
        }
        timeTest = 4;
        Debug.Log("RESEETING" + " " + timeTest);
        //StartCoroutine(getAverageRealtime());

}

}
