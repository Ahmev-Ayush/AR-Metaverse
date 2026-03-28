# 🚀 AR Metaverse
**An open-source, budget-friendly Spatial Computing workspace for Android.**

### 🎯 The Ultimate Aim
The ultimate goal of this project is to **democratize spatial computing and multi-monitor productivity**. 

High-end AR/VR headsets like the Meta Quest 3 and Apple Vision Pro offer incredible productivity features, allowing users to surround themselves with multiple massive virtual displays. However, these devices cost hundreds or thousands of dollars. 

**AR Metaverse** aims to bring that exact same futuristic, limitless workspace to the hardware people already own: a standard laptop and an Android smartphone. By combining AR tracking with zero-latency WebRTC streaming and virtual display drivers, this project transforms a single-screen laptop into an expansive, multi-monitor AR workstation that floats in your physical room—all at zero cost.

### ✨ Core Features
* **Virtual Multi-Monitor Setup:** Bypasses hardware limitations to create multiple virtual desktop screens from a single laptop.
* **Zero-Latency Streaming:** Utilizes WebRTC (via Unity Render Streaming) for real-time, peer-to-peer video transmission, ensuring that typing and mouse movements feel instantaneous.
* **Spatial Anchoring:** Built with AR Foundation to seamlessly track your physical environment and lock virtual screens to your real-world desk.
* **Highly Accessible:** Designed to run on standard Android phones, either handheld or slipped into a budget-friendly VR/AR phone enclosure (like Google Cardboard).

### 🛠️ Tech Stack
* **Game Engine:** Unity 3D
* **AR Framework:** AR Foundation / Google ARCore
* **Networking:** WebRTC / Unity Render Streaming / Web App
* **OS Integration:** Virtual Display Drivers (IddSampleDriver)

### 📂 Project Structure
* `Assets/`: Unity project assets including AR templates, scripts, scenes, and prefabs.
* `web app/`: The web application serving as the WebRTC signaling/streaming endpoint.
* `DEVLOG.md`: Tracking ongoing development progress.

### 🚀 Getting Started
1. Open the Unity project (`AR Metaverse`) in Unity Hub with Android Build Support.
2. Run the signaling server located in the `web app/` directory.
3. Build the Android `.apk` and deploy it to your ARCore-compatible smartphone to start your spatial workspace!

