//----------------------------------------------
//
//         Copyright © 2016  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class EditableCommentAttribute : PropertyAttribute {

		public Color headerColor
		{
			get;
			private set;
		}

		public Texture2D olRefresh
		{
			get;
			private set;
		}

		public Texture2D olCheckGreen
		{
			get;
			private set;
		}

		public EditableCommentAttribute()
		{
			headerColor = Color.black;
			olRefresh = (Texture2D)Resources.Load("OLRefresh");
			olCheckGreen = (Texture2D)Resources.Load("OLCheckGreen");
		}

		public EditableCommentAttribute(ColorEnum headerColor)
		{
			this.headerColor = ColorEx.GetColorByEnum(headerColor);
			olRefresh = (Texture2D)Resources.Load("OLRefresh");
			olCheckGreen = (Texture2D)Resources.Load("OLCheckGreen");
		}

		public EditableCommentAttribute(float r, float g, float b)
		{
			headerColor = new Color(r, g, b);
			olRefresh = (Texture2D)Resources.Load("OLRefresh");
			olCheckGreen = (Texture2D)Resources.Load("OLCheckGreen");
		}
	}
	
}
	