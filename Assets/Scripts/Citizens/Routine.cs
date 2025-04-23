using UnityEngine;
using System.Linq;

namespace Citizens
{

    [System.Serializable]
    public class PersonalInfo
    {

        public Vector3 home;
        public Vector3 work;

    }


    public class Routine : MonoBehaviour
    {

        private Citizen self;
        private PersonalInfo info;

        void Awake()
        {
            self = GetComponent<Citizen>();
            info = new PersonalInfo();
        }

        // Start is called before the first frame update
        void Start()
        {

            info.home = Cell.all_cells
                .Where(cell => cell.contents != null && cell.contents.model.is_residential())
                .Select(cell => cell.transform.position)
                .First();

            info.work = Cell.all_cells
                .Where(cell => cell.contents != null && cell.contents.model.is_business())
                .Select(cell => cell.transform.position)
                .DefaultIfEmpty(info.home)
                .First();

            Debug.Log($"{name}'s Routine: {JsonUtility.ToJson(info)}");

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
