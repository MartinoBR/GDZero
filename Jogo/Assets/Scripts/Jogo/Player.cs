﻿using UnityEngine;
using System.Collections;

public enum Direction 
{
	Up = 1,
	Down = 2,
	Right = 3,
	Left = 4
}

public enum CharClass 
{
	Warrior = 1,
	Barbarian,
	Paladin,
	Thief,
	Archer
}

public enum ItemType 
{
	Bow = 1,
	Shield,
	LargeShield,
	Speedy,
	Beer
}

public class Player : MonoBehaviour 
{
	private Rigidbody2D rigidbody2d;
	private Animator animatorController;
	private int totalKills = 0;
	private bool midAir = false;
	private float percentual = 0f;
	private float eggTime = 0;
	private Egg egg;

	public float MovieSpeed = 5;
	public float JumpSpeed = 7;
	public CharClass playerClass = CharClass.Archer;
	public bool IsRunning = false;
	public bool IsJumping = false;
	public bool IsSliding = false;
	public bool IsAttacking = false;
	public bool WithEgg = false;
	public bool NormalShield = false;
	public bool DoubleShield = false;
	public bool Bebado = false;
	public bool Dyieng = false;
	public bool IsAttackingArrow = false;
	public Direction Direction = Direction.Left;
	public ParticleSystem[] DustParticles;
	public ParticleSystem BubblusParticle;
	public GameObject NormalShieldObject;
	public GameObject DoubleShieldObject;
	public Transform EggPlace;
	public Transform Sight;
	public Arrow ArrowPrefab;
	public HUDBar HUDBar;
	public float LevelTime;
	
	public Sword AttackFront;
	public Sword BottonAttack;
	public Sword TopAttack;
	public AudioClip[] blitAudio;

	float contadorAudio = 0;
	float contadorAudioMorte = 1;

	void Start () 
	{
		rigidbody2d = this.GetComponent<Rigidbody2D>();
		animatorController = this.GetComponent<Animator>();
	}
	
	void Update () 
	{
		GenereteAnimation();
		HUDBar.UpdateBar(playerClass, eggTime/LevelTime);
		if (WithEgg)
			eggTime += Time.deltaTime;

		TopAttack.On = BottonAttack.On = AttackFront.On = false;
		if (IsAttacking) {
			swordTime += Time.deltaTime;
			if (this.Direction == Direction.Up)
				TopAttack.On = true;
			else if (this.Direction == Direction.Down)
				BottonAttack.On = true;
			else
				AttackFront.On = true;
		}
	}

	private void GenereteAnimation()
	{
		contadorAudio += Time.deltaTime;
		if (contadorAudio > 0.5) 
		{
			contadorAudio = 0;
		}
		if (contadorAudioMorte != 1) 
		{
			contadorAudioMorte += Time.deltaTime;
		}
		if (contadorAudioMorte > 1) 
		{
			contadorAudioMorte = 1;
		}
		animatorController.SetInteger("PlayerClass", (int)playerClass); 
		animatorController.SetBool("IsRunning", IsRunning);
		if (IsRunning && contadorAudio == 0) 
		{
			AudioSource.PlayClipAtPoint(blitAudio[0], transform.position);
		}
		animatorController.SetBool("IsJumping", IsJumping);
		animatorController.SetBool("IsSliding", IsSliding);
		animatorController.SetBool("IsAttacking", IsAttacking);
		if (IsAttacking && contadorAudio == 0) 
		{
			AudioSource.PlayClipAtPoint(blitAudio[1], transform.position);
		}
		animatorController.SetBool("IsDyieng", Dyieng);
		if (Dyieng && contadorAudioMorte == 1) 
		{
			AudioSource.PlayClipAtPoint(blitAudio[2], transform.position);
			contadorAudioMorte = 0;
		}
		animatorController.SetBool("IsAttackingArrow", IsAttackingArrow);
		animatorController.SetInteger("Direction", (int)this.Direction);
		if (Bebado) {
			if (!BubblusParticle.isPlaying)
				BubblusParticle.Play();
		}
		else
			if (BubblusParticle.isPlaying)
				BubblusParticle.Stop();
		
		NormalShieldObject.SetActive(NormalShield || DoubleShield);
		DoubleShieldObject.SetActive(DoubleShield);

		//if (IsAttackingArrow)
		//	Invoke("createArrow", 0.5f);
	}

	private void createArrow () 
	{
		Arrow arrow = (Arrow)Instantiate(ArrowPrefab, Sight.position, Sight.rotation);
		arrow.transform.localScale = gameObject.transform.localScale;	
	}

