using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cities", menuName = "Scriptable Objects/Cities")]
public class CitiesObject : ScriptableObject {
    public List<City> data = new List<City>();
}
