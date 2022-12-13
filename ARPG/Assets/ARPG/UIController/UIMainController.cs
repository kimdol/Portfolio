using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG.Ingame.UIControll
{
    public enum UIType : int
    {
        StaticInventory = 0,
        DynamicInventory = 1,
        StoreInventory = 2,
        Leaderboard = 3,
        Default,
    }

    public class UIMainController : MonoBehaviour
    {
        [Header("Default UIs: StaticInventory = 0, DI = 1, SI = 2, L = 3")]
        public GameObject[] UIs = null;

        [SerializeField]
        private StatsObject playerStats;

        // Update is called once per frame
        void Update()
        {
            if (playerStats.Health <= 0)
            {
                //Debug.Log("Leaderboard Enable");
                UIs[(int)UIType.Leaderboard].SetActive(true);
            }

            if (UIs[(int)UIType.StoreInventory].GetComponent<RectTransform>().anchoredPosition.y > -415.0f &&
                UIs[(int)UIType.StaticInventory].activeSelf)
            {
                //Debug.Log("StaticInventory Disable");
                UIs[(int)UIType.StaticInventory].GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(-700.0f, 0.0f, 0.0f);
            }
        }

        public void OnClickInventory()
        {
            if (UIs[(int)UIType.DynamicInventory].GetComponent<RectTransform>().anchoredPosition.x < 300.0f)
            {
                // 화면 밖으로 내보냄
                UIs[(int)UIType.DynamicInventory].GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(700.0f, 50.0f, 0.0f);
            }
            else
            {
                UIs[(int)UIType.DynamicInventory].GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(-35.0f, 50.0f, 0.0f);
            }
            
        }

        public void OnClickEquipment()
        {
            if (UIs[(int)UIType.StaticInventory].GetComponent<RectTransform>().anchoredPosition.x > -200.0f)
            {
                // 화면 밖으로 내보냄
                UIs[(int)UIType.StaticInventory].GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(-700.0f, 0.0f, 0.0f);
            }
            else
            {
                UIs[(int)UIType.StaticInventory].GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(87.0f, 0.0f, 0.0f);
            }
        }

        public void OnClickHappy()
        {
            UIs[(int)UIType.Leaderboard].SetActive(true);
        }


    }
}

