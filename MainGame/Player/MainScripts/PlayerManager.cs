using Godot;

public class PlayerManager : Node
{
	private string PlayMode = "Alive"; //The mods are: Alive, Dead, and Super
	public void SetPlayMode(string Mode)
	{
		if (Mode == "Alive" || Mode == "Dead" || Mode == "Super")
		{
			PlayMode = Mode;
			return;
		}
		PlayMode = "Alive";
	}

	private RigidBody2D playerRb;
	private Node2D playerRotation;
	private Particles2D flyPartilces;
	private const float maximumForce = 1200, flyLerp = 2f;
	private const float maximumAngle = 50, angleRotateLerp = .09f;
	private float movingForewardSpeed = 200, maximumForewardForce = 400;
	private MapManager buildMangaer;

	private PackedScene DieParticlesPath = ResourceLoader.Load("res://MainGame/Player/Prefabs/DieParticles/DieParticles.tscn") as PackedScene;
	private PackedScene DieUiPath = ResourceLoader.Load("res://MainGame/Player/Prefabs/DieUi/DieText.tscn") as PackedScene;

	public override void _Ready()
	{
		playerRb = GetNode<RigidBody2D>("Player");
		playerRotation = playerRb.GetNode<Node2D>("PlayerRotation");
		flyPartilces = playerRotation.GetNode<Particles2D>("FlyParticles");
		buildMangaer = GetNode<MapManager>("MapManager");
	}
	public override void _Process(float delta)
	{
		switch (PlayMode)
		{
			case "Alive":
				AliveFunctions();
				break;

			case "Dead":
				DieFunctions();
				break;

			case "Super":
				break;
		}
	}
	public void BodyEntered(Node body)
	{
		if (body.Name != "Player")
		{
			Die();
		}
	}

	private void AliveFunctions()
	{
		MoveForeward();
		Fly();
		SpawnBuildingIfNeeded();
		SpawnGroundIfNeeded();
	}
	private void Fly()
	{
		Vector2 currentForce = playerRb.GetAppliedForce();

		EffectForce(Input.IsActionPressed("Fly") ? -1 : 1);
		TurnEffect(Input.IsActionPressed("Fly"));
	}
	private void EffectForce(int Flying)
	{
		playerRb.AppliedForce = new Vector2(playerRb.AppliedForce.x, Mathf.Lerp(playerRb.AppliedForce.y, maximumForce * Flying, flyLerp));
		EffectRotation(Flying);
	}
	private void EffectRotation(int ToRight)
	{
		float nextRoatation = Mathf.Lerp(playerRotation.GlobalRotationDegrees, maximumAngle * ToRight, angleRotateLerp);
		playerRotation.GlobalRotationDegrees = nextRoatation;
	}
	private void TurnEffect(bool turnEffects)
	{
		if (flyPartilces.Emitting != turnEffects)
			flyPartilces.Emitting = turnEffects;
	}
	private void MoveForeward()
	{
		playerRb.AppliedForce = new Vector2(movingForewardSpeed, playerRb.LinearVelocity.y);
		if (playerRb.LinearVelocity.x > maximumForewardForce)
		{
			playerRb.LinearVelocity = new Vector2(maximumForewardForce, playerRb.LinearVelocity.y);
		}
	}

	private void SpawnBuildingIfNeeded()
	{
		if (playerRb.Position.x > buildMangaer.prevoiusStartPoint)
		{
			buildMangaer.InstantiateBuild();
		}
	}
	private void SpawnGroundIfNeeded()
	{
		if (playerRb.Position.x > buildMangaer.groundStartPoint.Position.x - 4750)
		{
			buildMangaer.InstantiateGround();
		}
	}


	private void DieFunctions()
	{
		if(Input.IsActionJustPressed("Fly"))
		{
			GetTree().ReloadCurrentScene();
		}
	}
	private void Die()
	{
		PlayDieEffects();

		DestroyThePlayerObject();

		ResetPlayerVeloctiy();

		SetPlayMode("Dead");
	}
	private void PlayDieEffects()
	{
		Particles2D InstntiateParticles = DieParticlesPath.Instance() as Particles2D;
		AddChild(InstntiateParticles);
		InstntiateParticles.GlobalPosition = playerRb.GlobalPosition;
		InstntiateParticles.OneShot = true; InstntiateParticles.Emitting = true;
	}
	private void DestroyThePlayerObject()
	{
		playerRotation.QueueFree();
	}
	private void ResetPlayerVeloctiy()
	{
		playerRb.LinearVelocity = Vector2.Zero;
		playerRb.AppliedForce = Vector2.Zero;
	}
}
