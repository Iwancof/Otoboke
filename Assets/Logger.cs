using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    static List<string> enableTags = new List<string>();
    public static void enable(string tag) {
        enableTags.Add(tag);
    }
    public static void disable(string tag) {
        if(enableTags.Contains(tag)) {
            enableTags.Remove(tag);
        }
    }
    public static void Log(string tag, string message) {
        if(enableTags.Contains(tag)) {
            Debug.Log($"{tag}: {message}");
        }
    }
}
