# Image Plate Reader
A program to monitor a folder and extract license plates from images in the selected folder.  Uses Openalpr to read licenses.

A windows WPF project that uses Openalpr .Net to read license plates from images.
https://github.com/openalpr/openalpr

There is an executable that should work out of the box in the exectuable folder.  You will need to extract the runtime_data folder in the same location as the program.  The program expects the folder to be runtime_data.  Start the project (VideoPlateReader.exe), click on monitor folder and select folder containing images to read plates.  The program will read new images that gets saved into folder and saves it into a log file.  The program will only look at new files based on created datetime newer than what is in LastFileProcessed.txt file.

It was made with video monitoring program such as Blue Iris in mind, where you can specify BI to save captured images be save to a specific folder upon an activated event.  You can set the monitor folder to this folder.

![alt text](https://raw.githubusercontent.com/wirble/ImagePlateReader/master/Executable/plates.png)

## Requires:

[OpenAlpr-net](https://github.com/openalpr/openalpr)

[Visual C++ Redistributable](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads)

Tested on Windows 10 64-bit version only
