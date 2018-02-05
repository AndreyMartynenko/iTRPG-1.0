using UnityEngine;
using System.Collections;
using System;
using Utility;

public static class GameSettings {
	
	#region =Variables=
	
	public enum LevelName {
		///Main menu
		MainMenu,
		///Character generation screen
		CharacterGeneration,
		///Movement test level
		MovementTest
	}
	
	public const float Version = 0.1f;
	
	//public static Vector3 startingPosition = new Vector3(740, 107, 610);
	
	#region =Resource paths=
	public const string PCMeshPath = "PC/Mesh/";
	public const string WEAPON_MESH_PATH = "Item/Mesh/Weapon/";
	
	public const string MEDIUM_WEAPON_ICON_PATH = "Item/Icon/Weapon/Medium/";
	public const string MEDIUM_WEAPON_MESH_PATH = "Item/Mesh/Weapon/Medium/";

	public const string HEADGEAR_ICON_PATH = "Item/Icon/Armor/Headgear/";
	public const string HEADGEAR_MESH_PATH = "Item/Mesh/Armor/Headgear/";
	#endregion
	
	#endregion
	
	static GameSettings() {
		//PC.Instance.Awake();
	}
	
	public static void SaveVersion() {
		PlayerPrefs.SetFloat("Ver", Version);
	}
	
	public static float LoadVersion() {
		return PlayerPrefs.GetFloat("Ver", 0);
	}
	
	public static void LoadAllData() {
		LoadName();
		LoadAttributes();
		LoadVitals();
		LoadSkills();
	}
	
	public static void SavePlayerPosition(Point position) {
		PlayerPrefs.SetInt("PC position x", PC.Instance.Position.x);
		PlayerPrefs.SetInt("PC position y", PC.Instance.Position.y);
	}
	
	public static Point LoadPlayerPosition() {
		return new Point(PlayerPrefs.GetInt("PC position x", PC.Instance.Position.x), 
		                 PlayerPrefs.GetInt("PC position x", PC.Instance.Position.y));
	}
	
	#region =Name=
	public static void SaveName(string name) {
		PlayerPrefs.SetString("PC name", name);
	}
	
	public static string LoadName() {
		return PlayerPrefs.GetString("PC name", "---");
	}
	#endregion
	
	#region =Primary attributes=
	public static void SaveAttribute(PrimaryAttributeName name, PrimaryAttribute attribute) {
		PlayerPrefs.SetInt("A_" + name + "_baseVal", attribute.BaseValue);
		PlayerPrefs.SetInt("A_" + name + "_exp", attribute.ExpToLevel);
	}
	
	public static void LoadAttribute(PrimaryAttributeName name) {
		PC.Instance.PrimaryAttributes[(int)name].BaseValue = PlayerPrefs.GetInt("A_" + name + "_baseVal", 0);
		PC.Instance.PrimaryAttributes[(int)name].ExpToLevel = PlayerPrefs.GetInt("A_" + name + "_exp", PrimaryAttribute.StartingExpValue);
	}
	
	public static void SaveAttributes(PrimaryAttribute[] attribute) {
		for(int cnt = 0; cnt < attribute.Length; cnt++)
			SaveAttribute((PrimaryAttributeName)cnt, attribute[cnt]);
	}
	
	public static void LoadAttributes() {
		for(int cnt = 0; cnt < (int)PrimaryAttributeName.Count; cnt++)
			LoadAttribute((PrimaryAttributeName)cnt);
	}
	#endregion
	
	#region =Skills=
	public static void SaveSkill(SkillName name, Skill skill) {
		PlayerPrefs.SetInt("S_" + name + "_baseVal", skill.BaseValue);
		PlayerPrefs.SetInt("S_" + name + "_exp", skill.ExpToLevel);
	}
	
	public static void LoadSkill(SkillName name) {
		PC.Instance.Skills[(int)name].BaseValue = PlayerPrefs.GetInt("S_" + name + "_baseVal", 0);
		PC.Instance.Skills[(int)name].ExpToLevel = PlayerPrefs.GetInt("S_" + name + "_exp", Skill.StartingExpValue);
		PC.Instance.Skills[(int)name].Update();
	}
	
	public static void SaveSkills(Skill[] skill) {
		for(int cnt = 0; cnt < skill.Length; cnt++)
			SaveSkill((SkillName)cnt, skill[cnt]);
	}
	
	public static void LoadSkills() {
		for(int cnt = 0; cnt < (int)SkillName.Count; cnt++)
			LoadSkill((SkillName)cnt);
	}
	#endregion
	
	#region =Vitals=
	public static void SaveVital(VitalName name, Vital vital) {
		PlayerPrefs.SetInt("V_" + name + "_baseVal", vital.BaseValue);
		PlayerPrefs.SetInt("V_" + name + "_exp", vital.ExpToLevel);
		PlayerPrefs.SetInt("V_" + name + "_currVal", vital.CurrentValue);
	}
	
	public static void LoadVital(VitalName name) {
		PC.Instance.Vitals[(int)name].BaseValue = PlayerPrefs.GetInt("V_" + name + "_baseVal", 0);
		PC.Instance.Vitals[(int)name].ExpToLevel = PlayerPrefs.GetInt("V_" + name + "_exp", Vital.StartingExpValue);
		PC.Instance.Vitals[(int)name].Update();
		PC.Instance.Vitals[(int)name].CurrentValue = PlayerPrefs.GetInt("V_" + name + "_currVal", 1);
	}
	
	public static void SaveVitals(Vital[] vital) {
		for(int cnt = 0; cnt < vital.Length; cnt++)
			SaveVital((VitalName)cnt, vital[cnt]);
	}
	
	public static void LoadVitals() {
		for(int cnt = 0; cnt < (int)VitalName.Count; cnt++)
			LoadVital((VitalName)cnt);
	}
	#endregion
}
