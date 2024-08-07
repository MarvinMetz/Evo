using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SharpDX;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Evo.Mono.Classes;

public class Creature : Entity
{
    private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public Intentions Intention { get; set; } = Intentions.None;
    public float VisualRange { get; set; }
    public float VisualAngel { get; set; }
    public float TurnSpeed { get; set; }
    public float MoveSpeed { get; set; }

    private Directions _turnDirection = Directions.None;

    public Vector2 _wanderTarget;

    private Random _random;

    public Degrees Direction { get; set; }

    private int _timeToWait;

    private int _randomSeed;

    private float _targetDistance;


    public World World { get; set; }

    public Creature(int id, World world) : this(id, world, Guid.NewGuid().GetHashCode())
    {
    }

    public Creature(int id, World world, int randomSeed)
    {
        _random = new Random(randomSeed);
        _randomSeed = randomSeed;
        Size = 16;
        Position = new Vector2(_random.Next(0 + Size / 2, world.Size - Size / 2),
            _random.Next(0 + Size / 2, world.Size - Size / 2));
        TurnSpeed = 5f;
        MoveSpeed = 1f;
        VisualRange = 100;
        VisualAngel = 130;
        World = world;
        Direction = GetRandomDirection(0, 360);
    }

    /*public override string ToString()
    {
        const string separator = " ";
        var sb = new StringBuilder();
        sb.Append($"Id: {Id}").Append(separator);
        sb.Append($"Position:{Position}").Append(separator);
        sb.Append($"Velocity:{Velocity}").Append(separator);
        sb.Append($"Direction:{Direction}").Append(separator);
        sb.Append($"Intention:{Intention}").Append(separator);
        sb.Append($"VisualRange:{VisualRange}").Append(separator);
        sb.Append($"VisualRange:{VisualRange}").Append(separator);

        return sb.ToString();
    }*/

    public void Update(GameTime gameTime)
    {
        if (IsIntentionFulfilled())
            FindNewIntention();
        ExecuteIntention();
    }

    private bool IsIntentionFulfilled()
    {
        if (Intention == Intentions.Wander)
            return ReachedTarget(_wanderTarget, MoveSpeed * 5);
        if (Intention == Intentions.Wait)
            return _timeToWait <= 0;
        if (Intention == Intentions.Turn)
            return !IsFacingBounds(Position, Direction);
        return Intention == Intentions.None;
    }

    private void FindNewIntention()
    {
        if (Intention == Intentions.None)
        {
            Intention = Intentions.Wander;
            FindWanderTarget();
        }
        else if (Intention == Intentions.Turn)
        {
            Intention = Intentions.Wander;
            FindWanderTarget();
        }
        else if (Intention == Intentions.Wander)
        {
            Intention = Intentions.Wait;
            _timeToWait = _random.Next(0, 120);
        }
        else if (Intention == Intentions.Wait)
        {
            Intention = Intentions.None;
        }
    }

    private void ExecuteIntention()
    {
        if (Intention == Intentions.None)
        {
            return;
        }
        else if (Intention == Intentions.Wait)
        {
            _timeToWait--;
        }
        else if (Intention == Intentions.Wander)
        {
            MoveToTarget(_wanderTarget, MoveSpeed);
        }
        else if (Intention == Intentions.Turn)
        {
            var oldDirection = Direction;
            var offset = _turnDirection == Directions.Left ? -(VisualAngel / 2) : +(VisualAngel / 2);
            var targetDirection = Direction + offset;
            var target = targetDirection.ToVector2() + Position;
            TurnToTarget(target);
            if (Math.Abs(oldDirection - Direction) <= 0.0001f)
                log.WarnFormat(
                    "Creature {0} did not turn as expected. Direction: {1}, Offset: {2}, Target: {3}",
                    Guid, Direction, offset, targetDirection);
        }
    }

    private void FindWanderTarget()
    {
        var degrees = GetRandomDirection(Direction, VisualAngel / 2, VisualAngel / 2);
        var isFacingBounds =
            IsFacingBounds(Position, degrees, Direction, Direction + VisualAngel / 2, Direction - VisualAngel / 2);
        var tries = 0;
        var foundNewDirection = false;
        while (!isFacingBounds && tries < 10 && !foundNewDirection)
        {
            degrees = GetRandomDirection(Direction, VisualAngel / 2, VisualAngel / 2);
            foundNewDirection = !IsFacingBounds(Position, degrees);
            tries++;
        }

        if (isFacingBounds || !foundNewDirection)
        {
            Intention = Intentions.Turn;
            _turnDirection = _random.Next(1, 3) == 1 ? Directions.Left : Directions.Right;
            return;
        }

        var distance = _random.NextFloat(VisualRange / 2, VisualRange);
        _wanderTarget = degrees.ToVector2() * distance + Position;
        _targetDistance = Vector2.Distance(Position, _wanderTarget);
    }

    private bool IsFacingBounds(Vector2 currentPosition, params Degrees[] directions)
    {
        return !Array.Exists(directions,
            d => World.IsPositionOnWorld(Size / 2, (d.ToVector2() * VisualRange) + currentPosition));
    }


    private void MoveToTarget(Vector2 target, float speed)
    {
        var oldPosition = Position;
        var oldDirection = Direction;
        TurnToTarget(target);
        var turnOnly = false;

        var potentialNewPosition = Position + Direction.ToVector2() * speed;
        if (Vector2.Distance(oldPosition, target) < Vector2.Distance(potentialNewPosition, target))
        {
            turnOnly = true;
        }
        else
        {
            Position = potentialNewPosition;
        }


        if (Vector2.Distance(Position, target) - _targetDistance >= 50)
        {
            log.WarnFormat(
                "Creature {0} moved away from target. Target: {1}, old Position: {2}, new Position: {3}, target Direction: {4}, old Direction: {5}, new Direction: {6}, target Distance: {7}, new Distance: {8}, Turn only: {9}",
                Guid, target, oldPosition, Position, "x", oldDirection, Direction, _targetDistance,
                Vector2.Distance(Position, target), turnOnly);
            Debug = true;
        }
    }

    private void TurnToTarget(Vector2 target)
    {
        var oldDirection = Direction;

        var targetVector = target - Position;
        Degrees targetDirection = MathHelper.ToDegrees((float)Math.Atan2(targetVector.Y, targetVector.X));
        var targetDirectionMovement = (targetDirection - Direction);
        Degrees targetDirectionMovementAfterTurnSpeed = Math.Clamp(targetDirectionMovement, -TurnSpeed, TurnSpeed);

        Direction += targetDirectionMovementAfterTurnSpeed;

        if(Debug)log.DebugFormat(
            "old Direction: {0}, target Direction: {1}, Direction Movement: {2}, Direction Movement Cap: {3}, new Direction: {4}",
            oldDirection, targetDirection, targetDirectionMovement, targetDirectionMovementAfterTurnSpeed, Direction);
    }

    private bool ReachedTarget(Vector2 target, float threshold)
    {
        var distance = Vector2.Distance(Position, target);
        //log.DebugFormat("Creature {0} distance to target: {1}", Guid, distance);
        return distance < threshold;
    }

    private Degrees GetRandomDirection(float min, float max)
    {
        var degree = _random.NextFloat(min, max);
        return degree;
    }

    private Degrees GetRandomDirection(Degrees currentDirection, float min, float max)
    {
        var minDegrees = ((float)currentDirection - min);
        var maxDegrees = ((float)currentDirection + max);
        var ranDegrees = GetRandomDirection(minDegrees, maxDegrees);
        return ranDegrees;
    }
}