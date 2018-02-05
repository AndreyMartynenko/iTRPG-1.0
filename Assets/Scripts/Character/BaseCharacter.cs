using UnityEngine;
using System.Collections;

public class BaseCharacter : MonoBehaviour {
	
	#region =Variables=
	
	private string _name;
	private int _level;
	private uint _freeExp;
	private int _remainingAP;		//ToDo: ver como obtener
	private int _hitRange;			//ToDo: ver como obtener
	
	private PrimaryAttribute[] _primaryAttributes;
	private Skill[] _skills;
	private Vital[] _vitals;
	
	private bool _inCombat;
	
	#endregion
	
	public virtual void Awake() {
		Debug.Log("BaseCharacter: Awake");
		
		_name = "";
		_level = 0;
		_freeExp = 0;
		
		_primaryAttributes = new PrimaryAttribute[(int)PrimaryAttributeName.Count];
		_vitals = new Vital[(int)VitalName.Count];
		_skills = new Skill[(int)SkillName.Count];
		
		SetupPrimaryAttributes();
		SetupVitals();
		SetupSkills();
		
		_inCombat = false;
	}
	
	void Start () {
		
	}
	
	void Update () {
		
	}

	#region =Getters & Setters=
	
	public string Name {
		set { _name = value; }
		get { return _name; }
	}

	public int Level {
		set { _level = value; }
		get { return _level; }
	}

	public uint FreeExp {
		set { _freeExp = value; }
		get { return _freeExp; }
	}
	
	public int RemainingAP {
		get { return _remainingAP; }
		set { _remainingAP = value; }
	}
	
	public int HitRange {
		get { return _hitRange; }
		set { _hitRange = value; }
	}
	
	/// <summary>
	/// Returns all attributes that character has.
	/// </summary>
	public PrimaryAttribute[] PrimaryAttributes {
		get { return _primaryAttributes; }
		set {
			for(int cnt = 0; cnt < value.Length; cnt++)
				_primaryAttributes[cnt] = value[cnt];
		}
	}
	
	/// <summary>
	/// Returns all skills that character has.
	/// </summary>
	public Skill[] Skills {
		get {
			ClearSkillModifiers();
			
			return _skills;
		}
		set {
			for(int cnt = 0; cnt < value.Length; cnt++)
				_skills[cnt] = value[cnt];
		}
	}
	
	/// <summary>
	/// Returns all vitals that character has.
	/// </summary>
	public Vital[] Vitals {
		get {
			ClearVitalModifiers();
			
			return _vitals;
		}
		set {
			for(int cnt = 0; cnt < value.Length; cnt++)
				_vitals[cnt] = value[cnt];
		}
	}
	
	public bool InCombat {
		get { return _inCombat; }
		set { _inCombat = value; }
	}
	
	#endregion
	
	public void AddExp(uint exp) {
		_freeExp += exp;
		
		CalculateLevel();
	}
	
	/// <summary>
	/// Take avg of all player skills and assign that as the player level.
	/// </summary>
	public void CalculateLevel() {
		
	}
	
	#region =Setup Attributes, Skills & Vitals=
	
	private void SetupPrimaryAttributes() {
		for(int cnt = 0; cnt < _primaryAttributes.Length; cnt++) {
			_primaryAttributes[cnt] = new PrimaryAttribute();
			_primaryAttributes[cnt].Name = ((PrimaryAttributeName)cnt).ToString(); //ToDo: revisar porq solo aca se utiliza
		}
	}
	
	private void SetupSkills() {
		for(int cnt = 0; cnt < _skills.Length; cnt++)
			_skills[cnt] = new Skill();
	}
	
	private void SetupVitals() {
		for(int cnt = 0; cnt < _vitals.Length; cnt++)
			_vitals[cnt] = new Vital();
	}
	
	#endregion
	
	#region =Calculate Skills & Vitals based on modifiers=
	
	public void UpdateStats() {
		for(int cnt = 0; cnt < _skills.Length; cnt++)
			_skills[cnt].Update();
		
		for(int cnt = 0; cnt < _vitals.Length; cnt++)
			_vitals[cnt].Update();
	}
	
	public void ClearSkillModifiers() {
		for(int cnt = 0; cnt < _skills.Length; cnt++)
			_skills[cnt].ClearModifiers();
		
		SetupSkillModifiers();
	}
	
	public void ClearVitalModifiers() {
		for(int cnt = 0; cnt < _vitals.Length; cnt++)
			_vitals[cnt].ClearModifiers();

		SetupVitalModifiers();
	}
	
	public void ClearModifiers() {
		ClearSkillModifiers();
		ClearVitalModifiers();
	}
	
	private void SetupSkillModifiers() {
		/*//MeleeOffence
		_skills[(int)SkillName.MeleeOffence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Strength], 0.33f));
		_skills[(int)SkillName.MeleeOffence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Dexterity], 0.33f));

		//MeleeDefence
		_skills[(int)SkillName.MeleeDefence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Strength], 0.33f));
		_skills[(int)SkillName.MeleeDefence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Dexterity], 0.33f));

		//RangedOffence
		_skills[(int)SkillName.RangedOffence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Dexterity], 0.33f));
		_skills[(int)SkillName.RangedOffence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Intuition], 0.33f));

		//RangedDefence
		_skills[(int)SkillName.RangedDefence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Dexterity], 0.33f));
		_skills[(int)SkillName.RangedDefence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Cunning], 0.33f));

		//PsyOffence
		_skills[(int)SkillName.PsyOffence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Intellect], 0.33f));
		_skills[(int)SkillName.PsyOffence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Willpower], 0.33f));

		//PsyDefence
		_skills[(int)SkillName.PsyDefence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Intellect], 0.33f));
		_skills[(int)SkillName.PsyDefence].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)AttributeName.Willpower], 0.33f));*/
	}
	
	private void SetupVitalModifiers() {
		//Health
		_vitals[(int)VitalName.Health].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)PrimaryAttributeName.Endurance], 0.5f));
		
		//Psy Energy
		_vitals[(int)VitalName.PsyEnergy].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)PrimaryAttributeName.Willpower], 1));
		
		//Stamina
		_vitals[(int)VitalName.Stamina].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)PrimaryAttributeName.Endurance], 1));

		//Action Points
		_vitals[(int)VitalName.Health].AddModifier(new ModifyingAttribute(_primaryAttributes[(int)PrimaryAttributeName.Endurance], 0.5f));
	}
	
	#endregion
}
