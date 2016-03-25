using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class SystemsMonitor {
        public float xBorder = 48;
        public float yBorder = 20;
        public int rightLinePadding = -15;
        public string labelFormat = "{0:0.0}";
        public string axisFormat = "{0:0.0}";
        public int gridLines = 1;
        public float axisRounding = 10f;
        public float anchorRadius = 1f;
        public Color lineColor = Color.magenta;

        readonly GUIStyle _labelTextStyle;
        readonly GUIStyle _centeredStyle;
        readonly Vector3[] _cachedLinePointVerticies;
        readonly Vector3[] _linePoints;

        public SystemsMonitor(int dataLength) {
            _labelTextStyle = new GUIStyle(GUI.skin.label);
            _labelTextStyle.alignment = TextAnchor.UpperRight;
            _centeredStyle = new GUIStyle();
            _centeredStyle.alignment = TextAnchor.UpperCenter;
            _centeredStyle.normal.textColor = Color.white;
            _linePoints = new Vector3[dataLength];
            _cachedLinePointVerticies = new [] {
                new Vector3(-1, 1 ,0) * anchorRadius,
                new Vector3(1, 1, 0) * anchorRadius,
                new Vector3(1, -1, 0) * anchorRadius,
                new Vector3(-1, -1, 0) * anchorRadius,
            };
        }

        public void Draw(float[] data, float height) {
            var rect = GUILayoutUtility.GetRect(EditorGUILayout.GetControlRect().width, height);
            var top = rect.y + yBorder;
            var floor = rect.y + rect.height - yBorder;
            var availableHeight = floor - top;
            var max = data.Length != 0 ? data.Max() : 0f;
            if (max % axisRounding != 0) {
                max = max + axisRounding - (max % axisRounding);
            }

            drawGridLines(top, rect.width, availableHeight, max);
            drawLine(data, floor, rect.width, availableHeight, max);
        }

        void drawGridLines(float top, float width, float availableHeight, float max) {
            var handleColor = Handles.color;
            Handles.color = Color.grey;
            var n = gridLines + 1;
            var lineSpacing = availableHeight / n;
            for (int i = 0; i <= n; i++) {
                var lineY = top + (lineSpacing * i);
                Handles.DrawLine(
                    new Vector2(xBorder, lineY),
                    new Vector2(width - rightLinePadding, lineY)
                );
                GUI.Label(
                    new Rect(0, lineY - 8, xBorder - 2, 50),
                    string.Format(axisFormat, max * (1f - ((float)i / (float)n))),
                    _labelTextStyle
                );
            }
            Handles.color = handleColor;
        }

        void drawLine(float[] data, float floor, float width, float availableHeight, float max) {
            var lineWidth = (float)(width - xBorder - rightLinePadding) / data.Length;
            var handleColor = Handles.color;
            var labelRect = new Rect();
            Vector2 newLine;
            bool mousePositionDiscovered = false;
            float mouseHoverDataValue = 0;
            float linePointScale;
            Handles.color = lineColor;
            Handles.matrix = Matrix4x4.identity;
            HandleUtility.handleMaterial.SetPass(0);
            for (int i = 0; i < data.Length; i++) {
                var value = data[i];
                var lineTop = floor - (availableHeight * (value / max));
                newLine = new Vector2(xBorder + (lineWidth * i), lineTop);
                _linePoints[i] = new Vector3(newLine.x, newLine.y, 0);
                linePointScale = 1f;
                if (!mousePositionDiscovered) {
                    var anchorPosRadius3 = anchorRadius * 3;
                    var anchorPosRadius6 = anchorRadius * 6;
                    var anchorPos = newLine - (Vector2.up * 0.5f);
                    labelRect = new Rect(anchorPos.x - anchorPosRadius3, anchorPos.y - anchorPosRadius3, anchorPosRadius6, anchorPosRadius6);
                    if (labelRect.Contains(Event.current.mousePosition)) {
                        mousePositionDiscovered = true;
                        mouseHoverDataValue = value;
                        linePointScale = 3f;
                    }
                }
                Handles.matrix = Matrix4x4.TRS(_linePoints[i], Quaternion.identity, Vector3.one * linePointScale);
                Handles.DrawAAConvexPolygon(_cachedLinePointVerticies);
            }
            Handles.matrix = Matrix4x4.identity;
            Handles.DrawAAPolyLine(2f, data.Length, _linePoints);

            if (mousePositionDiscovered) {
                labelRect.y -= 16;
                labelRect.width += 50;
                labelRect.x -= 25;
                GUI.Label(labelRect, string.Format(labelFormat, mouseHoverDataValue), _centeredStyle);
            }
            Handles.color = handleColor;
        }
    }
}