using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace BuildingUtils
{

    public enum BuildingModel
    {
        NONE,

        BUILDING_MIN,
        Bank,
        Flat_1,
        Flat_2,
        House_1,
        House_2,
        House_3,
        Shop,
        BUILDING_MAX,

        ROAD_MIN,
        Deadend,
        Straight,
        Curve,
        ThreeWay,
        FourWay,
        ROAD_MAX,
    }

    public enum Rotation
    {
        CW90,  // Clockwise by 90 degrees
        CCW90, // Counter-Clockwise by 90 degrees
    }

    public class BuildingInfo
    {
        public float air_pollution;
        public float noise_pollution;
        public float power_usage;
        public float max_capacity;
    }

    [System.Serializable]
    public class BuildingSerial
    {
        public string legible;
        public BuildingInfo info;
        public Cell attached_to;
        public BuildingModel model;
        public Vector3 rotation;
    }

    public class Building : MonoBehaviour
    {
        public static List<Vector3> building_positions = new List<Vector3>();
        public static Building hovering;
        public static Building last_hovered;

        public string legible;
        public BuildingInfo info;
        public float air_pollution;
        public float noise_pollution;
        public float power_usage;
        public float max_capacity;
        public Cell attached_to;
        public BuildingModel model;
        /*private Vector3 applied;*/

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
            Assert.IsNotNull(attached_to);
            resolve_name();
            /*applied = new Vector3();*/
        }

        public void make_connection(Cell cell)
        {
            this.attached_to = cell;
            cell.contents = this;
        }

        public void set_model_update_info(BuildingModel model)
        {
            this.model = model;
            this.info = model.get_building_info();
        }

        private void resolve_name()
        {
            legible = $"{model} at {attached_to.location}";
            legible += $" {transform.rotation}";
        }

        public Quaternion get_rotation()
        {
            return transform.rotation;
        }

        public void exp_apply_rotation(Rotation rot)
        {
            switch (rot)
            {
                case Rotation.CW90:
                    transform.Rotate(0, 90, 0);
                    break;
                case Rotation.CCW90:
                    transform.Rotate(0, -90, 0);
                    break;
            }
        }
        public void apply_rotation(Quaternion quat)
        {
            transform.rotation = quat;
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
            GameObject[] mouseText = GameObject.FindGameObjectsWithTag("MouseText");
            mouseText[0].GetComponent<MouseTestScript>().SetText("Hold Q to Rotate");
        }

        private void OnMouseExit()
        {
            hovering = null;
            last_hovered = this;
            GameObject[] mouseText = GameObject.FindGameObjectsWithTag("MouseText");
            mouseText[0].GetComponent<MouseTestScript>().SetText("");
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
                }
                if (Cell.building_mode == BuildingMode.PlacingBuilding
                    || Cell.building_mode == BuildingMode.PlacingRoad)
                {
                    if (Input.GetKey(KeyCode.Q))
                    {
                        exp_apply_rotation(Rotation.CW90);
                    }
                    resolve_name();
                }
            }
        }

        private static void DestroyBuilding(Building which)
        {
            which.attached_to.cell_type = CellType.None;
            which.attached_to.SetWalkableAndUpdate(false);
            bool r = Building.building_positions.Remove(which.gameObject.transform.position)
                || Building.building_positions.Remove(which.transform.position);
            print($"Removing {which} {(r ? "Succeeded" : "Failed")}");
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
