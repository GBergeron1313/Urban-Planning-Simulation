using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Citizens
{

    public class Building : MonoBehaviour
    {
        public static List<Vector3> building_positions = new List<Vector3>();

        public string uuid;
        public float air_pollution;
        public float noise_pollution;
        public float power_usage;
        public float max_capacity;
        public ZoneType building_type;

        public Cell attached_to;

        public static void ClearBuildings()
        {
            building_positions.Clear();
            building_positions = new List<Vector3>();
        }

        void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            uuid = Random.Range(int.MinValue, int.MaxValue).ToString();
            Assert.IsNotNull(attached_to);
            building_positions.Add(gameObject.transform.position);
            print("Placed " + uuid);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
