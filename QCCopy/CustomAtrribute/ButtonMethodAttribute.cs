using System;
using UnityEngine;

namespace lLCroweTool
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonMethodAttribute : PropertyAttribute
    {
        public readonly ButtonMethodDrawOrder DrawOrder;

        public ButtonMethodAttribute(ButtonMethodDrawOrder drawOrder = ButtonMethodDrawOrder.AfterInspector)
        {
            DrawOrder = drawOrder;
        }

    }
    public enum ButtonMethodDrawOrder
    {
        BeforeInspector,
        AfterInspector
    }
}