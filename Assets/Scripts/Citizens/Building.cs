using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Citizens
{

    public class Building : MonoBehaviour
    {
        public static List<Vector3> building_positions = new List<Vector3>();
        public static Building hovering;
        public static Building last_hovered;

        public string uuid;
        public string legible;
        public float air_pollution;
        public float noise_pollution;
        public float power_usage;
        public float max_capacity;
        public ZoneType building_type;
        public Cell attached_to;

        private Color highlighted;

        public Color normal_color
        {
            get { return normal_color; }
            set
            {
                normal_color = value;
                highlighted = Color.Lerp(normal_color, Color.black, 0.3f);
            }
        }

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
            resolve_name();
            building_positions.Add(gameObject.transform.position);
            print("Placed " + uuid);
        }

        private void resolve_name()
        {
            legible = $"{attached_to.cell_type} at {attached_to.location}";
        }

        private void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                hovering = null;
                last_hovered = this;
                return;
            }
            hovering = this;
        }

        private void OnMouseExit()
        {
            hovering = null;
            last_hovered = this;
        }

        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                last_hovered = this;
                hovering = null;
            }
            else
            {
                hovering = this;
            }
        }

        private void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (hovering is not null)
            {
                if (Cell.building_mode == BuildingMode.Removal)
                {
                    DestroyBuilding(hovering);
                    hovering = null;
                    last_hovered = null;
                }
            }
        }

        private static void DestroyBuilding(Building which)
        {
            print($"Removing {which}...");

            which.attached_to.SetCellTypeAndUpdate(CellType.None);
            bool r = Building.building_positions.Remove(which.gameObject.transform.position)
                || Building.building_positions.Remove(which.transform.position);
            print($"{(r ? "Removed" : "Couldn't Remove")}");
            Destroy(which.gameObject);
            Destroy(which);
        }

        private void OnMouseUp()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
