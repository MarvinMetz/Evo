using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SharpDX;
using BoundingSphere = Microsoft.Xna.Framework.BoundingSphere;
using Ray = Microsoft.Xna.Framework.Ray;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Evo.Mono.Classes;

public class Creature : Entity
{
    private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public Intentions Intention { get; set; } = Intentions.None;
    public float VisualRange { get; set; }
    public float VisualAngel { get; set; }
    public float MaxMoveSpeed { get; set; }
    public float MoveSpeedAcceleration { get; set; }
    private float _currentMoveSpeed;
    public float MaxTurnSpeed { get; set; }
    public float TurnSpeedAcceleration { get; set; }
    private float _currentTurnSpeed;

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
        MaxMoveSpeed = 1f;
        MoveSpeedAcceleration = 100f / 60f / 100f;
        MaxTurnSpeed = (float)(360 / (2.0 * (Size / 2.0) * Math.PI) * MaxMoveSpeed);
        TurnSpeedAcceleration = MoveSpeedAcceleration;
        VisualAngel = 130;
        VisualRange = 100;
        World = world;
        Direction = GetRandomDirection(0, 360);
        _currentMoveSpeed = 0;
        _currentTurnSpeed = 0;
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
        {
            if (ReachedTarget(_wanderTarget, MaxMoveSpeed * 2))
            {
                _currentMoveSpeed = 0;
                _currentTurnSpeed = 0;
                return true;
            }

            return false;
        }

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
            MoveToTarget(_wanderTarget);
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
            d => World.IsPositionOnWorld(Size, (d.ToVector2() * VisualRange) + currentPosition));
    }


    private void MoveToTarget(Vector2 target)
    {
        var oldPosition = Position;
        var oldDirection = Direction;
        TurnToTarget(target);
        var turnOnly = false;

        var newSpeed = _currentMoveSpeed;

        if (Vector2.Distance(oldPosition, target) <= GetStoppingDistance())
        {
            newSpeed = Math.Max(newSpeed - (MaxMoveSpeed * MoveSpeedAcceleration), 0);
        }
        else
        {
            newSpeed = Math.Min(newSpeed + (MaxMoveSpeed * MoveSpeedAcceleration), MaxMoveSpeed);
        }


        var potentialNewPosition = Position + Direction.ToVector2() * newSpeed;
        if (Vector2.Distance(oldPosition, target) < Vector2.Distance(potentialNewPosition, target))
        {
            turnOnly = true;
        }
        else
        {
            Position = potentialNewPosition;
            _currentMoveSpeed = newSpeed;
            //log.DebugFormat($"Speed: {newSpeed}, Stopping Distance: {GetStoppingDistance()}");
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

    private float GetStoppingDistance()
    {
        var stoppingDistance = (_currentMoveSpeed * _currentMoveSpeed) / (2 * MoveSpeedAcceleration * MaxMoveSpeed);

        return stoppingDistance;
    }

    private float GetStoppingDegree()
    {
        var stoppingDegree = (_currentTurnSpeed * _currentTurnSpeed) / (2 * TurnSpeedAcceleration * MaxTurnSpeed);

        return stoppingDegree;
    }

    private void TurnToTarget(Vector2 target)
    {
        var oldDirection = Direction;

        var targetVector = target - Position;
        Degrees targetDirection = MathHelper.ToDegrees((float)Math.Atan2(targetVector.Y, targetVector.X));
        var targetDirectionMovement = (targetDirection - Direction);

        if (Math.Abs(targetDirectionMovement) < 0.01f)
        {
            _currentTurnSpeed = 0;
            return;
        }

        float newTurnSpeed = _currentTurnSpeed;
        float stopDegree = GetStoppingDegree();
        if (Math.Abs(targetDirectionMovement) <= GetStoppingDegree())
        {
            newTurnSpeed = Math.Max(newTurnSpeed - (MaxTurnSpeed * TurnSpeedAcceleration), 0);
        }
        else
        {
            newTurnSpeed = Math.Min(newTurnSpeed + (MaxTurnSpeed * TurnSpeedAcceleration), MaxTurnSpeed);
        }

        Degrees targetDirectionMovementAfterTurnSpeed =
            Math.Clamp(targetDirectionMovement, -newTurnSpeed, newTurnSpeed);
        _currentTurnSpeed = newTurnSpeed;
        Direction += targetDirectionMovementAfterTurnSpeed;
        //log.DebugFormat("Stop Degree: {0}, Speed: {1}, Degrees left: {2}", stopDegree, newTurnSpeed, targetDirectionMovement);
        if (Debug)
            log.DebugFormat(
                "old Direction: {0}, target Direction: {1}, Direction Movement: {2}, Direction Movement Cap: {3}, new Direction: {4}",
                oldDirection, targetDirection, targetDirectionMovement, targetDirectionMovementAfterTurnSpeed,
                Direction);
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