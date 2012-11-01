Pylon Grid
==========

Pylon Grid is a fast and accurate algorithm for human head detection in range
images. It processes images captured by a stereo camera that is positioned 
vertically, pointing from the roof to the ground. A static grid of measure 
points (pylons) is used to detect the heads. 

The Pylon Grid algorithm detects all local minima in the range image and 
has a linear time complexity in respect to the number of pylons. One important 
prerequisite for applying the Pylon Grid algorithm to human head detection 
is a one-to-one relationship between human heads in the scene and local minima 
in the range image.

The algorithm has excellent accuracy and robustness of detection. Its linear 
complexity and predictable operation make it suitable for real-time application.

For more information see http://dx.doi.org/10.1016/j.neucom.2011.12.040

