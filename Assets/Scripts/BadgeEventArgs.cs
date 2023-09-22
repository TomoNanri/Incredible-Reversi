using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BadgeEventArgs : EventArgs
{
    public BadgeEventType EventType { get; }
    public BadgeEventArgs(BadgeEventType eventType)
    {
        EventType = eventType;
    }
}
