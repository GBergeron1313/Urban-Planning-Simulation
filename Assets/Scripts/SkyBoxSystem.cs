using UnityEngine;

public class SkyBoxSystem : MonoBehaviour
{
    // Sorry for changing this from camelCase to snake_case.
    private float rotation_speed = 1.0f;
    private Material skybox_material;
    private float starting_rotation;
    private float current_rotation;

    // Update is called once per frame
    private void Start()
    {
        skybox_material = RenderSettings.skybox;
        starting_rotation = skybox_material.GetFloat("_Rotation");
        current_rotation = starting_rotation;
    }

    void OnApplicationQuit()
    {
        // This prevents the Material file itself from being changed.
        // In turn, it prevents git from noticing the change to the 
        // material, and so now git doesn't ask us "hey, what about
        // this material file? Don't you want to commit that?".
        skybox_material.SetFloat("_Rotation", starting_rotation);
    }

    void Update()
    {
        // Talking to the GPU is expensive. I know this is
        // Unity—and that "computers are fast"—but trying to minimize
        // unnecessary traffic early on will help us later.
        current_rotation += rotation_speed * Time.deltaTime;
        skybox_material.SetFloat("_Rotation", current_rotation); // set
    }
}
