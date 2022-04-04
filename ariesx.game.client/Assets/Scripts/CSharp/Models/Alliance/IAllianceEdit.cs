using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Poukoute {
    public interface IAllianceEdit : IEventSystemHandler {
        int AllianceEmblem { get; set; }
    }
}
