# LearningGripByTeaching (Unity Project)

**LearningGripByTeaching** is a Unity-based project that demonstrates the concept of enhancing learning by teaching others. The system was developed using Unity 2022.3.21f1, a stable long-term support (LTS) version, in combination with the Oculus Integration SDK and Unity XR Interaction Toolkit to enable cross-platform compatibility and robust hand-tracking features. This repository contains both the Unity project and analysis code to evaluate user interaction data, specifically hand movements during pre-test and post-test phases.

## Overview

This project uses Unity to create a learning environment where users’ hand data is automatically collected during the **pre-test** and **post-test** stages. The hand data is stored in the default Oculus folder. Additionally, the project can send real-time data to a computer via WebSocket for further analysis.

## Requirements
1. **Unity 2022.3.21f1 (LTS)**  
   - This version ensures compatibility with the Oculus Integration SDK and Unity XR Interaction Toolkit.
2. **Oculus Integration SDK** (if you plan to deploy to Oculus devices)
3. **XR Interaction Toolkit** (for VR/AR functionalities)
4. **Git** (to clone the repository)

## Setup & Usage

### Cloning the Repository
1. Open your command line or terminal.
2. Navigate to the directory where you want to store the project.
3. Run the following command:
   ```bash
   git clone https://github.com/Pannnnestrella/LearningGripByTeaching.git

## Hand Data Collection
During pre-test and post-test stages:

The project will automatically collect the user’s hand data (position, rotation, etc.).
Data is stored in the Oculus default folder on the device. For an Oculus Quest, for example, check the Android/data/<Your App Package>/files path or the standard Oculus logging path.


##  WebSocket Configuration
If you want to send data to a computer for real-time analysis:

Open HandDataController.cs in Assets/Scripts/.
Find the line defining the WebSocket address (e.g., ws://<IP>:<PORT>).
Replace YOUR_COMPUTER_IP with your local machine’s IP address (e.g., 192.168.1.100) and set the correct port if needed.
Ensure the receiving server or analysis tool is listening on that IP/port.

// In HandDataController.cs
private string websocketUrl = "ws://192.168.1.100:8080"; // Replace with your computer's IP and desired port

## Data Analysis
Retrieve the Data:

If data is collected locally on the Oculus device, connect the device to your computer to access the folder where logs/data files are saved.
If the project is sending data via WebSocket, locate the folder or database on your computer that stores incoming data.
Use the Analysis Scripts:

In the Analysis code and data folder, you will find Python or other scripts for data processing.
Adjust paths or filenames to match the location of your data.
Run these scripts to generate visualizations or statistical analyses.
