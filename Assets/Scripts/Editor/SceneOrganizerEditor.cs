using UnityEngine;
using UnityEditor;

public class SceneOrganizerEditor
{
    [MenuItem("Tools/Organize Scene")]
    static void OrganizeScene()
    {
        OrganizeWalls();
        CreateVariedObstacles();
        SpreadCollectibles();
        SpreadDynamicBoxes();
        FixPetFollower();
        KeepOnlyOnePet();
        EditorUtility.DisplayDialog("Scene Organized", "Walls positioned at edges, obstacles reshaped, collectibles spread out, and pet fixed!", "OK");
    }

    static void OrganizeWalls()
    {
        GameObject westWall = GameObject.Find("Walls/West Wall");
        GameObject eastWall = GameObject.Find("Walls/East Wall");
        GameObject southWall = GameObject.Find("Walls/South Wall");
        GameObject northLeft = GameObject.Find("Walls/North Wall Left");
        GameObject northRight = GameObject.Find("Walls/North Wall Right");

        if (westWall != null)
        {
            Undo.RecordObject(westWall.transform, "Organize West Wall");
            westWall.transform.localPosition = new Vector3(-15f, 1f, 0f);
            westWall.transform.localScale = new Vector3(0.5f, 2f, 30f);
            westWall.transform.localRotation = Quaternion.identity;
        }

        if (eastWall != null)
        {
            Undo.RecordObject(eastWall.transform, "Organize East Wall");
            eastWall.transform.localPosition = new Vector3(15f, 1f, 0f);
            eastWall.transform.localScale = new Vector3(0.5f, 2f, 30f);
            eastWall.transform.localRotation = Quaternion.identity;
        }

        if (southWall != null)
        {
            Undo.RecordObject(southWall.transform, "Organize South Wall");
            southWall.transform.localPosition = new Vector3(0f, 1f, -15f);
            southWall.transform.localScale = new Vector3(30f, 2f, 0.5f);
            southWall.transform.localRotation = Quaternion.identity;
        }

        if (northLeft != null)
        {
            Undo.RecordObject(northLeft.transform, "Organize North Wall Left");
            northLeft.transform.localPosition = new Vector3(-7.5f, 1f, 15f);
            northLeft.transform.localScale = new Vector3(15f, 2f, 0.5f);
            northLeft.transform.localRotation = Quaternion.identity;
        }

        if (northRight != null)
        {
            Undo.RecordObject(northRight.transform, "Organize North Wall Right");
            northRight.transform.localPosition = new Vector3(7.5f, 1f, 15f);
            northRight.transform.localScale = new Vector3(15f, 2f, 0.5f);
            northRight.transform.localRotation = Quaternion.identity;
        }

        Debug.Log("Walls positioned at the edges of the 30x30 arena!");
    }

    static void CreateVariedObstacles()
    {
        GameObject cube1 = GameObject.Find("ground/Cube (1)");
        GameObject cube2 = GameObject.Find("ground/Cube (2)");
        GameObject cube3 = GameObject.Find("ground/Cube (3)");

        if (cube1 != null)
        {
            Undo.RecordObject(cube1.transform, "Create Tall Pillar");
            cube1.transform.localPosition = new Vector3(-3f, 1f, 3f);
            cube1.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);
            cube1.transform.localScale = new Vector3(0.8f, 2f, 0.8f);
        }

        if (cube2 != null)
        {
            Undo.RecordObject(cube2.transform, "Create Wide Barrier");
            cube2.transform.localPosition = new Vector3(4f, 0.75f, -3f);
            cube2.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            cube2.transform.localScale = new Vector3(3f, 1.5f, 0.5f);
        }

        if (cube3 != null)
        {
            Undo.RecordObject(cube3.transform, "Create Central Block");
            cube3.transform.localPosition = new Vector3(0f, 0.75f, 0.5f);
            cube3.transform.localRotation = Quaternion.Euler(0f, 30f, 0f);
            cube3.transform.localScale = new Vector3(2f, 1.5f, 1.5f);
        }