	private float jumpDirection = 0;
	public void Move(float direction, bool jump)
	{
		float velocityX = MovieSpeed;//rigidbody2d.velocity.X;
		//float velocityY = rigidbody2d.velocity.y;	

		if(jump && (!midAir || IsSliding))
		{
			IsJumping = true;
			midAir = true;
			IsSliding = false;
			//velocityY = JumpSpeed;
			rigidbody2d.velocity = new Vector3(rigidbody2d.velocity.x, JumpSpeed, 0.0f);

			jumpDirection = transform.localScale.x;
		}

		IsRunning = (direction != 0);
		//velocityX = MovieSpeed * direction;
		if(direction != 0)
		{
			if (midAir && transform.localScale.x != jumpDirection)
				velocityX /= 2;

			if(direction < 0)
			{
				this.Direction = Direction.Left;
				if (IsSliding)
					transform.localScale = new Vector3(0.6641114f, transform.localScale.y, transform.localScale.z);
				else 
					transform.localScale = new Vector3(-0.6641114f, transform.localScale.y, transform.localScale.z);

			}
			else if(direction > 0)
			{
				this.Direction = Direction.Right;
				if (IsSliding)
					transform.localScale = new Vector3(-0.6641114f, transform.localScale.y, transform.localScale.z);
				else 
					transform.localScale = new Vector3(0.6641114f, transform.localScale.y, transform.localScale.z);
			}


		}
		if (!IsSliding)
			rigidbody2d.velocity = new Vector3(velocityX * direction, rigidbody2d.velocity.y, 0.0f);
	}

	private float swordTime = 0;

	public void Attack(float direction, bool attacking)
	{
		if (attacking) 
		{
			IsAttacking = true;
			swordTime = 0;
		}
		else if (swordTime > 0.5f) 
			IsAttacking = false;

		if (direction > 0)
				this.Direction = Direction.Up;
		else if (direction < 0)
				this.Direction = Direction.Down;

		/*
		if (!animatorController.GetCurrentAnimatorStateInfo(0).IsName("WarriorAttackingFront") && !animatorController.GetCurrentAnimatorStateInfo(0).IsName("WarriorStandAtackArrow")) {
			attacking = false;
			animatorController.SetBool ("attacking", false);
		}
		
		if (attacking)
			swordTime += Time.deltaTime;
		else
			swordTime = 0;

		if (attack && !attacking) {
			attacking = true;
			animatorController.SetBool ("attacking", true);
		}
		
		if(swordTime > 0.1f)
		{

			Ray swordRange = new Ray (transform.position, transform.right);
			RaycastHit swordHit;
			Physics.Raycast (swordRange, out swordHit, 1.2f);
			
			if(swordHit.rigidbody != null){
				swordHit.rigidbody.AddForce(transform.right*10,ForceMode.Impulse);
			}
			
			//swordTime = 0;
			//animatorController.SetBool("attacking",false);
			//Instantiate(shot,transform.position,transform.rotation);
		}
		*/
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{
		switch (collision.collider.tag) 
		{
			case "Solido":
				IsJumping = IsSliding = midAir = false;
				break;

			case "Wall":
				IsRunning = false;
				IsSliding = true;
				break;
				
			case "Fogo":
				Hit();
				break;

			case "Ovo":
				AudioSource.PlayClipAtPoint(blitAudio[3], transform.position);
				GetEgg(collision.gameObject);
				break;

			case "Item":
				AudioSource.PlayClipAtPoint(blitAudio[3], transform.position);
				GetItem(collision.gameObject);
				Destroy(collision.gameObject);
				break;
		}
	}

	void OnCollisionExit2D(Collision2D collision) 
	{
		IsSliding = false;
	}

	public void Hit()
	{
		Debug.Log ("hit");
		rigidbody2d.velocity = new Vector3(rigidbody2d.velocity.x, 3f, 0.0f);
		if (DoubleShield) 
		{
			DoubleShield = false;
			NormalShield = true;
		}
		else if (NormalShield)
			NormalShield = false;
		else if (!Dyieng)
			Die();
	}

	private void Die() 
	{
		Dyieng = true;

		if (WithEgg) 
		{
			WithEgg = false;
			egg.IsFlying = false;
		} 
		else if (eggTime >= 1)
			eggTime--;

	}

	private void GetEgg(GameObject item) 
	{
		WithEgg = true;
		this.egg = item.GetComponent<Egg>();
	}

	private void GetItem(GameObject gameObject) 
	{
		Item item = gameObject.GetComponent<Item>();
		switch (item.Type) 
		{
			case ItemType.Shield:
				NormalShield = true;
				break;

			case ItemType.LargeShield:
				DoubleShield = true;
				break;
		}
	}

	private void PlayJumpingDust()
	{
		foreach(ParticleSystem particle in DustParticles)
			particle.Play();
	}

	
	public void Run(float direction)
	{
		IsRunning = (direction != 0);
		if(direction < 0)
		{
			this.Direction = Direction.Left;
			if (IsSliding)
				transform.localScale = new Vector3(0.6641114f, transform.localScale.y, transform.localScale.z);
			else
				transform.localScale = new Vector3(-0.6641114f, transform.localScale.y, transform.localScale.z);
		}
		else if (direction > 0)
		{
			this.Direction = Direction.Right;
			if (IsSliding)
				transform.localScale = new Vector3(-0.6641114f, transform.localScale.y, transform.localScale.z);
			else
				transform.localScale = new Vector3(0.6641114f, transform.localScale.y, transform.localScale.z);
		}
		
		rigidbody2d.velocity = new Vector3(MovieSpeed * direction, rigidbody2d.velocity.y, 0.0f);
	}
	
	public void Jump()
	{
		if (!midAir) 
		{
			IsJumping = true;
			midAir = true;
			IsSliding = false;
			rigidbody2d.velocity = new Vector3(rigidbody2d.velocity.x, JumpSpeed, 0.0f);
			
			//PlayJumpingDust();
			
			//jumpDirection = transform.localScale.x;
		}
	}

}
