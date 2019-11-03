using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System;

public class CNSMessageClient : MonoBehaviour
{
    private Dictionary<CNSMessageClient, MessageSendingParams> outgoingMessages = new Dictionary<CNSMessageClient, MessageSendingParams>();
    private Dictionary<CNSMessageClient, MessageReceivingParams> incomingMessages = new Dictionary<CNSMessageClient, MessageReceivingParams>();

    public Action<string> OnMessageRecievedCallback;
    public Action OnMessageFailedCallback;
    public int clientID;
    private Image image;

    public void Start()
    {
        CNSClientManager.RegisterClient(this);

        GetComponentInChildren<Text>().text = "Client No : " + clientID;
        OnMessageFailedCallback += () => LogOut("Failed to verify message");
        OnMessageRecievedCallback += SuccessfullMessage;
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (CNSClientManager.activeReceiver == this)
        {
            image.color = Color.red;
        }
        else if (CNSClientManager.activeSender == this)
        {
            image.color = Color.green;
        }
        else
        {
            image.color = Color.white;
        }
    }

    private void SuccessfullMessage(string message) 
    {
        
    }
    private void LogOut(string message) 
    {
        CNSClientManager.WriteLine(message);
    }


    public void AddMessageReceiver(CNSMessageClient other) 
    {
        if (outgoingMessages.ContainsKey(other))
            return;
        MessageSendingParams p = new MessageSendingParams();
        outgoingMessages.Add(other, p);
        other.AddMessageSender(this, new MessageReceivingParams(p.publicKey, p.encryptionKey, p.encryptionIV));
    }
    public void AddMessageSender(CNSMessageClient other,MessageReceivingParams receivingParams) 
    {
        if (incomingMessages.ContainsKey(other))
            return;
        incomingMessages.Add(other, receivingParams);
    }

    public void SendMessage(CNSMessageClient receiver,string message) 
    {
        AddMessageReceiver(receiver);
        MessageSendingParams p = outgoingMessages[receiver];
        string mess = p.SendMessage(message);
        receiver.ReceiveMessage(this,mess);
    }
    public void ReceiveMessage(CNSMessageClient sender,string cryptText) 
    {
        MessageReceivingParams p = incomingMessages[sender];
        string message;
        if(p.DecryptMessage(cryptText,out message)) 
        {
            OnMessageRecievedCallback?.Invoke(message);
            CNSClientManager.WriteLine("Received a verified message at Client " + clientID + " from Client " + sender.clientID + " : " + message );
        }
        else 
        {
            OnMessageFailedCallback?.Invoke();
        }
    }

}


public class MessageSendingParams 
{
    private RSAParameters privateKey;
    public RSAParameters publicKey;

    public string encryptionKey="";
    public string encryptionIV="";
    public MessageSendingParams() 
    {
        using(RSACryptoServiceProvider rsa= new RSACryptoServiceProvider()) 
        {
            privateKey = rsa.ExportParameters(true);
            publicKey = rsa.ExportParameters(false);
        }
        using(DESCryptoServiceProvider des = new DESCryptoServiceProvider()) 
        {
            des.GenerateIV();
            des.GenerateKey();
            encryptionKey = Convert.ToBase64String(des.Key);
            encryptionIV = Convert.ToBase64String(des.IV);
        }
    }
    public string SendMessage(string plainText) 
    {
        return CNS.GenerateMessage(plainText, privateKey, encryptionKey, encryptionIV);
    }
}

public class MessageReceivingParams 
{
    public RSAParameters publicKey;

    private string encryptionKey;
    private string encryptionIV ;
    public void SetEncryptionKey(string key)
    {
        encryptionKey = key;
    }
    public void SetEncryptionIV(string IV)
    {
        encryptionIV = IV;
    }

    public MessageReceivingParams(RSAParameters publicKey)
    {
        this.publicKey = publicKey;
    }

    public MessageReceivingParams(RSAParameters publicKey,string key,string IV)
    {
        this.publicKey = publicKey;
        encryptionKey = key;
        encryptionIV = IV;
    }


    public bool DecryptMessage(string cipherText,out string message) 
    {
        return CNS.ReproduceMessage(cipherText, out message, publicKey, encryptionKey, encryptionIV);
    }

}