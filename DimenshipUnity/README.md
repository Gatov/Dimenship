# DimenshipUnity

Unity 2D strategy game interface for Dimenship.

## Opening the project

1. Install Unity Hub: https://unity.com/download
2. Install Unity Editor 6000.0 LTS via Unity Hub
3. In Unity Hub, click **Open** and navigate to this directory
4. Accept any package resolution prompts

## Architecture

Game logic lives in `DimenshipLogic` (.NET 10 class library).
This Unity project will reference compiled DLLs from DimenshipLogic
as a Plugins assembly once the integration layer is built.

Scripts are organized under `Assets/Scripts/` and compiled
under the `DimenshipUnity` assembly definition.
