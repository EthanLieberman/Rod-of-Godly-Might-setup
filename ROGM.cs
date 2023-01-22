using UnityEngine;
using UnityEditor;

// creates the menu as a class of EditorWindow
public class ROGM : EditorWindow
{
    // the avatar object to add to the field in the inspector
    GameObject avatar;

    // creates the menu and adds it to the toolbar, on open creates a new window with the following name
    [MenuItem("Tools/Rod of Godly Might")]
    public static void ShowWindow()
    {
        GetWindow<ROGM>("Rod of Godly might");
    }

    //for asset scaling slider
    public float size = 1f;

    // initiate on window open
    void OnGUI()
    {
        Texture2D bgImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Rod of Godly Might/Editor/ROGM logo.png", typeof(Texture2D));
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), bgImage, ScaleMode.ScaleToFit);

        // Display a field for selecting the parent object
         avatar = (GameObject)EditorGUILayout.ObjectField("Your Avatar", avatar, typeof(GameObject), true);

        // If a avatar is selected, display the buttons to click
        if (avatar != null)
        {
            // Find the armature objects on the avatar
            Animator animator = avatar.GetComponent<Animator>();
            GameObject hips = animator.GetBoneTransform(HumanBodyBones.Hips).gameObject;
            GameObject chest = animator.GetBoneTransform(HumanBodyBones.Chest).gameObject;
            GameObject leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand).gameObject;
            GameObject rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand).gameObject;
            
            // if button is pressed
            if (!GameObject.Find("ROGM world")) {
                if (GUILayout.Button("Attune ROGM"))
                {
                // setup and instantiate the prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Rod of Godly Might/ROGM world.prefab");
                GameObject newObject = Instantiate(prefab, avatar.transform.position, Quaternion.identity);
                newObject.name = prefab.name;

                // parent the new prefab to the avatar
                newObject.transform.parent = avatar.transform;

                // gets the prefab scene object
                Transform rogm = GameObject.Find("ROGM world").transform;

                // gets the prefab parts
                GameObject rogmHipTarget = rogm.Find("ROGM hip target").gameObject;
                GameObject rogmChestTarget = rogm.Find("ROGM chest target").gameObject;
                GameObject rogmRightHandTarget = rogm.Find("ROGM hand target R").gameObject;
                GameObject rogmLeftHandTarget = rogm.Find("ROGM hand target L").gameObject;

                // parents prefab parts to avatar
                rogmHipTarget.transform.parent = hips.transform;
                rogmChestTarget.transform.parent = chest.transform;
                rogmLeftHandTarget.transform.parent = leftHand.transform;
                rogmRightHandTarget.transform.parent = rightHand.transform;

                }
            }

            if (GameObject.Find("ROGM world")) {


                //gets prefab parts
                GameObject rogm = avatar.transform.Find("ROGM world").gameObject;

                GameObject rogmHipTarget = hips.transform.Find("ROGM hip target").gameObject;
                GameObject rogmChestTarget = chest.transform.Find("ROGM chest target").gameObject;
                GameObject rogmLeftHandTarget = leftHand.transform.Find("ROGM hand target L").gameObject;
                GameObject rogmRightHandTarget = rightHand.transform.Find("ROGM hand target R").gameObject;



                EditorGUILayout.LabelField("Select targets to adjust if needed. Finalze to mirror adjustments R -> L");
                EditorGUILayout.LabelField("**Ideal hand placement is just under halfway down the length of the handle");

                //functions to spawn in placement objects
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Rod of Godly Might/ROGM marker.prefab");

                void spawnMarker(GameObject location)
                {
                GameObject newObject = Instantiate(prefab, location.transform.position, Quaternion.identity);
                newObject.name = prefab.name;
                newObject.transform.parent = location.transform;
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
                newObject.transform.localScale = rogm.transform.localScale;

                }

                //function to remove all placement prefabs
                void removeMarker()
                {
                    GameObject toDelete = GameObject.Find("ROGM marker");
                    if (toDelete != null)
                    {
                        DestroyImmediate(toDelete);
                    }
                }

                //function to hide real object
                void toggleReal(bool value)
                {
                    GameObject rogmContainer = avatar.transform.Find("ROGM world").Find("ROGM container").gameObject;
                    rogmContainer.SetActive(value);
                }

                void mirror(Transform source, Transform target)
                {
                    Vector3 position = source.transform.position;
                    position.x = -position.x;
                    target.transform.position = position;

                    Quaternion rotation = source.transform.rotation;
                    rotation.y = -rotation.y;
                    rotation.z = -rotation.z;
                    target.transform.rotation = rotation;
                };


                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Hip target"))
                {
                    Selection.activeGameObject = rogmHipTarget.transform.Find("Holster hip R").gameObject;
                    removeMarker();
                    toggleReal(false);
                    spawnMarker(rogmHipTarget.transform.Find("Holster hip R").gameObject);
                    Tools.current = Tool.Move;
                }

                if (GUILayout.Button("Back target"))
                {
                    Selection.activeGameObject = rogmChestTarget.transform.Find("Holster shoulder R").gameObject;
                    removeMarker();
                    toggleReal(false);
                    spawnMarker(rogmChestTarget.transform.Find("Holster shoulder R").gameObject);
                    Tools.current = Tool.Move;
                }

                if (GUILayout.Button("Hand target"))
                {
                    Selection.activeGameObject = rogmRightHandTarget;
                    removeMarker();
                    toggleReal(false);
                    spawnMarker(rogmRightHandTarget);
                    Tools.current = Tool.Move;
                }


                GUILayout.EndHorizontal();

                size = EditorGUILayout.Slider("Scale", size, 0f, 3f);
                if (rogm.transform.localScale.x != size)
                {
                    rogm.transform.localScale = new Vector3(size, size, size);
                    GameObject marker = GameObject.Find("ROGM marker");
                    if (marker != null)
                    {
                        marker.transform.localScale = new Vector3(size, size, size);
                    }
                }



                if (GUILayout.Button("Finalize placements"))
                {
                    removeMarker();
                    toggleReal(true);
                    mirror(rogmHipTarget.transform.Find("Holster hip R"), rogmHipTarget.transform.Find("Holster hip L"));
                    mirror(rogmChestTarget.transform.Find("Holster shoulder R"), rogmChestTarget.transform.Find("Holster shoulder L"));
                    mirror(rogmRightHandTarget.transform, rogmLeftHandTarget.transform);
                    Selection.activeGameObject = null;
                    Tools.current = Tool.Move;
                }

                // if button is pressed
                if (GUILayout.Button("Unattune ROGM"))
                {
                //deletes them from the avatar
                DestroyImmediate(rogmHipTarget);
                DestroyImmediate(rogmChestTarget);
                DestroyImmediate(rogmLeftHandTarget);
                DestroyImmediate(rogmRightHandTarget);
                DestroyImmediate(rogm);
                }
            }
        }
        // If an avatar is not selected, display a prompt to select one
        else
        {
            EditorGUILayout.HelpBox("Select your Avatar in the Hierarchy", MessageType.Info);
        }

        // Display the label
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Setup script by YerGodDamnRight#0202");
    }
}