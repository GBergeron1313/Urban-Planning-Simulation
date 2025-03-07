using System;
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

    [Serializable]
    public struct CitizenInfo
    {
        public CitizenModel model_type;
    }


    public class Citizen : MonoBehaviour
    {
        public static bool citizens_enabled = false;

        public static Cell CitizenCellLocation(Vector3 position) {
            return new Cell();
        }
        
        public static void ClearCitizens()
        {
            var citizens = GameObject.FindGameObjectsWithTag("Citizens");
            foreach (var citizen in citizens)
            {
                Destroy(citizen);
            }
            citizens_enabled = false;
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
