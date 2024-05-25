using NPSerialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPVisualEditor_Example
{
    public class VisualEditorExampleFunctions : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            CreateCase();
        }

        private void CreateCase()
        {
            m_functionsCase = new FunctionsCase(9999);
            InstanceContext.Instance.RegisterReference(m_functionsCase, m_functionsCase.ID);
        }

        public void TestFunctions()
        {
            m_functionsCase.LoadBehaviorNodes();
            m_functionsCase.StartBehavior();
        }

        private FunctionsCase m_functionsCase;
    }
}