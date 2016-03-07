using UnityEngine;
using System.Collections;

public class CursorLine : MonoBehaviour
{
    public int CursorId;
    public RSInput      HandInput;
    public Transform    Anchor;
    private LineRenderer mLineRenderer;

	void Start()
    {
        mLineRenderer = gameObject.GetComponent<LineRenderer>();
	}
	
	void Update()
    {
        mLineRenderer.SetPosition(0, Anchor.position);
        switch (CursorId) {
            case 0:
                mLineRenderer.SetPosition(1, HandInput.Cursor0Pos.position);
                break;
            case 1:
                mLineRenderer.SetPosition(1, HandInput.Cursor1Pos.position);
                break;
        }
	}
}
