using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class SystemMonitorEditor {
        public float xBorder = 48;
        public float yBorder = 20;
        public int rightLinePadding = 10;
        public string labelFormat = "{0:0.0}";
        public string axisFormat = "{0:0.0}";
        public int gridLines = 1;
        public float axisRounding = 10f;
        public float anchorRadius = 1f;
        public Color lineColor = Color.magenta;

        readonly GUIStyle _labelTextStyle;
        readonly GUIStyle _centeredStyle;

        public SystemMonitorEditor() {
            _labelTextStyle = new GUIStyle(GUI.skin.label);
            _labelTextStyle.alignment = TextAnchor.UpperRight;
            _centeredStyle = new GUIStyle();
            _centeredStyle.alignment = TextAnchor.UpperCenter;
            _centeredStyle.normal.textColor = Color.white;
        }

        public void Draw(float[] data, float height) {
            var rect = GUILayoutUtility.GetRect(Screen.width, height);
            var top = rect.y + yBorder;
            var floor = rect.y + rect.height - yBorder;
            var availableHeight = floor - top;
            var max = data.Length != 0 ? data.Max() : 0f;
            if (max % axisRounding != 0) {
                max = max + axisRounding - (max % axisRounding);
            }

            drawGridLines(top, availableHeight, max);
            drawLine(data, floor, availableHeight, max);
        }

        void drawGridLines(float top, float availableHeight, float max) {
            var handleColor = Handles.color;
            Handles.color = Color.grey;
            var n = gridLines + 1;
            var lineSpacing = availableHeight / n;
            for (int i = 0; i <= n; i++) {
                var lineY = top + (lineSpacing * i);
                Handles.DrawLine(
                    new Vector2(xBorder, lineY),
                    new Vector2(Screen.width - rightLinePadding, lineY)
                );
                GUI.Label(
                    new Rect(0, lineY - 8, xBorder - 2, 50),
                    string.Format(axisFormat, max * (1f - ((float)i / (float)n))),
                    _labelTextStyle
                );
            }
            Handles.color = handleColor;
        }

        void drawLine(float[] data, float floor, float availableHeight, float max) {
            var lineWidth = (float)(Screen.width - xBorder - rightLinePadding) / data.Length;
            var handleColor = Handles.color;
            Handles.color = lineColor;
            Vector2 prevLine = Vector2.zero;
            Vector2 newLine;
            for (int i = 0; i < data.Length; i++) {
                var value = data[i];
                var lineTop = floor - (availableHeight * (value / max));
                newLine = new Vector2(xBorder + (lineWidth * i), lineTop);
                if (i > 0) {
                    Handles.DrawAAPolyLine(prevLine, newLine);
                }
                prevLine = newLine;
                var anchorPosRadius3 = anchorRadius * 3;
                var anchorPosRadius6 = anchorRadius * 6;
                var anchorPos = newLine - (Vector2.up * 0.5f);
                var labelRect = new Rect(anchorPos.x - anchorPosRadius3, anchorPos.y - anchorPosRadius3, anchorPosRadius6, anchorPosRadius6);
                if (labelRect.Contains(Event.current.mousePosition)) {
                    Handles.DrawSolidDisc(anchorPos, Vector3.forward, anchorRadius * 2);
                    labelRect.y -= 16;
                    labelRect.width += 50;
                    labelRect.x -= 25;
                    GUI.Label(labelRect, string.Format(labelFormat, value), _centeredStyle);
                } else {
                    Handles.DrawSolidDisc(anchorPos, Vector3.forward, anchorRadius);
                }
            }
            Handles.color = handleColor;
        }
    }
}