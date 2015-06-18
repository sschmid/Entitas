using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class SystemMonitorEditor {
        const float xBorder = 50;
        const float yBorder = 20;
        const string formatString = "{0:0.0}";
        const string axisFormatString = "{0:0.0}";
        const int gridLines = 1;
        const float axisRounding = 10f;
        const float pipRadius = 1f;

        readonly Editor _editor;
        readonly GUIStyle _labelTextStyle;
        readonly GUIStyle _centeredStyle;

        Color _lineColor = Color.magenta;
        Color _fontColor = Color.white;

        float _windowHeight;
        float _barFloor;
        float _barTop;
        float _lineWidth;
        float _dataMax;

        public SystemMonitorEditor(Editor editor, float windowHeight) {
            _editor = editor;
            _windowHeight = windowHeight;
            _labelTextStyle = new GUIStyle();
            _labelTextStyle.alignment = TextAnchor.UpperRight;
            _labelTextStyle.normal.textColor = _fontColor;
            _centeredStyle = new GUIStyle();
            _centeredStyle.alignment = TextAnchor.UpperCenter;
            _centeredStyle.normal.textColor = _fontColor;
        }

        public void Draw(List<float> data) {
            var rect = GUILayoutUtility.GetRect(Screen.width, _windowHeight);
            _barTop = rect.y + yBorder;
            _lineWidth = (float)(Screen.width - (xBorder * 2)) / data.Count;
            _barFloor = rect.y + rect.height - yBorder;
            _dataMax = 0f;
            if (data.Count != 0) {
                _dataMax = data.Max();
            }
            if (_dataMax % axisRounding != 0) {
                _dataMax = _dataMax + axisRounding - (_dataMax % axisRounding);
            }

            drawGridLines(data);
        }

        void drawGridLines(List<float> data) {
            Handles.color = Color.grey;
            var lineSpacing = (_barFloor - _barTop) / (gridLines + 1);
            for (int i = 0; i <= gridLines + 1; i++) {
                Handles.DrawLine(new Vector2(xBorder, _barTop + (lineSpacing * i)),
                    new Vector2(Screen.width - xBorder, _barTop + (lineSpacing * i)));
                GUI.Label(new Rect(0, _barTop + (lineSpacing * i) - 8, xBorder - 2, 50),
                    string.Format(axisFormatString, (_dataMax * (1 - ((lineSpacing * i) / (_barFloor - _barTop))))),
                    _labelTextStyle);
            }
            drawLine(data);
        }

        void drawLine(List<float> lineData) {
            Vector2 previousLine = Vector2.zero;
            Vector2 newLine;
            Handles.color = _lineColor;

            for (int i = 0; i < lineData.Count; i++) {
                var lineTop = _barFloor - ((_barFloor - _barTop) * (lineData[i] / _dataMax));
                newLine = new Vector2(xBorder + (_lineWidth * i), lineTop);
                if (i > 0) {
                    Handles.DrawAAPolyLine(previousLine, newLine);
                }
                previousLine = newLine;
                var selectRect = new Rect((previousLine - (Vector2.up * 0.5f)).x - pipRadius * 3,
                                 (previousLine - (Vector2.up * 0.5f)).y - pipRadius * 3, pipRadius * 6, pipRadius * 6);
                if (selectRect.Contains(Event.current.mousePosition)) {
                    Handles.DrawSolidDisc(previousLine - (Vector2.up * 0.5f), Vector3.forward, pipRadius * 2);
                    selectRect.y -= 16;
                    selectRect.width += 50;
                    selectRect.x -= 25;
                    GUI.Label(selectRect, string.Format(formatString, lineData[i]), _centeredStyle);
                    if (_editor != null) {
                        _editor.Repaint();
                    }
                } else {
                    Handles.DrawSolidDisc(previousLine - (Vector2.up * 0.5f), Vector3.forward, pipRadius);
                }
            }
            Handles.color = Color.white;
        }
    }
}