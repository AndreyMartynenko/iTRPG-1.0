public class BaseStat {
	
	#region =Variables=
	
	public const int StartingExpValue = 100;
	
	private string _name;
	private int _baseValue;
	private int _buffValue;
	private int _expToLevel;
	private float _levelModifier;
	
	#endregion
	
	public BaseStat() {
		_name = "";
		_baseValue = 0;
		_buffValue = 0;
		_levelModifier = 1.5f;
		_expToLevel = StartingExpValue;
	}
	
	#region =Getters & Setters=
	
	public string Name {
		set { _name = value; }
		get { return _name; }
	}
	
	public int BaseValue {
		set { _baseValue = value; }
		get { return _baseValue; }
	}

	public int BuffValue {
		set { _buffValue = value; }
		get { return _buffValue; }
	}

	public int AdjustedBaseValue {
		get { return _baseValue + _buffValue; }
	}
	
	public int ExpToLevel {
		set { _expToLevel = value; }
		get { return _expToLevel; }
	}

	public float LevelModifier {
		set { _levelModifier = value; }
		get { return _levelModifier; }
	}
	
	#endregion
	
	public void LevelUp() {
		_expToLevel = (int)(_expToLevel * _levelModifier);
		_baseValue++;
	}
}
