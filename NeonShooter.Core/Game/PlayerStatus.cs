//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System.IO;

namespace NeonShooter
{
	internal static class PlayerStatus
	{
		// amount of time it takes, in seconds, for a multiplier to expire.
		private const float MultiplierExpiryTime = 0.8f;
		private const int MaxMultiplier = 20;

		public static int Lives { get; private set; }
		public static int Score { get; private set; }
		public static int HighScore { get; private set; }
		public static int Multiplier { get; private set; }
		public static bool IsGameOver { get { return Lives == 0; } }

		private static float _multiplierTimeLeft;	// time until the current multiplier expires
		private static int _scoreForExtraLife;		// score required to gain an extra life

		private const string HighScoreFilename = "highscore.txt";

		// Static constructor
		static PlayerStatus()
		{
			HighScore = LoadHighScore();
			Reset();
		}

		public static void Reset()
		{
			if (Score > HighScore)
				SaveHighScore(HighScore = Score);

			Score = 0;
			Multiplier = 1;
			Lives = 4;
			_scoreForExtraLife = 2000;
			_multiplierTimeLeft = 0;
		}

		public static void Update()
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

		public static void AddPoints(int basePoints)
		{
			if (PlayerShip.Instance.IsDead)
				return;

			Score += basePoints * Multiplier;
			while (Score >= _scoreForExtraLife)
			{
				_scoreForExtraLife += 2000;
				Lives++;
			}
		}

		public static void IncreaseMultiplier()
		{
			if (PlayerShip.Instance.IsDead)
				return;

			_multiplierTimeLeft = MultiplierExpiryTime;
			if (Multiplier < MaxMultiplier)
				Multiplier++;
		}

		public static void ResetMultiplier()
		{
			Multiplier = 1;
		}

		public static void RemoveLife()
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
}