using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IActionManager {
    void Throw(Queue<GameObject> diskQueue);
    int getDiskNumber();
    void setDiskNumber(int num);
}
