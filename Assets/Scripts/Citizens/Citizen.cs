using System;
using System.Collections.Generic;
using UnityEngine;

namespace Citizens
{
    public enum CitizenModel
    {
        female_casual = 0,
        female_dress,
        male_casual,
        male_suit,
        CITIZEN_MODELS_MAX
    }

    public struct CitizenInfo
    {
        public CitizenModel model_type;
    }


    public class Citizen : MonoBehaviour
    {
        public static bool citizens_enabled = false;
        public static List<GameObject> go_citizens = new List<GameObject>();

        public static Cell CitizenCellLocation(Vector3 position)
        {
            return new Cell();
        }

        public static void ClearCitizens()
        {
            foreach (var citizen in go_citizens)
            {
                Destroy(citizen);
            }
            go_citizens.Clear();
            go_citizens = new List<GameObject>();
        }

        private void Start()
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            throw new NotImplementedException();
        }
    }
}
