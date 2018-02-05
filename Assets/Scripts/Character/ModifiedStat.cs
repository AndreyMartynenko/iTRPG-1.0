using System.Collections.Generic;

public struct ModifyingAttribute {
	
	public PrimaryAttribute attribute;
	public float ratio;
	
	public ModifyingAttribute(PrimaryAttribute attribute, float ratio) {
		this.attribute = attribute;
		this.ratio = ratio;
	}
}

public class ModifiedStat : BaseStat {
	
	private List<ModifyingAttribute> modAttribute;			//list of attributes that modify that stat
	private int modValue;									//amount added to baseValue
	
	public ModifiedStat() {
		modAttribute = new List<ModifyingAttribute>();
		modValue = 0;
	}
	
	#region =Getters & Setters=
	
	public new int AdjustedBaseValue {
		get { return BaseValue + BuffValue + modValue; }
	}
	
	#endregion
	
	public void Update() { //Not UnityEngine Update
		CalculateModValue();
	}
	
	public void AddModifier(ModifyingAttribute mod) {
		modAttribute.Add(mod);
	}
	
	public void ClearModifiers() {
		modAttribute.Clear();
	}
	
	public void CalculateModValue() {
		modValue = 0;
		
		if(modAttribute.Count > 0)
			foreach(ModifyingAttribute attribute in modAttribute)
				modValue += (int)(attribute.attribute.AdjustedBaseValue * attribute.ratio);
	}
}
