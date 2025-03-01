using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Citizens
{
    // basically a singleton
    // manages buildings and the way citizens interact with them
    public class Building : MonoBehaviour
    {
        // private static int MAX_CITIZENS => 100;
        // private static int total_citizens = 0;

        private static List<GameObject> building_objects;
        private static LineRenderer path_renderer;
        private static Button road_button;
        public NavMeshAgent[] citizens;
        public static bool citizens_enabled;

        public static List<GameObject> go_citizens;

        public void push_citizen(GameObject go_citizen)
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

        public void TrackBuilding()
        {
            
        }

        public void RequestUpdate()
        {
            building_objects = FindObjectOfType<GridSystem>().GetBuildings();
        }

        public void UpdateCitizens()
        {
            int mod = building_objects.Count - 1;
            foreach (var citizen in citizens)
            {
                Assert.IsTrue(citizen.isOnNavMesh);
                if (citizen.remainingDistance < 0.25f)
                {
                    int rand = Random.Range(0, mod);
                    citizen.SetDestination(building_objects[rand].transform.position);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
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

            if (citizens_enabled)
            {
                UpdateCitizens();
            }
        }
    }
}