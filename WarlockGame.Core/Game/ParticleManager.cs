﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game;

public class ParticleManager<T>
{
	// This delegate will be called for each particle.
	private readonly Action<Particle> _updateParticle;
	private readonly CircularParticleArray _particleList;

	/// <summary>
	/// Allows creation of particles.
	/// </summary>
	/// <param name="capacity">The maximum number of particles. An array of this size will be pre-allocated.</param>
	/// <param name="updateParticle">A delegate that lets you specify custom behaviour for your particles. Called once per particle, per frame.</param>
	public ParticleManager(int capacity, Action<Particle> updateParticle)
	{
		_updateParticle = updateParticle;
		_particleList = new CircularParticleArray(capacity);

		// Populate the list with empty particle objects, for reuse.
		for (int i = 0; i < capacity; i++)
			_particleList[i] = new Particle();
	}

	/// <summary>
	/// Update particle state, to be called every frame.
	/// </summary>
	public void Update()
	{
		int removalCount = 0;
		for (int i = 0; i < _particleList.Count; i++)
		{
			var particle = _particleList[i];

			_updateParticle(particle);

			particle.PercentLife -= 1f / particle.Duration;

			// sift deleted particles to the end of the list
			Swap(_particleList, i - removalCount, i);

			// if the alpha < 0, delete this particle
			if (particle.PercentLife < 0)
				removalCount++;
		}
		_particleList.Count -= removalCount;
	}

	private static void Swap(CircularParticleArray list, int index1, int index2)
	{
		(list[index1], list[index2]) = (list[index2], list[index1]);
	}

	/// <summary>
	/// Draw the particles.
	/// </summary>
	public void Draw(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < _particleList.Count; i++)
		{
			var particle = _particleList[i];

			Vector2 origin = new Vector2((particle.Texture?.Width ?? 0) / 2, (particle.Texture?.Height ?? 0) / 2);
			if (particle.Texture != null)
			{
				spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Tint, particle.Orientation, origin, particle.Scale, 0, 0);
			}
		}
	}

	public void CreateParticle(Texture2D? texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
	{
		CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
	}

	public void CreateParticle(Texture2D? texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
	{
		Particle particle;
		if (_particleList.Count == _particleList.Capacity)
		{
			// if the list is full, overwrite the oldest particle, and rotate the circular list
			particle = _particleList[0];
			_particleList.Start++;
		}
		else
		{
			particle = _particleList[_particleList.Count];
			_particleList.Count++;
		}

		// Create the particle
		particle.Texture = texture;
		particle.Position = position;
		particle.Tint = tint;

		particle.Duration = duration;
		particle.PercentLife = 1f;
		particle.Scale = scale;
		particle.Orientation = theta;
		particle.State = state;
	}

	/// <summary>
	/// Destroys all particles
	/// </summary>
	public void Clear()
	{
		_particleList.Count = 0;
	}

	public int ParticleCount => _particleList.Count;

	public class Particle
	{
		public Texture2D? Texture;
		public Vector2 Position;
		public float Orientation;

		public Vector2 Scale = Vector2.One;

		public Color Tint;
		public float Duration;
		public float PercentLife = 1f;
		public T State = default!;
	}

	// Represents a circular array with an arbitrary starting point. It's useful for efficiently overwriting
	// the oldest particles when the array gets full. Simply overwrite particleList[0] and advance Start.
	private class CircularParticleArray
	{
		private int _start;
		public int Start
		{
			get => _start;
			set => _start = value % _list.Length;
		}

		public int Count { get; set; }
		public int Capacity => _list.Length;
		private readonly Particle[] _list = null!;

		public CircularParticleArray() { }  // for serialization

		public CircularParticleArray(int capacity)
		{
			_list = new Particle[capacity];
		}

		public Particle this[int i]
		{
			get => _list[(_start + i) % _list.Length];
			set => _list[(_start + i) % _list.Length] = value;
		}
	}
}