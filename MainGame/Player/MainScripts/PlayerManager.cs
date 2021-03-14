using Godot;

public class PlayerManager : Node
{
	private RigidBody2D playerRb;
	private Node2D playerRotation;
	private Particles2D flyPartilces;
	private const float maximumForce = 1200, flyLerp = 2f;
	private const float maximumAngle = 50, angleRotateLerp = .09f;
	private float movingForewardSpeed = 200, maximumForewardForce = 400;

	private MapManager buildMangaer;

	public override void _Ready()
	{
		playerRb = GetNode<RigidBody2D>("Player");
		playerRotation = playerRb.GetNode<Node2D>("PlayerRotation");
		flyPartilces = playerRotation.GetNode<Particles2D>("FlyParticles");
		buildMangaer = GetNode<MapManager>("MapManager");
	}
	public override void _Process(float delta)
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


	private void Die()
	{

	}
	private void _on_Area2D_body_entered(object body)
	{
		GD.Print("Touched");
		Die();
	}
}
