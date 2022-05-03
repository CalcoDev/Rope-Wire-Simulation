/*

Courtesy of Sebastian Lague:
    - Youtube       :   https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
    - Github Repo   :   https://github.com/SebLague/Cloth-and-IK-Test/tree/main/Assets/Visualize

*/

using UnityEngine;

namespace Visualization {
    public class VisualElement {
        public Mesh mesh;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Color colour;

        public VisualElement(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color colour) {
            this.mesh = mesh;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.colour = colour;
        }
    }
}