using System;
using System.Runtime.Serialization;

namespace OpenMLTD.MilliSim.Core.Entities {
    [Serializable]
    [DataContract]
    public sealed class PolyPoint {

        /// <summary>
        /// Delta tick compared to the main note. (?)
        /// </summary>
        [DataMember(Name = "subTick")]
        public int Subtick { get; set; }

        /// <summary>
        /// X position of this polypoint.
        /// </summary>
        [DataMember(Name = "positionX")]
        public float PositionX { get; set; }

    }
}
