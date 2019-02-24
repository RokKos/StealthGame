using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (ViewController))]
public class ViewControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        ViewController viewController = (ViewController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(viewController.transform.position, Vector3.up, Vector3.forward, 360, viewController.GetViewDistance());


        Handles.color = Color.blue;
        Vector3 leftLinePos = viewController.AngleToDir(viewController.transform.eulerAngles.y - viewController.GetViewAngle() / 2);
        Vector3 rightLinePos = viewController.AngleToDir(viewController.transform.eulerAngles.y + viewController.GetViewAngle() / 2);

        Handles.DrawLine(viewController.transform.position, viewController.transform.position + leftLinePos * viewController.GetViewDistance());
        Handles.DrawLine(viewController.transform.position, viewController.transform.position + rightLinePos * viewController.GetViewDistance());
    }
}
