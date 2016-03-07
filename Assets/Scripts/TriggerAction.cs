using UnityEngine;
using System.Collections;


public class TriggerAction : MonoBehaviour
{
    static int      kCooldown = 30;
    public int      AnimTime = 5;

    public Color    ColorOn,
                    ColorOff;

    public Vector3  ScaleOn;

    private bool    mIsActive = false;
    private bool    mIsAnimating = false;
    private int     mAnimCounter = 0;
    private int     mCooldown = kCooldown;
    private Material    mMaterial;

    void Start()
    {
        mMaterial = gameObject.GetComponent<MeshRenderer>().material;
        mMaterial.SetColor("_EmissionColor", ColorOff);
    }

    void Update()
    {
        if (mIsActive) {
            if (mCooldown >= 0) {
                mCooldown -= 1;
            }
            else {
                mIsActive = false;
                StartShrinking();
            }
        }
    }

    void OnTriggerEnter(Collider pCollider)
    {
        if (pCollider.gameObject.tag == "Hand") {
            StartGrowing();
        }
    }

    void OnTriggerStay(Collider pCollider)
    {
        if (pCollider.gameObject.tag == "Hand") {
            mCooldown = kCooldown;
            mIsActive = true;
        }
    }

    void OnTriggerExit(Collider pCollider)
    {
        if (pCollider.gameObject.tag == "Hand") {
            StartShrinking();
        }
    }

    IEnumerator CubeGrow()
    {
        while (mIsAnimating) {
            if (mAnimCounter <= AnimTime) {
                var ratio = mAnimCounter / (float)AnimTime;

                var newScale = Vector3.Lerp(Vector3.one, ScaleOn, ratio);
                gameObject.transform.localScale = newScale;

                var newColor = Color.Lerp(ColorOff, ColorOn, ratio);
                mMaterial.SetColor("_EmissionColor", newColor);
                mAnimCounter += 1;
            }
            else {
                mIsActive = true;
                mIsAnimating = false;
            }
            yield return null;
        }
    }

    IEnumerator CubeShrink()
    {
        while (mIsAnimating) {
            if (mAnimCounter <= AnimTime) {
                var ratio = 1.0f-(mAnimCounter / (float)AnimTime);

                var newScale = Vector3.Lerp(Vector3.one, ScaleOn, ratio);
                gameObject.transform.localScale = newScale;

                var newColor = Color.Lerp(ColorOff, ColorOn, ratio);
                mMaterial.SetColor("_EmissionColor", newColor);

                mAnimCounter += 1;
            }
            else {
                mIsAnimating = false;
                mIsActive = false;
            }
            yield return null;
        }
    }

    void StartGrowing()
    {
        if (!mIsAnimating)
        {
            mIsAnimating = true;
            mAnimCounter = 0;
            StartCoroutine("CubeGrow");
            mCooldown = kCooldown;
        }
    }

    void StartShrinking()
    {
        if (!mIsAnimating)
        {
            mIsAnimating = true;
            mAnimCounter = 0;
            StartCoroutine("CubeShrink");
        }
    }
}