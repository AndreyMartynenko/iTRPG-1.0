public enum PrimaryAttributeName {
	Strength,		//сила				грузоподъемность, холодное, метательное (дальность броска) и тежелое оружие, ОД, одежда
	Agility,		//ловкость			меткость, ОД, уклонение, холодное, лёгкое и средние оружие, сопротивление выбиванию слотов
	Intuition,		//интуиция			попадание, уклонение, меткость, средние и энергетическое оружие, дальность оружия и пси, обнаружение ловушек, обезвреживания ловушек
	Cunning,		//хитрость			установка ловушек, обнаружение ловушек, торговля, разговоры
	Intellect,		//интеллект			урон от пси, скорость обучения всех навыков, оборудование, медицина, одежда пси
	Willpower,		//сила воли			ОП, ОД, скорость восстановления ОП, сопротивление пси урону
	Endurance,		//выносливость		скорость растраты ОВ, ОД, скорость восстановления ОЗ, сопротивление физ уону
	Luck,			//удача				крит попадание, крит уклонение, лучшие находки, обнаружение тайников, сопротивление выбиванию слотов
	Count
}

public class PrimaryAttribute : BaseStat {
	
	//new public const int StartingExpValue = 75;
	
	public PrimaryAttribute() {
		//ExpToLevel = StartingExpValue;
		//LevelModifier = 1.1f;
	}
}