        Debug.Log("Obstacles reshaped: Tall pillar, wide barrier, and central block!");
    }

    static void SpreadCollectibles()
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-8f, 0.5f, 8f),
            new Vector3(-6f, 0.5f, -8f),
            new Vector3(8f, 0.5f, -6f),
            new Vector3(8f, 0.5f, 6f),
            new Vector3(-8f, 0.5f, -2f),
            new Vector3(6f, 0.5f, 8f),
            new Vector3(-4f, 0.5f, 4f),
            new Vector3(4f, 0.5f, -4f),
            new Vector3(-6f, 0.5f, 6f),
            new Vector3(6f, 0.5f, -6f),
            new Vector3(0f, 0.5f, 8f),
            new Vector3(0f, 0.5f, -8f)
        };

        for (int i = 0; i < 12; i++)
        {
            GameObject pickup = null;
            if (i == 0)
                pickup = GameObject.Find("PickUp Parent/PickUp");
            else
                pickup = GameObject.Find($"PickUp Parent/PickUp ({i})");

            if (pickup != null && i < positions.Length)
            {
                Undo.RecordObject(pickup.transform, $"Spread PickUp {i}");
                pickup.transform.position = positions[i];
            }
        }

        Debug.Log("Coins spread across the arena!");
    }

    static void SpreadDynamicBoxes()
    {
        Vector3[] boxPositions = new Vector3[]
        {
            new Vector3(-5f, 1f, 5f),
            new Vector3(5f, 1f, 5f),
            new Vector3(-5f, 1f, -5f),
            new Vector3(5f, 1f, -5f),
            new Vector3(0f, 1f, -6f)
        };

        for (int i = 0; i < 5; i++)
        {
            GameObject box = null;
            if (i == 0)
                box = GameObject.Find("Dynamicbox");
            else
                box = GameObject.Find($"Dynamicbox ({i})");

            if (box != null && i < boxPositions.Length)
            {
                Undo.RecordObject(box.transform, $"Spread Dynamicbox {i}");
                box.transform.position = boxPositions[i];
            }
        }

        Debug.Log("Pink cubes spread across the arena!");
    }

    static void FixPetFollower()
    {
        GameObject pet = GameObject.Find("Pet");
        if (pet != null)
        {
            Rigidbody rb = pet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Undo.RecordObject(rb, "Fix Pet Rigidbody");
                rb.linearDamping = 5f;
                rb.angularDamping = 5f;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.useGravity = true;
                rb.mass = 1f;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }

            CapsuleCollider collider = pet.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = Undo.AddComponent<CapsuleCollider>(pet);
                collider.height = 1.5f;
                collider.radius = 0.4f;
                collider.center = new Vector3(0f, 0.75f, 0f);
            }

            PetFollower follower = pet.GetComponent<PetFollower>();
            if (follower != null)
            {
                SerializedObject so = new SerializedObject(follower);
                so.FindProperty("followSpeed").floatValue = 8f;
                so.FindProperty("rotationSpeed").floatValue = 15f;
                so.FindProperty("drag").floatValue = 5f;
                so.FindProperty("acceleration").floatValue = 10f;
                so.FindProperty("stoppingDistance").floatValue = 0.5f;
                so.ApplyModifiedProperties();
            }

            GameObject visual = pet.transform.Find("Visual")?.gameObject;
            if (visual != null)
            {
                CharacterController charController = visual.GetComponent<CharacterController>();
                if (charController != null)
                {
                    Undo.DestroyObjectImmediate(charController);
                    Debug.Log("Removed CharacterController from Pet/Visual - it was conflicting with Rigidbody!");
                }

                Component creatureMover = visual.GetComponent("CreatureMover");
                if (creatureMover != null)
                {
                    Undo.DestroyObjectImmediate(creatureMover);
                    Debug.Log("Removed CreatureMover from Pet/Visual!");
                }

                Component movePlayerInput = visual.GetComponent("MovePlayerInput");
                if (movePlayerInput != null)
                {
                    Undo.DestroyObjectImmediate(movePlayerInput);
                    Debug.Log("Removed MovePlayerInput from Pet/Visual!");
                }
            }
        }

        Debug.Log("Pet follower fixed: Added collider, removed conflicting components!");
    }

    static void KeepOnlyOnePet()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        GameObject firstPet = null;
        int removed = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Pet" && obj.GetComponent<PetFollower>() != null)
            {
                if (firstPet == null)
                {
                    firstPet = obj;
                    Undo.RecordObject(firstPet.transform, "Position Pet");
                    firstPet.transform.position = new Vector3(3f, 0.5f, 3f);
                    
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        PetFollower follower = firstPet.GetComponent<PetFollower>();
                        if (follower != null)
                        {
                            Undo.RecordObject(follower, "Assign Player to Pet");
                            follower.player = player.transform;
                        }
                    }

                    Rigidbody rb = firstPet.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Undo.RecordObject(rb, "Set Pet Rigidbody Constraints");
                        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    }
                }
                else
                {
                    Undo.DestroyObjectImmediate(obj);
                    removed++;
                }
            }
        }

        Debug.Log(removed > 0 ? $"Kept 1 pet, removed {removed} duplicates!" : "Only 1 pet found - all good!");
    }
}
