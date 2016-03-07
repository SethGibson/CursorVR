# CursorVR - SR300 Cursor Mode + VR Test
## SR300 Setup
* Install the SR300 DCM and RealSense SDK from [Intel RealSense on IDZ](https://software.intel.com/en-us/intel-realsense-sdk/download)

## OSVR Setup
* Download the [OSVR Core binaries](http://access.osvr.com/binary/osvr-core), version 0.6.1115-g26bc354, 64-bit
 * Extract the archive to a local drive (but not the desktop), (e.g. C:\OSVR)
 * Navigate to the "bin" folder and run **osvr_server.exe** and (optional)create a shortcut on the desktop. 
 * Once running, do not close the console window or the connection to the OSVR HDK will be shutdown.
* (Optional) Download the [OSVR Tracker Viewer](http://access.osvr.com/binary/osvr-tracker-view), Build 212
 * Extract the archive to the local drive next to the Core Binaries (not the desktop)
 * Tracker View is useful for determining the HDKs connection.
 * Navigate to the extracted archive and run **OSVRTrackerView.exe** and (optional)create a shortcut on the desktop. 
 * The gnomon in the tracker view should reflect the orientation of the HDK. Tracker View does not have to be running to use the HDK, it's simply for debug and test. 
 
## Unity Setup
* Install Unity 5 from [Unity3D](http://unity3d.com/get-unity), 64-bit

## Running The Project
* Clone the repo or download the and extract the zip file to a local folder (e.g. C:/VR, C:/UnityProjects, etc, but not the desktop)
* To Run In The Editor:
 * Open Unity. If this is the first run, click the _OPEN_ button in the top right corner, and navigate to the project's root folder (e.g. C:/VR/CursorVR, C:/UnityProjects/CursorVR, etc). Otherwise, the project should appear in Unity's startup window.
 * In the editor, navigate to the _Assets_ folder and double-click SCN_VR, **OR** click File > Open Scene and double click SCN_VR
 * Make sure the HDK is connected, osvr_server is running, and the camera is connected, and hit the play button
 * Drag the _Game_ window over to the HDK and maximize the view
 
## Troubleshooting
* Occasionally, Unity will display the error message "Unable to create RenderManager". This can be ignored.
* If tracking isn't functioning, use Tracker View to determine that the HDK is being tracked, and double check the status messages in the osvr_server console window for errors regarding hardware detection
* Moving around too quickly sometimes jars the camera connection loose, keep head movements smooth and measured.
