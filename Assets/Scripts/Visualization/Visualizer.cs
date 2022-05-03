/*

Courtesy of Sebastian Lague:
    - Youtube       :   https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
    - Github Repo   :   https://github.com/SebLague/Cloth-and-IK-Test/tree/main/Assets/Visualize

*/

using UnityEngine;

namespace Visualization {
    public static class Visualizer {
        public static int DrawSphere(Vector3 position, float radius, Color colour) {
            return Manager.CreateVisualElement(Manager.sphereMesh, position, Quaternion.identity, Vector3.one * radius, colour);
        }

        public static int DrawLine(Vector3 start, Vector3 end, Quaternion rotation, float thickness, Color colour) {
            Vector3 centre = (start + end) / 2;
            Vector3 scale = new Vector3(thickness, (start - end).magnitude, thickness);
            return Manager.CreateVisualElement(Manager.cylinderMesh, centre, rotation, scale, colour);
        }

        public static void RemoveVisualElement(int idx) {
            Manager.RemoveVisualElement(idx);
        }

        public static void ClearCamera() {
            Manager.ClearCamera();
        }
    }
}