# QualisysUDP

[![stable](http://badges.github.io/stability-badges/dist/stable.svg)](http://github.com/badges/stability-badges)

Console app for streaming from Qualisys Track Manager (QTM) over UDP.

[Qualisys](https://github.com/qualisys/) provides SDKs for talking to QTM from various languages and platforms ([C#](https://github.com/qualisys/RTClientSDK.Net), [Python](https://github.com/qualisys/qualisys_python_sdk), [Unity](https://github.com/qualisys/Qualisys-Unity-SDK), [Node](https://github.com/qualisys/qualisys-rt)...). While in most cases you can use the SDK for your platform to receive data from QTM, some devices/platforms are not compatible with the SDKs and will require that you stream from a host computer.

I made this primarily to talk to a HoloLens, for precise registration of holograms with respect to the motion capture system. Check out [HoloLensUDP](https://github.com/mbaytas/HoloLensUDP) for HoloLens Unity assets.

## Dependencies

Requires [qualisys/RTClientSDK.Net](https://github.com/qualisys/RTClientSDK.Net).

---

This is derived from my work with [Qualisys AB](http://www.qualisys.com/), the [HAPPERN research group at Koç University](https://happern.ku.edu.tr/), and the [t2i Interaction Laboratory at Chalmers University of Technology](http://t2i.se/).
