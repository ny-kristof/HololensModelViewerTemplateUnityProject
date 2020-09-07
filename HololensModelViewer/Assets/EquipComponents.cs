using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.Boundary;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Inspectors;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class EquipComponents : MonoBehaviour
{
    [Serializable]
    public struct Child
    {
        public GameObject childobject;
        public Vector3 childorigpos;
        public Quaternion childorigrot;
        public int assemblylevel;
        public bool isinplace;

        public int AssemblyLevel
        {
            get => assemblylevel;
            set => assemblylevel = value;
        }
        public bool IsInPlace
        {
            get => isinplace;
            set => isinplace = value;
        }
    }



    [HideInInspector] public List<Child> childrenList;
    List<GameObject> childrenList2 = new List<GameObject>();
    List<List<Child>> AssManList;
    int[] Childperlevel;
    int currlevel = 0;
    public int CurrentAssLevel
    {
        get => currlevel;
        set => currlevel = value;
    }
    int maxasslevel = 0;


    bool initcalled = false;
    public void Init()
    {
        if (!initcalled)
        {
            
            Rigidbody rb_par = gameObject.GetComponent<Rigidbody>();
            rb_par.detectCollisions = false;
            rb_par.useGravity = false;
            rb_par.isKinematic = true;
            ObjectManipulator om_par = gameObject.GetComponent<ObjectManipulator>();
            om_par.enabled = false;
            NearInteractionGrabbable nig_par = gameObject.GetComponent<NearInteractionGrabbable>();
            nig_par.enabled = false;
            CreateClone();

            Transform tr_par = gameObject.GetComponent<Transform>();
            Quaternion origtr;
            origtr = tr_par.rotation;
            Camera camera = FindObjectOfType<Camera>();



            //get all transforms in the hierarchy, 
            //Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            //go through the transform array and only add their gameObjects to the list if they are not the parent (this) gameObject.
            for (int i = 0; i < childrenList.Count; i++)
            {
                Child child = childrenList[i];
                child.childorigpos = child.childobject.transform.position;
                child.childorigrot = child.childobject.transform.rotation;
                child.IsInPlace = false;
                Debug.Log(child.childobject.name + " assembly level is: " + child.AssemblyLevel);
                childrenList[i] = child;
                /*
                if (child != transform)
                {
                    
                    Child childtoadd = new Child();
                    childtoadd.childobject = child.gameObject;
                    childtoadd.childorigpos = child.position;
                    childtoadd.childorigrot = child.rotation;
                    childtoadd.AssemblyLevel = i;
                    childtoadd.IsInPlace = false;
                    Debug.Log(childtoadd.childobject.name + " assembly level is: " + childtoadd.AssemblyLevel);
                    childrenList.Add(childtoadd);
                    //origposs.Add(child.position);
                    

                }
                */
            }


            if (camera != null)
            {
                tr_par.eulerAngles = new Vector3(0, 0, 0);
                tr_par.eulerAngles = new Vector3(tr_par.eulerAngles.x, camera.GetComponent<Transform>().eulerAngles.y, tr_par.eulerAngles.z);
                //gameObject.GetComponent<Transform>().rotation = camera.transform.rotation;
            }
            else
            {
                Debug.Log("No camera found!!!");
            }
            List<float> distlist = CalculateMove2Pos();
            float distsum = 0;
            for (int i = 0; i < distlist.Count; i++)
            {
                distsum += distlist[i];
            }


            //go thorugh the list and add the component.
            for (int i = 0; i < childrenList.Count; i++)
            {
                MeshCollider mc = childrenList[i].childobject.AddComponent<MeshCollider>();
                mc.convex = true;
                mc.enabled = false;
                Rigidbody rb2 = childrenList[i].childobject.AddComponent<Rigidbody>();
                rb2.useGravity = true;
                rb2.isKinematic = true;
                EnablePhysics ep = childrenList[i].childobject.AddComponent<EnablePhysics>();
                //childrenList[i].childobject.AddComponent<BoundarySystemManager>();
                childrenList[i].childobject.AddComponent<NearInteractionGrabbable>();
                ObjectManipulator om = childrenList[i].childobject.AddComponent<ObjectManipulator>();
                //om.OnManipulationStarted.AddListener(ep.DropObject);
                MinMaxScaleConstraint mms = childrenList[i].childobject.AddComponent<MinMaxScaleConstraint>();
                mms.TargetTransform = childrenList[i].childobject.transform;
                mms.RelativeToInitialState = true;
                mms.ScaleMaximum = 1;
                mms.ScaleMinimum = 1;
                //childrenList[i].transform.localPosition = new Vector3((float)i, 2.0f, this.gameObject.transform.position.z);



                Move2Pos m2p = childrenList[i].childobject.AddComponent<Move2Pos>();
                //float xvalue = 2.0f * (float)i - (((childrenList.Count - 1.0f) * 2.0f) / 2);
                float xvalue = distlist[i] - (distlist[distlist.Count-1] / 2);
                m2p.Endpos = new Vector3(xvalue, 4.0f, 0);
                m2p.Startmove = true;
                //endpos = new Vector3((float)i, 1.0f, rb2.transform.position.z);
                //rb2.MovePosition(endpos);
            }
            //tr_par.rotation = origtr;
            GrabbableObjects();
            initcalled = true;
            Debug.Log("Current assembly level is: " + CurrentAssLevel);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //childrenList.Clear();
        childrenList2.Clear();
        GameObject handmenu = GameObject.FindGameObjectWithTag("HandMenu");
        if (handmenu != null)
        {
            //handbutton2.GetComponent<ButtonConfigHelper>().OnClick.AddListener(Init);
           ButtonConfigHelper[] Buttons = handmenu.GetComponentsInChildren<ButtonConfigHelper>(true);
            if (Buttons != null)
            {
                foreach (ButtonConfigHelper button in Buttons)
                {
                    if (button.gameObject.tag == "HandMenuButton2")
                    {
                        button.OnClick.AddListener(Init);
                    }
                }
            }
            else Debug.Log("No button found");
        }
        else Debug.Log("No hand menu found");
        for (int i = 0; i < childrenList.Count; i++)
        {
            if (childrenList[i].AssemblyLevel > maxasslevel)
            {
                maxasslevel = childrenList[i].AssemblyLevel;
            }
        }
        AssManList = new List<List<Child>>();
        for (int i = 0; i <= maxasslevel; i++)
        {
           List<Child> templist = new List<Child>();
            AssManList.Add(templist);
        }
        //AssManList.Clear();
        //Childperlevel = new List<int>(3); //childrenList.Count
        //Childperlevel.Clear();
        Childperlevel = new int[maxasslevel+1];
        for (int i = 0; i < childrenList.Count; i++)
        {
            int index = childrenList[i].AssemblyLevel;
            Childperlevel[index]++;
        }
        for (int i = 0; i < Childperlevel.Length; i++)
        {
            Debug.Log("There are " + Childperlevel[i] + " objects on level " + i);
        }

        gameObject.GetComponent<ObjectManipulator>().OnManipulationEnded.AddListener(gameObject.GetComponent<EnablePhysics>().DropObject);


        
    }


    // Update is called once per frame
    void Update()
    {
        if (initcalled)
        {
            //bool asslevelup = true;
            float proximity_threshold = 0.05f;
            for (int i = 0; i < childrenList.Count; i++)
            {
                float distance = (childrenList[i].childobject.transform.position - childrenList[i].childorigpos).magnitude;
                if (distance < proximity_threshold && childrenList[i].childobject.GetComponent<Move2Pos>().AssemblyCanStart && !childrenList[i].IsInPlace)
                {
                    childrenList[i].childobject.GetComponent<EnablePhysics>().InPlace();
                    childrenList[i].childobject.transform.position = childrenList[i].childorigpos;
                    childrenList[i].childobject.transform.rotation = childrenList[i].childorigrot;
                    Child tempchild = childrenList[i];
                    tempchild.IsInPlace = true;
                    childrenList[i] = tempchild;
                    //childrenList[i].IsInPlace = true;
                    childrenList2[i].SetActive(false);
                    childrenList[i].childobject.layer = 30;
                    /*
                    int index = childrenList[i].AssemblyLevel;
                    List<Child> templist = AssManList[index];
                    templist.Add(childrenList[i]);
                    AssManList[index] = templist;
                    */
                    AssManList[childrenList[i].AssemblyLevel].Add(childrenList[i]);
                    AssLevelManager();
                    GrabbableObjects();
                    Debug.Log("Current assembly level is: " + CurrentAssLevel);

                }
                
                if (distance >= proximity_threshold && childrenList[i].IsInPlace)
                {
                    Child tempchild = childrenList[i];
                    tempchild.IsInPlace = false;
                    childrenList[i] = tempchild;
                    childrenList[i].childobject.layer = 0;
                    childrenList2[i].SetActive(true);
                    AssManList[childrenList[i].AssemblyLevel].Remove(AssManList[childrenList[i].AssemblyLevel].Find(x => x.childobject.name == childrenList[i].childobject.name));
                    AssLevelManager();
                    GrabbableObjects();
                    Debug.Log("Current assembly level is: " + CurrentAssLevel);
                }
                
                /*
                if ((childrenList[i].AssemblyLevel == CurrentAssLevel) && !childrenList[i].IsInPlace)
                {
                    asslevelup = false;
                }
                */
            }
            
            /*
            if (asslevelup && (CurrentAssLevel < childrenList.Count))
            {
                CurrentAssLevel++;
                GrabbableObjects();
                Debug.Log("Current assembly level is: " + CurrentAssLevel);
            }
            */
        }

    }


    private void CreateClone()
    {
        GameObject hollowobj = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(hollowobj.GetComponent<EnablePhysics>());
        Destroy(hollowobj.GetComponent<ObjectManipulator>());
        Destroy(hollowobj.GetComponent<EquipComponents>());
        hollowobj.GetComponent<Rigidbody>().isKinematic = true;
        hollowobj.GetComponent<BoxCollider>().enabled = false;


        //get all transforms in the hierarchy
        Transform[] children2 = hollowobj.GetComponentsInChildren<Transform>(true);
        //go through the transform array and only add their gameObjects to the list if they are not the parent (this) gameObject.
        for (int i = 0; i < children2.Length; i++)
        {
            Transform child = children2[i];
            if (child != hollowobj.transform)
            {
                childrenList2.Add(child.gameObject);
            }
        }
        Shader transhader = Shader.Find("UI/Unlit/Transparent");
        //go thorugh the list and add the component.
        for (int i = 0; i < childrenList2.Count; i++)
        {
            MeshRenderer rend = childrenList2[i].GetComponent<MeshRenderer>();
            rend.material.shader = transhader;
            Color color = childrenList2[i].GetComponent<MeshRenderer>().material.color;
            color.a = 0.2f;
            childrenList2[i].GetComponent<MeshRenderer>().material.color = color;
        }

    }


    public void LevelSetter()
    {
        childrenList.Clear();
        //List<Child> children4assembly = new List<Child>();

        //get all transforms in the hierarchy, 
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        //go through the transform array and only add their gameObjects to the list if they are not the parent (this) gameObject.
        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            if (child != transform)
            {
                Child childtoadd = new Child();
                childtoadd.childobject = child.gameObject;
                childrenList.Add(childtoadd);
            }
        }
        //childrenList = children4assembly;
        Shader mrtk_shader = Shader.Find("Mixed Reality Toolkit/Standard");
        //go thorugh the list and add the component.
        for (int i = 0; i < childrenList.Count; i++)
        {
            MeshRenderer rend = childrenList[i].childobject.GetComponent<MeshRenderer>();
            Material tempMat = new Material(rend.sharedMaterial);
            tempMat.shader = mrtk_shader;
            tempMat.EnableKeyword("_PROXIMITY_LIGHT");
            //tempMat.SetFloat("_PROXIMITY_LIGHT", 1);
            rend.material = tempMat;
        }
    }


    public void GrabbableObjects()
    {
        for (int i = 0; i < childrenList.Count; i++)
        {
            ObjectManipulator om = childrenList[i].childobject.GetComponent<ObjectManipulator>();
            NearInteractionGrabbable ni = childrenList[i].childobject.GetComponent<NearInteractionGrabbable>();
            if (childrenList[i].AssemblyLevel == CurrentAssLevel)
            {
                om.enabled = true;
                ni.enabled = true;
            }
            else
            {
                ni.enabled = false;
                om.enabled = false;
            }
        }
        if (AssManList[CurrentAssLevel].Count == 0 && CurrentAssLevel != 0)
        {
            int last_level_notempty = CurrentAssLevel;
            while (last_level_notempty >= 0)
            {
                if (AssManList[last_level_notempty].Count != 0)
                {
                    for (int i = 0; i < AssManList[last_level_notempty].Count; i++)
                    {
                        AssManList[last_level_notempty][i].childobject.GetComponent<ObjectManipulator>().enabled = true;
                        AssManList[last_level_notempty][i].childobject.GetComponent<NearInteractionGrabbable>().enabled = true;
                    }
                    break;
                }
                last_level_notempty--;
            }
        }
    }

    private void AssLevelManager()
    {
        /*
        int inlevelcount = 0;
        for (int i = 0; i < childrenList.Count; i++)
        {
            if (childrenList[i].AssemblyLevel == CurrentAssLevel)
            {
                inlevelcount++;
            }
        }
        if (AssManList[CurrentAssLevel].Count == inlevelcount)
        {
            CurrentAssLevel++;
        }
        if (AssManList[CurrentAssLevel].Count == 0 && CurrentAssLevel > 0)
        {
            CurrentAssLevel--;
        }
        */
        for (int i = 0; i < AssManList.Count; i++)
        {
            if(AssManList[i].Count != Childperlevel[i])
            {
                CurrentAssLevel = i;
                break;
            }
        }
        
    }

    public List<float> CalculateMove2Pos()
    {
        List<float> distlist = new List<float>();
        distlist.Clear();
        distlist.Add(0f);
        for (int i = 1; i < childrenList.Count; i++)
        {
            float width1 = childrenList[i].childobject.GetComponent<MeshRenderer>().bounds.size.x*(1/gameObject.transform.localScale.x);
            float width2 = childrenList[i - 1].childobject.GetComponent<MeshRenderer>().bounds.size.x * (1 / gameObject.transform.localScale.x);
            float distance = (((width1 + width2) / 2))+ 1f;
            distance += distlist[i - 1];
            distlist.Add(distance);
        }
        return distlist;
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
