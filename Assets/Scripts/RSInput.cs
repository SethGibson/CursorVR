using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RSInput : MonoBehaviour
{
    public OSVR.Unity.SetRoomRotationUsingHead ResetHead;
    public GameObject   CursorObject;
    public float        CursorScale;
    public float        CursorSize = 1.0f;
    public bool         MapZ,
                        UseTwoHands;
    public Vector4      ZRange; //x=z min in y=zmax in z=zmin out w=zmax out

    private bool    mCursor0Active,
                    mCursor1Active;

    private GameObject          mCursor0,
                                mCursor1;

    private PXCMSenseManager    mRS;
    private PXCMHandData        mHand;

    public Transform Cursor0Pos
    {
        get { return mCursor0.transform;  }
    }

    public Transform Cursor1Pos
    {
        get { return mCursor1.transform; }
    }

    void Start()
    {
        if (CursorObject != null) {
            mCursor0 = (GameObject)Instantiate(CursorObject, Vector3.zero, Quaternion.identity);
            mCursor0.transform.SetParent(Camera.main.transform);
            mCursor0.transform.localScale = new Vector3(CursorSize, CursorSize, CursorSize);
            mCursor0Active = false;
            mCursor1 = (GameObject)Instantiate(CursorObject, Vector3.zero, Quaternion.identity);
            mCursor1.transform.SetParent(Camera.main.transform);
            mCursor1.transform.localScale = new Vector3(CursorSize, CursorSize, CursorSize);
            mCursor1Active = false;
        }

        mRS = PXCMSenseManager.CreateInstance();
        var stat = pxcmStatus.PXCM_STATUS_NO_ERROR;

        if(mRS!=null) {
            stat = mRS.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH, 640, 480);
            if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR) {
                stat = mRS.EnableHand();
                if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR) {
                    stat = mRS.Init();
                    Debug.Log("Sense Manager Started");
                }
            }

            else {
                Debug.Log("Unable to start Sense Manager");
            }
            if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                PXCMHandConfiguration cfg = mRS.QueryHand().CreateActiveConfiguration();
                if (cfg != null) {
                    stat = cfg.SetTrackingMode(PXCMHandData.TrackingModeType.TRACKING_MODE_CURSOR);
                    if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR) {
                        Debug.Log("Set Cursor Mode 1st Pass");
                        if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR) {
                            stat = cfg.EnableAllGestures(false);
                            if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                            {
                                stat = cfg.ApplyChanges();
                                if (stat >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                                {
                                    stat = cfg.Update();
                                }
                            }
                        }
                        else {
                            Debug.Log("Unable to enable gestures");
                        }
                    }
                    else {
                        Debug.Log("Unable to set Cursor Mode");
                    }
                    cfg.Dispose();
                }
            }

            else {
                Debug.Log("Unable to create hand config");
            }

            mHand = mRS.QueryHand().CreateOutput();
            if (mHand != null) {
                Debug.Log("Created Hand Data");
                var tracking = mRS.QueryHand().CreateActiveConfiguration().QueryTrackingMode();
                if(tracking==PXCMHandData.TrackingModeType.TRACKING_MODE_CURSOR) {
                    Debug.Log("Cursor Mode Enabled");
                }
            }
        }
        else {
            Debug.Log("Unable to configure hand module");
        }
        Application.targetFrameRate = 60;

        ResetHead.SetRoomRotation();
	}
	
	void Update()
    {
        if (mRS.AcquireFrame(false) >= pxcmStatus.PXCM_STATUS_NO_ERROR) {
            mHand.Update();
            getCursorPositions();
            mRS.ReleaseFrame();
        }
	}
    private void getCursorPositions()
    {
        var numHands = mHand.QueryNumberOfHands();
        int handId = -1;

        if (numHands < 1)
        {
            mCursor0.transform.localPosition = Vector3.zero;
            mCursor1.transform.localPosition = Vector3.zero;
        }

        if (!UseTwoHands)
        {
            mCursor1.transform.localPosition = Vector3.zero;
        }
        for (var i = 0; i < numHands; ++i)
        {
            mHand.QueryHandId(PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_ID, i, out handId);
            PXCMHandData.IHand handData;
            if (mHand.QueryHandDataById(handId, out handData) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                if (handData.HasCursor())
                {
                    PXCMHandData.ICursor cursor;
                    if (handData.QueryCursor(out cursor) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        var pos = cursor.QueryPointWorld();
                        if (MapZ)
                        {
                            var newZ = lmap(pos.z, ZRange.x, ZRange.y, ZRange.z, ZRange.w);
                            pos.z = newZ;
                        }
                        Vector3 newPos = new Vector3(pos.x, pos.y, pos.z) * CursorScale;
                        if(handData.QueryBodySide()==PXCMHandData.BodySideType.BODY_SIDE_RIGHT)
                        {
                            mCursor0.transform.localPosition = newPos;
                            if(getGestureState(PXCMHandData.BodySideType.BODY_SIDE_RIGHT)) {
                                mCursor0Active = !mCursor0Active;
                                if (mCursor0Active) {
                                    mCursor0.transform.localScale = new Vector3(CursorSize * 3, CursorSize * 3, CursorSize * 3);
                                }
                                else {
                                    mCursor0.transform.localScale = new Vector3(CursorSize, CursorSize, CursorSize);
                                }
                            }
                        }
                        else if (handData.QueryBodySide() == PXCMHandData.BodySideType.BODY_SIDE_LEFT && UseTwoHands)
                        {
                            mCursor1.transform.localPosition = newPos;
                            if (getGestureState(PXCMHandData.BodySideType.BODY_SIDE_LEFT))
                            {
                                mCursor1Active = !mCursor1Active;
                                if (mCursor1Active)
                                {
                                    mCursor1.transform.localScale = new Vector3(CursorSize * 3, CursorSize * 3, CursorSize * 3);
                                }
                                else
                                {
                                    mCursor1.transform.localScale = new Vector3(CursorSize, CursorSize, CursorSize);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        mCursor0.transform.localPosition = Vector3.zero;
                    }
                    else if (i == 1)
                    {
                        mCursor1.transform.localPosition = Vector3.zero;
                    }
                }
            }
        }
    }

    private bool getGestureState(PXCMHandData.BodySideType pSide)
    {
        var numGestures = mHand.QueryFiredGesturesNumber();
        for(int j=0;j< numGestures;++j) {
            PXCMHandData.GestureData gestData;
            if(mHand.QueryFiredGestureData(j, out gestData)>=pxcmStatus.PXCM_STATUS_NO_ERROR) {
                PXCMHandData.IHand hand;
                if(mHand.QueryHandDataById(gestData.handId, out hand)>=pxcmStatus.PXCM_STATUS_NO_ERROR) {
                    var gestName = gestData.name;
                    if(gestName.Contains("cursor_click")) {
                        if(hand.QueryBodySide()==pSide) {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    void OnDisable()
    {
        mRS.Close();
        mRS.Dispose();
    }

    private float lmap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
