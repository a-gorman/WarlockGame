//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System.IO;
using NeonShooter.Core.Game.Entity;

namespace NeonShooter.Core.Game;

class PlayerStatus
{
	// amount of time it takes, in seconds, for a multiplier to expire.
	private const float MultiplierExpiryTime = 0.8f;
	private const int MaxMultiplier = 20;

	public int Lives { get; private set; }
	public int Score { get; private set; }
	public int HighScore { get; private set; }
	public int Multiplier { get; private set; }
	
	public Player Player { get; }
	// public bool IsGameOver => Lives == 0;

	private float _multiplierTimeLeft;	// time until the current multiplier expires
	private int _scoreForExtraLife;		// score required to gain an extra life

	private const string HighScoreFilename = "highscore.txt";

	// Static constructor
	public PlayerStatus(Player player)
	{
		Player = player;
		HighScore = LoadHighScore();
		Reset();
	}

	public void Reset()
	{
		if (Score > HighScore)
			SaveHighScore(HighScore = Score);

		Score = 0;
		Multiplier = 1;
		Lives = 4;
		_scoreForExtraLife = 2000;
		_multiplierTimeLeft = 0;
	}

	public void Update()
	{
		if (Multiplier > 1)
		{
			// update the multiplier timer
			if ((_multiplierTimeLeft -= (float)NeonShooterGame.GameTime.ElapsedGameTime.TotalSeconds) <= 0)
			{
				_multiplierTimeLeft = MultiplierExpiryTime;
				ResetMultiplier();
			}
		}
	}

	public void AddPoints(int basePoints)
	{
		if (Player.Warlock.IsDead)
			return;

		Score += basePoints * Multiplier;
		while (Score >= _scoreForExtraLife)
		{
			_scoreForExtraLife += 2000;
			Lives++;
		}
	}

	public void IncreaseMultiplier()
	{
		if (Player.Warlock.IsDead)
			return;

		_multiplierTimeLeft = MultiplierExpiryTime;
		if (Multiplier < MaxMultiplier)
			Multiplier++;
	}

	public void ResetMultiplier()
	{
		Multiplier = 1;
	}

	public void RemoveLife()
	{
		Lives--;
	}

	private static int LoadHighScore() 
	{
		// return the saved high score if possible and return 0 otherwise
		int score;
		return File.Exists(HighScoreFilename) && int.TryParse(File.ReadAllText(HighScoreFilename), out score) ? score : 0;
	}

	private static void SaveHighScore(int score)
	{
		File.WriteAllText(HighScoreFilename, score.ToString());
	}
}
