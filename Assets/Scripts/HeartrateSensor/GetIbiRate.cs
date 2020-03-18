//////////////////////////////
//don't look at my garbage!!//
////Author: Lars Hulsmans.////
//////////////////////////////

using System.IO.Ports;
using System.Text;
using UnityEngine;

public class GetIbiRate : MonoBehaviour
{

    private SerialPort mySerialPort;

    public int ibiValue;
    public int heartrate;

    public string choice;
    public void Start()
    {
        try
        {
            mySerialPort = new SerialPort(choice, 115200, Parity.None, 8, StopBits.One);

            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = false;


            if (!mySerialPort.IsOpen)
            {
                mySerialPort.Open();
                Debug.Log(string.Format("connected to port: {0}", choice));
            }
        }
        catch
        {
            Debug.LogError(string.Format("a connection to {0} could not be made", choice));
        }
    }

    private void Update()
    {
        if (mySerialPort != null)
        {
            if (mySerialPort.IsOpen)
            {

                byte[] output = new byte[mySerialPort.BytesToRead];
                mySerialPort.Read(output, 0, output.Length);
                string val = Encoding.UTF8.GetString(output, 0, output.Length);
                if (!string.IsNullOrEmpty(val))
                {
                    int.TryParse(val, out ibiValue);
                    if(ibiValue > 0)
                    {
                        heartrate = (60000 / ibiValue);
                    }
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (mySerialPort.IsOpen)
        {
            mySerialPort.Close();
        }
    }
}
