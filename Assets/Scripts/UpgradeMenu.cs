using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] UpgradeButton buttonTemplate;

    List<UpgradeButton> buttons = new();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 20; ++i)
        {
            SpawnUpgrade("" + i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnUpgrade(string test)
    {
        UpgradeButton b = Instantiate(buttonTemplate, content.transform);
        b.SetText(test);

        buttons.Add(b);
    }
}
