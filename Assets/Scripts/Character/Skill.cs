public enum SkillName {
	ColdWeapon,
	LightWeapon,
	MediumWeapon,
	HeavyWeapon,
	EnergyWeapon,
	ThrownWeapon,
	RemoveTrap,			//обезвреживание ловушек		ловкость, интеллект
	Count
}

public class Skill : ModifiedStat {
	
	private bool _known;
	
	public Skill() {
		_known = false;
		//ExpToLevel = StartingExpValue;
		//LevelModifier = 1.1f;
	}
	
	public bool Known {
		set { _known = value; }
		get { return _known; }
	}
}
