# Image Plate Reader
A program to monitor a folder and extract license plates from images in the selected folder.  Uses Openalpr to read licenses.

A windows WPF project that uses Openalpr .Net to read license plates from images.
https://github.com/openalpr/openalpr

There is an executable that should work out of the box you can extract.  Start the project, click on monitor folder and select folder containing images to read plates.  The program will read new images that gets saved into folder andsaves it into a log file.

It was made with video monitoring program such as Blue Iris in which you can specify BI to save captured images be save to a specific folder upon an activated event.

![alt text](https://raw.githubusercontent.com/wirble/ImagePlateReader/master/Executable/plates.png)
