using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestBehavior : MonoBehaviour {
   
    Transform rootBones;
    List<Transform> bones;
    List<Vector3> bonesPos;

    public TextMesh boneRotations;
    public TextMesh boneRotationsDifference;
    public TextMesh bonePositions;

    [SerializeField] float rotateRate = 1f;
    [SerializeField] float rotateWidth = 1f;
    
    float timeCounter = 0f;

    Vector3 mousePos;

    [SerializeField] float hideRate = 1f;
    [SerializeField] float growthRate = 1f;
    [SerializeField] float hiddenTime = 2f;
    float hiddenTimeCalculate = 0f;

    [SerializeField] bool onClick = false;
    [SerializeField] bool isHidden = false;

    float minRotation = -359;
    float maxRotation = 359;

    // Use this for initialization
    void Start () {

        GetBones();
        
        InitBoneRotation();

        GetBonesPos();
    }

    // Update is called once per frame
    void Update () {
        //mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        //for (int i = 0; i < bones.Count; i++)
        //{
        //    bones[i].localRotation = Quaternion.LookRotation(Vector3.forward, mousePos - (bones[i].position));
        //}
        if (onClick)
        {
            isHidden = true;
            HideBody();
        }
        else if (isHidden)
        {
            if (hiddenTimeCalculate > hiddenTime)
            {
                ShowBody();
            }
            else
            {
                hiddenTimeCalculate += Time.deltaTime;
            }
        }
        else
        {
            timeCounter += Time.deltaTime;
            float y = Mathf.Sin(timeCounter * rotateRate) * rotateWidth;
            Vector3 m_axis = new Vector3(0, 0, y);
            m_axis.z = Mathf.Clamp(m_axis.z, minRotation, maxRotation);
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].localRotation = Quaternion.Euler(m_axis * (i + 1));
            }
        }
        

        DisplayBoneAngle();
        DisplayDifferencesBoneAngle();
        DisplayBonePos();
        
    }

    //get root bone and other bones connected to root bone
    private void GetBones()
    {
        //get root bone
        if (this.transform.name.Contains("root"))
        {
            rootBones = this.transform;
        }
        else
        {
            GetBonesPart(this.transform, "root");
        }

        //get bones connecting to root bone
        bones = new List<Transform>();
        GetBonesPart(rootBones, "bone");
    }

    private void GetBonesPart(Transform bonePartsTransform, string bonePart)
    {
        foreach (Transform bonePartTransform in bonePartsTransform)
        {
            if (bonePart.Equals("root"))
            {
                if (bonePartTransform.name.Contains(bonePart))
                {
                    rootBones = bonePartTransform;
                }
                else
                {
                    GetBonesPart(bonePartTransform, bonePart);
                }
            }
            else if (bonePart.Equals("bone"))
            {
                if (bonePartTransform.name.Contains(bonePart))
                {
                    bones.Add(bonePartTransform);
                    GetBonesPart(bonePartTransform, bonePart);
                }
            }
        }
    }

    //initialise all bone rotation
    private void InitBoneRotation()
    {
        for (int i = 0; i < bones.Count; i++)
        {
            bones[i].rotation = rootBones.rotation;
        }
        timeCounter = 0f;
    }

    private void GetBonesPos()
    {
        bonesPos = new List<Vector3>();
        for (int i = 0; i < bones.Count; i++)
        {
            bonesPos.Add(new Vector3(bones[i].localPosition.x, bones[i].localPosition.y));
        }
    }

    private void HideBody()
    {
        for (int i = bones.Count - 1; i >= 0; i--)
        {
            bool isPositiveX = bonesPos[i].x > 0;
            bool isPositiveY = bonesPos[i].y > 0;
            float x = bones[i].localPosition.x;
            float y = bones[i].localPosition.y;
            bool xDone = false;
            bool yDone = false;
            float hidingSpeed = Time.deltaTime * hideRate;
            //if the original position x is positive and the current position is still positive
            if (isPositiveX && x > 0)
            { x -= hidingSpeed; }
            //if the original position x is negative and the current position is still negative
            else if (!isPositiveX && x < 0)
            { x += hidingSpeed; }
            else
            {
                x = 0f;
                xDone = true;
            }

            //if the original position y is positive and the current position is still positive
            if (isPositiveY && y > 0)
            { y -= hidingSpeed; }
            //if the original position y is negative and the current position is still negative
            else if (!isPositiveY && y < 0)
            { y += hidingSpeed; }
            else
            {
                y = 0f;
                yDone = true;
            }

            bones[i].localPosition = new Vector3(x, y);

            if (xDone && yDone)
            {
                bones[i].localRotation = rootBones.rotation;
                continue;
            }
            else
                return;
        }
        onClick = false;
        InitBoneRotation();
    }

    private void ShowBody()
    {
        for (int i = 0; i < bones.Count; i++)
        {
            bool isPositiveX = bonesPos[i].x > 0;
            bool isPositiveY = bonesPos[i].y > 0;
            float x = bones[i].localPosition.x;
            float y = bones[i].localPosition.y;
            float growthSpeed = Time.deltaTime * growthRate;
            bool xDone = false;
            bool yDone = false;
            if (isPositiveX && x < bonesPos[i].x)
            {
                x += growthSpeed;
            }
            else if (!isPositiveX && x > bonesPos[i].x)
            {
                x -= growthSpeed;
            }
            else
            {
                x = bonesPos[i].x;
                xDone = true;
            }

            if (isPositiveY && y < bonesPos[i].y)
            {
                y += growthSpeed;
            }
            else if (!isPositiveY && y > bonesPos[i].y)
            {
                y -= growthSpeed;
            }
            else
            {
                y = bonesPos[i].y;
                yDone = true;
            }

            bones[i].localPosition = new Vector3(x, y);

            if (xDone && yDone)
                continue;
            else
                return;
        }
        isHidden = false;
        hiddenTimeCalculate = 0f;
    }

    private void OnMouseDown()
    {
        onClick = true;
    }

    private void DisplayBoneAngle()
    {
        boneRotations.text = "";
        for (int i = 0; i < bones.Count; i++)
        {
            boneRotations.text += "Bone" + (i + 1) + "   " 
                + " local: " + Mathf.Round(bones[i].localEulerAngles.z) 
                + " global: " + Mathf.Round(bones[i].eulerAngles.z) + "\n";
        }
    }

    private void DisplayDifferencesBoneAngle()
    {
        boneRotationsDifference.text = "";
        for (int i = 0; i < bones.Count-1; i++)
        {
            boneRotationsDifference.text += "Bone" + (i + 1) + "-Bone" + (i + 2) + "    " 
                + "Local: " + (Mathf.Round(bones[i].localEulerAngles.z) - Mathf.Round(bones[i + 1].localEulerAngles.z)) 
                + " Global: " + (Mathf.Round(bones[i].eulerAngles.z) - Mathf.Round(bones[i + 1].eulerAngles.z)) + "\n";
        }
    }

    private void DisplayBonePos()
    {
        bonePositions.text = "";
        for (int i = 0; i < bones.Count; i++)
        {
            bonePositions.text += "Bone" + (i + 1) + " x:" + bones[i].position.x + " y:" + bones[i].position.y + "\n";
        }
    }
}
