# Facial Expression based on Landmark Detection

## Table of Contents

- [Description](#description)
- [Installation](#installation)
- [Usage](#usage)

## Description

After covid-19, the need to meet online increases. And by the evolution of VR, AR field, people have more and more needs for online interaction like real life. Therefore, owning an online identity (avatar) is necessary to increase interaction between people, which will help people easily immerse themselves in the virtual reality environment more naturally.

1. Platforms

    - Windows Application
    - MacOS Application (optional)

2. Applications

    - VTuber - people can create an virtual avatar and control it by their motion, movement (optional)
    - VR meeting/conference - people can create their avata to envolve an meeting/conference so that increase interaction between them

3. Tools and technology

    - Live2D Cubism - To create the 3d model
    - Mediapipe - The library for facial recognition (face landmark detection)
    - Unity - Develoment platform
    - C# - mono - development language and framework
    - Python

## Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/nnvu-fit/iuacg-group-3-project.git
    ```

2. Install Unity:

    - Download and install Unity from the official Unity website: [https://unity.com/](https://unity.com/)

3. Open the Unity project:

    - Launch Unity.
    - Click on "Open" and navigate to the root folder of the cloned repository.
    - Select the Unity project file (usually with a .unity extension) and click "Open".

4. Install Python:

    - Download and install Python from the official Python website: [https://www.python.org/](https://www.python.org/)

5. Set up the Python environment:

    - Open a terminal or command prompt.
    - Navigate to the "./PythonSocket" directory of the cloned repository.
    - Install the required Python packages by running the following command:

        ```bash
        pip install -r requirements.txt
        ```

6. Configure the project:

    - Provide any necessary configuration details, such as API keys or database connection strings.
    - Update any relevant configuration files.

## Usage

1. Start the Unity project:

    - In Unity, click on the "Play" button to start the project.

2. Start the Python server:

    - Open a terminal or command prompt.
    - Navigate to the "./PythonSocket" directory of the cloned repository.
    - Run the following command to start the Python server:

        ```bash
        python server.py
        ```

3. Follow the instructions on how to use the project.
