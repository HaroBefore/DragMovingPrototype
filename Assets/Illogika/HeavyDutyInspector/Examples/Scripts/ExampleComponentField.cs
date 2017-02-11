//----------------------------------------------
//
//      Copyright © 2013 - 2015  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections;
using HeavyDutyInspector;

public class ExampleComponentField : MonoBehaviour {

	[ComponentFieldRestriction(typeof(Texture), typeof(MeshRenderer))]
	public ComponentField textureToLocalize;
}
