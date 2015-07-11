using UnityEngine;
using System.Collections;

/// <summary>
/// Expression. A primitive class that contains arousal and pleasure properties
/// </summary>
public class Expression
{

	private int arousal, pleasure;

	public Expression(int pleasure, int arousal)
	{
		this.Pleasure = pleasure;
		this.Arousal = arousal;
	}

	public int Arousal {
		get {
			return this.arousal;
		}
		set {
			arousal = value;
		}
	}

	public int Pleasure {
		get {
			return this.pleasure;
		}
		set {
			pleasure = value;
		}
	}

}
