using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;

namespace vc2opcua
{
    class VcComponent
    {
        [Import]
        public ISimComponent component = null;

        VcUtils _vcutils = new VcUtils();

        public VcComponent(ISimComponent inputcomponent)
        {
            component = inputcomponent;
        }

        #region Methods

        /// <summary>
        /// Returns signals from a component
        /// </summary>
        public List<ISignal> GetSignals()
        {
            List<ISignal> signals = new List<ISignal>();

            var behaviors = component.Behaviors;

            foreach (IBehavior behavior in behaviors)
            {
                var signalType = behavior.Type;

                if (signalType == BehaviorType.StringSignal ||
                    signalType == BehaviorType.BooleanSignal ||
                    signalType == BehaviorType.IntegerSignal ||
                    signalType == BehaviorType.RealSignal ||
                    signalType == BehaviorType.ComponentSignal)
                {
                    signals.Add((ISignal)behavior);
                }
            }
            return signals;
        }

        #endregion
    }
}
