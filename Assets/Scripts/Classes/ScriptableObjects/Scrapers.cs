using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScrapersObject", menuName = "Scriptable Objects/ScrapersObject")]
public class ScrapersObject : ScriptableObject {
    public List<Scraper> Scrapers = new List<Scraper>();
}
