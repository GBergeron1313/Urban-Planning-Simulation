using UnityEngine;
using System.Collections.Generic;
/*using UnityEngine.AI;*/
/*using UnityEngine.Assertions;*/

namespace Citizens
{

    public class Building : MonoBehaviour
    {
        public static List<Vector3> building_positions = new List<Vector3>();

        private float last_citizen_update;

        public static void ClearBuildings()
        {
            building_positions.Clear();
            building_positions = new List<Vector3>();
        }

        public static void TrackPosition(Vector3 tracked)
        {
            building_positions.Add(tracked);
        }

        // Start is called before the first frame update
        void Start()
        {
            name = "Building";
            last_citizen_update = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
        }
    }
}
