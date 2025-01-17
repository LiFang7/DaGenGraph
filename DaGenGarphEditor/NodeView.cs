﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DaGenGraph.Editor
{
    public class NodeView
    {
        #region Private Variables

        private NodeBase m_Node;
        private GraphWindow m_graphWindow;
        private int m_WindowId;
        private GraphBase m_Graph;
        private GUIStyle m_CurrentStyle;
        private GUIStyle m_NormalStyle;
        private GUIStyle m_SelectedStyle;
        private GUIStyle m_CurrentHeaderStyle;
        private GUIStyle m_HeaderNormalStyle;
        private GUIStyle m_HeaderSelectedStyle;
        private GUIStyle m_IconStyle;
        private Vector2 m_DeleteButtonSize;
        private Vector2 m_Offset;
        private Rect m_DrawRect;
        public bool isVisible = true;
        public bool zoomedBeyondPortDrawThreshold = false;
        public bool isSelected;
        private Rect m_GlowRect;
        private Rect m_HeaderRect;
        private Rect m_HeaderHoverRect;
        private Rect m_HeaderIconRect;
        private Rect m_HeaderTitleRect;
        private Rect m_BodyRect;
        private Rect m_FooterRect;
        private Rect m_NodeOutlineRect;
        private Color m_NodeGlowColor;
        private Color m_NodeHeaderAndFooterBackgroundColor;
        private Color m_NodeBodyColor;
        private Color m_NodeOutlineColor;
        private Color m_HeaderTextAndIconColor;

        #endregion

        #region Properties

        public int windowId => m_WindowId;
        public NodeBase node => m_Node;
        public GraphBase graph => m_Graph;
        public float x => m_Node.GetX();
        public float y => m_Node.GetY();
        public float width => m_Node.GetWidth();
        public float height => m_Node.GetHeight();
        public Vector2 position => m_Node.GetPosition();
        public Vector2 size => m_Node.GetSize();
        public Rect rect => m_Node.GetRect();
        public Rect drawRect => m_DrawRect;
        protected float dynamicHeight { get; set; }

        #endregion

        #region GUIStyles

        private static GUIStyle s_NodeArea;
        private static GUIStyle nodeArea => s_NodeArea ??= Styles.GetStyle("NodeArea");
        private static GUIStyle s_NodeBody;
        protected static GUIStyle nodeBody => s_NodeBody ??= Styles.GetStyle("NodeBody");
        private static GUIStyle s_NodeOutline;
        protected static GUIStyle nodeOutline => s_NodeOutline ??= Styles.GetStyle("NodeOutline");
        private static GUIStyle s_NodeGlow;
        protected static GUIStyle nodeGlowStyle => s_NodeGlow ??= Styles.GetStyle("NodeGlow");
        private static GUIStyle s_Dot;
        private static GUIStyle nodeDot => s_Dot ??= Styles.GetStyle("NodeDot");
        private static GUIStyle s_NodeHeader;
        private static GUIStyle nodeHeader => s_NodeHeader ??= Styles.GetStyle("NodeHeader");
        private static GUIStyle s_NodeFooter;
        private static GUIStyle nodeFooter => s_NodeFooter ??= Styles.GetStyle("NodeFooter");
        private static GUIStyle s_NodeHorizontalDivider;

        private static GUIStyle nodeHorizontalDivider =>
            s_NodeHorizontalDivider ??= Styles.GetStyle("NodeHorizontalDivider");

        private static GUIStyle nodeInputText = new GUIStyle(){alignment = TextAnchor.MiddleLeft, fontSize = 16};
        #endregion

        #region Static Variables

        private static readonly float s_HeaderHeight = 32;
        private static readonly float s_HeaderIconSize = 20;
        private static float headerIconPadding => (s_HeaderHeight - s_HeaderIconSize) / 2;

        #endregion

        #region Virtual Methods

        public virtual void Init(int windowId, NodeBase node, GraphBase graph, GraphWindow graphWindow)
        {
            m_WindowId = windowId;
            m_Node = node;
            m_Graph = graph;
            m_graphWindow = graphWindow;
        }

        public virtual void OnDoubleClick(EditorWindow window)
        {
        }

        protected virtual void OnNodeGUI()
        {
            DrawNodeBody();
            DrawNodePorts();
        }

        protected virtual GUIStyle GetIconStyle()
        {
            return m_IconStyle ??= nodeDot;
        }

        protected virtual Rect DrawPort(Port port)
        {
            //set Port to the left side of the node
            port.SetX(0);
            //set the y to the current draw height
            port.SetY(dynamicHeight);
            //set the Port width as the node width 
            port.SetWidth(node.GetWidth());
            port.SetHeight(24f);
            if (zoomedBeyondPortDrawThreshold) return port.GetRect();

            //set the Port color to white or black - depending on the current skin - (we assume it's not connected)
            var portColor = Color.gray;
            var dividerColor = Color.gray;

            //check if the Port is connected in order to color the divider to the input or output color
            if (port.isConnected)
            {
                portColor = port.isInput ? UColor.GetColor().portInputColor : UColor.GetColor().portOutputColor;
                dividerColor = UColor.GetColor().nodeDividerColor;
            }

            //in order not to overpower the design with bold colors -> we make the color fade out a bit in order to make the graph easier on the eyes
            var opacity = 0.3f;
            portColor.a = opacity;
            dividerColor.a = opacity * 0.8f;

            //check if we are in delete mode -> if true -> set the socket color to red (ONLY if the socket can be deleted)
            if (m_graphWindow.altKeyPressed)
            {
                //since we're in delete mode and this socket can be deConnect -> set its color to red
                if (port.isConnected)
                {
                    portColor = Color.red;
                    dividerColor = Color.red;
                }

                //since we're in delete mode -> make the socket color a bit stronger (regardless if it can be deleted or not) -> this is a design choice
                portColor.a = opacity * 1.2f;
                dividerColor.a = opacity * 1.2f;
            }

            //calculate the top divider rect position and size -> this is the thin line at the bottom of every Port (design choice)
            var topDividerRect = new Rect(port.GetRect().x + 6, port.GetRect().y + 1f, port.GetRect().width - 12, 1);

            //color the gui to the defined Port color
            GUI.color = dividerColor;
            //DRAW the horizontal divider at the top of the Port
            GUI.Box(topDividerRect, GUIContent.none, nodeHorizontalDivider);
            //reset the gui color            
            GUI.color = Color.white;

            //calculate the bottom divider rect position and size -> this is the thin line at the bottom of every Port (design choice)
            var bottomDividerRect = new Rect(port.GetRect().x + 6, port.GetRect().y + port.GetRect().height - 1,
                port.GetRect().width - 12, 1);

            //color the gui to the defined Port color
            GUI.color = dividerColor;
            //DRAW the horizontal divider at the bottom of the Port
            GUI.Box(bottomDividerRect, GUIContent.none, nodeHorizontalDivider);
            //reset the gui color            
            GUI.color = Color.white;
            var label = port.isInput ? "Input" : port.portName;
            var areaRect = new Rect(port.GetX() + 24, port.GetY(), port.GetWidth() - 48, port.GetHeight());
            GUILayout.BeginArea(areaRect);
            {
                GUILayout.BeginHorizontal();
                {
                    var labelStyle = Styles.GetStyle("NodePortText");
                    var content = new GUIContent(label);
                    var contentSize = labelStyle.CalcSize(content);
                    GUILayout.BeginVertical(GUILayout.Width(contentSize.x), GUILayout.Height(port.GetHeight()));
                    {
                        GUILayout.Space((port.GetHeight() - contentSize.y) / 2);
                        GUILayout.Label(content, labelStyle, GUILayout.Width(contentSize.x));
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            //calculate the Port hover rect -> this is the 'selection' box that appears when the mouse is over the Port
            port.UpdateHoverRect();
            if (port.showHover.faded > 0.01f)
            {
                var animatedRect = new Rect(port.hoverRect.x,
                    port.hoverRect.y + port.hoverRect.height * (1 - port.showHover.faded),
                    port.hoverRect.width,
                    port.hoverRect.height * port.showHover.faded);

                if (port.isConnected)
                {
                    switch (port.GetDirection())
                    {
                        case PortDirection.Input:
                            GUI.color = UColor.GetColor().portInputColor.WithAlpha(0.1f);
                            break;
                        case PortDirection.Output:
                            GUI.color = UColor.GetColor().portInputColor.WithAlpha(0.1f);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }

                //color the gui to the defined Port color
                GUI.color = portColor;
                //DRAW the animated overlay when the mouse is hovering over this Port 
                GUI.Box(animatedRect, new GUIContent(label), nodeHorizontalDivider);
                GUI.color = Color.white; //reset the gui color 
            }

            return port.GetRect();
        }

        #endregion

        #region Protected Methods

        protected void DrawNodeBody()
        {
            var initialColor = GUI.color;
            dynamicHeight = 0;
            m_DrawRect = new Rect(0, 0, width, height); //get node draw rect

            var leftX = m_DrawRect.x + 6;
            var bodyWidth = m_DrawRect.width - 12;

            m_GlowRect = new Rect(m_DrawRect.x, m_DrawRect.y, m_DrawRect.width, m_DrawRect.height - 2);
            dynamicHeight += 6;
            m_HeaderRect = new Rect(leftX, dynamicHeight, bodyWidth, s_HeaderHeight);
            m_HeaderIconRect = new Rect(m_HeaderRect.x + headerIconPadding * 2f, m_HeaderRect.y + headerIconPadding + 1,
                s_HeaderIconSize, s_HeaderIconSize);
            m_HeaderTitleRect = new Rect(m_HeaderIconRect.xMax + headerIconPadding * 1.5f, m_HeaderIconRect.y,
                m_HeaderRect.width - (s_HeaderIconSize + headerIconPadding * 5), m_HeaderIconRect.height);
            dynamicHeight += m_HeaderRect.height;
            var footerHeight = 10f;
            var bodyHeight = m_DrawRect.height - 32f - 2f - footerHeight - 12;
            m_BodyRect = new Rect(leftX, dynamicHeight, bodyWidth, bodyHeight);
            m_FooterRect = new Rect(leftX, m_DrawRect.height - footerHeight - 8, bodyWidth, footerHeight);
            m_NodeOutlineRect = new Rect(m_DrawRect.x, m_DrawRect.y, m_DrawRect.width, m_DrawRect.height - 2);

            GUI.color = m_NodeGlowColor;
            GUI.Box(m_GlowRect, GUIContent.none, nodeGlowStyle); //node glow

            GUI.color = m_NodeHeaderAndFooterBackgroundColor;
            GUI.Box(m_HeaderRect, GUIContent.none, nodeHeader); //header background

            GUI.color = m_HeaderTextAndIconColor;
            GUI.Box(m_HeaderIconRect, GUIContent.none, GetIconStyle()); //header icon

            GUI.color = initialColor;
            GUIStyle titleStyle = Styles.GetStyle("NodeHeaderText");
            GUI.Label(m_HeaderTitleRect, node.name, titleStyle /*titleStyle8*/); //header title

            GUI.color = m_NodeBodyColor;
            GUI.Box(m_BodyRect, GUIContent.none, nodeBody); //body background

            GUI.color = m_NodeHeaderAndFooterBackgroundColor;
            GUI.Box(m_FooterRect, GUIContent.none, nodeFooter); //footer background

            GUI.color = m_NodeOutlineColor;
            GUI.Box(m_NodeOutlineRect, GUIContent.none, nodeOutline); //node outline

            GUI.color = initialColor; //reset colors
        }

        protected void DrawNodePorts()
        {
            DrawPortsList(node.inputPorts);
            
            DrawContent();
            DrawPortsList(node.outputPorts);
        }
        protected virtual void DrawContent()
        {
            
        }
        protected void DrawPortsList(List<Port> ports)
        {
            if (ports == null) return;
            foreach (Port port in ports)
            {
                //Update HEIGHT 
                dynamicHeight += DrawPort(port).height;
            }
        }

        protected void UpdateNodeWidth(float width)
        {
            m_Node.SetWidth(width);
        }

        protected void UpdateNodeHeight(float height)
        {
            m_Node.SetHeight(height);
        }

        protected void UpdateNodePosition(Vector2 position)
        {
            m_Node.SetPosition(position);
        }

        #endregion

        #region Public Methods

        public void DrawNodeGUI(Rect graphArea, Vector2 panOffset, float zoomLevel)
        {
            m_Offset = panOffset;
            Vector2 windowToGridPosition = m_Node.GetPosition() + m_Offset / zoomLevel;
            var clientRect = new Rect(windowToGridPosition, m_Node.GetSize());
            GUI.Window(m_WindowId, clientRect, DrawNode, string.Empty, nodeArea);
        }

        #endregion

        #region Private Methods

        private void UpdateColors()
        {
            m_NodeGlowColor = UColor.GetColor().nodeGlowColor;
            m_NodeBodyColor = UColor.GetColor().nodeBodyColor;

            m_HeaderTextAndIconColor = UColor.GetColor().nodeHeaderIconColor;
            if (node.GetHasErrors())
            {
                m_HeaderTextAndIconColor = Color.red;
            }

            m_NodeOutlineColor = UColor.GetColor().nodeOutlineColor;
            m_NodeOutlineColor.a = GUI.color.a * (isSelected ? 1 : node.isHovered ? 0.4f : 0);
            m_NodeHeaderAndFooterBackgroundColor = UColor.GetColor().nodeHeaderAndFooterBackgroundColor;
            //NODE SELECETD COLOR
            if (EditorApplication.isPlaying)
            {
                if (node.ping)
                {
                    m_NodeOutlineColor = UColor.GetColor().nodePlayingOutlineColor;
                }
            }
            else if (node.ping)
            {
                node.ping = false;
            }
        }

        private void DrawNode(int id)
        {
            var color = GUI.color;
            UpdateColors();
            OnNodeGUI();
            GUI.color = color;
            dynamicHeight += m_FooterRect.height + 6 + 2;
            UpdateNodeHeight(dynamicHeight);
        }

        #endregion

        #region DrawProp
        
        protected string DrawPropString(string name, string val, float height = 24)
        {
            GUILayout.BeginHorizontal();
            var rect1 = new Rect(10, dynamicHeight, 70, height);
            GUI.Label(rect1, name);
            var rect2 = new Rect(72, dynamicHeight,  m_DrawRect.width - 83, height);
            var res = GUI.TextField(rect2, val, nodeInputText);
            GUILayout.EndHorizontal();
            dynamicHeight += height;
            return res;
        }

        protected void DrawPropEnum<T>(string name, T val, Action<T> onValueChange, float height = 24) where T : Enum
        {
            GUILayout.BeginHorizontal();
            var rect1 = new Rect(10, dynamicHeight, 70, height);
            GUI.Label(rect1, name);
            var rect2 = new Rect(72, dynamicHeight + 2,  m_DrawRect.width - 83, height-4);
            if (GUI.Button(rect2, val.ToString()))
            {
                var menu = new GenericMenu();
                var names = Enum.GetNames(val.GetType());
                var values = Enum.GetValues(val.GetType());
                for (int i = 0; i < names.Length; i++)
                {
                    var value = (T)values.GetValue(i);
                    menu.AddItem(new GUIContent(names[i]), value.Equals(val), () =>
                    {
                        onValueChange?.Invoke(value);
                    });
                }
                menu.ShowAsContext();
            }
            GUILayout.EndHorizontal();
            dynamicHeight += height;
        }
        
        #endregion
    }

    public abstract class NodeView<T> : NodeView where T : NodeBase
    {
        public T node => base.node as T;
    }
}