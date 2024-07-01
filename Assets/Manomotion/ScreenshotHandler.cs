using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Android;

public class ScreenshotHandler : MonoBehaviour
{
    public Button captureButton;  // Reference to the Button
    public RawImage displayImage; // UI RawImage to display the screenshot
    private string screenshotPath;

    void Start()
    {
        if (captureButton != null)
        {
            // Add the CaptureScreenshot method to the button's onClick event
            captureButton.onClick.AddListener(CaptureScreenshot);
        }

        // Request storage permissions on Android
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
        }
    }

    public void CaptureScreenshot()
    {
        // Define the file path and name for the screenshot
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        screenshotPath = Path.Combine(Application.persistentDataPath, "Screenshot_" + timestamp + ".png");

        // Capture the screenshot and save it to the file path
        ScreenCapture.CaptureScreenshot(screenshotPath);

        Debug.Log("Screenshot saved to: " + screenshotPath);

        // Start a coroutine to load the screenshot and display it
        StartCoroutine(LoadAndDisplayScreenshot());

        // Save the screenshot to the gallery
        SaveToGallery(screenshotPath);
    }

    private IEnumerator LoadAndDisplayScreenshot()
    {
        // Wait for the end of the frame to ensure the screenshot is saved
        yield return new WaitForEndOfFrame();

        // Load the screenshot from the file path
        byte[] screenshotBytes = File.ReadAllBytes(screenshotPath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(screenshotBytes);

        // Assign the texture to the RawImage component
        displayImage.texture = texture;
        displayImage.enabled = true;
    }

    private void SaveToGallery(string path)
    {
#if UNITY_ANDROID
        string albumName = "MyAppScreenshots";
        string galleryPath = Path.Combine("/storage/emulated/0/Pictures", albumName);
        
        if (!Directory.Exists(galleryPath))
        {
            Directory.CreateDirectory(galleryPath);
        }

        string fileName = Path.GetFileName(path);
        string destinationPath = Path.Combine(galleryPath, fileName);

        File.Copy(path, destinationPath, true);

        // Refresh Android gallery
        AndroidJavaClass mediaScanClass = new AndroidJavaClass("android.media.MediaScannerConnection");
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        mediaScanClass.CallStatic("scanFile", activity, new string[] { destinationPath }, null, null);

        Debug.Log("Screenshot saved to gallery: " + destinationPath);
#elif UNITY_IOS
        // Save the screenshot to the iOS gallery
        string albumName = "MyAppScreenshots";
        // Plugin needed for saving the screenshot to iOS gallery
        IOSGallerySave(path, albumName);
        Debug.Log("Screenshot saved to iOS gallery");
#endif
    }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSGallerySave(string path, string album);
#endif
}