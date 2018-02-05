public enum VitalName {
	Health,				//ОЗ - HP		zavisit ot urovnia
	PsyEnergy,			//ОП - PP
	Stamina,			//ОВ - SP
	ActionPoints,		//ОД - AP
	Count
}

public class Vital : ModifiedStat {
	
	private int _currentValue;
	
	public Vital() {
		_currentValue = 0;
		//ExpToLevel = StartingExpValue;
		//LevelModifier = 1.1f;
	}
	
	public int CurrentValue {
		get {
			if(_currentValue > AdjustedBaseValue)
				_currentValue = AdjustedBaseValue;
			
			return _currentValue;
		}
		set { _currentValue = value; }
	}
}
