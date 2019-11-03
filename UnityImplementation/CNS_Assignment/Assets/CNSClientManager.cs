using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CNSClientManager : MonoBehaviour
{
    public static CNSClientManager instance;
    public static CNSMessageClient activeSender;
    public static CNSMessageClient activeReceiver;
    public static bool active=true;
    public static int clientCount=0;

    public InputField messageField;
    public Text debugText;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void RegisterClient(CNSMessageClient client) 
    {
        client.GetComponent<Button>().onClick.AddListener(()=>instance.SetReceiver(client));
        client.clientID = clientCount;
        clientCount++;
    }

    public void SetReceiver(CNSMessageClient client) 
    {
        if (activeReceiver == client) 
        {
            activeReceiver = null;
            active = !active;
            return;
        }
        if (activeSender == client) 
        {
            activeSender = null;
            active = !active;
            return;
        }

        if (active) 
        {
            activeSender = client;
        }
        else 
        {
            activeReceiver = client;
        }
        active = !active;
    }

    public void SendMessage() 
    {
        if(activeSender == null || activeReceiver == null)
        {
            WriteLine("No sender/receiver!");
            return;
        }
        else if (instance.messageField.text == "") 
        {
            WriteLine("No text to be sent !");
            return;
        }

        activeSender.SendMessage(activeReceiver, instance.messageField.text);
    }

    public static void WriteLine(object message) 
    {
        instance.debugText.text = instance.debugText.text + "\n"+message.ToString() ;
    }
    public static void ClearLines() 
    {
        instance.debugText.text = "";
    }
}
