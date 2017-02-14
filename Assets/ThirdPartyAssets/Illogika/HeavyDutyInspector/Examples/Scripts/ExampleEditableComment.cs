//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using HeavyDutyInspector;
using Object = UnityEngine.Object;

public class ExampleEditableComment : MonoBehaviour
{
	[EditableComment]
	public string editableComment;
}
