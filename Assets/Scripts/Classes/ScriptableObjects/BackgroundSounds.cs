using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundSounds", menuName = "Scriptable Objects/BackgroundSounds")]
public class BackgroundSoundsObject : ScriptableObject {
    public List<BackgroundSound> data = new List<BackgroundSound>();
}
