using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnOnKey : MonoBehaviour {
    [SerializeField]
    private KeyCode[] m_Codes;
    [SerializeField]
    private GameObject[] m_Prefabs;
    
	void Update () {
        int Length = Mathf.Min(m_Codes.Length, m_Prefabs.Length);

	    for(int Index = 0; Index < Length; Index++) {
            if(Input.GetKeyDown(m_Codes[Index])) {
                Ray MouseWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit Info;
                if(Physics.Raycast(MouseWorldRay, out Info, 1000.0f)) {
                    GameObject NewObject = Instantiate(m_Prefabs[Index]);
                    NewObject.transform.position = Info.point + Info.normal * 1.0f;
                }
            }
        }
	}
}
