using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowGraph.Model
{

    public abstract class FlowAsset : ScriptableObject
    {
        [SerializeField]
        private List<VariableInfo> inputs;

        public List<VariableInfo> Inputs
        {
            get
            {
                return inputs;
            }
            set { inputs = value; }
        }

        public abstract FlowNode CreateFlowNode();

        public virtual IEnumerable<VariableInfo> GetAllOvrrideInputs()
        {
            if (inputs == null)
                return VariableInfo.EmptyArray;
            return inputs;
        }


    }

}