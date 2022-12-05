# Touch Visualizer
 Visualize touch positions with moving average and harmonic filters
 
 ![Touch Visualizer](https://user-images.githubusercontent.com/918937/205679682-f3b4806a-9325-429d-8370-c455e761ad96.png)
 
 * Functionality
     * Basic touch visualizer
     * Touch filter visualization
         * Moving average filter
         * Harmonic filter
     * Benmark game
 
 * Demo video: https://youtu.be/K0C1Lnbmw4U
 
 ## Dependencies
 * Unity 2021.3.15f1 
     * with iOS Build Support
 * Xcode 14.1
 * iPhone 12 Pro
     * Should also work on iPhone models with more screen pixels. Not yet optimized for lower-resolution screens.
 
 ## Build
 * Prerequisites: You should have already configured your Apple Developer Team ID in Xcode, and enabled `Developer Mode` on your iPhone.
 * In Unity Hub, open the Unity project.
 * In Unity Editor menu, go to `Edit/Project Settings...`. In the Project Settings window, go to `Player` tab.
 * On the top of the tab, configure `Company Name` according to your Apple Developer configuration. 
 * Under `Other Settings` section, configure `Signing Team ID` according to your Apple Developer configuration, and make sure `Automatically Sign` is selected.
 * In Unity Editor menu, go to `File/Build Settings...`. Make sure `iOS` is the selected platform and `Scenes/AppScene` is selected in `Scenes In Build`.
 * Connect your iPhone with your Mac using a USB cable.
 * Click `Build And Run` button. Unity Editor will build the project into a Xcode project, and automatically open Xcode to build it and run it on your iPhone.
