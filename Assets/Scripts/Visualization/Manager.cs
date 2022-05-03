/*

Courtesy of Sebastian Lague:
    - Youtube       :   https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
    - Github Repo   :   https://github.com/SebLague/Cloth-and-IK-Test/tree/main/Assets/Visualize

*/

using System.Collections.Generic;
using UnityEngine;

namespace Visualization {
    public static class Manager {
        public static Mesh sphereMesh;
        public static Mesh cylinderMesh;

        private static Material material;
        private static MaterialPropertyBlock materialProperties;

        private static int lastInputFrame = -1;
        private static List<VisualElement> drawList;

        static Manager() {
            Camera.onPreCull -= Draw;
            Camera.onPreCull += Draw;

            Init();
        }

        private static void Init() {
            if (materialProperties == null)
                materialProperties = new MaterialPropertyBlock();

            if (drawList == null)
                drawList = new List<VisualElement>();

            if (material == null)
                material = new Material(Shader.Find("Particles/Standard Unlit"));

            if (sphereMesh == null) {
                sphereMesh = new Mesh();
                MeshGenerators.SphereMesh.GenerateMesh(sphereMesh);
            }

            if (cylinderMesh == null) {
                cylinderMesh = new Mesh();
                MeshGenerators.CylinderMesh.GenerateMesh(cylinderMesh);
            }

            AddedVisualElement();
            drawList.Clear();
        }

        private static void AddedVisualElement() {
            lastInputFrame = Time.frameCount;
        }

        public static int AddVisualElement (VisualElement element) {
            AddedVisualElement();
            
            for (int i = 0; i < drawList.Count; i++) {
                if (drawList[i] == null) {
                    drawList[i] = element;
                    return i;
                }
            }

            drawList.Add(element);
            return drawList.Count - 1;
        }

        public static int CreateVisualElement(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color colour) {
            return AddVisualElement(new VisualElement (mesh, position, rotation, scale, colour));
        }

        // TODO (calco): this seems bad?
        public static void RemoveVisualElement(int idx) {
            drawList[idx] = null;
        }

        public static void ClearCamera() {
            drawList.Clear();
        }

        private static void Draw(Camera camera) {
            if (!camera || lastInputFrame == Time.frameCount - 1)
                return;

            // Something changed, and we have a camera to render to
            for (int i = 0; i < drawList.Count; i++) {
                if (drawList[i] == null)
                    continue;
                
                VisualElement elem = drawList[i];
                Matrix4x4 matrix = Matrix4x4.TRS(elem.position, elem.rotation, elem.scale);
                
                materialProperties.SetColor("_Color", elem.colour);
                Graphics.DrawMesh(elem.mesh, matrix, material, 0, camera, 0, materialProperties);
            }

            ClearCamera();
        }
    }
}