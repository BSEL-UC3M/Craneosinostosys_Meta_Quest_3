# AR guidance for craniosynostosis surgery with Meta Quest 3

This repository has been created by Lucía Cabello Martín, from Universidad Carlos III de Madrid.

## 📝 Project Overview

This repository contains everything you need to run an Augmented Reality (AR) application for **Meta Quest 3** that assists surgeons during osteotomies in craniosynostosis surgery. The app overlays patient-specific 3D models (skull, surgical guides and cutting lines) directly onto the patient. The virtual models are registered to the real world using two **ArUco markers** placed on the surgical guides, without the need for external navigation systems.

The repository also includes a **3D Slicer module** used to validate the accuracy of the system with an optical tracker. This module is used only for experimental validation and is not required by the AR app. The system was validated on a 3D-printed phantom, showing mean tracing errors between 1.4 and 2.8 mm in four of the five cutting lines evaluated, using low-cost, standalone consumer hardware.

## 💻 Requirements and Setup

The project was developed and tested using the following environment:

| Component | Version / Platform | Notes |
|---|---|---|
| AR Platform | Meta Quest 3 | Standalone video see-through headset |
| Game Engine | Unity 6000.3.2f1 | Android build support required |
| AR Framework | Meta XR SDK [VERSION] | Passthrough Camera API and Interaction SDK |
| Computer Vision | OpenCV for Unity [VERSION] | Paid Unity Asset Store package, not included in this repository |
| Language | C# / Python | AR application / 3D Slicer module |
| 3D Slicer | [VERSION] or above | With SlicerIGT and SlicerOpenIGTLink extensions (validation only) |

## 📄 Scientific Publication and Citation

This repository complements the following work:

**Title:** Desarrollo y validación de un sistema de realidad aumentada para la asistencia quirúrgica en craneosinostosis mediante Meta Quest 3 (Master's Thesis).
**Author:** Lucía Cabello Martín.
**Supervisors:** Alicia Pose-Díez-de-la-Lastra, Simon Drouin.
**Institution:** Universidad Carlos III de Madrid, 2026.

Please cite this work if you use this code or any part of it in your projects or publications.

## 🛠️ Repository Structure

The repository is organized as follows:

- **3D models:** Patient-specific models (skull, cutting lines and surgical guides) and the ArUco markers (IDs 0 and 2, 40 × 40 mm) used for registration.
- **AR App - Meta Quest 3:** The Unity project of the AR application to be installed in the Meta Quest 3 headset.
- **3D Slicer validation module:** The 3D Slicer module used to quantify the accuracy of the AR overlay with an optical tracking system (OptiTrack + PLUS toolkit).

## 🚀 Installation and Usage

1. Open the AR app project with Unity 6000.3.2f1 and import OpenCV for Unity from the Unity Asset Store.
2. Switch the build platform to Android, connect the Meta Quest 3 (developer mode) and select **Build and Run**. Grant camera access when the app starts for the first time.
3. Print the ArUco markers at exactly 40 × 40 mm and attach them to the surgical guides.
4. In the app, select the patient and look at the markers. The models are anchored once the marker border turns green.

## 📧 Contact

For any questions or inquiries regarding the project, please contact the author or the leader of the IGT research group, from Departamento de Bioingeniería, Universidad Carlos III de Madrid (UC3M):

Lucía Cabello Martín (Author): [EMAIL]

Javier Pascau (Senior Author): jpascau@ing.uc3m.es

For more information about the research group, feel free to have a look at our website: https://igt.uc3m.es/.

## 🙏 Acknowledgements

[FUNDING PENDING]

The ArUco detection pipeline is based on [TakashiYoshinaga/QuestArUcoMarkerTracking](https://github.com/TakashiYoshinaga/QuestArUcoMarkerTracking).
