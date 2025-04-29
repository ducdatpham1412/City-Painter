using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScapeSounds", menuName = "Scriptable Objects/ScapeSounds")]
public class ScrapeSoundsObject : ScriptableObject {
    public List<ScrapeSound> data = new List<ScrapeSound>();
}
