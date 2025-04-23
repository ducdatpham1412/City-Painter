using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SfxSounds", menuName = "Scriptable Objects/SfxSounds")]
public class SfxSoundsObject : ScriptableObject {
    public List<SfxSound> data = new List<SfxSound>();
}
