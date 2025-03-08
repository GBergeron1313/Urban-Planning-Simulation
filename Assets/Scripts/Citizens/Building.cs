using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Citizens
{

    public class Building : MonoBehaviour
    {
        // private static int MAX_CITIZENS => 100;
        // private static int total_citizens = 0;

        private static List<GameObject> building_objects;
        public static List<Vector3> building_positions = new List<Vector3>();
        private static Button road_button;
        public NavMeshAgent[] citizens;

        private float last_citizen_update;

        public static List<GameObject> go_citizens;

        public static void ClearBuildings()
        {
            building_objects.Clear();
            building_positions.Clear();
        }

        public void AddCitizen(GameObject go_citizen)
        {
            go_citizens.Add(go_citizen);
        }

        public static void TrackPosition(Vector3 tracked)
        {
            building_positions.Add(tracked);
        }

        public void RequestUpdate()
        {
            GridSystem grid = GameObject.Find("Grid").GetComponent<GridSystem>();
            List<GameObject> cells = grid.GetCells();
            building_positions.Clear();
            foreach (var cell in cells)
            {
                Cell c = cell.GetComponent<Cell>();
                if (c.cell_type == CellType.Building)
                {
                    building_positions.Add(c.gameObject.transform.position);
                }
            }
        }

        public void UpdateCitizens()
        {
            int mod = building_positions.Count;
            foreach (var citizen in go_citizens)
            {
                var nma = citizen.GetComponent<NavMeshAgent>();
                Assert.IsTrue(nma.isOnNavMesh);
                if (nma.remainingDistance < 0.25f)
                {
                    int rand = Random.Range(0, mod);
                    nma.SetDestination(building_positions[rand]);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            name = "Building";
            last_citizen_update = Time.time;
            building_objects ??= FindObjectOfType<GridSystem>().GetBuildings();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                RequestUpdate();
            }

            if (Citizen.citizens_enabled)
            {
                if (Time.time - last_citizen_update > 5.0f)
                {
                    last_citizen_update = Time.time;
                    UpdateCitizens();
                }
            }
        }
    }
}
