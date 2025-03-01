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