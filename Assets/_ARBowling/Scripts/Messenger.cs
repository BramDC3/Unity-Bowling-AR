using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This script needs a Canvas with a TMP_Text to work
public class Messenger : MonoBehaviour
{
    public static Messenger Instance;
    [SerializeField] private TMP_Text _messageUI;
    private Queue<string> _messageQueue = new Queue<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void EnqueueMessage(string message, float messageTime)
    {
        _messageQueue.Enqueue(message);
        UpdateMessagesUI();
        Invoke("ClearOldestMessage", messageTime);
    }

    void UpdateMessagesUI()
    {
        _messageUI.text = "";
        foreach (var message in _messageQueue)
        {
            _messageUI.text += message + "\n";
        }
    }

    void ClearOldestMessage()
    {
        _messageQueue.Dequeue();
        UpdateMessagesUI();
    }

}
