using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Debug
{
    public class DebugInfo : MonoBehaviour
    {
        public string DebugInfoValue;
        public bool CopyToClipboard;

        void Update()
        {
            if (CopyToClipboard)
            {
                TextEditor te = new TextEditor();
                te.content = new GUIContent(DebugInfoValue);
                te.SelectAll();
                te.Copy();

                CopyToClipboard = false;
            }
        }
    }
}
