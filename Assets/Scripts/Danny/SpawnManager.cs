using BuildingUtils;
using Citizens;
using UnityEngine;
/*using UnityEngine.AI;*/
/*using UnityEngine.Assertions;*/
using UnityEngine.UI;

namespace UrbanPlanning
{
    public class SpawnManager : MonoBehaviour
    {
        public GameObject[] npcPrefabs; // Assign 4 prefabs in Inspector
        public Button npcButton; // Assign your button in Inspector
        public Transform spawnArea; // Assign an area where NPCs will spawn
        public static readonly int npcCount = 100; // Number of NPCs to spawn
        public static bool spawned_and_moving = false;

        public GameObject buildingSpawner;

        void Start()
        {
            name = "SpawnManager";
            npcButton.onClick.AddListener(SpawnNPCs);

        }

        public void SpawnNPCsFrom(string[] citizen_names, Vector3[] positions, Vector3[] destinations)
        {
            buildingSpawner = GameObject.Find("CreateBuilding");

            if (Building.building_positions.Count == 0)
                throw new UnityException("Can't spawn npcs when there are no buildings");

            // Don't let them spawn more NPCs
            npcButton.interactable = false;
            npcButton.onClick = null;
            npcButton.enabled = false;

            for (int i = 0; i < buildingSpawner.GetComponent<CreateBuilding>().totalPop; i++)
            {
                string[] name_id_index = citizen_names[i].Split('_');
                CitizenModel prefab_index = CitizenModel.Parse<CitizenModel>(name_id_index[2]);
                GameObject npc =
                    Instantiate(npcPrefabs[((int)prefab_index)],
                            positions[i],
                            Quaternion.identity);
                npc.name = citizen_names[i];
                npc.tag = "Citizens";

                if (npc is null)
                {
                    throw new UnityException("NPCAnimator was null");
                }

                Citizen citizen = npc.AddComponent<Citizen>()
                    .with_model(prefab_index)
                    .with_position(positions[i])
                    .with_destination(destinations[i])
                    .with_enabled_movement(
                            SimCore.Instance.state
                            == SimState.Running);
            }

            Citizen.citizens_enabled = true;
        }

        public void SpawnNPCs()
        {
            buildingSpawner = GameObject.Find("CreateBuilding");

            if (Building.building_positions.Count == 0)
                throw new UnityException("Can't spawn npcs when there are no buildings");

            // Don't let them spawn more NPCs
            npcButton.interactable = false;
            npcButton.onClick = null;
            npcButton.enabled = false;

            for (int i = 0; i < buildingSpawner.GetComponent<CreateBuilding>().totalPop; i++)
            {
                int rand_prefab_idx = Random.Range(0, npcPrefabs.Length);
                int rand_position_idx = Random.Range(0, Building.building_positions.Count);
                GameObject npc = Instantiate(
                        npcPrefabs[rand_prefab_idx],
                        Building.building_positions[rand_position_idx],
                        Quaternion.identity);

                // For Saving and reloading, each citizen needs a unique name.
                // Saving the prefab index is somewhat hacky, but it works for now.
                // TODO: Make a real "Citizen" class to store this kind of info.
                npc.name = $"Citizen_{i}_{rand_prefab_idx}";

                npc.tag = "Citizens";

                Citizen citizen = npc.AddComponent<Citizen>()
                    .with_model((CitizenModel)rand_prefab_idx)
                    .with_position(Building.building_positions[rand_position_idx])
                    .with_enabled_movement(
                            SimCore.Instance.state
                            == SimState.Running);
            }

            Citizen.citizens_enabled = true;
        }
    }
}
