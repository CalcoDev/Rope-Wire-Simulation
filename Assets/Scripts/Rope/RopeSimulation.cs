using System.Collections.Generic;
using UnityEngine;

using Visualization;

public class RopeSimulation : MonoBehaviour {
    [System.Serializable]
    public class Point {
        public Vector2 position, prevPosition;
        public bool locked;
    }

    [System.Serializable]
    public class Stick {
        public Point pointA, pointB;
        public float length;
        public bool locked;

        public Stick(Point pointA, Point pointB) {
            this.pointA = pointA;
            this.pointB = pointB;
            length = Vector2.Distance(pointA.position, pointB.position);
        }
    }

    [Header("Visualising settings")]
    [SerializeField] private Camera cam;

    [SerializeField] private float pointRadius = 0.1f;
    [SerializeField] private float stickThickness = 0.05f;

    [SerializeField] private Color backgroundCol = new Color(60f, 65f, 110f);
    [SerializeField] private Color pointCol = new Color(255f, 255f, 247f);
    [SerializeField] private Color lockedPointCol = new Color(255f, 84f, 127f);
    [SerializeField] private Color stickCol = new Color(198f, 203f, 211f);

    [Header("Simulation settings")]
    [SerializeField] private float gravity = 10f;
    [SerializeField] private int numIterations = 10;
    public bool simulating;

    private List<Point> points;
    private List<Stick> sticks;

    private Point hoveredOverPoint;
    private Point selectedPoint;

    private Stick hoveredOverStick;

    private Vector2 mousePos;

    private void Start() {
        points = new List<Point>();
        sticks = new List<Stick>();
    }

    private void Update() {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        HandleInput();

        if (simulating) {
            Simulate();
        }
    }

    private void LateUpdate() {
        Draw();
    }

    private void HandleInput() {
        // Simulate when pressing space
        if (Input.GetKeyDown(KeyCode.Space)) {
            simulating = !simulating;
        }

        if (simulating) {
            // Make mouse cursor cut through lines
            if (Input.GetMouseButton(0)) {
                hoveredOverStick = MouseOverStick();

                if (hoveredOverStick != null) {
                    sticks.Remove(hoveredOverStick);
                }
            }

            return;
        }

        // Create new point using mouse click
        hoveredOverPoint = MouseOverPoint();
        if (Input.GetMouseButtonDown(0)) {
            if (hoveredOverPoint == null) {
                points.Add(new Point() {
                    position = mousePos,
                    prevPosition = mousePos
                });
            }
            // Connect two points by clicking on one and hovering to another
            else {
                selectedPoint = hoveredOverPoint;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (selectedPoint != null && hoveredOverPoint != null) {
                sticks.Add(new Stick(hoveredOverPoint, selectedPoint));
            }

            selectedPoint = null;
        }


        // Lock point by right clicking
        if (Input.GetMouseButtonDown(1)) {
            if (hoveredOverPoint != null) {
                hoveredOverPoint.locked = !hoveredOverPoint.locked;
            }
        }

        // Remove point by middle clicking
        if (Input.GetMouseButton(2)) {
            if (hoveredOverPoint != null) {
                // Remove sticks containing that point
                // TODO(calco): make this ... better?
                for (int i = 0; i < sticks.Count; i++) {
                    if (sticks[i].pointA == hoveredOverPoint || sticks[i].pointB == hoveredOverPoint) {
                        sticks.RemoveAt(i);
                        i--;
                    }
                }
                
                points.Remove(hoveredOverPoint);
            }
            
            if (hoveredOverStick != null) {
                sticks.Remove(hoveredOverStick);
            }
        }
    }

    private Point MouseOverPoint() {
        foreach (Point point in points) {
            if (Vector2.Distance(mousePos, point.position) < pointRadius)
                return point;
        }

        return null;
    }

    private Stick MouseOverStick() {
        foreach (Stick stick in sticks) {
            if (PointLineCollision(stick.pointA.position, stick.pointB.position, stickThickness, mousePos))
                return stick;
        }

        return null;
    }

    private bool PointLineCollision(Vector3 lineStart, Vector3 lineEnd, float lineThickness, Vector3 point) {
        float d1 = Vector3.Distance(point, lineStart);
        float d2 = Vector3.Distance(point, lineEnd);
        float lineLen = Vector3.Distance(lineStart, lineEnd);

        return d1 + d2 >= lineLen - lineThickness * 0.5f && d1 + d2 <= lineLen + lineThickness * 0.5f;
    }

    private void Simulate() {
        foreach (Point point in points) {
            if (!point.locked) {
                Vector2 positionBeforeUpdate = point.position;
                point.position += point.position - point.prevPosition;
                point.position += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
                point.prevPosition = positionBeforeUpdate;
            }
        }

        for (int i = 0; i < numIterations; i++) {
            foreach (Stick stick in sticks)
            {
                Vector2 stickCentre = (stick.pointA.position + stick.pointB.position) * 0.5f;
                Vector2 stickDir = (stick.pointA.position - stick.pointB.position).normalized;

                if (!stick.pointA.locked)
                    stick.pointA.position = stickCentre + stickDir * stick.length * 0.5f;
                if (!stick.pointB.locked)
                    stick.pointB.position = stickCentre - stickDir * stick.length * 0.5f;
            }
        }
    }

    private void Draw() {
        cam.backgroundColor = backgroundCol;

        for (int i = 0; i < points.Count; i++) {
            Visualizer.DrawSphere(points[i].position, pointRadius, points[i].locked ? lockedPointCol : pointCol);
        }

        for (int i = 0; i < sticks.Count; i++) {
            Quaternion rotation = Quaternion.FromToRotation(Vector2.up, (sticks[i].pointA.position - sticks[i].pointB.position).normalized);
            Visualizer.DrawLine(sticks[i].pointA.position, sticks[i].pointB.position, rotation, stickThickness, stickCol);
        }

        if (selectedPoint != null) {
            Quaternion rotation = Quaternion.FromToRotation(Vector2.up, (mousePos - selectedPoint.position).normalized);
            Visualizer.DrawLine(selectedPoint.position, mousePos, rotation, stickThickness, Color.black);
        }
    }
}