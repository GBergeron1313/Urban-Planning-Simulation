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
        private static List<Vector3> building_positions = new List<Vector3>();
        private static LineRenderer path_renderer;
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

        public static void RedrawPaths()
        {
            // Bunch of line rendering options so the paths aren't stupid looking
            // By stupid looking I mean: barely visible, magenta, facing the camera like a doom sprite
            path_renderer.positionCount = building_objects.Count;
            path_renderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
            path_renderer.alignment = LineAlignment.TransformZ;
            path_renderer.startWidth = 0.5f;
            path_renderer.endWidth = 0.5f;
            path_renderer.material.color = Color.Lerp(Color.gray, Color.blue, 0.0625f);
            path_renderer.loop = true;

            // without this, Z-fighting occurs and the line starts looking glitchy
            var y_off = Vector3.up * 0.01f;

            for (int i = 0; i < building_objects.Count; i++)
            {
                path_renderer.SetPosition(i, building_objects[i].transform.position + y_off);
            }
        }

        public void TrackPosition(Vector3 tracked)
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
                    Debug.LogWarning(c.gameObject.transform.position);
                    building_positions.Add(c.gameObject.transform.position);
                }
            }
            Debug.LogWarning(JsonUtility.ToJson(building_positions));
        }

        public void UpdateCitizens()
        {
            Debug.LogWarning("Updating Citizens");
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
            road_button ??= GameObject.Find("RoadButton").GetComponent<Button>();
            road_button.onClick.AddListener(RequestUpdate);
            road_button.onClick.AddListener(RedrawPaths);
            building_objects ??= FindObjectOfType<GridSystem>().GetBuildings();
            path_renderer ??= gameObject.AddComponent<LineRenderer>();
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
